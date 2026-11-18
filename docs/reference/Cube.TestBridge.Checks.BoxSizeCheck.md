# <a id="Cube_TestBridge_Checks_BoxSizeCheck"></a> Class BoxSizeCheck

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

Checks the box size and zoom (#90; PROJECT.md, "Cube", "Zoom", "On screen", "Culling"): set extents change the
projected wireframe's proportions, zooming changes which points are inside, resetting restores 40 × 40 × 40, and
an idle frame changes no settings.

```csharp
internal sealed class BoxSizeCheck : BridgeCheck
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BridgeCheck](Cube.TestBridge.BridgeCheck.md) ← 
[BoxSizeCheck](Cube.TestBridge.Checks.BoxSizeCheck.md)


## Remarks

<p>
The view is set straight on and level (heading 0, tilt 0) and the cube north up, so the wireframe's height and width
on screen are exactly the box's height and width at one scale: a 20-wide, 80-tall box must measure four times as
tall as wide. A fake fixed 18 yalms east of the player is inside the default cube (half-width 20), outside after
setting the zoom slider (#94) to 32 through the same [ZoomSlider](Cube.Core.ZoomSlider.md#Cube_Core_ZoomSlider) the settings window uses (half-width
16), and inside again after "Reset size".
</p>
<p>
Dragging the gizmo's handles needs a real mouse, so it is a manual step in <code>docs/manual-tests.md</code>; the maths
that turns a drag into a resize or zoom is unit tested in <code>GizmoModelTests</code>. The settings, fakes and window
state are changed in memory only and restored in [Cleanup](Cube.TestBridge.Checks.BoxSizeCheck.md#Cube_TestBridge_Checks_BoxSizeCheck_Cleanup).
</p>

## Fields

### <a id="Cube_TestBridge_Checks_BoxSizeCheck_IdleFrames"></a> IdleFrames

Frames watched with nothing touched, to check the gizmo leaves the settings alone when idle.

```csharp
private const int IdleFrames = 10
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_BoxSizeCheck_RatioTolerance"></a> RatioTolerance

How far the measured height-to-width ratio may be from the box's, for rounding.

```csharp
private const float RatioTolerance = 0.01
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_BoxSizeCheck_TallBox"></a> TallBox

The box drawn: tall and narrow, so its proportions can't be mistaken for a cube's.

```csharp
private static readonly Vector3 TallBox
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_TestBridge_Checks_BoxSizeCheck_ZoomedLargest"></a> ZoomedLargest

The zoom slider value set (largest extent): the default cube becomes 32 × 32 × 32, half-width 16.

```csharp
private const float ZoomedLargest = 32
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_BoxSizeCheck_saved"></a> saved

The user's settings before the check, restored by [Cleanup](Cube.TestBridge.Checks.BoxSizeCheck.md#Cube_TestBridge_Checks_BoxSizeCheck_Cleanup).

```csharp
private readonly CubeSettings saved
```

#### Field Value

 [CubeSettings](Cube.Core.CubeSettings.md)

### <a id="Cube_TestBridge_Checks_BoxSizeCheck_wasOpen"></a> wasOpen

The window's open state before the check, restored by [Cleanup](Cube.TestBridge.Checks.BoxSizeCheck.md#Cube_TestBridge_Checks_BoxSizeCheck_Cleanup).

```csharp
private bool wasOpen
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Properties

### <a id="Cube_TestBridge_Checks_BoxSizeCheck_Name"></a> Name

Short unique name, shown in the results, e.g. <code>window-opens-and-closes</code>.

```csharp
public override string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Cube_TestBridge_Checks_BoxSizeCheck_Timeout"></a> Timeout

The longest the check may run before it fails as timed out.

```csharp
public override TimeSpan Timeout { get; }
```

#### Property Value

 [TimeSpan](https://learn.microsoft.com/dotnet/api/system.timespan)

#### Remarks

Ten seconds covers checks that wait a few hundred frames; override for longer ones.

## Methods

### <a id="Cube_TestBridge_Checks_BoxSizeCheck_Cleanup_Cube_TestBridge_BridgeContext_"></a> Cleanup\(BridgeContext\)

Puts back anything the check changed (e.g. the window's open state). Called after every run,
including when the check failed, threw or timed out, so a check never leaves the plugin altered.

```csharp
public override void Cleanup(BridgeContext context)
```

#### Parameters

`context` [BridgeContext](Cube.TestBridge.BridgeContext.md)

The same context the check ran with.

### <a id="Cube_TestBridge_Checks_BoxSizeCheck_Run_Cube_TestBridge_BridgeContext_"></a> Run\(BridgeContext\)

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

### <a id="Cube_TestBridge_Checks_BoxSizeCheck_WireRatio_Cube_CubeWindow_"></a> WireRatio\(CubeWindow\)

The last frame's wireframe height on screen divided by its width.

```csharp
private static float WireRatio(CubeWindow window)
```

#### Parameters

`window` [CubeWindow](Cube.CubeWindow.md)

The cube window.

#### Returns

 [float](https://learn.microsoft.com/dotnet/api/system.single)
