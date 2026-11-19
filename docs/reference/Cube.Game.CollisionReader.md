# <a id="Cube_Game_CollisionReader"></a> Class CollisionReader

Namespace: [Cube.Game](Cube.Game.md)  
Assembly: Cube.dll  

Reads the collision the game client already has loaded around the player into a [CollisionCache](Cube.Core.CollisionCache.md#Cube_Core_CollisionCache)
(PROJECT.md, "Collision (phase 2)"), the way #28 recommends (<code>docs/research-notes.md</code>, "Where to get
collision data"): walk the collision scene's colliders, take mesh colliders through their node trees and primitive
colliders through their world matrix, culling against the cube's region first.

```csharp
internal sealed class CollisionReader
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[CollisionReader](Cube.Game.CollisionReader.md)


## Remarks

<p>
<b>Read-only</b> (PROJECT.md, "Read-only game access"): plain memory reads plus the read-only
`FFXIVClientStructs.FFXIV.Common.Component.BGCollision.Collider.GetColliderType` getter, on the framework thread like the #28 probe. Nothing is written,
hooked or sent, and nothing depends on vnavmesh.
</p>
<p>
<b>Spread over frames</b> by [CollisionCache](Cube.Core.CollisionCache.md#Cube_Core_CollisionCache), which stops between colliders when the frame's budget
is spent. No game pointer is kept between frames: each frame finds its place again by index from the start of the
list, so a collider unloaded in between is never touched.
</p>

## Fields

### <a id="Cube_Game_CollisionReader_hasZone"></a> hasZone

Whether [zone](Cube.Game.CollisionReader.md#Cube_Game_CollisionReader_zone) has been set.

```csharp
private bool hasZone
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Game_CollisionReader_refine"></a> refine

The node-splitting state shared by every frame's [SceneSource](Cube.Game.CollisionReader.SceneSource.md#Cube_Game_CollisionReader_SceneSource) (#127).

```csharp
private readonly CollisionReader.RefineState refine
```

#### Field Value

 [CollisionReader](Cube.Game.CollisionReader.md).[RefineState](Cube.Game.CollisionReader.RefineState.md)

### <a id="Cube_Game_CollisionReader_zone"></a> zone

The zone the split-piece cache belongs to.

```csharp
private uint zone
```

#### Field Value

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

## Properties

### <a id="Cube_Game_CollisionReader_Cache"></a> Cache

The cache the walk fills; read by the drawing code (#31) and the test bridge.

```csharp
public CollisionCache Cache { get; }
```

#### Property Value

 [CollisionCache](Cube.Core.CollisionCache.md)

### <a id="Cube_Game_CollisionReader_NodePieces"></a> NodePieces

The pieces mesh nodes were split into (#127); the test bridge reads its size.

```csharp
internal NodePieceCache NodePieces { get; }
```

#### Property Value

 [NodePieceCache](Cube.Core.NodePieceCache.md)

### <a id="Cube_Game_CollisionReader_SimulatedZone"></a> SimulatedZone

A zone id the test bridge puts in place of the real one, so zone changes can be checked without a human
teleporting (#33). Null uses the game's zone. The scene itself is still the real one. Debug builds only.

```csharp
internal uint? SimulatedZone { get; set; }
```

#### Property Value

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)?

## Methods

### <a id="Cube_Game_CollisionReader_FindScene"></a> FindScene\(\)

The collision scene, or null if the module or scene isn't there.

```csharp
private static Scene* FindScene()
```

#### Returns

 Scene\*

#### Remarks

The game always has exactly one scene (FFXIVClientStructs' <code>SceneManager.FirstScene</code> notes).

### <a id="Cube_Game_CollisionReader_Stamp_FFXIVClientStructs_FFXIV_Common_Component_BGCollision_Scene__"></a> Stamp\(Scene\*\)

What identifies the loaded collision: the first collider's address and the collider count, as #28 suggests.
Terrain streaming in or out, or objects being added or removed, changes one of them and so starts a new gather.

```csharp
private static ulong Stamp(Scene* scene)
```

#### Parameters

`scene` Scene\*

The collision scene.

#### Returns

 [ulong](https://learn.microsoft.com/dotnet/api/system.uint64)

### <a id="Cube_Game_CollisionReader_Update_System_Numerics_Vector3_System_Numerics_Vector3_Cube_Core_CollisionDetail_"></a> Update\(Vector3, Vector3, CollisionDetail\)

One frame's collision work: drops the cache on a zone change, and gathers (or continues gathering) when it is
stale. Called from the cube window's draw path only while the cube is shown, so nothing is read while hidden
(PROJECT.md, "When hidden").

```csharp
public bool Update(Vector3 centre, Vector3 size, CollisionDetail detail)
```

#### Parameters

`centre` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The cube's centre (the player).

`size` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The cube's width, height and depth in yalms.

`detail` [CollisionDetail](Cube.Core.CollisionDetail.md)

The detail setting (#32). Part of the stamp the cache compares, so a new level gathers again at once.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

True if a gather completed this frame.
