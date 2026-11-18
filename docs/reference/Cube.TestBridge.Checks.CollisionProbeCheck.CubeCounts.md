# <a id="Cube_TestBridge_Checks_CollisionProbeCheck_CubeCounts"></a> Struct CollisionProbeCheck.CubeCounts

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

What one walk found inside one cube size.

```csharp
private struct CollisionProbeCheck.CubeCounts
```


## Fields

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_CubeCounts_Colliders"></a> Colliders

Colliders whose world box crosses the cube.

```csharp
public int Colliders
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_CubeCounts_ExtentSum"></a> ExtentSum

Sum of the crossing colliders' largest sides, for the mean.

```csharp
public float ExtentSum
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_CubeCounts_LeafExtentSum"></a> LeafExtentSum

Sum of those nodes' largest sides, for the mean.

```csharp
public float LeafExtentSum
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_CubeCounts_Leaves"></a> Leaves

Mesh nodes holding triangles whose box crosses the cube.

```csharp
public int Leaves
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_CubeCounts_MaxExtent"></a> MaxExtent

The largest side of any crossing collider's box.

```csharp
public float MaxExtent
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_CubeCounts_MaxLeafExtent"></a> MaxLeafExtent

The largest side of any of those nodes' boxes.

```csharp
public float MaxLeafExtent
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_CubeCounts_NodesVisited"></a> NodesVisited

Mesh nodes looked at (including those skipped as outside).

```csharp
public int NodesVisited
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_CubeCounts_NotRaycastable"></a> NotRaycastable

Crossing colliders not active for raycasts (visibility flag 1 clear).

```csharp
public int NotRaycastable
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_CubeCounts_OffLayer0"></a> OffLayer0

Crossing colliders not on layer 0, the layer the raycast uses.

```csharp
public int OffLayer0
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_CubeCounts_Triangles"></a> Triangles

Triangles in those nodes.

```csharp
public int Triangles
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_CubeCounts_TypeCounts"></a> TypeCounts

Those colliders by type.

```csharp
public CollisionProbeCheck.TypeCounts TypeCounts
```

#### Field Value

 [CollisionProbeCheck](Cube.TestBridge.Checks.CollisionProbeCheck.md).[TypeCounts](Cube.TestBridge.Checks.CollisionProbeCheck.TypeCounts.md)

## Properties

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_CubeCounts_MeanExtent"></a> MeanExtent

Gets the mean largest side of the crossing colliders' boxes.

```csharp
public readonly float MeanExtent { get; }
```

#### Property Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_CubeCounts_MeanLeafExtent"></a> MeanLeafExtent

Gets the mean largest side of the crossing leaf nodes' boxes.

```csharp
public readonly float MeanLeafExtent { get; }
```

#### Property Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)
