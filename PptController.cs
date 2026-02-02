using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PromptLogic.Controllers
{
    public class PptController : IController, IDisposable
    {
        private Thread _pptThread;
        private SynchronizationContext _pptContext;
        private readonly SynchronizationContext _uiContext;
        private ManualResetEvent _threadReady = new ManualResetEvent(false);


        private PowerPointSlideController _ppt;
        bool _isConnected = false;
        public bool IsEnabled => _isConnected;

        public event EventHandler Disconnected;
        public event EventHandler<SlideChangedEventArgs> SlideChanged;
        public event EventHandler SlideShowBegin;
        public event EventHandler SlideShowEnded;
        public event EventHandler TimingsDetected;
        public event Action ConnectToWebView;
        public event Action Connected;

        public PptController() 
        {
            _uiContext = SynchronizationContext.Current;

            _pptThread = new Thread(PptThreadStart);
            _pptThread.SetApartmentState(ApartmentState.STA);
            _pptThread.IsBackground = true;
            _pptThread.Start();

            // Wait until the thread signals that it is ready
            _threadReady.WaitOne();
        }
        public string Name => "ppt";

        public int SlideCount => InvokeOnPptThread(() => _ppt.SlideCount);
        public string GetSlideTitle(int index) { return InvokeOnPptThread(() => _ppt.GetSlideTitle(index)); }
        public string GetNotesForSlide(int index) { return InvokeOnPptThread(() => _ppt.GetNotesForSlide(index)); }
        public string GetNotesForCurrentSlide() { return InvokeOnPptThread(() => _ppt.GetNotesForCurrentSlide()); }
        public bool IsSlideShowRunning() { return InvokeOnPptThread(() => _ppt.IsSlideShowRunning); }
        private void OnSlideChangedInternal(object sender, SlideChangedEventArgs e) { _uiContext.Post(_ => SlideChanged?.Invoke(this, e), null); }
        private void OnSlideShowBeginInternal(object sender, EventArgs e) { _uiContext.Post(_ => { SlideShowBegin?.Invoke(this, EventArgs.Empty); }, null); }
        public void GoToSlide(int index) { InvokeOnPptThread(() => _ppt.GoToSlide(index)); }
        public void Refocus() { InvokeOnPptThread(() => { _ppt.RefocusSlideShowWindow(); }); }
        public void Resume() { InvokeOnPptThread(() => { _ppt.Resume(); }); }
        public void NextSlide() { InvokeOnPptThread(() => { _ppt.NextSlide(); }); }
        public void EndSlideShow() { InvokeOnPptThread(() =>  {_ppt.EndSlideShow(); }); }
        public void MonitorTimerStop() { InvokeOnPptThread(() => { _ppt.MonitorTimerStop(); }); }
        private void PptThreadStart()
        {
            _pptContext = new WindowsFormsSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(_pptContext);

            _ppt = new PowerPointSlideController();
            _ppt.SlideChanged += OnSlideChangedInternal;
            _ppt.SlideShowBegin += OnSlideShowBeginInternal;
            _threadReady.Set();

            Application.Run(); // message pump for COM
        }
        private void InvokeOnPptThread(Action action)
        {
            _pptContext.Post(_ => action(), null);
        }

        public void Enable()
        {
            // Hook Disconnected
            _ppt.Disconnected += (s, e) => { Disconnected?.Invoke(this, EventArgs.Empty); };

            // Connect to PowerPoint
            // Schedule COM work onto the PowerPoint thread
            InvokeOnPptThread(() =>
            {
                if (!_ppt.Connect())
                {
                    _isConnected = false;
                    _uiContext.Post(_ =>
                    {
                        ControllerEvent?.Invoke(this, new ControllerEventArgs
                        {
                            Prefix = "ppt",
                            Type = ControllerEventType.Error,
                            Message = "Could not connect."
                        });
                    }, null);
                    return;
                }

                _isConnected = true;

                //Forward all SlideController events
                _ppt.SlideShowEnded += (s, e) => { SlideShowEnded?.Invoke(this, EventArgs.Empty); };
                _ppt.SlideChanged += (s, e) => { SlideChanged?.Invoke(this, e); };
                _ppt.TimingsDetected += (s, e) => { TimingsDetected?.Invoke(this, EventArgs.Empty); };

                // Handle timings
                if (_ppt.PresentationHasTimings())
                    _uiContext.Post(_ => TimingsDetected?.Invoke(this, EventArgs.Empty), null);

            });

            Connected?.Invoke();

            ConnectToWebView?.Invoke();
        }
        private T InvokeOnPptThread<T>(Func<T> func)
{
    var tcs = new TaskCompletionSource<T>();

    _pptContext.Post(_ =>
    {
        try
        {
            tcs.SetResult(func());
        }
        catch (Exception ex)
        {
            tcs.SetException(ex);
        }
    }, null);

    return tcs.Task.Result; // blocks UI thread safely
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
