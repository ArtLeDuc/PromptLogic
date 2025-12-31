using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Office = Microsoft.Office.Core;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace Teleprompter
{
    public partial class MainForm : Form, IWebViewActions, ITeleprompterPreview
    {
        private ISlideController _slides = null;
        WebMessageService _service = null;
        private SlideEngine _selectedEngine;
        private string _lastNotesSent = null;
        const string _htmlPath = @"E:\source\Teleprompter\Teleprompter\Web\index.html";


        public MainForm()
        {
            InitializeComponent();
            _selectedEngine = SettingsManager.Settings.SelectedEngine;

            var s = SettingsManager.Settings;
            if (s.WindowWidth > 0 && s.WindowHeight > 0)
            {
                this.StartPosition = FormStartPosition.Manual;

                this.Left =   s.WindowLeft;
                this.Top =    s.WindowTop;
                this.Width =  s.WindowWidth;
                this.Height = s.WindowHeight;

                //Check if the form shows up on any monitor
                //If not move it to the primary monitor.
                Rectangle proposed = new Rectangle(
                        s.WindowLeft,
                        s.WindowTop,
                        s.WindowWidth,
                        s.WindowHeight);

                bool isVisible = Screen.AllScreens.Any(g => g.WorkingArea.IntersectsWith(proposed));
                if (!isVisible)
                {
                    var primary = Screen.PrimaryScreen.WorkingArea;

                    this.Left = primary.Left + 50;
                    this.Top = primary.Top + 50;
                }
            }
        }
        private async void InitializeWebView()
        {
            webView.CoreWebView2InitializationCompleted += (s, g) =>
            {
                if (g.IsSuccess)
                {
                    // HTML load event
                    webView.CoreWebView2.DOMContentLoaded += (s2, e2) =>
                    {
                        ApplyAllSettings();
                    };

//                    LoadInitialPage();
                }
            };

            await webView.EnsureCoreWebView2Async();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            webView.Source = new Uri(_htmlPath);

            InitializeWebView();
        
            double speedValue = SettingsManager.Settings.ScrollSpeed;
            traSpeed.Value = (int)speedValue;

            this.FormClosing += MainForm_FormClosing;

            toolTip1.SetToolTip(btnCollapsedStart, "Start Text Scrolling");
            toolTip1.SetToolTip(btnCollapsedPause, "Pause/Resume Text Scrolling");
            toolTip1.SetToolTip(btnCollapsedStop, "Stop Text Scrolling resets to begining of current text");
            toolTip1.SetToolTip(btnCollapsedConnect, "Connect to a slide show");
            toolTip1.SetToolTip(btnExpand, "Expand control panel");
            toolTip1.SetToolTip(btnCollapse, "Collapse control panel");
            toolTip1.SetToolTip(btnWebDebugger, "Start the browser debugger");
            toolTip1.SetToolTip(btnSettings, "Teleprompter Settings");
            toolTip1.SetToolTip(traSpeed, "Adjust scrolling speed");
            toolTip1.SetToolTip(btnStop, "Stop Text scrolling resets to begining of the current text");
            toolTip1.SetToolTip(btnPause, "Pause/Resume text Scrolling");
            toolTip1.SetToolTip(btnStart, "Start Text Scrolling");
            toolTip1.SetToolTip(cmbStartSlide, "Select the slide that will be selected when start pressed");
            toolTip1.SetToolTip(btnConnect, "Connect to a slide show");

            toolTip1.AutoPopDelay = 5000;
            toolTip1.InitialDelay = 300;
            toolTip1.ReshowDelay = 100;
            toolTip1.ShowAlways = true;

            ToggleCollapse(SettingsManager.Settings.IsCollapsed);

#if !DEBUG
            btnWebDebugger.Visible = false;
#endif
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SettingsManager.Settings.WindowLeft = this.Left;
            SettingsManager.Settings.WindowTop = this.Top;
            SettingsManager.Settings.WindowWidth = this.Width;
            SettingsManager.Settings.WindowHeight = this.Height;
            SettingsManager.Save();
        }
        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_EX_NOACTIVATE = 0x08000000;
                const int WS_EX_TOPMOST = 0x00000008;

                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WS_EX_NOACTIVATE | WS_EX_TOPMOST;
                return cp;
            }
        }
        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        public void SendToWebView(string message)
        {
            webView.CoreWebView2.PostWebMessageAsString(message);
        }

        public void ExecuteScriptAsync(string script)
        {
            webView.ExecuteScriptAsync(script);
        }
        private void LoadSlideSelectionCombo()
        {
            cmbStartSlide.Items.Clear();

            for (int i = 1; i <= _slides.SlideCount; i++)
            {
                string title = _slides.GetSlideTitle(i);
                if (string.IsNullOrWhiteSpace(title))
                    title = $"Slide {i}";

                int item = cmbStartSlide.Items.Add(title);
            }

            cmbStartSlide.SelectedIndex = SettingsManager.Settings.StartSlideIndex;

        }

        private void SendNotesToWebView(string notes)
        {
            if (notes == _lastNotesSent)
                return;
            _lastNotesSent = notes;


            this.Invoke((Action)(() =>
            {
                // Escape for JS
                string escaped = notes
                    .Replace("\r\n", "\n")   // CRLF → LF
                    .Replace("\r", "\n")     // CR → LF
                    .Replace("\v", "\n")     // VT → LF
                    .Replace("\\", "\\\\")   // escape backslashes
                    .Replace("\"", "\\\"")   // escape quotes
                    .Replace("\n", "\\n");   // LF → JS newline

                webView.ExecuteScriptAsync($"loadNotes(\"{escaped}\")");
            }));
        }
        private void LoadNotesForCurrentSlide()
        {
            string notes = _slides.GetNotesForCurrentSlide();
            SendNotesToWebView(notes);
        }
        private void LoadNotesForSlide(int index)
        {
            string notes = _slides.GetNotesForSlide(index);
            SendNotesToWebView(notes);
        }
        public void LoadInitialPage()
        {
            webView.CoreWebView2.Navigate(_htmlPath);
        }

        public void InvokeOnUIThread(Action action)
        {
            if (InvokeRequired)
                BeginInvoke(action);
            else
                action();
        }

        public void OnSlideChanged(int index)
        {
            if (InvokeRequired)
            {
                Debug.WriteLine($"OnSlideChanged next slide: {index}");
                BeginInvoke(new Action(() => OnSlideChanged(index)));
                return;
            }

            LoadNotesForSlide(index);
            btnPause.Text = "Pause";
            isPaused = false;
        }

        private void traSpeed_ValueChanged(object sender, EventArgs e)
        {
            SettingsManager.Settings.ScrollSpeed = traSpeed.Value;
            //The range wants to be between 0.01 and 1
            double speed = (double)traSpeed.Value/100.0;
            string jsSpeed = speed.ToString(CultureInfo.InvariantCulture);
            webView.ExecuteScriptAsync($"setSpeed({jsSpeed});");

        }

        private void btnCollapse_Click(object sender, EventArgs e)
        {
            ToggleCollapse(true);
        }

        private void Controller_Disconnected(object sender, EventArgs e)
        {
            LoadInitialPage(); // reload index.html
            btnConnect.Enabled = true;
//            btnDisconnect.Enabled = false;
        }
        private void btnExpand_Click(object sender, EventArgs e)
        {
            ToggleCollapse(false);
        }
        private void ToggleCollapse(bool Collapse)
        {
            if (Collapse)
            {
                pnlCollapsed.Visible = true;
                pnlControl.Visible = false;
                SettingsManager.Settings.IsCollapsed = true;
            }
            else
            {
                pnlCollapsed.Visible = false;
                pnlControl.Visible = true;
                SettingsManager.Settings.IsCollapsed = false;
            }
        }

        private void ConnectSlideShow()
        {
            _slides = SlideControllerFactory.Create(_selectedEngine);
            _slides.Disconnected += Controller_Disconnected;

            if (!_slides.Connect(this))
            {
                MessageBox.Show("Could not connect.");
                return;
            }
            _slides.SlideShowBegin += (s, g) => {
                this.BeginInvoke((Action)(() => LoadSlideSelectionCombo()));
            };

            if (_slides.PresentationHasTimings())
            {
                var result = MessageBox.Show(
                    "This presentation has recorded timings. Clear them?",
                    "Timings Detected",
                    MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                    _slides.ClearAllTimings();
            }

            if (_service != null)
                webView.CoreWebView2.WebMessageReceived -= _service.Handler;

            _service = new WebMessageService(this);   // inject controller
            _service.SetSlideController(_slides);

            webView.CoreWebView2.WebMessageReceived += _service.Handler;


            //            int startSlide = 1;
            //            if (int.TryParse(txtStartSlide.Text, out int value) && value > 0)
            //                startSlide = value;
            //            if (startSlide < 1 || startSlide > _slides.SlideCount)
            //            {
            //                MessageBox.Show("Starting slide is not contained within the current slides.\nStarting using slide 1.", "Starting Slide", MessageBoxButtons.OK);
            //                startSlide = 1;
            //            }

            if (_slides.IsSlideShowRunning)
            {
                LoadNotesForCurrentSlide();
                LoadSlideSelectionCombo();
            }
        }
        private void btnConnect_Click(object sender, EventArgs e)
        {
            ConnectSlideShow();
        }
        private void btnCollapsedConnect_Click(object sender, EventArgs e)
        {
            ConnectSlideShow();
        }
        private void StartSlideShow()
        {
            if (_slides != null)
                _slides.GoToSlide(cmbStartSlide.SelectedIndex + 1);

            double speedValue = SettingsManager.Settings.ScrollSpeed / 100.0;
            webView.ExecuteScriptAsync($"setSpeed({speedValue});");

            webView.ExecuteScriptAsync("startTeleprompter()");
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            StartSlideShow();
        }
        private void btnCollapsedStart_Click(object sender, EventArgs e)
        {
            StartSlideShow();
        }

        private bool isPaused = false;
        private void PauseSlideShow()
        {
            if (!isPaused)
            {
                // Currently scrolling → pause it
                webView.ExecuteScriptAsync("pauseScroll()");
                btnPause.Text = ">> Resume";
                btnCollapsedPause.Text = ">>";
                isPaused = true;
            }
            else
            {
                // Currently paused → resume scrolling
                webView.ExecuteScriptAsync("startScroll()");
                btnPause.Text = "|| Pause";
                btnCollapsedPause.Text = "||";
                isPaused = false;
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            PauseSlideShow();
        }
        private void btnCollapsedPause_Click(object sender, EventArgs e)
        {
            PauseSlideShow();
        }
        private void StopSlideShow()
        {
            webView.ExecuteScriptAsync("stopScroll()");
            btnPause.Text = "Pause";
            isPaused = false;
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            StopSlideShow();
        }

        private void btnCollapsedStop_Click(object sender, EventArgs e)
        {
            StopSlideShow();
        }

        [DllImport("user32.dll")] static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

        const int GWL_EXSTYLE = -20;
        const uint WS_EX_TOPMOST = 0x00000008;

        bool IsActuallyTopMost()
        {
            uint exStyle = GetWindowLong(this.Handle, GWL_EXSTYLE);
            return (exStyle & WS_EX_TOPMOST) != 0;
        }

        private Settings settingsWindow;
        private void OpenSettings()
        {
            if (settingsWindow == null || settingsWindow.IsDisposed)
            {
                settingsWindow = new Settings(this);

                // Sync with actual topmost state
                settingsWindow.TopMost = IsActuallyTopMost();

                var screen = Screen.FromControl(this);
                var work = screen.WorkingArea;

                int desiredX = this.Right;
                int desiredY = this.Top;

                bool fitsRight = desiredX + settingsWindow.Width <= work.Right;

                if (fitsRight)
                {
                    settingsWindow.StartPosition = FormStartPosition.Manual;
                    settingsWindow.Location = new System.Drawing.Point(desiredX, desiredY);
                }
                else
                {
                    settingsWindow.StartPosition = FormStartPosition.CenterParent;
                }

                settingsWindow.Show();
                settingsWindow.Focus();
            }
            else
            {
                settingsWindow.Focus();
            }
        }
        private void btnSettings_Click(object sender, EventArgs e)
        {
            OpenSettings();
        }
        private void btnWebDebugger_Click(object sender, EventArgs e)
        {
            webView.CoreWebView2.OpenDevToolsWindow();
        }

        private void ApplyAllSettings()
        {
            var s = SettingsManager.Settings;

            // Font family
            ((ITeleprompterPreview)this).ApplyFont(s.TeleprompterFont);

            // Font size
            ((ITeleprompterPreview)this).ApplyFontSize(s.FontSize);

            // Line spacing
//            ((ITeleprompterPreview)this).ApplyLineSpacing(s.LineSpacing);

            // Text color
            ((ITeleprompterPreview)this).ApplyTextColor(s.TextColor);

            // Background color
            ((ITeleprompterPreview)this).ApplyBackgroundColor(s.BackColor);

            // Highlight band height
//            ((ITeleprompterPreview)this).ApplyHighlightHeight(s.HighlightHeight);

            // Highlight band opacity
//            ((ITeleprompterPreview)this).ApplyHighlightOpacity(s.HighlightOpacity);

            // Highlight band vertical position
//            ((ITeleprompterPreview)this).ApplyHighlightTop(s.HighlightTopPercent);
        }
        private void btnLoadSampleScript_Click(object sender, EventArgs e)
        {
            
            string escaped = SampleScripts.Default
                .Replace("\r\n", "\n")   // CRLF → LF
                .Replace("\r", "\n")     // CR → LF
                .Replace("\v", "\n")     // VT → LF
                .Replace("\\", "\\\\")   // escape backslashes
                .Replace("\"", "\\\"")   // escape quotes
                .Replace("\n", "\\n");   // LF → JS newline
            webView.ExecuteScriptAsync($"loadNotes(\"{escaped}\")");
        }
    }
}
