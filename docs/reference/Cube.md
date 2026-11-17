# <a id="Cube"></a> Namespace Cube

**The plugin: everything that touches Dalamud or the game.** It is kept as thin as possible: it reads game state, hands it to `Cube.Core` and draws the result. Entry point: `Plugin`. The cube is drawn in `CubeWindow`, a Dalamud window toggled by `/cube`. Game access is read-only (PROJECT.md, "Goal"), and every handler is released when the plugin unloads.

### Namespaces

 [Cube.Core](Cube.Core.md)

 [Cube.TestBridge](Cube.TestBridge.md)

 [Cube.Tests](Cube.Tests.md)

### Classes

 [CubeWindow](Cube.CubeWindow.md)

The window the cube is drawn in (PROJECT.md, "Features": a Dalamud window toggled by <code>/cube</code>).

 [Plugin](Cube.Plugin.md)

Entry point. Registers the <code>/cube</code> command and the cube window, and logs which build loaded.
