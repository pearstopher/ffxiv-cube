# <a id="Cube_Tests_ViewSettingsTests"></a> Class ViewSettingsTests

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Checks the saved view orientation and the view control's placement (PROJECT.md, "View orientation control": a
small control in a corner of the window, bottom right by default; the chosen orientation is saved; there's a way
to reset to the default).

```csharp
public class ViewSettingsTests
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ViewSettingsTests](Cube.Tests.ViewSettingsTests.md)


## Methods

### <a id="Cube_Tests_ViewSettingsTests_Bad_saved_view_values_are_repaired_System_Single_System_Single_System_Single_System_Single_"></a> Bad\_saved\_view\_values\_are\_repaired\(float, float, float, float\)

```csharp
[Theory]
[InlineData(new object[] { NaN, 10, 0, 30 })]
[InlineData(new object[] { 20, Infinity, 0, 30 })]
[InlineData(new object[] { 400, 120, 40, 90 })]
[InlineData(new object[] { -30, -95, -30, -90 })]
public void Bad_saved_view_values_are_repaired(float yaw, float pitch, float expectedYaw, float expectedPitch)
```

#### Parameters

`yaw` [float](https://learn.microsoft.com/dotnet/api/system.single)

`pitch` [float](https://learn.microsoft.com/dotnet/api/system.single)

`expectedYaw` [float](https://learn.microsoft.com/dotnet/api/system.single)

`expectedPitch` [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_ViewSettingsTests_Control_sits_in_the_chosen_corner_Cube_Core_WidgetCorner_System_Single_System_Single_"></a> Control\_sits\_in\_the\_chosen\_corner\(WidgetCorner, float, float\)

```csharp
[Theory]
[InlineData(new object[] { WidgetCorner.BottomRight, 310, 220 })]
[InlineData(new object[] { WidgetCorner.BottomLeft, 10, 220 })]
[InlineData(new object[] { WidgetCorner.TopRight, 310, 20 })]
[InlineData(new object[] { WidgetCorner.TopLeft, 10, 20 })]
public void Control_sits_in_the_chosen_corner(WidgetCorner corner, float x, float y)
```

#### Parameters

`corner` [WidgetCorner](Cube.Core.WidgetCorner.md)

`x` [float](https://learn.microsoft.com/dotnet/api/system.single)

`y` [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_ViewSettingsTests_Control_size_follows_the_window_within_limits_System_Single_System_Single_"></a> Control\_size\_follows\_the\_window\_within\_limits\(float, float\)

```csharp
[Theory]
[InlineData(new object[] { 100, 48 })]
[InlineData(new object[] { 5000, 120 })]
[InlineData(new object[] { 400, 72 })]
public void Control_size_follows_the_window_within_limits(float side, float expected)
```

#### Parameters

`side` [float](https://learn.microsoft.com/dotnet/api/system.single)

`expected` [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_ViewSettingsTests_Control_stays_outside_the_cube_in_a_square_window"></a> Control\_stays\_outside\_the\_cube\_in\_a\_square\_window\(\)

```csharp
[Fact]
public void Control_stays_outside_the_cube_in_a_square_window()
```

### <a id="Cube_Tests_ViewSettingsTests_Default_view_is_from_behind_at_30_degrees_with_the_control_bottom_right"></a> Default\_view\_is\_from\_behind\_at\_30\_degrees\_with\_the\_control\_bottom\_right\(\)

```csharp
[Fact]
public void Default_view_is_from_behind_at_30_degrees_with_the_control_bottom_right()
```

### <a id="Cube_Tests_ViewSettingsTests_Reset_to_defaults_also_resets_the_view_and_corner"></a> Reset\_to\_defaults\_also\_resets\_the\_view\_and\_corner\(\)

```csharp
[Fact]
public void Reset_to_defaults_also_resets_the_view_and_corner()
```

### <a id="Cube_Tests_ViewSettingsTests_Reset_view_restores_the_default"></a> Reset\_view\_restores\_the\_default\(\)

```csharp
[Fact]
public void Reset_view_restores_the_default()
```

### <a id="Cube_Tests_ViewSettingsTests_Turning_the_view_is_stored_in_the_settings_and_bumps_the_revision"></a> Turning\_the\_view\_is\_stored\_in\_the\_settings\_and\_bumps\_the\_revision\(\)

```csharp
[Fact]
public void Turning_the_view_is_stored_in_the_settings_and_bumps_the_revision()
```

### <a id="Cube_Tests_ViewSettingsTests_Unknown_corner_falls_back_to_bottom_right"></a> Unknown\_corner\_falls\_back\_to\_bottom\_right\(\)

```csharp
[Fact]
public void Unknown_corner_falls_back_to_bottom_right()
```
