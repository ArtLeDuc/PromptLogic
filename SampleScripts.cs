using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teleprompter
{
    public static class SampleScripts
    {
        public static string Default = @"This is a sample script for testing the teleprompter.
It is written with natural pacing, clear sentences,
and enough variation to help evaluate scrolling behavior.

As the text moves, pay attention to how easily your eyes
track each line. Notice whether the highlight band feels
comfortable, and whether the font size supports relaxed reading.

You can adjust the speed, spacing, and colors at any time.
The goal is to find a combination that feels effortless.

Longer passages help test smooth scrolling, so here is a bit more.
Imagine you are preparing for a presentation, a tutorial,
or a recorded message. The teleprompter should support your flow,
not distract from it.

If the text feels too fast, slow it down.
If it feels too small, increase the font size.
If the lines feel crowded, adjust the spacing.

This script continues for a little while longer so you can
observe how the teleprompter behaves over time.
Consistency is key. Smooth motion, predictable timing,
and comfortable readability all contribute to a better experience.

When you are satisfied with the settings, you can replace this
sample text with your actual script. Until then, use this passage
to fine‑tune the teleprompter to your liking.
.stop
";

        public static string breakTest = @"Welcome to the Teleprompter Test Script.
This script simulates real PowerPoint notes.

Here is a normal paragraph break.

This is a long line intended to wrap naturally across multiple visual lines in the teleprompter window. It should confirm that line-height is consistent and that wrapped lines do not introduce unexpected spacing.
This line is created with Shift+Enter to simulate a line break.
This is another Shift+Enter line break.

Now we test custom spacing commands.
B1:
This line follows a break1 block.
B2:
This line follows a break2 block.
B3:
This line follows a break3 block.

Now we test dot-commands.
.pause(2)
This line should appear after a two-second pause.
.marker
This line should register a marker command.

Now we test consecutive line breaks.
Line A
Line B
Line C

Now we test consecutive paragraph breaks.


This line follows two empty paragraphs.

Final paragraph to confirm runway behavior.";
    }

}
