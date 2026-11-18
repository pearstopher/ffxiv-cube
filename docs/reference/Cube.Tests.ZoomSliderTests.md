# <a id="Cube_Tests_ZoomSliderTests"></a> Class ZoomSliderTests

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Checks the settings window's zoom slider (#94; PROJECT.md, "Zoom" and "Control"): its value is the box's largest
extent; moving it scales all three extents together keeping their ratios; its range stops where the smallest
extent reaches 1 and the largest 100; it stores nothing, so changing an extent moves it; dragging back and forth
never drifts.

```csharp
public class ZoomSliderTests
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ZoomSliderTests](Cube.Tests.ZoomSliderTests.md)


## Fields

### <a id="Cube_Tests_ZoomSliderTests_TallBox"></a> TallBox

```csharp
private static readonly Vector3 TallBox
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

## Methods

### <a id="Cube_Tests_ZoomSliderTests_A_box_at_the_one_to_hundred_limit_cannot_zoom"></a> A\_box\_at\_the\_one\_to\_hundred\_limit\_cannot\_zoom\(\)

```csharp
[Fact]
public void A_box_at_the_one_to_hundred_limit_cannot_zoom()
```

### <a id="Cube_Tests_ZoomSliderTests_A_cube_can_zoom_over_the_whole_1_to_100_range"></a> A\_cube\_can\_zoom\_over\_the\_whole\_1\_to\_100\_range\(\)

```csharp
[Fact]
public void A_cube_can_zoom_over_the_whole_1_to_100_range()
```

### <a id="Cube_Tests_ZoomSliderTests_A_new_drag_starts_from_the_size_at_that_point"></a> A\_new\_drag\_starts\_from\_the\_size\_at\_that\_point\(\)

```csharp
[Fact]
public void A_new_drag_starts_from_the_size_at_that_point()
```

### <a id="Cube_Tests_ZoomSliderTests_AssertClose_System_Numerics_Vector3_System_Numerics_Vector3_"></a> AssertClose\(Vector3, Vector3\)

```csharp
private static void AssertClose(Vector3 expected, Vector3 actual)
```

#### Parameters

`expected` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

`actual` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Tests_ZoomSliderTests_Changing_one_extent_moves_the_slider"></a> Changing\_one\_extent\_moves\_the\_slider\(\)

```csharp
[Fact]
public void Changing_one_extent_moves_the_slider()
```

### <a id="Cube_Tests_ZoomSliderTests_Corrupt_size_reads_as_the_clamped_size"></a> Corrupt\_size\_reads\_as\_the\_clamped\_size\(\)

```csharp
[Fact]
public void Corrupt_size_reads_as_the_clamped_size()
```

### <a id="Cube_Tests_ZoomSliderTests_Default_cube_reads_40"></a> Default\_cube\_reads\_40\(\)

```csharp
[Fact]
public void Default_cube_reads_40()
```

### <a id="Cube_Tests_ZoomSliderTests_Dragging_back_and_forth_returns_exactly_to_the_start"></a> Dragging\_back\_and\_forth\_returns\_exactly\_to\_the\_start\(\)

```csharp
[Fact]
public void Dragging_back_and_forth_returns_exactly_to_the_start()
```

### <a id="Cube_Tests_ZoomSliderTests_Dragging_keeps_the_ratios"></a> Dragging\_keeps\_the\_ratios\(\)

```csharp
[Fact]
public void Dragging_keeps_the_ratios()
```

### <a id="Cube_Tests_ZoomSliderTests_Range_stops_where_the_smallest_reaches_1_and_the_largest_100"></a> Range\_stops\_where\_the\_smallest\_reaches\_1\_and\_the\_largest\_100\(\)

```csharp
[Fact]
public void Range_stops_where_the_smallest_reaches_1_and_the_largest_100()
```

### <a id="Cube_Tests_ZoomSliderTests_Slider_to_extents_to_slider_round_trips"></a> Slider\_to\_extents\_to\_slider\_round\_trips\(\)

```csharp
[Fact]
public void Slider_to_extents_to_slider_round_trips()
```

### <a id="Cube_Tests_ZoomSliderTests_Slider_value_is_the_largest_extent"></a> Slider\_value\_is\_the\_largest\_extent\(\)

```csharp
[Fact]
public void Slider_value_is_the_largest_extent()
```

### <a id="Cube_Tests_ZoomSliderTests_Updating_and_dragging_do_not_allocate"></a> Updating\_and\_dragging\_do\_not\_allocate\(\)

```csharp
[Fact]
public void Updating_and_dragging_do_not_allocate()
```

### <a id="Cube_Tests_ZoomSliderTests_Values_past_the_limits_clamp_and_keep_the_ratios_System_Single_System_Single_System_Single_System_Single_"></a> Values\_past\_the\_limits\_clamp\_and\_keep\_the\_ratios\(float, float, float, float\)

```csharp
[Theory]
[InlineData(new object[] { 5, 1.25, 5, 2.5 })]
[InlineData(new object[] { 0.5, 1, 4, 2 })]
[InlineData(new object[] { 500, 25, 100, 50 })]
public void Values_past_the_limits_clamp_and_keep_the_ratios(float value, float x, float y, float z)
```

#### Parameters

`value` [float](https://learn.microsoft.com/dotnet/api/system.single)

`x` [float](https://learn.microsoft.com/dotnet/api/system.single)

`y` [float](https://learn.microsoft.com/dotnet/api/system.single)

`z` [float](https://learn.microsoft.com/dotnet/api/system.single)
