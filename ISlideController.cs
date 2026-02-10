using Microsoft.Office.Interop.PowerPoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static PromptLogic.Controllers.PptController;

namespace PromptLogic
{
    public interface ISlideController
    {
//        bool Connect(); // (IWebViewActions ui);
        void Disconnect();

        int SlideCount { get; }
        int CurrentSlide { get; }
        bool IsSlideShowRunning { get; }
        void NextSlide();
        void PreviousSlide();
        void GoToSlide(int index);
        string GetNotes(int index);
        void Pause();
        void Resume();
        void RefocusSlideShowWindow();
        bool PresentationHasTimings();  //This will return true if the implementation has timings.
                                        //In powerpoint the timings can get in the way of going to the next slide.
        void ClearAllTimings();         //So we will clear the timings in this call.  If the implementation doesn't
                                        //contain timings we will just do nothing in this call.
        void HookSlideShowEvents();
        string GetNotesForCurrentSlide();
        string GetNotesForSlide(int index);
        SlideShowState State { get; }
        string GetSlideTitle(int index);
        void MonitorTimerStop();
        void EndSlideShow();
        bool SupportsNotes { get; set; }

        // Events
        event EventHandler<SlideChangedEventArgs> SlideChanged;
        event EventHandler SlideShowBegin;
        event EventHandler Disconnected;
        event EventHandler SlideShowEnded;
    }
}
