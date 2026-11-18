# <a id="Cube_Tests_ViewOrientationTests"></a> Class ViewOrientationTests

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Checks the user's view orientation and the projection it drives (PROJECT.md, "View orientation control": the view
control is a rotate gizmo that turns how the cube is shown, any orientation including roll; default from above and
behind at 30°, front face away; orientations saved by the old yaw-and-tilt control carry over; "Efficiency and
memory": no per-frame allocations).

```csharp
public class ViewOrientationTests
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ViewOrientationTests](Cube.Tests.ViewOrientationTests.md)


## Fields

### <a id="Cube_Tests_ViewOrientationTests_Deg"></a> Deg

```csharp
private const float Deg = 0.017453292
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_ViewOrientationTests_Origin"></a> Origin

```csharp
private static readonly Vector2 Origin
```

#### Field Value

 [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

## Methods

### <a id="Cube_Tests_ViewOrientationTests_AssertSameAxes_Cube_Core_ViewOrientation_Cube_Core_ViewOrientation_"></a> AssertSameAxes\(ViewOrientation, ViewOrientation\)

```csharp
private static void AssertSameAxes(ViewOrientation expected, ViewOrientation actual)
```

#### Parameters

`expected` [ViewOrientation](Cube.Core.ViewOrientation.md)

`actual` [ViewOrientation](Cube.Core.ViewOrientation.md)

### <a id="Cube_Tests_ViewOrientationTests_Default_view_is_unchanged_from_the_old_control_default"></a> Default\_view\_is\_unchanged\_from\_the\_old\_control\_default\(\)

```csharp
[Fact]
public void Default_view_is_unchanged_from_the_old_control_default()
```

### <a id="Cube_Tests_ViewOrientationTests_Default_view_reproduces_the_old_fixed_30_degree_projection"></a> Default\_view\_reproduces\_the\_old\_fixed\_30\_degree\_projection\(\)

```csharp
[Fact]
public void Default_view_reproduces_the_old_fixed_30_degree_projection()
```

### <a id="Cube_Tests_ViewOrientationTests_Dragging_the_x_ring_turns_the_cube_around_its_own_x_axis"></a> Dragging\_the\_x\_ring\_turns\_the\_cube\_around\_its\_own\_x\_axis\(\)

```csharp
[Fact]
public void Dragging_the_x_ring_turns_the_cube_around_its_own_x_axis()
```

### <a id="Cube_Tests_ViewOrientationTests_Fitted_cube_stays_inside_the_region_and_keeps_its_size_at_any_orientation_System_Single_System_Single_"></a> Fitted\_cube\_stays\_inside\_the\_region\_and\_keeps\_its\_size\_at\_any\_orientation\(float, float\)

```csharp
[Theory]
[InlineData(new object[] { 0, 30 })]
[InlineData(new object[] { 60, 75 })]
[InlineData(new object[] { -150, -60 })]
[InlineData(new object[] { 10, 90 })]
public void Fitted_cube_stays_inside_the_region_and_keeps_its_size_at_any_orientation(float yaw, float pitch)
```

#### Parameters

`yaw` [float](https://learn.microsoft.com/dotnet/api/system.single)

`pitch` [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_ViewOrientationTests_Gizmo_drift_and_scale_are_removed"></a> Gizmo\_drift\_and\_scale\_are\_removed\(\)

```csharp
[Fact]
public void Gizmo_drift_and_scale_are_removed()
```

### <a id="Cube_Tests_ViewOrientationTests_Gizmo_model_of_the_default_view_is_seen_from_above_and_behind"></a> Gizmo\_model\_of\_the\_default\_view\_is\_seen\_from\_above\_and\_behind\(\)

```csharp
[Fact]
public void Gizmo_model_of_the_default_view_is_seen_from_above_and_behind()
```

### <a id="Cube_Tests_ViewOrientationTests_Looking_straight_down_puts_forward_at_the_top_of_the_screen"></a> Looking\_straight\_down\_puts\_forward\_at\_the\_top\_of\_the\_screen\(\)

```csharp
[Fact]
public void Looking_straight_down_puts_forward_at_the_top_of_the_screen()
```

### <a id="Cube_Tests_ViewOrientationTests_Old_tilt_is_clamped_and_heading_wrapped"></a> Old\_tilt\_is\_clamped\_and\_heading\_wrapped\(\)

```csharp
[Fact]
public void Old_tilt_is_clamped_and_heading_wrapped()
```

### <a id="Cube_Tests_ViewOrientationTests_Old_yaw_and_tilt_give_the_same_projection_as_the_old_control_System_Single_System_Single_"></a> Old\_yaw\_and\_tilt\_give\_the\_same\_projection\_as\_the\_old\_control\(float, float\)

```csharp
[Theory]
[InlineData(new object[] { 0, 30 })]
[InlineData(new object[] { 45, 30 })]
[InlineData(new object[] { -120, 10 })]
[InlineData(new object[] { 170, -40 })]
[InlineData(new object[] { -179, 89 })]
[InlineData(new object[] { 25, 90 })]
[InlineData(new object[] { -60, -90 })]
public void Old_yaw_and_tilt_give_the_same_projection_as_the_old_control(float yaw, float pitch)
```

#### Parameters

`yaw` [float](https://learn.microsoft.com/dotnet/api/system.single)

`pitch` [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_ViewOrientationTests_Projecting_and_converting_do_not_allocate"></a> Projecting\_and\_converting\_do\_not\_allocate\(\)

```csharp
[Fact]
public void Projecting_and_converting_do_not_allocate()
```

### <a id="Cube_Tests_ViewOrientationTests_Projection_Cube_Core_ViewOrientation_"></a> Projection\(ViewOrientation\)

```csharp
private static ViewProjection Projection(ViewOrientation view)
```

#### Parameters

`view` [ViewOrientation](Cube.Core.ViewOrientation.md)

#### Returns

 [ViewProjection](Cube.Core.ViewProjection.md)

### <a id="Cube_Tests_ViewOrientationTests_Rolled_Cube_Core_ViewOrientation_System_Single_"></a> Rolled\(ViewOrientation, float\)

```csharp
private static ViewOrientation Rolled(ViewOrientation view, float degrees)
```

#### Parameters

`view` [ViewOrientation](Cube.Core.ViewOrientation.md)

`degrees` [float](https://learn.microsoft.com/dotnet/api/system.single)

#### Returns

 [ViewOrientation](Cube.Core.ViewOrientation.md)

### <a id="Cube_Tests_ViewOrientationTests_Rolling_the_view_changes_the_projection"></a> Rolling\_the\_view\_changes\_the\_projection\(\)

```csharp
[Fact]
public void Rolling_the_view_changes_the_projection()
```

### <a id="Cube_Tests_ViewOrientationTests_Rotation_round_trips_through_the_gizmo_model_matrix_System_Single_System_Single_System_Single_"></a> Rotation\_round\_trips\_through\_the\_gizmo\_model\_matrix\(float, float, float\)

```csharp
[Theory]
[InlineData(new object[] { 0, 30, 0 })]
[InlineData(new object[] { 45, 30, 0 })]
[InlineData(new object[] { -120, 10, 40 })]
[InlineData(new object[] { 170, -40, -75 })]
[InlineData(new object[] { 25, 90, 120 })]
[InlineData(new object[] { -60, -90, 180 })]
public void Rotation_round_trips_through_the_gizmo_model_matrix(float yaw, float pitch, float roll)
```

#### Parameters

`yaw` [float](https://learn.microsoft.com/dotnet/api/system.single)

`pitch` [float](https://learn.microsoft.com/dotnet/api/system.single)

`roll` [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_ViewOrientationTests_SamplePoints"></a> SamplePoints\(\)

```csharp
private static IEnumerable<Vector3> SamplePoints()
```

#### Returns

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)\>

### <a id="Cube_Tests_ViewOrientationTests_Space_System_Single_"></a> Space\(float\)

```csharp
private static CubeSpace Space(float yaw = 0)
```

#### Parameters

`yaw` [float](https://learn.microsoft.com/dotnet/api/system.single)

#### Returns

 [CubeSpace](Cube.Core.CubeSpace.md)

### <a id="Cube_Tests_ViewOrientationTests_Straight_down_shows_only_the_top_face"></a> Straight\_down\_shows\_only\_the\_top\_face\(\)

```csharp
[Fact]
public void Straight_down_shows_only_the_top_face()
```

### <a id="Cube_Tests_ViewOrientationTests_Tilted_from_below_shows_the_bottom_face_and_not_the_top"></a> Tilted\_from\_below\_shows\_the\_bottom\_face\_and\_not\_the\_top\(\)

```csharp
[Fact]
public void Tilted_from_below_shows_the_bottom_face_and_not_the_top()
```

### <a id="Cube_Tests_ViewOrientationTests_Turned_view_shows_three_faces_of_a_cube"></a> Turned\_view\_shows\_three\_faces\_of\_a\_cube\(\)

```csharp
[Fact]
public void Turned_view_shows_three_faces_of_a_cube()
```

### <a id="Cube_Tests_ViewOrientationTests_Turning_the_view_a_quarter_turn_right_shows_the_cube_from_its_left_side"></a> Turning\_the\_view\_a\_quarter\_turn\_right\_shows\_the\_cube\_from\_its\_left\_side\(\)

```csharp
[Fact]
public void Turning_the_view_a_quarter_turn_right_shows_the_cube_from_its_left_side()
```

### <a id="Cube_Tests_ViewOrientationTests_Unusable_gizmo_matrix_is_rejected"></a> Unusable\_gizmo\_matrix\_is\_rejected\(\)

```csharp
[Fact]
public void Unusable_gizmo_matrix_is_rejected()
```

### <a id="Cube_Tests_ViewOrientationTests_Unusable_rotation_falls_back_to_the_default"></a> Unusable\_rotation\_falls\_back\_to\_the\_default\(\)

```csharp
[Fact]
public void Unusable_rotation_falls_back_to_the_default()
```

### <a id="Cube_Tests_ViewOrientationTests_View_axes_are_orthonormal_at_any_orientation_System_Single_System_Single_"></a> View\_axes\_are\_orthonormal\_at\_any\_orientation\(float, float\)

```csharp
[Theory]
[InlineData(new object[] { 0, 30 })]
[InlineData(new object[] { 45, 30 })]
[InlineData(new object[] { -120, 10 })]
[InlineData(new object[] { 170, -40 })]
[InlineData(new object[] { 30, 90 })]
[InlineData(new object[] { -75, -90 })]
public void View_axes_are_orthonormal_at_any_orientation(float yaw, float pitch)
```

#### Parameters

`yaw` [float](https://learn.microsoft.com/dotnet/api/system.single)

`pitch` [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_ViewOrientationTests_Wireframe_changes_when_the_view_is_turned"></a> Wireframe\_changes\_when\_the\_view\_is\_turned\(\)

```csharp
[Fact]
public void Wireframe_changes_when_the_view_is_turned()
```
