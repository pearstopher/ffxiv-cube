# <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Scan"></a> Class CollisionFitProbeCheck.Scan

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

What [Examine](Cube.TestBridge.Checks.CollisionFitProbeCheck.md#Cube_TestBridge_Checks_CollisionFitProbeCheck_Examine) found.

```csharp
private sealed class CollisionFitProbeCheck.Scan
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[CollisionFitProbeCheck.Scan](Cube.TestBridge.Checks.CollisionFitProbeCheck.Scan.md)


## Fields

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Scan_AreaMax"></a> AreaMax

High corner of the examined region.

```csharp
public Vector3 AreaMax
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Scan_AreaMin"></a> AreaMin

Low corner of the examined region.

```csharp
public Vector3 AreaMin
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Scan_BigNodeVolume"></a> BigNodeVolume

Their drawn volume.

```csharp
public float BigNodeVolume
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Scan_BigNodes"></a> BigNodes

Nodes with a longest side of at least 3 yalms.

```csharp
public int BigNodes
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Scan_BigSweepBoxes"></a> BigSweepBoxes

Their box count after the sweep.

```csharp
public int BigSweepBoxes
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Scan_BigSweepMilliseconds"></a> BigSweepMilliseconds

Sweep time spent on those nodes.

```csharp
public double BigSweepMilliseconds
```

#### Field Value

 [double](https://learn.microsoft.com/dotnet/api/system.double)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Scan_BigSweepVolume"></a> BigSweepVolume

Their volume after the sweep.

```csharp
public float BigSweepVolume
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Scan_BigTriangles"></a> BigTriangles

Triangles in those nodes.

```csharp
public int BigTriangles
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Scan_Centre"></a> Centre

The player's position.

```csharp
public Vector3 Centre
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Scan_ComponentVolume"></a> ComponentVolume

Total volume of the gap-split boxes.

```csharp
public float ComponentVolume
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Scan_Components"></a> Components

Boxes if every node were split at gaps.

```csharp
public int Components
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Scan_Histogram"></a> Histogram

Nodes per band of (gap-split volume / node volume).

```csharp
public int[] Histogram
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)\[\]

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Scan_Inner"></a> Inner

Of those, nodes that also have children.

```csharp
public int Inner
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Scan_InnerComponentVolume"></a> InnerComponentVolume

Gap-split volume of inner nodes only.

```csharp
public float InnerComponentVolume
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Scan_InnerEqualsChildren"></a> InnerEqualsChildren

Inner nodes whose box is no bigger than their children's boxes together.

```csharp
public int InnerEqualsChildren
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Scan_InnerNodeVolume"></a> InnerNodeVolume

Node-box volume of inner nodes only.

```csharp
public float InnerNodeVolume
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Scan_InnerTightVolume"></a> InnerTightVolume

Tight-box volume of inner nodes only.

```csharp
public float InnerTightVolume
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Scan_Leaf"></a> Leaf

Of those, nodes with no children.

```csharp
public int Leaf
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Scan_Lines"></a> Lines

Log lines to write.

```csharp
public List<string> Lines
```

#### Field Value

 [List](https://learn.microsoft.com/dotnet/api/system.collections.generic.list\-1)<[string](https://learn.microsoft.com/dotnet/api/system.string)\>

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Scan_Meshes"></a> Meshes

Blocking meshes in the region.

```csharp
public int Meshes
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Scan_Milliseconds"></a> Milliseconds

Wall-clock time of the whole analysis.

```csharp
public double Milliseconds
```

#### Field Value

 [double](https://learn.microsoft.com/dotnet/api/system.double)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Scan_NodeVolume"></a> NodeVolume

Total volume of the node boxes drawn today.

```csharp
public float NodeVolume
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Scan_Nodes"></a> Nodes

Nodes with triangles analysed.

```csharp
public int Nodes
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Scan_OverTheLimit"></a> OverTheLimit

Nodes with more triangles than the reader splits (they stay their coarse box); 0 is the goal (#129).

```csharp
public int OverTheLimit
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Scan_RayHits"></a> RayHits

Of those, boxes with a ray hit inside.

```csharp
public int RayHits
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Scan_RayTested"></a> RayTested

Wasteful boxes ray-tested.

```csharp
public int RayTested
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Scan_SkippedTriangles"></a> SkippedTriangles

Triangles too large to fill in.

```csharp
public int SkippedTriangles
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Scan_Split"></a> Split

Nodes whose triangles fall into more than one group of cells.

```csharp
public int Split
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Scan_SweepBoxes"></a> SweepBoxes

Boxes from the triangle sweep over all nodes.

```csharp
public int SweepBoxes
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Scan_SweepMilliseconds"></a> SweepMilliseconds

Time spent in the sweep itself (Debug build, allocating: an upper bound).

```csharp
public double SweepMilliseconds
```

#### Field Value

 [double](https://learn.microsoft.com/dotnet/api/system.double)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Scan_SweepRayHits"></a> SweepRayHits

Of those, boxes with a ray hit inside.

```csharp
public int SweepRayHits
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Scan_SweepRayTested"></a> SweepRayTested

Sweep boxes of the wasteful nodes that were ray-tested.

```csharp
public int SweepRayTested
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Scan_SweepSplit"></a> SweepSplit

Nodes the sweep split into more than one box.

```csharp
public int SweepSplit
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Scan_SweepVolume"></a> SweepVolume

Total volume of the sweep's boxes.

```csharp
public float SweepVolume
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Scan_TightVolume"></a> TightVolume

Total volume of the tight boxes of own triangles.

```csharp
public float TightVolume
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Scan_Triangles"></a> Triangles

Own triangles read.

```csharp
public int Triangles
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Scan_Worst"></a> Worst

Worst-fitting nodes with the volume they waste, for the sorted listing.

```csharp
public List<(float Waste, string Text)> Worst
```

#### Field Value

 [List](https://learn.microsoft.com/dotnet/api/system.collections.generic.list\-1)<\([float](https://learn.microsoft.com/dotnet/api/system.single) [Waste](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.single,system.string\-.waste), [string](https://learn.microsoft.com/dotnet/api/system.string) [Text](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.single,system.string\-.text)\)\>
