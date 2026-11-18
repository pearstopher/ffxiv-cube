# <a id="Cube_Core_CollisionClipper"></a> Class CollisionClipper

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

Clips collision boxes to the cube (PROJECT.md, "Collision (phase 2)": each collider inside or crossing the cube is
shown as a simplified box, clipped to the cube). Boxes wholly outside are left out; the rest are cut back to the
part inside.

```csharp
public static class CollisionClipper
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[CollisionClipper](Cube.Core.CollisionClipper.md)


## Remarks

<p>
The boxes are world-axis-aligned ([CollisionBox](Cube.Core.CollisionBox.md#Cube_Core_CollisionBox)) but the cube is turned by its yaw, so the exact
overlap of a box and the cube is usually not a box. The clipper keeps the spec's "simplified box" and makes it as
tight as a world-aligned box can be: the height is cut exactly, and sideways the cube's square footprint is cut
to the box's footprint (Sutherland–Hodgman against four axis lines, at most eight corners), and the clipped box
is the bounds of what remains. With the cube north up, or at any multiple of 90°, that is the exact overlap; at
other yaws a box can reach past a side of the cube by at most the corner triangle the box's own corner cuts off.
</p>
<p>
Runs every frame over the cached boxes, so it allocates nothing: the footprint is computed once per call and the
polygon buffers are on the stack (PROJECT.md, "Efficiency and memory").
</p>

## Fields

### <a id="Cube_Core_CollisionClipper_MaxCorners"></a> MaxCorners

Most corners a clipped footprint can have: a rectangle cut by four lines.

```csharp
private const int MaxCorners = 8
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Methods

### <a id="Cube_Core_CollisionClipper_Clip_Cube_Core_CubeSpace__System_ReadOnlySpan_Cube_Core_CollisionBox__System_Span_Cube_Core_CollisionBox__"></a> Clip\(in CubeSpace, ReadOnlySpan<CollisionBox\>, Span<CollisionBox\>\)

Clips every box to the cube and writes the ones that reach into it, in order, to
<code class="paramref">destination</code>.

```csharp
public static int Clip(in CubeSpace cube, ReadOnlySpan<CollisionBox> source, Span<CollisionBox> destination)
```

#### Parameters

`cube` [CubeSpace](Cube.Core.CubeSpace.md)

This frame's cube.

`source` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[CollisionBox](Cube.Core.CollisionBox.md)\>

The cached world boxes ([Items](Cube.Core.CollisionBoxSet.md#Cube_Core_CollisionBoxSet_Items)).

`destination` [Span](https://learn.microsoft.com/dotnet/api/system.span\-1)<[CollisionBox](Cube.Core.CollisionBox.md)\>

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

### <a id="Cube_Core_CollisionClipper_TryClip_Cube_Core_CubeSpace__Cube_Core_CollisionBox__Cube_Core_CollisionBox__"></a> TryClip\(in CubeSpace, in CollisionBox, out CollisionBox\)

Clips one box to the cube.

```csharp
public static bool TryClip(in CubeSpace cube, in CollisionBox box, out CollisionBox clipped)
```

#### Parameters

`cube` [CubeSpace](Cube.Core.CubeSpace.md)

The cube.

`box` [CollisionBox](Cube.Core.CollisionBox.md)

A world box.

`clipped` [CollisionBox](Cube.Core.CollisionBox.md)

The part of the box inside the cube, as a world box, if any.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

False if the box doesn't reach into the cube (touching a face counts as reaching it).

### <a id="Cube_Core_CollisionClipper_TryClip_Cube_Core_CubeSpace__Cube_Core_CollisionClipper_Footprint__Cube_Core_CollisionBox__System_Span_System_Numerics_Vector2__System_Span_System_Numerics_Vector2__Cube_Core_CollisionBox__"></a> TryClip\(in CubeSpace, in Footprint, in CollisionBox, Span<Vector2\>, Span<Vector2\>, out CollisionBox\)

Clips one box: height first (no rotation needed, the cheapest reject), then the footprint.

```csharp
private static bool TryClip(in CubeSpace cube, in CollisionClipper.Footprint footprint, in CollisionBox box, Span<Vector2> a, Span<Vector2> b, out CollisionBox clipped)
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

`clipped` [CollisionBox](Cube.Core.CollisionBox.md)

The clipped box.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)
