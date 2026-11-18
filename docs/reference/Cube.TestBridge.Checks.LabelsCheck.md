# <a id="Cube_TestBridge_Checks_LabelsCheck"></a> Class LabelsCheck

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

Checks every other player in the cube gets exactly one name label showing their name, next to their cube and on
top of it, and the local player gets one only while the own-name setting (#112) is on (PROJECT.md, "Features": other players are labelled with their names;
"Labels": the name drawn next to the cube). Uses the standard fake players (#16) so it works with nobody around.

```csharp
internal sealed class LabelsCheck : BridgeCheck
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BridgeCheck](Cube.TestBridge.BridgeCheck.md) ← 
[LabelsCheck](Cube.TestBridge.Checks.LabelsCheck.md)


## Remarks

The text is checked against what the window actually passed to ImGui ([LastLabelText](Cube.CubeWindow.md#Cube_CubeWindow_LastLabelText)),
not just against the draw list, so a label showing the wrong player's name fails. Names stay in memory: the
check reports counts only (PROJECT.md, "Privacy").

## Fields

### <a id="Cube_TestBridge_Checks_LabelsCheck_FramesToWait"></a> FramesToWait

Frames to wait for the window to draw.

```csharp
private const int FramesToWait = 30
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_LabelsCheck_wasOpen"></a> wasOpen

The window's open state before the check.

```csharp
private bool wasOpen
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Properties

### <a id="Cube_TestBridge_Checks_LabelsCheck_Name"></a> Name

Short unique name, shown in the results, e.g. <code>window-opens-and-closes</code>.

```csharp
public override string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

## Methods

### <a id="Cube_TestBridge_Checks_LabelsCheck_Cleanup_Cube_TestBridge_BridgeContext_"></a> Cleanup\(BridgeContext\)

Puts back anything the check changed (e.g. the window's open state). Called after every run,
including when the check failed, threw or timed out, so a check never leaves the plugin altered.

```csharp
public override void Cleanup(BridgeContext context)
```

#### Parameters

`context` [BridgeContext](Cube.TestBridge.BridgeContext.md)

The same context the check ran with.

### <a id="Cube_TestBridge_Checks_LabelsCheck_Run_Cube_TestBridge_BridgeContext_"></a> Run\(BridgeContext\)

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

### <a id="Cube_TestBridge_Checks_LabelsCheck_Verify_Cube_TestBridge_BridgeContext_Cube_CubeWindow_"></a> Verify\(BridgeContext, CubeWindow\)

Checks the last frame's labels.

```csharp
private static CheckStep Verify(BridgeContext context, CubeWindow window)
```

#### Parameters

`context` [BridgeContext](Cube.TestBridge.BridgeContext.md)

For metrics.

`window` [CubeWindow](Cube.CubeWindow.md)

The cube window.

#### Returns

 [CheckStep](Cube.TestBridge.CheckStep.md)
