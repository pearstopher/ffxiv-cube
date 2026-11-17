# <a id="Cube_Core"></a> Namespace Cube.Core

**All the logic, with no Dalamud, game or Lumina references,** so it can be unit tested without launching the game (enforced by `CoreAssemblyTests`). This is where the cube's geometry, projection and drawing order live. Per-frame code here must not allocate (PROJECT.md, "Efficiency and memory").

### Classes

 [AtomicFile](Cube.Core.AtomicFile.md)

Writes a file so that readers only ever see the old or the complete new content, never a half-written one.

 [BuildInfo](Cube.Core.BuildInfo.md)

Formats the build details the plugin logs when it loads, so a stale build
(wrong configuration, or not rebuilt) is obvious in <code>/xllog</code>.

 [CoreAssembly](Cube.Core.CoreAssembly.md)

Marker for the Core assembly. Core holds all the cube logic (geometry, projection, drawing order) and
must not depend on Dalamud or the game, so it can be unit tested on its own.

 [DrawCostSampler](Cube.Core.DrawCostSampler.md)

Keeps the cost of the last few frames of the plugin's draw path: time taken and bytes allocated.
The in-game test bridge reads it to check the budgets in PROJECT.md ("Efficiency and memory"):
under 0.5 ms per frame on average and zero bytes allocated per frame in steady state.

 [ViewYaw](Cube.Core.ViewYaw.md)

Chooses the yaw of the view around the cube (PROJECT.md, "View"): in <b>Camera</b> mode it follows the
game camera so "forward on screen" matches the game view; in <b>North up</b> mode it is fixed with
north at the back.

### Structs

 [CubeSpace](Cube.Core.CubeSpace.md)

The cube of space around the local player (PROJECT.md, "Exact behaviour rules", Cube): axis-aligned to
the world, centred on the player, with an edge of 10–100 yalms (default 40). Answers "is this point
inside?" and turns world positions into cube-local view coordinates.

### Enums

 [OrientationMode](Cube.Core.OrientationMode.md)

How the view around the cube is turned (PROJECT.md, "View": orientation mode setting).
