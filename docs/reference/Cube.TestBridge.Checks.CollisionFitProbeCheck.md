# <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck"></a> Class CollisionFitProbeCheck

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

Research probe for #125: how well do the mesh node boxes match the real collision, and what would match it better?
PROJECT.md, "Phase 2", wants the cube to show what the player bumps into, so a box over a walkable gap (a counter
with an opening) or over nothing (a big square nobody can touch) misleads.

```csharp
internal sealed class CollisionFitProbeCheck : BridgeCheck
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BridgeCheck](Cube.TestBridge.BridgeCheck.md) ← 
[CollisionFitProbeCheck](Cube.TestBridge.Checks.CollisionFitProbeCheck.md)


## Remarks

<p>
For every blocking mesh node that holds triangles near the player, the probe compares three boxes: the node's own
box (what the reader draws today), the tight box around only the node's <b>own</b> triangles, and the boxes around
the connected groups of filled 0.5-yalm cells those triangles touch (the "split at gaps" idea). It logs the
covered volumes, how many nodes split, the cost of the analysis, and the worst-fitting nodes with a ray test of
each ([HasHit](Cube.TestBridge.Checks.CollisionPhantomsCheck.md#Cube_TestBridge_Checks_CollisionPhantomsCheck_HasHit)), in <code>Fit #125</code> lines.
</p>
<p>
The question behind it: an inner tree node that holds triangles of its own gets a box that spans its whole
subtree, so two blocks with a gap between them come out as one long box. The probe counts such nodes and measures
how much space their boxes waste.
</p>
<p>
Read-only (PROJECT.md, "Read-only game access"): memory reads, getters and the game's raycast. It allocates freely
because it is a Debug research probe, not the per-frame path.
</p>

## Constructors

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck__ctor_Dalamud_Plugin_Services_IPluginLog_"></a> CollisionFitProbeCheck\(IPluginLog\)

Research probe for #125: how well do the mesh node boxes match the real collision, and what would match it better?
PROJECT.md, "Phase 2", wants the cube to show what the player bumps into, so a box over a walkable gap (a counter
with an opening) or over nothing (a big square nobody can touch) misleads.

```csharp
public CollisionFitProbeCheck(IPluginLog log)
```

#### Parameters

`log` [IPluginLog](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Plugin/Services/IPluginLog.cs)

Where the findings are logged, at Information so they show without Debug+.

#### Remarks

<p>
For every blocking mesh node that holds triangles near the player, the probe compares three boxes: the node's own
box (what the reader draws today), the tight box around only the node's <b>own</b> triangles, and the boxes around
the connected groups of filled 0.5-yalm cells those triangles touch (the "split at gaps" idea). It logs the
covered volumes, how many nodes split, the cost of the analysis, and the worst-fitting nodes with a ray test of
each ([HasHit](Cube.TestBridge.Checks.CollisionPhantomsCheck.md#Cube_TestBridge_Checks_CollisionPhantomsCheck_HasHit)), in <code>Fit #125</code> lines.
</p>
<p>
The question behind it: an inner tree node that holds triangles of its own gets a box that spans its whole
subtree, so two blocks with a gap between them come out as one long box. The probe counts such nodes and measures
how much space their boxes waste.
</p>
<p>
Read-only (PROJECT.md, "Read-only game access"): memory reads, getters and the game's raycast. It allocates freely
because it is a Debug research probe, not the per-frame path.
</p>

## Fields

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_BigSide"></a> BigSide

A node whose longest side is at least this (yalms) counts as "big" in the cost-of-refining numbers.

```csharp
private const float BigSide = 3
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Cell"></a> Cell

Edge of the cells the occupancy grid uses, in yalms.

```csharp
private const float Cell = 0.5
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_HalfEdge"></a> HalfEdge

Half the edge of the region around the player that is examined, in yalms.

```csharp
private const float HalfEdge = 40
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Listed"></a> Listed

How many of the worst-fitting nodes are listed.

```csharp
private const int Listed = 15
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_MaxCellsPerTriangle"></a> MaxCellsPerTriangle

A triangle whose bounds cover more cells than this is counted but not filled in (keeps the probe short).

```csharp
private const int MaxCellsPerTriangle = 20000
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_MinGap"></a> MinGap

The smallest gap, in yalms, the triangle sweep splits at. The player's collision is about 0.5 yalm wide, so a
narrower gap can't be walked through and need not be shown.

```csharp
private const float MinGap = 0.5
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_scan"></a> scan

The scan in progress; set by [Run](Cube.TestBridge.Checks.CollisionFitProbeCheck.md#Cube_TestBridge_Checks_CollisionFitProbeCheck_Run) so the helpers need not pass it along.

```csharp
private CollisionFitProbeCheck.Scan scan
```

#### Field Value

 [CollisionFitProbeCheck](Cube.TestBridge.Checks.CollisionFitProbeCheck.md).[Scan](Cube.TestBridge.Checks.CollisionFitProbeCheck.Scan.md)

## Properties

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Name"></a> Name

Short unique name, shown in the results, e.g. <code>window-opens-and-closes</code>.

```csharp
public override string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Timeout"></a> Timeout

The longest the check may run before it fails as timed out.

```csharp
public override TimeSpan Timeout { get; }
```

#### Property Value

 [TimeSpan](https://learn.microsoft.com/dotnet/api/system.timespan)

#### Remarks

Ten seconds covers checks that wait a few hundred frames; override for longer ones.

## Methods

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_AddCells_System_Collections_Generic_HashSet_System_Int64__System_Numerics_Vector3_System_Numerics_Vector3_"></a> AddCells\(HashSet<long\>, Vector3, Vector3\)

Marks every cell a triangle's bounds touch.

```csharp
private static bool AddCells(HashSet<long> cells, Vector3 lo, Vector3 hi)
```

#### Parameters

`cells` [HashSet](https://learn.microsoft.com/dotnet/api/system.collections.generic.hashset\-1)<[long](https://learn.microsoft.com/dotnet/api/system.int64)\>

The occupied cells.

`lo` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

Low corner of the triangle's bounds.

`hi` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

High corner of the triangle's bounds.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

False if the triangle was too large to fill in.

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_AnalyseNode_FFXIVClientStructs_FFXIV_Common_Component_BGCollision_MeshPCB_FileNode__FFXIVClientStructs_FFXIV_Common_Component_BGCollision_Math_Matrix4x3__FFXIVClientStructs_FFXIV_Common_Component_BGCollision_Collider__System_Int32_System_Numerics_Vector3_System_Numerics_Vector3_"></a> AnalyseNode\(FileNode\*, in Matrix4x3, Collider\*, int, Vector3, Vector3\)

Measures one node's box against its own triangles.

```csharp
private void AnalyseNode(MeshPCB.FileNode* node, in Matrix4x3 world, Collider* collider, int depth, Vector3 min, Vector3 max)
```

#### Parameters

`node` MeshPCB.FileNode\*

The node (it has triangles).

`world` Matrix4x3

The mesh's world matrix.

`collider` Collider\*

The mesh collider.

`depth` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The node's depth in its tree.

`min` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The world box of the node's local bounds, low corner.

`max` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The world box of the node's local bounds, high corner.

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_ChildVolume_FFXIVClientStructs_FFXIV_Common_Component_BGCollision_MeshPCB_FileNode__FFXIVClientStructs_FFXIV_Common_Component_BGCollision_Math_Matrix4x3__"></a> ChildVolume\(FileNode\*, in Matrix4x3\)

The volume of a child node's world box, or 0 for no child.

```csharp
private static float ChildVolume(MeshPCB.FileNode* child, in Matrix4x3 world)
```

#### Parameters

`child` MeshPCB.FileNode\*

The child node, or null.

`world` Matrix4x3

The mesh's world matrix.

#### Returns

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Component_System_Numerics_Vector3_System_Int32_"></a> Component\(Vector3, int\)

One component of a vector by axis number.

```csharp
private static float Component(Vector3 v, int axis)
```

#### Parameters

`v` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The vector.

`axis` [int](https://learn.microsoft.com/dotnet/api/system.int32)

0 for X, 1 for Y, 2 for Z.

#### Returns

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Components_System_Collections_Generic_HashSet_System_Int64__System_Single__System_Single__"></a> Components\(HashSet<long\>, out float, out float\)

Splits the filled cells into groups that touch face to face and adds up the volume of each group's bounding box.

```csharp
private static int Components(HashSet<long> cells, out float volume, out float largest)
```

#### Parameters

`cells` [HashSet](https://learn.microsoft.com/dotnet/api/system.collections.generic.hashset\-1)<[long](https://learn.microsoft.com/dotnet/api/system.int64)\>

The occupied cells (consumed).

`volume` [float](https://learn.microsoft.com/dotnet/api/system.single)

Out: total volume of the groups' boxes.

`largest` [float](https://learn.microsoft.com/dotnet/api/system.single)

Out: volume of the largest group's box.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

The number of groups.

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Examine"></a> Examine\(\)

Walks every blocking mesh near the player and collects the findings.

```csharp
private string? Examine()
```

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)?

An error message, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>.

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_HistogramText_System_Int32___"></a> HistogramText\(int\[\]\)

Formats the ratio histogram as "label count" pairs.

```csharp
private static IEnumerable<string> HistogramText(int[] histogram)
```

#### Parameters

`histogram` [int](https://learn.microsoft.com/dotnet/api/system.int32)\[\]

Counts per bucket.

#### Returns

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[string](https://learn.microsoft.com/dotnet/api/system.string)\>

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Key_System_Int32_System_Int32_System_Int32_"></a> Key\(int, int, int\)

Packs a cell's integer coordinates into one key.

```csharp
private static long Key(int x, int y, int z)
```

#### Parameters

`x` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Cell X.

`y` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Cell Y.

`z` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Cell Z.

#### Returns

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Run_Cube_TestBridge_BridgeContext_"></a> Run\(BridgeContext\)

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

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Sweep_System_Collections_Generic_List_System_ValueTuple_System_Numerics_Vector3_System_Numerics_Vector3___System_Collections_Generic_List_System_ValueTuple_System_Numerics_Vector3_System_Numerics_Vector3___System_Int32_"></a> Sweep\(List<\(Vector3 Lo, Vector3 Hi\)\>, List<\(Vector3 Lo, Vector3 Hi\)\>, int\)

Splits triangle bounds into groups wherever a gap of at least [MinGap](Cube.TestBridge.Checks.CollisionFitProbeCheck.md#Cube_TestBridge_Checks_CollisionFitProbeCheck_MinGap) separates them along an axis,
recursively, and adds the group boxes to <code class="paramref">output</code>: one box per group of triangles that the
player could not squeeze between. A group that doesn't split on any axis is one box around its triangles.

```csharp
private static void Sweep(List<(Vector3 Lo, Vector3 Hi)> triangles, List<(Vector3 Lo, Vector3 Hi)> output, int depth)
```

#### Parameters

`triangles` [List](https://learn.microsoft.com/dotnet/api/system.collections.generic.list\-1)<\([Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3) [Lo](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.numerics.vector3,system.numerics.vector3\-.lo), [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3) [Hi](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.numerics.vector3,system.numerics.vector3\-.hi)\)\>

Triangle bounds (sorted in place).

`output` [List](https://learn.microsoft.com/dotnet/api/system.collections.generic.list\-1)<\([Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3) [Lo](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.numerics.vector3,system.numerics.vector3\-.lo), [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3) [Hi](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.numerics.vector3,system.numerics.vector3\-.hi)\)\>

The boxes found.

`depth` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Recursion depth, capped.

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Visit_System_Collections_Generic_HashSet_System_Int64__System_Collections_Generic_Stack_System_Int64__System_Int32_System_Int32_System_Int32_"></a> Visit\(HashSet<long\>, Stack<long\>, int, int, int\)

Moves a neighbouring cell from the unvisited set to the stack, if it is filled.

```csharp
private static void Visit(HashSet<long> cells, Stack<long> stack, int x, int y, int z)
```

#### Parameters

`cells` [HashSet](https://learn.microsoft.com/dotnet/api/system.collections.generic.hashset\-1)<[long](https://learn.microsoft.com/dotnet/api/system.int64)\>

The unvisited filled cells.

`stack` [Stack](https://learn.microsoft.com/dotnet/api/system.collections.generic.stack\-1)<[long](https://learn.microsoft.com/dotnet/api/system.int64)\>

The cells still to expand.

`x` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Cell X.

`y` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Cell Y.

`z` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Cell Z.

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_Volume_System_Numerics_Vector3_System_Numerics_Vector3_"></a> Volume\(Vector3, Vector3\)

Volume of a box, or 0 if it is empty or inverted.

```csharp
private static float Volume(Vector3 min, Vector3 max)
```

#### Parameters

`min` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

Low corner.

`max` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

High corner.

#### Returns

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_CollisionFitProbeCheck_WalkNode_FFXIVClientStructs_FFXIV_Common_Component_BGCollision_MeshPCB_FileNode__FFXIVClientStructs_FFXIV_Common_Component_BGCollision_Math_Matrix4x3__FFXIVClientStructs_FFXIV_Common_Component_BGCollision_Collider__System_Int32_"></a> WalkNode\(FileNode\*, in Matrix4x3, Collider\*, int\)

Analyses one node with triangles, then its children (skipping subtrees outside the region).

```csharp
private void WalkNode(MeshPCB.FileNode* node, in Matrix4x3 world, Collider* collider, int depth)
```

#### Parameters

`node` MeshPCB.FileNode\*

The node, or null.

`world` Matrix4x3

The mesh's world matrix.

`collider` Collider\*

The mesh collider.

`depth` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Recursion depth, capped against a bad pointer.
