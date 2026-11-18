# <a id="Cube_Core_GizmoModel"></a> Class GizmoModel

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

The model matrix the corner gizmo works on, which carries both the view orientation (#79) and the cube's size
(#90), and the rules for reading a drag back from it (PROJECT.md, "View orientation control" and "Control" under
the box size: the axis handles change width, height or depth, the centre handle is zoom).

```csharp
public static class GizmoModel
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[GizmoModel](Cube.Core.GizmoModel.md)


## Remarks

<p>
ImGuizmo's rotate and scale operations share one model matrix. Its rotation is the view orientation
([ToGizmoModel](Cube.Core.ViewOrientation.md#Cube_Core_ViewOrientation_ToGizmoModel)); its rows are scaled by the cube's extents over
[Default](Cube.Core.BoxExtents.md#Cube_Core_BoxExtents_Default), so the default cube has scale 1. Rows are the model's local axes, and local axis i
is the cube's axis i, so the x, y and z scale handles change the width, height and depth whatever the current
orientation is.
</p>
<p>
Reading back, a drag is sorted into what it changed so one never disturbs the other (#90: "the rotation from #79
still works and isn't disturbed by scaling"): a turn only when the rotation moved by more than float noise, a
resize only on axes whose scale moved. All three scales moving by the same ratio is the centre handle, which zooms
with [Zoom](Cube.Core.BoxExtents.md#Cube_Core_BoxExtents_Zoom) so the ratios survive clamping. Pure maths on structs: nothing is allocated, so
it is safe in the per-frame path.
</p>

## Fields

### <a id="Cube_Core_GizmoModel_RotationTolerance"></a> RotationTolerance

Smallest change in any of the view's axes that counts as a turn. ImGuizmo's re-orthonormalising leaves noise
around 1e-6; one pixel of drag turns the view by hundredths of a radian.

```csharp
public const float RotationTolerance = 0.0001
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_GizmoModel_ScaleTolerance"></a> ScaleTolerance

Smallest change in an axis's scale ratio that counts as a resize on that axis.

```csharp
public const float ScaleTolerance = 0.0001
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_GizmoModel_UniformTolerance"></a> UniformTolerance

How close the three ratios must be (relative to the largest) for a drag to count as the centre handle's zoom.

```csharp
public const float UniformTolerance = 0.001
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

## Methods

### <a id="Cube_Core_GizmoModel_Build_Cube_Core_ViewOrientation__System_Numerics_Vector3_"></a> Build\(in ViewOrientation, Vector3\)

The model matrix for the gizmo: the view's rotation with each local axis scaled by the cube's extent.

```csharp
public static Matrix4x4 Build(in ViewOrientation view, Vector3 size)
```

#### Parameters

`view` [ViewOrientation](Cube.Core.ViewOrientation.md)

The current view orientation.

`size` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The cube's width, height and depth; clamped first.

#### Returns

 [Matrix4x4](https://learn.microsoft.com/dotnet/api/system.numerics.matrix4x4)

#### Remarks

Scaling rows before the Z mirror in [ToGizmoModel](Cube.Core.ViewOrientation.md#Cube_Core_ViewOrientation_ToGizmoModel) is the same as after it: the
mirror only flips signs, so it commutes with a diagonal scale.

### <a id="Cube_Core_GizmoModel_Differs_Cube_Core_ViewOrientation__Cube_Core_ViewOrientation__"></a> Differs\(in ViewOrientation, in ViewOrientation\)

Whether two orientations differ by more than [RotationTolerance](Cube.Core.GizmoModel.md#Cube_Core_GizmoModel_RotationTolerance) in any axis component.

```csharp
private static bool Differs(in ViewOrientation a, in ViewOrientation b)
```

#### Parameters

`a` [ViewOrientation](Cube.Core.ViewOrientation.md)

One orientation.

`b` [ViewOrientation](Cube.Core.ViewOrientation.md)

The other.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_GizmoModel_MaxAbs_System_Numerics_Vector3_"></a> MaxAbs\(Vector3\)

The largest absolute component of a vector.

```csharp
private static float MaxAbs(Vector3 v)
```

#### Parameters

`v` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The vector.

#### Returns

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_GizmoModel_Read_System_Numerics_Matrix4x4__Cube_Core_ViewOrientation__System_Numerics_Vector3_"></a> Read\(in Matrix4x4, in ViewOrientation, Vector3\)

Reads what a gizmo drag changed: the view, the size, both or neither.

```csharp
public static GizmoChange Read(in Matrix4x4 model, in ViewOrientation view, Vector3 size)
```

#### Parameters

`model` [Matrix4x4](https://learn.microsoft.com/dotnet/api/system.numerics.matrix4x4)

The model matrix after <code>ImGuizmo.Manipulate</code>.

`view` [ViewOrientation](Cube.Core.ViewOrientation.md)

The view the matrix was built from.

`size` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The size the matrix was built from.

#### Returns

 [GizmoChange](Cube.Core.GizmoChange.md)

The new view and size, and which of them changed. Unchanged parts are returned exactly as given.

### <a id="Cube_Core_GizmoModel_ReadSize_System_Numerics_Matrix4x4__System_Numerics_Vector3_"></a> ReadSize\(in Matrix4x4, Vector3\)

The size a matrix holds: per-axis changes on the axes that moved, or a zoom when all three moved together.

```csharp
private static Vector3 ReadSize(in Matrix4x4 model, Vector3 size)
```

#### Parameters

`model` [Matrix4x4](https://learn.microsoft.com/dotnet/api/system.numerics.matrix4x4)

The model matrix.

`size` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The clamped size it was built from.

#### Returns

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_GizmoModel_RowLength_System_Single_System_Single_System_Single_"></a> RowLength\(float, float, float\)

The length of one matrix row: that local axis's scale.

```csharp
private static float RowLength(float x, float y, float z)
```

#### Parameters

`x` [float](https://learn.microsoft.com/dotnet/api/system.single)

First entry.

`y` [float](https://learn.microsoft.com/dotnet/api/system.single)

Second entry.

`z` [float](https://learn.microsoft.com/dotnet/api/system.single)

Third entry.

#### Returns

 [float](https://learn.microsoft.com/dotnet/api/system.single)
