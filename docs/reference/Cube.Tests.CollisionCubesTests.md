# <a id="Cube_Tests_CollisionCubesTests"></a> Class CollisionCubesTests

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Checks collision boxes are drawn as solid boxes sorted with the players (PROJECT.md, "Collision (phase 2)": each
collider inside or crossing the cube is shown as a simplified box; "Draw order": far things are drawn first, so
near ones cover far ones; "Cheap drawing": only the visible faces; "Efficiency and memory": no per-frame
allocations).

```csharp
public class CollisionCubesTests
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[CollisionCubesTests](Cube.Tests.CollisionCubesTests.md)


## Fields

### <a id="Cube_Tests_CollisionCubesTests_Grey"></a> Grey

```csharp
private static readonly uint Grey
```

#### Field Value

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

## Methods

### <a id="Cube_Tests_CollisionCubesTests_Box_in_the_cube_gets_its_visible_faces_in_the_collision_colour_with_its_own_tag"></a> Box\_in\_the\_cube\_gets\_its\_visible\_faces\_in\_the\_collision\_colour\_with\_its\_own\_tag\(\)

```csharp
[Fact]
public void Box_in_the_cube_gets_its_visible_faces_in_the_collision_colour_with_its_own_tag()
```

### <a id="Cube_Tests_CollisionCubesTests_Collision_tags_are_negative_and_lead_back_to_their_box"></a> Collision\_tags\_are\_negative\_and\_lead\_back\_to\_their\_box\(\)

```csharp
[Fact]
public void Collision_tags_are_negative_and_lead_back_to_their_box()
```

### <a id="Cube_Tests_CollisionCubesTests_Cube_shaped_box_is_drawn_exactly_like_a_player_cube_System_Single_"></a> Cube\_shaped\_box\_is\_drawn\_exactly\_like\_a\_player\_cube\(float\)

```csharp
[Theory]
[InlineData(new object[] { 0 })]
[InlineData(new object[] { 0.4 })]
[InlineData(new object[] { 2.5 })]
[InlineData(new object[] { -1.9 })]
public void Cube_shaped_box_is_drawn_exactly_like_a_player_cube(float yaw)
```

#### Parameters

`yaw` [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_CollisionCubesTests_Drawing_a_full_set_of_boxes_does_not_allocate"></a> Drawing\_a\_full\_set\_of\_boxes\_does\_not\_allocate\(\)

```csharp
[Fact]
public void Drawing_a_full_set_of_boxes_does_not_allocate()
```

### <a id="Cube_Tests_CollisionCubesTests_Far_depth_is_the_depth_of_the_farthest_corner_System_Single_"></a> Far\_depth\_is\_the\_depth\_of\_the\_farthest\_corner\(float\)

```csharp
[Theory]
[InlineData(new object[] { 0 })]
[InlineData(new object[] { 0.4 })]
[InlineData(new object[] { 2.5 })]
public void Far_depth_is_the_depth_of_the_farthest_corner(float yaw)
```

#### Parameters

`yaw` [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_CollisionCubesTests_Flat_floor_seen_from_above_shows_its_top_face_only"></a> Flat\_floor\_seen\_from\_above\_shows\_its\_top\_face\_only\(\)

```csharp
[Fact]
public void Flat_floor_seen_from_above_shows_its_top_face_only()
```

### <a id="Cube_Tests_CollisionCubesTests_Floor_reaching_towards_the_viewer_is_still_painted_under_a_player_standing_on_it"></a> Floor\_reaching\_towards\_the\_viewer\_is\_still\_painted\_under\_a\_player\_standing\_on\_it\(\)

```csharp
[Fact]
public void Floor_reaching_towards_the_viewer_is_still_painted_under_a_player_standing_on_it()
```

### <a id="Cube_Tests_CollisionCubesTests_Floor_under_the_player_is_painted_before_the_player"></a> Floor\_under\_the\_player\_is\_painted\_before\_the\_player\(\)

```csharp
[Fact]
public void Floor_under_the_player_is_painted_before_the_player()
```

### <a id="Cube_Tests_CollisionCubesTests_Frame_Cube_Core_CubeSpace__Cube_Core_CollisionBox___Cube_Core_NearbyPlayer___"></a> Frame\(in CubeSpace, CollisionBox\[\], NearbyPlayer\[\]\)

```csharp
private static DrawPrimitive[] Frame(in CubeSpace space, CollisionBox[] boxes, NearbyPlayer[] players)
```

#### Parameters

`space` [CubeSpace](Cube.Core.CubeSpace.md)

`boxes` [CollisionBox](Cube.Core.CollisionBox.md)\[\]

`players` [NearbyPlayer](Cube.Core.NearbyPlayer.md)\[\]

#### Returns

 [DrawPrimitive](Cube.Core.DrawPrimitive.md)\[\]

### <a id="Cube_Tests_CollisionCubesTests_Line_shaped_box_draws_nothing"></a> Line\_shaped\_box\_draws\_nothing\(\)

```csharp
[Fact]
public void Line_shaped_box_draws_nothing()
```

### <a id="Cube_Tests_CollisionCubesTests_Player_in_front_of_a_wall_is_painted_over_it"></a> Player\_in\_front\_of\_a\_wall\_is\_painted\_over\_it\(\)

```csharp
[Fact]
public void Player_in_front_of_a_wall_is_painted_over_it()
```

### <a id="Cube_Tests_CollisionCubesTests_Projection_Cube_Core_CubeSpace__"></a> Projection\(in CubeSpace\)

```csharp
private static ViewProjection Projection(in CubeSpace space)
```

#### Parameters

`space` [CubeSpace](Cube.Core.CubeSpace.md)

#### Returns

 [ViewProjection](Cube.Core.ViewProjection.md)

### <a id="Cube_Tests_CollisionCubesTests_Self"></a> Self\(\)

```csharp
private static NearbyPlayer Self()
```

#### Returns

 [NearbyPlayer](Cube.Core.NearbyPlayer.md)

### <a id="Cube_Tests_CollisionCubesTests_Small_box_on_a_big_one_is_painted_after_it"></a> Small\_box\_on\_a\_big\_one\_is\_painted\_after\_it\(\)

```csharp
[Fact]
public void Small_box_on_a_big_one_is_painted_after_it()
```

### <a id="Cube_Tests_CollisionCubesTests_Space_System_Single_"></a> Space\(float\)

```csharp
private static CubeSpace Space(float yaw = 0)
```

#### Parameters

`yaw` [float](https://learn.microsoft.com/dotnet/api/system.single)

#### Returns

 [CubeSpace](Cube.Core.CubeSpace.md)
