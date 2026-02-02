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

    public class SlideChangedEventArgs : EventArgs
    {
        public int NewSlideIndex { get; }

        public SlideChangedEventArgs(int newSlideIndex)
        {
            NewSlideIndex = newSlideIndex;
        }
    }

    public interface IController : IDisposable
    {
        string Name { get; }
        void Enable();
        Task ExecuteCommandAsync(string command, string[] args);
        bool IsEnabled { get; }

        event EventHandler<ControllerEventArgs> ControllerEvent;

    }
}
