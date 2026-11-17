# <a id="Cube_Core"></a> Namespace Cube.Core

**All the logic, with no Dalamud, game or Lumina references,** so it can be unit tested without launching the game (enforced by `CoreAssemblyTests`). This is where the cube's geometry, projection and drawing order live. Per-frame code here must not allocate (PROJECT.md, "Efficiency and memory").

### Classes

 [BuildInfo](Cube.Core.BuildInfo.md)

Formats the build details the plugin logs when it loads, so a stale build
(wrong configuration, or not rebuilt) is obvious in <code>/xllog</code>.

 [CoreAssembly](Cube.Core.CoreAssembly.md)

Marker for the Core assembly. Core holds all the cube logic (geometry, projection, drawing order) and
must not depend on Dalamud or the game, so it can be unit tested on its own.
