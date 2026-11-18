# <a id="Cube_Tests_ViewOrientationTests"></a> Class ViewOrientationTests

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Checks the user's view orientation and the projection it drives (PROJECT.md, "View orientation control": the view
control turns how the cube is shown, yaw and tilt; default from above and behind at 30°, front face away; the
orientation is saved and can be reset; "Efficiency and memory": no per-frame allocations).

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

### <a id="Cube_Tests_ViewOrientationTests_Default_view_reproduces_the_old_fixed_30_degree_projection"></a> Default\_view\_reproduces\_the\_old\_fixed\_30\_degree\_projection\(\)

```csharp
[Fact]
public void Default_view_reproduces_the_old_fixed_30_degree_projection()
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

### <a id="Cube_Tests_ViewOrientationTests_Gizmo_drag_to_the_side_reads_back_as_a_turned_view"></a> Gizmo\_drag\_to\_the\_side\_reads\_back\_as\_a\_turned\_view\(\)

```csharp
[Fact]
public void Gizmo_drag_to_the_side_reads_back_as_a_turned_view()
```

### <a id="Cube_Tests_ViewOrientationTests_Gizmo_view_from_straight_above_keeps_its_heading"></a> Gizmo\_view\_from\_straight\_above\_keeps\_its\_heading\(\)

```csharp
[Fact]
public void Gizmo_view_from_straight_above_keeps_its_heading()
```

### <a id="Cube_Tests_ViewOrientationTests_Gizmo_view_matrix_matches_a_right_handed_look_at_from_above_and_behind"></a> Gizmo\_view\_matrix\_matches\_a\_right\_handed\_look\_at\_from\_above\_and\_behind\(\)

```csharp
[Fact]
public void Gizmo_view_matrix_matches_a_right_handed_look_at_from_above_and_behind()
```

### <a id="Cube_Tests_ViewOrientationTests_Gizmo_view_matrix_round_trips_System_Single_System_Single_"></a> Gizmo\_view\_matrix\_round\_trips\(float, float\)

```csharp
[Theory]
[InlineData(new object[] { 0, 30 })]
[InlineData(new object[] { 45, 30 })]
[InlineData(new object[] { -120, 10 })]
[InlineData(new object[] { 170, -40 })]
[InlineData(new object[] { -179, 89 })]
[InlineData(new object[] { 25, 90 })]
[InlineData(new object[] { -60, -90 })]
public void Gizmo_view_matrix_round_trips(float yaw, float pitch)
```

#### Parameters

`yaw` [float](https://learn.microsoft.com/dotnet/api/system.single)

`pitch` [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_ViewOrientationTests_Looking_straight_down_puts_forward_at_the_top_of_the_screen"></a> Looking\_straight\_down\_puts\_forward\_at\_the\_top\_of\_the\_screen\(\)

```csharp
[Fact]
public void Looking_straight_down_puts_forward_at_the_top_of_the_screen()
```

### <a id="Cube_Tests_ViewOrientationTests_Pitch_is_clamped_and_yaw_wrapped"></a> Pitch\_is\_clamped\_and\_yaw\_wrapped\(\)

```csharp
[Fact]
public void Pitch_is_clamped_and_yaw_wrapped()
```

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
