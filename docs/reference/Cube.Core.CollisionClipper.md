# <a id="Cube_Core_CollisionClipper"></a> Class CollisionClipper

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

Clips collision boxes to the cube (PROJECT.md, "Collision (phase 2)": each collider inside or crossing the cube is
shown as a simplified box, clipped to the cube). Boxes wholly outside are left out; the rest are cut back to the
part inside, as a [ClippedBox](Cube.Core.ClippedBox.md#Cube_Core_ClippedBox).

```csharp
public static class CollisionClipper
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[CollisionClipper](Cube.Core.CollisionClipper.md)


## Remarks

<p>
The boxes are world-axis-aligned ([CollisionBox](Cube.Core.CollisionBox.md#Cube_Core_CollisionBox)) but the cube is turned by its yaw, so the overlap of
a box and the cube is an upright prism: the height is cut exactly, and sideways the cube's square footprint is cut
to the box's footprint (Sutherland–Hodgman against four axis lines), leaving a convex polygon of at most eight
corners. That polygon is kept as the prism's footprint, so nothing drawn reaches outside the cube at any yaw (#93).
With the cube north up, at any multiple of 90°, or for a box wholly inside the cube's footprint, the polygon is the
box's own rectangle and the result is marked [IsRectangle](Cube.Core.ClippedBox.md#Cube_Core_ClippedBox_IsRectangle), which draws as a plain box.
</p>
<p>
Runs every frame the cube moves, over the cached boxes, so it allocates nothing: the footprint is computed once
per call and the polygon buffers are on the stack (PROJECT.md, "Efficiency and memory").
</p>

## Fields

### <a id="Cube_Core_CollisionClipper_AxisTolerance"></a> AxisTolerance

How far off an axis an edge may be and still count as world-aligned, in yalms (rotation rounding).

```csharp
private const float AxisTolerance = 0.0001
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_CollisionClipper_SameCorner"></a> SameCorner

Corners closer than this (in yalms, squared) are one corner, and an edge shorter than it doesn't count as
turned: float noise from the rotation, far below anything visible.

```csharp
private const float SameCorner = 1E-08
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_CollisionClipper_ScratchCorners"></a> ScratchCorners

Scratch polygon size: room for the duplicate corners a cut makes when a corner lies exactly on a line, before
they are removed.

```csharp
private const int ScratchCorners = 16
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Methods

### <a id="Cube_Core_CollisionClipper_Clip_Cube_Core_CubeSpace__System_ReadOnlySpan_Cube_Core_CollisionBox__System_Span_Cube_Core_ClippedBox__"></a> Clip\(in CubeSpace, ReadOnlySpan<CollisionBox\>, Span<ClippedBox\>\)

Clips every box to the cube and writes the ones that reach into it, in order, to
<code class="paramref">destination</code>.

```csharp
public static int Clip(in CubeSpace cube, ReadOnlySpan<CollisionBox> source, Span<ClippedBox> destination)
```

#### Parameters

`cube` [CubeSpace](Cube.Core.CubeSpace.md)

This frame's cube.

`source` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[CollisionBox](Cube.Core.CollisionBox.md)\>

The cached world boxes ([Items](Cube.Core.CollisionBoxSet.md#Cube_Core_CollisionBoxSet_Items)).

`destination` [Span](https://learn.microsoft.com/dotnet/api/system.span\-1)<[ClippedBox](Cube.Core.ClippedBox.md)\>

Where the clipped boxes go; boxes beyond its length are not written.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

How many boxes were written.

### <a id="Cube_Core_CollisionClipper_Cut_System_ReadOnlySpan_System_Numerics_Vector2__System_Int32_System_Span_System_Numerics_Vector2__System_Int32_System_Single_System_Boolean_"></a> Cut\(ReadOnlySpan<Vector2\>, int, Span<Vector2\>, int, float, bool\)

One Sutherland–Hodgman step: keeps the part of a convex polygon on one side of an axis line (inclusive).

```csharp
private static int Cut(ReadOnlySpan<Vector2> input, int count, Span<Vector2> output, int axis, float line, bool keepAbove)
```

#### Parameters

`input` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)\>

The polygon's corners.

`count` [int](https://learn.microsoft.com/dotnet/api/system.int32)

How many corners it has.

`output` [Span](https://learn.microsoft.com/dotnet/api/system.span\-1)<[Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)\>

Where the cut polygon goes.

`axis` [int](https://learn.microsoft.com/dotnet/api/system.int32)

0 cuts along X (a line of constant X), 1 along Z.

`line` [float](https://learn.microsoft.com/dotnet/api/system.single)

The line's X or Z.

`keepAbove` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

True keeps coordinates ≥ the line, false keeps ≤.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

How many corners the cut polygon has.

### <a id="Cube_Core_CollisionClipper_IsAxisAlignedRectangle_System_ReadOnlySpan_System_Numerics_Vector2__"></a> IsAxisAlignedRectangle\(ReadOnlySpan<Vector2\>\)

Whether a polygon is four corners joined by world-axis-aligned edges: a rectangle equal to its bounds.

```csharp
private static bool IsAxisAlignedRectangle(ReadOnlySpan<Vector2> polygon)
```

#### Parameters

`polygon` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)\>

The corners.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_CollisionClipper_RemoveDuplicates_System_Span_System_Numerics_Vector2__"></a> RemoveDuplicates\(Span<Vector2\>\)

Drops corners that repeat the one before them (a cut through an existing corner adds it twice), including the
last against the first.

```csharp
private static int RemoveDuplicates(Span<Vector2> polygon)
```

#### Parameters

`polygon` [Span](https://learn.microsoft.com/dotnet/api/system.span\-1)<[Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)\>

The corners, compacted in place.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

How many corners are left.

### <a id="Cube_Core_CollisionClipper_SignedArea_System_ReadOnlySpan_System_Numerics_Vector2__"></a> SignedArea\(ReadOnlySpan<Vector2\>\)

The signed area of a polygon (shoelace); positive when the corners go counter-clockwise.

```csharp
private static float SignedArea(ReadOnlySpan<Vector2> polygon)
```

#### Parameters

`polygon` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)\>

The corners.

#### Returns

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_CollisionClipper_TryClip_Cube_Core_CubeSpace__Cube_Core_CollisionBox__Cube_Core_ClippedBox__"></a> TryClip\(in CubeSpace, in CollisionBox, out ClippedBox\)

Clips one box to the cube.

```csharp
public static bool TryClip(in CubeSpace cube, in CollisionBox box, out ClippedBox clipped)
```

#### Parameters

`cube` [CubeSpace](Cube.Core.CubeSpace.md)

The cube.

`box` [CollisionBox](Cube.Core.CollisionBox.md)

A world box.

`clipped` [ClippedBox](Cube.Core.ClippedBox.md)

The part of the box inside the cube, if any.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

False if the box doesn't reach into the cube (touching a face counts as reaching it).

### <a id="Cube_Core_CollisionClipper_TryClip_Cube_Core_CubeSpace__Cube_Core_CollisionClipper_Footprint__Cube_Core_CollisionBox__System_Span_System_Numerics_Vector2__System_Span_System_Numerics_Vector2__Cube_Core_ClippedBox__"></a> TryClip\(in CubeSpace, in Footprint, in CollisionBox, Span<Vector2\>, Span<Vector2\>, out ClippedBox\)

Clips one box: height first (no rotation needed, the cheapest reject), then the footprint.

```csharp
private static bool TryClip(in CubeSpace cube, in CollisionClipper.Footprint footprint, in CollisionBox box, Span<Vector2> a, Span<Vector2> b, out ClippedBox clipped)
```

#### Parameters

`cube` [CubeSpace](Cube.Core.CubeSpace.md)

The cube.

`footprint` [CollisionClipper](Cube.Core.CollisionClipper.md).[Footprint](Cube.Core.CollisionClipper.Footprint.md)

The cube's footprint corners in world X/Z.

`box` [CollisionBox](Cube.Core.CollisionBox.md)

The box.

`a` [Span](https://learn.microsoft.com/dotnet/api/system.span\-1)<[Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)\>

Scratch polygon buffer.

`b` [Span](https://learn.microsoft.com/dotnet/api/system.span\-1)<[Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)\>

Second scratch polygon buffer.

`clipped` [ClippedBox](Cube.Core.ClippedBox.md)

The clipped box.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)
