using System;
using System.Collections.Generic;
using System.Linq;
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

        void ITeleprompterPreview.ApplyLineSpacing(double spacing)
        {
            string js = $"document.documentElement.style.setProperty('--line-spacing', '{spacing}');";
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

    }
}