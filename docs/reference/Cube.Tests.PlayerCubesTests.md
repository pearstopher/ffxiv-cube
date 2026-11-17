# <a id="Cube_Tests_PlayerCubesTests"></a> Class PlayerCubesTests

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Checks players are drawn as small coloured cubes (PROJECT.md, "Players": about 1/40 of the big cube's edge, minimum
a few pixels; "Colours": one colour each for self, party/alliance and others; "Draw order": near cubes cover far
ones; "Efficiency and memory": no per-frame allocations).

```csharp
public class PlayerCubesTests
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[PlayerCubesTests](Cube.Tests.PlayerCubesTests.md)


## Fields

### <a id="Cube_Tests_PlayerCubesTests_Elevation"></a> Elevation

```csharp
private static readonly float Elevation
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

## Methods

### <a id="Cube_Tests_PlayerCubesTests_Drawing_players_does_not_allocate"></a> Drawing\_players\_does\_not\_allocate\(\)

```csharp
[Fact]
public void Drawing_players_does_not_allocate()
```

### <a id="Cube_Tests_PlayerCubesTests_Each_player_gets_faces_in_their_colour_tagged_with_their_index"></a> Each\_player\_gets\_faces\_in\_their\_colour\_tagged\_with\_their\_index\(\)

```csharp
[Fact]
public void Each_player_gets_faces_in_their_colour_tagged_with_their_index()
```

### <a id="Cube_Tests_PlayerCubesTests_Each_relation_has_its_own_colour_Cube_Core_PlayerRelation_"></a> Each\_relation\_has\_its\_own\_colour\(PlayerRelation\)

```csharp
[Theory]
[InlineData(new object[] { PlayerRelation.Self })]
[InlineData(new object[] { PlayerRelation.Party })]
[InlineData(new object[] { PlayerRelation.Other })]
public void Each_relation_has_its_own_colour(PlayerRelation relation)
```

#### Parameters

`relation` [PlayerRelation](Cube.Core.PlayerRelation.md)

### <a id="Cube_Tests_PlayerCubesTests_Local_player_cube_is_at_the_centre_of_the_window"></a> Local\_player\_cube\_is\_at\_the\_centre\_of\_the\_window\(\)

```csharp
[Fact]
public void Local_player_cube_is_at_the_centre_of_the_window()
```

### <a id="Cube_Tests_PlayerCubesTests_Nearer_player_is_drawn_after_farther_player"></a> Nearer\_player\_is\_drawn\_after\_farther\_player\(\)

```csharp
[Fact]
public void Nearer_player_is_drawn_after_farther_player()
```

### <a id="Cube_Tests_PlayerCubesTests_Player_System_Numerics_Vector3_Cube_Core_PlayerRelation_"></a> Player\(Vector3, PlayerRelation\)

```csharp
private static NearbyPlayer Player(Vector3 position, PlayerRelation relation)
```

#### Parameters

`position` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

`relation` [PlayerRelation](Cube.Core.PlayerRelation.md)

#### Returns

 [NearbyPlayer](Cube.Core.NearbyPlayer.md)

### <a id="Cube_Tests_PlayerCubesTests_Player_cube_is_one_fortieth_of_the_big_cube"></a> Player\_cube\_is\_one\_fortieth\_of\_the\_big\_cube\(\)

```csharp
[Fact]
public void Player_cube_is_one_fortieth_of_the_big_cube()
```

### <a id="Cube_Tests_PlayerCubesTests_Player_cube_never_gets_smaller_than_the_minimum_on_screen"></a> Player\_cube\_never\_gets\_smaller\_than\_the\_minimum\_on\_screen\(\)

```csharp
[Fact]
public void Player_cube_never_gets_smaller_than_the_minimum_on_screen()
```

### <a id="Cube_Tests_PlayerCubesTests_Player_cube_scales_with_the_big_cube"></a> Player\_cube\_scales\_with\_the\_big\_cube\(\)

```csharp
[Fact]
public void Player_cube_scales_with_the_big_cube()
```

### <a id="Cube_Tests_PlayerCubesTests_Space_System_Single_"></a> Space\(float\)

```csharp
private static CubeSpace Space(float edge = 40)
```

#### Parameters

`edge` [float](https://learn.microsoft.com/dotnet/api/system.single)

#### Returns

 [CubeSpace](Cube.Core.CubeSpace.md)
