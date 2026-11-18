# <a id="Cube_TestBridge_Checks_NamesAndTogglesCheck"></a> Class NamesAndTogglesCheck

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

Checks #112 end to end through the window: each per-kind toggle removes exactly its kind of cube from the next frame
(and the local player is never removed), the own-name setting adds or removes the local player's label, and the
name position setting makes every label centred above its cube. Uses fakes of every kind so it works with nobody
around; the game's real reads of event objects are covered by the research probe and
<code>docs/manual-tests.md</code>.

```csharp
internal sealed class NamesAndTogglesCheck : BridgeCheck
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BridgeCheck](Cube.TestBridge.BridgeCheck.md) ← 
[NamesAndTogglesCheck](Cube.TestBridge.Checks.NamesAndTogglesCheck.md)


## Fields

### <a id="Cube_TestBridge_Checks_NamesAndTogglesCheck_FramesToWait"></a> FramesToWait

Frames to wait for the window to draw.

```csharp
private const int FramesToWait = 30
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_NamesAndTogglesCheck_Kinds"></a> Kinds

The fakes: one of every kind, all inside the cube.

```csharp
private static readonly (string Name, PlayerRelation Relation)[] Kinds
```

#### Field Value

 \([string](https://learn.microsoft.com/dotnet/api/system.string) [Name](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.string,cube.core.playerrelation\-.name), [PlayerRelation](Cube.Core.PlayerRelation.md) [Relation](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.string,cube.core.playerrelation\-.relation)\)\[\]

### <a id="Cube_TestBridge_Checks_NamesAndTogglesCheck_saved"></a> saved

The settings before the check, restored afterwards.

```csharp
private CubeSettings? saved
```

#### Field Value

 [CubeSettings](Cube.Core.CubeSettings.md)?

### <a id="Cube_TestBridge_Checks_NamesAndTogglesCheck_timedOut"></a> timedOut

Set when the window didn't draw in time.

```csharp
private bool timedOut
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_TestBridge_Checks_NamesAndTogglesCheck_wasOpen"></a> wasOpen

The window's open state before the check.

```csharp
private bool wasOpen
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Properties

### <a id="Cube_TestBridge_Checks_NamesAndTogglesCheck_Name"></a> Name

Short unique name, shown in the results, e.g. <code>window-opens-and-closes</code>.

```csharp
public override string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

## Methods

### <a id="Cube_TestBridge_Checks_NamesAndTogglesCheck_Cleanup_Cube_TestBridge_BridgeContext_"></a> Cleanup\(BridgeContext\)

Puts back anything the check changed (e.g. the window's open state). Called after every run,
including when the check failed, threw or timed out, so a check never leaves the plugin altered.

```csharp
public override void Cleanup(BridgeContext context)
```

#### Parameters

`context` [BridgeContext](Cube.TestBridge.BridgeContext.md)

The same context the check ran with.

### <a id="Cube_TestBridge_Checks_NamesAndTogglesCheck_CountFakes_Cube_CubeWindow_System_Nullable_Cube_Core_PlayerRelation__"></a> CountFakes\(CubeWindow, PlayerRelation?\)

How many of this check's fakes are in the frame, of one relation or (null) of any.

```csharp
private static int CountFakes(CubeWindow window, PlayerRelation? relation)
```

#### Parameters

`window` [CubeWindow](Cube.CubeWindow.md)

The cube window.

`relation` [PlayerRelation](Cube.Core.PlayerRelation.md)?

The relation to count, or null for all.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_NamesAndTogglesCheck_Draw_Cube_CubeWindow_"></a> Draw\(CubeWindow\)

Waits for the window to draw two more frames, setting [timedOut](Cube.TestBridge.Checks.NamesAndTogglesCheck.md#Cube_TestBridge_Checks_NamesAndTogglesCheck_timedOut) if it doesn't.

```csharp
private IEnumerable<CheckStep> Draw(CubeWindow window)
```

#### Parameters

`window` [CubeWindow](Cube.CubeWindow.md)

The cube window.

#### Returns

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[CheckStep](Cube.TestBridge.CheckStep.md)\>

### <a id="Cube_TestBridge_Checks_NamesAndTogglesCheck_HasSelf_Cube_CubeWindow_"></a> HasSelf\(CubeWindow\)

Whether the local player is in the frame.

```csharp
private static bool HasSelf(CubeWindow window)
```

#### Parameters

`window` [CubeWindow](Cube.CubeWindow.md)

The cube window.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_TestBridge_Checks_NamesAndTogglesCheck_Run_Cube_TestBridge_BridgeContext_"></a> Run\(BridgeContext\)

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
