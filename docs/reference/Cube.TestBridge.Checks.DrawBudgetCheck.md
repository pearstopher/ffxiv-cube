# <a id="Cube_TestBridge_Checks_DrawBudgetCheck"></a> Class DrawBudgetCheck

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

Checks the draw path stays within PROJECT.md's phase 1 budgets ("Efficiency and memory"):
under 0.5 ms per frame on average and zero bytes allocated per frame in steady state.
A budget failure is a failed test, not a note (CLAUDE.md).

```csharp
internal sealed class DrawBudgetCheck : BridgeCheck
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BridgeCheck](Cube.TestBridge.BridgeCheck.md) ← 
[DrawBudgetCheck](Cube.TestBridge.Checks.DrawBudgetCheck.md)


## Remarks

Opens the window with [UseCrowd](Cube.TestBridge.FakePlayers.md#Cube_TestBridge_FakePlayers_UseCrowd) (the most players the game can have around, each with a
cube and a full-length label), lets it warm up (first-use caches, JIT, name strings) and then measures
[FramesToMeasure](Cube.TestBridge.Checks.DrawBudgetCheck.md#Cube_TestBridge_Checks_DrawBudgetCheck_FramesToMeasure) frames with the window's [DrawCostSampler](Cube.Core.DrawCostSampler.md#Cube_Core_DrawCostSampler).

## Fields

### <a id="Cube_TestBridge_Checks_DrawBudgetCheck_BudgetMilliseconds"></a> BudgetMilliseconds

The phase 1 average frame-time budget, in milliseconds.

```csharp
public const double BudgetMilliseconds = 0.5
```

#### Field Value

 [double](https://learn.microsoft.com/dotnet/api/system.double)

### <a id="Cube_TestBridge_Checks_DrawBudgetCheck_FramesToMeasure"></a> FramesToMeasure

Frames measured.

```csharp
private const int FramesToMeasure = 120
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_DrawBudgetCheck_WarmUpFrames"></a> WarmUpFrames

Frames drawn before measuring starts.

```csharp
private const int WarmUpFrames = 30
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_DrawBudgetCheck_wasOpen"></a> wasOpen

The window's open state before the check, restored by [Cleanup](Cube.TestBridge.Checks.DrawBudgetCheck.md#Cube_TestBridge_Checks_DrawBudgetCheck_Cleanup).

```csharp
private bool wasOpen
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Properties

### <a id="Cube_TestBridge_Checks_DrawBudgetCheck_Name"></a> Name

Short unique name, shown in the results, e.g. <code>window-opens-and-closes</code>.

```csharp
public override string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

## Methods

### <a id="Cube_TestBridge_Checks_DrawBudgetCheck_Cleanup_Cube_TestBridge_BridgeContext_"></a> Cleanup\(BridgeContext\)

Puts back anything the check changed (e.g. the window's open state). Called after every run,
including when the check failed, threw or timed out, so a check never leaves the plugin altered.

```csharp
public override void Cleanup(BridgeContext context)
```

#### Parameters

`context` [BridgeContext](Cube.TestBridge.BridgeContext.md)

The same context the check ran with.

### <a id="Cube_TestBridge_Checks_DrawBudgetCheck_Run_Cube_TestBridge_BridgeContext_"></a> Run\(BridgeContext\)

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
