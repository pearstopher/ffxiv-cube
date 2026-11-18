# <a id="Cube_TestBridge_Checks_DrawListMatchesCoreCheck"></a> Class DrawListMatchesCoreCheck

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

Checks the window drew exactly what Core produced (PROJECT.md, "Automated testing": the primitives drawn
match Core's output): rebuilds the last frame independently from the same cube space and projection,
compares it primitive by primitive (and each polygon's corners, #93), and checks one ImGui call was issued per
primitive.

```csharp
internal sealed class DrawListMatchesCoreCheck : BridgeCheck
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BridgeCheck](Cube.TestBridge.BridgeCheck.md) ← 
[DrawListMatchesCoreCheck](Cube.TestBridge.Checks.DrawListMatchesCoreCheck.md)


## Remarks

The frame is the wireframe (12 edges) plus one solid cube per player in the window's player set (#17) and a
label per other player (#18), and the collision boxes clipped to the cube (#31).
Later features extend what [BuildFrame](Cube.CubeWindow.md#Cube_CubeWindow_BuildFrame) builds, not this comparison.

## Fields

### <a id="Cube_TestBridge_Checks_DrawListMatchesCoreCheck_FramesToWait"></a> FramesToWait

Frames to wait for the window to draw after opening it.

```csharp
private const int FramesToWait = 30
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_DrawListMatchesCoreCheck_expected"></a> expected

An independent list to rebuild the frame into; allocated once per check run.

```csharp
private readonly DrawList expected
```

#### Field Value

 [DrawList](Cube.Core.DrawList.md)

### <a id="Cube_TestBridge_Checks_DrawListMatchesCoreCheck_wasOpen"></a> wasOpen

The window's open state before the check, restored by [Cleanup](Cube.TestBridge.Checks.DrawListMatchesCoreCheck.md#Cube_TestBridge_Checks_DrawListMatchesCoreCheck_Cleanup).

```csharp
private bool wasOpen
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Properties

### <a id="Cube_TestBridge_Checks_DrawListMatchesCoreCheck_Name"></a> Name

Short unique name, shown in the results, e.g. <code>window-opens-and-closes</code>.

```csharp
public override string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

## Methods

### <a id="Cube_TestBridge_Checks_DrawListMatchesCoreCheck_Cleanup_Cube_TestBridge_BridgeContext_"></a> Cleanup\(BridgeContext\)

Puts back anything the check changed (e.g. the window's open state). Called after every run,
including when the check failed, threw or timed out, so a check never leaves the plugin altered.

```csharp
public override void Cleanup(BridgeContext context)
```

#### Parameters

`context` [BridgeContext](Cube.TestBridge.BridgeContext.md)

The same context the check ran with.

### <a id="Cube_TestBridge_Checks_DrawListMatchesCoreCheck_Run_Cube_TestBridge_BridgeContext_"></a> Run\(BridgeContext\)

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
