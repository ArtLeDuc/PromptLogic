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

        public event Action NextSlide;
        public event Action Resume;
        public event Action Refocus;
        public event Action EndSlideShow;
        public event Action StartSlideShow;
        public event Action UnlockInput;
        public event Action<int> Pause;

        public event Action<string> ObsEnable;
        public event Action<string> PptEnable;
        public event Action<string, string[]> ObsCommandRequested;

        public WebMessageService()
        {
            _handlers = new Dictionary<string, Action<JObject>>(StringComparer.OrdinalIgnoreCase)
            {
                ["refocusSlideshow"] = _ => Refocus?.Invoke(),
                ["nextSlide"] = _ =>
                {
                    NextSlide?.Invoke();
                },
                ["pause"] = obj => 
                {
                    string[] args = obj["args"]?.ToObject<string[]>() ?? Array.Empty<string>();
                    int duration = int.TryParse(args?[0], out var value) ? value : 0;
                    Pause?.Invoke(duration); 
                },
                ["stop"] = _ => EndSlideShow?.Invoke(),
                ["start"] = _ => StartSlideShow?.Invoke(),
                ["scrollStarted"] = _ => { UnlockInput?.Invoke(); },
                ["ppt_enable"] = obj =>
                {
                    string? filePath = (string?)obj["argument"] ?? "";
                    PptEnable?.Invoke(filePath);
                },
                ["obs_enable"] = obj => { string? sceneCollection = (string?)obj["argument"] ?? ""; ObsEnable?.Invoke(sceneCollection); },
                ["obs_mute"] = RequestObsCommand,
                ["obs_unmute"] = RequestObsCommand,
                ["obs_scene"] = RequestObsCommand,
                ["obs_record_start"] = RequestObsCommand,
                ["obs_record_stop"] = RequestObsCommand,
                ["obs_source_show"] = RequestObsCommand,
                ["obs_source_hide"] = RequestObsCommand,
                ["obs_transition"] = RequestObsCommand
            };
        }

        private void RequestObsCommand(JObject obj)
        {
            string? command = (string?)obj["action"];
            string[] args = obj["args"]?.ToObject<string[]>() ?? Array.Empty<string>();
            ObsCommandRequested?.Invoke(command, args);
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
    }
}
