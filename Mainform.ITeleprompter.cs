using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptLogic
{
    public partial class MainForm : ITeleprompter
    {
        private string _lastNotesSent = null;
        public bool IsStopped { get; private set; } = true;
        public bool IsPaused { get; private set; } = false;
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
            IsStopped = false;

            double speedValue = SettingsManager.Settings.ScrollSpeed / 100.0;
            webView.ExecuteScriptAsync($"setSpeed({speedValue});");

            webView.ExecuteScriptAsync("startTeleprompter()");
        }
        public void PauseTeleprompter()
        {
            if (IsPaused)
            {
                // Currently paused → resume scrolling
                webView.ExecuteScriptAsync("startScroll()");
                IsPaused = false;
            }
            else
            {
                // Currently scrolling → pause it
                webView.ExecuteScriptAsync("pauseScroll()");
                IsPaused = true;
            }
        }
        public void StopTeleprompter()
        {
            webView.ExecuteScriptAsync("stopScroll()");
            IsPaused = false;
            IsStopped = true;
        }

        public void SetManualScrolling()
        {
            webView.CoreWebView2.ExecuteScriptAsync(
                $"document.getElementById('scrollContainer').style.overflowY = '{(IsStopped ? "auto" : "hidden")}';"
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
