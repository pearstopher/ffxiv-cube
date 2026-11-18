# <a id="Cube_TestBridge_Checks_OrientationCheck"></a> Class OrientationCheck

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

Checks both orientation modes (PROJECT.md, "Cube" and "Cube on screen never turns by itself"): in Camera mode the
cube's alignment yaw matches the live camera's heading, in North up mode it is 0 whatever the camera does; and
turning the camera leaves the wireframe exactly where it was on screen while the contents rotate, entering and
leaving at the sides.

```csharp
internal sealed class OrientationCheck : BridgeCheck
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BridgeCheck](Cube.TestBridge.BridgeCheck.md) ← 
[OrientationCheck](Cube.TestBridge.Checks.OrientationCheck.md)


## Remarks

<p>
The live camera is read again here, independently of the window, through [CameraReader](Cube.Game.CameraReader.md#Cube_Game_CameraReader) and turned
into a heading with [FromForward](Cube.Core.ViewYaw.md#Cube_Core_ViewYaw_FromForward). A small tolerance covers the camera moving between the
window's draw and this check, plus [ChangeThreshold](Cube.Core.ViewYaw.md#Cube_Core_ViewYaw_ChangeThreshold). If the camera happens to face north,
North up can't be told apart from Camera mode; the <code>cameraYaw</code> metric shows when that was the case.
</p>
<p>
The turning part uses the window's Debug-only simulated camera, so no game input is needed: two fakes are fixed in
the world (one east of the player, one near a horizontal corner), the "camera" faces north and then north-east,
and the check compares the 12 wireframe lines and where the fakes went. The mode, simulated camera and fakes are
changed in memory only and restored afterwards.
</p>

## Fields

### <a id="Cube_TestBridge_Checks_OrientationCheck_FacingNorth"></a> FacingNorth

The simulated camera facing north: alignment yaw 0.

```csharp
private static readonly Vector3 FacingNorth
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_TestBridge_Checks_OrientationCheck_FacingNorthEast"></a> FacingNorthEast

The simulated camera facing north-east: alignment yaw π/4, the turn that moves corners furthest.

```csharp
private static readonly Vector3 FacingNorthEast
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_TestBridge_Checks_OrientationCheck_LineTolerance"></a> LineTolerance

How far a wireframe end may move between the two simulated yaws, in pixels: float noise only.

```csharp
private const float LineTolerance = 0.01
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_OrientationCheck_Tolerance"></a> Tolerance

Largest allowed difference between the cube's yaw and the camera heading, in radians (about 1°).

```csharp
private const float Tolerance = 0.02
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_OrientationCheck_wasMode"></a> wasMode

The orientation setting before the check.

```csharp
private OrientationMode wasMode
```

#### Field Value

 [OrientationMode](Cube.Core.OrientationMode.md)

### <a id="Cube_TestBridge_Checks_OrientationCheck_wasOpen"></a> wasOpen

The window's open state before the check.

```csharp
private bool wasOpen
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Properties

### <a id="Cube_TestBridge_Checks_OrientationCheck_Name"></a> Name

Short unique name, shown in the results, e.g. <code>window-opens-and-closes</code>.

```csharp
public override string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

## Methods

### <a id="Cube_TestBridge_Checks_OrientationCheck_Cleanup_Cube_TestBridge_BridgeContext_"></a> Cleanup\(BridgeContext\)

Puts back anything the check changed (e.g. the window's open state). Called after every run,
including when the check failed, threw or timed out, so a check never leaves the plugin altered.

```csharp
public override void Cleanup(BridgeContext context)
```

#### Parameters

`context` [BridgeContext](Cube.TestBridge.BridgeContext.md)

The same context the check ran with.

### <a id="Cube_TestBridge_Checks_OrientationCheck_HasFake_Cube_CubeWindow_System_Int32_"></a> HasFake\(CubeWindow, int\)

Whether a world-fixed fake is in the cube and drawn (it has at least one face tagged as its own).

```csharp
internal static bool HasFake(CubeWindow window, int fakeIndex)
```

#### Parameters

`window` [CubeWindow](Cube.CubeWindow.md)

The cube window.

`fakeIndex` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Which fake, in the order added.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_TestBridge_Checks_OrientationCheck_Lines_Cube_CubeWindow_"></a> Lines\(CubeWindow\)

The last frame's wireframe lines, as end points on screen.

```csharp
private static List<(Vector2 A, Vector2 B)> Lines(CubeWindow window)
```

#### Parameters

`window` [CubeWindow](Cube.CubeWindow.md)

The cube window.

#### Returns

 [List](https://learn.microsoft.com/dotnet/api/system.collections.generic.list\-1)<\([Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2) [A](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.numerics.vector2,system.numerics.vector2\-.a), [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2) [B](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.numerics.vector2,system.numerics.vector2\-.b)\)\>

### <a id="Cube_TestBridge_Checks_OrientationCheck_Run_Cube_TestBridge_BridgeContext_"></a> Run\(BridgeContext\)

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

### <a id="Cube_TestBridge_Checks_OrientationCheck_SameLines_System_Collections_Generic_List_System_ValueTuple_System_Numerics_Vector2_System_Numerics_Vector2___System_Collections_Generic_List_System_ValueTuple_System_Numerics_Vector2_System_Numerics_Vector2___System_String__"></a> SameLines\(List<\(Vector2 A, Vector2 B\)\>, List<\(Vector2 A, Vector2 B\)\>, out string\)

Whether two frames drew the same 12 lines (in any order, since depth sorting may reorder them).

```csharp
private static bool SameLines(List<(Vector2 A, Vector2 B)> before, List<(Vector2 A, Vector2 B)> after, out string why)
```

#### Parameters

`before` [List](https://learn.microsoft.com/dotnet/api/system.collections.generic.list\-1)<\([Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2) [A](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.numerics.vector2,system.numerics.vector2\-.a), [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2) [B](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.numerics.vector2,system.numerics.vector2\-.b)\)\>

The first frame's lines.

`after` [List](https://learn.microsoft.com/dotnet/api/system.collections.generic.list\-1)<\([Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2) [A](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.numerics.vector2,system.numerics.vector2\-.a), [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2) [B](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.numerics.vector2,system.numerics.vector2\-.b)\)\>

The second frame's lines.

`why` [string](https://learn.microsoft.com/dotnet/api/system.string)

What differed, if anything.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)
