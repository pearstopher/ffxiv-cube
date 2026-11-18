# <a id="Cube_Core_CubeOutline"></a> Struct CubeOutline

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

The big cube's outline on screen: the convex hull of its eight projected corners. Used to check that nothing drawn
for the contents reaches outside the cube (PROJECT.md, "Collision (phase 2)": clipped to the cube; #93).

```csharp
public readonly struct CubeOutline
```


## Remarks

The projection is orthographic, so a point inside the cube projects inside this outline; the reverse test is what
the unit tests and the in-game bridge use on every collision corner. Built once per check, then each test is a
few cross products. A struct with its corners stored inline: nothing is allocated.

## Constructors

### <a id="Cube_Core_CubeOutline__ctor_Cube_Core_CubeSpace__Cube_Core_ViewProjection__"></a> CubeOutline\(in CubeSpace, in ViewProjection\)

Projects the cube's corners and keeps their convex hull.

```csharp
public CubeOutline(in CubeSpace space, in ViewProjection projection)
```

#### Parameters

`space` [CubeSpace](Cube.Core.CubeSpace.md)

The cube.

`projection` [ViewProjection](Cube.Core.ViewProjection.md)

The view's projection.

## Fields

### <a id="Cube_Core_CubeOutline_count"></a> count

How many hull corners there are (4 to 6 for a box seen from any angle, fewer if degenerate).

```csharp
private readonly int count
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_CubeOutline_hull"></a> hull

The hull's corners in screen pixels, in order around it.

```csharp
private readonly FootprintCorners hull
```

#### Field Value

 [FootprintCorners](Cube.Core.FootprintCorners.md)

## Methods

### <a id="Cube_Core_CubeOutline_Contains_System_Numerics_Vector2_System_Single_"></a> Contains\(Vector2, float\)

Whether a screen point is inside the outline or within a tolerance of it.

```csharp
public bool Contains(Vector2 point, float tolerance)
```

#### Parameters

`point` [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

A point in screen pixels.

`tolerance` [float](https://learn.microsoft.com/dotnet/api/system.single)

How far outside still counts, in pixels (float rounding).

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_CubeOutline_Cross_System_Numerics_Vector2_System_Numerics_Vector2_System_Numerics_Vector2_"></a> Cross\(Vector2, Vector2, Vector2\)

The z of (b − a) × (c − a): positive when a, b, c turn counter-clockwise.

```csharp
private static float Cross(Vector2 a, Vector2 b, Vector2 c)
```

#### Parameters

`a` [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

First point.

`b` [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

Second point.

`c` [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

Third point.

#### Returns

 [float](https://learn.microsoft.com/dotnet/api/system.single)
