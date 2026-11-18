# <a id="Cube_Tests_PolygonAndOutlineTests"></a> Class PolygonAndOutlineTests

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Checks the draw list's polygons and the cube's screen outline (#93; PROJECT.md, "Cheap drawing": one ImGui call per
face; "Efficiency and memory": fixed buffers, nothing allocated per frame, bounded memory; "Collision (phase 2)":
collision clipped to the cube, which the outline is used to check).

```csharp
public class PolygonAndOutlineTests
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[PolygonAndOutlineTests](Cube.Tests.PolygonAndOutlineTests.md)


## Fields

### <a id="Cube_Tests_PolygonAndOutlineTests_Pentagon"></a> Pentagon

```csharp
private static readonly Vector2[] Pentagon
```

#### Field Value

 [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)\[\]

## Methods

### <a id="Cube_Tests_PolygonAndOutlineTests_Adding_polygons_does_not_allocate"></a> Adding\_polygons\_does\_not\_allocate\(\)

```csharp
[Fact]
public void Adding_polygons_does_not_allocate()
```

### <a id="Cube_Tests_PolygonAndOutlineTests_Cube_outline_holds_its_own_corners_and_centre_but_not_points_beyond_System_Single_System_Single_"></a> Cube\_outline\_holds\_its\_own\_corners\_and\_centre\_but\_not\_points\_beyond\(float, float\)

```csharp
[Theory]
[InlineData(new object[] { 0, 0 })]
[InlineData(new object[] { 0.7, 0 })]
[InlineData(new object[] { 0.7, 70 })]
public void Cube_outline_holds_its_own_corners_and_centre_but_not_points_beyond(float yaw, float viewYaw)
```

#### Parameters

`yaw` [float](https://learn.microsoft.com/dotnet/api/system.single)

`viewYaw` [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_PolygonAndOutlineTests_Polygon_is_dropped_when_the_point_buffer_is_full_and_room_returns_after_clear"></a> Polygon\_is\_dropped\_when\_the\_point\_buffer\_is\_full\_and\_room\_returns\_after\_clear\(\)

```csharp
[Fact]
public void Polygon_is_dropped_when_the_point_buffer_is_full_and_room_returns_after_clear()
```

### <a id="Cube_Tests_PolygonAndOutlineTests_Polygon_keeps_its_corners_through_the_sort"></a> Polygon\_keeps\_its\_corners\_through\_the\_sort\(\)

```csharp
[Fact]
public void Polygon_keeps_its_corners_through_the_sort()
```
