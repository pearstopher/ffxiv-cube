# <a id="Cube_Core_ViewOrientation"></a> Struct ViewOrientation

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

How the user has turned the cube on screen with the view control (PROJECT.md, "View orientation control"): a full
rotation, roll included. Default: from above and behind, 30° elevation, front face away from the viewer.

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
A quaternion rather than angles, because the control is ImGuizmo's rotate gizmo, whose rings can turn the cube
around any axis, so any orientation is allowed (PROJECT.md: "including roll"). The rotation takes a point in view
coordinates to the viewer's (right, up, forward) frame. The viewer's axes are worked out once in the constructor,
so [ViewProjection](Cube.Core.ViewProjection.md#Cube_Core_ViewProjection) only does dot products per point.
</p>
<p>
Orientations saved by the old yaw-and-tilt control (#63) are converted with [FromDegrees](Cube.Core.ViewOrientation.md#Cube_Core_ViewOrientation_FromDegrees).
[ToGizmoModel](Cube.Core.ViewOrientation.md#Cube_Core_ViewOrientation_ToGizmoModel) and [TryFromGizmoModel](Cube.Core.ViewOrientation.md#Cube_Core_ViewOrientation_TryFromGizmoModel) convert to and from the model matrix the gizmo
turns. Pure maths, so it is unit tested without the game.
</p>

## Constructors

### <a id="Cube_Core_ViewOrientation__ctor_System_Numerics_Quaternion_"></a> ViewOrientation\(Quaternion\)

Creates an orientation from a rotation, normalised. A rotation that isn't usable (zero length or not a number,
e.g. from a corrupt file) gives the default view, so a bad value never produces a broken cube.

```csharp
public ViewOrientation(Quaternion rotation)
```

#### Parameters

`rotation` [Quaternion](https://learn.microsoft.com/dotnet/api/system.numerics.quaternion)

Takes a point in view coordinates to the viewer's frame: x along the screen's right, y up, z away from the
viewer.

## Fields

### <a id="Cube_Core_ViewOrientation_DefaultPitchDegrees"></a> DefaultPitchDegrees

Default tilt of the view above the horizontal, in degrees (PROJECT.md: 30°).

```csharp
public const float DefaultPitchDegrees = 30
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_ViewOrientation_MaxPitchDegrees"></a> MaxPitchDegrees

Highest tilt the old yaw-and-tilt control allowed: looking straight down (and −90° straight up).

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

### <a id="Cube_Core_ViewOrientation_Right"></a> Right

The screen's right direction, in view coordinates.

```csharp
public Vector3 Right { get; }
```

#### Property Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_ViewOrientation_Rotation"></a> Rotation

The rotation from view coordinates to the viewer's frame, normalised. This is what's saved.

```csharp
public Quaternion Rotation { get; }
```

#### Property Value

 [Quaternion](https://learn.microsoft.com/dotnet/api/system.numerics.quaternion)

### <a id="Cube_Core_ViewOrientation_Up"></a> Up

The screen's up direction, in view coordinates.

```csharp
public Vector3 Up { get; }
```

#### Property Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

## Methods

### <a id="Cube_Core_ViewOrientation_DefaultRotation"></a> DefaultRotation\(\)

The default rotation: heading 0, tilt 30° (computed, not stored, so [Default](Cube.Core.ViewOrientation.md#Cube_Core_ViewOrientation_Default) can't recurse).

```csharp
private static Quaternion DefaultRotation()
```

#### Returns

 [Quaternion](https://learn.microsoft.com/dotnet/api/system.numerics.quaternion)

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

### <a id="Cube_Core_ViewOrientation_FromAxes_System_Numerics_Vector3_System_Numerics_Vector3_System_Numerics_Vector3_"></a> FromAxes\(Vector3, Vector3, Vector3\)

A rotation matrix whose columns are the viewer's axes, so a point times it gives its viewer coordinates.

```csharp
private static Matrix4x4 FromAxes(Vector3 right, Vector3 up, Vector3 forward)
```

#### Parameters

`right` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The screen's right, in view coordinates.

`up` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The screen's up.

`forward` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The viewing direction.

#### Returns

 [Matrix4x4](https://learn.microsoft.com/dotnet/api/system.numerics.matrix4x4)

### <a id="Cube_Core_ViewOrientation_FromDegrees_System_Single_System_Single_"></a> FromDegrees\(float, float\)

An orientation from a heading and a tilt in degrees: the old yaw-and-tilt control's saved values (#63), and a
handy way to name a view in tests. The tilt is clamped to ±90°, as the old control did.

```csharp
public static ViewOrientation FromDegrees(float yawDegrees, float pitchDegrees)
```

#### Parameters

`yawDegrees` [float](https://learn.microsoft.com/dotnet/api/system.single)

The viewer's heading: 0 looks forward (+Z in view coordinates, the front face away from the viewer), positive
turns the viewer to the right (looking from above, clockwise).

`pitchDegrees` [float](https://learn.microsoft.com/dotnet/api/system.single)

Tilt of the view above the horizontal; negative looks up from below.

#### Returns

 [ViewOrientation](Cube.Core.ViewOrientation.md)

### <a id="Cube_Core_ViewOrientation_GetHashCode"></a> GetHashCode\(\)

Returns the hash code for this instance.

```csharp
public override int GetHashCode()
```

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

A 32-bit signed integer that is the hash code for this instance.

### <a id="Cube_Core_ViewOrientation_IsUsable_System_Numerics_Quaternion_"></a> IsUsable\(Quaternion\)

Whether a rotation can be normalised: every part a number and not (nearly) zero length.

```csharp
private static bool IsUsable(Quaternion q)
```

#### Parameters

`q` [Quaternion](https://learn.microsoft.com/dotnet/api/system.numerics.quaternion)

The rotation.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_ViewOrientation_Mirror_System_Numerics_Matrix4x4_"></a> Mirror\(Matrix4x4\)

Converts a rotation between view coordinates and ImGuizmo's right-handed world by mirroring Z on both sides
(S·M·S with S = diag(1, 1, −1)), which negates the entries that mix Z with X or Y. Its own inverse.

```csharp
private static Matrix4x4 Mirror(Matrix4x4 m)
```

#### Parameters

`m` [Matrix4x4](https://learn.microsoft.com/dotnet/api/system.numerics.matrix4x4)

A rotation matrix.

#### Returns

 [Matrix4x4](https://learn.microsoft.com/dotnet/api/system.numerics.matrix4x4)

### <a id="Cube_Core_ViewOrientation_ToGizmoModel"></a> ToGizmoModel\(\)

The model matrix for ImGuizmo's rotate gizmo: the cube's rotation as seen by a camera on the +Z axis looking
at the centre (laid out like `System.Numerics.Matrix4x4.CreateLookAt(System.Numerics.Vector3%2cSystem.Numerics.Vector3%2cSystem.Numerics.Vector3)`, the way ImGuizmo expects).

```csharp
public Matrix4x4 ToGizmoModel()
```

#### Returns

 [Matrix4x4](https://learn.microsoft.com/dotnet/api/system.numerics.matrix4x4)

#### Remarks

View coordinates are left-handed (right, up, forward), ImGuizmo's world is right-handed, so Z is mirrored on
both sides of the rotation. The gizmo's x, y and z rings then lie around the cube's own x, y and z axes, so the
ring colours match the cube's axes and dragging the x ring turns the cube around its x axis.

### <a id="Cube_Core_ViewOrientation_TryFromGizmoModel_System_Numerics_Matrix4x4__Cube_Core_ViewOrientation__"></a> TryFromGizmoModel\(in Matrix4x4, out ViewOrientation\)

Reads an orientation back from the model matrix ImGuizmo's rotate gizmo returned after a drag. Any scale or
drift the gizmo's maths added is removed.

```csharp
public static bool TryFromGizmoModel(in Matrix4x4 model, out ViewOrientation view)
```

#### Parameters

`model` [Matrix4x4](https://learn.microsoft.com/dotnet/api/system.numerics.matrix4x4)

The model matrix, laid out as by [ToGizmoModel](Cube.Core.ViewOrientation.md#Cube_Core_ViewOrientation_ToGizmoModel).

`view` [ViewOrientation](Cube.Core.ViewOrientation.md)

The orientation it holds.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

False if the matrix holds no usable rotation; the caller then keeps the old view.

## Operators

### <a id="Cube_Core_ViewOrientation_op_Equality_Cube_Core_ViewOrientation_Cube_Core_ViewOrientation_"></a> operator ==\(ViewOrientation, ViewOrientation\)

Two orientations are equal when their rotations are exactly equal.

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

Two orientations differ when their rotations differ.

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
