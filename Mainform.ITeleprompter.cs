using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teleprompter
{
    public partial class MainForm : ITeleprompter
    {
        private string _lastNotesSent = null;
        private bool isPaused = false;
        private bool isStopped = true;

        public void ExecuteScriptAsync(string script)
        {
            webView.ExecuteScriptAsync(script);
        }

        public void SendNotesToWebView(string notes)
        {
            if (notes == _lastNotesSent)
                return;
            _lastNotesSent = notes;

            this.Invoke((Action)(() =>
            {
                // Escape for JS
                string escaped = notes
                    .Replace("\\", "\\\\")
                    .Replace("\"", "\\\"")
                    .Replace("\n", "\\n")
                    .Replace("\r", "\\r")
                    .Replace("\v", "\\v");
                webView.ExecuteScriptAsync($"loadNotes(\"{escaped}\")");
            }));
        }

        public void StartTeleprompter()
        {
            isStopped = false;

            double speedValue = SettingsManager.Settings.ScrollSpeed / 100.0;
            webView.ExecuteScriptAsync($"setSpeed({speedValue});");

            webView.ExecuteScriptAsync("startTeleprompter()");
        }
        public void PauseTeleprompter()
        {
            if (isPaused)
            {
                // Currently paused → resume scrolling
                webView.ExecuteScriptAsync("startScroll()");
                isPaused = false;

            }
            else
            {
                // Currently scrolling → pause it
                webView.ExecuteScriptAsync("pauseScroll()");
                isPaused = true;

            }
        }
        public void StopTeleprompter()
        {

            webView.ExecuteScriptAsync("stopScroll()");
            isPaused = false;
            isStopped = true;
        }

        public void SetManualScrolling()
        {
            webView.CoreWebView2.ExecuteScriptAsync(
                $"document.getElementById('scrollContainer').style.overflowY = '{(isStopped ? "auto" : "hidden")}';"
            );

        }

        public void ScrollByWheel(int delta)
        {
            webView.CoreWebView2.ExecuteScriptAsync(
                $"scrollByWheel({{ delta: {delta} }});"
            );
        }

    }
}
