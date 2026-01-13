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
using Teleprompter;

namespace Teleprompter
{
    public partial class MainForm : ITeleprompterPreview
    {
        void ITeleprompterPreview.ApplyFont(string fontName)
        {
            string safe = fontName.Replace("'", "\\'");
            string js = $"document.documentElement.style.setProperty('--font-family', '{safe}, sans-serif');";
            webView.CoreWebView2.ExecuteScriptAsync(js);
        }

        void ITeleprompterPreview.ApplyFontSize(int size)
        {
            string js = $"document.documentElement.style.setProperty('--font-size', '{size}px');";
            webView.CoreWebView2.ExecuteScriptAsync(js);
        }
        void ITeleprompterPreview.ApplyLineSpacing(int spacing)
        {
            string js = $"document.documentElement.style.setProperty('--line-spacing', '{spacing}px');";
            webView.CoreWebView2.ExecuteScriptAsync(js);
        }
        void ITeleprompterPreview.ApplyParagraphSpacing(int spacing)
        {
            string js = $"document.documentElement.style.setProperty('--paragraph-spacing', '{spacing}px');";
            webView.CoreWebView2.ExecuteScriptAsync(js);
        }
        void ITeleprompterPreview.ApplyBreakSpacing1(int spacing)
        {
            string js = $"document.documentElement.style.setProperty('--break-spacing1', '{spacing}px');";
            webView.CoreWebView2.ExecuteScriptAsync(js);
        }
        void ITeleprompterPreview.ApplyBreakSpacing2(int spacing)
        {
            string js = $"document.documentElement.style.setProperty('--break-spacing2', '{spacing}px');";
            webView.CoreWebView2.ExecuteScriptAsync(js);
        }
        void ITeleprompterPreview.ApplyBreakSpacing3(int spacing)
        {
            string js = $"document.documentElement.style.setProperty('--break-spacing3', '{spacing}px');";
            webView.CoreWebView2.ExecuteScriptAsync(js);
        }

        void ITeleprompterPreview.ApplyHighlightHeight(int px)
        {
            string js = $"document.documentElement.style.setProperty('--highlight-band-height', '{px}px');";
            webView.CoreWebView2.ExecuteScriptAsync(js);
        }
        void ITeleprompterPreview.ApplyHighlightOpacity(double opacity)
        {
            string js = $"document.documentElement.style.setProperty('--highlight-band-opacity', '{opacity}');";
            webView.CoreWebView2.ExecuteScriptAsync(js);
        }
        void ITeleprompterPreview.ApplyHighlightTop(int percent)
        {
            string js = $"document.documentElement.style.setProperty('--highlight-band-top', '{percent}%');";
            webView.CoreWebView2.ExecuteScriptAsync(js);
        }

        void ITeleprompterPreview.ApplyHighlightVisible(bool visible)
        {
            string js = $"document.documentElement.style.setProperty('--highlight-band-visible', '{(visible ? "block":"none")}')";
            webView.CoreWebView2.ExecuteScriptAsync(js);
        }
        void ITeleprompterPreview.ApplyHighlightColor(string color)
        {
            string js = $"document.documentElement.style.setProperty('--highlight-band-color', '{color}');";
            webView.CoreWebView2.ExecuteScriptAsync(js);
        }

        void ITeleprompterPreview.ApplyHighlightTriggerPoint(double triggerPoint)
        {
            string js = $"document.documentElement.style.setProperty('--trigger-point-top', '{triggerPoint}%');";
            webView.CoreWebView2.ExecuteScriptAsync(js);
        }

        void ITeleprompterPreview.ApplyHighlightBandTriggerPointVisible(bool triggerVisible)
        {
            string js = $"document.documentElement.style.setProperty('--trigger-visible', '{(triggerVisible ? "block" : "none")}')";
            webView.CoreWebView2.ExecuteScriptAsync(js);
        }
        void ITeleprompterPreview.ApplyHighlightBandTriggerPointColor(string color)
        {
            string js = $"document.documentElement.style.setProperty('--trigger-color', '{color}');";
            webView.CoreWebView2.ExecuteScriptAsync(js);
        }
        void ITeleprompterPreview.ApplyMainBorderStyle(FormBorderStyle borderStyle)
        {
            this.FormBorderStyle = borderStyle;
        }

        void ITeleprompterPreview.ApplyHighlightLines(int lines)
        {
            string js = $"setHighlightBand({lines});";
            webView.CoreWebView2.ExecuteScriptAsync(js);
        }

        void ITeleprompterPreview.ApplyTextColor(string color)
        {
            string js = $"document.documentElement.style.setProperty('--text-color', '{color}');";
            webView.CoreWebView2.ExecuteScriptAsync(js);
        }
        void ITeleprompterPreview.ApplyBackgroundColor(string color)
        {
            string js = $"document.documentElement.style.setProperty('--background-color', '{color}');";
            webView.CoreWebView2.ExecuteScriptAsync(js);
        }
        void ITeleprompterPreview.ApplyAllSettings()
        {
            ApplyAllSettings(); // calls your private MainForm method
        }

        void ITeleprompterPreview.ApplyShowControlSidebar(bool bShowControlSidebar)
        {
            if (bShowControlSidebar)
            {
                if (SettingsManager.Settings.IsCollapsed == true)
                    pnlCollapsed.Visible = true;
                else
                    pnlControl.Visible = true;

            }
            else
            {
                pnlControl.Visible = false;
                pnlCollapsed.Visible = false;
            }
        }
        void ITeleprompterPreview.ApplyMirrorText(bool mirrorText)
        {
            ApplyMirror(mirrorText);
        }

        void ITeleprompterPreview.ApplyWindowStyles(bool alwaysOnTop, bool nonActivating)
        {
            ApplyWindowStyles(alwaysOnTop, nonActivating);
        }

        void ITeleprompterPreview.ApplyScrollSpeed(double speed)
        {
            string jsSpeed = speed.ToString(CultureInfo.InvariantCulture);
            webView.ExecuteScriptAsync($"setSpeed({jsSpeed});");
        }
    }
}
