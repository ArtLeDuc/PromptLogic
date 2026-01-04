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
        void ApplyTextColor(string color);
        void ApplyBackgroundColor(string color);
        void ApplyLineSpacing(int spacing);
        void ApplyParagraphSpacing(int spacing);
        void ApplyBreakSpacing1(int spacing);
        void ApplyBreakSpacing2(int spacing);
        void ApplyBreakSpacing3(int spacing);
        void ApplyHighlightVisible(bool visible);
        void ApplyHighlightHeight(int px);
        void ApplyHighlightOpacity(double opacity);
        void ApplyHighlightTop(int percent);
        void ApplyHighlightColor(string color);
        void ApplyHighlightLines(int lines);
        void ApplyHighlightTriggerPoint(double triggerPoint);
        void ApplyAllSettings();
    }
}
