# Manual tests

Only what the in-game test bridge can't check (`docs/in-game-testing.md`). Optional, before a release. Report results in the format from CLAUDE.md.

## Settings window (#20)

The bridge checks every setting changes the frame and that saving writes the file, but it can't click the controls.

1. Type `/cube config`: the settings window opens. `/cube` still shows and hides the cube.
2. Drag the Width, Height and Depth sliders (#90): the cube stretches into a box along that axis while dragging. Drag the Zoom slider (#94): all three shrink or grow together keeping the shape (contents look bigger as it goes down), and the Width/Height/Depth sliders follow it; it stops where the smallest side reaches 10 or the largest 100. Drag it far down and back to where it started: the box is exactly the size it was. Move the Height slider: the Zoom slider follows when height is the longest side. "Reset size" gives 40 × 40 × 40 and the Zoom slider reads 40.
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

## Resize the cube (#90)

The bridge checks that set sizes draw in the right proportions, that zooming changes what's inside, reset, and that the gizmo leaves the settings alone when idle. The maths that turns a handle drag into a resize or zoom is unit tested. It can't drag the scale handles with a real mouse.

The gizmo uses one `Manipulate` call with Rotate | Scale. If the scale handles and the rings fight over the mouse (a drag on one does the other), report it: the fix is two separate calls.

1. `/cube`: the corner gizmo now also has three scale handles (red, green and blue lines with a square end, inside the rings) and a centre handle.
2. Drag the red (x) handle outwards: the cube gets wider (a box), nothing else changes. Drag it inwards: narrower. It stops at 10 and 100 yalms. The window doesn't move during the drag.
3. Drag the green (y) handle: only the height changes. Drag the blue (z) handle: only the depth changes.
4. Turn the view with a ring first (any angle, roll too), then drag each scale handle again: each still changes its own extent (the handle lies along that axis of the box).
5. Drag the centre handle: the whole box grows or shrinks, keeping its shape; it stops when one side reaches 10 or 100 and keeps its shape there.
6. Drag a ring after resizing: the box turns and keeps its size.
7. Other players and NPCs stay the same small size while the box narrows; anything outside the box on any one axis disappears.
8. Reload the plugin: the size is still the one you chose. `/cube config`, "Reset size": back to 40 × 40 × 40.

## Player height in cube (#91)

The bridge checks that the setting changes which players above or below are inside, moves the player's marker off the middle, shifts the collision region, and that reset restores 50%. It can't drag the real slider.

1. `/cube` and `/cube config`. "Player height in cube" shows 50%; your cube is in the middle of the big one.
2. Drag the slider to 0%: your cube slides down to the bottom face of the big cube, which stays where it is in the window. Collision and players above you fill the box; nothing below you is drawn.
3. Drag it to 100%: your cube goes to the top face and the box shows what's below you.
4. Change the height slider or zoom while at 25%: you stay a quarter of the way up the box.
5. Reload the plugin: the slider keeps its value. "Reset to defaults": back to 50%.

## Collision boxes compared with vnavmesh (#31)

The bridge checks the boxes are inside the cube, in the collision colour, and that the floor is painted under the player (`collision-drawn`), and the snapshot shows them. Whether they match the real walls and floors needs eyes on the game, with vnavmesh's collision debug view as the reference (vnavmesh is only a comparison tool, not a dependency).

1. Stand in a house or apartment interior, `/cube`: grey-blue see-through boxes show the floor under your cube and the walls around it, roughly where the room's walls are.
2. Turn the camera: the boxes turn with the players; the big cube's wireframe stays still.
3. Open vnavmesh's debug window and turn on its collision geometry view. The walls and floors it outlines inside the 40-yalm cube around you should each have a box in the cube in the same place (boxes may be coarser: one per collision node, not per triangle).
4. Walk out of the room or across a big room: the boxes follow within a few steps (a new gather every quarter of the cube's edge).
5. Repeat outdoors (a city street and open field): ground, buildings and big rocks show as boxes; nothing far outside the cube is drawn.
6. Teleport to another zone: the old zone's boxes disappear at once, and the new zone's appear shortly after loading.

## Collision stays inside the cube while turning (#93)

The bridge (`collision-inside-cube`) turns a simulated camera to 0°, 30°, 45° and 90° and checks every corner of every collision face is inside the cube's outline, with the real scenery and with a synthetic floor three times the cube's size. What it can't do is turn the real camera smoothly and look.

1. Settings: orientation "Rotate contents with camera", collision on. Stand on open ground (a big floor or field, so the ground is bigger than the cube), `/cube`.
2. Turn the camera slowly through a full circle. The ground and other collision stay inside the big cube's wireframe the whole way round: no corners poke past its edges, at 45° or anywhere else.
3. The collision turns with the players, as one scene: the ground's outline follows the cube's sides rather than staying still while everything else turns.
