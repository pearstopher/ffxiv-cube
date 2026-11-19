# <a id="Cube_Game_MeshReader_SceneSource"></a> Struct MeshReader.SceneSource

Namespace: [Cube.Game](Cube.Game.md)  
Assembly: Cube.dll  

The game's collision scene as an [ITriangleSource](Cube.Core.ITriangleSource.md#Cube_Core_ITriangleSource); made fresh each frame so no collider pointer outlives it.

```csharp
private struct MeshReader.SceneSource : ITriangleSource
```

#### Implements

[ITriangleSource](Cube.Core.ITriangleSource.md)


## Constructors

### <a id="Cube_Game_MeshReader_SceneSource__ctor_FFXIVClientStructs_FFXIV_Common_Component_BGCollision_Scene__"></a> SceneSource\(Scene\*\)

The game's collision scene as an [ITriangleSource](Cube.Core.ITriangleSource.md#Cube_Core_ITriangleSource); made fresh each frame so no collider pointer outlives it.

```csharp
public SceneSource(Scene* scene)
```

#### Parameters

`scene` Scene\*

The collision scene.

## Fields

### <a id="Cube_Game_MeshReader_SceneSource_MaxDepth"></a> MaxDepth

Deepest mesh node tree walked; only stops a bad pointer recursing forever.

```csharp
private const int MaxDepth = 64
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Game_MeshReader_SceneSource_current"></a> current

The collider [OfferNext](Cube.Game.MeshReader.SceneSource.md#Cube_Game_MeshReader_SceneSource_OfferNext) reads next, or null at the end of the list.

```csharp
private Collider* current
```

#### Field Value

 Collider\*

## Properties

### <a id="Cube_Game_MeshReader_SceneSource_Now"></a> Now

The source's clock, in the ticks the cache's budget is given in.

```csharp
public readonly long Now { get; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

## Methods

### <a id="Cube_Game_MeshReader_SceneSource_OfferMesh_FFXIVClientStructs_FFXIV_Common_Component_BGCollision_ColliderMesh__Cube_Core_TriangleSet_"></a> OfferMesh\(ColliderMesh\*, TriangleSet\)

Reads a mesh collider's triangles unless its world box misses the region.

```csharp
private static void OfferMesh(ColliderMesh* mesh, TriangleSet set)
```

#### Parameters

`mesh` ColliderMesh\*

The mesh collider.

`set` [TriangleSet](Cube.Core.TriangleSet.md)

The set being gathered.

### <a id="Cube_Game_MeshReader_SceneSource_OfferNext_Cube_Core_TriangleSet_"></a> OfferNext\(TriangleSet\)

Offers every triangle of the current collider that reaches the set's region, then moves to the next collider.

```csharp
public bool OfferNext(TriangleSet set)
```

#### Parameters

`set` [TriangleSet](Cube.Core.TriangleSet.md)

The set being gathered.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

False if there was no current collider: the walk is complete.

### <a id="Cube_Game_MeshReader_SceneSource_OfferNode_FFXIVClientStructs_FFXIV_Common_Component_BGCollision_MeshPCB_FileNode__System_Numerics_Matrix4x4__Cube_Core_TriangleSet_System_Int32_"></a> OfferNode\(FileNode\*, in Matrix4x4, TriangleSet, int\)

Reads one node's triangles and then its children's, skipping any subtree whose box misses the region.

```csharp
private static void OfferNode(MeshPCB.FileNode* node, in Matrix4x4 world, TriangleSet set, int depth)
```

#### Parameters

`node` MeshPCB.FileNode\*

The node, or null.

`world` [Matrix4x4](https://learn.microsoft.com/dotnet/api/system.numerics.matrix4x4)

The mesh's world matrix.

`set` [TriangleSet](Cube.Core.TriangleSet.md)

The set being gathered.

`depth` [int](https://learn.microsoft.com/dotnet/api/system.int32)

How deep in the tree; capped at [MaxDepth](Cube.Game.MeshReader.SceneSource.md#Cube_Game_MeshReader_SceneSource_MaxDepth).

### <a id="Cube_Game_MeshReader_SceneSource_Seek_System_Int32_"></a> Seek\(int\)

Moves to the collider at <code class="paramref">index</code> in the scene's list.

```csharp
public bool Seek(int index)
```

#### Parameters

`index` [int](https://learn.microsoft.com/dotnet/api/system.int32)

How many colliders to skip from the start of the list.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

False if the list has fewer colliders than that.

### <a id="Cube_Game_MeshReader_SceneSource_UnitReaches_FFXIVClientStructs_FFXIV_Common_Component_BGCollision_Math_Matrix4x3__System_Numerics_Vector3_System_Numerics_Vector3_Cube_Core_TriangleSet_"></a> UnitReaches\(in Matrix4x3, Vector3, Vector3, TriangleSet\)

Whether a primitive's unit shape placed by its matrix reaches the region: the cull before tessellating it.

```csharp
private static bool UnitReaches(in Matrix4x3 world, Vector3 localMin, Vector3 localMax, TriangleSet set)
```

#### Parameters

`world` Matrix4x3

The collider's world matrix.

`localMin` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The shape's low corner in its own space.

`localMax` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The shape's high corner in its own space.

`set` [TriangleSet](Cube.Core.TriangleSet.md)

The set being gathered.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)
