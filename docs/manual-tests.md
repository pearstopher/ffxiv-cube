# Manual tests

Only what the in-game test bridge can't check (`docs/in-game-testing.md`). Optional, before a release. Report results in the format from CLAUDE.md.

## Settings window (#20)

The bridge checks every setting changes the frame and that saving writes the file, but it can't click the controls.

1. Type `/cube config`: the settings window opens. `/cube` still shows and hides the cube.
2. Drag the cube size slider: the cube changes while dragging.
3. Change a colour, untick "Show names", switch to "North up", lower the opacity: each shows in the cube at once.
4. Reload the plugin in `/xlplugins`: the changed settings are still there.
5. Click "Reset to defaults": everything goes back (40 yalms, Camera, names on, blue/green/orange, full opacity).
