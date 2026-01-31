##[0.2.0] - Unreleased
### Added
- Multi-Argument command support
- Introduced Controller pattern
	- OBS Basic OBS integration 
		- Created the OBS Controller
		- .obs_enable(<scene collection>)
		<br>Should be placed at the begining of a script used to load OBS if necessary and load the scene collection 
		- .obs_mute(<source>)
		<br>Mutes the specified source
		- .obs_unmute(<source>)
		<br>Un-Mutes the specified source
		- .obs_scene(<scene>)
		<br>Selects the specified scene
		- .obs_record_start
		- .obs_record_stop
		- .obs_source_show(<sceneName>, <sourceName>)
		<br>Will show the item specified in the sceneName, sourceName within the scene.  You will need to use .obs_scene to show the scene
		- .obs_source_hide(<sceneName>, <sourceName>)
		<br>Will nide the item specified in the sceneName, sourceName within the scene.  You will need to use .obs_scene to show the scene
		- .obs_transition(<type>, <duration>)
		<br>Selects the transition type and duration.  If you set the duration you may need to also issue a .pause(n) to wait for the transition to happen.
	- Introduced the Power Point Controller
		- Move PowerPoint command handling into PptController
		- Extract PowerPoint logic from MainForm into dedicated controller
		- Introduce PptController as the new PowerPoint command layer
		- Refactor PowerPoint operations into modular controller
		- Centralize PowerPoint slide control in PptController

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
	