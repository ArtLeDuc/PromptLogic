using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PromptLogic;

namespace PromptLogic
{
    public partial class MainForm : ITeleprompterControl
    {
        void ITeleprompterControl.ApplyFont(string fontName)
        {
            string safe = fontName.Replace("'", "\\'");
            string js = $"document.documentElement.style.setProperty('--font-family', '{safe}, sans-serif');";
            webView.CoreWebView2.ExecuteScriptAsync(js);
        }

        void ITeleprompterControl.ApplyFontSize(int size)
        {
            string js = $"document.documentElement.style.setProperty('--font-size', '{size}px');";
            webView.CoreWebView2.ExecuteScriptAsync(js);
        }
        void ITeleprompterControl.ApplyLineSpacing(int spacing)
        {
            string js = $"document.documentElement.style.setProperty('--line-spacing', '{spacing}px');";
            webView.CoreWebView2.ExecuteScriptAsync(js);
        }
        void ITeleprompterControl.ApplyParagraphSpacing(int spacing)
        {
            string js = $"document.documentElement.style.setProperty('--paragraph-spacing', '{spacing}px');";
            webView.CoreWebView2.ExecuteScriptAsync(js);
        }
        void ITeleprompterControl.ApplyBreakSpacing1(int spacing)
        {
            string js = $"document.documentElement.style.setProperty('--break-spacing1', '{spacing}px');";
            webView.CoreWebView2.ExecuteScriptAsync(js);
        }
        void ITeleprompterControl.ApplyBreakSpacing2(int spacing)
        {
            string js = $"document.documentElement.style.setProperty('--break-spacing2', '{spacing}px');";
            webView.CoreWebView2.ExecuteScriptAsync(js);
        }
        void ITeleprompterControl.ApplyBreakSpacing3(int spacing)
        {
            string js = $"document.documentElement.style.setProperty('--break-spacing3', '{spacing}px');";
            webView.CoreWebView2.ExecuteScriptAsync(js);
        }

        void ITeleprompterControl.ApplyHighlightHeight(int px)
        {
            string js = $"document.documentElement.style.setProperty('--highlight-band-height', '{px}px');";
            webView.CoreWebView2.ExecuteScriptAsync(js);
        }
        void ITeleprompterControl.ApplyHighlightOpacity(double opacity)
        {
            string js = $"document.documentElement.style.setProperty('--highlight-band-opacity', '{opacity}');";
            webView.CoreWebView2.ExecuteScriptAsync(js);
        }
        void ITeleprompterControl.ApplyHighlightTop(int percent)
        {
            string js = $"document.documentElement.style.setProperty('--highlight-band-top', '{percent}%');";
            webView.CoreWebView2.ExecuteScriptAsync(js);
        }

        void ITeleprompterControl.ApplyHighlightVisible(bool visible)
        {
            string js = $"document.documentElement.style.setProperty('--highlight-band-visible', '{(visible ? "block" : "none")}')";
            webView.CoreWebView2.ExecuteScriptAsync(js);
        }
        void ITeleprompterControl.ApplyHighlightColor(string color)
        {
            string js = $"document.documentElement.style.setProperty('--highlight-band-color', '{color}');";
            webView.CoreWebView2.ExecuteScriptAsync(js);
        }

        void ITeleprompterControl.ApplyHighlightTriggerPoint(double triggerPoint)
        {
            string js = $"document.documentElement.style.setProperty('--trigger-point-top', '{triggerPoint}%');";
            webView.CoreWebView2.ExecuteScriptAsync(js);
        }

        void ITeleprompterControl.ApplyHighlightBandTriggerPointVisible(bool triggerVisible)
        {
            string js = $"document.documentElement.style.setProperty('--trigger-visible', '{(triggerVisible ? "block" : "none")}')";
            webView.CoreWebView2.ExecuteScriptAsync(js);
        }
        void ITeleprompterControl.ApplyHighlightBandTriggerPointColor(string color)
        {
            string js = $"document.documentElement.style.setProperty('--trigger-color', '{color}');";
            webView.CoreWebView2.ExecuteScriptAsync(js);
        }

        void ITeleprompterControl.ApplyHighlightLines(int lines)
        {
            string js = $"setHighlightBand({lines});";
            webView.CoreWebView2.ExecuteScriptAsync(js);
        }

        void ITeleprompterControl.ApplyTextColor(string color)
        {
            string js = $"document.documentElement.style.setProperty('--text-color', '{color}');";
            webView.CoreWebView2.ExecuteScriptAsync(js);
        }
        void ITeleprompterControl.ApplyBackgroundColor(string color)
        {
            string js = $"document.documentElement.style.setProperty('--background-color', '{color}');";
            webView.CoreWebView2.ExecuteScriptAsync(js);
        }
        void ITeleprompterControl.ApplyAllSettings()
        {
            ApplyAllSettings(); // calls your private MainForm method
        }

        void ITeleprompterControl.SetControlPanelVisible(bool isVisible)
        {
            ((ITeleprompterControl)this).ApplySetControlPanelState(isVisible, SettingsManager.Settings.controlPanelCompressed);
        }

        void ITeleprompterControl.SetControlPanelCompressed(bool compressed)
        {
            ((ITeleprompterControl)this).ApplySetControlPanelState(SettingsManager.Settings.controlPanelVisible, compressed);
        }

        void ITeleprompterControl.ApplySetControlPanelState(bool isVisible, bool isCompressed)
        {
            SettingsManager.Settings.controlPanelVisible = isVisible;
            SettingsManager.Settings.controlPanelCompressed = isCompressed;

            if (isVisible)
            {
                if (isCompressed)
                {
                    pnlControl.Visible = false;
                    pnlCollapsed.Visible = true;
                }
                else
                {
                    pnlControl.Visible = true;
                    pnlCollapsed.Visible = false;
                }
            }
            else
            {
                pnlControl.Visible = false;
                pnlCollapsed.Visible = false;
            }

            rightClickMenu.UpdateControlPanelMenuChecks(isVisible, isCompressed);
        }
        void ITeleprompterControl.ApplyMirrorText(bool mirrorText)
        {
            ApplyMirror(mirrorText);
        }

        void ITeleprompterControl.ApplyAlwaysOnTop(bool alwaysOnTop)
        {
            ApplyAlwaysOnTop(alwaysOnTop);
        }

        void ITeleprompterControl.ApplyScrollSpeed(double speed)
        {
            string jsSpeed = speed.ToString(CultureInfo.InvariantCulture);
            webView.ExecuteScriptAsync($"setSpeed({jsSpeed});");
        }

        void ITeleprompterControl.ConnectSlideShow()
        {
            if (_slides != null)
                _slides.Disconnect();

            _slides = SlideControllerFactory.Create(_selectedEngine);
            _slides.Disconnected += Controller_Disconnected;
            _slides.SlideShowEnded += SlideShowEnd;

            if (!_slides.Connect())
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

            if (_slides != null && _slides.IsSlideShowRunning)
            {
                LoadNotesForCurrentSlide();
                LoadSlideSelectionCombo();
            }
        }

        void ITeleprompterControl.PauseSlideShow()
        {
            PauseTeleprompter();
            UpdateControls();
        }
        void ITeleprompterControl.StopSlideShow()
        {
            StopTeleprompter();
            UpdateControls();
        }
        void ITeleprompterControl.StartSlideShow()
        {
            LockInput();

            if (_slides != null)
                _slides.GoToSlide(cmbStartSlide.SelectedIndex + 1);
            StartTeleprompter();
            UpdateControls();
        }
        void ITeleprompterControl.EndSlideShow()
        {
            StopTeleprompter();
            _slides.EndSlideShow();
        }
        void ITeleprompterControl.CloseApplication()
        {
            Close();
        }

        void ITeleprompterControl.ApplyMainBorderStyle(FormBorderStyle borderStyle)
        {
            this.FormBorderStyle = borderStyle;
            rightClickMenu.UpdateBorderStyleMenuChecks(borderStyle == FormBorderStyle.Sizable);
        }

        void ITeleprompterControl.OpenSpeedSettings()
        {
            OpenSpeedSetting();
        }
        void ITeleprompterControl.OpenHighlightSettings()
        {
            OpenHighlightSetting();
        }
        void ITeleprompterControl.OpenSettings()
        {
            OpenSettings();
        }

        void ITeleprompterControl.LoadSampleScript()
        {
            LoadSampleScript();
        }

        void ITeleprompterControl.LockInput()
        {
            LockInput();
        }

        void ITeleprompterControl.UnlockInput()
        {
            UnlockInput();
        }
        public void MonitorTimerStop()
        {
            _slides.MonitorTimerStop();
        }
    }
}
