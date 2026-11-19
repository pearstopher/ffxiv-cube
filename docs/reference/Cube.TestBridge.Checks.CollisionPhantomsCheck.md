# <a id="Cube_TestBridge_Checks_CollisionPhantomsCheck"></a> Class CollisionPhantomsCheck

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

Research and regression check for #107: some collision boxes are drawn where the player can walk straight through.
PROJECT.md, "Phase 2", says collision means "what the player would bump into, walk on, or be stopped by", so such
boxes ("phantoms") must not be drawn.

```csharp
internal sealed class CollisionPhantomsCheck : BridgeCheck
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BridgeCheck](Cube.TestBridge.BridgeCheck.md) ← 
[CollisionPhantomsCheck](Cube.TestBridge.Checks.CollisionPhantomsCheck.md)


## Remarks

<p>
For every collider box near the player (primitive colliders, and mesh nodes that hold triangles), the check casts
rays through it with the game's own movement raycast (`FFXIVClientStructs.FFXIV.Common.Component.BGCollision.BGCollisionModule.RaycastMaterialFilter(System.Numerics.Vector3%2cSystem.Numerics.Vector3%2cFFXIVClientStructs.FFXIV.Common.Component.BGCollision.RaycastHit%40%2cSystem.Single)`):
a 3 × 3 grid straight down from its top, and two heights from each side. A box with no hit point inside it is a
phantom. The boxes are grouped by the fields that could separate them (type, `FFXIVClientStructs.FFXIV.Common.Component.BGCollision.Collider.VisibilityFlags`,
`FFXIVClientStructs.FFXIV.Common.Component.BGCollision.Collider.LayerMask`, `FFXIVClientStructs.FFXIV.Common.Component.BGCollision.Collider.ObjectMaterialValue`, `FFXIVClientStructs.FFXIV.Common.Component.BGCollision.Collider.ObjectMaterialMask`),
and each group's phantom share is logged in one <code>Phantoms #107</code> line together with the nearest phantoms, so
they can be matched to what is on screen.
</p>
<p>
Read-only (PROJECT.md, "Read-only game access"): memory reads, getters and the raycast only.
</p>

## Constructors

### <a id="Cube_TestBridge_Checks_CollisionPhantomsCheck__ctor_Dalamud_Plugin_Services_IPluginLog_"></a> CollisionPhantomsCheck\(IPluginLog\)

Research and regression check for #107: some collision boxes are drawn where the player can walk straight through.
PROJECT.md, "Phase 2", says collision means "what the player would bump into, walk on, or be stopped by", so such
boxes ("phantoms") must not be drawn.

```csharp
public CollisionPhantomsCheck(IPluginLog log)
```

#### Parameters

`log` [IPluginLog](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Plugin/Services/IPluginLog.cs)

Where the findings are logged, at Information so they show without Debug+.

#### Remarks

<p>
For every collider box near the player (primitive colliders, and mesh nodes that hold triangles), the check casts
rays through it with the game's own movement raycast (`FFXIVClientStructs.FFXIV.Common.Component.BGCollision.BGCollisionModule.RaycastMaterialFilter(System.Numerics.Vector3%2cSystem.Numerics.Vector3%2cFFXIVClientStructs.FFXIV.Common.Component.BGCollision.RaycastHit%40%2cSystem.Single)`):
a 3 × 3 grid straight down from its top, and two heights from each side. A box with no hit point inside it is a
phantom. The boxes are grouped by the fields that could separate them (type, `FFXIVClientStructs.FFXIV.Common.Component.BGCollision.Collider.VisibilityFlags`,
`FFXIVClientStructs.FFXIV.Common.Component.BGCollision.Collider.LayerMask`, `FFXIVClientStructs.FFXIV.Common.Component.BGCollision.Collider.ObjectMaterialValue`, `FFXIVClientStructs.FFXIV.Common.Component.BGCollision.Collider.ObjectMaterialMask`),
and each group's phantom share is logged in one <code>Phantoms #107</code> line together with the nearest phantoms, so
they can be matched to what is on screen.
</p>
<p>
Read-only (PROJECT.md, "Read-only game access"): memory reads, getters and the raycast only.
</p>

## Fields

### <a id="Cube_TestBridge_Checks_CollisionPhantomsCheck_HalfEdge"></a> HalfEdge

Half the edge of the region around the player that is examined, in yalms.

```csharp
private const float HalfEdge = 150
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_CollisionPhantomsCheck_HitObjects"></a> HitObjects

Which kinds of collider (type, layer, material) any ray hit, and how often: the kinds the game's movement raycast can ever hit.

```csharp
private static readonly Dictionary<string, int> HitObjects
```

#### Field Value

 [Dictionary](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary\-2)<[string](https://learn.microsoft.com/dotnet/api/system.string), [int](https://learn.microsoft.com/dotnet/api/system.int32)\>

### <a id="Cube_TestBridge_Checks_CollisionPhantomsCheck_HitTolerance"></a> HitTolerance

How far a hit point may be outside a box and still count as inside it (box rounding, compressed vertices).

```csharp
private const float HitTolerance = 0.25
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_CollisionPhantomsCheck_ListedPhantoms"></a> ListedPhantoms

How many of the nearest phantoms are listed in the log.

```csharp
private const int ListedPhantoms = 25
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionPhantomsCheck_MaxBoxes"></a> MaxBoxes

Most boxes tested, so the check's one frame of raycasts stays short.

```csharp
private const int MaxBoxes = 4000
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionPhantomsCheck_scan"></a> scan

The scan in progress; set by [Examine](Cube.TestBridge.Checks.CollisionPhantomsCheck.md#Cube_TestBridge_Checks_CollisionPhantomsCheck_Examine) so the helpers need not pass it along.

```csharp
private CollisionPhantomsCheck.Result scan
```

#### Field Value

 [CollisionPhantomsCheck](Cube.TestBridge.Checks.CollisionPhantomsCheck.md).[Result](Cube.TestBridge.Checks.CollisionPhantomsCheck.Result.md)

## Properties

### <a id="Cube_TestBridge_Checks_CollisionPhantomsCheck_Name"></a> Name

Short unique name, shown in the results, e.g. <code>window-opens-and-closes</code>.

```csharp
public override string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Cube_TestBridge_Checks_CollisionPhantomsCheck_Timeout"></a> Timeout

The longest the check may run before it fails as timed out.

```csharp
public override TimeSpan Timeout { get; }
```

#### Property Value

 [TimeSpan](https://learn.microsoft.com/dotnet/api/system.timespan)

#### Remarks

Ten seconds covers checks that wait a few hundred frames; override for longer ones.

## Methods

### <a id="Cube_TestBridge_Checks_CollisionPhantomsCheck_Examine"></a> Examine\(\)

Tests every box near the player and collects the findings.

```csharp
private CollisionPhantomsCheck.Result Examine()
```

#### Returns

 [CollisionPhantomsCheck](Cube.TestBridge.Checks.CollisionPhantomsCheck.md).[Result](Cube.TestBridge.Checks.CollisionPhantomsCheck.Result.md)

### <a id="Cube_TestBridge_Checks_CollisionPhantomsCheck_HasHit_Cube_Core_CollisionBox_FFXIVClientStructs_FFXIV_Common_Component_BGCollision_Collider__"></a> HasHit\(CollisionBox, Collider\*\)

Whether any of the box's rays hits its own collider inside the box (something else hit there doesn't count: it would be that other thing the player bumps into).

```csharp
private static bool HasHit(CollisionBox box, Collider* collider)
```

#### Parameters

`box` [CollisionBox](Cube.Core.CollisionBox.md)

The box.

`collider` Collider\*

The collider the box came from.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_TestBridge_Checks_CollisionPhantomsCheck_HasHitIn_System_Numerics_Vector3_System_Numerics_Vector3_FFXIVClientStructs_FFXIV_Common_Component_BGCollision_Collider__"></a> HasHitIn\(Vector3, Vector3, Collider\*\)

The same ray test for a box given by its corners, for the #125 fit probe.

```csharp
internal static bool HasHitIn(Vector3 min, Vector3 max, Collider* collider)
```

#### Parameters

`min` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

Low corner.

`max` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

High corner.

`collider` Collider\*

The collider the box came from.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_TestBridge_Checks_CollisionPhantomsCheck_Hits_System_Numerics_Vector3_System_Numerics_Vector3_System_Single_System_Numerics_Vector3_System_Numerics_Vector3_System_Numerics_Vector3_FFXIVClientStructs_FFXIV_Common_Component_BGCollision_Collider__"></a> Hits\(Vector3, Vector3, float, Vector3, Vector3, Vector3, Collider\*\)

One raycast; true if it hits at a point inside the (slightly grown) box.

```csharp
private static bool Hits(Vector3 origin, Vector3 direction, float reach, Vector3 min, Vector3 max, Vector3 tolerance, Collider* collider)
```

#### Parameters

`origin` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

Where the ray starts.

`direction` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The ray's direction.

`reach` [float](https://learn.microsoft.com/dotnet/api/system.single)

How far it reaches.

`min` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The box's low corner.

`max` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The box's high corner.

`tolerance` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

How far outside the box a hit still counts.

`collider` Collider\*

The collider the hit must belong to.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_TestBridge_Checks_CollisionPhantomsCheck_Run_Cube_TestBridge_BridgeContext_"></a> Run\(BridgeContext\)

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

### <a id="Cube_TestBridge_Checks_CollisionPhantomsCheck_Tally_FFXIVClientStructs_FFXIV_Common_Component_BGCollision_Collider__"></a> Tally\(Collider\*\)

Counts one hit object into [HitObjects](Cube.TestBridge.Checks.CollisionPhantomsCheck.md#Cube_TestBridge_Checks_CollisionPhantomsCheck_HitObjects).

```csharp
private static void Tally(Collider* collider)
```

#### Parameters

`collider` Collider\*

The collider hit, or null.

### <a id="Cube_TestBridge_Checks_CollisionPhantomsCheck_TestBox_Cube_Core_CollisionBox_FFXIVClientStructs_FFXIV_Common_Component_BGCollision_Collider__FFXIVClientStructs_FFXIV_Common_Component_BGCollision_ColliderType_System_Boolean_System_String_"></a> TestBox\(CollisionBox, Collider\*, ColliderType, bool, string\)

Casts the rays through one box and records whether any hit lands inside it.

```csharp
private void TestBox(CollisionBox box, Collider* collider, ColliderType type, bool kept, string source)
```

#### Parameters

`box` [CollisionBox](Cube.Core.CollisionBox.md)

The box.

`collider` Collider\*

The collider it came from.

`type` ColliderType

Its type.

`kept` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether the reader's filter keeps it.

`source` [string](https://learn.microsoft.com/dotnet/api/system.string)

"node" or "primitive".

### <a id="Cube_TestBridge_Checks_CollisionPhantomsCheck_TestPrimitive_FFXIVClientStructs_FFXIV_Common_Component_BGCollision_Math_Matrix4x3__System_Numerics_Vector3_System_Numerics_Vector3_FFXIVClientStructs_FFXIV_Common_Component_BGCollision_Collider__FFXIVClientStructs_FFXIV_Common_Component_BGCollision_ColliderType_System_Boolean_"></a> TestPrimitive\(in Matrix4x3, Vector3, Vector3, Collider\*, ColliderType, bool\)

Tests one primitive collider's box.

```csharp
private void TestPrimitive(in Matrix4x3 world, Vector3 localMin, Vector3 localMax, Collider* collider, ColliderType type, bool kept)
```

#### Parameters

`world` Matrix4x3

The collider's world matrix.

`localMin` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

Its shape's low corner in its own space.

`localMax` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

Its shape's high corner in its own space.

`collider` Collider\*

The collider.

`type` ColliderType

Its type.

`kept` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether the reader's filter keeps it.

### <a id="Cube_TestBridge_Checks_CollisionPhantomsCheck_WalkNode_FFXIVClientStructs_FFXIV_Common_Component_BGCollision_MeshPCB_FileNode__System_Numerics_Matrix4x4__FFXIVClientStructs_FFXIV_Common_Component_BGCollision_Collider__FFXIVClientStructs_FFXIV_Common_Component_BGCollision_ColliderType_System_Boolean_System_Int32_"></a> WalkNode\(FileNode\*, in Matrix4x4, Collider\*, ColliderType, bool, int\)

Walks a mesh node tree, testing the boxes of nodes that hold triangles and cross the region.

```csharp
private void WalkNode(MeshPCB.FileNode* node, in Matrix4x4 world, Collider* collider, ColliderType type, bool kept, int depth)
```

#### Parameters

`node` MeshPCB.FileNode\*

The node, or null.

`world` [Matrix4x4](https://learn.microsoft.com/dotnet/api/system.numerics.matrix4x4)

The mesh's world matrix.

`collider` Collider\*

The mesh collider.

`type` ColliderType

Its type.

`kept` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether the reader's filter keeps it.

`depth` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Recursion depth, capped against a bad pointer.
