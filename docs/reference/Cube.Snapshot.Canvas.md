# <a id="Cube_Snapshot_Canvas"></a> Class Canvas

Namespace: [Cube.Snapshot](Cube.Snapshot.md)  
Assembly: Cube.Snapshot.dll  

An RGBA image in memory that lines and filled polygons can be drawn onto, with alpha blending like ImGui's.
Just enough rasterising to see what the plugin drew; not a general graphics library.

```csharp
public sealed class Canvas
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[Canvas](Cube.Snapshot.Canvas.md)


## Remarks

Polygons are filled by testing each pixel centre in their bounding box against every edge (they are convex:
cube faces). Lines are drawn as thin quads, so both go through the same fill.

## Constructors

### <a id="Cube_Snapshot_Canvas__ctor_System_Int32_System_Int32_System_UInt32_"></a> Canvas\(int, int, uint\)

Creates a canvas filled with one colour.

```csharp
public Canvas(int width, int height, uint background)
```

#### Parameters

`width` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Width in pixels.

`height` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Height in pixels.

`background` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

Fill colour, packed <code>0xAABBGGRR</code> like the draw list.

## Fields

### <a id="Cube_Snapshot_Canvas_pixels"></a> pixels

Pixels as RGBA bytes, row by row from the top.

```csharp
private readonly byte[] pixels
```

#### Field Value

 [byte](https://learn.microsoft.com/dotnet/api/system.byte)\[\]

## Properties

### <a id="Cube_Snapshot_Canvas_Height"></a> Height

Height in pixels.

```csharp
public int Height { get; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Snapshot_Canvas_Pixels"></a> Pixels

The RGBA bytes, row by row from the top.

```csharp
public ReadOnlySpan<byte> Pixels { get; }
```

#### Property Value

 [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

### <a id="Cube_Snapshot_Canvas_Width"></a> Width

Width in pixels.

```csharp
public int Width { get; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Methods

### <a id="Cube_Snapshot_Canvas_Blend_System_Int32_System_Int32_System_UInt32_"></a> Blend\(int, int, uint\)

Blends a colour over one pixel ("source over", like ImGui).

```csharp
private void Blend(int x, int y, uint colour)
```

#### Parameters

`x` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Column.

`y` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Row.

`colour` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

Packed colour.

### <a id="Cube_Snapshot_Canvas_DrawLine_System_Numerics_Vector2_System_Numerics_Vector2_System_UInt32_System_Single_"></a> DrawLine\(Vector2, Vector2, uint, float\)

Draws a line as a quad of the given thickness (at least one pixel, so thin lines don't vanish).

```csharp
public void DrawLine(Vector2 a, Vector2 b, uint colour, float thickness)
```

#### Parameters

`a` [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

Start.

`b` [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

End.

`colour` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

Packed colour.

`thickness` [float](https://learn.microsoft.com/dotnet/api/system.single)

Width in pixels.

### <a id="Cube_Snapshot_Canvas_FillConvex_System_ReadOnlySpan_System_Numerics_Vector2__System_UInt32_"></a> FillConvex\(ReadOnlySpan<Vector2\>, uint\)

Fills a convex polygon, in either winding order.

```csharp
public void FillConvex(ReadOnlySpan<Vector2> points, uint colour)
```

#### Parameters

`points` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)\>

Corners in order round the outline.

`colour` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

Packed <code>0xAABBGGRR</code>; its alpha blends over what's there.

### <a id="Cube_Snapshot_Canvas_GetPixel_System_Int32_System_Int32_"></a> GetPixel\(int, int\)

Reads one pixel as packed <code>0xAABBGGRR</code>.

```csharp
public uint GetPixel(int x, int y)
```

#### Parameters

`x` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Column.

`y` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Row.

#### Returns

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

### <a id="Cube_Snapshot_Canvas_Inside_System_ReadOnlySpan_System_Numerics_Vector2__System_Numerics_Vector2_"></a> Inside\(ReadOnlySpan<Vector2\>, Vector2\)

Whether a point is inside a convex polygon: on the same side of every edge.

```csharp
private static bool Inside(ReadOnlySpan<Vector2> points, Vector2 p)
```

#### Parameters

`points` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)\>

The polygon.

`p` [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

The point.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)
