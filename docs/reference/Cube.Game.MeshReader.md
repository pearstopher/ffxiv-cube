# <a id="Cube_Game_MeshReader"></a> Class MeshReader

Namespace: [Cube.Game](Cube.Game.md)  
Assembly: Cube.dll  

Reads the collision triangles the game client has loaded around the player into a [TriangleCache](Cube.Core.TriangleCache.md#Cube_Core_TriangleCache) for
the Mesh collision style (#100; PROJECT.md, "Collision style"): mesh colliders' triangles through their world matrix,
and primitive colliders as their real shapes in triangles ([PrimitiveMeshes](Cube.Core.PrimitiveMeshes.md#Cube_Core_PrimitiveMeshes)).

```csharp
internal sealed class MeshReader
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[MeshReader](Cube.Game.MeshReader.md)


## Remarks

<p>
The same walk as [CollisionReader](Cube.Game.CollisionReader.md#Cube_Game_CollisionReader) (<code>docs/research-notes.md</code>, #28 and #99): the same colliders are
kept ([Blocks](Cube.Core.CollisionFilter.md#Cube_Core_CollisionFilter_Blocks)), whole colliders and tree nodes outside the gathered region are skipped
before their triangles are read, and the walk is spread over frames by [TriangleCache](Cube.Core.TriangleCache.md#Cube_Core_TriangleCache). Read-only: plain
memory reads plus the read-only `FFXIVClientStructs.FFXIV.Common.Component.BGCollision.Collider.GetColliderType`; no game pointer is kept between frames.
Nothing is simplified: every triangle of every node that reaches the region is kept, and the cull only rejects
triangles that can't touch the cube.
</p>

## Fields

### <a id="Cube_Game_MeshReader_reportedDropped"></a> reportedDropped

The most triangles dropped for lack of room already logged, so a larger scene is logged again but a repeat isn't.

```csharp
private int reportedDropped
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Properties

### <a id="Cube_Game_MeshReader_Cache"></a> Cache

The cache the walk fills; read by the clipping and the test bridge.

```csharp
public TriangleCache Cache { get; }
```

#### Property Value

 [TriangleCache](Cube.Core.TriangleCache.md)

### <a id="Cube_Game_MeshReader_SimulatedZone"></a> SimulatedZone

A zone id the bridge puts in place of the real one (see [SimulatedZone](Cube.Game.CollisionReader.md#Cube_Game_CollisionReader_SimulatedZone)). Debug builds only.

```csharp
internal uint? SimulatedZone { get; set; }
```

#### Property Value

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)?

## Methods

### <a id="Cube_Game_MeshReader_FindScene"></a> FindScene\(\)

The collision scene, or null if the module or scene isn't there.

```csharp
private static Scene* FindScene()
```

#### Returns

 Scene\*

### <a id="Cube_Game_MeshReader_Release"></a> Release\(\)

Drops the triangles and their buffers, when the style goes back to Boxes or collision is turned off.

```csharp
public void Release()
```

### <a id="Cube_Game_MeshReader_Stamp_FFXIVClientStructs_FFXIV_Common_Component_BGCollision_Scene__"></a> Stamp\(Scene\*\)

What identifies the loaded collision: the first collider's address and the collider count (as in [CollisionReader](Cube.Game.CollisionReader.md#Cube_Game_CollisionReader)).

```csharp
private static ulong Stamp(Scene* scene)
```

#### Parameters

`scene` Scene\*

The collision scene.

#### Returns

 [ulong](https://learn.microsoft.com/dotnet/api/system.uint64)

### <a id="Cube_Game_MeshReader_Update_System_Numerics_Vector3_System_Numerics_Vector3_"></a> Update\(Vector3, Vector3\)

One frame's mesh work: drops the cache on a zone change, and gathers (or continues gathering) when it is stale.
Called from the cube window's draw path only while Mesh is the chosen style and the cube is shown.

```csharp
public bool Update(Vector3 centre, Vector3 size)
```

#### Parameters

`centre` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The cube's centre.

`size` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The cube's width, height and depth in yalms.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

True if a gather completed this frame.
