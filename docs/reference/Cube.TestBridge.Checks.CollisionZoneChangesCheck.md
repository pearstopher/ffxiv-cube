# <a id="Cube_TestBridge_Checks_CollisionZoneChangesCheck"></a> Class CollisionZoneChangesCheck

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

Checks memory stays bounded across zone changes (PROJECT.md, "Bounded memory": caches have fixed caps, nothing
grows with time spent in game, and collision data is dropped when you change zone; #33).

```csharp
internal sealed class CollisionZoneChangesCheck : BridgeCheck
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BridgeCheck](Cube.TestBridge.BridgeCheck.md) ← 
[CollisionZoneChangesCheck](Cube.TestBridge.Checks.CollisionZoneChangesCheck.md)


## Remarks

<p>
No human is in game to teleport, so the check changes zone by putting simulated zone ids in place of the real one
([SimulatedZone](Cube.Game.CollisionReader.md#Cube_Game_CollisionReader_SimulatedZone)) and finally switching back to the real zone. The collision cache
sees exactly what it sees after a teleport: a different zone id, so it drops both sets and gathers afresh. Only the
scene stays the same, which lets the check compare each new gather with the first.
</p>
<p>
Pass criteria: every change is counted ([ZoneChanges](Cube.Core.CollisionCache.md#Cube_Core_CollisionCache_ZoneChanges)) and followed by a completed
gather; the ready set is always one of the same two buffers (nothing is allocated for a new zone) at the same
capacity; and no gathering or drawing frame allocates. The process heap is recorded but not judged, as in
[SoakCheck](Cube.TestBridge.Checks.SoakCheck.md#Cube_TestBridge_Checks_SoakCheck).
</p>

## Fields

### <a id="Cube_TestBridge_Checks_CollisionZoneChangesCheck_Changes"></a> Changes

How many simulated zone changes are made (plus one back to the real zone).

```csharp
private const int Changes = 10
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionZoneChangesCheck_FirstSimulatedZone"></a> FirstSimulatedZone

First simulated zone id; far above the game's territory ids so it can't match a real one.

```csharp
private const uint FirstSimulatedZone = 800000
```

#### Field Value

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

### <a id="Cube_TestBridge_Checks_CollisionZoneChangesCheck_saved"></a> saved

The user's settings before the check, restored by [Cleanup](Cube.TestBridge.Checks.CollisionZoneChangesCheck.md#Cube_TestBridge_Checks_CollisionZoneChangesCheck_Cleanup).

```csharp
private readonly CubeSettings saved
```

#### Field Value

 [CubeSettings](Cube.Core.CubeSettings.md)

### <a id="Cube_TestBridge_Checks_CollisionZoneChangesCheck_wasOpen"></a> wasOpen

The window's open state before the check, restored by [Cleanup](Cube.TestBridge.Checks.CollisionZoneChangesCheck.md#Cube_TestBridge_Checks_CollisionZoneChangesCheck_Cleanup).

```csharp
private bool wasOpen
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Properties

### <a id="Cube_TestBridge_Checks_CollisionZoneChangesCheck_Name"></a> Name

Short unique name, shown in the results, e.g. <code>window-opens-and-closes</code>.

```csharp
public override string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Cube_TestBridge_Checks_CollisionZoneChangesCheck_Timeout"></a> Timeout

The longest the check may run before it fails as timed out.

```csharp
public override TimeSpan Timeout { get; }
```

#### Property Value

 [TimeSpan](https://learn.microsoft.com/dotnet/api/system.timespan)

#### Remarks

Ten seconds covers checks that wait a few hundred frames; override for longer ones.

## Methods

### <a id="Cube_TestBridge_Checks_CollisionZoneChangesCheck_Cleanup_Cube_TestBridge_BridgeContext_"></a> Cleanup\(BridgeContext\)

Puts back anything the check changed (e.g. the window's open state). Called after every run,
including when the check failed, threw or timed out, so a check never leaves the plugin altered.

```csharp
public override void Cleanup(BridgeContext context)
```

#### Parameters

`context` [BridgeContext](Cube.TestBridge.BridgeContext.md)

The same context the check ran with.

### <a id="Cube_TestBridge_Checks_CollisionZoneChangesCheck_Run_Cube_TestBridge_BridgeContext_"></a> Run\(BridgeContext\)

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
