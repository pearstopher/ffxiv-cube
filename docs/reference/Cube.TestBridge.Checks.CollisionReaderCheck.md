# <a id="Cube_TestBridge_Checks_CollisionReaderCheck"></a> Class CollisionReaderCheck

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

Checks the collision reader (#30; PROJECT.md, "Collision (phase 2)" and "Efficiency and memory") in the running
game: once the first gather completes there is a box under the player's feet, and repeated gathers stay within
their per-frame time budget, allocate nothing and stay under the box cap.

```csharp
internal sealed class CollisionReaderCheck : BridgeCheck
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BridgeCheck](Cube.TestBridge.BridgeCheck.md) ← 
[CollisionReaderCheck](Cube.TestBridge.Checks.CollisionReaderCheck.md)


## Remarks

<p>
<b>Under the feet.</b> The game's own downward raycast from just above the player finds the floor they stand on;
the check passes if that point lies in one of the gathered boxes (within [HitTolerance](Cube.TestBridge.Checks.CollisionReaderCheck.md#Cube_TestBridge_Checks_CollisionReaderCheck_HitTolerance)). This is the
same cross-check #28's probe used, so the reader and the game's collision agree. The raycast is a read-only game
function that takes the module's shared lock itself.
</p>
<p>
<b>Budgets.</b> The check forces [Gathers](Cube.TestBridge.Checks.CollisionReaderCheck.md#Cube_TestBridge_Checks_CollisionReaderCheck_Gathers) fresh gathers ([Invalidate](Cube.Core.CollisionCache.md#Cube_Core_CollisionCache_Invalidate)) and
reads the window's [GatherCost](Cube.CubeWindow.md#Cube_CubeWindow_GatherCost): no gathering frame may allocate, and none may run longer
than [MaxFrameMilliseconds](Cube.TestBridge.Checks.CollisionReaderCheck.md#Cube_TestBridge_Checks_CollisionReaderCheck_MaxFrameMilliseconds), the budget plus room for the one collider that crosses it (the cache only
checks the clock between colliders). The draw budget itself is still checked by [DrawBudgetCheck](Cube.TestBridge.Checks.DrawBudgetCheck.md#Cube_TestBridge_Checks_DrawBudgetCheck),
which no longer counts gathering.
</p>

## Fields

### <a id="Cube_TestBridge_Checks_CollisionReaderCheck_Gathers"></a> Gathers

How many gathers are forced and measured.

```csharp
private const int Gathers = 10
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionReaderCheck_HitTolerance"></a> HitTolerance

How far the floor hit may be outside a box and still count as inside (box rounding, compressed vertices; as in #28).

```csharp
private const float HitTolerance = 0.25
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_CollisionReaderCheck_MaxFrameMilliseconds"></a> MaxFrameMilliseconds

The longest a gathering frame may take: the 0.2 ms budget plus the collider that crosses it (one mesh tree walk,
a few hundred nodes at most) and the timer's own overhead.

```csharp
internal const double MaxFrameMilliseconds = 1
```

#### Field Value

 [double](https://learn.microsoft.com/dotnet/api/system.double)

### <a id="Cube_TestBridge_Checks_CollisionReaderCheck_RayLength"></a> RayLength

How far the test ray reaches down, in yalms.

```csharp
private const float RayLength = 5
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_CollisionReaderCheck_RayStartHeight"></a> RayStartHeight

How far above the player's feet the test ray starts, in yalms.

```csharp
private const float RayStartHeight = 1
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_CollisionReaderCheck_wasOpen"></a> wasOpen

The window's open state before the check, restored by [Cleanup](Cube.TestBridge.Checks.CollisionReaderCheck.md#Cube_TestBridge_Checks_CollisionReaderCheck_Cleanup).

```csharp
private bool wasOpen
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Properties

### <a id="Cube_TestBridge_Checks_CollisionReaderCheck_Name"></a> Name

Short unique name, shown in the results, e.g. <code>window-opens-and-closes</code>.

```csharp
public override string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Cube_TestBridge_Checks_CollisionReaderCheck_Timeout"></a> Timeout

The longest the check may run before it fails as timed out.

```csharp
public override TimeSpan Timeout { get; }
```

#### Property Value

 [TimeSpan](https://learn.microsoft.com/dotnet/api/system.timespan)

#### Remarks

Ten seconds covers checks that wait a few hundred frames; override for longer ones.

## Methods

### <a id="Cube_TestBridge_Checks_CollisionReaderCheck_Cleanup_Cube_TestBridge_BridgeContext_"></a> Cleanup\(BridgeContext\)

Puts back anything the check changed (e.g. the window's open state). Called after every run,
including when the check failed, threw or timed out, so a check never leaves the plugin altered.

```csharp
public override void Cleanup(BridgeContext context)
```

#### Parameters

`context` [BridgeContext](Cube.TestBridge.BridgeContext.md)

The same context the check ran with.

### <a id="Cube_TestBridge_Checks_CollisionReaderCheck_FindFloor_System_Numerics_Vector3_System_ReadOnlySpan_Cube_Core_CollisionBox__System_Numerics_Vector3__Cube_Core_CollisionBox__"></a> FindFloor\(Vector3, ReadOnlySpan<CollisionBox\>, out Vector3, out CollisionBox\)

Casts the game's raycast straight down from just above the player and looks for a gathered box holding the hit.

```csharp
private static CollisionReaderCheck.Floor FindFloor(Vector3 feet, ReadOnlySpan<CollisionBox> boxes, out Vector3 hit, out CollisionBox box)
```

#### Parameters

`feet` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The player's position (their feet).

`boxes` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[CollisionBox](Cube.Core.CollisionBox.md)\>

The gathered boxes.

`hit` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

Where the ray hit the floor.

`box` [CollisionBox](Cube.Core.CollisionBox.md)

The box holding the hit, if any.

#### Returns

 [CollisionReaderCheck](Cube.TestBridge.Checks.CollisionReaderCheck.md).[Floor](Cube.TestBridge.Checks.CollisionReaderCheck.Floor.md)

### <a id="Cube_TestBridge_Checks_CollisionReaderCheck_Run_Cube_TestBridge_BridgeContext_"></a> Run\(BridgeContext\)

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
