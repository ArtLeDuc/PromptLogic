using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptLogic.Controllers
{
    public class PptController : IController, IDisposable
    {
        public event EventHandler Disconnected;

        private readonly PowerPointSlideController _ppt;
        bool _isConnected = false;
        public ISlideController SlideController => (ISlideController)_ppt;
        public bool IsEnabled => _isConnected;

        public event Action<int> SlideChanged;
        public event EventHandler SlideShowBegin;
        public event EventHandler SlideShowEnded;
        public event EventHandler TimingsDetected;
        public event Action ConnectToWebView;
        public event Action Connected;

        public PptController() 
        {
            _ppt = new PowerPointSlideController();
        }
        public string Name => "ppt";

        public int SlideCount => _ppt.SlideCount;
        public string GetSlideTitle(int index) { return _ppt.GetSlideTitle(index); }
        public string GetNotesForSlide(int index) { return _ppt.GetNotesForSlide(index); }
        public void Enable() 
        {
            // Hook Disconnected
            _ppt.Disconnected += (s, e) => { Disconnected?.Invoke(this, EventArgs.Empty); };

            // Connect to PowerPoint
            if (!_ppt.Connect())
            {
                _isConnected = false;
                ControllerEvent?.Invoke(this, new ControllerEventArgs{ Prefix = "ppt", Type = ControllerEventType.Error, Message = "Could not connect." });
                return;
            }

            _isConnected = true;

            //Forward all SlideController events
            _ppt.SlideShowBegin += (s, e) => { SlideShowBegin?.Invoke(this, EventArgs.Empty); };
            _ppt.SlideShowEnded += (s, e) => { SlideShowEnded?.Invoke(this, EventArgs.Empty); };
            _ppt.SlideChanged += index => { SlideChanged?.Invoke(index); };
            _ppt.TimingsDetected += (s, e) => { TimingsDetected?.Invoke(this, EventArgs.Empty); };

            ConnectToWebView?.Invoke();
            // Handle timings
            if (_ppt.PresentationHasTimings())
                TimingsDetected?.Invoke(this, EventArgs.Empty);

            Connected?.Invoke();
        }
        public Task ExecuteCommandAsync(string command, string[] args) { return Task.CompletedTask; }
        public void Dispose() { }

        public event EventHandler<ControllerEventArgs> ControllerEvent;
        protected virtual void OnControllerEvent(string prefix, string message, ControllerEventType type)
        {
            var handler = ControllerEvent;
            if (handler != null)
            {
                handler(this, new ControllerEventArgs
                {
                    Prefix = prefix,
                    Message = message,
                    Type = type
                });
            }
        }
        public void ClearAllTimings()
        {
            _ppt.ClearAllTimings();
        }
    }
}
