# <a id="Cube_TestBridge_Checks_CollisionInsideCubeCheck"></a> Class CollisionInsideCubeCheck

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

Checks collision never reaches outside the cube on screen, at any camera angle, in either orientation mode (#93;
PROJECT.md, "Collision (phase 2)": clipped to the cube; "Cube": in "Rotate contents with camera" the contents turn
with the camera).

```csharp
internal sealed class CollisionInsideCubeCheck : BridgeCheck
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BridgeCheck](Cube.TestBridge.BridgeCheck.md) ← 
[CollisionInsideCubeCheck](Cube.TestBridge.Checks.CollisionInsideCubeCheck.md)


## Remarks

<p>
In "Rotate contents with camera" the Debug-only simulated camera turns the cube to 0°, 30°, 45° and 90°; in "North
up" the cube stays at 0° and the view is turned instead. For each, every corner of every collision face in the
window's real frame must be inside the cube's projected outline ([CubeOutline](Cube.Core.CubeOutline.md#Cube_Core_CubeOutline)), within a small
tolerance for rounding.
</p>
<p>
The real scenery may not hold a box bigger than the cube, the case that showed the bug, so each angle also clips a
synthetic floor three times the cube's size, centred under the player, with the same cube and projection, and
checks its faces the same way. Before the fix that floor's corners reached up to √2 past the cube's sides at 30°
and 45°. Settings and the simulated camera are changed in memory only and restored in [Cleanup](Cube.TestBridge.Checks.CollisionInsideCubeCheck.md#Cube_TestBridge_Checks_CollisionInsideCubeCheck_Cleanup).
</p>

## Fields

### <a id="Cube_TestBridge_Checks_CollisionInsideCubeCheck_CameraYaws"></a> CameraYaws

The cube yaws tried in Camera mode, in degrees.

```csharp
private static readonly float[] CameraYaws
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)\[\]

### <a id="Cube_TestBridge_Checks_CollisionInsideCubeCheck_Tolerance"></a> Tolerance

How far outside the outline a corner may be, in pixels: float rounding only.

```csharp
private const float Tolerance = 0.05
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_CollisionInsideCubeCheck_YawTolerance"></a> YawTolerance

Largest allowed difference between the cube's yaw and the simulated camera's, in radians.

```csharp
private const float YawTolerance = 0.02
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_CollisionInsideCubeCheck_saved"></a> saved

The user's settings before the check, restored by [Cleanup](Cube.TestBridge.Checks.CollisionInsideCubeCheck.md#Cube_TestBridge_Checks_CollisionInsideCubeCheck_Cleanup).

```csharp
private readonly CubeSettings saved
```

#### Field Value

 [CubeSettings](Cube.Core.CubeSettings.md)

### <a id="Cube_TestBridge_Checks_CollisionInsideCubeCheck_synthetic"></a> synthetic

The synthetic floor's faces, rebuilt for each angle; allocated once per check run.

```csharp
private readonly DrawList synthetic
```

#### Field Value

 [DrawList](Cube.Core.DrawList.md)

### <a id="Cube_TestBridge_Checks_CollisionInsideCubeCheck_wasOpen"></a> wasOpen

The window's open state before the check, restored by [Cleanup](Cube.TestBridge.Checks.CollisionInsideCubeCheck.md#Cube_TestBridge_Checks_CollisionInsideCubeCheck_Cleanup).

```csharp
private bool wasOpen
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Properties

### <a id="Cube_TestBridge_Checks_CollisionInsideCubeCheck_Name"></a> Name

Short unique name, shown in the results, e.g. <code>window-opens-and-closes</code>.

```csharp
public override string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Cube_TestBridge_Checks_CollisionInsideCubeCheck_Timeout"></a> Timeout

The longest the check may run before it fails as timed out.

```csharp
public override TimeSpan Timeout { get; }
```

#### Property Value

 [TimeSpan](https://learn.microsoft.com/dotnet/api/system.timespan)

#### Remarks

Ten seconds covers checks that wait a few hundred frames; override for longer ones.

## Methods

### <a id="Cube_TestBridge_Checks_CollisionInsideCubeCheck_CheckFrame_Cube_CubeWindow_System_String_System_Int32__"></a> CheckFrame\(CubeWindow, string, ref int\)

Checks the window's last frame and the synthetic floor against the cube's outline.

```csharp
private string? CheckFrame(CubeWindow window, string angle, ref int liveCorners)
```

#### Parameters

`window` [CubeWindow](Cube.CubeWindow.md)

The cube window.

`angle` [string](https://learn.microsoft.com/dotnet/api/system.string)

Which angle this is, for the message.

`liveCorners` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Incremented for every real collision corner checked.

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)?

What was wrong, or null.

### <a id="Cube_TestBridge_Checks_CollisionInsideCubeCheck_Cleanup_Cube_TestBridge_BridgeContext_"></a> Cleanup\(BridgeContext\)

Puts back anything the check changed (e.g. the window's open state). Called after every run,
including when the check failed, threw or timed out, so a check never leaves the plugin altered.

```csharp
public override void Cleanup(BridgeContext context)
```

#### Parameters

`context` [BridgeContext](Cube.TestBridge.BridgeContext.md)

The same context the check ran with.

### <a id="Cube_TestBridge_Checks_CollisionInsideCubeCheck_CountPrisms_System_ReadOnlySpan_Cube_Core_ClippedBox__"></a> CountPrisms\(ReadOnlySpan<ClippedBox\>\)

How many clipped boxes are prisms rather than plain boxes.

```csharp
private static int CountPrisms(ReadOnlySpan<ClippedBox> boxes)
```

#### Parameters

`boxes` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[ClippedBox](Cube.Core.ClippedBox.md)\>

The clipped boxes.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionInsideCubeCheck_FindOutside_Cube_Core_DrawList_Cube_Core_CubeOutline__System_Int32__System_String__"></a> FindOutside\(DrawList, in CubeOutline, ref int, out string\)

Finds a corner of a face in a list that lies outside the cube's outline.

```csharp
private static bool FindOutside(DrawList list, in CubeOutline outline, ref int checkedCorners, out string where)
```

#### Parameters

`list` [DrawList](Cube.Core.DrawList.md)

The faces.

`outline` [CubeOutline](Cube.Core.CubeOutline.md)

The cube's outline.

`checkedCorners` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Incremented for every corner checked.

`where` [string](https://learn.microsoft.com/dotnet/api/system.string)

The offending corner and its face's tag, if any.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_TestBridge_Checks_CollisionInsideCubeCheck_Run_Cube_TestBridge_BridgeContext_"></a> Run\(BridgeContext\)

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
