# <a id="Cube_TestBridge_Checks_TriangleProbeCheck"></a> Class TriangleProbeCheck

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

Research probe for #99 (can the real collision triangles be drawn in the cube?): measures, in the running game,
reading every triangle in the cube, clipping them to a turned box, deduplicating their edges, projecting and
sorting them, and the cost of issuing them to ImGui as wireframe, filled triangles, or both.

```csharp
internal sealed class TriangleProbeCheck : BridgeCheck
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BridgeCheck](Cube.TestBridge.BridgeCheck.md) ← 
[TriangleProbeCheck](Cube.TestBridge.Checks.TriangleProbeCheck.md)


## Remarks

<p>
Everything is measured at the default (40 yalm) and largest (100 yalm) cube, with the box turned 30 degrees (#93:
the cube turns with the camera, so clipping is done in box space). Reading and clipping use only plain memory reads
like [CollisionProbeCheck](Cube.TestBridge.Checks.CollisionProbeCheck.md#Cube_TestBridge_Checks_CollisionProbeCheck); drawing goes through [ProbeDraw](Cube.CubeWindow.md#Cube_CubeWindow_ProbeDraw), which the window
calls inside its own draw so real ImGui calls are timed on the window's draw list. The buffers are large and fixed,
allocated when the check starts and dropped in [Cleanup](Cube.TestBridge.Checks.TriangleProbeCheck.md#Cube_TestBridge_Checks_TriangleProbeCheck_Cleanup), so allocation counts for the timed steps are
the steps' own. Results are logged in one <code>Probe #99</code> line and written as metrics; the check fails only when
there is nothing to read.
</p>
<p>
Primitive colliders are turned into triangles here only to count and time them: a box is 12, a plane 2, a cylinder
an 8-sided prism (32) and a sphere an icosahedron (20). Their real shapes are approximate; their counts are small.
</p>

## Constructors

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck__ctor_Dalamud_Plugin_Services_IPluginLog_"></a> TriangleProbeCheck\(IPluginLog\)

Research probe for #99 (can the real collision triangles be drawn in the cube?): measures, in the running game,
reading every triangle in the cube, clipping them to a turned box, deduplicating their edges, projecting and
sorting them, and the cost of issuing them to ImGui as wireframe, filled triangles, or both.

```csharp
public TriangleProbeCheck(IPluginLog log)
```

#### Parameters

`log` [IPluginLog](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Plugin/Services/IPluginLog.cs)

Where the findings are logged, at Information.

#### Remarks

<p>
Everything is measured at the default (40 yalm) and largest (100 yalm) cube, with the box turned 30 degrees (#93:
the cube turns with the camera, so clipping is done in box space). Reading and clipping use only plain memory reads
like [CollisionProbeCheck](Cube.TestBridge.Checks.CollisionProbeCheck.md#Cube_TestBridge_Checks_CollisionProbeCheck); drawing goes through [ProbeDraw](Cube.CubeWindow.md#Cube_CubeWindow_ProbeDraw), which the window
calls inside its own draw so real ImGui calls are timed on the window's draw list. The buffers are large and fixed,
allocated when the check starts and dropped in [Cleanup](Cube.TestBridge.Checks.TriangleProbeCheck.md#Cube_TestBridge_Checks_TriangleProbeCheck_Cleanup), so allocation counts for the timed steps are
the steps' own. Results are logged in one <code>Probe #99</code> line and written as metrics; the check fails only when
there is nothing to read.
</p>
<p>
Primitive colliders are turned into triangles here only to count and time them: a box is 12, a plane 2, a cylinder
an 8-sided prism (32) and a sphere an icosahedron (20). Their real shapes are approximate; their counts are small.
</p>

## Fields

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_ClipCapacity"></a> ClipCapacity

Most clipped triangles kept: clipping can split an edge triangle into several.

```csharp
private const int ClipCapacity = 400000
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_DrawCap"></a> DrawCap

The most primitives the draw step issues in one frame, so a huge scene can't flood the game's draw list.

```csharp
private const int DrawCap = 60000
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_EdgeSlots"></a> EdgeSlots

Slots in the edge table: a power of two above twice the most edges.

```csharp
private const int EdgeSlots = 2097152
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_FramesPerCell"></a> FramesPerCell

Frames sampled for each draw mode and cube size, per phase (idle and turning).

```csharp
private const int FramesPerCell = 8
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_ReadCapacity"></a> ReadCapacity

Most world triangles kept when reading; more are counted but dropped.

```csharp
private const int ReadCapacity = 300000
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_Tilt"></a> Tilt

The fixed tilt of the viewing angle, in radians, so floors show as areas.

```csharp
private const float Tilt = 0.6108653
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_VertexScale"></a> VertexScale

Quantisation of vertices when matching shared edges: 1/32 yalm.

```csharp
private const float VertexScale = 32
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_Yaw"></a> Yaw

The cube's turn in the measurements, in radians: a worst-case, not axis-aligned.

```csharp
private const float Yaw = 0.5235988
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_armed"></a> armed

Whether the draw hook should do its work this frame.

```csharp
private bool armed
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_capped"></a> capped

Whether the last project step found more primitives than [DrawCap](Cube.TestBridge.Checks.TriangleProbeCheck.md#Cube_TestBridge_Checks_TriangleProbeCheck_DrawCap).

```csharp
private bool capped
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_clipped"></a> clipped

Clipped triangles in box space (3 vertices each).

```csharp
private Vector3[]? clipped
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)\[\]?

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_clippedCount"></a> clippedCount

Triangles after clipping.

```csharp
private int clippedCount
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_edgeCount"></a> edgeCount

Unique edges.

```csharp
private int edgeCount
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_edgeKeys"></a> edgeKeys

The edge table's keys (0 = empty).

```csharp
private ulong[]? edgeKeys
```

#### Field Value

 [ulong](https://learn.microsoft.com/dotnet/api/system.uint64)\[\]?

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_edgeScreen"></a> edgeScreen

Projected edge ends, 2 per edge.

```csharp
private Vector2[]? edgeScreen
```

#### Field Value

 [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)\[\]?

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_edgeTri"></a> edgeTri

The edge table's first triangle per key.

```csharp
private int[]? edgeTri
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)\[\]?

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_edges"></a> edges

Unique edges in box space (2 vertices each).

```csharp
private Vector3[]? edges
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)\[\]?

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_fresh"></a> fresh

Whether an idle cell still has to project its first frame.

```csharp
private bool fresh
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_half"></a> half

The cube's half edge for the cell being measured.

```csharp
private float half
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_hookBytes"></a> hookBytes

Hook allocations this frame.

```csharp
private long hookBytes
```

#### Field Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_hookMs"></a> hookMs

Hook time this frame in milliseconds, set by [DrawHook](Cube.TestBridge.Checks.TriangleProbeCheck.md#Cube_TestBridge_Checks_TriangleProbeCheck_DrawHook).

```csharp
private double hookMs
```

#### Field Value

 [double](https://learn.microsoft.com/dotnet/api/system.double)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_hookPrims"></a> hookPrims

Hook primitives issued this frame.

```csharp
private int hookPrims
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_keys"></a> keys

Depth sort keys per clipped triangle.

```csharp
private float[]? keys
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)\[\]?

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_mode"></a> mode

The current cell's mode.

```csharp
private TriangleProbeCheck.Mode mode
```

#### Field Value

 [TriangleProbeCheck](Cube.TestBridge.Checks.TriangleProbeCheck.md).[Mode](Cube.TestBridge.Checks.TriangleProbeCheck.Mode.md)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_order"></a> order

Triangle indexes, sorted by key.

```csharp
private int[]? order
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)\[\]?

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_triColour"></a> triColour

Flat-shade colour per clipped triangle.

```csharp
private uint[]? triColour
```

#### Field Value

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)\[\]?

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_triScreen"></a> triScreen

Projected triangle corners, 3 per clipped triangle.

```csharp
private Vector2[]? triScreen
```

#### Field Value

 [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)\[\]?

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_turning"></a> turning

Whether the current cell re-projects and re-sorts every frame (view turning) or only draws cached data (idle).

```csharp
private bool turning
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_viewYaw"></a> viewYaw

The viewing yaw for the current frame, advanced while turning.

```csharp
private float viewYaw
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_window"></a> window

The window, for its drawing region.

```csharp
private CubeWindow? window
```

#### Field Value

 [CubeWindow](Cube.CubeWindow.md)?

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_world"></a> world

World triangles read from the scene (3 vertices each).

```csharp
private Vector3[]? world
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)\[\]?

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_worldCount"></a> worldCount

Triangles read, wholly or partly in the cube's bounds, and kept.

```csharp
private int worldCount
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Properties

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_Name"></a> Name

Short unique name, shown in the results, e.g. <code>window-opens-and-closes</code>.

```csharp
public override string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_Timeout"></a> Timeout

The longest the check may run before it fails as timed out.

```csharp
public override TimeSpan Timeout { get; }
```

#### Property Value

 [TimeSpan](https://learn.microsoft.com/dotnet/api/system.timespan)

#### Remarks

Ten seconds covers checks that wait a few hundred frames; override for longer ones.

## Methods

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_AddBox_FFXIVClientStructs_FFXIV_Common_Component_BGCollision_Math_Matrix4x3__"></a> AddBox\(in Matrix4x3\)

Adds a unit box (-1..1) through a matrix as 12 triangles.

```csharp
private int AddBox(in Matrix4x3 m)
```

#### Parameters

`m` Matrix4x3

The collider's world matrix.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_AddIcosahedron_FFXIVClientStructs_FFXIV_Common_Component_BGCollision_Math_Matrix4x3__"></a> AddIcosahedron\(in Matrix4x3\)

Adds a unit sphere as a 20-triangle icosahedron.

```csharp
private int AddIcosahedron(in Matrix4x3 m)
```

#### Parameters

`m` Matrix4x3

The collider's world matrix.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_AddPlane_FFXIVClientStructs_FFXIV_Common_Component_BGCollision_Math_Matrix4x3__"></a> AddPlane\(in Matrix4x3\)

Adds a flat unit square (-1..1 in X and Y) as 2 triangles.

```csharp
private int AddPlane(in Matrix4x3 m)
```

#### Parameters

`m` Matrix4x3

The collider's world matrix.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_AddPrism_FFXIVClientStructs_FFXIV_Common_Component_BGCollision_Math_Matrix4x3__System_Int32_"></a> AddPrism\(in Matrix4x3, int\)

Adds a unit cylinder (radius 1, height -1..1 along Y) as a prism with caps.

```csharp
private int AddPrism(in Matrix4x3 m, int sides)
```

#### Parameters

`m` Matrix4x3

The collider's world matrix.

`sides` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Sides of the prism.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_AddTriangle_System_Numerics_Vector3_System_Numerics_Vector3_System_Numerics_Vector3_"></a> AddTriangle\(Vector3, Vector3, Vector3\)

Appends a world triangle if there is room.

```csharp
private void AddTriangle(Vector3 a, Vector3 b, Vector3 c)
```

#### Parameters

`a` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

First corner.

`b` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

Second corner.

`c` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

Third corner.

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_Allocate"></a> Allocate\(\)

Allocates the probe's large fixed buffers once.

```csharp
private void Allocate()
```

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_Cleanup_Cube_TestBridge_BridgeContext_"></a> Cleanup\(BridgeContext\)

Puts back anything the check changed (e.g. the window's open state). Called after every run,
including when the check failed, threw or timed out, so a check never leaves the plugin altered.

```csharp
public override void Cleanup(BridgeContext context)
```

#### Parameters

`context` [BridgeContext](Cube.TestBridge.BridgeContext.md)

The same context the check ran with.

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_ClipPlane_System_ReadOnlySpan_System_Numerics_Vector3__System_Int32_System_Span_System_Numerics_Vector3__System_Int32_System_Single_"></a> ClipPlane\(ReadOnlySpan<Vector3\>, int, Span<Vector3\>, int, float\)

One Sutherland-Hodgman step: clips a polygon against one plane.

```csharp
private static int ClipPlane(ReadOnlySpan<Vector3> input, int count, Span<Vector3> output, int plane, float h)
```

#### Parameters

`input` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)\>

The polygon.

`count` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Its corner count.

`output` [Span](https://learn.microsoft.com/dotnet/api/system.span\-1)<[Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)\>

Where the clipped polygon goes.

`plane` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The plane.

`h` [float](https://learn.microsoft.com/dotnet/api/system.single)

The box's half edge.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_Coplanar_System_Numerics_Vector3___System_Int32_System_Int32_"></a> Coplanar\(Vector3\[\], int, int\)

Whether two triangles (by first vertex index) have nearly the same normal.

```csharp
private static bool Coplanar(Vector3[] c, int a, int b)
```

#### Parameters

`c` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)\[\]

The clipped vertex buffer.

`a` [int](https://learn.microsoft.com/dotnet/api/system.int32)

First triangle's first vertex index.

`b` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Second triangle's first vertex index.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_CountSmall_System_Single_"></a> CountSmall\(float\)

Counts clipped triangles whose projected bounds are under one pixel in both directions.

```csharp
private int CountSmall(float size)
```

#### Parameters

`size` [float](https://learn.microsoft.com/dotnet/api/system.single)

The cube's edge.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_DrawHook_Dalamud_Bindings_ImGui_ImDrawListPtr_"></a> DrawHook\(ImDrawListPtr\)

Runs inside the window's draw: when armed, re-projects (if turning) and issues the current mode's ImGui calls on
the window's draw list, timing the lot and counting allocations.

```csharp
private void DrawHook(ImDrawListPtr target)
```

#### Parameters

`target` [ImDrawListPtr](https://github.com/goatcorp/Dalamud/blob/master/imgui/Dalamud.Bindings.ImGui/Custom/Generated/Generated/Structs.gen.cs)

The window's draw list.

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_Free"></a> Free\(\)

Drops the buffers so the probe leaves nothing behind.

```csharp
private void Free()
```

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_Inside_System_Numerics_Vector3_System_Int32_System_Single_"></a> Inside\(Vector3, int, float\)

The signed distance of a point inside one of the six planes (positive is inside).

```csharp
private static float Inside(Vector3 p, int plane, float h)
```

#### Parameters

`p` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

A point.

`plane` [int](https://learn.microsoft.com/dotnet/api/system.int32)

0..5: -X, +X, -Y, +Y, -Z, +Z.

`h` [float](https://learn.microsoft.com/dotnet/api/system.single)

The box's half edge.

#### Returns

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_MeasureClip_System_Numerics_Vector3_"></a> MeasureClip\(Vector3\)

Clips the world triangles to the turned box in box space (the box's centre at the origin), writing the result to
the clipped buffer; timed. Wholly inside triangles are copied, wholly outside ones dropped, and the rest cut
with Sutherland-Hodgman against the six planes and fanned into triangles.

```csharp
private TriangleProbeCheck.ClipResult MeasureClip(Vector3 centre)
```

#### Parameters

`centre` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The cube's centre in the world.

#### Returns

 [TriangleProbeCheck](Cube.TestBridge.Checks.TriangleProbeCheck.md).[ClipResult](Cube.TestBridge.Checks.TriangleProbeCheck.ClipResult.md)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_MeasureDedup"></a> MeasureDedup\(\)

Deduplicates shared edges across the clipped triangles; timed.

```csharp
private TriangleProbeCheck.DedupResult MeasureDedup()
```

#### Returns

 [TriangleProbeCheck](Cube.TestBridge.Checks.TriangleProbeCheck.md).[DedupResult](Cube.TestBridge.Checks.TriangleProbeCheck.DedupResult.md)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_MeasureProject"></a> MeasureProject\(\)

Projects and sorts the clipped triangles once, timed; what a view change would cost.

```csharp
private TriangleProbeCheck.Cost MeasureProject()
```

#### Returns

 [TriangleProbeCheck](Cube.TestBridge.Checks.TriangleProbeCheck.md).[Cost](Cube.TestBridge.Checks.TriangleProbeCheck.Cost.md)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_MeasureRead_System_Numerics_Vector3__System_String__"></a> MeasureRead\(out Vector3, out string?\)

Reads every triangle near the cube into the world buffer, timed.

```csharp
private TriangleProbeCheck.ReadResult MeasureRead(out Vector3 centre, out string? failure)
```

#### Parameters

`centre` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The player position (the cube's centre).

`failure` [string](https://learn.microsoft.com/dotnet/api/system.string)?

Why nothing could be read, or null.

#### Returns

 [TriangleProbeCheck](Cube.TestBridge.Checks.TriangleProbeCheck.md).[ReadResult](Cube.TestBridge.Checks.TriangleProbeCheck.ReadResult.md)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_Outcode_System_Numerics_Vector3_System_Single_"></a> Outcode\(Vector3, float\)

Which of the six planes a point is outside, as bits.

```csharp
private static int Outcode(Vector3 p, float h)
```

#### Parameters

`p` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

A point in box space.

`h` [float](https://learn.microsoft.com/dotnet/api/system.single)

The box's half edge.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_ProjectAndSort_System_Single_System_Boolean_"></a> ProjectAndSort\(float, bool\)

Projects every clipped triangle (orthographic, turned by <code class="paramref">yaw</code> and tilted) into the window's
region, flat-shades it by its normal, and depth-sorts the triangle indexes; also projects the edges.

```csharp
private void ProjectAndSort(float yaw, bool all)
```

#### Parameters

`yaw` [float](https://learn.microsoft.com/dotnet/api/system.single)

The view's turn in radians.

`all` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether to project triangles and edges both (otherwise only what the mode draws).

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_Quantise_System_Numerics_Vector3_"></a> Quantise\(Vector3\)

Packs a box-space point into 36 bits at 1/32 yalm.

```csharp
private static ulong Quantise(Vector3 v)
```

#### Parameters

`v` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The point.

#### Returns

 [ulong](https://learn.microsoft.com/dotnet/api/system.uint64)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_ReadNode_FFXIVClientStructs_FFXIV_Common_Component_BGCollision_MeshPCB_FileNode__FFXIVClientStructs_FFXIV_Common_Component_BGCollision_Math_Matrix4x3__System_Numerics_Vector3_System_Numerics_Vector3_System_Int32__System_Int32_"></a> ReadNode\(FileNode\*, in Matrix4x3, Vector3, Vector3, ref int, int\)

Reads one mesh node and its children whose boxes cross the cull box.

```csharp
private void ReadNode(MeshPCB.FileNode* node, in Matrix4x3 matrix, Vector3 cullMin, Vector3 cullMax, ref int inBounds, int depth)
```

#### Parameters

`node` MeshPCB.FileNode\*

The node, or null.

`matrix` Matrix4x3

The mesh's world matrix.

`cullMin` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

Low corner of the cull box.

`cullMax` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

High corner of the cull box.

`inBounds` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Running count of triangles in crossing nodes.

`depth` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Recursion depth, capped.

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_ReadScene_FFXIVClientStructs_FFXIV_Common_Component_BGCollision_BGCollisionModule__System_Numerics_Vector3_System_Numerics_Vector3_System_Int32__System_Int32__"></a> ReadScene\(BGCollisionModule\*, Vector3, Vector3, out int, out int\)

Walks the colliders, culling first, writing triangles to the world buffer.

```csharp
private void ReadScene(BGCollisionModule* module, Vector3 cullMin, Vector3 cullMax, out int inBounds, out int primitive)
```

#### Parameters

`module` BGCollisionModule\*

The collision module.

`cullMin` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

Low corner of the cull box.

`cullMax` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

High corner of the cull box.

`inBounds` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Triangles in nodes crossing the cull box.

`primitive` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Triangles from primitive colliders.

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_Run_Cube_TestBridge_BridgeContext_"></a> Run\(BridgeContext\)

Runs the check, one step per frame, on the framework thread. Yield [NextFrame](Cube.TestBridge.CheckStep.md#Cube_TestBridge_CheckStep_NextFrame)
to wait a frame; finish with [Pass](Cube.TestBridge.CheckStep.md#Cube_TestBridge_CheckStep_Pass) or [Fail](Cube.TestBridge.CheckStep.md#Cube_TestBridge_CheckStep_Fail).

```csharp
public override IEnumerable<CheckStep> Run(BridgeContext context)
```

#### Parameters

`context` [BridgeContext](Cube.TestBridge.BridgeContext.md)

The plugin parts the check can look at, and where it records metrics.

#### Returns

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[CheckStep](Cube.TestBridge.CheckStep.md)\>

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_RunClip_System_Numerics_Vector3_System_Int32__System_Int32__System_Int32__"></a> RunClip\(Vector3, out int, out int, out int\)

One clip pass.

```csharp
private void RunClip(Vector3 centre, out int inside, out int outside, out int cut)
```

#### Parameters

`centre` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The cube's centre in the world.

`inside` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Triangles wholly inside.

`outside` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Triangles wholly outside.

`cut` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Triangles that needed cutting.

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_RunDedup_System_Int32__System_Int32__"></a> RunDedup\(out int, out int\)

One edge pass: hashes quantised vertex pairs into the open-addressing table.

```csharp
private void RunDedup(out int shared, out int coplanar)
```

#### Parameters

`shared` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Edges met a second time.

`coplanar` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Of those, edges whose two triangles are coplanar.

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_ToBox_System_Numerics_Vector3_System_Single_System_Single_"></a> ToBox\(Vector3, float, float\)

Rotates a world offset into box space.

```csharp
private static Vector3 ToBox(Vector3 d, float cos, float sin)
```

#### Parameters

`d` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

Offset from the box's centre.

`cos` [float](https://learn.microsoft.com/dotnet/api/system.single)

Cosine of the inverse turn.

`sin` [float](https://learn.microsoft.com/dotnet/api/system.single)

Sine of the inverse turn.

#### Returns

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)
