# <a id="Cube"></a> Namespace Cube

**The plugin: everything that touches Dalamud or the game.** It is kept as thin as possible: it reads game state, hands it to `Cube.Core` and draws the result. Entry point: `Plugin`. The cube is drawn in `CubeWindow`, a Dalamud window toggled by `/cube`. Game access is read-only (PROJECT.md, "Goal"), and every handler is released when the plugin unloads.

### Namespaces

 [Cube.Core](Cube.Core.md)

 [Cube.Game](Cube.Game.md)

 [Cube.Snapshot](Cube.Snapshot.md)

 [Cube.TestBridge](Cube.TestBridge.md)

 [Cube.Tests](Cube.Tests.md)

### Classes

 [Configuration](Cube.Configuration.md)

The saved settings: Core's [CubeSettings](Cube.Core.CubeSettings.md#Cube_Core_CubeSettings) plus the version number Dalamud's configuration system
needs. Stored by Dalamud in <code>pluginConfigs\Cube.json</code>; contains no names or IDs (PROJECT.md, "Privacy").

 [CubeWindow](Cube.CubeWindow.md)

The window the cube is drawn in (PROJECT.md, "Features": a Dalamud window toggled by <code>/cube</code>, showing a
wireframe cube with the player at its centre).

 [ErrorReporter](Cube.ErrorReporter.md)

Logs exceptions caught in one game-facing handler, throttled with [ErrorThrottle](Cube.Core.ErrorThrottle.md#Cube_Core_ErrorThrottle) so a handler that
fails every frame logs about once every ten seconds (CLAUDE.md, "Dalamud rules": wrap every game-facing handler in
try/catch and log; never let an exception reach the game).

 [Plugin](Cube.Plugin.md)

Entry point. Registers the <code>/cube</code> command, the cube and settings windows, and logs which build loaded.

 [SettingsWindow](Cube.SettingsWindow.md)

The settings window, opened with <code>/cube config</code> or Dalamud's settings button (PROJECT.md, "Features": cube
size, orientation mode, labels on/off, colours and opacity; "NPCs": NPCs on/off and their colour; "View orientation
control": the control's corner and a way to reset the view; #32: collision on/off and detail).
