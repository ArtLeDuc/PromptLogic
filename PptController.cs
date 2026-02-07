#nullable disable
using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Office = Microsoft.Office.Core;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using Microsoft.VisualBasic;
using System.Runtime.InteropServices;


namespace PromptLogic.Controllers
{
    public class PptController : IController, ISlideController, IDisposable
    {
        private PowerPoint.Application _app = null;
        private PowerPoint.Presentation _presentation = null;
        private PowerPoint.SlideShowWindow _slideShowWindow = null;
        private PowerPoint.SlideShowView _slideShowView = null;
        private readonly object _monitorLock = new object();

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
                _app = GetRunningPowerPoint();// (PowerPoint.Application)Interaction.GetObject(null, "PowerPoint.Application");
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
                try
                {
                    MonitorTimerStop();
                }
                catch { }

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
                catch
                {
                    // Never throw during disconnect
                }

                // --- 2. Release COM objects (future-proof) ---
                ReleaseComObject(ref _slideShowWindow);
                ReleaseComObject(ref _slideShowView);
                ReleaseComObject(ref _presentation);

                // DO NOT release _app or call Quit() because we did NOT create PowerPoint.

                // --- 3. Clear service reference ---
                //            _service = null;

                // --- 4. Notify UI ---
                try
                {
                    Disconnected?.Invoke(this, EventArgs.Empty);
                }
                catch
                {
                    // UI errors should never break disconnect
                }
            });
        }

        private Thread _pptThread;
        private SynchronizationContext _pptContext;
        private readonly SynchronizationContext _uiContext;
        private ManualResetEvent _threadReady = new ManualResetEvent(false);

        private System.Windows.Forms.Timer _monitorTimer;

        bool _isConnected = false;
        public bool IsEnabled => _isConnected;

        public event EventHandler Disconnected;
        public event EventHandler<SlideChangedEventArgs> SlideChanged;
        public event EventHandler SlideShowBegin;
        public event EventHandler SlideShowEnded;
        public event EventHandler TimingsDetected;
        public event Action Ready;

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
                    return _app.ActivePresentation.Slides.Count;
                });
            }
        }
        public string GetSlideTitle(int index)
        {
            return InvokeOnPptThread(() =>
            {
                var win = GetWindow();
                var slide = win.Presentation.Slides[index];

                try
                {
                    var titleShape = slide.Shapes.Title;
                    if (titleShape != null)
                        return titleShape.TextFrame.TextRange.Text;
                }
                catch { }

                return "Slide " + index;
            });
        }
        public string GetNotesForSlide(int index)
        {
            return InvokeOnPptThread(() =>
            {
                try
                {
                    var slide = _presentation.Slides[index];
                    string note = slide.NotesPage.Shapes.Placeholders[2].TextFrame.TextRange.Text;
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
                slideIndex = InvokeOnPptThread(() => { return _slideShowView.Slide.SlideIndex; });

            if (slideIndex > 0)
                return GetNotesForSlide(slideIndex);
            else
                return string.Empty;
        }
        public void GoToSlide(int index)
        {
            InvokeOnPptThread(() =>
            {
                var win = GetWindow();
                win.View.GotoSlide(index);
            });
        }
        public void NextSlide()
        {
            InvokeOnPptThread(() =>
            {
                var win = GetWindow();
                EnsureRunning(win);
                win.View.Next();
            });
        }
        public void EndSlideShow()
        {
            InvokeOnPptThread(() =>
            {
                var win = GetWindow();
                win.View.Exit();
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
                int index = Wn.View.Slide.SlideIndex;
                SlideChanged?.Invoke(this, new SlideChangedEventArgs(index));
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
                if (_app.SlideShowWindows.Count > 0)
                {
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
                foreach (PowerPoint.Slide slide in _presentation.Slides)
                {
                    var trans = slide.SlideShowTransition;
                    trans.AdvanceOnTime = MsoTriState.msoFalse;
                    trans.AdvanceTime = 0;
                }
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
                var state = GetWindow().View.State;

                switch (state)
                {
                    case PowerPoint.PpSlideShowState.ppSlideShowRunning:
                        return SlideShowState.Running;
                    case PowerPoint.PpSlideShowState.ppSlideShowPaused:
                        return SlideShowState.Paused;
                    case PowerPoint.PpSlideShowState.ppSlideShowBlackScreen:
                        return SlideShowState.BlackScreen;
                    case PowerPoint.PpSlideShowState.ppSlideShowWhiteScreen:
                        return SlideShowState.WhiteScreen;
                    case PowerPoint.PpSlideShowState.ppSlideShowDone:
                        return SlideShowState.Done;
                    default:
                        return SlideShowState.Unknown;
                }
            }
        }

        private PowerPoint.SlideShowWindow GetWindow()
        {
            if (_app.SlideShowWindows.Count == 0)
                throw new InvalidOperationException("No slideshow running.");

            return _app.SlideShowWindows[1];
        }
        private void EnsureRunning(PowerPoint.SlideShowWindow win)
        {
            if (win.View.State == PowerPoint.PpSlideShowState.ppSlideShowPaused)
            {
                win.View.State = PowerPoint.PpSlideShowState.ppSlideShowRunning;
            }
        }

        public int CurrentSlide { 
            get 
            {
                return InvokeOnPptThread(() => GetWindow().View.CurrentShowPosition);
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
                        return _app?.SlideShowWindows.Count > 0 ? true : false;
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
                win.View.Previous();
            }); 
        }
        public string GetNotes(int index) 
        { 
            return InvokeOnPptThread(() =>
            {
                return _presentation.Slides[index].NotesPage.Shapes.Placeholders[2].TextFrame.TextRange.Text;
            }); 
        }
        public void Resume() 
        { 
            InvokeOnPptThread(() =>
            {
                try
                {
                    var win = GetWindow();
                    win.View.State = PowerPoint.PpSlideShowState.ppSlideShowRunning;
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
                    var win = GetWindow();
                    win.View.State = PowerPoint.PpSlideShowState.ppSlideShowPaused;
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
                    win.View.State = PpSlideShowState.ppSlideShowRunning;
                }
                catch { }
            }); 
        }
        private bool PresentationHasTimingsInternal() 
        {
            foreach (PowerPoint.Slide slide in _presentation.Slides)
            {
                var trans = slide.SlideShowTransition;

                if (trans.AdvanceOnTime == MsoTriState.msoTrue &&
                    trans.AdvanceTime > 0)
                {
                    return true;
                }
            }

            return false;
        }
        private void MonitorTimer_Tick(object sender, EventArgs e)
        {
            if (_slideShowView == null)
                return;

            try
            {
                // If the slideshow window is gone, slideshow ended
                if (_slideShowView?.State != PowerPoint.PpSlideShowState.ppSlideShowDone &&
                    _app.SlideShowWindows.Count > 0)
                {
                    // COM heartbeat — keeps PowerPoint events flowing
                    int current = _slideShowView?.Slide?.SlideIndex ?? -1;
                }
                else
                {
                    OnSlideShowEndInternal();
                    return;
                }

            }
            catch
            {
                // Ignore COM timing issues
            }
        }

        private void ReleaseComObject<T>(ref T obj) where T : class
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

    }
}
