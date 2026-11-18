# Manual tests

Only what the in-game test bridge can't check (`docs/in-game-testing.md`). Optional, before a release. Report results in the format from CLAUDE.md.

## Settings window (#20)

The bridge checks every setting changes the frame and that saving writes the file, but it can't click the controls.

1. Type `/cube config`: the settings window opens. `/cube` still shows and hides the cube.
2. Drag the cube size slider: the cube changes while dragging.
3. Change a colour, untick "Show names", switch to "Keep contents fixed (north stays put)", lower the opacity: each shows in the cube at once.
4. Near townspeople: untick "Show NPCs": the purple NPC cubes disappear, players stay. Tick it again and change the "NPCs" colour: they come back in the new colour.
5. Near walls (#32): set collision "Detail" to "Coarse": the grey-blue collision boxes become fewer, bigger blocks at once; "Medium" is in between; "Full" is back to the close fit. Untick "Show collision": the collision boxes disappear; tick it again: they come back.
6. Reload the plugin in `/xlplugins`: the changed settings are still there.
7. Click "Reset to defaults": everything goes back (40 yalms, Rotate contents with camera, names on, NPCs on, collision on at Full detail, blue/green/orange/purple, full opacity).

## View control (#63, rotate gizmo since #79)

The bridge checks that a turned or rolled view changes the frame correctly, that the camera never turns it, the control's corner, and reset. It can't drag ImGuizmo's rings with a real mouse.

1. `/cube`: a small rotate gizmo (red, green and blue rings plus a white outer ring) sits in the bottom-right corner of the cube window, filling its corner square and clear of the big cube.
2. Drag the red (x) ring: the big cube turns around its own left-right axis (tilts forward or back). The window itself doesn't move during the drag.
3. Drag the green (y) ring: the cube turns around its own up axis. Drag the blue (z) ring: it turns around its front-back axis.
4. Drag the white outer ring: the cube rolls around the screen's axis (it spins flat on the screen).
5. Hovering a ring highlights it; letting go keeps the new view (it doesn't spring back).
6. If you had turned the view with the old cube-shaped control before updating, the first load after the update shows the same view as before.
7. Turn your character and the camera: the view you chose stays as it is (only the contents turn, in "Rotate contents with camera" mode).
8. Drag the window by empty space away from the gizmo: the window still moves as usual.
9. Reload the plugin in `/xlplugins`: the view is still the one you chose, roll included.
10. `/cube config`, "View control corner" → Top left: the widget moves to the top-left corner. "Reset view": back to the default (from behind and above, 30°).

## Collision boxes compared with vnavmesh (#31)

The bridge checks the boxes are inside the cube, in the collision colour, and that the floor is painted under the player (`collision-drawn`), and the snapshot shows them. Whether they match the real walls and floors needs eyes on the game, with vnavmesh's collision debug view as the reference (vnavmesh is only a comparison tool, not a dependency).

1. Stand in a house or apartment interior, `/cube`: grey-blue see-through boxes show the floor under your cube and the walls around it, roughly where the room's walls are.
2. Turn the camera: the boxes turn with the players; the big cube's wireframe stays still.
3. Open vnavmesh's debug window and turn on its collision geometry view. The walls and floors it outlines inside the 40-yalm cube around you should each have a box in the cube in the same place (boxes may be coarser: one per collision node, not per triangle).
4. Walk out of the room or across a big room: the boxes follow within a few steps (a new gather every quarter of the cube's edge).
5. Repeat outdoors (a city street and open field): ground, buildings and big rocks show as boxes; nothing far outside the cube is drawn.
6. Teleport to another zone: the old zone's boxes disappear at once, and the new zone's appear shortly after loading.
