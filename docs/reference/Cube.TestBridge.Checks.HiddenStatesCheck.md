# <a id="Cube_TestBridge_Checks_HiddenStatesCheck"></a> Class HiddenStatesCheck

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

Checks the cube is hidden when logged out, during loading screens, cutscenes, GPose and in PvP areas, and comes back
afterwards (PROJECT.md, "When hidden": the window is just empty then, never an error; "Efficiency and memory": no
work and no allocations while there's nothing to draw).

```csharp
internal sealed class HiddenStatesCheck : BridgeCheck
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BridgeCheck](Cube.TestBridge.BridgeCheck.md) ← 
[HiddenStatesCheck](Cube.TestBridge.Checks.HiddenStatesCheck.md)


## Remarks

A real cutscene, loading screen or PvP match can't be started from a check, so each state is simulated through
[SimulatedState](Cube.CubeWindow.md#Cube_CubeWindow_SimulatedState), which replaces only the game-state read; everything after it is the
real draw path. The real state is checked first: with the game in normal play the cube must be shown. The
condition flags behind the real read were confirmed by the #10 probe.

## Fields

### <a id="Cube_TestBridge_Checks_HiddenStatesCheck_FramesPerState"></a> FramesPerState

Frames measured in each hidden state for allocations.

```csharp
private const int FramesPerState = 5
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_HiddenStatesCheck_Hidden"></a> Hidden

The hidden states to simulate, with the reason each must report.

```csharp
private static readonly (GameState State, Visibility Expected)[] Hidden
```

#### Field Value

 \([GameState](Cube.Core.GameState.md) [State](https://learn.microsoft.com/dotnet/api/system.valuetuple\-cube.core.gamestate,cube.core.visibility\-.state), [Visibility](Cube.Core.Visibility.md) [Expected](https://learn.microsoft.com/dotnet/api/system.valuetuple\-cube.core.gamestate,cube.core.visibility\-.expected)\)\[\]

### <a id="Cube_TestBridge_Checks_HiddenStatesCheck_wasOpen"></a> wasOpen

The window's open state before the check.

```csharp
private bool wasOpen
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Properties

### <a id="Cube_TestBridge_Checks_HiddenStatesCheck_Name"></a> Name

Short unique name, shown in the results, e.g. <code>window-opens-and-closes</code>.

```csharp
public override string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

## Methods

### <a id="Cube_TestBridge_Checks_HiddenStatesCheck_Cleanup_Cube_TestBridge_BridgeContext_"></a> Cleanup\(BridgeContext\)

Puts back anything the check changed (e.g. the window's open state). Called after every run,
including when the check failed, threw or timed out, so a check never leaves the plugin altered.

```csharp
public override void Cleanup(BridgeContext context)
```

#### Parameters

`context` [BridgeContext](Cube.TestBridge.BridgeContext.md)

The same context the check ran with.

### <a id="Cube_TestBridge_Checks_HiddenStatesCheck_Run_Cube_TestBridge_BridgeContext_"></a> Run\(BridgeContext\)

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
