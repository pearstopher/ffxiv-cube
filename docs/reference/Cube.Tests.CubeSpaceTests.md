# <a id="Cube_Tests_CubeSpaceTests"></a> Class CubeSpaceTests

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Checks the cube of space (PROJECT.md, "Exact behaviour rules": Cube, View, Players): axis-aligned, centred on
the player, edge 10–100 yalms (default 40); players outside are not shown at all; containment is inclusive on
faces; the view turns by yaw (north up: east right, north forward). Also the zero-allocation rule
("Efficiency and memory").

```csharp
public class CubeSpaceTests
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[CubeSpaceTests](Cube.Tests.CubeSpaceTests.md)


## Fields

### <a id="Cube_Tests_CubeSpaceTests_Player"></a> Player

```csharp
private static readonly Vector3 Player
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

## Methods

### <a id="Cube_Tests_CubeSpaceTests_AssertClose_System_Numerics_Vector3_System_Numerics_Vector3_"></a> AssertClose\(Vector3, Vector3\)

```csharp
private static void AssertClose(Vector3 expected, Vector3 actual)
```

#### Parameters

`expected` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

`actual` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Tests_CubeSpaceTests_Containment_and_transform_do_not_allocate"></a> Containment\_and\_transform\_do\_not\_allocate\(\)

```csharp
[Fact]
public void Containment_and_transform_do_not_allocate()
```

### <a id="Cube_Tests_CubeSpaceTests_Corner_is_inside_System_Single_System_Single_System_Single_"></a> Corner\_is\_inside\(float, float, float\)

```csharp
[Theory]
[InlineData(new object[] { 20, 20, 20 })]
[InlineData(new object[] { -20, -20, -20 })]
[InlineData(new object[] { 20, -20, 20 })]
public void Corner_is_inside(float dx, float dy, float dz)
```

#### Parameters

`dx` [float](https://learn.microsoft.com/dotnet/api/system.single)

`dy` [float](https://learn.microsoft.com/dotnet/api/system.single)

`dz` [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_CubeSpaceTests_Cube_is_axis_aligned_so_yaw_does_not_change_containment"></a> Cube\_is\_axis\_aligned\_so\_yaw\_does\_not\_change\_containment\(\)

```csharp
[Fact]
public void Cube_is_axis_aligned_so_yaw_does_not_change_containment()
```

### <a id="Cube_Tests_CubeSpaceTests_Default_edge_is_40_yalms"></a> Default\_edge\_is\_40\_yalms\(\)

```csharp
[Fact]
public void Default_edge_is_40_yalms()
```

### <a id="Cube_Tests_CubeSpaceTests_Edge_is_clamped_to_10_to_100_yalms_System_Single_System_Single_"></a> Edge\_is\_clamped\_to\_10\_to\_100\_yalms\(float, float\)

```csharp
[Theory]
[InlineData(new object[] { 40, 40 })]
[InlineData(new object[] { 5, 10 })]
[InlineData(new object[] { 10, 10 })]
[InlineData(new object[] { 100, 100 })]
[InlineData(new object[] { 250, 100 })]
[InlineData(new object[] { NaN, 40 })]
public void Edge_is_clamped_to_10_to_100_yalms(float requested, float expected)
```

#### Parameters

`requested` [float](https://learn.microsoft.com/dotnet/api/system.single)

`expected` [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_CubeSpaceTests_Looking_east_puts_east_forward_and_south_on_the_right"></a> Looking\_east\_puts\_east\_forward\_and\_south\_on\_the\_right\(\)

```csharp
[Fact]
public void Looking_east_puts_east_forward_and_south_on_the_right()
```

### <a id="Cube_Tests_CubeSpaceTests_Looking_south_mirrors_north_up"></a> Looking\_south\_mirrors\_north\_up\(\)

```csharp
[Fact]
public void Looking_south_mirrors_north_up()
```

### <a id="Cube_Tests_CubeSpaceTests_NorthUp_System_Single_"></a> NorthUp\(float\)

```csharp
private static CubeSpace NorthUp(float edge = 40)
```

#### Parameters

`edge` [float](https://learn.microsoft.com/dotnet/api/system.single)

#### Returns

 [CubeSpace](Cube.Core.CubeSpace.md)

### <a id="Cube_Tests_CubeSpaceTests_North_up_puts_east_on_the_right_and_north_forward"></a> North\_up\_puts\_east\_on\_the\_right\_and\_north\_forward\(\)

```csharp
[Fact]
public void North_up_puts_east_on_the_right_and_north_forward()
```

### <a id="Cube_Tests_CubeSpaceTests_Player_far_above_or_below_is_outside_even_when_horizontally_close_System_Single_"></a> Player\_far\_above\_or\_below\_is\_outside\_even\_when\_horizontally\_close\(float\)

```csharp
[Theory]
[InlineData(new object[] { 25 })]
[InlineData(new object[] { -25 })]
public void Player_far_above_or_below_is_outside_even_when_horizontally_close(float dy)
```

#### Parameters

`dy` [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_CubeSpaceTests_Player_position_is_the_centre"></a> Player\_position\_is\_the\_centre\(\)

```csharp
[Fact]
public void Player_position_is_the_centre()
```

### <a id="Cube_Tests_CubeSpaceTests_Point_exactly_on_a_face_is_inside_System_Single_System_Single_System_Single_"></a> Point\_exactly\_on\_a\_face\_is\_inside\(float, float, float\)

```csharp
[Theory]
[InlineData(new object[] { 20, 0, 0 })]
[InlineData(new object[] { -20, 0, 0 })]
[InlineData(new object[] { 0, 20, 0 })]
[InlineData(new object[] { 0, -20, 0 })]
[InlineData(new object[] { 0, 0, 20 })]
[InlineData(new object[] { 0, 0, -20 })]
public void Point_exactly_on_a_face_is_inside(float dx, float dy, float dz)
```

#### Parameters

`dx` [float](https://learn.microsoft.com/dotnet/api/system.single)

`dy` [float](https://learn.microsoft.com/dotnet/api/system.single)

`dz` [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_CubeSpaceTests_Point_just_outside_a_face_is_outside_System_Single_System_Single_System_Single_"></a> Point\_just\_outside\_a\_face\_is\_outside\(float, float, float\)

```csharp
[Theory]
[InlineData(new object[] { 20.01, 0, 0 })]
[InlineData(new object[] { 0, 0, -20.01 })]
[InlineData(new object[] { 20.01, 20.01, 20.01 })]
public void Point_just_outside_a_face_is_outside(float dx, float dy, float dz)
```

#### Parameters

`dx` [float](https://learn.microsoft.com/dotnet/api/system.single)

`dy` [float](https://learn.microsoft.com/dotnet/api/system.single)

`dz` [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_CubeSpaceTests_Point_outside_is_rejected_before_it_is_transformed"></a> Point\_outside\_is\_rejected\_before\_it\_is\_transformed\(\)

```csharp
[Fact]
public void Point_outside_is_rejected_before_it_is_transformed()
```

### <a id="Cube_Tests_CubeSpaceTests_Recentred_cube_keeps_the_view_rotation_and_moves_with_the_player"></a> Recentred\_cube\_keeps\_the\_view\_rotation\_and\_moves\_with\_the\_player\(\)

```csharp
[Fact]
public void Recentred_cube_keeps_the_view_rotation_and_moves_with_the_player()
```
