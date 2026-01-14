using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Office = Microsoft.Office.Core;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;


namespace Teleprompter
{
    public class WebMessageService
    {
        private ISlideController _slides;
        private readonly IWebViewActions _ui;

        public WebMessageService(IWebViewActions ui)
        {
            _ui = ui;
        }

        public void Handler(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            var obj = JObject.Parse(e.WebMessageAsJson);
            string action = (string)obj["action"];

            switch (action)
            {
                case "unpauseSlideshow":
                    UnpauseSlideShowWindow();
                    break;
                case "refocusSlideshow":
                    RefocusSlideShowWindow();
                    break;
                case "nextSlide":
                    _slides?.NextSlide();
                    break;
                case "pause":
                 
                    int duration = obj["duration"]?.Value<int>() ?? 0;
                    //if duration is > 0 we will just send it down to JS to pause loop
                    //if 0 or < we need to send it to the mainform to pause that updates all the controls
                    if (duration > 0)
                        _ui.SendToWebView(Newtonsoft.Json.JsonConvert.SerializeObject(new { action = "pause", duration }));
                    else
                        _ui.PauseSlideShow();
                    break;
                case "stop":
                    _ui.ExecuteScriptAsync("stopScroll()");
                    break;

                    // future commands:
                    // case "resume":
                    // case "speed":
                    // case "goto":
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
