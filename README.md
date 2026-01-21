# PromptLogic
A modular, maker‑friendly teleprompter engine for educators, creators, and performers.
PromptLogic is a Windows‑native teleprompter designed with clarity, predictability, and teachability at its core. It’s built for people who want full control over their workflow - from educators running live sessions to creators recording video to performers reading scripts with confidence.
This project is part of Vermont Creative Technologies, LLC, and reflects a philosophy of modular engineering, transparent architecture, and long‑term maintainability.

## **User Guide:**  https://artleduc.github.io/PromptLogic/userguide.html

## Features
- Modular architecture - predictable, teachable, and easy to understand
- Precise scrolling engine - deterministic behavior in both run and edit modes
- Overlay‑based window controls - clean resizing, dragging, and interaction
- Maker‑friendly design - built for people who value clarity over magic
- Actively evolving - new UI refinements, workflow improvements, and community‑driven features

## Why PromptLogic?
PromptLogic grew out of a simple need: I couldn’t find a teleprompter that matched the way I think and work. Most tools were either too opaque, too automated, or too rigid for the kind of predictable, maker‑friendly workflow I wanted.
So PromptLogic was built around a different philosophy - one centered on clarity, control, and long‑term maintainability.
It’s designed for educators, creators, and performers who appreciate:
- Predictable behavior - the script moves exactly when and how you expect
- A clear mental model - no hidden logic or surprising side effects
- A tool you can trust - stable, teachable, and easy to understand
- Full workflow control - from display modes to dot‑command scripting
- A foundation that grows with you - modular, maintainable, and built for extension.<br>
PromptLogic isn’t a reaction to other teleprompters.
It’s simply the tool I wanted but couldn’t find - and now it’s here for anyone who values the same principles.

## Getting Started
PromptLogic is currently in active development.
Binary releases will be published here once the first public build is ready.
For now, you can:
- Follow development in the repository
- Join the community discussions
- Report bugs or request features
- Share your workflow needs and ideas

## Installation
PromptLogic is distributed as a lightweight Windows application.
You can install it using the standard installer or run it as a portable app.

Option 1: Installer (recommended)
- Download the latest PromptLogic‑Setup.exe from the Releases page.
- Run the installer.
- No administrator permissions are required.
- PromptLogic installs into your AppData\Local folder so it can update and run without UAC prompts.
- Launch PromptLogic from the Start Menu under PromptLogic.

Option 2: Portable Edition<br>
A portable ZIP package is also available for users who prefer a self‑contained folder.
- Download PromptLogic_Portable.zip from the release.
- Extract it anywhere you like.
- Run PromptLogic.exe.  
  Note: The portable version does not auto‑update and may require the WebView2 Runtime if it’s not already installed on your system.

## Display Modes
PromptLogic supports multiple presentation modes depending on your workflow:
- Full Control Panel - all controls visible
![Full control panel](docs/Images/PromptLogic%20Main%20Window.png)

- Compressed Panel - minimal controls, more screen space
![Compressed control panel](docs/Images/PromptLogic%20Compressed%20Control%20Panel.png)

- No Panel - right‑click menu only
![Hidden control panel](docs/Images/PromptLogic%20With%20No%20Control%20Panel.png)

- Borderless Mode - clean, distraction‑free teleprompter window
![Borderless](docs/Images/PromptLogic%20Borderless%20display.png)

  These modes can be switched at any time during operation.

## Running PromptLogic
Start Menu<br>
- Open the Start Menu and search for PromptLogic.<br>

Portable Edition<br>
- Double‑click PromptLogic.exe from the extracted folder.

First‑Run Behavior
On first launch, PromptLogic will:
- Open in Full Control Panel mode
- Detect your system DPI and scale accordingly
- Load a sample script (depending on build configuration)
No configuration is required to begin using the teleprompter.

## Known Limitations (rev 0.1)
PromptLogic integrates with PowerPoint for slide tracking and notes‑based scripting.
However, PowerPoint’s automation layer can temporarily block events during animations that include audio.
During slides with sound effects or embedded audio, you may notice:
- Delayed dot‑command execution
- Brief pauses in UI responsiveness
- Slight stuttering in scroll timing
  
This is a limitation of PowerPoint’s COM model, not PromptLogic.
Recommendation for the smoothest experience, avoid:
- Slide animations with audio
- Auto‑play sound effects
- Media‑triggered transitions.<br>
  PromptLogic will continue running but command timing may be affected.

## Version Information
rev 0.1 - First Public Release
- Core teleprompter engine
- Deterministic scrolling
- Dot‑command scripting
- Multiple display modes
- PowerPoint integration (notes‑based)
- Window overlays for resizing and interaction
- Initial user guide and documentation

## Roadmap
PromptLogic is early in its journey, and the foundation is intentionally simple, stable, and modular. The roadmap focuses on expanding capability without compromising predictability.

Near‑Term (0.2 – 0.4)<br>
Focused on strengthening the core experience and smoothing out early workflows.
- Native support for text‑based scripts, allowing creators to load .txt or .md files as teleprompter scripts 
- Additional dot‑command features
- Improved script editor ergonomics
- More display‑mode refinements
- Optional auto‑save and recovery
- Better PowerPoint integration
- OBS WebSocket integration (scene switching, recording control)
- Expanded user guide and examples

Mid‑Term (0.5 – 0.8)<br>
  Expanding flexibility and customization for creators with more complex setups.
- Script library and quick‑switching
- Customizable themes and color profiles
- Presenter notes overlay
- Multi‑monitor workflow improvements
- OBS status feedback and two‑way communication
- Optional “presentation mode” with simplified UI

Long‑Term (1.0 and beyond)<br>
  Larger architectural enhancements that open the door to advanced workflows and integrations.
- Plugin architecture
- Network‑controlled teleprompter mode
- Remote operator console
- Deep OBS plugin integration
- Scripting API for automation
- Cross‑platform exploration

## Community & Support
PromptLogic has a few clear “doors” depending on what you need:
- Found a bug?<br>Open a Bug Report in the Issues tab.
- Have an idea or feature request?<br>Use Issues → Feature Request<br>or<br>Start a conversation in Discussions → Ideas & Suggestions<br>
- Need help or have a question?<br>Visit Discussions → Help & How‑To
- Want to share feedback, praise, or stories?<br>Use Discussions → General Chat or Show & Tell / Stories
- Not comfortable with GitHub?<br>Use the [Feedback Form](https://forms.gle/tt4MfUo9thVvdVgXA)
  <br>All major feedback from the form will be reviewed and added to GitHub when appropriate.<br>
  For more details, see [SUPPORT.md](.github/SUPPORT.md).

## Contributing
Contributions are welcome - from bug reports to feature ideas to architectural discussions.<br>
Before contributing, please read:
  - [CONTRIBUTING.md](CONTRIBUTING.md)
  - [SUPPORT.md](.github/SUPPORT.md) for how to get help
  - [Issue templates](https://github.com/ArtLeDuc/PromptLogic/issues) for structured reports<br>
PromptLogic values clarity, modularity, and predictable behavior.<br>
Contributions that align with these principles are especially appreciated.

