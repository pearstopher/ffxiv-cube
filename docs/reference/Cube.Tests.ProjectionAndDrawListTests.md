# <a id="Cube_Tests_ProjectionAndDrawListTests"></a> Class ProjectionAndDrawListTests

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Checks projection and the draw list (PROJECT.md, "View": fixed-angle view from above and behind, default 30°;
"Draw order": far things first so near cubes cover far ones; "Cheap drawing": only visible faces, the big cube a
wireframe; "Cube on screen never turns by itself": the camera turns the contents, never the wireframe;
"Efficiency and memory": reused buffers, no per-frame allocations).

```csharp
public class ProjectionAndDrawListTests
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ProjectionAndDrawListTests](Cube.Tests.ProjectionAndDrawListTests.md)


## Fields

### <a id="Cube_Tests_ProjectionAndDrawListTests_Elevation"></a> Elevation

```csharp
private static readonly float Elevation
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_ProjectionAndDrawListTests_Origin"></a> Origin

```csharp
private static readonly Vector2 Origin
```

#### Field Value

 [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

## Methods

### <a id="Cube_Tests_ProjectionAndDrawListTests_Big_cube_is_a_wireframe_of_12_edges"></a> Big\_cube\_is\_a\_wireframe\_of\_12\_edges\(\)

```csharp
[Fact]
public void Big_cube_is_a_wireframe_of_12_edges()
```

### <a id="Cube_Tests_ProjectionAndDrawListTests_Building_and_sorting_a_frame_does_not_allocate"></a> Building\_and\_sorting\_a\_frame\_does\_not\_allocate\(\)

```csharp
[Fact]
public void Building_and_sorting_a_frame_does_not_allocate()
```

### <a id="Cube_Tests_ProjectionAndDrawListTests_Clear_empties_the_list_for_the_next_frame"></a> Clear\_empties\_the\_list\_for\_the\_next\_frame\(\)

```csharp
[Fact]
public void Clear_empties_the_list_for_the_next_frame()
```

### <a id="Cube_Tests_ProjectionAndDrawListTests_Contents_turn_with_the_camera_while_the_wireframe_stays"></a> Contents\_turn\_with\_the\_camera\_while\_the\_wireframe\_stays\(\)

```csharp
[Fact]
public void Contents_turn_with_the_camera_while_the_wireframe_stays()
```

### <a id="Cube_Tests_ProjectionAndDrawListTests_Cube_centre_projects_to_the_origin"></a> Cube\_centre\_projects\_to\_the\_origin\(\)

```csharp
[Fact]
public void Cube_centre_projects_to_the_origin()
```

### <a id="Cube_Tests_ProjectionAndDrawListTests_Default_elevation_is_30_degrees"></a> Default\_elevation\_is\_30\_degrees\(\)

```csharp
[Fact]
public void Default_elevation_is_30_degrees()
```

### <a id="Cube_Tests_ProjectionAndDrawListTests_Equal_depths_keep_the_order_they_were_added_in"></a> Equal\_depths\_keep\_the\_order\_they\_were\_added\_in\(\)

```csharp
[Fact]
public void Equal_depths_keep_the_order_they_were_added_in()
```

### <a id="Cube_Tests_ProjectionAndDrawListTests_Far_cubes_are_drawn_before_near_cubes"></a> Far\_cubes\_are\_drawn\_before\_near\_cubes\(\)

```csharp
[Fact]
public void Far_cubes_are_drawn_before_near_cubes()
```

### <a id="Cube_Tests_ProjectionAndDrawListTests_Fitted_cube_stays_inside_the_region_at_any_yaw_System_Single_"></a> Fitted\_cube\_stays\_inside\_the\_region\_at\_any\_yaw\(float\)

```csharp
[Theory]
[InlineData(new object[] { 0 })]
[InlineData(new object[] { 0.785 })]
[InlineData(new object[] { 2 })]
public void Fitted_cube_stays_inside_the_region_at_any_yaw(float yaw)
```

#### Parameters

`yaw` [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_ProjectionAndDrawListTests_Fitted_projection_is_centred_in_the_region"></a> Fitted\_projection\_is\_centred\_in\_the\_region\(\)

```csharp
[Fact]
public void Fitted_projection_is_centred_in_the_region()
```

### <a id="Cube_Tests_ProjectionAndDrawListTests_Forward_appears_higher_on_screen_and_further_away"></a> Forward\_appears\_higher\_on\_screen\_and\_further\_away\(\)

```csharp
[Fact]
public void Forward_appears_higher_on_screen_and_further_away()
```

### <a id="Cube_Tests_ProjectionAndDrawListTests_Full_list_drops_extra_primitives_instead_of_growing"></a> Full\_list\_drops\_extra\_primitives\_instead\_of\_growing\(\)

```csharp
[Fact]
public void Full_list_drops_extra_primitives_instead_of_growing()
```

### <a id="Cube_Tests_ProjectionAndDrawListTests_Higher_points_are_nearer_the_viewer_above"></a> Higher\_points\_are\_nearer\_the\_viewer\_above\(\)

```csharp
[Fact]
public void Higher_points_are_nearer_the_viewer_above()
```

### <a id="Cube_Tests_ProjectionAndDrawListTests_Pack_uses_the_imgui_layout"></a> Pack\_uses\_the\_imgui\_layout\(\)

```csharp
[Fact]
public void Pack_uses_the_imgui_layout()
```

### <a id="Cube_Tests_ProjectionAndDrawListTests_Projection"></a> Projection\(\)

```csharp
private static ViewProjection Projection()
```

#### Returns

 [ViewProjection](Cube.Core.ViewProjection.md)

### <a id="Cube_Tests_ProjectionAndDrawListTests_Right_in_view_is_right_on_screen"></a> Right\_in\_view\_is\_right\_on\_screen\(\)

```csharp
[Fact]
public void Right_in_view_is_right_on_screen()
```

### <a id="Cube_Tests_ProjectionAndDrawListTests_Shade_darkens_colour_but_keeps_alpha"></a> Shade\_darkens\_colour\_but\_keeps\_alpha\(\)

```csharp
[Fact]
public void Shade_darkens_colour_but_keeps_alpha()
```

### <a id="Cube_Tests_ProjectionAndDrawListTests_Solid_cube_at_an_angle_shows_three_faces"></a> Solid\_cube\_at\_an\_angle\_shows\_three\_faces\(\)

```csharp
[Fact]
public void Solid_cube_at_an_angle_shows_three_faces()
```

### <a id="Cube_Tests_ProjectionAndDrawListTests_Solid_cube_facing_north_shows_top_and_the_face_towards_the_viewer"></a> Solid\_cube\_facing\_north\_shows\_top\_and\_the\_face\_towards\_the\_viewer\(\)

```csharp
[Fact]
public void Solid_cube_facing_north_shows_top_and_the_face_towards_the_viewer()
```

### <a id="Cube_Tests_ProjectionAndDrawListTests_Space_System_Single_"></a> Space\(float\)

```csharp
private static CubeSpace Space(float yaw = 0)
```

#### Parameters

`yaw` [float](https://learn.microsoft.com/dotnet/api/system.single)

#### Returns

 [CubeSpace](Cube.Core.CubeSpace.md)

### <a id="Cube_Tests_ProjectionAndDrawListTests_Top_face_is_brightest_and_sides_are_shaded"></a> Top\_face\_is\_brightest\_and\_sides\_are\_shaded\(\)

```csharp
[Fact]
public void Top_face_is_brightest_and_sides_are_shaded()
```

### <a id="Cube_Tests_ProjectionAndDrawListTests_Up_is_up_on_screen_foreshortened_by_the_elevation"></a> Up\_is\_up\_on\_screen\_foreshortened\_by\_the\_elevation\(\)

```csharp
[Fact]
public void Up_is_up_on_screen_foreshortened_by_the_elevation()
```

### <a id="Cube_Tests_ProjectionAndDrawListTests_Wireframe_is_identical_for_any_camera_yaw"></a> Wireframe\_is\_identical\_for\_any\_camera\_yaw\(\)

```csharp
[Fact]
public void Wireframe_is_identical_for_any_camera_yaw()
```
