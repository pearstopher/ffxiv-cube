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

1. `/cube`: a small rotate gizmo (red, green and blue rings plus a white outer ring) sits in the bottom-right corner of the cube window, filling its corner square and clear of the big cube. Next to it, inward along the bottom edge, is a second gizmo of the same size (the resize gizmo, see below); the two don't overlap.
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

Since #97 the resize gizmo is a separate square next to the rotate gizmo (own `Manipulate` call, Scale only), so the handles can't start a rotation.

1. `/cube`: next to the rotate gizmo is a second square with three scale handles (red, green and blue lines with a square end) and a centre handle, lined up with the cube's sides.
2. Drag the red (x) handle outwards: the cube gets wider (a box), nothing else changes. Drag it inwards: narrower. It stops at 1 and 100 yalms (also drag one width/height/depth slider down to 1: the cube becomes a thin slab and the markers keep their size). The window doesn't move during the drag.
3. Drag the green (y) handle: only the height changes. Drag the blue (z) handle: only the depth changes.
4. Turn the view with a ring first (any angle, roll too), then drag each scale handle again: each still changes its own extent (the handle lies along that axis of the box).
5. Drag the centre handle: the whole box grows or shrinks, keeping its shape; it stops when one side reaches 10 or 100 and keeps its shape there.
6. Drag a ring in the rotate gizmo after resizing: the box turns and keeps its size.
7. Other players and NPCs stay the same small size while the box narrows; anything outside the box on any one axis disappears.
8. Reload the plugin: the size is still the one you chose. `/cube config`, "Reset size": back to 40 × 40 × 40.

## Player height in cube (#91)

The bridge checks that the setting changes which players above or below are inside, moves the player's marker off the middle, shifts the collision region, and that reset restores 50%. It can't drag the real slider.

1. `/cube` and `/cube config`. "Player height in cube" shows 50%; your cube is in the middle of the big one.
2. Drag the slider to 0%: your cube slides down to the bottom face of the big cube, which stays where it is in the window. Collision and players above you fill the box; nothing below you is drawn.
3. Drag it to 100%: your cube goes to the top face and the box shows what's below you.
4. Change the height slider or zoom while at 25%: you stay a quarter of the way up the box.
5. Reload the plugin: the slider keeps its value. "Reset to defaults": back to 50%.

## Jump (#105)

The bridge checks everything except the real jump flag (it can't make the character jump): with a simulated flag and a scripted height the cube holds its height, the world stays still, the marker rises, and landing higher snaps the cube. This session tells us which flag the game raises and what the height does. A Debug build logs a `Jump trace:` line per frame while a jump flag is set and for half a second after.

1. `/cube`. Stand still and jump. The walls and everything in the cube stay still; your marker rises and lands. No jump: the cube follows you as before.
2. Run and jump. The world scrolls sideways as you run (x and z follow) but doesn't bob up and down.
3. Jump onto a ledge a few yalms higher. On landing the cube jumps to the new height in one frame. Then jump off a ledge down: same.
4. Walk off a ledge without jumping (and fall, swim, mount): the cube follows you smoothly, no hold.
5. Set "Player height in cube" to 0% or 100% and jump: your marker reaches the face and stays at it instead of leaving the box.
6. Report with the usual block, plus the `Jump trace:` lines from `dalamud.log` for each of steps 1-4 (look for which of `Jumping` / `Jumping61` is True, and whether `y` rises while a flag is set, or before or after it).

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

## Split rotate and resize gizmos (#97)

The bridge checks both squares' placement for two corners and that idle frames change nothing. It can't use a real mouse.

1. `/cube`: two gizmos side by side in the corner, not overlapping.
2. Drag each ring (red, green, blue, white outer) in the rotate gizmo: only the cube's orientation changes, never its size. The resize gizmo doesn't react.
3. Drag each handle and the centre in the resize gizmo: only the size changes, never the orientation. The rotate gizmo doesn't react.
4. The window doesn't move while the mouse is on, or dragging in, either gizmo; dragging empty space still moves it.
5. `/cube config`, "View control corner": try all four corners. Both gizmos move together, the resize one always inward of the rotate one.
6. Shrink the window until it is very narrow: both gizmos shrink and never overlap.

## Phantom collision boxes (#107)

1. Walk to a spot where a collision box used to appear that you could walk through (for example, an invisible
   trigger area or a flat patch of ground with a box over it). Check that no box is drawn there any more.
2. Walk up to a real wall or a solid object and check its box is still drawn.
3. Run `python scripts/ingame_test.py` there and check `collision-phantoms` passes. Its log lines (`Phantom #107
   nearest ...` in `dalamud.log`) list the nearest remaining boxes the game's raycast doesn't hit, with their fields.

## Enemy colour and [T] / [!] markers (#111)

1. Stand near a hostile enemy (a FATE mob or overworld monster) and check its cube is red, not the purple of friendly NPCs. Change "Enemies" in the settings window and check the colour follows.
2. Target the enemy: its label starts with `[T]`. Target a friendly NPC or another player: they get `[T]` too. Clear the target: the marker goes.
3. Let the enemy attack you: its label starts with `[!]`; if it is also your target, `[T] [!]`. A friendly NPC never shows `[!]`.
4. Check `dalamud.log` for `Probe`/error lines. If `[!]` is wrong (missing while being hit, or on enemies that aren't attacking you), report it: the enemy list read (`UIState.Hater`) would need swapping for another field.

## Names, per-kind toggles and interactables (#112)

1. Open the settings window. With "Show your name" ticked your own cube has your name beside it; untick it and the name goes. Names of other people are unaffected.
2. Set "Name position" to "Centred above": every name sits centred over its cube, just above the top corner, and stays readable at the window edge. Back to "To the side": names return to the right of the cubes.
3. Untick "Show players", "Show NPCs", "Show enemies" and "Show interactables" one at a time near each kind: only that kind disappears; you never do.
4. In a housing ward or city, stand near an "Entrance", "Placard", "Summoning Bell" or "Market Board": each shows a teal cube with that name. Objects the game marks with only an arrow or icon, and furniture or gardening plants, show nothing. Change "Interactables" and the colour follows.
5. Check the cube follows each interactable in the game. If one that has a text label in the game doesn't show (or an icon-only one does), report its name and place: the filter is "an event object with a name" (`docs/research-notes.md`).

## Collision boxes split at gaps (#127)

1. Stand inside a house with a counter that has an opening (or any room with a kitchen counter or a table with space under it). Open the cube with collision on. Within a second or two of arriving, the box over the counter should split so the opening shows as empty space instead of one long box.
2. Look for a large square box drawn over empty floor or air in the room. It should be gone, or much smaller, once the boxes have sharpened (they start as the old coarse boxes and refine a few hundred triangles a frame).
3. Walk around: the boxes near you should sharpen in each new spot, and a wall or floor you can bump into should never lose its box (no holes). Report any wall or floor that has no box.
4. Run `python scripts/ingame_test.py` there and check `collision-split` passes (its metrics list the nodes refined and the pieces kept).

## Counter opening and splitting of big nodes (#127, #129)

1. In a house, stand at a counter with an opening (a block on each side and a gap you could fall into) and look at the cube from above: the opening shows as a gap between two boxes, not one long box.
2. Walk around the room: no large box floats over empty space; real walls, floors and furniture still have their boxes.
3. Run `python scripts/ingame_test.py` at the counter and check `probe-collision-near` and `probe-collision-fit` pass; `fitNodesOverSplitLimit` in `results.json` is 0.
