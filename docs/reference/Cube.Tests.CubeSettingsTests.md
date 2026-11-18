# <a id="Cube_Tests_CubeSettingsTests"></a> Class CubeSettingsTests

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Checks the settings' defaults and repair rules (PROJECT.md, "Features": settings for cube size, orientation mode,
labels on/off, colours and opacity; "NPCs": on by default, purple; "Cube": edge 10–100, default 40; "View": Camera
mode by default; saved files from older versions still load) and the
<code>/cube config</code> command.

```csharp
public class CubeSettingsTests
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[CubeSettingsTests](Cube.Tests.CubeSettingsTests.md)


## Fields

### <a id="Cube_Tests_CubeSettingsTests_JsonOptions"></a> JsonOptions

Serialises public fields, as Dalamud's configuration serialiser does, so these tests see the same saved shape.

```csharp
private static readonly JsonSerializerOptions JsonOptions
```

#### Field Value

 [JsonSerializerOptions](https://learn.microsoft.com/dotnet/api/system.text.json.jsonserializeroptions)

## Methods

### <a id="Cube_Tests_CubeSettingsTests_Chosen_colour_is_used_for_its_relation"></a> Chosen\_colour\_is\_used\_for\_its\_relation\(\)

```csharp
[Fact]
public void Chosen_colour_is_used_for_its_relation()
```

### <a id="Cube_Tests_CubeSettingsTests_Config_saved_before_npc_settings_existed_loads_with_npcs_on_in_purple"></a> Config\_saved\_before\_npc\_settings\_existed\_loads\_with\_npcs\_on\_in\_purple\(\)

```csharp
[Fact]
public void Config_saved_before_npc_settings_existed_loads_with_npcs_on_in_purple()
```

### <a id="Cube_Tests_CubeSettingsTests_Copying_settings_copies_every_value_and_counts_as_a_change"></a> Copying\_settings\_copies\_every\_value\_and\_counts\_as\_a\_change\(\)

```csharp
[Fact]
public void Copying_settings_copies_every_value_and_counts_as_a_change()
```

### <a id="Cube_Tests_CubeSettingsTests_Corrupt_colour_falls_back_to_the_default_and_out_of_range_is_clamped"></a> Corrupt\_colour\_falls\_back\_to\_the\_default\_and\_out\_of\_range\_is\_clamped\(\)

```csharp
[Fact]
public void Corrupt_colour_falls_back_to_the_default_and_out_of_range_is_clamped()
```

### <a id="Cube_Tests_CubeSettingsTests_Corrupt_npc_colour_falls_back_to_purple"></a> Corrupt\_npc\_colour\_falls\_back\_to\_purple\(\)

```csharp
[Fact]
public void Corrupt_npc_colour_falls_back_to_purple()
```

### <a id="Cube_Tests_CubeSettingsTests_Cube_config_opens_settings_and_anything_else_toggles_the_cube_System_String_System_Boolean_"></a> Cube\_config\_opens\_settings\_and\_anything\_else\_toggles\_the\_cube\(string?, bool\)

```csharp
[Theory]
[InlineData(new object[] { "config", true })]
[InlineData(new object[] { "  Config ", true })]
[InlineData(new object[] { "", false })]
[InlineData(new object[] { null, false })]
[InlineData(new object[] { "configure", false })]
public void Cube_config_opens_settings_and_anything_else_toggles_the_cube(string? args, bool isConfig)
```

#### Parameters

`args` [string](https://learn.microsoft.com/dotnet/api/system.string)?

`isConfig` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Tests_CubeSettingsTests_Cube_size_is_kept_within_10_to_100_System_Single_System_Single_"></a> Cube\_size\_is\_kept\_within\_10\_to\_100\(float, float\)

```csharp
[Theory]
[InlineData(new object[] { 5, 10 })]
[InlineData(new object[] { 500, 100 })]
[InlineData(new object[] { NaN, 40 })]
[InlineData(new object[] { 65, 65 })]
public void Cube_size_is_kept_within_10_to_100(float stored, float expected)
```

#### Parameters

`stored` [float](https://learn.microsoft.com/dotnet/api/system.single)

`expected` [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_CubeSettingsTests_Default_colours_are_four_different_colours"></a> Default\_colours\_are\_four\_different\_colours\(\)

```csharp
[Fact]
public void Default_colours_are_four_different_colours()
```

### <a id="Cube_Tests_CubeSettingsTests_Defaults_match_the_spec"></a> Defaults\_match\_the\_spec\(\)

```csharp
[Fact]
public void Defaults_match_the_spec()
```

### <a id="Cube_Tests_CubeSettingsTests_Every_change_bumps_the_revision"></a> Every\_change\_bumps\_the\_revision\(\)

```csharp
[Fact]
public void Every_change_bumps_the_revision()
```

### <a id="Cube_Tests_CubeSettingsTests_Npc_settings_survive_a_save_and_load"></a> Npc\_settings\_survive\_a\_save\_and\_load\(\)

```csharp
[Fact]
public void Npc_settings_survive_a_save_and_load()
```

### <a id="Cube_Tests_CubeSettingsTests_Opacity_applies_to_every_player_colour"></a> Opacity\_applies\_to\_every\_player\_colour\(\)

```csharp
[Fact]
public void Opacity_applies_to_every_player_colour()
```

### <a id="Cube_Tests_CubeSettingsTests_Opacity_is_kept_visible_and_at_most_full_System_Single_System_Single_"></a> Opacity\_is\_kept\_visible\_and\_at\_most\_full\(float, float\)

```csharp
[Theory]
[InlineData(new object[] { 0, 0.1 })]
[InlineData(new object[] { 2, 1 })]
[InlineData(new object[] { NaN, 1 })]
public void Opacity_is_kept_visible_and_at_most_full(float stored, float expected)
```

#### Parameters

`stored` [float](https://learn.microsoft.com/dotnet/api/system.single)

`expected` [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_CubeSettingsTests_Unknown_orientation_falls_back_to_camera"></a> Unknown\_orientation\_falls\_back\_to\_camera\(\)

```csharp
[Fact]
public void Unknown_orientation_falls_back_to_camera()
```
