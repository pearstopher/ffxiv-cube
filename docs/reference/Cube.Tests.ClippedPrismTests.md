# <a id="Cube_Tests_ClippedPrismTests"></a> Class ClippedPrismTests

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Checks collision stays inside the cube at every cube yaw (#93; PROJECT.md, "Collision (phase 2)": each collider is
shown as a simplified box, clipped to the cube; "Cube": in "Rotate contents with camera" the contents turn with
the camera). A box cut by a side of a turned cube is kept as the prism the cut leaves, never as a world-aligned
box that pokes out of the cube; "Efficiency and memory": clipping and drawing allocate nothing.

```csharp
public class ClippedPrismTests
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ClippedPrismTests](Cube.Tests.ClippedPrismTests.md)


## Fields

### <a id="Cube_Tests_ClippedPrismTests_Grey"></a> Grey

```csharp
private static readonly uint Grey
```

#### Field Value

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

### <a id="Cube_Tests_ClippedPrismTests_HugeFloor"></a> HugeFloor

```csharp
private static readonly CollisionBox HugeFloor
```

#### Field Value

 [CollisionBox](Cube.Core.CollisionBox.md)

### <a id="Cube_Tests_ClippedPrismTests_Precision"></a> Precision

```csharp
private const float Precision = 0.001
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_ClippedPrismTests_Scenery"></a> Scenery

```csharp
private static readonly CollisionBox[] Scenery
```

#### Field Value

 [CollisionBox](Cube.Core.CollisionBox.md)\[\]

## Methods

### <a id="Cube_Tests_ClippedPrismTests_Area_Cube_Core_ClippedBox__"></a> Area\(in ClippedBox\)

```csharp
private static float Area(in ClippedBox box)
```

#### Parameters

`box` [ClippedBox](Cube.Core.ClippedBox.md)

#### Returns

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_ClippedPrismTests_At_multiples_of_90_degrees_every_clipped_box_is_a_plain_box_cut_at_the_cube_faces_System_Single_"></a> At\_multiples\_of\_90\_degrees\_every\_clipped\_box\_is\_a\_plain\_box\_cut\_at\_the\_cube\_faces\(float\)

```csharp
[Theory]
[InlineData(new object[] { 0 })]
[InlineData(new object[] { 90 })]
[InlineData(new object[] { 180 })]
[InlineData(new object[] { -90 })]
public void At_multiples_of_90_degrees_every_clipped_box_is_a_plain_box_cut_at_the_cube_faces(float yawDegrees)
```

#### Parameters

`yawDegrees` [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_ClippedPrismTests_Box_inside_a_turned_cube_stays_a_plain_box"></a> Box\_inside\_a\_turned\_cube\_stays\_a\_plain\_box\(\)

```csharp
[Fact]
public void Box_inside_a_turned_cube_stays_a_plain_box()
```

### <a id="Cube_Tests_ClippedPrismTests_Clipped_footprint_never_leaves_the_turned_cube_System_Single_"></a> Clipped\_footprint\_never\_leaves\_the\_turned\_cube\(float\)

```csharp
[Theory]
[InlineData(new object[] { 0 })]
[InlineData(new object[] { 17 })]
[InlineData(new object[] { 30 })]
[InlineData(new object[] { 45 })]
[InlineData(new object[] { 90 })]
[InlineData(new object[] { 200 })]
[InlineData(new object[] { -61 })]
public void Clipped_footprint_never_leaves_the_turned_cube(float yawDegrees)
```

#### Parameters

`yawDegrees` [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_ClippedPrismTests_Clipping_and_drawing_prisms_for_a_full_set_does_not_allocate"></a> Clipping\_and\_drawing\_prisms\_for\_a\_full\_set\_does\_not\_allocate\(\)

```csharp
[Fact]
public void Clipping_and_drawing_prisms_for_a_full_set_does_not_allocate()
```

### <a id="Cube_Tests_ClippedPrismTests_Draw_Cube_Core_CubeSpace__Cube_Core_ViewProjection__System_ReadOnlySpan_Cube_Core_CollisionBox__"></a> Draw\(in CubeSpace, in ViewProjection, ReadOnlySpan<CollisionBox\>\)

```csharp
private static DrawList Draw(in CubeSpace space, in ViewProjection projection, ReadOnlySpan<CollisionBox> boxes)
```

#### Parameters

`space` [CubeSpace](Cube.Core.CubeSpace.md)

`projection` [ViewProjection](Cube.Core.ViewProjection.md)

`boxes` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[CollisionBox](Cube.Core.CollisionBox.md)\>

#### Returns

 [DrawList](Cube.Core.DrawList.md)

### <a id="Cube_Tests_ClippedPrismTests_Every_drawn_collision_corner_is_inside_the_cube_outline_System_Single_System_Single_System_Single_"></a> Every\_drawn\_collision\_corner\_is\_inside\_the\_cube\_outline\(float, float, float\)

```csharp
[Theory]
[MemberData("YawsAndViews", new object[] { })]
public void Every_drawn_collision_corner_is_inside_the_cube_outline(float yawDegrees, float viewYaw, float viewPitch)
```

#### Parameters

`yawDegrees` [float](https://learn.microsoft.com/dotnet/api/system.single)

`viewYaw` [float](https://learn.microsoft.com/dotnet/api/system.single)

`viewPitch` [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_ClippedPrismTests_Floor_bigger_than_the_cube_turned_45_degrees_is_cut_to_the_turned_square"></a> Floor\_bigger\_than\_the\_cube\_turned\_45\_degrees\_is\_cut\_to\_the\_turned\_square\(\)

```csharp
[Fact]
public void Floor_bigger_than_the_cube_turned_45_degrees_is_cut_to_the_turned_square()
```

### <a id="Cube_Tests_ClippedPrismTests_InsideCube_Cube_Core_CubeSpace__System_Numerics_Vector3_"></a> InsideCube\(in CubeSpace, Vector3\)

```csharp
private static bool InsideCube(in CubeSpace cube, Vector3 world)
```

#### Parameters

`cube` [CubeSpace](Cube.Core.CubeSpace.md)

`world` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Tests_ClippedPrismTests_Prism_shows_its_top_and_the_sides_facing_the_viewer_only"></a> Prism\_shows\_its\_top\_and\_the\_sides\_facing\_the\_viewer\_only\(\)

```csharp
[Fact]
public void Prism_shows_its_top_and_the_sides_facing_the_viewer_only()
```

### <a id="Cube_Tests_ClippedPrismTests_Radians_System_Single_"></a> Radians\(float\)

```csharp
private static float Radians(float degrees)
```

#### Parameters

`degrees` [float](https://learn.microsoft.com/dotnet/api/system.single)

#### Returns

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_ClippedPrismTests_Square_cut_at_all_four_corners_by_a_turned_cube_has_eight_corners_counter_clockwise"></a> Square\_cut\_at\_all\_four\_corners\_by\_a\_turned\_cube\_has\_eight\_corners\_counter\_clockwise\(\)

```csharp
[Fact]
public void Square_cut_at_all_four_corners_by_a_turned_cube_has_eight_corners_counter_clockwise()
```

### <a id="Cube_Tests_ClippedPrismTests_Turned_floor_seen_from_above_draws_one_top_polygon_and_no_sides"></a> Turned\_floor\_seen\_from\_above\_draws\_one\_top\_polygon\_and\_no\_sides\(\)

```csharp
[Fact]
public void Turned_floor_seen_from_above_draws_one_top_polygon_and_no_sides()
```

### <a id="Cube_Tests_ClippedPrismTests_Vertices_Cube_Core_DrawList_Cube_Core_DrawPrimitive_"></a> Vertices\(DrawList, DrawPrimitive\)

```csharp
private static IEnumerable<Vector2> Vertices(DrawList list, DrawPrimitive p)
```

#### Parameters

`list` [DrawList](Cube.Core.DrawList.md)

`p` [DrawPrimitive](Cube.Core.DrawPrimitive.md)

#### Returns

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)\>

### <a id="Cube_Tests_ClippedPrismTests_YawsAndViews"></a> YawsAndViews\(\)

```csharp
public static TheoryData<float, float, float> YawsAndViews()
```

#### Returns

 TheoryData<[float](https://learn.microsoft.com/dotnet/api/system.single), [float](https://learn.microsoft.com/dotnet/api/system.single), [float](https://learn.microsoft.com/dotnet/api/system.single)\>
