using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Teleprompter
{
    public interface ITeleprompter
    {
        void SendNotesToWebView(string notes);
        void StartTeleprompter();
        void PauseTeleprompter();
        void StopTeleprompter();
        void SetManualScrolling();
        void ScrollByWheel(int delta);
        void ExecuteScriptAsync(string script);
    }
}
