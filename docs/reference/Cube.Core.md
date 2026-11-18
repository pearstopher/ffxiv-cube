# <a id="Cube_Core"></a> Namespace Cube.Core

**All the logic, with no Dalamud, game or Lumina references,** so it can be unit tested without launching the game (enforced by `CoreAssemblyTests`). This is where the cube's geometry, projection and drawing order live. Per-frame code here must not allocate (PROJECT.md, "Efficiency and memory").

### Classes

 [AtomicFile](Cube.Core.AtomicFile.md)

Writes a file so that readers only ever see the old or the complete new content, never a half-written one.

 [BuildInfo](Cube.Core.BuildInfo.md)

Formats the build details the plugin logs when it loads, so a stale build
(wrong configuration, or not rebuilt) is obvious in <code>/xllog</code>.

 [Colour](Cube.Core.Colour.md)

Packs and shades colours in the 32-bit layout ImGui uses (<code>0xAABBGGRR</code>), without Core depending on ImGui.

 [CommandArgs](Cube.Core.CommandArgs.md)

Reads what was typed after <code>/cube</code> (PROJECT.md, "Features": <code>/cube</code> toggles the window, <code>/cube config</code> opens the settings).

 [CoreAssembly](Cube.Core.CoreAssembly.md)

Marker for the Core assembly. Core holds all the cube logic (geometry, projection, drawing order) and
must not depend on Dalamud or the game, so it can be unit tested on its own.

 [CubeGeometry](Cube.Core.CubeGeometry.md)

Turns cubes into draw-list primitives: the big cube as a wireframe, and solid cubes (players, later
collision boxes) as their visible faces only (PROJECT.md: "Everything it draws is a cube"; "Cheap drawing":
only the visible faces of a cube).

 [CubeSettings](Cube.Core.CubeSettings.md)

The user's settings (PROJECT.md, "Features": cube size, orientation mode, labels on/off, colours and opacity),
with their defaults and the rules for valid values. The plugin's saved configuration derives from this, so the
rules are unit tested without Dalamud.

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

 [NameCache](Cube.Core.NameCache.md)

Turns the game's UTF-8 name bytes into strings, once per name: a string is only built when the bytes in an
object slot change (PROJECT.md, "Efficiency and memory": name labels are cached and only rebuilt when a name
changes). Reading the same names every frame allocates nothing.

 [PlayerCubes](Cube.Core.PlayerCubes.md)

Draws each player in the cube as a small solid cube in their relation's colour, with a name label for other
players (PROJECT.md, "Players": player cube size fixed relative to the big cube, about 1/40 of its edge, minimum a
few pixels; "Features": the local player is a cube at the centre in its own colour, other players are labelled
with their names; "Labels": the name drawn next to the cube, overlaps are fine).

 [PlayerSet](Cube.Core.PlayerSet.md)

The players inside the cube this frame (PROJECT.md, "Players": every player character in the object table whose
position is inside the cube; players outside are not shown at all). The game reader offers every player it
finds; this class culls, classifies and names them.

 [ViewYaw](Cube.Core.ViewYaw.md)

Chooses the cube's alignment yaw (PROJECT.md, "Cube"): in <b>Camera</b> mode it follows the game camera's
horizontal direction so "forward in the cube" matches the game view; in <b>North up</b> mode it is fixed with
north at the front. The yaw turns the contents, never the wireframe (see [CubeSpace](Cube.Core.CubeSpace.md#Cube_Core_CubeSpace)).

 [VisibilityRules](Cube.Core.VisibilityRules.md)

Decides whether to draw (PROJECT.md, "When hidden": not logged in, loading screens, cutscenes and GPose; the
window is just empty then, never an error).

### Structs

 [CubeSpace](Cube.Core.CubeSpace.md)

The cube of space around the local player (PROJECT.md, "Exact behaviour rules", Cube): centred on the player,
with an edge of 10–100 yalms (default 40), turned about the vertical axis by its alignment [Yaw](Cube.Core.CubeSpace.md#Cube_Core_CubeSpace_Yaw).
Answers "is this point inside?" and turns world positions into cube-local view coordinates.

 [DrawPrimitive](Cube.Core.DrawPrimitive.md)

One thing to draw, already projected to window pixels: the plugin turns each into one ImGui call
(PROJECT.md, "Cheap drawing": the fewest primitives that do the job).

 [GameState](Cube.Core.GameState.md)

The game states that decide whether the cube is drawn (PROJECT.md, "When hidden"). Read once per frame by the
plugin from Dalamud's client state and condition flags (<code>docs/research-notes.md</code>, "When to hide").

 [NearbyPlayer](Cube.Core.NearbyPlayer.md)

One player inside the cube this frame: where they are, how they relate to the local player, and their
(cached) name for the label.

 [PlayerPalette](Cube.Core.PlayerPalette.md)

One colour per player relation (PROJECT.md, "Colours": self, party/alliance members, everyone else; one colour
each). Built from the user's [CubeSettings](Cube.Core.CubeSettings.md#Cube_Core_CubeSettings).

 [ViewProjection](Cube.Core.ViewProjection.md)

Projects view coordinates (from [CubeSpace](Cube.Core.CubeSpace.md#Cube_Core_CubeSpace)) onto the window: a fixed-angle view from above
and behind (PROJECT.md, "View": default 30° elevation).

### Enums

 [OrientationMode](Cube.Core.OrientationMode.md)

How the cube is aligned, and so what happens to its contents (PROJECT.md, "Cube": the orientation setting). The
cube's frame stays still on screen in both modes.

 [PlayerRelation](Cube.Core.PlayerRelation.md)

How a player relates to the local player, which picks their colour (PROJECT.md, "Colours": self,
party/alliance members, everyone else; one colour each).

 [PrimitiveKind](Cube.Core.PrimitiveKind.md)

What a [DrawPrimitive](Cube.Core.DrawPrimitive.md#Cube_Core_DrawPrimitive) draws.

 [Visibility](Cube.Core.Visibility.md)

Whether the cube is drawn, and if not, why (PROJECT.md, "When hidden"). Decided by [VisibilityRules](Cube.Core.VisibilityRules.md#Cube_Core_VisibilityRules).
