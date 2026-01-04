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

    }
}
