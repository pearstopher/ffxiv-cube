# <a id="Cube_TestBridge_Checks_CollisionProbeCheck"></a> Class CollisionProbeCheck

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

Research probe for #28 (where to get collision data): measures, in the running game, the three candidate
ways of finding what the player would bump into inside the cube, so <code>docs/research-notes.md</code> can pick
one on numbers rather than guesses.

```csharp
internal sealed class CollisionProbeCheck : BridgeCheck
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BridgeCheck](Cube.TestBridge.BridgeCheck.md) ← 
[CollisionProbeCheck](Cube.TestBridge.Checks.CollisionProbeCheck.md)


## Remarks

<ol><li><b>Collider bounds</b>: walk the collision scene's collider list (as vnavmesh's debug view does) and
take each collider's world box. Cheapest, but one box per collider, and terrain colliders are large.</li><li><b>Mesh node bounds</b>: for mesh colliders crossing the cube, walk the mesh's node tree (each node
has a local box and up to 256 vertices) and take the boxes of nodes holding triangles. Finer boxes, still
a plain memory read.</li><li><b>Raycasts</b>: a grid of downward rays through the cube with the game's own
`FFXIVClientStructs.FFXIV.Common.Component.BGCollision.BGCollisionModule.RaycastMaterialFilter(System.Numerics.Vector3%2cSystem.Numerics.Vector3%2cFFXIVClientStructs.FFXIV.Common.Component.BGCollision.RaycastHit%40%2cSystem.Single)`, which gives
the floor height under each grid point.</li><li><b>Coarse voxels</b> (PROJECT.md allows them if boxes are too coarse): every triangle in the crossing
nodes marks the 2-yalm cells its bounds touch. Compared with the cells the node boxes would fill, this shows
whether node boxes are fine enough to draw.</li></ol>

Each is timed per frame and its allocations counted, at the default and the largest cube; the raycast hits
are cross-checked against the node boxes so the two methods confirm each other. Counts and times are logged
in one <code>Probe #28</code> line. The check fails only when there is nothing to read (not logged in, or no
collision scene), so it never blocks a run.
Read-only (PROJECT.md, "Read-only game access"): it reads memory and calls only getters and the raycast,
which takes the collision module's shared lock itself; nothing is written or hooked.

## Constructors

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck__ctor_Dalamud_Plugin_Services_IPluginLog_"></a> CollisionProbeCheck\(IPluginLog\)

Research probe for #28 (where to get collision data): measures, in the running game, the three candidate
ways of finding what the player would bump into inside the cube, so <code>docs/research-notes.md</code> can pick
one on numbers rather than guesses.

```csharp
public CollisionProbeCheck(IPluginLog log)
```

#### Parameters

`log` [IPluginLog](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Plugin/Services/IPluginLog.cs)

Where the findings are logged, at Information so they show without Debug+.

#### Remarks

<ol><li><b>Collider bounds</b>: walk the collision scene's collider list (as vnavmesh's debug view does) and
take each collider's world box. Cheapest, but one box per collider, and terrain colliders are large.</li><li><b>Mesh node bounds</b>: for mesh colliders crossing the cube, walk the mesh's node tree (each node
has a local box and up to 256 vertices) and take the boxes of nodes holding triangles. Finer boxes, still
a plain memory read.</li><li><b>Raycasts</b>: a grid of downward rays through the cube with the game's own
`FFXIVClientStructs.FFXIV.Common.Component.BGCollision.BGCollisionModule.RaycastMaterialFilter(System.Numerics.Vector3%2cSystem.Numerics.Vector3%2cFFXIVClientStructs.FFXIV.Common.Component.BGCollision.RaycastHit%40%2cSystem.Single)`, which gives
the floor height under each grid point.</li><li><b>Coarse voxels</b> (PROJECT.md allows them if boxes are too coarse): every triangle in the crossing
nodes marks the 2-yalm cells its bounds touch. Compared with the cells the node boxes would fill, this shows
whether node boxes are fine enough to draw.</li></ol>

Each is timed per frame and its allocations counted, at the default and the largest cube; the raycast hits
are cross-checked against the node boxes so the two methods confirm each other. Counts and times are logged
in one <code>Probe #28</code> line. The check fails only when there is nothing to read (not logged in, or no
collision scene), so it never blocks a run.
Read-only (PROJECT.md, "Read-only game access"): it reads memory and calls only getters and the raycast,
which takes the collision module's shared lock itself; nothing is written or hooked.

## Fields

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_DefaultHalfEdge"></a> DefaultHalfEdge

Half the default cube edge: what most players will see.

```csharp
private const float DefaultHalfEdge = 20
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_HitTolerance"></a> HitTolerance

How far a raycast hit may be outside a node box and still count as inside it (box rounding, compressed vertices).

```csharp
private const float HitTolerance = 0.25
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_MaxHalfEdge"></a> MaxHalfEdge

Half the largest cube edge: the worst case for counts and cost.

```csharp
private const float MaxHalfEdge = 50
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_MovementLayer"></a> MovementLayer

The layer mask the raycast wrapper passes: layer 0 (the layer movement uses, per vnavmesh's notes); also
used to count the colliders a "walk into" query would see.

```csharp
private const ulong MovementLayer = 1
```

#### Field Value

 [ulong](https://learn.microsoft.com/dotnet/api/system.uint64)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_RayGrid"></a> RayGrid

Raycast grid points per side: 20 × 20 = 400 rays over the largest cube, one every 5 yalms.

```csharp
private const int RayGrid = 20
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_SampleSeconds"></a> SampleSeconds

How long the probe samples, in seconds: enough frames for a stable average at background frame rates.

```csharp
private const double SampleSeconds = 3
```

#### Field Value

 [double](https://learn.microsoft.com/dotnet/api/system.double)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_VoxelsPerSide"></a> VoxelsPerSide

Voxel cells per side of the largest cube: 50, so each cell is 2 yalms, about a character's height.

```csharp
private const int VoxelsPerSide = 50
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_boxCells"></a> boxCells

Cells covered by the leaf node boxes: how much of the cube the node boxes would fill if drawn.

```csharp
private readonly CollisionProbeCheck.VoxelGrid boxCells
```

#### Field Value

 [CollisionProbeCheck](Cube.TestBridge.Checks.CollisionProbeCheck.md).[VoxelGrid](Cube.TestBridge.Checks.CollisionProbeCheck.VoxelGrid.md)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_leafBoxes"></a> leafBoxes

Node boxes from the last walk that collected them, for the raycast cross-check; filled once.

```csharp
private readonly List<(Vector3 Min, Vector3 Max)> leafBoxes
```

#### Field Value

 [List](https://learn.microsoft.com/dotnet/api/system.collections.generic.list\-1)<\([Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3) [Min](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.numerics.vector3,system.numerics.vector3\-.min), [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3) [Max](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.numerics.vector3,system.numerics.vector3\-.max)\)\>

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_triangleCells"></a> triangleCells

Cells touched by an actual triangle's bounds: what a coarse voxel grid would draw.

```csharp
private readonly CollisionProbeCheck.VoxelGrid triangleCells
```

#### Field Value

 [CollisionProbeCheck](Cube.TestBridge.Checks.CollisionProbeCheck.md).[VoxelGrid](Cube.TestBridge.Checks.CollisionProbeCheck.VoxelGrid.md)

## Properties

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_Name"></a> Name

Short unique name, shown in the results, e.g. <code>window-opens-and-closes</code>.

```csharp
public override string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_Timeout"></a> Timeout

The longest the check may run before it fails as timed out.

```csharp
public override TimeSpan Timeout { get; }
```

#### Property Value

 [TimeSpan](https://learn.microsoft.com/dotnet/api/system.timespan)

#### Remarks

Ten seconds covers checks that wait a few hundred frames; override for longer ones.

## Methods

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_Cleanup_Cube_TestBridge_BridgeContext_"></a> Cleanup\(BridgeContext\)

Puts back anything the check changed (e.g. the window's open state). Called after every run,
including when the check failed, threw or timed out, so a check never leaves the plugin altered.

```csharp
public override void Cleanup(BridgeContext context)
```

#### Parameters

`context` [BridgeContext](Cube.TestBridge.BridgeContext.md)

The same context the check ran with.

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_CrossCheckRaycasts_System_Numerics_Vector3_System_Int32__System_Int32__Cube_TestBridge_Checks_CollisionProbeCheck_TypeCounts__"></a> CrossCheckRaycasts\(Vector3, out int, out int, out TypeCounts\)

Casts the ray grid once more, keeping the hits, and counts how many land inside one of the leaf node
boxes collected on the first frame: if the node walk sees the same geometry as the game's raycast,
nearly all should.

```csharp
private int CrossCheckRaycasts(Vector3 centre, out int inBoxes, out int inCells, out CollisionProbeCheck.TypeCounts byType)
```

#### Parameters

`centre` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The cube's centre from the first frame.

`inBoxes` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Hits inside a leaf node box.

`inCells` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Hits in a cell marked by a triangle (the voxel grids must hold the first frame).

`byType` [CollisionProbeCheck](Cube.TestBridge.Checks.CollisionProbeCheck.md).[TypeCounts](Cube.TestBridge.Checks.CollisionProbeCheck.TypeCounts.md)

Hits per type of collider hit.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

How many rays hit something.

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_Extent_System_Numerics_Vector3_System_Numerics_Vector3_"></a> Extent\(Vector3, Vector3\)

The largest side of a box.

```csharp
private static float Extent(Vector3 min, Vector3 max)
```

#### Parameters

`min` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

Low corner.

`max` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

High corner.

#### Returns

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_Overlaps_System_Numerics_Vector3_System_Numerics_Vector3_System_Numerics_Vector3_System_Numerics_Vector3_"></a> Overlaps\(Vector3, Vector3, Vector3, Vector3\)

Whether two boxes overlap (touching counts).

```csharp
internal static bool Overlaps(Vector3 aMin, Vector3 aMax, Vector3 bMin, Vector3 bMax)
```

#### Parameters

`aMin` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

First box's low corner.

`aMax` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

First box's high corner.

`bMin` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

Second box's low corner.

`bMax` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

Second box's high corner.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_RayOrigin_System_Numerics_Vector3_System_Int32_"></a> RayOrigin\(Vector3, int\)

Top of the largest cube above grid point <code class="paramref">index</code>, spaced evenly over its floor plan.

```csharp
private static Vector3 RayOrigin(Vector3 centre, int index)
```

#### Parameters

`centre` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The cube's centre (the player).

`index` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Grid point, row-major.

#### Returns

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_Run_Cube_TestBridge_BridgeContext_"></a> Run\(BridgeContext\)

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

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_TakeSample_System_Boolean_"></a> TakeSample\(bool\)

One frame of all three methods: the collider walk and node walk at the largest cube (timed, with
allocations), their counts again at the default cube, and the timed raycast grid.

```csharp
private CollisionProbeCheck.Sample TakeSample(bool collectLeaves)
```

#### Parameters

`collectLeaves` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether to keep the largest cube's leaf node boxes for the cross-check (allocates; first frame only).

#### Returns

 [CollisionProbeCheck](Cube.TestBridge.Checks.CollisionProbeCheck.md).[Sample](Cube.TestBridge.Checks.CollisionProbeCheck.Sample.md)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_TransformBox_FFXIVClientStructs_FFXIV_Common_Component_BGCollision_Math_Matrix4x3__System_Numerics_Vector3_System_Numerics_Vector3_System_Numerics_Vector3__System_Numerics_Vector3__"></a> TransformBox\(in Matrix4x3, Vector3, Vector3, out Vector3, out Vector3\)

Smallest world-aligned box around a local box after a world transform: the centre goes through the
matrix, and each half-extent axis adds its absolute row to the world half-extent.

```csharp
internal static void TransformBox(in Matrix4x3 world, Vector3 localMin, Vector3 localMax, out Vector3 min, out Vector3 max)
```

#### Parameters

`world` Matrix4x3

The transform.

`localMin` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The local box's low corner.

`localMax` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The local box's high corner.

`min` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The world box's low corner.

`max` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The world box's high corner.

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_UnitShapeBounds_FFXIVClientStructs_FFXIV_Common_Component_BGCollision_Math_Matrix4x3__System_Numerics_Vector3__System_Numerics_Vector3__"></a> UnitShapeBounds\(in Matrix4x3, out Vector3, out Vector3\)

World box of a box, cylinder, sphere or plane collider: the unit shape (-1..1) through its world matrix.

```csharp
internal static void UnitShapeBounds(in Matrix4x3 world, out Vector3 min, out Vector3 max)
```

#### Parameters

`world` Matrix4x3

The collider's world matrix.

`min` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The box's low corner.

`max` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The box's high corner.

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_Walk_FFXIVClientStructs_FFXIV_Common_Component_BGCollision_BGCollisionModule__System_Numerics_Vector3_System_Single_System_Boolean_Cube_TestBridge_Checks_CollisionProbeCheck_Sample__System_Collections_Generic_List_System_ValueTuple_System_Numerics_Vector3_System_Numerics_Vector3___Cube_TestBridge_Checks_CollisionProbeCheck_VoxelGrid_Cube_TestBridge_Checks_CollisionProbeCheck_VoxelGrid_"></a> Walk\(BGCollisionModule\*, Vector3, float, bool, ref Sample, List<\(Vector3 Min, Vector3 Max\)\>?, VoxelGrid?, VoxelGrid?\)

Walks every collider in every scene, counts those whose world box crosses the cube, and optionally walks
the node tree of each crossing mesh collider.

```csharp
private static CollisionProbeCheck.CubeCounts Walk(BGCollisionModule* module, Vector3 centre, float half, bool nodes, ref CollisionProbeCheck.Sample sample, List<(Vector3 Min, Vector3 Max)>? leaves, CollisionProbeCheck.VoxelGrid? boxes = null, CollisionProbeCheck.VoxelGrid? triangles = null)
```

#### Parameters

`module` BGCollisionModule\*

The collision module.

`centre` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The cube's centre.

`half` [float](https://learn.microsoft.com/dotnet/api/system.single)

Half the cube's edge.

`nodes` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether to walk mesh node trees too.

`sample` [CollisionProbeCheck](Cube.TestBridge.Checks.CollisionProbeCheck.md).[Sample](Cube.TestBridge.Checks.CollisionProbeCheck.Sample.md)

Scene-wide totals (filled on every walk; cheap).

`leaves` [List](https://learn.microsoft.com/dotnet/api/system.collections.generic.list\-1)<\([Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3) [Min](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.numerics.vector3,system.numerics.vector3\-.min), [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3) [Max](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.numerics.vector3,system.numerics.vector3\-.max)\)\>?

Where to put leaf node boxes, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a> to keep none.

`boxes` [CollisionProbeCheck](Cube.TestBridge.Checks.CollisionProbeCheck.md).[VoxelGrid](Cube.TestBridge.Checks.CollisionProbeCheck.VoxelGrid.md)?

Grid to mark leaf node boxes in, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>.

`triangles` [CollisionProbeCheck](Cube.TestBridge.Checks.CollisionProbeCheck.md).[VoxelGrid](Cube.TestBridge.Checks.CollisionProbeCheck.VoxelGrid.md)?

Grid to mark each triangle's bounds in, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>.

#### Returns

 [CollisionProbeCheck](Cube.TestBridge.Checks.CollisionProbeCheck.md).[CubeCounts](Cube.TestBridge.Checks.CollisionProbeCheck.CubeCounts.md)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_WalkNode_FFXIVClientStructs_FFXIV_Common_Component_BGCollision_MeshPCB_FileNode__FFXIVClientStructs_FFXIV_Common_Component_BGCollision_Math_Matrix4x3__System_Numerics_Vector3_System_Numerics_Vector3_Cube_TestBridge_Checks_CollisionProbeCheck_CubeCounts__System_Collections_Generic_List_System_ValueTuple_System_Numerics_Vector3_System_Numerics_Vector3___Cube_TestBridge_Checks_CollisionProbeCheck_VoxelGrid_Cube_TestBridge_Checks_CollisionProbeCheck_VoxelGrid_System_Int32_"></a> WalkNode\(FileNode\*, in Matrix4x3, Vector3, Vector3, ref CubeCounts, List<\(Vector3 Min, Vector3 Max\)\>?, VoxelGrid?, VoxelGrid?, int\)

Walks one mesh node and its children, skipping any subtree whose box misses the cube, and counts the
nodes holding triangles (leaves) that cross it.

```csharp
private static void WalkNode(MeshPCB.FileNode* node, in Matrix4x3 world, Vector3 cubeMin, Vector3 cubeMax, ref CollisionProbeCheck.CubeCounts counts, List<(Vector3 Min, Vector3 Max)>? leaves, CollisionProbeCheck.VoxelGrid? boxes, CollisionProbeCheck.VoxelGrid? triangles, int depth)
```

#### Parameters

`node` MeshPCB.FileNode\*

The node, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>.

`world` Matrix4x3

The mesh collider's world matrix.

`cubeMin` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The cube's low corner.

`cubeMax` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The cube's high corner.

`counts` [CollisionProbeCheck](Cube.TestBridge.Checks.CollisionProbeCheck.md).[CubeCounts](Cube.TestBridge.Checks.CollisionProbeCheck.CubeCounts.md)

Totals to add to.

`leaves` [List](https://learn.microsoft.com/dotnet/api/system.collections.generic.list\-1)<\([Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3) [Min](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.numerics.vector3,system.numerics.vector3\-.min), [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3) [Max](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.numerics.vector3,system.numerics.vector3\-.max)\)\>?

Where to put leaf boxes, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>.

`boxes` [CollisionProbeCheck](Cube.TestBridge.Checks.CollisionProbeCheck.md).[VoxelGrid](Cube.TestBridge.Checks.CollisionProbeCheck.VoxelGrid.md)?

Grid to mark leaf boxes in, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>.

`triangles` [CollisionProbeCheck](Cube.TestBridge.Checks.CollisionProbeCheck.md).[VoxelGrid](Cube.TestBridge.Checks.CollisionProbeCheck.VoxelGrid.md)?

Grid to mark triangle bounds in, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>.

`depth` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Recursion depth, capped so a bad pointer can't recurse forever.
