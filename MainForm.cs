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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static PromptLogic.TransparentOverlay;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using Office = Microsoft.Office.Core;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace PromptLogic
{
    public partial class MainForm : Form, IWebViewActions, ITeleprompter, ITeleprompterControl
    {
        private ISlideController _slides = null;
        WebMessageService _service = null;
        private SlideEngine _selectedEngine;
        const string _htmlPath = @"Web\index.html";
        public readonly WindowManager _windowManager;
        private TransparentOverlay dragOverlay = null;
        private ScrollSpeed scrollSpeed;
        private HighlightBand highlightBand;
        private Settings settingsWindow;
        private ControlPanelOptions controlPanelOptions;
        private bool _inputLocked = false;
        private RightClickMenu rightClickMenu;
        private bool IsOnAnyScreen(Rectangle rect)
        {
            return Screen.AllScreens.Any(s => s.WorkingArea.IntersectsWith(rect));
        }
        public bool InputLocked
        {
            get => _inputLocked;
            set
            {
                _inputLocked = value;
                UpdateInputState();
            }
        }

        public void LockInput()
        {
            InputLocked = true;
        }

        public void UnlockInput()
        {
            InputLocked = false;
        }
        private void UpdateInputState()
        {
            this.Enabled = !InputLocked;
            _windowManager.LockInput(InputLocked);
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

            rightClickMenu = new RightClickMenu(this);
            this.ContextMenu = rightClickMenu;
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

            var userDataFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "PromptLogic",
                    "WebView2"
                );

            var env = await CoreWebView2Environment.CreateAsync(null, userDataFolder);
            await webView.EnsureCoreWebView2Async(env);

            var htmlPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                _htmlPath
            );
            webView.Source = new Uri(htmlPath);

        }

        
        private void ShowContextMenu(System.Drawing.Point screenPos)
        {
            if (InputLocked)
                return;

            this.ContextMenu.Show(this, this.PointToClient(screenPos));
        }
        

        private void MainForm_Load(object sender, EventArgs e)
        {
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

            ((ITeleprompterControl)this).ApplySetControlPanelState(SettingsManager.Settings.controlPanelVisible, SettingsManager.Settings.controlPanelCompressed);

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
            if (webView.InvokeRequired)
            {
                webView.BeginInvoke(new Action(() => SendToWebView(message)));
                return;
            }

            var core = webView.CoreWebView2;
            if (core == null || webView.IsDisposed)
                return;

            core.PostWebMessageAsString(message);
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
            SendNotesToWebView("Waiting for slide notes press Connect Or Choose Load Sample Script.<br>When ready press Start.</br>");
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
            IsPaused = false;
        }

        private void btnCollapse_Click(object sender, EventArgs e)
        {
            ((ITeleprompterControl)this).ApplySetControlPanelState(SettingsManager.Settings.controlPanelVisible, true);
        }

        private void Controller_Disconnected(object sender, EventArgs e)
        {
            LoadInitialPage(); // reload index.html
            btnConnect.Enabled = true;
        }
        private void btnExpand_Click(object sender, EventArgs e)
        {
            ((ITeleprompterControl)this).ApplySetControlPanelState(SettingsManager.Settings.controlPanelVisible, false);
        }
        private void ConnectToWebView()
        {
            if (_service != null)
                webView.CoreWebView2.WebMessageReceived -= _service.Handler;

            _service = new WebMessageService(this);   // inject controller
            _service.SetSlideController(_slides);

            webView.CoreWebView2.WebMessageReceived += _service.Handler;

        }
        private void btnConnect_Click(object sender, EventArgs e)
        {
            ((ITeleprompterControl)this).ConnectSlideShow();
        }
        private void btnCollapsedConnect_Click(object sender, EventArgs e)
        {
            ((ITeleprompterControl)this).ConnectSlideShow();
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            ((ITeleprompterControl)this).StartSlideShow();
        }
        private void btnCollapsedStart_Click(object sender, EventArgs e)
        {
            ((ITeleprompterControl)this).StartSlideShow();
        }
        public void UpdateControls()
        {
            if (IsPaused)
            {
                btnPause.Text = "» Resume";
                btnCollapsedPause.Text = "»";
            }
            else
            {
                btnPause.Text = "❚❚Pause";
                btnCollapsedPause.Text = "❚❚";
            }

            btnCollapsedConnect.Enabled = IsStopped;
            btnLoadSampleScript.Enabled = IsStopped;
            btnSettings.Enabled = IsStopped;
            cmbStartSlide.Enabled = IsStopped;
            btnConnect.Enabled = IsStopped;

            btnStart.Enabled = IsStopped;
            btnCollapsedStart.Enabled = IsStopped;

            btnPause.Enabled = !IsStopped;
            btnCollapsedPause.Enabled = !IsStopped;

            btnStop.Enabled = !IsStopped;
            btnCollapsedStop.Enabled = !IsStopped;

            if (settingsWindow != null && settingsWindow.Visible)
                settingsWindow.Enabled = IsStopped;

            SetManualScrolling();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            ((ITeleprompterControl)this).PauseSlideShow();
        }
        private void btnCollapsedPause_Click(object sender, EventArgs e)
        {
            ((ITeleprompterControl)this).PauseSlideShow();
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            ((ITeleprompterControl)this).StopSlideShow();
        }

        private void btnCollapsedStop_Click(object sender, EventArgs e)
        {
            ((ITeleprompterControl)this).StopSlideShow();
        }

        bool IsActuallyTopMost()
        {
            uint exStyle = NativeMethods.GetWindowLong((IntPtr)this.Handle, (int)NativeMethods.GWL_EXSTYLE);
            return (exStyle & NativeMethods.WS_EX_TOPMOST) != 0;
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
            ((ITeleprompterControl)this).ApplyFont(s.TeleprompterFont);
            // Font size
            ((ITeleprompterControl)this).ApplyFontSize(s.FontSize);
            // Text color
            ((ITeleprompterControl)this).ApplyTextColor(s.TextColor);
            // Background color
            ((ITeleprompterControl)this).ApplyBackgroundColor(s.BackColor);
            //Line spacing
            ((ITeleprompterControl)this).ApplyLineSpacing(s.LineSpacing);
            //Paragraph spacing
            ((ITeleprompterControl)this).ApplyParagraphSpacing(s.ParagraphSpacing);

            ((ITeleprompterControl)this).ApplyHighlightVisible(s.HighlightBandVisible);
            ((ITeleprompterControl)this).ApplyHighlightOpacity(s.HighlightBandOpacity);
            ((ITeleprompterControl)this).ApplyHighlightColor(s.HighlightBandColor);
            ((ITeleprompterControl)this).ApplyHighlightLines(s.HighlightHeightLines);
            ((ITeleprompterControl)this).ApplyHighlightTriggerPoint(s.HighlightBandTriggerPoint);
            ((ITeleprompterControl)this).ApplyHighlightTop(s.HighlightBandDistanceFromTop);

            ((ITeleprompterControl)this).ApplyHighlightBandTriggerPointVisible(s.HighlightBandTriggerPointVisible);
            ((ITeleprompterControl)this).ApplyHighlightBandTriggerPointColor(s.HighlightBandTriggerPointColor);

            ((ITeleprompterControl)this).ApplyMainBorderStyle(s.MainFormBorderStyle);
            ((ITeleprompterControl)this).ApplySetControlPanelState(s.controlPanelVisible, s.controlPanelCompressed);

            ((ITeleprompterControl)this).ApplyMirrorText(s.MirrorText);

            ((ITeleprompterControl)this).ApplyAlwaysOnTop(s.AlwaysOnTop);

        }

        public void LoadSampleScript()
        {
            SendNotesToWebView(SampleScripts.Default);
            ConnectToWebView();
        }
        private void btnLoadSampleScript_Click(object sender, EventArgs e)
        {
            LoadSampleScript();
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

        private void OpenSettings()
        {
            if (settingsWindow == null || settingsWindow.IsDisposed)
            {
                settingsWindow = new Settings(this);
                _windowManager.ShowPopup(settingsWindow);
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

        public void OpenBorderNControl()
        {
            if (controlPanelOptions == null || controlPanelOptions.IsDisposed)
            {
                controlPanelOptions = new ControlPanelOptions(this);
                _windowManager.ShowPopup(controlPanelOptions);
            }
            else
            {
                _windowManager.ClosePopup(controlPanelOptions);
            }

        }

        private void btnBorderNControl_Click(object sender, EventArgs e)
        {
            OpenBorderNControl();
        }

        private void btnCollapsedControlPnl_Click(object sender, EventArgs e)
        {
            OpenBorderNControl();
        }

    }
}
