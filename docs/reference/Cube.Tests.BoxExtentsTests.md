# <a id="Cube_Tests_BoxExtentsTests"></a> Class BoxExtentsTests

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Checks the box size rules (PROJECT.md, "Cube" and "Zoom", #90): each extent 1–100, default 40; zoom scales all
three together keeping their ratios, clamped so no extent leaves 1–100; zooming in means less space.

```csharp
public class BoxExtentsTests
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BoxExtentsTests](Cube.Tests.BoxExtentsTests.md)


## Methods

### <a id="Cube_Tests_BoxExtentsTests_AssertClose_System_Numerics_Vector3_System_Numerics_Vector3_"></a> AssertClose\(Vector3, Vector3\)

```csharp
private static void AssertClose(Vector3 expected, Vector3 actual)
```

#### Parameters

`expected` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

`actual` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Tests_BoxExtentsTests_Clamping_and_zooming_do_not_allocate"></a> Clamping\_and\_zooming\_do\_not\_allocate\(\)

```csharp
[Fact]
public void Clamping_and_zooming_do_not_allocate()
```

### <a id="Cube_Tests_BoxExtentsTests_Corrupt_extent_becomes_the_default"></a> Corrupt\_extent\_becomes\_the\_default\(\)

```csharp
[Fact]
public void Corrupt_extent_becomes_the_default()
```

### <a id="Cube_Tests_BoxExtentsTests_Default_size_is_40_on_every_axis"></a> Default\_size\_is\_40\_on\_every\_axis\(\)

```csharp
[Fact]
public void Default_size_is_40_on_every_axis()
```

### <a id="Cube_Tests_BoxExtentsTests_Largest_picks_the_biggest_axis"></a> Largest\_picks\_the\_biggest\_axis\(\)

```csharp
[Fact]
public void Largest_picks_the_biggest_axis()
```

### <a id="Cube_Tests_BoxExtentsTests_Unusable_zoom_factor_leaves_the_size_alone_System_Single_"></a> Unusable\_zoom\_factor\_leaves\_the\_size\_alone\(float\)

```csharp
[Theory]
[InlineData(new object[] { 0 })]
[InlineData(new object[] { -2 })]
[InlineData(new object[] { NaN })]
[InlineData(new object[] { Infinity })]
public void Unusable_zoom_factor_leaves_the_size_alone(float factor)
```

#### Parameters

`factor` [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_BoxExtentsTests_Zoom_at_the_one_to_hundred_limit_does_not_throw"></a> Zoom\_at\_the\_one\_to\_hundred\_limit\_does\_not\_throw\(\)

```csharp
[Fact]
public void Zoom_at_the_one_to_hundred_limit_does_not_throw()
```

### <a id="Cube_Tests_BoxExtentsTests_Zoom_in_means_less_space"></a> Zoom\_in\_means\_less\_space\(\)

```csharp
[Fact]
public void Zoom_in_means_less_space()
```

### <a id="Cube_Tests_BoxExtentsTests_Zoom_in_stops_when_the_smallest_extent_reaches_1_and_keeps_the_ratios"></a> Zoom\_in\_stops\_when\_the\_smallest\_extent\_reaches\_1\_and\_keeps\_the\_ratios\(\)

```csharp
[Fact]
public void Zoom_in_stops_when_the_smallest_extent_reaches_1_and_keeps_the_ratios()
```

### <a id="Cube_Tests_BoxExtentsTests_Zoom_out_keeps_the_ratios"></a> Zoom\_out\_keeps\_the\_ratios\(\)

```csharp
[Fact]
public void Zoom_out_keeps_the_ratios()
```

### <a id="Cube_Tests_BoxExtentsTests_Zoom_out_stops_when_the_largest_extent_reaches_100_and_keeps_the_ratios"></a> Zoom\_out\_stops\_when\_the\_largest\_extent\_reaches\_100\_and\_keeps\_the\_ratios\(\)

```csharp
[Fact]
public void Zoom_out_stops_when_the_largest_extent_reaches_100_and_keeps_the_ratios()
```
