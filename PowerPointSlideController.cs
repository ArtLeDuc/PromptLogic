using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static PromptLogic.Controllers.PptController;
using Office = Microsoft.Office.Core;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace PromptLogic
{
    public class PowerPointSlideController : ISlideController
    {
        // PowerPoint COM objects
        private PowerPoint.Application _app = null;
        private PowerPoint.Presentation _presentation = null;
        private PowerPoint.SlideShowWindow _slideShowWindow = null;
        private PowerPoint.SlideShowView _slideShowView = null;

        // Events exposed to the UI
        public event EventHandler<SlideChangedEventArgs> SlideChanged;
        public event EventHandler SlideShowBegin;
        public event EventHandler Disconnected;
        public event EventHandler TimingsDetected;
        public event EventHandler SlideShowEnded;

        // Internal state
        private bool _isConnected = false;

        private System.Windows.Forms.Timer _monitorTimer;


        public PowerPointSlideController()
        {

        }

        public SlideShowState State
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

        public int SlideCount => _app.ActivePresentation.Slides.Count;

        public int CurrentSlide => GetWindow().View.CurrentShowPosition;

        public void NextSlide()
        {
            var win = GetWindow();
            EnsureRunning(win);
            win.View.Next();
        }

        public void PreviousSlide()
        {
            var win = GetWindow();
            EnsureRunning(win);
            win.View.Previous();
        }

        public void GoToSlide(int index)
        {
            var win = GetWindow();
            win.View.GotoSlide(index);
        }

        public string GetNotes(int index) =>
            _presentation.Slides[index].NotesPage.Shapes.Placeholders[2].TextFrame.TextRange.Text;

        public SlideShowState GetState() => State;

        public void Pause()
        {
            try
            {
                var win = GetWindow();
                win.View.State = PpSlideShowState.ppSlideShowPaused;
            }
            catch { }
        }

        public void Resume()
        {
            try
            {
                var win = GetWindow();
                win.View.State = PpSlideShowState.ppSlideShowRunning;
            }
            catch { }
        }

        public string GetSlideTitle(int index)
        {
            var win = GetWindow();
            var slide = win.Presentation.Slides[index];

            try
            {
                var titleShape = slide.Shapes.Title;
                if (titleShape != null)
                    return titleShape.TextFrame.TextRange.Text;
            }
            catch
            {               
            }
            return "Slide " + index;
        }


        [DllImport("user32.dll")] private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")] private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_SHOW = 5;
        private const int SW_RESTORE = 9;

        public void RefocusSlideShowWindow()
        {
            try
            {
                var win = GetWindow();
                IntPtr hwnd = (IntPtr)win.HWND;

                SetForegroundWindow(hwnd);
                win.Activate();

                // Step 1: Unpause the slideshow
                win.View.State = PpSlideShowState.ppSlideShowRunning;
            }
            catch { }
        }

        public bool PresentationHasTimings()
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

        public void EndSlideShow()
        {
            var win = GetWindow();
//            win.View.Exit();
        }
        public void ClearAllTimings()
        {
            foreach (PowerPoint.Slide slide in _presentation.Slides)
            {
                var trans = slide.SlideShowTransition;
                trans.AdvanceOnTime = MsoTriState.msoFalse;
                trans.AdvanceTime = 0;
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
        public void MonitorTimerStop()
        {
            _monitorTimer?.Stop();
        }

        public void HookSlideShowEvents()
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

            _monitorTimer = new System.Windows.Forms.Timer();
            _monitorTimer.Interval = 50; // 50ms
            _monitorTimer.Tick += MonitorTimer_Tick;
            _monitorTimer.Start();

            _isConnected = true;

        }
        private void OnSlideShowBegin(PowerPoint.SlideShowWindow Wn)
        {
            SlideShowBegin?.Invoke(this, EventArgs.Empty);
            _slideShowView = GetWindow()?.View;
        }

        public bool Connect() // (IWebViewActions ui)
        {
            // Try to get the running PowerPoint instance
            try
            {
                _app = Marshal.GetActiveObject("PowerPoint.Application") as PowerPoint.Application;
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

            if (PresentationHasTimings())
                TimingsDetected?.Invoke(this, EventArgs.Empty);

            
            return true;
        }

        public string GetNotesForCurrentSlide()
        {
            if (_slideShowView == null)
                return string.Empty;

            return GetNotesForSlide(_slideShowView.Slide.SlideIndex);
        }

        public string GetNotesForSlide(int index)
        {
            try
            {
                var slide = _presentation.Slides[index];
                return slide.NotesPage.Shapes.Placeholders[2].TextFrame.TextRange.Text;
            }
            catch
            {
                return string.Empty;
            }
        }

        public bool IsSlideShowRunning
        {
            get
            {
                try
                {
                    return _app?.SlideShowWindows.Count > 0 ? true : false;
                }
                catch
                {
                    return false;
                }
            }
        }

        public void Disconnect()
        {
            _isConnected = false;

            // --- 0. Stop monitor timer ---
            try
            {
                if (_monitorTimer != null)
                {
                    _monitorTimer.Stop();
                    _monitorTimer.Tick -= MonitorTimer_Tick;
                    _monitorTimer.Dispose();
                    _monitorTimer = null;
                }
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
        }

        private void OnSlideShowEnd()
        {
            SlideShowEnded?.Invoke(this, EventArgs.Empty);

            // Optionally trigger Disconnect()
            Disconnect();
        }

        private void OnSlideShowEnd(PowerPoint.Presentation Pres)
        {
            OnSlideShowEnd();
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
                    OnSlideShowEnd();
                    return;
                }

            }
            catch
            {
                // Ignore COM timing issues
            }
        }
    }
}
