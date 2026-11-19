# <a id="Cube_Game"></a> Namespace Cube.Game

**Reading the game.** Small, read-only readers that turn game memory into plain values for `Cube.Core`: the camera's direction now, nearby players later. They only read memory through FFXIVClientStructs on the game's main thread; they hook nothing, call no game functions that change anything, and allocate nothing per frame (PROJECT.md, "Read-only game access" and "Efficiency and memory"). Where each value comes from, and why, is in `docs/research-notes.md` (#10).

### Classes

 [CameraReader](Cube.Game.CameraReader.md)

Reads which way the game camera is looking, for the Camera orientation mode (PROJECT.md, "View": the view's
yaw follows the game camera so "forward on screen" matches the game view).

 [CollisionReader](Cube.Game.CollisionReader.md)

Reads the collision the game client already has loaded around the player into a [CollisionCache](Cube.Core.CollisionCache.md#Cube_Core_CollisionCache)
(PROJECT.md, "Collision (phase 2)"), the way #28 recommends (<code>docs/research-notes.md</code>, "Where to get
collision data"): walk the collision scene's colliders, take mesh colliders through their node trees and primitive
colliders through their world matrix, culling against the cube's region first.

 [GameStateReader](Cube.Game.GameStateReader.md)

Reads the states that hide the cube (PROJECT.md, "When hidden") from Dalamud, using the sources chosen in #10
(<code>docs/research-notes.md</code>, "When to hide"): `Dalamud.Plugin.Services.IClientState.IsLoggedIn`,
`Dalamud.Plugin.Services.IClientState.IsGPosing` and `Dalamud.Plugin.Services.IClientState.IsPvP`, and the loading-screen and cutscene
condition flags.

 [PlayerReader](Cube.Game.PlayerReader.md)

Reads the local player, the players around them and the named NPCs straight from the game's object manager and
offers each to [PlayerSet](Cube.Core.PlayerSet.md#Cube_Core_PlayerSet) (PROJECT.md, "Players" and "NPCs"). The approach chosen in #10
(<code>docs/research-notes.md</code>): FFXIVClientStructs, not Dalamud's object table, because it allocates nothing per
frame.

 [CollisionReader.RefineState](Cube.Game.CollisionReader.RefineState.md)

Everything the node splitting keeps between frames (#127): the cache of pieces, the scratch space it works in
(allocated once, so splitting allocates nothing per frame), and this frame's triangle budget.

### Structs

 [CollisionReader.SceneSource](Cube.Game.CollisionReader.SceneSource.md)

The game's collision scene as an [ICollisionSource](Cube.Core.ICollisionSource.md#Cube_Core_ICollisionSource). Lives for one frame only (made fresh in
[Update](Cube.Game.CollisionReader.md#Cube_Game_CollisionReader_Update)), so the collider pointer it holds is never carried into a later frame.
