# <a id="Cube_Core_ClippedMesh"></a> Class ClippedMesh

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

The Mesh style's drawing data (#100; PROJECT.md, "Collision style"): the cached triangles' edges, clipped to the
cube, with exact duplicates removed, and projected to the window. Everything is rebuilt only when its inputs change,
and a rebuild is spread over frames while the previous result keeps being drawn.

```csharp
public sealed class ClippedMesh
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ClippedMesh](Cube.Core.ClippedMesh.md)


## Remarks

<p>
<b>What is shown.</b> Every edge of every cached triangle, clipped exactly to the cube ([SegmentClip](Cube.Core.SegmentClip.md#Cube_Core_SegmentClip)).
An edge shared by two triangles is stored once (an exact match of its clipped corners, so nothing that is drawn
changes); edges wholly outside the cube are skipped before anything else is computed for them.
</p>
<p>
<b>Rebuilt only when needed.</b> The inputs are the gathered triangle set (which set, and the cache's completed
count) and the cube's centre, size and yaw. The cube turns with the camera, so a turning camera is a new input every
frame; a rebuild reads tens of thousands of triangles, so it is done in slices of <code>budgetTicks</code> per frame. A
started rebuild is always finished (never restarted for a newer input, or a turning camera would never finish) and the
next one starts after it. Meanwhile the previous result is drawn: its edges are moved through the world into the current
cube and clipped to it again at projection time ([Project](Cube.Core.ClippedMesh.md#Cube_Core_ClippedMesh_Project)), so a stale result lags the camera by a few
frames but never shows anything outside the cube.
</p>
<p>
<b>Bounded.</b> Two edge buffers (built and ready, swapped) and a hash table are allocated on first use and dropped by
[Release](Cube.Core.ClippedMesh.md#Cube_Core_ClippedMesh_Release). An edge that doesn't fit is counted in [Dropped](Cube.Core.ClippedMesh.md#Cube_Core_ClippedMesh_Dropped) for the plugin to report; nothing
is cut silently. Nothing here allocates once the buffers exist.
</p>

## Constructors

### <a id="Cube_Core_ClippedMesh__ctor_System_Int32_"></a> ClippedMesh\(int\)

Creates an empty mesh; no buffer is allocated until the first build.

```csharp
public ClippedMesh(int edgeCapacity = 196608)
```

#### Parameters

`edgeCapacity` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Most unique edges kept per buffer.

## Fields

### <a id="Cube_Core_ClippedMesh_ClockInterval"></a> ClockInterval

How many triangles are processed between looks at the clock.

```csharp
private const int ClockInterval = 256
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_ClippedMesh_DefaultEdgeCapacity"></a> DefaultEdgeCapacity

Default most edges kept: a triangle has three edges and neighbours share them, so 131,072 triangles (the
[TriangleSet](Cube.Core.TriangleSet.md#Cube_Core_TriangleSet) default) give at most about 200,000 unique edges. 24 bytes each, 4.7 MB per buffer.

```csharp
public const int DefaultEdgeCapacity = 196608
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_ClippedMesh_buildCompleted"></a> buildCompleted

The cache's completed count the build started from.

```csharp
private long buildCompleted
```

#### Field Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Cube_Core_ClippedMesh_buildCount"></a> buildCount

Edges the build has kept so far.

```csharp
private int buildCount
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_ClippedMesh_buildCube"></a> buildCube

The cube the build in progress clips to.

```csharp
private CubeSpace buildCube
```

#### Field Value

 [CubeSpace](Cube.Core.CubeSpace.md)

### <a id="Cube_Core_ClippedMesh_buildDropped"></a> buildDropped

Edges the build has had to drop for lack of room.

```csharp
private int buildDropped
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_ClippedMesh_buildEdges"></a> buildEdges

The edges being built, for [buildCube](Cube.Core.ClippedMesh.md#Cube_Core_ClippedMesh_buildCube).

```csharp
private Vector3[]? buildEdges
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)\[\]?

### <a id="Cube_Core_ClippedMesh_buildSet"></a> buildSet

The set being read by the build.

```csharp
private TriangleSet? buildSet
```

#### Field Value

 [TriangleSet](Cube.Core.TriangleSet.md)?

### <a id="Cube_Core_ClippedMesh_building"></a> building

Whether a build is in progress.

```csharp
private bool building
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_ClippedMesh_edgeCapacity"></a> edgeCapacity

The edge capacity.

```csharp
private readonly int edgeCapacity
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_ClippedMesh_hasReady"></a> hasReady

Whether the ready buffer holds a finished build.

```csharp
private bool hasReady
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_ClippedMesh_nextTriangle"></a> nextTriangle

The next triangle the build processes.

```csharp
private int nextTriangle
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_ClippedMesh_projectedBuild"></a> projectedBuild

The number of the build the screen positions came from; -1 when none.

```csharp
private long projectedBuild
```

#### Field Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Cube_Core_ClippedMesh_projectedCube"></a> projectedCube

What the screen positions were last projected for: the cube, the view and the ready build's number.

```csharp
private CubeSpace projectedCube
```

#### Field Value

 [CubeSpace](Cube.Core.CubeSpace.md)

### <a id="Cube_Core_ClippedMesh_projectedView"></a> projectedView

The projection the screen positions were last made with.

```csharp
private ViewProjection projectedView
```

#### Field Value

 [ViewProjection](Cube.Core.ViewProjection.md)

### <a id="Cube_Core_ClippedMesh_readyCompleted"></a> readyCompleted

The cache's completed count when the ready edges were built.

```csharp
private long readyCompleted
```

#### Field Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Cube_Core_ClippedMesh_readyCount"></a> readyCount

How many edges the ready buffer holds.

```csharp
private int readyCount
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_ClippedMesh_readyCube"></a> readyCube

The cube the ready edges were clipped for.

```csharp
private CubeSpace readyCube
```

#### Field Value

 [CubeSpace](Cube.Core.CubeSpace.md)

### <a id="Cube_Core_ClippedMesh_readyEdges"></a> readyEdges

The edges' ends in cube-local coordinates of [readyCube](Cube.Core.ClippedMesh.md#Cube_Core_ClippedMesh_readyCube), two per edge.

```csharp
private Vector3[]? readyEdges
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)\[\]?

### <a id="Cube_Core_ClippedMesh_readySet"></a> readySet

The set the ready edges were built from.

```csharp
private TriangleSet? readySet
```

#### Field Value

 [TriangleSet](Cube.Core.TriangleSet.md)?

### <a id="Cube_Core_ClippedMesh_screen"></a> screen

The projected ends of the ready edges, two per kept edge.

```csharp
private Vector2[]? screen
```

#### Field Value

 [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)\[\]?

### <a id="Cube_Core_ClippedMesh_table"></a> table

The hash table of edge numbers plus one (0 is empty) used while building.

```csharp
private int[]? table
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)\[\]?

## Properties

### <a id="Cube_Core_ClippedMesh_Builds"></a> Builds

How many builds have finished.

```csharp
public long Builds { get; private set; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Cube_Core_ClippedMesh_BuiltFor"></a> BuiltFor

The cube the ready edges were clipped for.

```csharp
public CubeSpace BuiltFor { get; }
```

#### Property Value

 [CubeSpace](Cube.Core.CubeSpace.md)

### <a id="Cube_Core_ClippedMesh_Count"></a> Count

How many unique edges the ready build holds.

```csharp
public int Count { get; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_ClippedMesh_Dropped"></a> Dropped

How many edges the ready build couldn't keep for lack of room; plugin and bridge report it.

```csharp
public int Dropped { get; private set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_ClippedMesh_Edges"></a> Edges

The ready edges' ends in cube-local coordinates of [BuiltFor](Cube.Core.ClippedMesh.md#Cube_Core_ClippedMesh_BuiltFor), two per edge.

```csharp
public ReadOnlySpan<Vector3> Edges { get; }
```

#### Property Value

 [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)\>

### <a id="Cube_Core_ClippedMesh_IsAllocated"></a> IsAllocated

Whether the buffers are allocated.

```csharp
public bool IsAllocated { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_ClippedMesh_IsBuilding"></a> IsBuilding

Whether a build is in progress, so the picture lags the inputs.

```csharp
public bool IsBuilding { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_ClippedMesh_LargestDropped"></a> LargestDropped

The largest [Dropped](Cube.Core.ClippedMesh.md#Cube_Core_ClippedMesh_Dropped) of any finished build.

```csharp
public int LargestDropped { get; private set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_ClippedMesh_ProjectedCount"></a> ProjectedCount

How many segments the last [Project](Cube.Core.ClippedMesh.md#Cube_Core_ClippedMesh_Project) made (edges still inside the current cube); two corners each.

```csharp
public int ProjectedCount { get; private set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_ClippedMesh_Projections"></a> Projections

How many times [Project](Cube.Core.ClippedMesh.md#Cube_Core_ClippedMesh_Project) actually projected (not just reused its result).

```csharp
public long Projections { get; private set; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Cube_Core_ClippedMesh_Screen"></a> Screen

The projected segments' ends, two per segment, as of the last [Project](Cube.Core.ClippedMesh.md#Cube_Core_ClippedMesh_Project).

```csharp
public ReadOnlySpan<Vector2> Screen { get; }
```

#### Property Value

 [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)\>

## Methods

### <a id="Cube_Core_ClippedMesh_AddEdge_System_Numerics_Vector3_System_Numerics_Vector3_System_Int32_System_Int32_System_Numerics_Vector3_"></a> AddEdge\(Vector3, Vector3, int, int, Vector3\)

Clips one edge and keeps it unless an identical one is already kept. The ends are put in a fixed order first, so
the same edge met from either neighbouring triangle clips to the same numbers and matches exactly.

```csharp
private void AddEdge(Vector3 p, Vector3 q, int codeP, int codeQ, Vector3 half)
```

#### Parameters

`p` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

One end, in cube-local coordinates.

`q` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The other end.

`codeP` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The outcode of <code class="paramref">p</code>.

`codeQ` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The outcode of <code class="paramref">q</code>.

`half` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The cube's half extents.

### <a id="Cube_Core_ClippedMesh_AddTriangle_Cube_Core_CubeSpace__System_Numerics_Vector3_System_Numerics_Vector3_System_Numerics_Vector3_System_Numerics_Vector3_"></a> AddTriangle\(in CubeSpace, Vector3, Vector3, Vector3, Vector3\)

Clips a triangle's three edges to the cube and keeps those that remain and are new.

```csharp
private void AddTriangle(in CubeSpace cube, Vector3 half, Vector3 a, Vector3 b, Vector3 c)
```

#### Parameters

`cube` [CubeSpace](Cube.Core.CubeSpace.md)

The cube.

`half` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The cube's half extents.

`a` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

First corner, in world coordinates.

`b` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

Second corner.

`c` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

Third corner.

### <a id="Cube_Core_ClippedMesh_Before_System_Numerics_Vector3_System_Numerics_Vector3_"></a> Before\(Vector3, Vector3\)

Whether <code class="paramref">a</code> sorts before <code class="paramref">b</code> by X, then Y, then Z.

```csharp
private static bool Before(Vector3 a, Vector3 b)
```

#### Parameters

`a` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

One point.

`b` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The other.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_ClippedMesh_Begin_Cube_Core_CubeSpace__Cube_Core_TriangleSet_System_Int64_"></a> Begin\(in CubeSpace, TriangleSet, long\)

Allocates the buffers if needed and starts a build for a cube.

```csharp
private void Begin(in CubeSpace cube, TriangleSet set, long completed)
```

#### Parameters

`cube` [CubeSpace](Cube.Core.CubeSpace.md)

The cube to clip to.

`set` [TriangleSet](Cube.Core.TriangleSet.md)

The triangles.

`completed` [long](https://learn.microsoft.com/dotnet/api/system.int64)

The cache's completed count.

### <a id="Cube_Core_ClippedMesh_Build_System_Int64_"></a> Build\(long\)

Processes triangles until the budget is spent or they run out.

```csharp
private bool Build(long budgetTicks)
```

#### Parameters

`budgetTicks` [long](https://learn.microsoft.com/dotnet/api/system.int64)

The most ticks to spend.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

True if the build finished.

### <a id="Cube_Core_ClippedMesh_Clear"></a> Clear\(\)

Drops the result and any build in progress; buffers are kept.

```csharp
public void Clear()
```

### <a id="Cube_Core_ClippedMesh_Hash_System_Numerics_Vector3_System_Numerics_Vector3_"></a> Hash\(Vector3, Vector3\)

A hash of both ends' float bits.

```csharp
private static uint Hash(Vector3 p, Vector3 q)
```

#### Parameters

`p` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

First end.

`q` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

Second end.

#### Returns

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

### <a id="Cube_Core_ClippedMesh_Project_Cube_Core_CubeSpace__Cube_Core_ViewProjection__"></a> Project\(in CubeSpace, in ViewProjection\)

Projects the ready edges to the window, only when the cube, the view or the edges changed since the last call.
When the edges were clipped for an older cube (a rebuild is still going), each is carried into the current cube
through the world and clipped to it, so nothing is ever drawn outside the cube (PROJECT.md, "Collision style").

```csharp
public bool Project(in CubeSpace cube, in ViewProjection projection)
```

#### Parameters

`cube` [CubeSpace](Cube.Core.CubeSpace.md)

This frame's cube.

`projection` [ViewProjection](Cube.Core.ViewProjection.md)

This frame's projection.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

True if the screen segments were made again.

### <a id="Cube_Core_ClippedMesh_Release"></a> Release\(\)

Drops the buffers as well, so Boxes mode holds no Mesh memory.

```csharp
public void Release()
```

### <a id="Cube_Core_ClippedMesh_Update_Cube_Core_CubeSpace__Cube_Core_TriangleCache_System_Int64_"></a> Update\(in CubeSpace, TriangleCache, long\)

Brings the clipped edges up to date with the cube and the gathered triangles, spending at most
<code class="paramref">budgetTicks</code> of `System.Diagnostics.Stopwatch` time on a rebuild this frame.

```csharp
public bool Update(in CubeSpace cube, TriangleCache cache, long budgetTicks)
```

#### Parameters

`cube` [CubeSpace](Cube.Core.CubeSpace.md)

This frame's cube.

`cache` [TriangleCache](Cube.Core.TriangleCache.md)

The triangle cache.

`budgetTicks` [long](https://learn.microsoft.com/dotnet/api/system.int64)

The most clock ticks this call may spend building.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

True if a build finished, so the edges changed.
