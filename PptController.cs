#nullable disable
using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using Office = Microsoft.Office.Core;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;


namespace PromptLogic.Controllers
{
    public class PptController : IController, ISlideController, IDisposable
    {
        private PowerPoint.Application _app = null;
        private PowerPoint.Presentation _presentation = null;
        private PowerPoint.SlideShowWindow _slideShowWindow = null;
        private PowerPoint.SlideShowView _slideShowView = null;
        private PowerPoint.SlideShowWindows _slideShowWindows = null;
        private PowerPoint.Presentations _presentations = null;
        private PowerPoint.SlideShowWindow _activeSlideShowWindow = null;

        private readonly object _monitorLock = new object();
        private bool _powerPointRunning = false;
        private bool _startedSlideShow = false;

        private Thread _pptThread;
        private SynchronizationContext _pptContext;
        private readonly SynchronizationContext _uiContext;
        private ManualResetEvent _threadReady = new ManualResetEvent(false);

        private System.Windows.Forms.Timer _monitorTimer;

        bool _isConnected = false;
        public bool IsEnabled => _isConnected;
        public bool SupportsNotes { get; set; } = true;
        string _filePath = null;

        public event EventHandler Disconnected;
        public event EventHandler<SlideChangedEventArgs> SlideChanged;
        public event EventHandler SlideShowBegin;
        public event EventHandler SlideShowEnded;
        public event EventHandler TimingsDetected;
        public event Action Ready;

        private void EnsurePowerPointIsRunning()
        {
            // If we already have an Application object, nothing to do
            if (_app != null)
                return;

            // Try to attach to an existing PowerPoint instance
            try
            {
                _app = (PowerPoint.Application)Interaction.GetObject(null, "PowerPoint.Application");
            }
            catch
            {
                _app = null;
            }

            // If PowerPoint was not running, start a new instance
            if (_app == null)
            {
                _app = new PowerPoint.Application
                {
                    Visible = MsoTriState.msoTrue
                };
                _powerPointRunning = true;
            }
        }
        private void StartSlideShow()
        {
            if (_presentation == null)
                return;

            // Configure slideshow settings
            var settings = _presentation.SlideShowSettings;
            settings.ShowType = PowerPoint.PpSlideShowType.ppShowTypeSpeaker;
            settings.LoopUntilStopped = MsoTriState.msoFalse;

            // Start the slideshow
            var window = settings.Run();

            // Store references for later control
            _slideShowWindow = window;
            _slideShowView = window.View;
            _startedSlideShow = true;
            _slideShowWindows = _app.SlideShowWindows;
        }
        public void Configure(object config)
        {
        }
        public void OpenFile(string path)
        {
            EnsurePowerPointIsRunning();

            // Load the presentation
            _presentations = _app.Presentations;
            _presentation = _presentations.Open(path, WithWindow: MsoTriState.msoFalse);

            // Prepare slideshow
            StartSlideShow();
        }
        public bool PresentationHasTimings()
        {
            return InvokeOnPptThread(() =>
            {
                return PresentationHasTimingsInternal();
            });
        }

        public static PowerPoint.Application GetRunningPowerPoint()
        {
            var clsid = new Guid("91493441-5A91-11CF-8700-00AA0060263B"); // PowerPoint.Application
            try
            {
                NativeMethods.GetActiveObject(ref clsid, IntPtr.Zero, out object obj);
                return (PowerPoint.Application)obj;
            }
            catch
            {
                return null;
            }
        }

        private bool Connect()
        {
            // Try to get the running PowerPoint instance
            try
            {
                _app = GetRunningPowerPoint();
            }
            catch
            {
                MessageBox.Show("PowerPoint is not running. Please open your presentation first.");
                return false;
            }

            // Try to get the active presentation
            try
            {
                _presentation = _app.ActivePresentation;
            }
            catch
            {
                MessageBox.Show("No active presentation found. Please open a PowerPoint file.");
                return false;
            }

            // Hook events and attach to slideshow if already running
            HookSlideShowEvents();

            if (PresentationHasTimingsInternal())
                TimingsDetected?.Invoke(this, EventArgs.Empty);


            return true;
        }
        public void Disconnect()
        {
            InvokeOnPptThread(() =>
            {
                _isConnected = false;

                // --- 0. Stop monitor timer ---
                try { MonitorTimerStop(); } catch { }

                // --- 1. Unhook PowerPoint.Application events ---
                try
                {
                    if (_app != null)
                    {
                        _app.SlideShowNextSlide -= SlideShowNextSlide;
                        _app.SlideShowBegin -= OnSlideShowBegin;
                        _app.SlideShowEnd -= OnSlideShowEnd;
                    }
                }
                catch { }

                // --- 2. Release YOUR COM fields ---
                ReleaseComObject(ref _slideShowView);
                ReleaseComObject(ref _slideShowWindow);
                ReleaseComObject(ref _activeSlideShowWindow);
                ReleaseComObject(ref _presentation);
                ReleaseComObject(ref _slideShowWindows);
                ReleaseComObject(ref _presentations);

                // --- 3. Release PowerPoint.Application CHILD COM OBJECTS ---
                if (_app != null)
                {
                    try
                    {
                        var windows = _app.Windows;
                        ReleaseComObject(ref windows);

                        var slideShowWindows = _app.SlideShowWindows;
                        ReleaseComObject(ref slideShowWindows);

                        var presentations = _app.Presentations;
                        ReleaseComObject(ref presentations);

                        var protectedViewWindows = _app.ProtectedViewWindows;
                        ReleaseComObject(ref protectedViewWindows);

                        var commandBars = _app.CommandBars;
                        ReleaseComObject(ref commandBars);
                    }
                    catch { }
                }

                // --- 4. Quit PowerPoint if we created it ---
                if (_powerPointRunning && _app != null)
                {
                    try {
                        System.GC.Collect();
                        GC.WaitForPendingFinalizers();
                        _app.Quit(); 
                    } catch { }

                    try
                    {
                        ReleaseComObject(ref _app);
                    }
                    catch { }

                    _app = null;
                }

                // --- 5. Notify UI ---
                try { Disconnected?.Invoke(this, EventArgs.Empty); } catch { }
            });
        }

        public PptController()
        {
            _uiContext = SynchronizationContext.Current;

            _pptThread = new Thread(PptThreadStart);
            _pptThread.SetApartmentState(ApartmentState.STA);
            _pptThread.IsBackground = true;
            _pptThread.Start();

            // Wait until the thread signals that it is ready
            _threadReady.WaitOne();
        }
        public string Name => "ppt";

        public int SlideCount
        {
            get
            {
                return InvokeOnPptThread(() =>
                {
                    Slides slides = _presentation.Slides;
                    int slideCount = slides.Count;
                    ReleaseComObject(ref slides);
                    return slideCount;
                });
            }
        }
        public string GetSlideTitle(int index)
        {
            return InvokeOnPptThread(() =>
            {
                string title = "Slide " + index;
                var win = GetWindow();
                var slides = _presentation.Slides;
                var slide = slides[index];
                var shapes = slide.Shapes;
                PowerPoint.Shape titleShape = null;

                try
                {
                    titleShape = shapes.Title;
                    if (titleShape != null)
                        title = titleShape.TextFrame.TextRange.Text;
                }
                catch {}
                
                ReleaseComObject(ref slide);
                ReleaseComObject(ref shapes);
                ReleaseComObject(ref titleShape);
                ReleaseComObject(ref slides);

                return title;
            });
        }
        public string GetNotesForSlide(int index)
        {
            return InvokeOnPptThread(() =>
            {
                try
                {
                    var slides = _presentation.Slides;
                    var slide = slides[index];
                    var notesPage = slide.NotesPage;
                    var shapes = notesPage.Shapes;
                    var placeholders = shapes.Placeholders;
                    var placeholder = placeholders[2];
                    var textFrame = placeholder.TextFrame;
                    var textRange = textFrame.TextRange;

                    string note = textRange.Text;

                    ReleaseComObject(ref textRange);
                    ReleaseComObject(ref textFrame);
                    ReleaseComObject(ref placeholder);
                    ReleaseComObject(ref placeholders);
                    ReleaseComObject(ref shapes);
                    ReleaseComObject(ref notesPage);
                    ReleaseComObject(ref slide);
                    ReleaseComObject(ref slides);

                    return note;
                }
                catch
                {
                    return string.Empty;
                }
            });
        }
        public string GetNotesForCurrentSlide()
        {
            int slideIndex = -1;

            if (_slideShowView != null)
                slideIndex = InvokeOnPptThread(() => 
                {
                    Slide slide = _slideShowView.Slide;
                    int slideIndex = slide.SlideIndex;
                    ReleaseComObject(ref slide);
                    return slideIndex;
                });

            if (slideIndex > 0)
                return GetNotesForSlide(slideIndex);
            else
                return string.Empty;
        }
        public void GoToSlide(int index)
        {
            InvokeOnPptThread(() =>
            {
                _slideShowView.GotoSlide(index);
            });
        }
        public void NextSlide()
        {
            InvokeOnPptThread(() =>
            {
                var win = GetWindow();
                EnsureRunning(win);
                _slideShowView.Next();
            });
        }
        public void EndSlideShow()
        {
            InvokeOnPptThread(() =>
            {
                if (!_startedSlideShow)
                    return;

                _slideShowView.Exit();
            });
        }

        public void MonitorTimerStart()
        {

            lock (_monitorLock)
            {
                _monitorTimer = new System.Windows.Forms.Timer();
                _monitorTimer.Interval = 50; // 50ms
                _monitorTimer.Tick += MonitorTimer_Tick;
                _monitorTimer.Start();
            }

        }
        public void MonitorTimerStop()
        {
            lock (_monitorLock)
            {
                if (_monitorTimer != null)
                {
                    _monitorTimer.Stop();
                    _monitorTimer.Tick -= MonitorTimer_Tick;
                    _monitorTimer.Dispose();
                    _monitorTimer = null;
                }
            }
        }
        private void PptThreadStart()
        {
            _pptContext = new WindowsFormsSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(_pptContext);

            _threadReady.Set();

            System.Windows.Forms.Application.Run(); // message pump for COM
        }
        private void InvokeOnPptThread(Action action)
        {
            _pptContext.Post(_ => action(), null);
        }

        public void Enable()
        {
            // Connect to PowerPoint
            // Schedule COM work onto the PowerPoint thread
            InvokeOnPptThread(() =>
            {
                if (!Connect())
                {
                    _isConnected = false;
                    _uiContext.Post(_ =>
                    {
                        ControllerEvent?.Invoke(this, new ControllerEventArgs
                        {
                            Prefix = "ppt",
                            Type = ControllerEventType.Error,
                            Message = "Could not connect."
                        });
                    }, null);
                    return;
                }

                _isConnected = true;

                // Handle timings
                if (PresentationHasTimingsInternal())
                    _uiContext.Post(_ => TimingsDetected?.Invoke(this, EventArgs.Empty), null);

                Ready?.Invoke();

            });

        }
        private T InvokeOnPptThread<T>(Func<T> func)
        {
            // If we're already on the PPT thread, run directly
            if (SynchronizationContext.Current == _pptContext)
            {
                return func();
            }

            var tcs = new TaskCompletionSource<T>();

            _pptContext.Post(_ =>
            {
                try
                {
                    tcs.SetResult(func());
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }, null);

            return tcs.Task.Result; // safe now, because we won't block the PPT thread
        }

        public Task ExecuteCommandAsync(string command, string[] args) { return Task.CompletedTask; }
        public void Dispose() 
        {
            Disconnect();
        }

        public event EventHandler<ControllerEventArgs> ControllerEvent;
        protected virtual void OnControllerEvent(string prefix, string message, ControllerEventType type)
        {
            var handler = ControllerEvent;
            if (handler != null)
            {
                handler(this, new ControllerEventArgs
                {
                    Prefix = prefix,
                    Message = message,
                    Type = type
                });
            }
        }
        private void SlideShowNextSlide(PowerPoint.SlideShowWindow Wn)
        {
            try
            {
                var slide = _slideShowView.Slide;
                int index = slide.SlideIndex;
                SlideChanged?.Invoke(this, new SlideChangedEventArgs(index));
                ReleaseComObject(ref slide);

            }
            catch
            {
                // Optional: log or ignore
            }

        }

        public void HookSlideShowEvents()
        {
            InvokeOnPptThread(() =>
            {
                // Hook PowerPoint.Application events
                _app.SlideShowNextSlide += SlideShowNextSlide;
                _app.SlideShowBegin += OnSlideShowBegin;
                _app.SlideShowEnd += OnSlideShowEnd;

                // If a slideshow is already running, attach to it
                if (_slideShowWindows.Count > 0)
                {
                    if (_slideShowView == null)
                        _slideShowView = GetWindow()?.View;
                }
                else
                {
                    MessageBox.Show("Start the slideshow (F5) to begin syncing notes.");
                }

                MonitorTimerStart();

                _isConnected = true;
            });
        }
        private void OnSlideShowBegin(PowerPoint.SlideShowWindow Wn)
        {
            // Marshal to UI thread
            _uiContext.Post(_ =>
            {
                SlideShowBegin?.Invoke(this, EventArgs.Empty);
            }, null);

            if (_slideShowView == null)
                _slideShowView = GetWindow()?.View;
        }
        private void OnSlideShowEndInternal()
        {
            SlideShowEnded?.Invoke(this, EventArgs.Empty);

            // Optionally trigger Disconnect()
            Disconnect();
        }
        private void OnSlideShowEnd(PowerPoint.Presentation Pres)
        {
            OnSlideShowEndInternal();
        }


        public void ClearAllTimings()
        {
            InvokeOnPptThread(() =>
            {
                var slides = _presentation.Slides;

                for (int i = 1; i <= slides.Count; i++)
                {
                    PowerPoint.Slide slide = slides[i];
                    var trans = slide.SlideShowTransition;
                    trans.AdvanceOnTime = MsoTriState.msoFalse;
                    trans.AdvanceTime = 0;
                    ReleaseComObject(ref trans);
                    ReleaseComObject(ref slide);
                }

                ReleaseComObject(ref slides);
            });
        }

        public SlideShowState State
        {
            get
            {
                return InvokeOnPptThread(() =>
                {
                    return StateInternal;
                });
            }
        }
        private SlideShowState StateInternal
        {
            get
            {
                PpSlideShowState state = _slideShowView.State;

                SlideShowState slideShowState = SlideShowState.Unknown;

                switch (state)
                {
                    case PowerPoint.PpSlideShowState.ppSlideShowRunning:
                        slideShowState = SlideShowState.Running;
                        break;
                    case PowerPoint.PpSlideShowState.ppSlideShowPaused:
                        slideShowState = SlideShowState.Paused;
                        break;
                    case PowerPoint.PpSlideShowState.ppSlideShowBlackScreen:
                        slideShowState = SlideShowState.BlackScreen;
                        break;
                    case PowerPoint.PpSlideShowState.ppSlideShowWhiteScreen:
                        slideShowState = SlideShowState.WhiteScreen;
                        break;
                    case PowerPoint.PpSlideShowState.ppSlideShowDone:
                        slideShowState = SlideShowState.Done;
                        break;
                }

                return slideShowState;
            }
        }

        private PowerPoint.SlideShowWindow GetWindow()
        {
            if (_activeSlideShowWindow == null)
            {
                if (_slideShowWindows.Count == 0)
                    throw new InvalidOperationException("No slideshow running.");

                _activeSlideShowWindow = _slideShowWindows[1];
            }
            return _activeSlideShowWindow;
        }
        private void EnsureRunning(PowerPoint.SlideShowWindow win)
        {
            if (_slideShowView.State == PowerPoint.PpSlideShowState.ppSlideShowPaused)
            {
                _slideShowView.State = PowerPoint.PpSlideShowState.ppSlideShowRunning;
            }
        }

        public int CurrentSlide { 
            get 
            {
                return InvokeOnPptThread(() =>
                {
                    int pos = _slideShowView.CurrentShowPosition;

                    return pos;
                });
            }
        }
        public bool IsSlideShowRunning 
        {
            get
            {
                return InvokeOnPptThread(() =>
                {
                    try
                    {
                        return _slideShowWindows?.Count > 0 ? true : false;
                    }
                    catch
                    {
                        return false;
                    }
                });
            } 
        }
        public void PreviousSlide() 
        { 
            InvokeOnPptThread(() =>
            {
                var win = GetWindow();
                EnsureRunning(win);
                _slideShowView.Previous();
            }); 
        }
        public string GetNotes(int index) 
        { 
            return GetNotesForSlide(index);
        }
        public void Resume() 
        { 
            InvokeOnPptThread(() =>
            {
                try
                {
                    _slideShowView.State = PowerPoint.PpSlideShowState.ppSlideShowRunning;
                }
                catch { }
            });
        }

        public void Pause() 
        {
            InvokeOnPptThread(() =>
            {
                try
                {
                    _slideShowView.State = PowerPoint.PpSlideShowState.ppSlideShowPaused;
                }
                catch { }
            }); 
        }
        public void RefocusSlideShowWindow() 
        { 
            InvokeOnPptThread(() =>
            {
                try
                {
                    var win = GetWindow();
                    IntPtr hwnd = (IntPtr)win.HWND;

                    NativeMethods.SetForegroundWindow(hwnd);
                    win.Activate();

                    // Step 1: Unpause the slideshow
                    _slideShowView.State = PpSlideShowState.ppSlideShowRunning;
                }
                catch { }
            }); 
        }
        private bool PresentationHasTimingsInternal() 
        {
            bool retval = false;

            var slides = _presentation.Slides;

            for (int i = 1; i <= slides.Count && !retval; i++)
            {
                PowerPoint.Slide slide = slides[i];
                var trans = slide.SlideShowTransition;

                if (trans.AdvanceOnTime == MsoTriState.msoTrue && trans.AdvanceTime > 0)
                    retval = true;

                ReleaseComObject(ref trans);
                ReleaseComObject(ref slide);
            }
            
            ReleaseComObject(ref slides);

            return retval;
        }

        private void MonitorTimer_Tick(object sender, EventArgs e)
        {
            if (_slideShowView == null)
                return;

            // SAFE: enum, not COM
            var state = _slideShowView.State;

            if (state == PowerPoint.PpSlideShowState.ppSlideShowDone ||
                _slideShowWindows == null || _slideShowWindows.Count == 0)
            {
                OnSlideShowEndInternal();
                return;
            }

            PowerPoint.Slide tempSlide = null;

            try
            {
                // COM getter isolated so any exception still releases the RCW
                tempSlide = _slideShowView.Slide;
                int current = tempSlide.SlideIndex;
            }
            catch
            {
                // ignore COM timing issues
            }
            finally
            {
                ReleaseComObjectNoLog(ref tempSlide);
            }
        }
        private void ReleaseComObjectNoLog<T>(ref T obj) where T : class
        {
            try
            {
                if (obj != null && Marshal.IsComObject(obj))
                    Marshal.ReleaseComObject(obj);
            }
            catch
            {
                // Ignore — COM cleanup must never throw
            }
            finally
            {
                obj = null;
            }
        }

        private void ReleaseComObject<T>(ref T obj) where T : class
        {
            if (obj == null)
                return;

            try
            {
                int count = Marshal.ReleaseComObject(obj);
                Debug.WriteLine($"[COM RELEASE] {typeof(T).Name} -> remaining RCW refs: {count}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[COM RELEASE ERROR] {typeof(T).Name}: {ex.Message}");
            }
            finally
            {
                obj = null;
            }
        }

    }
}
