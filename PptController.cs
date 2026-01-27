using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptLogic.Controllers
{
    public class PptController : IController, IDisposable
    {
        private readonly PowerPointSlideController _ppt;
        bool _isConnected = false;
        public bool IsEnabled => _isConnected;
        public PptController() 
        {
            _ppt = new PowerPointSlideController();
        }
        public string Name => "ppt";
        public void Enable() { }
        public Task ExecuteCommandAsync(string command, string[] args) { return Task.CompletedTask; }
        public void Dispose() { }

        public event EventHandler<ControllerEventArgs> ControllerEvent;
    }
}
