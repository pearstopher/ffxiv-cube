# <a id="Cube_Tests_GizmoModelTests"></a> Class GizmoModelTests

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Checks the corner gizmo's shared model matrix (PROJECT.md, "View orientation control"; #90 "Control"): the axis
handles change width, height or depth in the cube's own axes whatever the orientation, the centre handle zooms
keeping the ratios, a turn never changes the size and a resize never changes the view. ImGuizmo's handles are
simulated by scaling the matrix's rows (its local axes) the way its scale operation does.

```csharp
public class GizmoModelTests
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[GizmoModelTests](Cube.Tests.GizmoModelTests.md)


## Fields

### <a id="Cube_Tests_GizmoModelTests_Turned"></a> Turned

```csharp
private static readonly ViewOrientation Turned
```

#### Field Value

 [ViewOrientation](Cube.Core.ViewOrientation.md)

## Methods

### <a id="Cube_Tests_GizmoModelTests_AssertClose_System_Numerics_Vector3_System_Numerics_Vector3_"></a> AssertClose\(Vector3, Vector3\)

```csharp
private static void AssertClose(Vector3 expected, Vector3 actual)
```

#### Parameters

`expected` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

`actual` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Tests_GizmoModelTests_Axis_handle_changes_only_its_own_extent_and_not_the_view_System_Int32_"></a> Axis\_handle\_changes\_only\_its\_own\_extent\_and\_not\_the\_view\(int\)

```csharp
[Theory]
[InlineData(new object[] { 0 })]
[InlineData(new object[] { 1 })]
[InlineData(new object[] { 2 })]
public void Axis_handle_changes_only_its_own_extent_and_not_the_view(int axis)
```

#### Parameters

`axis` [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Tests_GizmoModelTests_Axis_handle_is_clamped_to_1_to_100"></a> Axis\_handle\_is\_clamped\_to\_1\_to\_100\(\)

```csharp
[Fact]
public void Axis_handle_is_clamped_to_1_to_100()
```

### <a id="Cube_Tests_GizmoModelTests_Centre_handle_zooms_keeping_ratios_even_at_the_limit"></a> Centre\_handle\_zooms\_keeping\_ratios\_even\_at\_the\_limit\(\)

```csharp
[Fact]
public void Centre_handle_zooms_keeping_ratios_even_at_the_limit()
```

### <a id="Cube_Tests_GizmoModelTests_Component_System_Numerics_Vector3_System_Int32_"></a> Component\(Vector3, int\)

```csharp
private static float Component(Vector3 v, int i)
```

#### Parameters

`v` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

`i` [int](https://learn.microsoft.com/dotnet/api/system.int32)

#### Returns

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_GizmoModelTests_Default_cube_has_scale_one_so_the_model_is_the_plain_view"></a> Default\_cube\_has\_scale\_one\_so\_the\_model\_is\_the\_plain\_view\(\)

```csharp
[Fact]
public void Default_cube_has_scale_one_so_the_model_is_the_plain_view()
```

### <a id="Cube_Tests_GizmoModelTests_Reading_does_not_allocate"></a> Reading\_does\_not\_allocate\(\)

```csharp
[Fact]
public void Reading_does_not_allocate()
```

### <a id="Cube_Tests_GizmoModelTests_Turning_a_box_keeps_its_size_exactly"></a> Turning\_a\_box\_keeps\_its\_size\_exactly\(\)

```csharp
[Fact]
public void Turning_a_box_keeps_its_size_exactly()
```

### <a id="Cube_Tests_GizmoModelTests_Untouched_model_changes_nothing"></a> Untouched\_model\_changes\_nothing\(\)

```csharp
[Fact]
public void Untouched_model_changes_nothing()
```
