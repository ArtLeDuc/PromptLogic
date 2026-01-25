using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Office = Microsoft.Office.Core;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;


namespace PromptLogic
{
    public class WebMessageService
    {
        private readonly Dictionary<string, Action<JObject>> _handlers;

        private ISlideController _slides;
        private readonly IWebViewActions _ui;

        public WebMessageService(IWebViewActions ui)
        {
            _ui = ui;
            _handlers = new Dictionary<string, Action<JObject>>(StringComparer.OrdinalIgnoreCase)
            {
                ["unpauseSlideshow"] = _ => UnpauseSlideShowWindow(),
                ["refocusSlideshow"] = _ => RefocusSlideShowWindow(),
                ["nextSlide"] = _ => _slides?.NextSlide(),
                ["pause"] = HandlePause,
                ["stop"] = _ => _ui.ExecuteScriptAsync("stopScroll()"),
                ["start"] = _ => ((ITeleprompterControl)_ui).StartSlideShow(),
                ["scrollStarted"] = _ => ((ITeleprompterControl)_ui).UnlockInput(),
                ["obs_enable"] = ObsEnable,
                ["obs_mute"] = ExecuteCommand,
                ["obs_unmute"] = ExecuteCommand,
                ["obs_scene"] = ExecuteCommand,
                ["obs_record_start"] = ExecuteCommand,
                ["obs_record_stop"] = ExecuteCommand,
                ["obs_source_show"] = ExecuteCommand,
                ["obs_source_hide"] = ExecuteCommand,
                ["obs_trasnition"] = ExecuteCommand
            };
        }

        private void ExecuteCommand(JObject obj)
        {
            string command = (string)obj["action"];
            string[] args = obj["args"]?.ToObject<string[]>() ?? Array.Empty<string>();

            Task.Run(() =>
                ((ITeleprompterControl)_ui)
                    .ExecuteControllerCommand("obs", command, args)
            );
        }
        private void ObsEnable(JObject obj)
        {
            string sceneCollection = (string)obj["argument"];
            ((ITeleprompterControl)_ui).EnableController("obs", sceneCollection);
        }

        private void HandlePause(JObject obj)
        {
            string[] args = obj["args"]?.ToObject<string[]>() ?? Array.Empty<string>();
            int duration = int.TryParse(args?[0], out var value) ? value : 0;

            if (duration > 0)
                _ui.SendToWebView(JsonConvert.SerializeObject(new { action = "pause", duration }));
            else
                ((ITeleprompterControl)_ui).PauseSlideShow();
        }

        public void Handler(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            var obj = JObject.Parse(e.WebMessageAsJson);
            string action = (string)obj["action"];

            if (_handlers.TryGetValue(action, out var handler))
            {
                handler(obj);
            }
            else
            {
                // Optional: log unknown command
            }
        }

        void UnpauseSlideShowWindow() => _slides?.Resume();

        private void RefocusSlideShowWindow() => _slides?.RefocusSlideShowWindow();

        public void SetSlideController(ISlideController slides)
        {
            _slides = slides;

            if (slides != null)
            {
                // Subscribe to controller events
                _slides.SlideChanged += OnSlideChanged;
            }
        }

        private void OnSlideChanged(int index)
        {
            _ui.OnSlideChanged(index);
        }

    }
}
