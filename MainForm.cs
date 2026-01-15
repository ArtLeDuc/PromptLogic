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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using static Teleprompter.TransparentOverlay;
using Office = Microsoft.Office.Core;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace Teleprompter
{
    public partial class MainForm : Form, IWebViewActions, ITeleprompter, ITeleprompterPreview
    {
        private ISlideController _slides = null;
        WebMessageService _service = null;
        private SlideEngine _selectedEngine;
        const string _htmlPath = @"E:\source\Teleprompter\Teleprompter\Web\index.html";
        private Settings settingsWindow;
        private ScrollSpeed scrollSpeed;
        private HighlightBand highlightBand;
        private readonly WindowManager _windowManager;

        private TransparentOverlay dragOverlay = null;
        private bool IsOnAnyScreen(Rectangle rect)
        {
            return Screen.AllScreens.Any(s => s.WorkingArea.IntersectsWith(rect));
        }

        public MainForm()
        {
            InitializeComponent();
            _windowManager = new WindowManager(this);

            _selectedEngine = SettingsManager.Settings.SelectedEngine;

            var s = SettingsManager.Settings;

            if (SettingsManager.Settings.MainFormBorderStyle == FormBorderStyle.None)
            {
                this.FormBorderStyle = FormBorderStyle.None;

                if (!SettingsManager.Settings.BorderlessSize.IsEmpty)
                {
                    // Restore client size and location
                    this.StartPosition = FormStartPosition.Manual;
                    this.Location = SettingsManager.Settings.BorderlessLocation;
                    this.ClientSize = SettingsManager.Settings.BorderlessSize;
                }
            }
            else
            {
                this.FormBorderStyle = FormBorderStyle.Sizable;

                if (!SettingsManager.Settings.WindowBounds.IsEmpty)
                {
                    // Restore full bounds
                    this.StartPosition = FormStartPosition.Manual;
                    this.Bounds = SettingsManager.Settings.WindowBounds;
                }
            }

            //Ensure this is on a monitor
            if (!IsOnAnyScreen(this.Bounds))
            {
                var bounds = SettingsManager.Settings.WindowBounds;
                var primary = Screen.PrimaryScreen.WorkingArea;
                bounds.X = primary.Left + 50;
                bounds.Y = primary.Top + 50;
                this.Bounds = bounds;
            }

            dragOverlay = new TransparentOverlay();
            dragOverlay.Dock = DockStyle.Fill;
            dragOverlay.BackColor = Color.Transparent;
            dragOverlay.Visible = true;
            dragOverlay.MouseDown += DragArea_MouseDown;
            dragOverlay.OverlayMouseWheel += DragOverlay_OverlayMouseWheel;
            dragOverlay.RightClickRequested += pos => ShowContextMenu(pos);
            dragOverlay.TargetForm = this;

            this.Controls.Add(dragOverlay);
            dragOverlay.BringToFront();
            
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
                        LoadInitialPage();
                    };
                }
            };

            await webView.EnsureCoreWebView2Async();
        }

        private void ShowContextMenu(System.Drawing.Point screenPos)
        {
            var menu = new ContextMenuStrip();

            menu.Items.Add("Connect", null, (_, __) => ConnectSlideShow());

            if (!isStopped)
            {
                if (isPaused)
                    menu.Items.Add("Resume", null, (_, __) => PauseSlideShow());
                else
                    menu.Items.Add("Pause", null, (_, __) => PauseSlideShow());
                menu.Items.Add("Stop", null, (_, __) => StopSlideShow());
            }
            else
            {
                menu.Items.Add("Start", null, (_, __) => StartSlideShow());
            }

            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add("Speed...", null, (_, __) => OpenSpeedSetting());
            menu.Items.Add("Highlight...", null, (_,__) => OpenHighlightSetting());
            menu.Items.Add("Settings...", null, (_, __) => OpenSettings());
            menu.Items.Add(new ToolStripSeparator());

            if (!pnlCollapsed.Visible && !pnlControl.Visible)
                menu.Items.Add("Show Control Panel", null, (_, __) => ((ITeleprompterPreview)this).ApplyShowControlSidebar(true));
            else 
                menu.Items.Add("Hide Control Panel", null, (_, __) => ((ITeleprompterPreview)this).ApplyShowControlSidebar(false));

            menu.Items.Add(new ToolStripSeparator());

            menu.Items.Add("Close", null, (_, __) => Close());

            menu.Show(screenPos);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            webView.Source = new Uri(_htmlPath);

            InitializeWebView();
        
            this.FormClosing += MainForm_FormClosing;

            toolTip1.SetToolTip(btnCollapsedStart, "Start Text Scrolling");
            toolTip1.SetToolTip(btnCollapsedPause, "Pause/Resume Text Scrolling");
            toolTip1.SetToolTip(btnCollapsedStop, "Stop Text Scrolling resets to begining of current text");
            toolTip1.SetToolTip(btnCollapsedConnect, "Connect to a slide show");
            toolTip1.SetToolTip(btnExpand, "Expand control panel");
            toolTip1.SetToolTip(btnCollapse, "Collapse control panel");
            toolTip1.SetToolTip(btnWebDebugger, "Start the browser debugger");
            toolTip1.SetToolTip(btnSettings, "Teleprompter Settings");
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
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                if (SettingsManager.Settings.NonActivating)
                    cp.ExStyle |= NativeMethods.WS_EX_NOACTIVATE;
                return cp;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.FormBorderStyle == FormBorderStyle.None)
            {
                // Borderless mode: save the *client* size and location
                SettingsManager.Settings.BorderlessSize = this.ClientSize;
                SettingsManager.Settings.BorderlessLocation = this.Location;
                SettingsManager.Settings.MainFormBorderStyle = FormBorderStyle.None;
            }
            else
            {
                // Normal mode: save the full window bounds
                SettingsManager.Settings.WindowBounds = this.Bounds;
                SettingsManager.Settings.MainFormBorderStyle = FormBorderStyle.Sizable;
            }              

            SettingsManager.Save();
        }
        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        public void SendToWebView(string message)
        {
            webView.CoreWebView2.PostWebMessageAsString(message);
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
        private void LoadNotesForCurrentSlide()
        {
            string notes = _slides.GetNotesForCurrentSlide();
            SendNotesToWebView(notes);
            ApplyAllSettings();
        }
        private void LoadNotesForSlide(int index)
        {
            string notes = _slides.GetNotesForSlide(index);
            SendNotesToWebView(notes);
        }
        public void LoadInitialPage()
        {
            ClearTeleprompter();
        }
        public void ClearTeleprompter()
        {
            SendNotesToWebView("Connected. Waiting for slide notes — press Connect Or choose Load Sample Script.<br>When ready press Start.</br>");
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
                BeginInvoke(new Action(() => OnSlideChanged(index)));
                return;
            }

            LoadNotesForSlide(index);
            btnPause.Text = "Pause";
            isPaused = false;
        }

        private void btnCollapse_Click(object sender, EventArgs e)
        {
            ToggleCollapse(true);
        }

        private void Controller_Disconnected(object sender, EventArgs e)
        {
            LoadInitialPage(); // reload index.html
            btnConnect.Enabled = true;
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
        private void ConnectToWebView()
        {
            if (_service != null)
                webView.CoreWebView2.WebMessageReceived -= _service.Handler;

            _service = new WebMessageService(this);   // inject controller
            _service.SetSlideController(_slides);

            webView.CoreWebView2.WebMessageReceived += _service.Handler;

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

            ConnectToWebView();

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
        public void StartSlideShow()
        {
            if (_slides != null)
                _slides.GoToSlide(cmbStartSlide.SelectedIndex + 1);
            StartTeleprompter();
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            StartSlideShow();
            UpdateControls();
        }
        private void btnCollapsedStart_Click(object sender, EventArgs e)
        {
            StartSlideShow();
            UpdateControls();
        }
        public void UpdateControls()
        {
            if (isPaused)
            {
                btnPause.Text = ">> Resume";
                btnCollapsedPause.Text = ">>";
            }
            else
            {
                btnPause.Text = "|| Pause";
                btnCollapsedPause.Text = "||";
            }

            btnCollapsedConnect.Enabled = isStopped;
            btnLoadSampleScript.Enabled = isStopped;
            btnSettings.Enabled = isStopped;
            cmbStartSlide.Enabled = isStopped;
            btnConnect.Enabled = isStopped;

            btnStart.Enabled = isStopped;
            btnCollapsedStart.Enabled = isStopped;

            btnPause.Enabled = !isStopped;
            btnCollapsedPause.Enabled = !isStopped;

            btnStop.Enabled = !isStopped;
            btnCollapsedStop.Enabled = !isStopped;

            if (settingsWindow != null && settingsWindow.Visible)
                settingsWindow.Enabled = isStopped;

            SetManualScrolling();
        }
        public void PauseSlideShow()
        {
            PauseTeleprompter();
            UpdateControls();
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
            StopTeleprompter();
            UpdateControls();
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
            // Text color
            ((ITeleprompterPreview)this).ApplyTextColor(s.TextColor);
            // Background color
            ((ITeleprompterPreview)this).ApplyBackgroundColor(s.BackColor);
            //Line spacing
            ((ITeleprompterPreview)this).ApplyLineSpacing(s.LineSpacing);
            //Paragraph spacing
            ((ITeleprompterPreview)this).ApplyParagraphSpacing(s.ParagraphSpacing);

            ((ITeleprompterPreview)this).ApplyHighlightVisible(s.HighlightBandVisible);
            ((ITeleprompterPreview)this).ApplyHighlightOpacity(s.HighlightBandOpacity);
            ((ITeleprompterPreview)this).ApplyHighlightColor(s.HighlightBandColor);
            ((ITeleprompterPreview)this).ApplyHighlightLines(s.HighlightHeightLines);
            ((ITeleprompterPreview)this).ApplyHighlightTriggerPoint(s.HighlightBandTriggerPoint);
            ((ITeleprompterPreview)this).ApplyHighlightTop(s.HighlightBandDistanceFromTop);

            ((ITeleprompterPreview)this).ApplyHighlightBandTriggerPointVisible(s.HighlightBandTriggerPointVisible);
            ((ITeleprompterPreview)this).ApplyHighlightBandTriggerPointColor(s.HighlightBandTriggerPointColor);

            ((ITeleprompterPreview)this).ApplyMainBorderStyle(s.MainFormBorderStyle);
            ((ITeleprompterPreview)this).ApplyShowControlSidebar(s.ShowControlSidebar);

            ((ITeleprompterPreview)this).ApplyMirrorText(s.MirrorText);

            ((ITeleprompterPreview)this).ApplyAlwaysOnTop(s.AlwaysOnTop);

        }

        private void btnLoadSampleScript_Click(object sender, EventArgs e)
        {
            SendNotesToWebView(SampleScripts.Default);
            ConnectToWebView();
        }

        private void DragArea_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            // Ask the overlay if we're on a resize edge
            var edge = dragOverlay.GetResizeEdge(e.Location);
            if (edge != ResizeEdge.None)
                return;

            NativeMethods.ReleaseCapture();
            NativeMethods.SendMessage(this.Handle, NativeMethods.WM_NCLBUTTONDOWN, (IntPtr)NativeMethods.HTCAPTION, IntPtr.Zero);
        }

        private void DragOverlay_OverlayMouseWheel(object sender, MouseEventArgs e)
        {
            int delta = e.Delta; // -120 or +120

            ScrollByWheel(delta);
        }

        private void ApplyMirror(bool enabled)
        {
            string value = enabled ? "-1" : "1";
            webView.CoreWebView2.ExecuteScriptAsync(
                $"document.body.style.transform = 'scaleX({value})';"
            );
        }

        private void ApplyAlwaysOnTop(bool topMost)
        {
            NativeMethods.SetWindowPos(this.Handle,
                topMost ? NativeMethods.HWND_TOPMOST : NativeMethods.HWND_NOTOPMOST,
                0, 0, 0, 0,
                NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOACTIVATE
            );
        }

        private void OpenSpeedSetting()
        {
            if (scrollSpeed == null || scrollSpeed.IsDisposed)
            {
                scrollSpeed = new ScrollSpeed(this);
                _windowManager.ShowPopup(scrollSpeed);
            }
            else
            {
                _windowManager.ClosePopup(scrollSpeed);
            }
        }

        private void OpenHighlightSetting()
        {
            if (highlightBand == null || highlightBand.IsDisposed)
            {
                highlightBand = new HighlightBand(this);
                _windowManager.ShowPopup(highlightBand);
            }
            else
            {
                _windowManager.ClosePopup(highlightBand); 
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenSpeedSetting();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenSpeedSetting();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenHighlightSetting();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenHighlightSetting();
        }

        public bool IsAppTopMost() 
        {
            return SettingsManager.Settings.AlwaysOnTop;
        }
    }
}
