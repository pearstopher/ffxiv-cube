# <a id="Cube_TestBridge_BridgeCheck"></a> Class BridgeCheck

Namespace: [Cube.TestBridge](Cube.TestBridge.md)  
Assembly: Cube.dll  

A named in-game check, registered with [Register](Cube.TestBridge.BridgeRunner.md#Cube_TestBridge_BridgeRunner_Register). Each feature that touches
the game adds checks for its behaviour (CLAUDE.md, "Building and testing"), so the running game
verifies it with no human in game.

```csharp
internal abstract class BridgeCheck
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BridgeCheck](Cube.TestBridge.BridgeCheck.md)

#### Derived

[BoxSizeCheck](Cube.TestBridge.Checks.BoxSizeCheck.md), 
[BuildStampCheck](Cube.TestBridge.Checks.BuildStampCheck.md), 
[CollisionDrawCheck](Cube.TestBridge.Checks.CollisionDrawCheck.md), 
[CollisionProbeCheck](Cube.TestBridge.Checks.CollisionProbeCheck.md), 
[CollisionReaderCheck](Cube.TestBridge.Checks.CollisionReaderCheck.md), 
[CollisionSettingsCheck](Cube.TestBridge.Checks.CollisionSettingsCheck.md), 
[CollisionZoneChangesCheck](Cube.TestBridge.Checks.CollisionZoneChangesCheck.md), 
[DrawBudgetCheck](Cube.TestBridge.Checks.DrawBudgetCheck.md), 
[DrawListMatchesCoreCheck](Cube.TestBridge.Checks.DrawListMatchesCoreCheck.md), 
[FakePlayersCheck](Cube.TestBridge.Checks.FakePlayersCheck.md), 
[FrameSnapshotCheck](Cube.TestBridge.Checks.FrameSnapshotCheck.md), 
[GameStateProbeCheck](Cube.TestBridge.Checks.GameStateProbeCheck.md), 
[HiddenStatesCheck](Cube.TestBridge.Checks.HiddenStatesCheck.md), 
[LabelsCheck](Cube.TestBridge.Checks.LabelsCheck.md), 
[NoErrorsCaughtCheck](Cube.TestBridge.Checks.NoErrorsCaughtCheck.md), 
[OrientationCheck](Cube.TestBridge.Checks.OrientationCheck.md), 
[PlayerCubesCheck](Cube.TestBridge.Checks.PlayerCubesCheck.md), 
[PlayersMatchObjectTableCheck](Cube.TestBridge.Checks.PlayersMatchObjectTableCheck.md), 
[SettingsCheck](Cube.TestBridge.Checks.SettingsCheck.md), 
[SettingsWindowCheck](Cube.TestBridge.Checks.SettingsWindowCheck.md), 
[SoakCheck](Cube.TestBridge.Checks.SoakCheck.md), 
[ViewControlCheck](Cube.TestBridge.Checks.ViewControlCheck.md), 
[WindowOpenCloseCheck](Cube.TestBridge.Checks.WindowOpenCloseCheck.md)


## Properties

### <a id="Cube_TestBridge_BridgeCheck_Name"></a> Name

Short unique name, shown in the results, e.g. <code>window-opens-and-closes</code>.

```csharp
public abstract string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Cube_TestBridge_BridgeCheck_SoakOnly"></a> SoakOnly

Whether the check runs only in soak runs (requested with <code>ingame_test.py --soak</code>) instead of every run.
For checks that take minutes.

```csharp
public virtual bool SoakOnly { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_TestBridge_BridgeCheck_Timeout"></a> Timeout

The longest the check may run before it fails as timed out.

```csharp
public virtual TimeSpan Timeout { get; }
```

#### Property Value

 [TimeSpan](https://learn.microsoft.com/dotnet/api/system.timespan)

#### Remarks

Ten seconds covers checks that wait a few hundred frames; override for longer ones.

## Methods

### <a id="Cube_TestBridge_BridgeCheck_Cleanup_Cube_TestBridge_BridgeContext_"></a> Cleanup\(BridgeContext\)

Puts back anything the check changed (e.g. the window's open state). Called after every run,
including when the check failed, threw or timed out, so a check never leaves the plugin altered.

```csharp
public virtual void Cleanup(BridgeContext context)
```

#### Parameters

`context` [BridgeContext](Cube.TestBridge.BridgeContext.md)

The same context the check ran with.

### <a id="Cube_TestBridge_BridgeCheck_Run_Cube_TestBridge_BridgeContext_"></a> Run\(BridgeContext\)

Runs the check, one step per frame, on the framework thread. Yield [NextFrame](Cube.TestBridge.CheckStep.md#Cube_TestBridge_CheckStep_NextFrame)
to wait a frame; finish with [Pass](Cube.TestBridge.CheckStep.md#Cube_TestBridge_CheckStep_Pass) or [Fail](Cube.TestBridge.CheckStep.md#Cube_TestBridge_CheckStep_Fail).

```csharp
public abstract IEnumerable<CheckStep> Run(BridgeContext context)
```

#### Parameters

`context` [BridgeContext](Cube.TestBridge.BridgeContext.md)

The plugin parts the check can look at, and where it records metrics.

#### Returns

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[CheckStep](Cube.TestBridge.CheckStep.md)\>

### <a id="Cube_TestBridge_BridgeCheck_WaitForDraws_Cube_CubeWindow_System_Int32_"></a> WaitForDraws\(CubeWindow, int\)

Waits until the cube window has drawn two more frames, so a change made before the call is in the last
frame. Ends with a failure step if it doesn't draw within <code class="paramref">maxFrames</code> frames (e.g. the
game UI is hidden); the runner stops the check at that step.

```csharp
protected static IEnumerable<CheckStep> WaitForDraws(CubeWindow window, int maxFrames = 30)
```

#### Parameters

`window` [CubeWindow](Cube.CubeWindow.md)

The cube window.

`maxFrames` [int](https://learn.microsoft.com/dotnet/api/system.int32)

How many frames to wait at most.

#### Returns

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[CheckStep](Cube.TestBridge.CheckStep.md)\>
