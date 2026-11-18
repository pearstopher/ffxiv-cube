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

### <a id="Cube_Tests_ViewSettingsTests_Corrupt_old_yaw_and_tilt_give_the_default_view"></a> Corrupt\_old\_yaw\_and\_tilt\_give\_the\_default\_view\(\)

```csharp
[Fact]
public void Corrupt_old_yaw_and_tilt_give_the_default_view()
```

### <a id="Cube_Tests_ViewSettingsTests_Corrupt_saved_rotation_becomes_the_default_view_System_Single_System_Single_System_Single_System_Single_"></a> Corrupt\_saved\_rotation\_becomes\_the\_default\_view\(float, float, float, float\)

```csharp
[Theory]
[InlineData(new object[] { NaN, 0, 0, 1 })]
[InlineData(new object[] { 0, Infinity, 0, 1 })]
[InlineData(new object[] { 0, 0, 0, 0 })]
public void Corrupt_saved_rotation_becomes_the_default_view(float x, float y, float z, float w)
```

#### Parameters

`x` [float](https://learn.microsoft.com/dotnet/api/system.single)

`y` [float](https://learn.microsoft.com/dotnet/api/system.single)

`z` [float](https://learn.microsoft.com/dotnet/api/system.single)

`w` [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_ViewSettingsTests_Current_file_keeps_its_rotation_and_ignores_the_old_fields"></a> Current\_file\_keeps\_its\_rotation\_and\_ignores\_the\_old\_fields\(\)

```csharp
[Fact]
public void Current_file_keeps_its_rotation_and_ignores_the_old_fields()
```

### <a id="Cube_Tests_ViewSettingsTests_Default_view_is_from_behind_at_30_degrees_with_the_control_bottom_right"></a> Default\_view\_is\_from\_behind\_at\_30\_degrees\_with\_the\_control\_bottom\_right\(\)

```csharp
[Fact]
public void Default_view_is_from_behind_at_30_degrees_with_the_control_bottom_right()
```

### <a id="Cube_Tests_ViewSettingsTests_Gizmos_shrink_in_a_narrow_window_instead_of_overlapping"></a> Gizmos\_shrink\_in\_a\_narrow\_window\_instead\_of\_overlapping\(\)

```csharp
[Fact]
public void Gizmos_shrink_in_a_narrow_window_instead_of_overlapping()
```

### <a id="Cube_Tests_ViewSettingsTests_Old_fields_are_never_written_to_the_file"></a> Old\_fields\_are\_never\_written\_to\_the\_file\(\)

```csharp
[Fact]
public void Old_fields_are_never_written_to_the_file()
```

### <a id="Cube_Tests_ViewSettingsTests_Old_file_from_before_the_view_control_loads_the_default_view"></a> Old\_file\_from\_before\_the\_view\_control\_loads\_the\_default\_view\(\)

```csharp
[Fact]
public void Old_file_from_before_the_view_control_loads_the_default_view()
```

### <a id="Cube_Tests_ViewSettingsTests_Old_file_with_yaw_and_tilt_loads_as_the_same_orientation_System_Single_System_Single_"></a> Old\_file\_with\_yaw\_and\_tilt\_loads\_as\_the\_same\_orientation\(float, float\)

```csharp
[Theory]
[InlineData(new object[] { 45, 60 })]
[InlineData(new object[] { -120, -20 })]
[InlineData(new object[] { 0, 90 })]
public void Old_file_with_yaw_and_tilt_loads_as_the_same_orientation(float yaw, float pitch)
```

#### Parameters

`yaw` [float](https://learn.microsoft.com/dotnet/api/system.single)

`pitch` [float](https://learn.microsoft.com/dotnet/api/system.single)

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

### <a id="Cube_Tests_ViewSettingsTests_ResizeGizmo_sits_beside_the_rotate_gizmo_inward_from_the_corner_Cube_Core_WidgetCorner_System_Single_System_Single_"></a> ResizeGizmo\_sits\_beside\_the\_rotate\_gizmo\_inward\_from\_the\_corner\(WidgetCorner, float, float\)

```csharp
[Theory]
[InlineData(new object[] { WidgetCorner.BottomRight, 260, 220 })]
[InlineData(new object[] { WidgetCorner.BottomLeft, 60, 220 })]
[InlineData(new object[] { WidgetCorner.TopRight, 260, 20 })]
[InlineData(new object[] { WidgetCorner.TopLeft, 60, 20 })]
public void ResizeGizmo_sits_beside_the_rotate_gizmo_inward_from_the_corner(WidgetCorner corner, float x, float y)
```

#### Parameters

`corner` [WidgetCorner](Cube.Core.WidgetCorner.md)

`x` [float](https://learn.microsoft.com/dotnet/api/system.single)

`y` [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_ViewSettingsTests_Rolled_view_is_stored_as_is"></a> Rolled\_view\_is\_stored\_as\_is\(\)

```csharp
[Fact]
public void Rolled_view_is_stored_as_is()
```

### <a id="Cube_Tests_ViewSettingsTests_Turning_the_view_is_stored_in_the_settings_and_bumps_the_revision"></a> Turning\_the\_view\_is\_stored\_in\_the\_settings\_and\_bumps\_the\_revision\(\)

```csharp
[Fact]
public void Turning_the_view_is_stored_in_the_settings_and_bumps_the_revision()
```

### <a id="Cube_Tests_ViewSettingsTests_TwoGizmos_never_overlap_and_stay_inside_the_region_Cube_Core_WidgetCorner_System_Single_System_Single_"></a> TwoGizmos\_never\_overlap\_and\_stay\_inside\_the\_region\(WidgetCorner, float, float\)

```csharp
[Theory]
[InlineData(new object[] { WidgetCorner.BottomRight, 350, 250 })]
[InlineData(new object[] { WidgetCorner.TopLeft, 350, 250 })]
[InlineData(new object[] { WidgetCorner.BottomLeft, 60, 400 })]
[InlineData(new object[] { WidgetCorner.TopRight, 30, 30 })]
[InlineData(new object[] { WidgetCorner.BottomRight, 10, 10 })]
public void TwoGizmos_never_overlap_and_stay_inside_the_region(WidgetCorner corner, float width, float height)
```

#### Parameters

`corner` [WidgetCorner](Cube.Core.WidgetCorner.md)

`width` [float](https://learn.microsoft.com/dotnet/api/system.single)

`height` [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_ViewSettingsTests_Unknown_corner_falls_back_to_bottom_right"></a> Unknown\_corner\_falls\_back\_to\_bottom\_right\(\)

```csharp
[Fact]
public void Unknown_corner_falls_back_to_bottom_right()
```

### <a id="Cube_Tests_ViewSettingsTests_Unnormalised_saved_rotation_is_normalised"></a> Unnormalised\_saved\_rotation\_is\_normalised\(\)

```csharp
[Fact]
public void Unnormalised_saved_rotation_is_normalised()
```
