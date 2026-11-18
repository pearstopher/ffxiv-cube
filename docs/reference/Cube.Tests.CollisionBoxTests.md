# <a id="Cube_Tests_CollisionBoxTests"></a> Class CollisionBoxTests

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Checks the collision box model (PROJECT.md, "Collision (phase 2)": colliders inside or crossing the cube are shown
as simplified boxes clipped to the cube; stale data is refreshed when the player moves; "Efficiency and memory":
bounded cache with a hard cap, collision data dropped on a zone change, no per-frame allocations). The box source
(one box per mesh node or primitive collider through its world matrix) is the #28 recommendation in
<code>docs/research-notes.md</code>.

```csharp
public class CollisionBoxTests
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[CollisionBoxTests](Cube.Tests.CollisionBoxTests.md)


## Fields

### <a id="Cube_Tests_CollisionBoxTests_Centre"></a> Centre

```csharp
private static readonly Vector3 Centre
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Tests_CollisionBoxTests_Precision"></a> Precision

```csharp
private const float Precision = 0.0001
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

## Methods

### <a id="Cube_Tests_CollisionBoxTests_Around_System_Numerics_Vector3_System_Numerics_Vector3_"></a> Around\(Vector3, Vector3\)

```csharp
private static CollisionBox Around(Vector3 centre, Vector3 half)
```

#### Parameters

`centre` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

`half` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

#### Returns

 [CollisionBox](Cube.Core.CollisionBox.md)

### <a id="Cube_Tests_CollisionBoxTests_AssertBox_Cube_Core_CollisionBox_Cube_Core_CollisionBox_"></a> AssertBox\(CollisionBox, CollisionBox\)

```csharp
private static void AssertBox(CollisionBox expected, CollisionBox actual)
```

#### Parameters

`expected` [CollisionBox](Cube.Core.CollisionBox.md)

`actual` [CollisionBox](Cube.Core.CollisionBox.md)

### <a id="Cube_Tests_CollisionBoxTests_Box_System_Single_System_Single_System_Single_System_Single_System_Single_System_Single_"></a> Box\(float, float, float, float, float, float\)

```csharp
private static CollisionBox Box(float x0, float y0, float z0, float x1, float y1, float z1)
```

#### Parameters

`x0` [float](https://learn.microsoft.com/dotnet/api/system.single)

`y0` [float](https://learn.microsoft.com/dotnet/api/system.single)

`z0` [float](https://learn.microsoft.com/dotnet/api/system.single)

`x1` [float](https://learn.microsoft.com/dotnet/api/system.single)

`y1` [float](https://learn.microsoft.com/dotnet/api/system.single)

`z1` [float](https://learn.microsoft.com/dotnet/api/system.single)

#### Returns

 [CollisionBox](Cube.Core.CollisionBox.md)

### <a id="Cube_Tests_CollisionBoxTests_Box_beside_the_cube_is_left_out"></a> Box\_beside\_the\_cube\_is\_left\_out\(\)

```csharp
[Fact]
public void Box_beside_the_cube_is_left_out()
```

### <a id="Cube_Tests_CollisionBoxTests_Box_beyond_the_region_is_not_kept"></a> Box\_beyond\_the\_region\_is\_not\_kept\(\)

```csharp
[Fact]
public void Box_beyond_the_region_is_not_kept()
```

### <a id="Cube_Tests_CollisionBoxTests_Box_crossing_the_cube_north_up_is_cut_exactly_at_its_faces"></a> Box\_crossing\_the\_cube\_north\_up\_is\_cut\_exactly\_at\_its\_faces\(\)

```csharp
[Fact]
public void Box_crossing_the_cube_north_up_is_cut_exactly_at_its_faces()
```

### <a id="Cube_Tests_CollisionBoxTests_Box_holding_the_last_box_replaces_it"></a> Box\_holding\_the\_last\_box\_replaces\_it\(\)

```csharp
[Fact]
public void Box_holding_the_last_box_replaces_it()
```

### <a id="Cube_Tests_CollisionBoxTests_Box_in_the_corner_gap_of_a_turned_cube_is_left_out"></a> Box\_in\_the\_corner\_gap\_of\_a\_turned\_cube\_is\_left\_out\(\)

```csharp
[Fact]
public void Box_in_the_corner_gap_of_a_turned_cube_is_left_out()
```

### <a id="Cube_Tests_CollisionBoxTests_Box_inside_the_cube_is_unchanged_by_clipping"></a> Box\_inside\_the\_cube\_is\_unchanged\_by\_clipping\(\)

```csharp
[Fact]
public void Box_inside_the_cube_is_unchanged_by_clipping()
```

### <a id="Cube_Tests_CollisionBoxTests_Box_inside_the_last_box_is_merged_into_it"></a> Box\_inside\_the\_last\_box\_is\_merged\_into\_it\(\)

```csharp
[Fact]
public void Box_inside_the_last_box_is_merged_into_it()
```

### <a id="Cube_Tests_CollisionBoxTests_Box_inside_the_region_is_kept"></a> Box\_inside\_the\_region\_is\_kept\(\)

```csharp
[Fact]
public void Box_inside_the_region_is_kept()
```

### <a id="Cube_Tests_CollisionBoxTests_Boxes_that_differ_on_two_axes_are_not_merged"></a> Boxes\_that\_differ\_on\_two\_axes\_are\_not\_merged\(\)

```csharp
[Fact]
public void Boxes_that_differ_on_two_axes_are_not_merged()
```

### <a id="Cube_Tests_CollisionBoxTests_Clipped_box_follows_the_turned_cube_closely"></a> Clipped\_box\_follows\_the\_turned\_cube\_closely\(\)

```csharp
[Fact]
public void Clipped_box_follows_the_turned_cube_closely()
```

### <a id="Cube_Tests_CollisionBoxTests_Clipped_box_never_reaches_beyond_the_box_or_the_cube_height"></a> Clipped\_box\_never\_reaches\_beyond\_the\_box\_or\_the\_cube\_height\(\)

```csharp
[Fact]
public void Clipped_box_never_reaches_beyond_the_box_or_the_cube_height()
```

### <a id="Cube_Tests_CollisionBoxTests_Clipping_a_list_keeps_only_boxes_in_the_cube_in_order"></a> Clipping\_a\_list\_keeps\_only\_boxes\_in\_the\_cube\_in\_order\(\)

```csharp
[Fact]
public void Clipping_a_list_keeps_only_boxes_in_the_cube_in_order()
```

### <a id="Cube_Tests_CollisionBoxTests_Clipping_stops_when_the_destination_is_full"></a> Clipping\_stops\_when\_the\_destination\_is\_full\(\)

```csharp
[Fact]
public void Clipping_stops_when_the_destination_is_full()
```

### <a id="Cube_Tests_CollisionBoxTests_Flat_floor_box_is_valid_but_inside_out_or_nan_boxes_are_not"></a> Flat\_floor\_box\_is\_valid\_but\_inside\_out\_or\_nan\_boxes\_are\_not\(\)

```csharp
[Fact]
public void Flat_floor_box_is_valid_but_inside_out_or_nan_boxes_are_not()
```

### <a id="Cube_Tests_CollisionBoxTests_Fresh_set_needs_a_gather"></a> Fresh\_set\_needs\_a\_gather\(\)

```csharp
[Fact]
public void Fresh_set_needs_a_gather()
```

### <a id="Cube_Tests_CollisionBoxTests_Full_set_drops_extra_boxes_instead_of_growing"></a> Full\_set\_drops\_extra\_boxes\_instead\_of\_growing\(\)

```csharp
[Fact]
public void Full_set_drops_extra_boxes_instead_of_growing()
```

### <a id="Cube_Tests_CollisionBoxTests_Gathering_and_clipping_every_frame_does_not_allocate"></a> Gathering\_and\_clipping\_every\_frame\_does\_not\_allocate\(\)

```csharp
[Fact]
public void Gathering_and_clipping_every_frame_does_not_allocate()
```

### <a id="Cube_Tests_CollisionBoxTests_Invalid_box_is_rejected_and_counted"></a> Invalid\_box\_is\_rejected\_and\_counted\(\)

```csharp
[Fact]
public void Invalid_box_is_rejected_and_counted()
```

### <a id="Cube_Tests_CollisionBoxTests_Lined_up_boxes_with_a_gap_between_them_are_not_merged"></a> Lined\_up\_boxes\_with\_a\_gap\_between\_them\_are\_not\_merged\(\)

```csharp
[Fact]
public void Lined_up_boxes_with_a_gap_between_them_are_not_merged()
```

### <a id="Cube_Tests_CollisionBoxTests_Moving_a_quarter_of_the_cube_needs_a_new_gather"></a> Moving\_a\_quarter\_of\_the\_cube\_needs\_a\_new\_gather\(\)

```csharp
[Fact]
public void Moving_a_quarter_of_the_cube_needs_a_new_gather()
```

### <a id="Cube_Tests_CollisionBoxTests_Node_bounds_through_a_translation_move_with_it"></a> Node\_bounds\_through\_a\_translation\_move\_with\_it\(\)

```csharp
[Fact]
public void Node_bounds_through_a_translation_move_with_it()
```

### <a id="Cube_Tests_CollisionBoxTests_Region_covers_the_cube_at_every_yaw_with_a_margin"></a> Region\_covers\_the\_cube\_at\_every\_yaw\_with\_a\_margin\(\)

```csharp
[Fact]
public void Region_covers_the_cube_at_every_yaw_with_a_margin()
```

### <a id="Cube_Tests_CollisionBoxTests_Rotated_node_gets_the_world_box_around_its_turned_corners"></a> Rotated\_node\_gets\_the\_world\_box\_around\_its\_turned\_corners\(\)

```csharp
[Fact]
public void Rotated_node_gets_the_world_box_around_its_turned_corners()
```

### <a id="Cube_Tests_CollisionBoxTests_Scene_change_or_new_cube_size_needs_a_new_gather"></a> Scene\_change\_or\_new\_cube\_size\_needs\_a\_new\_gather\(\)

```csharp
[Fact]
public void Scene_change_or_new_cube_size_needs_a_new_gather()
```

### <a id="Cube_Tests_CollisionBoxTests_Small_moves_and_turning_the_camera_do_not_need_a_new_gather"></a> Small\_moves\_and\_turning\_the\_camera\_do\_not\_need\_a\_new\_gather\(\)

```csharp
[Fact]
public void Small_moves_and_turning_the_camera_do_not_need_a_new_gather()
```

### <a id="Cube_Tests_CollisionBoxTests_Unit_primitive_through_a_scale_becomes_a_box_of_that_size"></a> Unit\_primitive\_through\_a\_scale\_becomes\_a\_box\_of\_that\_size\(\)

```csharp
[Fact]
public void Unit_primitive_through_a_scale_becomes_a_box_of_that_size()
```

### <a id="Cube_Tests_CollisionBoxTests_Wall_segments_that_line_up_end_to_end_merge_into_one_box"></a> Wall\_segments\_that\_line\_up\_end\_to\_end\_merge\_into\_one\_box\(\)

```csharp
[Fact]
public void Wall_segments_that_line_up_end_to_end_merge_into_one_box()
```

### <a id="Cube_Tests_CollisionBoxTests_Zero_thickness_wall_keeps_its_exact_face_when_clipped"></a> Zero\_thickness\_wall\_keeps\_its\_exact\_face\_when\_clipped\(\)

```csharp
[Fact]
public void Zero_thickness_wall_keeps_its_exact_face_when_clipped()
```

### <a id="Cube_Tests_CollisionBoxTests_Zone_change_drops_every_box"></a> Zone\_change\_drops\_every\_box\(\)

```csharp
[Fact]
public void Zone_change_drops_every_box()
```
