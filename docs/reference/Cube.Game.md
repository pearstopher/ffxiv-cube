# <a id="Cube_Game"></a> Namespace Cube.Game

**Reading the game.** Small, read-only readers that turn game memory into plain values for `Cube.Core`: the camera's direction now, nearby players later. They only read memory through FFXIVClientStructs on the game's main thread; they hook nothing, call no game functions that change anything, and allocate nothing per frame (PROJECT.md, "Read-only game access" and "Efficiency and memory"). Where each value comes from, and why, is in `docs/research-notes.md` (#10).

### Classes

 [CameraReader](Cube.Game.CameraReader.md)

Reads which way the game camera is looking, for the Camera orientation mode (PROJECT.md, "View": the view's
yaw follows the game camera so "forward on screen" matches the game view).
