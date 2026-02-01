using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptLogic
{
    public static class SampleScripts
    {
        public static string Default = @".obs_enable(Two Scenes)
.obs_record_start()
.obs_scene(Scene)
.obs_unmute(Crackle)
This is a sample script for testing the teleprompter.
It is written with natural pacing, clear sentences,
and enough variation to help evaluate scrolling behavior.
.obs_scene(Scene 2)
As the text moves, pay attention to how easily your eyes
track each line. Notice whether the highlight band feels
comfortable, and whether the font size supports relaxed reading.
.obs_source_show(Scene 2, Text)
You can adjust the speed, spacing, and colors at any time.
The goal is to find a combination that feels effortless.
.obs_source_hide(Scene 2, Text)
Longer passages help test smooth scrolling, so here is a bit more.
.obs_mute(Crackle)
Imagine you are preparing for a presentation, a tutorial,
or a recorded message. The teleprompter should support your flow,
not distract from it.
.obs_scene(Scene)
If the text feels too fast, slow it down.
If it feels too small, increase the font size.
.obs_source_show(Scene 2, Text)
If the lines feel crowded, adjust the spacing.

This script continues for a little while longer so you can
observe how the teleprompter behaves over time.
.obs_scene(Scene 2)
Consistency is key. Smooth motion, predictable timing,
and comfortable readability all contribute to a better experience.
.obs_unmute(Crackle)
When you are satisfied with the settings, you can replace this
sample text with your actual script. Until then, use this passage
to fine‑tune the teleprompter to your liking.

.pause(5000)
The script will now pause for 5 seconds.
Reset to the beginning of the script and play again.
This loop will continue until you press stop.
.obs_record_stop()
.stop
.start
";

    }
}
