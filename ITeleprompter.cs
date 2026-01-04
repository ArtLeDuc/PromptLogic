using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teleprompter
{
    public interface ITeleprompter
    {
        void SendNotesToWebView(string notes);
    }
}
