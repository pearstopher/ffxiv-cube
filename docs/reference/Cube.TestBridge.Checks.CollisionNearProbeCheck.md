# <a id="Cube_TestBridge_Checks_CollisionNearProbeCheck"></a> Class CollisionNearProbeCheck

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

Research probe for #129: what collision really sits around the player, and what does the game's own movement
raycast say about it? Used with the player standing in a gap (a counter's opening) to see why the cube draws it closed.

```csharp
internal sealed class CollisionNearProbeCheck : BridgeCheck
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BridgeCheck](Cube.TestBridge.BridgeCheck.md) ← 
[CollisionNearProbeCheck](Cube.TestBridge.Checks.CollisionNearProbeCheck.md)


## Remarks

<p>
Logs, as <code>Near #129</code> lines, every collider within [Reach](Cube.TestBridge.Checks.CollisionNearProbeCheck.md#Cube_TestBridge_Checks_CollisionNearProbeCheck_Reach) yalms (type, layer, material, layout id,
whether the reader keeps it, its world box), each mesh node holding triangles there with its triangles' bounds, and
a ring of horizontal raycasts at three heights (what the game itself treats as blocking, with the collider hit).
Read-only (PROJECT.md, "Read-only game access"); it allocates freely because it is a Debug research probe.
</p>

## Constructors

### <a id="Cube_TestBridge_Checks_CollisionNearProbeCheck__ctor_Dalamud_Plugin_Services_IPluginLog_"></a> CollisionNearProbeCheck\(IPluginLog\)

Research probe for #129: what collision really sits around the player, and what does the game's own movement
raycast say about it? Used with the player standing in a gap (a counter's opening) to see why the cube draws it closed.

```csharp
public CollisionNearProbeCheck(IPluginLog log)
```

#### Parameters

`log` [IPluginLog](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Plugin/Services/IPluginLog.cs)

Where the findings are logged, at Information so they show without Debug+.

#### Remarks

<p>
Logs, as <code>Near #129</code> lines, every collider within [Reach](Cube.TestBridge.Checks.CollisionNearProbeCheck.md#Cube_TestBridge_Checks_CollisionNearProbeCheck_Reach) yalms (type, layer, material, layout id,
whether the reader keeps it, its world box), each mesh node holding triangles there with its triangles' bounds, and
a ring of horizontal raycasts at three heights (what the game itself treats as blocking, with the collider hit).
Read-only (PROJECT.md, "Read-only game access"); it allocates freely because it is a Debug research probe.
</p>

## Fields

### <a id="Cube_TestBridge_Checks_CollisionNearProbeCheck_MaxTrianglesPerNode"></a> MaxTrianglesPerNode

Most triangles listed per node, so one big node can't flood the log.

```csharp
private const int MaxTrianglesPerNode = 300
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionNearProbeCheck_Reach"></a> Reach

How far from the player, in yalms, colliders and triangles are listed.

```csharp
private const float Reach = 4
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

## Properties

### <a id="Cube_TestBridge_Checks_CollisionNearProbeCheck_Name"></a> Name

Short unique name, shown in the results, e.g. <code>window-opens-and-closes</code>.

```csharp
public override string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Cube_TestBridge_Checks_CollisionNearProbeCheck_Timeout"></a> Timeout

The longest the check may run before it fails as timed out.

```csharp
public override TimeSpan Timeout { get; }
```

#### Property Value

 [TimeSpan](https://learn.microsoft.com/dotnet/api/system.timespan)

#### Remarks

Ten seconds covers checks that wait a few hundred frames; override for longer ones.

## Methods

### <a id="Cube_TestBridge_Checks_CollisionNearProbeCheck_Examine_System_Collections_Generic_List_System_String__"></a> Examine\(List<string\>\)

Collects the findings into <code class="paramref">lines</code>.

```csharp
private string? Examine(List<string> lines)
```

#### Parameters

`lines` [List](https://learn.microsoft.com/dotnet/api/system.collections.generic.list\-1)<[string](https://learn.microsoft.com/dotnet/api/system.string)\>

The log lines.

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)?

An error message, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>.

### <a id="Cube_TestBridge_Checks_CollisionNearProbeCheck_Run_Cube_TestBridge_BridgeContext_"></a> Run\(BridgeContext\)

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

### <a id="Cube_TestBridge_Checks_CollisionNearProbeCheck_Walk_FFXIVClientStructs_FFXIV_Common_Component_BGCollision_MeshPCB_FileNode__FFXIVClientStructs_FFXIV_Common_Component_BGCollision_Math_Matrix4x3__System_Numerics_Vector3_System_Numerics_Vector3_System_Int32_System_Collections_Generic_List_System_String__System_Int32__"></a> Walk\(FileNode\*, in Matrix4x3, Vector3, Vector3, int, List<string\>, ref int\)

Logs the nodes with triangles that reach the region, with their triangles' bounds.

```csharp
private static void Walk(MeshPCB.FileNode* node, in Matrix4x3 world, Vector3 lo, Vector3 hi, int depth, List<string> lines, ref int listed)
```

#### Parameters

`node` MeshPCB.FileNode\*

The node, or null.

`world` Matrix4x3

The mesh's world matrix.

`lo` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

Region low corner.

`hi` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

Region high corner.

`depth` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Recursion depth.

`lines` [List](https://learn.microsoft.com/dotnet/api/system.collections.generic.list\-1)<[string](https://learn.microsoft.com/dotnet/api/system.string)\>

The log lines.

`listed` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Nodes listed so far for this mesh.
