using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptLogic
{
    public enum SlideShowState
    {
        Unknown = 0,
        Running = 1,
        Paused = 2,
        BlackScreen = 3,
        WhiteScreen = 4,
        Done = 5
    }
}
