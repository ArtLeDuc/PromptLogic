using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teleprompter
{
    public static class SlideControllerFactory
    {
        public static ISlideController Create(SlideEngine engine)
        {
            switch (engine)
            {
                case SlideEngine.PowerPoint:
                    return new PowerPointSlideController();

//                case SlideEngine.OBS:
//                    return new OBSSlideController();

                // Future engines go here:
                // case SlideEngine.PDF:
                //     return new PDFSlideController();

                default:
                    throw new NotSupportedException($"Engine {engine} is not supported.");
            }
        }
    }
}
