# <a id="Cube_Tests_PlayerHeightTests"></a> Class PlayerHeightTests

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Checks "Player height in cube" (PROJECT.md, "Exact behaviour rules", Cube; #91): a fraction p of the box's height,
0–1, default 0.5; the box spans <code>player.y - height × p</code> to <code>player.y + height × (1 - p)</code> with x and z
centred on the player; culling uses the shifted box; on screen the box stays centred, so the player's marker moves
off the middle; the setting is saved, older configs load as 50%, and reset to defaults restores it. Also the
zero-allocation rule ("Efficiency and memory").

```csharp
public class PlayerHeightTests
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[PlayerHeightTests](Cube.Tests.PlayerHeightTests.md)


## Fields

### <a id="Cube_Tests_PlayerHeightTests_JsonOptions"></a> JsonOptions

```csharp
private static readonly JsonSerializerOptions JsonOptions
```

#### Field Value

 [JsonSerializerOptions](https://learn.microsoft.com/dotnet/api/system.text.json.jsonserializeroptions)

### <a id="Cube_Tests_PlayerHeightTests_Player"></a> Player

```csharp
private static readonly Vector3 Player
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

## Methods

### <a id="Cube_Tests_PlayerHeightTests_At_0_percent_things_just_below_are_culled_and_things_near_the_top_kept"></a> At\_0\_percent\_things\_just\_below\_are\_culled\_and\_things\_near\_the\_top\_kept\(\)

```csharp
[Fact]
public void At_0_percent_things_just_below_are_culled_and_things_near_the_top_kept()
```

### <a id="Cube_Tests_PlayerHeightTests_At_100_percent_things_just_above_are_culled_and_things_near_the_bottom_kept"></a> At\_100\_percent\_things\_just\_above\_are\_culled\_and\_things\_near\_the\_bottom\_kept\(\)

```csharp
[Fact]
public void At_100_percent_things_just_above_are_culled_and_things_near_the_bottom_kept()
```

### <a id="Cube_Tests_PlayerHeightTests_At_50_percent_the_box_is_the_centred_cube"></a> At\_50\_percent\_the\_box\_is\_the\_centred\_cube\(\)

```csharp
[Fact]
public void At_50_percent_the_box_is_the_centred_cube()
```

### <a id="Cube_Tests_PlayerHeightTests_Bounds_follow_a_zoomed_height"></a> Bounds\_follow\_a\_zoomed\_height\(\)

```csharp
[Fact]
public void Bounds_follow_a_zoomed_height()
```

### <a id="Cube_Tests_PlayerHeightTests_Bounds_span_height_times_p_below_and_height_times_one_minus_p_above_System_Single_"></a> Bounds\_span\_height\_times\_p\_below\_and\_height\_times\_one\_minus\_p\_above\(float\)

```csharp
[Theory]
[InlineData(new object[] { 0 })]
[InlineData(new object[] { 0.25 })]
[InlineData(new object[] { 0.5 })]
[InlineData(new object[] { 1 })]
public void Bounds_span_height_times_p_below_and_height_times_one_minus_p_above(float p)
```

#### Parameters

`p` [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_PlayerHeightTests_Bounds_use_the_clamped_height"></a> Bounds\_use\_the\_clamped\_height\(\)

```csharp
[Fact]
public void Bounds_use_the_clamped_height()
```

### <a id="Cube_Tests_PlayerHeightTests_Box_System_Single_System_Nullable_System_Numerics_Vector3__"></a> Box\(float, Vector3?\)

```csharp
private static CubeSpace Box(float fraction, Vector3? size = null)
```

#### Parameters

`fraction` [float](https://learn.microsoft.com/dotnet/api/system.single)

`size` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)?

#### Returns

 [CubeSpace](Cube.Core.CubeSpace.md)

### <a id="Cube_Tests_PlayerHeightTests_Building_and_culling_with_a_player_height_do_not_allocate"></a> Building\_and\_culling\_with\_a\_player\_height\_do\_not\_allocate\(\)

```csharp
[Fact]
public void Building_and_culling_with_a_player_height_do_not_allocate()
```

### <a id="Cube_Tests_PlayerHeightTests_Copying_settings_keeps_the_player_height"></a> Copying\_settings\_keeps\_the\_player\_height\(\)

```csharp
[Fact]
public void Copying_settings_keeps_the_player_height()
```

### <a id="Cube_Tests_PlayerHeightTests_Fraction_is_clamped_to_0_to_1_System_Single_System_Single_"></a> Fraction\_is\_clamped\_to\_0\_to\_1\(float, float\)

```csharp
[Theory]
[InlineData(new object[] { -0.5, 0 })]
[InlineData(new object[] { 1.5, 1 })]
[InlineData(new object[] { NaN, 0.5 })]
[InlineData(new object[] { 0.3, 0.3 })]
public void Fraction_is_clamped_to_0_to_1(float requested, float expected)
```

#### Parameters

`requested` [float](https://learn.microsoft.com/dotnet/api/system.single)

`expected` [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_PlayerHeightTests_Older_configs_load_as_50_percent_System_Int32_"></a> Older\_configs\_load\_as\_50\_percent\(int\)

```csharp
[Theory]
[InlineData(new object[] { 1 })]
[InlineData(new object[] { 2 })]
[InlineData(new object[] { 3 })]
public void Older_configs_load_as_50_percent(int fileVersion)
```

#### Parameters

`fileVersion` [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Tests_PlayerHeightTests_Player_marker_moves_off_the_middle_on_screen"></a> Player\_marker\_moves\_off\_the\_middle\_on\_screen\(\)

```csharp
[Fact]
public void Player_marker_moves_off_the_middle_on_screen()
```

### <a id="Cube_Tests_PlayerHeightTests_Recentred_box_matches_a_fresh_one"></a> Recentred\_box\_matches\_a\_fresh\_one\(\)

```csharp
[Fact]
public void Recentred_box_matches_a_fresh_one()
```

### <a id="Cube_Tests_PlayerHeightTests_Reset_to_defaults_restores_50_percent"></a> Reset\_to\_defaults\_restores\_50\_percent\(\)

```csharp
[Fact]
public void Reset_to_defaults_restores_50_percent()
```

### <a id="Cube_Tests_PlayerHeightTests_Setting_defaults_to_50_percent_and_is_repaired"></a> Setting\_defaults\_to\_50\_percent\_and\_is\_repaired\(\)

```csharp
[Fact]
public void Setting_defaults_to_50_percent_and_is_repaired()
```

### <a id="Cube_Tests_PlayerHeightTests_Setting_is_saved_and_loaded"></a> Setting\_is\_saved\_and\_loaded\(\)

```csharp
[Fact]
public void Setting_is_saved_and_loaded()
```

### <a id="Cube_Tests_PlayerHeightTests_Shift_is_vertical_only"></a> Shift\_is\_vertical\_only\(\)

```csharp
[Fact]
public void Shift_is_vertical_only()
```
