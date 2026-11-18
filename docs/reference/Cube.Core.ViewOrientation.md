# <a id="Cube_Core_ViewOrientation"></a> Struct ViewOrientation

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

How the user has turned the cube on screen with the view control (PROJECT.md, "View orientation control"): a
heading (yaw) and a tilt (pitch) of the viewer around the cube. Default: from above and behind, 30° elevation,
front face away from the viewer.

```csharp
public readonly struct ViewOrientation : IEquatable<ViewOrientation>
```

#### Implements

[IEquatable<ViewOrientation\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)


## Remarks

<p>
This only changes how the cube is shown. It is separate from [Yaw](Cube.Core.CubeSpace.md#Cube_Core_CubeSpace_Yaw), which turns the
contents with the camera: the camera and player movement never change the view orientation, and the view control
never changes what is inside the cube.
</p>
<p>
Two angles rather than a full rotation, because the view control never rolls the view (ImGuizmo keeps the world's
up direction, even when looking straight down) and two numbers are easy to save, check and reset. The viewer's
axes are worked out once in the constructor, so [ViewProjection](Cube.Core.ViewProjection.md#Cube_Core_ViewProjection) only does dot products per point.
</p>
<p>
The view control is ImGuizmo's view manipulator, which works on a right-handed camera view matrix.
[ToGizmoView](Cube.Core.ViewOrientation.md#Cube_Core_ViewOrientation_ToGizmoView) and [FromGizmoView](Cube.Core.ViewOrientation.md#Cube_Core_ViewOrientation_FromGizmoView) convert, so the plugin can hand ImGuizmo a matrix and
read the user's change back. Pure maths, so it is unit tested without the game.
</p>

## Constructors

### <a id="Cube_Core_ViewOrientation__ctor_System_Single_System_Single_"></a> ViewOrientation\(float, float\)

Creates an orientation, wrapping the yaw into −π…π and clamping the pitch to ±90°.

```csharp
public ViewOrientation(float yaw, float pitch)
```

#### Parameters

`yaw` [float](https://learn.microsoft.com/dotnet/api/system.single)

The viewer's heading, in radians: 0 looks forward (+Z in view coordinates, the front face away from the viewer),
positive turns the viewer to the right (looking from above, clockwise).

`pitch` [float](https://learn.microsoft.com/dotnet/api/system.single)

Tilt of the view above the horizontal, in radians; negative looks up from below.

## Fields

### <a id="Cube_Core_ViewOrientation_DefaultPitchDegrees"></a> DefaultPitchDegrees

Default tilt of the view above the horizontal, in degrees (PROJECT.md: 30°).

```csharp
public const float DefaultPitchDegrees = 30
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_ViewOrientation_MaxPitchDegrees"></a> MaxPitchDegrees

Highest tilt: looking straight down. Lower limit is straight up (the negative).

```csharp
public const float MaxPitchDegrees = 90
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_ViewOrientation_Radians"></a> Radians

Converts degrees to radians.

```csharp
private const float Radians = 0.017453292
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

## Properties

### <a id="Cube_Core_ViewOrientation_Default"></a> Default

The default view (PROJECT.md): from behind, 30° above, front face away.

```csharp
public static ViewOrientation Default { get; }
```

#### Property Value

 [ViewOrientation](Cube.Core.ViewOrientation.md)

### <a id="Cube_Core_ViewOrientation_Forward"></a> Forward

The direction the viewer looks in, in view coordinates; depth grows along it.

```csharp
public Vector3 Forward { get; }
```

#### Property Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_ViewOrientation_Pitch"></a> Pitch

The view's tilt above the horizontal, in radians (−π/2…π/2).

```csharp
public float Pitch { get; }
```

#### Property Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_ViewOrientation_PitchDegrees"></a> PitchDegrees

The pitch in degrees, as saved in the settings.

```csharp
public float PitchDegrees { get; }
```

#### Property Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_ViewOrientation_Right"></a> Right

The screen's right direction, in view coordinates. Always horizontal: the view never rolls.

```csharp
public Vector3 Right { get; }
```

#### Property Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_ViewOrientation_Up"></a> Up

The screen's up direction, in view coordinates.

```csharp
public Vector3 Up { get; }
```

#### Property Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_ViewOrientation_Yaw"></a> Yaw

The viewer's heading, in radians (−π…π).

```csharp
public float Yaw { get; }
```

#### Property Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_ViewOrientation_YawDegrees"></a> YawDegrees

The yaw in degrees, as saved in the settings.

```csharp
public float YawDegrees { get; }
```

#### Property Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

## Methods

### <a id="Cube_Core_ViewOrientation_Equals_Cube_Core_ViewOrientation_"></a> Equals\(ViewOrientation\)

Indicates whether the current object is equal to another object of the same type.

```csharp
public bool Equals(ViewOrientation other)
```

#### Parameters

`other` [ViewOrientation](Cube.Core.ViewOrientation.md)

An object to compare with this object.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> if the current object is equal to the <code class="paramref">other</code> parameter; otherwise, <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a>.

### <a id="Cube_Core_ViewOrientation_Equals_System_Object_"></a> Equals\(object?\)

Indicates whether this instance and a specified object are equal.

```csharp
public override bool Equals(object? obj)
```

#### Parameters

`obj` [object](https://learn.microsoft.com/dotnet/api/system.object)?

The object to compare with the current instance.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> if <code class="paramref">obj</code> and this instance are the same type and represent the same value; otherwise, <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a>.

### <a id="Cube_Core_ViewOrientation_FromDegrees_System_Single_System_Single_"></a> FromDegrees\(float, float\)

An orientation from angles in degrees, as stored in the settings.

```csharp
public static ViewOrientation FromDegrees(float yawDegrees, float pitchDegrees)
```

#### Parameters

`yawDegrees` [float](https://learn.microsoft.com/dotnet/api/system.single)

Heading, in degrees.

`pitchDegrees` [float](https://learn.microsoft.com/dotnet/api/system.single)

Tilt, in degrees.

#### Returns

 [ViewOrientation](Cube.Core.ViewOrientation.md)

### <a id="Cube_Core_ViewOrientation_FromGizmoView_System_Numerics_Matrix4x4__"></a> FromGizmoView\(in Matrix4x4\)

Reads an orientation back from an ImGuizmo view matrix (after the user dragged or clicked the view control).
Any roll is dropped, and a view that somehow went past straight up or down is clamped.

```csharp
public static ViewOrientation FromGizmoView(in Matrix4x4 view)
```

#### Parameters

`view` [Matrix4x4](https://learn.microsoft.com/dotnet/api/system.numerics.matrix4x4)

The view matrix ImGuizmo returned.

#### Returns

 [ViewOrientation](Cube.Core.ViewOrientation.md)

#### Remarks

The matrix's columns are the camera's right, up and back axes in ImGuizmo's world, which is view coordinates
with Z mirrored (see [ToGizmoView](Cube.Core.ViewOrientation.md#Cube_Core_ViewOrientation_ToGizmoView)). The yaw comes from the right axis, which stays horizontal and
well defined even looking straight down; if it ever isn't usable, the forward direction is used instead.

### <a id="Cube_Core_ViewOrientation_GetHashCode"></a> GetHashCode\(\)

Returns the hash code for this instance.

```csharp
public override int GetHashCode()
```

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

A 32-bit signed integer that is the hash code for this instance.

### <a id="Cube_Core_ViewOrientation_Mirror_System_Numerics_Vector3_"></a> Mirror\(Vector3\)

Converts between view coordinates and ImGuizmo's right-handed world by mirroring Z.

```csharp
private static Vector3 Mirror(Vector3 v)
```

#### Parameters

`v` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

A direction.

#### Returns

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_ViewOrientation_ToGizmoView_System_Single_"></a> ToGizmoView\(float\)

The camera view matrix ImGuizmo's view manipulator expects for this orientation: right-handed, looking at the
cube's centre from <code class="paramref">distance</code> away, laid out like `System.Numerics.Matrix4x4.CreateLookAt(System.Numerics.Vector3%2cSystem.Numerics.Vector3%2cSystem.Numerics.Vector3)`.

```csharp
public Matrix4x4 ToGizmoView(float distance)
```

#### Parameters

`distance` [float](https://learn.microsoft.com/dotnet/api/system.single)

How far the camera is from the centre; only matters to ImGuizmo's own maths.

#### Returns

 [Matrix4x4](https://learn.microsoft.com/dotnet/api/system.numerics.matrix4x4)

#### Remarks

View coordinates are left-handed (right, up, forward), ImGuizmo's world is right-handed, so Z is mirrored on
the way in and out. The widget's own cube therefore turns exactly like the drawn cube.

## Operators

### <a id="Cube_Core_ViewOrientation_op_Equality_Cube_Core_ViewOrientation_Cube_Core_ViewOrientation_"></a> operator ==\(ViewOrientation, ViewOrientation\)

Two orientations are equal when their angles are exactly equal.

```csharp
public static bool operator ==(ViewOrientation left, ViewOrientation right)
```

#### Parameters

`left` [ViewOrientation](Cube.Core.ViewOrientation.md)

One orientation.

`right` [ViewOrientation](Cube.Core.ViewOrientation.md)

The other.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_ViewOrientation_op_Inequality_Cube_Core_ViewOrientation_Cube_Core_ViewOrientation_"></a> operator \!=\(ViewOrientation, ViewOrientation\)

Two orientations differ when either angle differs.

```csharp
public static bool operator !=(ViewOrientation left, ViewOrientation right)
```

#### Parameters

`left` [ViewOrientation](Cube.Core.ViewOrientation.md)

One orientation.

`right` [ViewOrientation](Cube.Core.ViewOrientation.md)

The other.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)
