# <a id="Cube_Tests_NamesAndTogglesTests"></a> Class NamesAndTogglesTests

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Checks #112 (GitHub issue text): the own-name setting (on by default), name position (side by default, or centred
above), a toggle per kind of cube (all on by default), and the text-labelled interactable kind (own colour, own cap,
nameless objects left out).

```csharp
public class NamesAndTogglesTests
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[NamesAndTogglesTests](Cube.Tests.NamesAndTogglesTests.md)


## Fields

### <a id="Cube_Tests_NamesAndTogglesTests_Centre"></a> Centre

```csharp
private static readonly Vector3 Centre
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Tests_NamesAndTogglesTests_Name"></a> Name

```csharp
private static readonly byte[] Name
```

#### Field Value

 [byte](https://learn.microsoft.com/dotnet/api/system.byte)\[\]

### <a id="Cube_Tests_NamesAndTogglesTests_Projection"></a> Projection

```csharp
private static readonly ViewProjection Projection
```

#### Field Value

 [ViewProjection](Cube.Core.ViewProjection.md)

### <a id="Cube_Tests_NamesAndTogglesTests_Space"></a> Space

```csharp
private static readonly CubeSpace Space
```

#### Field Value

 [CubeSpace](Cube.Core.CubeSpace.md)

## Methods

### <a id="Cube_Tests_NamesAndTogglesTests_A_bad_name_position_is_repaired_to_side"></a> A\_bad\_name\_position\_is\_repaired\_to\_side\(\)

```csharp
[Fact]
public void A_bad_name_position_is_repaired_to_side()
```

### <a id="Cube_Tests_NamesAndTogglesTests_Begin_System_Boolean_System_Boolean_System_Boolean_System_Boolean_"></a> Begin\(bool, bool, bool, bool\)

```csharp
private static PlayerSet Begin(bool npcs = true, bool players = true, bool enemies = true, bool interactables = true)
```

#### Parameters

`npcs` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

`players` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

`enemies` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

`interactables` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

#### Returns

 [PlayerSet](Cube.Core.PlayerSet.md)

### <a id="Cube_Tests_NamesAndTogglesTests_Centred_label_is_over_the_cube_and_flagged_for_centring"></a> Centred\_label\_is\_over\_the\_cube\_and\_flagged\_for\_centring\(\)

```csharp
[Fact]
public void Centred_label_is_over_the_cube_and_flagged_for_centring()
```

### <a id="Cube_Tests_NamesAndTogglesTests_CopyFrom_copies_the_new_settings"></a> CopyFrom\_copies\_the\_new\_settings\(\)

```csharp
[Fact]
public void CopyFrom_copies_the_new_settings()
```

### <a id="Cube_Tests_NamesAndTogglesTests_Defaults_show_everything_with_names_at_the_side"></a> Defaults\_show\_everything\_with\_names\_at\_the\_side\(\)

```csharp
[Fact]
public void Defaults_show_everything_with_names_at_the_side()
```

### <a id="Cube_Tests_NamesAndTogglesTests_Enemies_off_drops_enemies_but_keeps_friendly_npcs"></a> Enemies\_off\_drops\_enemies\_but\_keeps\_friendly\_npcs\(\)

```csharp
[Fact]
public void Enemies_off_drops_enemies_but_keeps_friendly_npcs()
```

### <a id="Cube_Tests_NamesAndTogglesTests_Interactable_cap_drops_extras_without_pushing_out_players_or_npcs"></a> Interactable\_cap\_drops\_extras\_without\_pushing\_out\_players\_or\_npcs\(\)

```csharp
[Fact]
public void Interactable_cap_drops_extras_without_pushing_out_players_or_npcs()
```

### <a id="Cube_Tests_NamesAndTogglesTests_Interactable_colour_is_in_the_palette_and_differs_from_the_others"></a> Interactable\_colour\_is\_in\_the\_palette\_and\_differs\_from\_the\_others\(\)

```csharp
[Fact]
public void Interactable_colour_is_in_the_palette_and_differs_from_the_others()
```

### <a id="Cube_Tests_NamesAndTogglesTests_Interactable_cube_is_drawn_in_the_interactable_colour"></a> Interactable\_cube\_is\_drawn\_in\_the\_interactable\_colour\(\)

```csharp
[Fact]
public void Interactable_cube_is_drawn_in_the_interactable_colour()
```

### <a id="Cube_Tests_NamesAndTogglesTests_Interactable_inside_the_cube_is_kept_as_an_interactable"></a> Interactable\_inside\_the\_cube\_is\_kept\_as\_an\_interactable\(\)

```csharp
[Fact]
public void Interactable_inside_the_cube_is_kept_as_an_interactable()
```

### <a id="Cube_Tests_NamesAndTogglesTests_Interactable_outside_the_cube_is_not_kept"></a> Interactable\_outside\_the\_cube\_is\_not\_kept\(\)

```csharp
[Fact]
public void Interactable_outside_the_cube_is_not_kept()
```

### <a id="Cube_Tests_NamesAndTogglesTests_Interactables_off_keeps_none_and_asks_the_reader_to_skip_them"></a> Interactables\_off\_keeps\_none\_and\_asks\_the\_reader\_to\_skip\_them\(\)

```csharp
[Fact]
public void Interactables_off_keeps_none_and_asks_the_reader_to_skip_them()
```

### <a id="Cube_Tests_NamesAndTogglesTests_Labels_Cube_Core_NearbyPlayer___System_Boolean_System_Boolean_Cube_Core_NamePosition_"></a> Labels\(NearbyPlayer\[\], bool, bool, NamePosition\)

```csharp
private static DrawPrimitive[] Labels(NearbyPlayer[] players, bool labels = true, bool selfLabel = false, NamePosition position = NamePosition.Side)
```

#### Parameters

`players` [NearbyPlayer](Cube.Core.NearbyPlayer.md)\[\]

`labels` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

`selfLabel` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

`position` [NamePosition](Cube.Core.NamePosition.md)

#### Returns

 [DrawPrimitive](Cube.Core.DrawPrimitive.md)\[\]

### <a id="Cube_Tests_NamesAndTogglesTests_Npcs_and_enemies_both_off_makes_the_reader_skip_npcs"></a> Npcs\_and\_enemies\_both\_off\_makes\_the\_reader\_skip\_npcs\(\)

```csharp
[Fact]
public void Npcs_and_enemies_both_off_makes_the_reader_skip_npcs()
```

### <a id="Cube_Tests_NamesAndTogglesTests_Npcs_off_drops_friendly_npcs_but_keeps_enemies"></a> Npcs\_off\_drops\_friendly\_npcs\_but\_keeps\_enemies\(\)

```csharp
[Fact]
public void Npcs_off_drops_friendly_npcs_but_keeps_enemies()
```

### <a id="Cube_Tests_NamesAndTogglesTests_Old_files_load_the_new_settings_at_their_defaults"></a> Old\_files\_load\_the\_new\_settings\_at\_their\_defaults\(\)

```csharp
[Fact]
public void Old_files_load_the_new_settings_at_their_defaults()
```

### <a id="Cube_Tests_NamesAndTogglesTests_OnlyNamedObjectsAreTextInteractables_System_String_System_Boolean_"></a> OnlyNamedObjectsAreTextInteractables\(string, bool\)

```csharp
[Theory]
[InlineData(new object[] { "Entrance", true })]
[InlineData(new object[] { "", false })]
public void OnlyNamedObjectsAreTextInteractables(string name, bool expected)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)

`expected` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Tests_NamesAndTogglesTests_Own_name_is_labelled_only_when_asked"></a> Own\_name\_is\_labelled\_only\_when\_asked\(\)

```csharp
[Fact]
public void Own_name_is_labelled_only_when_asked()
```

### <a id="Cube_Tests_NamesAndTogglesTests_Own_name_setting_is_independent_of_the_names_setting"></a> Own\_name\_setting\_is\_independent\_of\_the\_names\_setting\(\)

```csharp
[Fact]
public void Own_name_setting_is_independent_of_the_names_setting()
```

### <a id="Cube_Tests_NamesAndTogglesTests_Players_off_drops_other_players_but_never_the_local_player"></a> Players\_off\_drops\_other\_players\_but\_never\_the\_local\_player\(\)

```csharp
[Fact]
public void Players_off_drops_other_players_but_never_the_local_player()
```
