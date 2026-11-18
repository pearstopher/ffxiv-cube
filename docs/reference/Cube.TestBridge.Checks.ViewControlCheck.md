# <a id="Cube_TestBridge_Checks_ViewControlCheck"></a> Class ViewControlCheck

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

Checks the view orientation control (PROJECT.md, "View orientation control"): a turned view changes the projected
wireframe exactly as Core says, turning the camera never changes the view, the control is drawn in the chosen
corner, and resetting brings back the default view. Writes a draw-list snapshot from several angles.

```csharp
internal sealed class ViewControlCheck : BridgeCheck
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BridgeCheck](Cube.TestBridge.BridgeCheck.md) ← 
[ViewControlCheck](Cube.TestBridge.Checks.ViewControlCheck.md)


## Remarks

<p>
The orientation is set through the settings, which is exactly where the control writes the user's drag
([SetView](Cube.Core.CubeSettings.md#Cube_Core_CubeSettings_SetView)), so this tests everything after the mouse: settings → projection → frame.
Dragging ImGuizmo's widget itself needs a real mouse and is a manual step (<code>docs/manual-tests.md</code>).
</p>
<p>
Settings are changed in memory only, never saved, and the user's values are restored in [Cleanup](Cube.TestBridge.Checks.ViewControlCheck.md#Cube_TestBridge_Checks_ViewControlCheck_Cleanup).
The snapshots (<code>drawlist-view-*.json</code>) contain geometry only, like [FrameSnapshotCheck](Cube.TestBridge.Checks.FrameSnapshotCheck.md#Cube_TestBridge_Checks_FrameSnapshotCheck)'s.
</p>

## Constructors

### <a id="Cube_TestBridge_Checks_ViewControlCheck__ctor_System_String_"></a> ViewControlCheck\(string\)

Checks the view orientation control (PROJECT.md, "View orientation control"): a turned view changes the projected
wireframe exactly as Core says, turning the camera never changes the view, the control is drawn in the chosen
corner, and resetting brings back the default view. Writes a draw-list snapshot from several angles.

```csharp
public ViewControlCheck(string folder)
```

#### Parameters

`folder` [string](https://learn.microsoft.com/dotnet/api/system.string)

The bridge folder the snapshots are written to.

#### Remarks

<p>
The orientation is set through the settings, which is exactly where the control writes the user's drag
([SetView](Cube.Core.CubeSettings.md#Cube_Core_CubeSettings_SetView)), so this tests everything after the mouse: settings → projection → frame.
Dragging ImGuizmo's widget itself needs a real mouse and is a manual step (<code>docs/manual-tests.md</code>).
</p>
<p>
Settings are changed in memory only, never saved, and the user's values are restored in [Cleanup](Cube.TestBridge.Checks.ViewControlCheck.md#Cube_TestBridge_Checks_ViewControlCheck_Cleanup).
The snapshots (<code>drawlist-view-*.json</code>) contain geometry only, like [FrameSnapshotCheck](Cube.TestBridge.Checks.FrameSnapshotCheck.md#Cube_TestBridge_Checks_FrameSnapshotCheck)'s.
</p>

## Fields

### <a id="Cube_TestBridge_Checks_ViewControlCheck_Angles"></a> Angles

The angles snapshotted (yaw°, pitch°, name): default, turned and tilted, from below, straight down.

```csharp
private static readonly (float Yaw, float Pitch, string Name)[] Angles
```

#### Field Value

 \([float](https://learn.microsoft.com/dotnet/api/system.single) [Yaw](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.single,system.single,system.string\-.yaw), [float](https://learn.microsoft.com/dotnet/api/system.single) [Pitch](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.single,system.single,system.string\-.pitch), [string](https://learn.microsoft.com/dotnet/api/system.string) [Name](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.single,system.single,system.string\-.name)\)\[\]

### <a id="Cube_TestBridge_Checks_ViewControlCheck_LineTolerance"></a> LineTolerance

How far a wireframe end may be from Core's own projection, in pixels: float noise only.

```csharp
private const float LineTolerance = 0.01
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_ViewControlCheck_saved"></a> saved

The user's settings before the check.

```csharp
private readonly CubeSettings saved
```

#### Field Value

 [CubeSettings](Cube.Core.CubeSettings.md)

### <a id="Cube_TestBridge_Checks_ViewControlCheck_wasOpen"></a> wasOpen

The window's open state before the check.

```csharp
private bool wasOpen
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Properties

### <a id="Cube_TestBridge_Checks_ViewControlCheck_Name"></a> Name

Short unique name, shown in the results, e.g. <code>window-opens-and-closes</code>.

```csharp
public override string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Cube_TestBridge_Checks_ViewControlCheck_Timeout"></a> Timeout

The longest the check may run before it fails as timed out.

```csharp
public override TimeSpan Timeout { get; }
```

#### Property Value

 [TimeSpan](https://learn.microsoft.com/dotnet/api/system.timespan)

#### Remarks

Ten seconds covers checks that wait a few hundred frames; override for longer ones.

## Methods

### <a id="Cube_TestBridge_Checks_ViewControlCheck_Cleanup_Cube_TestBridge_BridgeContext_"></a> Cleanup\(BridgeContext\)

Puts back anything the check changed (e.g. the window's open state). Called after every run,
including when the check failed, threw or timed out, so a check never leaves the plugin altered.

```csharp
public override void Cleanup(BridgeContext context)
```

#### Parameters

`context` [BridgeContext](Cube.TestBridge.BridgeContext.md)

The same context the check ran with.

### <a id="Cube_TestBridge_Checks_ViewControlCheck_Lines_Cube_CubeWindow_"></a> Lines\(CubeWindow\)

The last frame's wireframe lines, as end points on screen.

```csharp
private static List<(Vector2 A, Vector2 B)> Lines(CubeWindow window)
```

#### Parameters

`window` [CubeWindow](Cube.CubeWindow.md)

The cube window.

#### Returns

 [List](https://learn.microsoft.com/dotnet/api/system.collections.generic.list\-1)<\([Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2) [A](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.numerics.vector2,system.numerics.vector2\-.a), [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2) [B](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.numerics.vector2,system.numerics.vector2\-.b)\)\>

### <a id="Cube_TestBridge_Checks_ViewControlCheck_MatchesCore_Cube_CubeWindow_Cube_Core_ViewOrientation_"></a> MatchesCore\(CubeWindow, ViewOrientation\)

Checks the last frame used the wanted orientation and its 12 wireframe lines are where Core projects the cube's
corners with that orientation.

```csharp
private static string? MatchesCore(CubeWindow window, ViewOrientation want)
```

#### Parameters

`window` [CubeWindow](Cube.CubeWindow.md)

The cube window.

`want` [ViewOrientation](Cube.Core.ViewOrientation.md)

The orientation set in the settings.

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)?

What was wrong, or null.

### <a id="Cube_TestBridge_Checks_ViewControlCheck_MaxMove_System_Collections_Generic_List_System_ValueTuple_System_Numerics_Vector2_System_Numerics_Vector2___System_Collections_Generic_List_System_ValueTuple_System_Numerics_Vector2_System_Numerics_Vector2___"></a> MaxMove\(List<\(Vector2 A, Vector2 B\)\>, List<\(Vector2 A, Vector2 B\)\>\)

The furthest any line of <code class="paramref">before</code> is from its nearest line in <code class="paramref">after</code>
(lines may be reordered by the depth sort); infinite if the counts differ.

```csharp
private static float MaxMove(List<(Vector2 A, Vector2 B)> before, List<(Vector2 A, Vector2 B)> after)
```

#### Parameters

`before` [List](https://learn.microsoft.com/dotnet/api/system.collections.generic.list\-1)<\([Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2) [A](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.numerics.vector2,system.numerics.vector2\-.a), [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2) [B](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.numerics.vector2,system.numerics.vector2\-.b)\)\>

One frame's lines.

`after` [List](https://learn.microsoft.com/dotnet/api/system.collections.generic.list\-1)<\([Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2) [A](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.numerics.vector2,system.numerics.vector2\-.a), [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2) [B](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.numerics.vector2,system.numerics.vector2\-.b)\)\>

Another frame's lines.

#### Returns

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_ViewControlCheck_Run_Cube_TestBridge_BridgeContext_"></a> Run\(BridgeContext\)

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
