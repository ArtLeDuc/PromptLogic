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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Office = Microsoft.Office.Core;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace Teleprompter
{
    public partial class Form1 : Form, IWebViewActions
    {
        ISlideController _controller;
        private ISlideController _slides = null;
        WebMessageService _service = null;
        private SlideEngine _selectedEngine;


        public Form1()
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
        private void Form1_Load(object sender, EventArgs e)
        {
            string htmlPath = @"C:\Users\ajled\teleprompter\web\index.html";
            webView21.Source = new Uri(htmlPath);

            webView21.CoreWebView2InitializationCompleted += (s, g) =>
            {
                webView21.CoreWebView2.Settings.AreDevToolsEnabled = true;
            };

            double speedValue = SettingsManager.Settings.ScrollSpeed;
            traSpeed.Value = (int)speedValue;

            this.FormClosing += MainForm_FormClosing;

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
            webView21.CoreWebView2.PostWebMessageAsString(message);
        }

        public void ExecuteScriptAsync(string script)
        {
            webView21.ExecuteScriptAsync(script);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            _slides.GoToSlide(cmbStartSlide.SelectedIndex+1);

            double speedValue = SettingsManager.Settings.ScrollSpeed / 100.0;
            webView21.ExecuteScriptAsync($"setSpeed({speedValue});");

            webView21.ExecuteScriptAsync("startTeleprompter()");
        }

        private bool isPaused = false;
        private void btnPause_Click(object sender, EventArgs e)
        {
            if (!isPaused)
            {
                // Currently scrolling → pause it
                webView21.ExecuteScriptAsync("pauseScroll()");
                btnPause.Text = "Resume";
                isPaused = true;
            }
            else
            {
                // Currently paused → resume scrolling
                webView21.ExecuteScriptAsync("startScroll()");
                btnPause.Text = "Pause";
                isPaused = false;
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            webView21.ExecuteScriptAsync("stopScroll()");
            btnPause.Text = "Pause";
            isPaused = false;
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
        private void btnConnect_Click(object sender, EventArgs e)
        {
            _slides = SlideControllerFactory.Create(_selectedEngine);
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
                webView21.CoreWebView2.WebMessageReceived -= _service.Handler;

            _service = new WebMessageService(this);   // inject controller
            _service.SetSlideController(_slides);

            webView21.CoreWebView2.WebMessageReceived += _service.Handler;


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

        private void LoadNotesForCurrentSlide()
        {
            string notes = _slides.GetNotesForCurrentSlide();

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

                webView21.ExecuteScriptAsync($"loadNotes(\"{escaped}\")");
            }));
        }

        private void btnWebDebugger_Click(object sender, EventArgs e)
        {
            webView21.CoreWebView2.OpenDevToolsWindow();
        }

        public void OnSlideChanged(int index)
        {
            this.Invoke((Action)(() =>
            {
                LoadNotesForCurrentSlide();
                btnPause.Text = "Pause";
                isPaused = false;
            }));
        }

        private void traSpeed_ValueChanged(object sender, EventArgs e)
        {
            SettingsManager.Settings.ScrollSpeed = traSpeed.Value;
            //The range wants to be between 0.01 and 1
            double speed = (double)traSpeed.Value/100.0;
            string jsSpeed = speed.ToString(CultureInfo.InvariantCulture);
            webView21.ExecuteScriptAsync($"setSpeed({jsSpeed});");

        }
    }
}
