using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptLogic
{
    public enum ControllerEventType
    {
        Info,
        Warning,
        Error,
        Connected,
        Disconnected
    }

    public class ControllerEventArgs : EventArgs
    {
        public string Prefix { get; set; }
        public string Message { get; set; }
        public ControllerEventType Type { get; set; }
    }


    public interface IController : IDisposable
    {
        void Enable();
        void EnqueueCommand(string command, params string[] args);
        bool IsEnabled { get; }

        event EventHandler<ControllerEventArgs> ControllerEvent;

    }
}
