# <a id="Cube_Tests_CubeAnchorTests"></a> Class CubeAnchorTests

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Checks "Jump" (PROJECT.md, "Exact behaviour rules", Cube; #105): while the player jumps the cube's height is held at
the take-off height and x and z still follow; landing snaps back to the player straight away; anything without the
jump flag (walking off a ledge, falling) follows as before; the player's own marker is always drawn, kept at the face
it would cross; and the zero-allocation rule ("Efficiency and memory").

```csharp
public class CubeAnchorTests
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[CubeAnchorTests](Cube.Tests.CubeAnchorTests.md)


## Fields

### <a id="Cube_Tests_CubeAnchorTests_Ground"></a> Ground

```csharp
private static readonly Vector3 Ground
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Tests_CubeAnchorTests_Name"></a> Name

```csharp
private static readonly byte[] Name
```

#### Field Value

 [byte](https://learn.microsoft.com/dotnet/api/system.byte)\[\]

## Methods

### <a id="Cube_Tests_CubeAnchorTests_A_second_jump_holds_its_own_take_off_height"></a> A\_second\_jump\_holds\_its\_own\_take\_off\_height\(\)

```csharp
[Fact]
public void A_second_jump_holds_its_own_take_off_height()
```

### <a id="Cube_Tests_CubeAnchorTests_Begin_Cube_Core_CubeSpace_"></a> Begin\(CubeSpace\)

```csharp
private static PlayerSet Begin(CubeSpace space)
```

#### Parameters

`space` [CubeSpace](Cube.Core.CubeSpace.md)

#### Returns

 [PlayerSet](Cube.Core.PlayerSet.md)

### <a id="Cube_Tests_CubeAnchorTests_During_a_jump_the_height_is_held_at_take_off_and_x_and_z_follow"></a> During\_a\_jump\_the\_height\_is\_held\_at\_take\_off\_and\_x\_and\_z\_follow\(\)

```csharp
[Fact]
public void During_a_jump_the_height_is_held_at_take_off_and_x_and_z_follow()
```

### <a id="Cube_Tests_CubeAnchorTests_Landing_snaps_to_the_players_height_in_the_frame_the_flag_clears_System_Single_"></a> Landing\_snaps\_to\_the\_players\_height\_in\_the\_frame\_the\_flag\_clears\(float\)

```csharp
[Theory]
[InlineData(new object[] { 5 })]
[InlineData(new object[] { 9 })]
[InlineData(new object[] { -3 })]
public void Landing_snaps_to_the_players_height_in_the_frame_the_flag_clears(float landingY)
```

#### Parameters

`landingY` [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_CubeAnchorTests_Other_players_outside_the_box_are_still_culled_while_held"></a> Other\_players\_outside\_the\_box\_are\_still\_culled\_while\_held\(\)

```csharp
[Fact]
public void Other_players_outside_the_box_are_still_culled_while_held()
```

### <a id="Cube_Tests_CubeAnchorTests_Reset_drops_the_hold_and_the_remembered_height"></a> Reset\_drops\_the\_hold\_and\_the\_remembered\_height\(\)

```csharp
[Fact]
public void Reset_drops_the_hold_and_the_remembered_height()
```

### <a id="Cube_Tests_CubeAnchorTests_Take_off_height_is_the_previous_frames_height_if_the_flag_arrives_a_frame_late"></a> Take\_off\_height\_is\_the\_previous\_frames\_height\_if\_the\_flag\_arrives\_a\_frame\_late\(\)

```csharp
[Fact]
public void Take_off_height_is_the_previous_frames_height_if_the_flag_arrives_a_frame_late()
```

### <a id="Cube_Tests_CubeAnchorTests_The_held_height_gets_the_player_height_offset_like_any_player_position"></a> The\_held\_height\_gets\_the\_player\_height\_offset\_like\_any\_player\_position\(\)

```csharp
[Fact]
public void The_held_height_gets_the_player_height_offset_like_any_player_position()
```

### <a id="Cube_Tests_CubeAnchorTests_The_local_marker_inside_the_box_is_left_where_it_is"></a> The\_local\_marker\_inside\_the\_box\_is\_left\_where\_it\_is\(\)

```csharp
[Fact]
public void The_local_marker_inside_the_box_is_left_where_it_is()
```

### <a id="Cube_Tests_CubeAnchorTests_The_local_marker_is_kept_at_the_face_it_would_cross_System_Single_"></a> The\_local\_marker\_is\_kept\_at\_the\_face\_it\_would\_cross\(float\)

```csharp
[Theory]
[InlineData(new object[] { 0 })]
[InlineData(new object[] { 0.5 })]
[InlineData(new object[] { 1 })]
public void The_local_marker_is_kept_at_the_face_it_would_cross(float playerHeight)
```

#### Parameters

`playerHeight` [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_CubeAnchorTests_Update_and_Reset_allocate_nothing"></a> Update\_and\_Reset\_allocate\_nothing\(\)

```csharp
[Fact]
public void Update_and_Reset_allocate_nothing()
```

### <a id="Cube_Tests_CubeAnchorTests_Walking_off_a_ledge_without_the_flag_follows_the_fall"></a> Walking\_off\_a\_ledge\_without\_the\_flag\_follows\_the\_fall\(\)

```csharp
[Fact]
public void Walking_off_a_ledge_without_the_flag_follows_the_fall()
```

### <a id="Cube_Tests_CubeAnchorTests_While_held_the_world_stays_put_in_the_cube_and_the_player_moves_up"></a> While\_held\_the\_world\_stays\_put\_in\_the\_cube\_and\_the\_player\_moves\_up\(\)

```csharp
[Fact]
public void While_held_the_world_stays_put_in_the_cube_and_the_player_moves_up()
```

### <a id="Cube_Tests_CubeAnchorTests_Without_a_jump_the_anchor_is_exactly_the_player"></a> Without\_a\_jump\_the\_anchor\_is\_exactly\_the\_player\(\)

```csharp
[Fact]
public void Without_a_jump_the_anchor_is_exactly_the_player()
```
