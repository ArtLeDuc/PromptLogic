#nullable disable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace PromptLogic.Controllers
{
    public class TxtController : IController
    {
        bool _isConnected = false;
        public bool IsEnabled => _isConnected;
        public string Name => "txt";
        public string _filePath = "";

        public event EventHandler<ControllerEventArgs> ControllerEvent;
        public event Action Ready;
        public event Action<string> NotesChanged;

        public void Enable()
        {
            try
            {
                _isConnected = true;

                Ready?.Invoke();

                Open();
            }
            catch (Exception ex)
            {
                OnControllerEvent("txt", ex.Message, ControllerEventType.Error);
            }
        }
        public void Configure(object config)
        {
            //Empty does nothing
        }

        public void OpenFile(string path)
        {
            _filePath = path;

            if (_isConnected)
                Open();
        }
        private void Open()
        {
            if (string.IsNullOrEmpty(_filePath))
                return;

            string text = File.ReadAllText(_filePath);
            NotesChanged?.Invoke(text);
        }

        public Task ExecuteCommandAsync(string command, string[] args) { return Task.CompletedTask; }
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
        public void Dispose()
        {
            _isConnected = false;
        }

    }
}
