##[0.2.0] - Unreleased<br>
Added
- Multi?argument command support for script commands
- Controller pattern introduced across the app
- OBS Controller
- .obs_enable(<scene collection>) — initialize OBS and load scene collection
- .obs_mute(<source>) — mute a source
- .obs_unmute(<source>) — unmute a source
- .obs_scene(<scene>) — switch scenes
- .obs_record_start / .obs_record_stop
- .obs_source_show(<scene>, <source>) — show a source within a scene
- .obs_source_hide(<scene>, <source>) — hide a source within a scene
- .obs_transition(<type>, <duration>) — set transition type and duration
- PowerPoint Controller (PptController)
- New dedicated command layer for all PowerPoint operations
- Centralized slide navigation, slideshow control, and event handling

Changed
- Moved all PowerPoint COM automation to a dedicated STA thread
- Prevents UI freezes
- Ensures deterministic COM execution
- Cleanly marshals events back to UI thread
- Merged PowerPointSlideController into PptController
- Removed legacy controller
- Unified PowerPoint logic under one module
- Repaired WebView2 command routing after merge
- Refactored PowerPoint operations into modular controller structure
- Extracted logic from MainForm
- Eliminated recursive event wiring and reentrancy traps

Fixed
- Resolved all remaining COM reference issues
- Verified all RCWs release cleanly, including Application
- Eliminated hidden RCWs created by slideshow operations
- Hardened MonitorTimer_Tick with leak?proof COM access pattern
- Improved shutdown reliability
- Clarified PowerPoint’s internal timing behavior
- Ensured deterministic teardown of slideshow and COM objects
- Corrected WebView2 command flow after controller merge

Improved
- Overall automation stability for PowerPoint and OBS
- Event pipeline reliability
- Ensured all PowerPoint events marshal safely
- Removed cross?thread UI hazards
- Documentation clarity
- Documented expected PowerPoint “dirty state” behavior when exiting slideshow
- Improved internal notes around COM timing and shutdown



##[0.1.0] - 2026-01-20
### Added
- Initial public release
- Teleprompter engine
	- Only supporting Power Point slides
- Dot-command parser
	- .pause<n>
	Will pause scrolling for the specified time. Setting it to 0 will pause perminately and require the resume button to continue
	- .stop
	Will stop scrolling and reset the engine to the begining of the current script
	- .nextslide
	Will move to the next slide in the presentation.  With powerpoint slides this will cause the notes from the next slide to be loaded. 
	- .start
	Will start the engine scrolling  This command is only useful if you want to repeat the slide that is currently scrolling
	