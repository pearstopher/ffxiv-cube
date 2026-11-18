# <a id="Cube_TestBridge_Checks_SoakCheck"></a> Class SoakCheck

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

Soak: draws the worst case for ten minutes, changing zone every half minute, and checks nothing grows (PROJECT.md,
"Efficiency and memory": zero bytes allocated per frame in steady state, bounded memory, nothing grows with time
spent in game, collision data dropped on a zone change). Runs only when asked for with
<code>ingame_test.py --soak</code>.

```csharp
internal sealed class SoakCheck : BridgeCheck
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BridgeCheck](Cube.TestBridge.BridgeCheck.md) ← 
[SoakCheck](Cube.TestBridge.Checks.SoakCheck.md)


## Remarks

<p>
Pass criteria, all about the plugin's own work: zero bytes allocated by the draw path over every steady-state frame
of the soak ([LifetimeAllocatedBytes](Cube.Core.DrawCostSampler.md#Cube_Core_DrawCostSampler_LifetimeAllocatedBytes)), and an average under the frame-time
budget. Frames where a real player walked into view and their name string was built once are excluded from the
byte count ([NameBuildFrames](Cube.CubeWindow.md#Cube_CubeWindow_NameBuildFrames)) and reported; the fakes' names never change, so in an empty
area that count is 0.
</p>
<p>
<b>Collision (#33).</b> The soak runs in the [WorstCase](Cube.TestBridge.Checks.WorstCase.md#Cube_TestBridge_Checks_WorstCase), collision on. Every
[ZoneChangeInterval](Cube.TestBridge.Checks.SoakCheck.md#Cube_TestBridge_Checks_SoakCheck_ZoneChangeInterval) it switches the reader to another simulated zone
([SimulatedZone](Cube.Game.CollisionReader.md#Cube_Game_CollisionReader_SimulatedZone)), so the cache drops everything and gathers again, as after a
teleport. Gathering frames are kept out of the draw cost (PROJECT.md: budgeted background gathering is exempt) and
judged on their own: zero bytes over the whole soak, and every zone change followed by a completed gather.
</p>
<p>
The process's managed heap (`System.GC.GetTotalMemory(System.Boolean)`) and gen-0 collections are recorded as metrics
but not judged: the heap is shared with Dalamud and every other plugin, so it moves for reasons that aren't ours.
Since the draw path allocates nothing and every buffer is allocated once at load, the plugin's share can't grow.
</p>

## Fields

### <a id="Cube_TestBridge_Checks_SoakCheck_Duration"></a> Duration

How long to draw for.

```csharp
private static readonly TimeSpan Duration
```

#### Field Value

 [TimeSpan](https://learn.microsoft.com/dotnet/api/system.timespan)

### <a id="Cube_TestBridge_Checks_SoakCheck_FirstSimulatedZone"></a> FirstSimulatedZone

First simulated zone id; far above the game's territory ids so it can't match a real one.

```csharp
private const uint FirstSimulatedZone = 900000
```

#### Field Value

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

### <a id="Cube_TestBridge_Checks_SoakCheck_WarmUpFrames"></a> WarmUpFrames

Frames drawn before measuring starts.

```csharp
private const int WarmUpFrames = 60
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_SoakCheck_ZoneChangeInterval"></a> ZoneChangeInterval

How often the soak changes the simulated zone.

```csharp
private static readonly TimeSpan ZoneChangeInterval
```

#### Field Value

 [TimeSpan](https://learn.microsoft.com/dotnet/api/system.timespan)

### <a id="Cube_TestBridge_Checks_SoakCheck_saved"></a> saved

The user's settings before the check, restored by [Cleanup](Cube.TestBridge.Checks.SoakCheck.md#Cube_TestBridge_Checks_SoakCheck_Cleanup).

```csharp
private readonly CubeSettings saved
```

#### Field Value

 [CubeSettings](Cube.Core.CubeSettings.md)

### <a id="Cube_TestBridge_Checks_SoakCheck_wasOpen"></a> wasOpen

The window's open state before the check.

```csharp
private bool wasOpen
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Properties

### <a id="Cube_TestBridge_Checks_SoakCheck_Name"></a> Name

Short unique name, shown in the results, e.g. <code>window-opens-and-closes</code>.

```csharp
public override string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Cube_TestBridge_Checks_SoakCheck_SoakOnly"></a> SoakOnly

Whether the check runs only in soak runs (requested with <code>ingame_test.py --soak</code>) instead of every run.
For checks that take minutes.

```csharp
public override bool SoakOnly { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_TestBridge_Checks_SoakCheck_Timeout"></a> Timeout

The longest the check may run before it fails as timed out.

```csharp
public override TimeSpan Timeout { get; }
```

#### Property Value

 [TimeSpan](https://learn.microsoft.com/dotnet/api/system.timespan)

#### Remarks

Ten seconds covers checks that wait a few hundred frames; override for longer ones.

## Methods

### <a id="Cube_TestBridge_Checks_SoakCheck_Cleanup_Cube_TestBridge_BridgeContext_"></a> Cleanup\(BridgeContext\)

Puts back anything the check changed (e.g. the window's open state). Called after every run,
including when the check failed, threw or timed out, so a check never leaves the plugin altered.

```csharp
public override void Cleanup(BridgeContext context)
```

#### Parameters

`context` [BridgeContext](Cube.TestBridge.BridgeContext.md)

The same context the check ran with.

### <a id="Cube_TestBridge_Checks_SoakCheck_Run_Cube_TestBridge_BridgeContext_"></a> Run\(BridgeContext\)

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
