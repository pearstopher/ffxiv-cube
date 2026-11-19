# <a id="Cube_TestBridge_Checks_CollisionSplitCheck"></a> Class CollisionSplitCheck

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

Checks collision node boxes are split at gaps in the running game, lazily and within the budget (#127; PROJECT.md,
"Collision (phase 2)" and "Efficiency and memory").

```csharp
internal sealed class CollisionSplitCheck : BridgeCheck
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BridgeCheck](Cube.TestBridge.BridgeCheck.md) ← 
[CollisionSplitCheck](Cube.TestBridge.Checks.CollisionSplitCheck.md)


## Remarks

<p>
Starts from an empty piece cache and keeps gathering (the reader asks for another gather by itself while nodes are
still coarse) until a gather leaves the cache unchanged, at the largest cube and full detail. Pass criteria: the
refining settles within [MaxGathers](Cube.TestBridge.Checks.CollisionSplitCheck.md#Cube_TestBridge_Checks_CollisionSplitCheck_MaxGathers) gathers; the piece cache stays within its fixed capacity; the floor
under the player (the game's own downward raycast, as in [CollisionDrawCheck](Cube.TestBridge.Checks.CollisionDrawCheck.md#Cube_TestBridge_Checks_CollisionDrawCheck)) is still inside a
gathered box after refining, so splitting left no hole under the player; and no gathering frame allocates.
</p>
<p>
The box count and the piece counts are reported as metrics. Comparing refined boxes with ray hits over a whole
region (the #125 probe's measure) is left to [CollisionFitProbeCheck](Cube.TestBridge.Checks.CollisionFitProbeCheck.md#Cube_TestBridge_Checks_CollisionFitProbeCheck); the visual check is a manual step
in <code>docs/manual-tests.md</code>.
</p>

## Fields

### <a id="Cube_TestBridge_Checks_CollisionSplitCheck_MaxGathers"></a> MaxGathers

Most gathers the refining may take to settle (300 triangles a frame; a busy place needs a few dozen).

```csharp
private const int MaxGathers = 60
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionSplitCheck_saved"></a> saved

The user's settings before the check, restored by [Cleanup](Cube.TestBridge.Checks.CollisionSplitCheck.md#Cube_TestBridge_Checks_CollisionSplitCheck_Cleanup).

```csharp
private readonly CubeSettings saved
```

#### Field Value

 [CubeSettings](Cube.Core.CubeSettings.md)

### <a id="Cube_TestBridge_Checks_CollisionSplitCheck_wasOpen"></a> wasOpen

The window's open state before the check, restored by [Cleanup](Cube.TestBridge.Checks.CollisionSplitCheck.md#Cube_TestBridge_Checks_CollisionSplitCheck_Cleanup).

```csharp
private bool wasOpen
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Properties

### <a id="Cube_TestBridge_Checks_CollisionSplitCheck_Name"></a> Name

Short unique name, shown in the results, e.g. <code>window-opens-and-closes</code>.

```csharp
public override string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Cube_TestBridge_Checks_CollisionSplitCheck_Timeout"></a> Timeout

The longest the check may run before it fails as timed out.

```csharp
public override TimeSpan Timeout { get; }
```

#### Property Value

 [TimeSpan](https://learn.microsoft.com/dotnet/api/system.timespan)

#### Remarks

Ten seconds covers checks that wait a few hundred frames; override for longer ones.

## Methods

### <a id="Cube_TestBridge_Checks_CollisionSplitCheck_Cleanup_Cube_TestBridge_BridgeContext_"></a> Cleanup\(BridgeContext\)

Puts back anything the check changed (e.g. the window's open state). Called after every run,
including when the check failed, threw or timed out, so a check never leaves the plugin altered.

```csharp
public override void Cleanup(BridgeContext context)
```

#### Parameters

`context` [BridgeContext](Cube.TestBridge.BridgeContext.md)

The same context the check ran with.

### <a id="Cube_TestBridge_Checks_CollisionSplitCheck_Run_Cube_TestBridge_BridgeContext_"></a> Run\(BridgeContext\)

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
