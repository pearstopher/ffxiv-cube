# <a id="Cube_Tests_PlayerLabelsTests"></a> Class PlayerLabelsTests

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Checks name labels (PROJECT.md, "Features": other players are labelled with their names, labels on/off;
"Labels": the name drawn next to the cube, overlaps are fine; "Draw order": near cubes cover far ones;
"Efficiency and memory": labels cached, no string building per frame).

```csharp
public class PlayerLabelsTests
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[PlayerLabelsTests](Cube.Tests.PlayerLabelsTests.md)


## Fields

### <a id="Cube_Tests_PlayerLabelsTests_Elevation"></a> Elevation

```csharp
private static readonly float Elevation
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_PlayerLabelsTests_Projection"></a> Projection

```csharp
private static readonly ViewProjection Projection
```

#### Field Value

 [ViewProjection](Cube.Core.ViewProjection.md)

### <a id="Cube_Tests_PlayerLabelsTests_Space"></a> Space

```csharp
private static readonly CubeSpace Space
```

#### Field Value

 [CubeSpace](Cube.Core.CubeSpace.md)

## Methods

### <a id="Cube_Tests_PlayerLabelsTests_Build_Cube_Core_NearbyPlayer___System_Boolean_"></a> Build\(NearbyPlayer\[\], bool\)

```csharp
private static DrawPrimitive[] Build(NearbyPlayer[] players, bool labels = true)
```

#### Parameters

`players` [NearbyPlayer](Cube.Core.NearbyPlayer.md)\[\]

`labels` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

#### Returns

 [DrawPrimitive](Cube.Core.DrawPrimitive.md)\[\]

### <a id="Cube_Tests_PlayerLabelsTests_Building_labels_does_not_allocate"></a> Building\_labels\_does\_not\_allocate\(\)

```csharp
[Fact]
public void Building_labels_does_not_allocate()
```

### <a id="Cube_Tests_PlayerLabelsTests_Each_other_player_gets_one_label_tagged_with_them"></a> Each\_other\_player\_gets\_one\_label\_tagged\_with\_them\(\)

```csharp
[Fact]
public void Each_other_player_gets_one_label_tagged_with_them()
```

### <a id="Cube_Tests_PlayerLabelsTests_Label_is_painted_after_its_own_cube"></a> Label\_is\_painted\_after\_its\_own\_cube\(\)

```csharp
[Fact]
public void Label_is_painted_after_its_own_cube()
```

### <a id="Cube_Tests_PlayerLabelsTests_Label_sits_to_the_right_of_its_cube_at_any_yaw_System_Single_"></a> Label\_sits\_to\_the\_right\_of\_its\_cube\_at\_any\_yaw\(float\)

```csharp
[Theory]
[InlineData(new object[] { 0 })]
[InlineData(new object[] { 0.785 })]
[InlineData(new object[] { 2 })]
public void Label_sits_to_the_right_of_its_cube_at_any_yaw(float yaw)
```

#### Parameters

`yaw` [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_PlayerLabelsTests_Labels_off_draws_no_labels_but_keeps_the_cubes"></a> Labels\_off\_draws\_no\_labels\_but\_keeps\_the\_cubes\(\)

```csharp
[Fact]
public void Labels_off_draws_no_labels_but_keeps_the_cubes()
```

### <a id="Cube_Tests_PlayerLabelsTests_Local_player_is_not_labelled"></a> Local\_player\_is\_not\_labelled\(\)

```csharp
[Fact]
public void Local_player_is_not_labelled()
```

### <a id="Cube_Tests_PlayerLabelsTests_Nearer_cube_is_painted_over_a_farther_players_label"></a> Nearer\_cube\_is\_painted\_over\_a\_farther\_players\_label\(\)

```csharp
[Fact]
public void Nearer_cube_is_painted_over_a_farther_players_label()
```

### <a id="Cube_Tests_PlayerLabelsTests_Overlapping_players_each_keep_their_label"></a> Overlapping\_players\_each\_keep\_their\_label\(\)

```csharp
[Fact]
public void Overlapping_players_each_keep_their_label()
```

### <a id="Cube_Tests_PlayerLabelsTests_Player_System_Numerics_Vector3_Cube_Core_PlayerRelation_System_String_"></a> Player\(Vector3, PlayerRelation, string\)

```csharp
private static NearbyPlayer Player(Vector3 position, PlayerRelation relation, string name = "Someone")
```

#### Parameters

`position` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

`relation` [PlayerRelation](Cube.Core.PlayerRelation.md)

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)

#### Returns

 [NearbyPlayer](Cube.Core.NearbyPlayer.md)
