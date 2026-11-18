# <a id="Cube_Core"></a> Namespace Cube.Core

**All the logic, with no Dalamud, game or Lumina references,** so it can be unit tested without launching the game (enforced by `CoreAssemblyTests`). This is where the cube's geometry, projection and drawing order live. Per-frame code here must not allocate (PROJECT.md, "Efficiency and memory").

### Classes

 [AtomicFile](Cube.Core.AtomicFile.md)

Writes a file so that readers only ever see the old or the complete new content, never a half-written one.

 [BoxExtents](Cube.Core.BoxExtents.md)

The rules for the cube's size (PROJECT.md, "Exact behaviour rules", Cube; changed 2026-11-18, #90): the cube is a
box with three extents in yalms, width (x), height (y) and depth (z), each 10–100, default 40. Zoom scales all three
together, keeping their ratios.

 [BuildInfo](Cube.Core.BuildInfo.md)

Formats the build details the plugin logs when it loads, so a stale build
(wrong configuration, or not rebuilt) is obvious in <code>/xllog</code>.

 [ClippedCollision](Cube.Core.ClippedCollision.md)

The cached collision boxes clipped to this frame's cube, ready for [CollisionCubes](Cube.Core.CollisionCubes.md#Cube_Core_CollisionCubes) (PROJECT.md,
"Collision (phase 2)": boxes clipped to the cube). Clips again only when the cube or the gathered boxes change.

 [CollisionBoxSet](Cube.Core.CollisionBoxSet.md)

The cached collision boxes around the cube (PROJECT.md, "Collision (phase 2)" and "Efficiency and memory"): the
game reader walks the client's collision scene and offers every box it finds; this class culls them to the area
around the cube, merges the cheap cases, holds at most a fixed number, and says when the list is stale.

 [CollisionCache](Cube.Core.CollisionCache.md)

Gathers the collision boxes around the cube a few colliders per frame, within a time budget, and keeps the last
complete set for drawing (PROJECT.md, "Collision (phase 2)" and "Efficiency and memory").

 [CollisionClipper](Cube.Core.CollisionClipper.md)

Clips collision boxes to the cube (PROJECT.md, "Collision (phase 2)": each collider inside or crossing the cube is
shown as a simplified box, clipped to the cube). Boxes wholly outside are left out; the rest are cut back to the
part inside, as a [ClippedBox](Cube.Core.ClippedBox.md#Cube_Core_ClippedBox).

 [CollisionCubes](Cube.Core.CollisionCubes.md)

Draws the collision around the player as solid boxes in the cube, sorted together with the players (PROJECT.md,
"Collision (phase 2)": each collider inside or crossing the cube is shown as a simplified box, clipped to the cube;
"Draw order": far things first, so near ones cover far ones).

 [CollisionDetails](Cube.Core.CollisionDetails.md)

The rules behind each [CollisionDetail](Cube.Core.CollisionDetail.md#Cube_Core_CollisionDetail) level (#32): how deep the game reader walks a mesh's node tree,
and how a change of level makes the collision cache gather again straight away.

 [Colour](Cube.Core.Colour.md)

Packs and shades colours in the 32-bit layout ImGui uses (<code>0xAABBGGRR</code>), without Core depending on ImGui.

 [CommandArgs](Cube.Core.CommandArgs.md)

Reads what was typed after <code>/cube</code> (PROJECT.md, "Features": <code>/cube</code> toggles the window, <code>/cube config</code> opens the settings).

 [CoreAssembly](Cube.Core.CoreAssembly.md)

Marker for the Core assembly. Core holds all the cube logic (geometry, projection, drawing order) and
must not depend on Dalamud or the game, so it can be unit tested on its own.

 [CubeGeometry](Cube.Core.CubeGeometry.md)

Turns cubes into draw-list primitives: the big cube as a wireframe, and a single solid cube as its visible faces
(PROJECT.md: "Everything it draws is a cube"; "Cheap drawing": only the visible faces of a cube). Also holds the
face shades every solid box uses.

 [CubeSettings](Cube.Core.CubeSettings.md)

The user's settings (PROJECT.md, "Features": cube size (width, height and depth since #90), player height in the cube (#91), orientation mode, labels on/off, colours and opacity; "NPCs":
NPCs on/off; "View orientation control": the saved view orientation and the control's corner; #32: collision on/off
and detail), with their defaults
and the rules for valid values. The plugin's saved configuration derives from this, so the rules are unit tested
without Dalamud.

 [DrawCostSampler](Cube.Core.DrawCostSampler.md)

Keeps the cost of the last few frames of the plugin's draw path: time taken and bytes allocated.
The in-game test bridge reads it to check the budgets in PROJECT.md ("Efficiency and memory"):
under 0.5 ms per frame on average and zero bytes allocated per frame in steady state.

 [DrawList](Cube.Core.DrawList.md)

The frame's primitives, in window pixels, sorted far to near (PROJECT.md, "Draw order": painter's algorithm,
so near cubes cover far ones). Built by [CubeGeometry](Cube.Core.CubeGeometry.md#Cube_Core_CubeGeometry), drawn by the plugin, and dumped by the
test bridge.

 [ErrorThrottle](Cube.Core.ErrorThrottle.md)

Decides when a repeated error is worth logging. A handler that fails every frame would otherwise write 60 log
lines a second and bury everything else (CLAUDE.md, "Dalamud rules": catch and log, never let an exception reach
the game).

 [GizmoModel](Cube.Core.GizmoModel.md)

The model matrix the corner gizmo works on, which carries both the view orientation (#79) and the cube's size
(#90), and the rules for reading a drag back from it (PROJECT.md, "View orientation control" and "Control" under
the box size: the axis handles change width, height or depth, the centre handle is zoom).

 [NameCache](Cube.Core.NameCache.md)

Turns the game's UTF-8 name bytes into strings, once per name: a string is only built when the bytes in an
object slot change (PROJECT.md, "Efficiency and memory": name labels are cached and only rebuilt when a name
changes). Reading the same names every frame allocates nothing.

 [PlayerCubes](Cube.Core.PlayerCubes.md)

Draws each player in the cube as a small solid cube in their relation's colour, with a name label for other
players (PROJECT.md, "Players": player cube size fixed relative to the big cube, about 1/40 of its largest extent,
minimum a few pixels; "Features": the local player is a cube at the centre in its own colour, other players are labelled
with their names; "Labels": the name drawn next to the cube, overlaps are fine; "NPCs": named NPCs are shown the
same way, in the NPC colour).

 [PlayerHeight](Cube.Core.PlayerHeight.md)

The rules for where the player sits vertically in the box (PROJECT.md, "Exact behaviour rules", Cube, "Player height
in cube"; #91): a fraction <code>p</code> of the box's height, from 0 (player on the bottom face) to 1 (player on the top
face), default 0.5 (centred). The box spans <code>player.y - height × p</code> to <code>player.y + height × (1 - p)</code>;
x and z stay centred on the player.

 [PlayerSet](Cube.Core.PlayerSet.md)

The players and named NPCs inside the cube this frame (PROJECT.md, "Players": every player character in the
object table whose position is inside the cube; "NPCs": every named NPC inside the cube, shown the same way;
anyone outside is not shown at all). The game reader offers everyone it finds; this class culls, classifies and
names them.

 [ViewWidget](Cube.Core.ViewWidget.md)

Where the view control goes and how big it is (PROJECT.md, "View orientation control": a small control in a corner
of the window). Kept in Core so the placement rules are unit tested.

 [ViewYaw](Cube.Core.ViewYaw.md)

Chooses the cube's alignment yaw (PROJECT.md, "Cube"): in <b>Camera</b> mode it follows the game camera's
horizontal direction so "forward in the cube" matches the game view; in <b>North up</b> mode it is fixed with
north at the front. The yaw turns the contents, never the wireframe (see [CubeSpace](Cube.Core.CubeSpace.md#Cube_Core_CubeSpace)).

 [VisibilityRules](Cube.Core.VisibilityRules.md)

Decides whether to draw (PROJECT.md, "When hidden": not logged in, loading screens, cutscenes, GPose and PvP
areas; the window is just empty then, never an error).

### Structs

 [BoxProjector](Cube.Core.BoxProjector.md)

Projects world-axis-aligned boxes (player cubes, collision boxes) onto the window for one frame, as their visible
faces only (PROJECT.md: "Everything it draws is a cube"; "Cheap drawing": only the visible faces of a cube).

 [BoxShades](Cube.Core.BoxShades.md)

The colours of a box's three visible faces, worked out once per colour per frame ([Shades](Cube.Core.BoxProjector.md#Cube_Core_BoxProjector_Shades)).

 [ClippedBox](Cube.Core.ClippedBox.md)

One collision box clipped to the cube (PROJECT.md, "Collision (phase 2)": each collider is shown as a simplified
box, clipped to the cube): an upright prism whose footprint is the part of the box's footprint inside the cube's
turned square, cut to the cube's height.

 [CollisionBox](Cube.Core.CollisionBox.md)

One piece of collision as a simplified, world-axis-aligned box (PROJECT.md, "Collision (phase 2)": each collider
inside or crossing the cube is shown as a simplified box, not the game's model).

 [CubeOutline](Cube.Core.CubeOutline.md)

The big cube's outline on screen: the convex hull of its eight projected corners. Used to check that nothing drawn
for the contents reaches outside the cube (PROJECT.md, "Collision (phase 2)": clipped to the cube; #93).

 [CubeSpace](Cube.Core.CubeSpace.md)

The cube of space around the local player (PROJECT.md, "Exact behaviour rules", Cube): centred on the player
horizontally, and vertically too unless "Player height in cube" (#91, [PlayerHeight](Cube.Core.PlayerHeight.md#Cube_Core_PlayerHeight)) moves them up or down,
a box with a width, height and depth of 10–100 yalms each (default 40, see [BoxExtents](Cube.Core.BoxExtents.md#Cube_Core_BoxExtents)), turned about
the vertical axis by its alignment [Yaw](Cube.Core.CubeSpace.md#Cube_Core_CubeSpace_Yaw). Answers "is this point inside?" and turns world positions into
cube-local view coordinates.

 [DrawPrimitive](Cube.Core.DrawPrimitive.md)

One thing to draw, already projected to window pixels: the plugin turns each into one ImGui call
(PROJECT.md, "Cheap drawing": the fewest primitives that do the job).

 [CollisionClipper.Footprint](Cube.Core.CollisionClipper.Footprint.md)

The cube's rectangular footprint in world X/Z (stored as `System.Numerics.Vector2` X and Y), with its bounds for the
cheap reject. Computed once per clip pass.

 [FootprintCorners](Cube.Core.FootprintCorners.md)

Inline storage for a [ClippedBox](Cube.Core.ClippedBox.md#Cube_Core_ClippedBox)'s footprint corners, so the struct holds no array.

 [GameState](Cube.Core.GameState.md)

The game states that decide whether the cube is drawn (PROJECT.md, "When hidden"). Read once per frame by the
plugin from Dalamud's client state and condition flags (<code>docs/research-notes.md</code>, "When to hide").

 [GizmoChange](Cube.Core.GizmoChange.md)

What a gizmo drag changed ([Read](Cube.Core.GizmoModel.md#Cube_Core_GizmoModel_Read)).

 [NearbyPlayer](Cube.Core.NearbyPlayer.md)

One player or named NPC inside the cube this frame: where they are, how they relate to the local player, and
their (cached) name for the label. NPCs use the same record so they share the players' culling, draw order and
label code (PROJECT.md, "NPCs": shown the same way as players).

 [PlayerPalette](Cube.Core.PlayerPalette.md)

One colour per relation (PROJECT.md, "Colours": self, party/alliance members, everyone else, NPCs; one colour
each). Built from the user's [CubeSettings](Cube.Core.CubeSettings.md#Cube_Core_CubeSettings).

 [ViewOrientation](Cube.Core.ViewOrientation.md)

How the user has turned the cube on screen with the view control (PROJECT.md, "View orientation control"): a full
rotation, roll included. Default: from above and behind, 30° elevation, front face away from the viewer.

 [ViewProjection](Cube.Core.ViewProjection.md)

Projects view coordinates (from [CubeSpace](Cube.Core.CubeSpace.md#Cube_Core_CubeSpace)) onto the window, as seen from the user's chosen
[ViewOrientation](Cube.Core.ViewOrientation.md#Cube_Core_ViewOrientation) (PROJECT.md, "View orientation control"; default from above and behind at 30°).

 [ZoomSlider](Cube.Core.ZoomSlider.md)

The settings window's zoom slider (#94; PROJECT.md, "Zoom"): one value, the box's largest extent in yalms, that
scales width, height and depth together keeping their ratios. It saves nothing of its own: its value always comes
from the current extents, so moving a width/height/depth slider or a gizmo handle moves it too.

### Interfaces

 [ICollisionSource](Cube.Core.ICollisionSource.md)

Where [CollisionCache](Cube.Core.CollisionCache.md#Cube_Core_CollisionCache) gets its boxes from: in the plugin, the game's collision scene
(<code>Cube.Game.CollisionReader</code>); in the tests, a list of fake colliders.

### Enums

 [CollisionDetail](Cube.Core.CollisionDetail.md)

How finely collision is drawn (#32, the collision detail setting; PROJECT.md, "Collision (phase 2)": colliders are
shown as simplified boxes). Each level is a depth limit on the walk down a mesh collider's node tree; primitive
colliders (boxes, cylinders, spheres, planes) are one box at every level.

 [OrientationMode](Cube.Core.OrientationMode.md)

How the cube is aligned, and so what happens to its contents (PROJECT.md, "Cube": the orientation setting). The
cube's frame stays still on screen in both modes.

 [PlayerRelation](Cube.Core.PlayerRelation.md)

How someone in the cube relates to the local player, which picks their colour (PROJECT.md, "Colours": self,
party/alliance members, everyone else, NPCs; one colour each).

 [PrimitiveKind](Cube.Core.PrimitiveKind.md)

What a [DrawPrimitive](Cube.Core.DrawPrimitive.md#Cube_Core_DrawPrimitive) draws.

 [Visibility](Cube.Core.Visibility.md)

Whether the cube is drawn, and if not, why (PROJECT.md, "When hidden"). Decided by [VisibilityRules](Cube.Core.VisibilityRules.md#Cube_Core_VisibilityRules).

 [WidgetCorner](Cube.Core.WidgetCorner.md)

Which corner of the cube window the view control sits in (PROJECT.md, "View orientation control": bottom right by
default). Placement is worked out by [Position](Cube.Core.ViewWidget.md#Cube_Core_ViewWidget_Position).
