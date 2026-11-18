# Manual tests

Only what the in-game test bridge can't check (`docs/in-game-testing.md`). Optional, before a release. Report results in the format from CLAUDE.md.

## Settings window (#20)

The bridge checks every setting changes the frame and that saving writes the file, but it can't click the controls.

1. Type `/cube config`: the settings window opens. `/cube` still shows and hides the cube.
2. Drag the cube size slider: the cube changes while dragging.
3. Change a colour, untick "Show names", switch to "Keep contents fixed (north stays put)", lower the opacity: each shows in the cube at once.
4. Near townspeople: untick "Show NPCs": the purple NPC cubes disappear, players stay. Tick it again and change the "NPCs" colour: they come back in the new colour.
5. Reload the plugin in `/xlplugins`: the changed settings are still there.
6. Click "Reset to defaults": everything goes back (40 yalms, Rotate contents with camera, names on, NPCs on, blue/green/orange/purple, full opacity).

## View control (#63)

The bridge checks that a turned view changes the frame correctly, that the camera never turns it, the control's corner, and reset. It can't drag ImGuizmo's widget with a real mouse.

1. `/cube`: a small x/y/z cube widget sits in the bottom-right corner of the cube window, clear of the big cube.
2. Drag across the widget: the big cube turns with it (left/right turns it, up/down tilts it, down to looking from below). The window itself doesn't move during the drag.
3. Click one face of the widget: the view swings round to look straight at that side (or straight down for the top).
4. Turn your character and the camera: the view you chose stays as it is (only the contents turn, in "Rotate contents with camera" mode).
5. Drag the window by empty space away from the widget: the window still moves as usual.
6. Reload the plugin in `/xlplugins`: the view is still the one you chose.
7. `/cube config`, "View control corner" → Top left: the widget moves to the top-left corner. "Reset view": back to the default (from behind and above, 30°).
