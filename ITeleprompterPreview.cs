using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teleprompter
{
    public interface ITeleprompterPreview
    {
        void ApplyFont(string fontName);
        void ApplyFontSize(int size);
        void ApplyLineSpacing(double spacing);
        void ApplyHighlightHeight(int px);
        void ApplyHighlightOpacity(double opacity);
        void ApplyHighlightTop(int percent);
        void ApplyTextColor(string color);
        void ApplyBackgroundColor(string color);
        void ApplyAllSettings();

    }
}
