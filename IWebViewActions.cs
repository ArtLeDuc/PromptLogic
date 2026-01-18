using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptLogic
{
    public interface IWebViewActions
    {
        void SendToWebView(string message);
        void ExecuteScriptAsync(string script);
        void OnSlideChanged(int index);
        void InvokeOnUIThread(Action action);
        void LoadInitialPage();
    }
}
