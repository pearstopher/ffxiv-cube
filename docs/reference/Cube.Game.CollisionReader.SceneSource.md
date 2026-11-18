# <a id="Cube_Game_CollisionReader_SceneSource"></a> Struct CollisionReader.SceneSource

Namespace: [Cube.Game](Cube.Game.md)  
Assembly: Cube.dll  

The game's collision scene as an [ICollisionSource](Cube.Core.ICollisionSource.md#Cube_Core_ICollisionSource). Lives for one frame only (made fresh in
[Update](Cube.Game.CollisionReader.md#Cube_Game_CollisionReader_Update)), so the collider pointer it holds is never carried into a later frame.

```csharp
private struct CollisionReader.SceneSource : ICollisionSource
```

#### Implements

[ICollisionSource](Cube.Core.ICollisionSource.md)


## Constructors

### <a id="Cube_Game_CollisionReader_SceneSource__ctor_FFXIVClientStructs_FFXIV_Common_Component_BGCollision_Scene__System_Int32_"></a> SceneSource\(Scene\*, int\)

The game's collision scene as an [ICollisionSource](Cube.Core.ICollisionSource.md#Cube_Core_ICollisionSource). Lives for one frame only (made fresh in
[Update](Cube.Game.CollisionReader.md#Cube_Game_CollisionReader_Update)), so the collider pointer it holds is never carried into a later frame.

```csharp
public SceneSource(Scene* scene, int detailDepth)
```

#### Parameters

`scene` Scene\*

The collision scene.

`detailDepth` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The deepest node-tree level offered ([MaxNodeDepth](Cube.Core.CollisionDetails.md#Cube_Core_CollisionDetails_MaxNodeDepth), from the detail setting).

## Fields

### <a id="Cube_Game_CollisionReader_SceneSource_MaxDepth"></a> MaxDepth

Deepest mesh node tree walked. The trees are balanced binary trees of a few hundred nodes; the cap only
stops a bad pointer recursing forever.

```csharp
private const int MaxDepth = 64
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Game_CollisionReader_SceneSource_current"></a> current

The collider [OfferNext](Cube.Game.CollisionReader.SceneSource.md#Cube_Game_CollisionReader_SceneSource_OfferNext) reads next, or null at the end of the list.

```csharp
private Collider* current
```

#### Field Value

 Collider\*

## Properties

### <a id="Cube_Game_CollisionReader_SceneSource_Now"></a> Now

The source's clock, in the ticks the cache's budget is given in (Stopwatch ticks in the plugin).

```csharp
public readonly long Now { get; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Cube_Game_CollisionReader_SceneSource_PlaneMax"></a> PlaneMax

A plane collider's high corner.

```csharp
private static Vector3 PlaneMax { get; }
```

#### Property Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Game_CollisionReader_SceneSource_PlaneMin"></a> PlaneMin

A plane collider's low corner: it is flat in its own Z, (-1,-1,0) to (1,1,0).

```csharp
private static Vector3 PlaneMin { get; }
```

#### Property Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Game_CollisionReader_SceneSource_UnitMin"></a> UnitMin

The low corner of a box, cylinder or sphere collider's unit shape: (-1,-1,-1) to (1,1,1).

```csharp
private static Vector3 UnitMin { get; }
```

#### Property Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

## Methods

### <a id="Cube_Game_CollisionReader_SceneSource_OfferMesh_FFXIVClientStructs_FFXIV_Common_Component_BGCollision_ColliderMesh__Cube_Core_CollisionBoxSet_System_Int32_"></a> OfferMesh\(ColliderMesh\*, CollisionBoxSet, int\)

Offers a mesh collider: skipped whole if its world box misses the region (the cheap first cull), otherwise
one box per node holding triangles, down to the detail setting's depth, through the collider's world matrix.

```csharp
private static void OfferMesh(ColliderMesh* mesh, CollisionBoxSet set, int detailDepth)
```

#### Parameters

`mesh` ColliderMesh\*

The mesh collider.

`set` [CollisionBoxSet](Cube.Core.CollisionBoxSet.md)

The set being gathered.

`detailDepth` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The deepest tree level offered.

### <a id="Cube_Game_CollisionReader_SceneSource_OfferNext_Cube_Core_CollisionBoxSet_"></a> OfferNext\(CollisionBoxSet\)

Offers every box of the current collider that reaches the set's region ([Overlaps](Cube.Core.CollisionBoxSet.md#Cube_Core_CollisionBoxSet_Overlaps)
lets it skip whole colliders and subtrees first), then moves to the next collider.

```csharp
public bool OfferNext(CollisionBoxSet set)
```

#### Parameters

`set` [CollisionBoxSet](Cube.Core.CollisionBoxSet.md)

The set being gathered.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

False if there was no current collider: the walk is complete.

#### Remarks

Colliders the player can't bump into are skipped ([Blocks](Cube.Core.CollisionFilter.md#Cube_Core_CollisionFilter_Blocks): raycast-inactive,
off the movement layer, or a primitive without the blocking material bit; #28, #107). Streamed colliders
only load and unload terrain meshes, which are in the list themselves, so they add no box.

### <a id="Cube_Game_CollisionReader_SceneSource_OfferNode_FFXIVClientStructs_FFXIV_Common_Component_BGCollision_MeshPCB_FileNode__System_Numerics_Matrix4x4__Cube_Core_CollisionBoxSet_System_Int32_System_Int32_"></a> OfferNode\(FileNode\*, in Matrix4x4, CollisionBoxSet, int, int\)

Offers one mesh node's box if it holds triangles, then its children, skipping any subtree whose box misses
the region (#28: that is what keeps the walk to a few hundred nodes). At the detail setting's depth the
node's box stands for its whole subtree (it holds its children's boxes), so the walk stops there.

```csharp
private static void OfferNode(MeshPCB.FileNode* node, in Matrix4x4 world, CollisionBoxSet set, int depth, int detailDepth)
```

#### Parameters

`node` MeshPCB.FileNode\*

The node, or null.

`world` [Matrix4x4](https://learn.microsoft.com/dotnet/api/system.numerics.matrix4x4)

The mesh's world matrix.

`set` [CollisionBoxSet](Cube.Core.CollisionBoxSet.md)

The set being gathered.

`depth` [int](https://learn.microsoft.com/dotnet/api/system.int32)

How deep in the tree; capped at [MaxDepth](Cube.Game.CollisionReader.SceneSource.md#Cube_Game_CollisionReader_SceneSource_MaxDepth).

`detailDepth` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The deepest tree level offered (#32).

### <a id="Cube_Game_CollisionReader_SceneSource_OfferUnit_FFXIVClientStructs_FFXIV_Common_Component_BGCollision_Math_Matrix4x3__System_Numerics_Vector3_System_Numerics_Vector3_Cube_Core_CollisionBoxSet_"></a> OfferUnit\(in Matrix4x3, Vector3, Vector3, CollisionBoxSet\)

Offers a primitive collider: its unit shape through its world matrix, if it reaches the region.

```csharp
private static void OfferUnit(in Matrix4x3 world, Vector3 localMin, Vector3 localMax, CollisionBoxSet set)
```

#### Parameters

`world` Matrix4x3

The collider's world matrix.

`localMin` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The shape's low corner in its own space.

`localMax` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The shape's high corner in its own space.

`set` [CollisionBoxSet](Cube.Core.CollisionBoxSet.md)

The set being gathered.

### <a id="Cube_Game_CollisionReader_SceneSource_Seek_System_Int32_"></a> Seek\(int\)

Moves to the collider at <code class="paramref">index</code> in the scene's list, ready for [OfferNext](Cube.Core.ICollisionSource.md#Cube_Core_ICollisionSource_OfferNext).

```csharp
public bool Seek(int index)
```

#### Parameters

`index` [int](https://learn.microsoft.com/dotnet/api/system.int32)

How many colliders to skip from the start of the list.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

False if the list has fewer colliders than that.
