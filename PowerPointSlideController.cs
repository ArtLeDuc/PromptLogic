using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Office = Microsoft.Office.Core;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace Teleprompter
{
    public class PowerPointSlideController : ISlideController
    {
        private PowerPoint.Application _app = null;
        private PowerPoint.Presentation _presentation;
        private WebMessageService _service;
        public event Action<int> SlideChanged;
        public event EventHandler SlideShowBegin;

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

        public int CurrentSlide =>
            GetWindow().View.CurrentShowPosition;

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

            var titleShape = slide.Shapes.Title;
            if (titleShape != null)
                return titleShape.TextFrame.TextRange.Text;
            else
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
            SlideChanged?.Invoke(Wn.View.CurrentShowPosition);
        }

        public void HookSlideShowEvents()
        {
            _app.SlideShowNextSlide += SlideShowNextSlide;
            _app.SlideShowBegin += OnSlideShowBegin;

            // If a slideshow is already running, attach to it
            if (_app.SlideShowWindows.Count > 0)
            {
                var slideShowWindow = GetWindow();
            }
            else
            {
                MessageBox.Show("Start the slideshow (F5) to begin syncing notes.");
            }
        }
        private void OnSlideShowBegin(PowerPoint.SlideShowWindow Wn)
        {
            SlideShowBegin?.Invoke(this, EventArgs.Empty);
        }

        public bool Connect(IWebViewActions ui)
        {

            try
            {
                // Try to attach to a running instance
                _app = (PowerPoint.Application)System.Runtime.InteropServices.Marshal.GetActiveObject("PowerPoint.Application");
                MessageBox.Show("Connected to running PowerPoint.");
            }
            catch
            {
                // Not running — ask user if they want to launch it
                var result = MessageBox.Show("PowerPoint is not running. Launch it now?", "PowerPoint", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    _app = new PowerPoint.Application();
                    _app.Visible = Office.MsoTriState.msoTrue;

                    // Ask user to pick a file
                    OpenFileDialog dlg = new OpenFileDialog();
                    dlg.Filter = "PowerPoint Files|*.pptx;*.pptm;*.ppt";

                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        _app.Presentations.Open(dlg.FileName, WithWindow: Office.MsoTriState.msoTrue);
                    }
                    else
                    {
                        MessageBox.Show("No file selected.");
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            if (_app.Presentations.Count > 0)
            {
                _presentation = _app.ActivePresentation;
            }
            else
            {
                // No presentation open — return false or throw
                return false;
            }

            if (PresentationHasTimings())
            {
                var result = MessageBox.Show(
                    "This presentation has recorded timings.\n" +
                    "Clear them now?",
                    "Timings Detected",
                    MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                    ClearAllTimings();
            }

            // Now create the service
            _service = new WebMessageService(ui);

            // At this point, pptApp is guaranteed to be valid
            HookSlideShowEvents();

//            if (_app.SlideShowWindows.Count == 0)
//                _app.ActivePresentation.SlideShowSettings.Run();
//            _app.SlideShowWindows[1].View.State = PowerPoint.PpSlideShowState.ppSlideShowPaused;

            return true;
        }

        public string GetNotesForCurrentSlide()
        {
            var win = GetWindow();
            var slide = win.View.Slide;

            string notes = slide.NotesPage.Shapes.Placeholders[2].TextFrame.TextRange.Text;

            return notes;
        }

        public bool IsSlideShowRunning
        {
            get
            {
                try
                {
                    return _app.SlideShowWindows.Count > 0;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
