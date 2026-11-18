# <a id="Cube_TestBridge_Checks_CollisionProbeCheck_Sample"></a> Struct CollisionProbeCheck.Sample

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

One frame's measurements.

```csharp
private struct CollisionProbeCheck.Sample
```


## Fields

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_Sample_BoxCells"></a> BoxCells

Cells covered by leaf node boxes.

```csharp
public int BoxCells
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_Sample_Centre"></a> Centre

The player's position: the cube's centre.

```csharp
public Vector3 Centre
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_Sample_CollidersBytes"></a> CollidersBytes

Bytes allocated by the collider walk.

```csharp
public long CollidersBytes
```

#### Field Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_Sample_CollidersMs"></a> CollidersMs

Time for the collider walk alone at the largest cube.

```csharp
public double CollidersMs
```

#### Field Value

 [double](https://learn.microsoft.com/dotnet/api/system.double)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_Sample_Default"></a> Default

Counts at the default cube.

```csharp
public CollisionProbeCheck.CubeCounts Default
```

#### Field Value

 [CollisionProbeCheck](Cube.TestBridge.Checks.CollisionProbeCheck.md).[CubeCounts](Cube.TestBridge.Checks.CollisionProbeCheck.CubeCounts.md)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_Sample_Error"></a> Error

Why nothing could be read, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>.

```csharp
public string? Error
```

#### Field Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)?

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_Sample_Max"></a> Max

Counts at the largest cube.

```csharp
public CollisionProbeCheck.CubeCounts Max
```

#### Field Value

 [CollisionProbeCheck](Cube.TestBridge.Checks.CollisionProbeCheck.md).[CubeCounts](Cube.TestBridge.Checks.CollisionProbeCheck.CubeCounts.md)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_Sample_NodesBytes"></a> NodesBytes

Bytes allocated by the node walk.

```csharp
public long NodesBytes
```

#### Field Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_Sample_NodesMs"></a> NodesMs

Time for the collider walk plus node walk at the largest cube.

```csharp
public double NodesMs
```

#### Field Value

 [double](https://learn.microsoft.com/dotnet/api/system.double)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_Sample_RaysMs"></a> RaysMs

Time for the 400-ray grid.

```csharp
public double RaysMs
```

#### Field Value

 [double](https://learn.microsoft.com/dotnet/api/system.double)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_Sample_Scenes"></a> Scenes

Collision scenes.

```csharp
public int Scenes
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_Sample_Streaming"></a> Streaming

The collision module's streaming sphere (xyz centre, w radius): how far around the camera collision is loaded.

```csharp
public Vector4 Streaming
```

#### Field Value

 [Vector4](https://learn.microsoft.com/dotnet/api/system.numerics.vector4)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_Sample_Total"></a> Total

Colliders in all scenes.

```csharp
public int Total
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_Sample_TriangleCells"></a> TriangleCells

Cells touched by a triangle's bounds.

```csharp
public int TriangleCells
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_Sample_TypeCounts"></a> TypeCounts

All colliders by type.

```csharp
public CollisionProbeCheck.TypeCounts TypeCounts
```

#### Field Value

 [CollisionProbeCheck](Cube.TestBridge.Checks.CollisionProbeCheck.md).[TypeCounts](Cube.TestBridge.Checks.CollisionProbeCheck.TypeCounts.md)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_Sample_VoxelBytes"></a> VoxelBytes

Bytes allocated by the voxel walk.

```csharp
public long VoxelBytes
```

#### Field Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_Sample_VoxelMs"></a> VoxelMs

Time for a full walk that also marks both voxel grids (every triangle in the cube).

```csharp
public double VoxelMs
```

#### Field Value

 [double](https://learn.microsoft.com/dotnet/api/system.double)
