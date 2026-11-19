# <a id="Cube_Core_SegmentClip"></a> Class SegmentClip

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

Clips a line segment to the cube in cube-local coordinates (#100; PROJECT.md, "Collision style": everything is
clipped exactly to the cube). The Mesh style draws triangle <em>edges</em>, so clipping segments (Liang–Barsky)
is all it needs: unlike clipping whole triangles it never invents an edge along the cube's face.

```csharp
public static class SegmentClip
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[SegmentClip](Cube.Core.SegmentClip.md)


## Remarks

Cube-local coordinates have the cube centred on the origin ([ToView](Cube.Core.CubeSpace.md#Cube_Core_CubeSpace_ToView)), so its faces are the
planes ±<code>half</code> on each axis. A segment wholly inside is returned untouched, so uncut edges keep their exact
corners; a cut end is moved onto the face it crosses, then clamped into the box so rounding can never leave it a
hair outside.

## Methods

### <a id="Cube_Core_SegmentClip_Clip_System_Numerics_Vector3__System_Numerics_Vector3__System_Numerics_Vector3_"></a> Clip\(ref Vector3, ref Vector3, Vector3\)

Clips a segment to the box, moving its ends in.

```csharp
public static bool Clip(ref Vector3 a, ref Vector3 b, Vector3 half)
```

#### Parameters

`a` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

One end; replaced by the clipped end.

`b` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The other end; replaced by the clipped end.

`half` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The cube's half extents.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

False if no part of the segment is inside the box (the ends are then unchanged).

### <a id="Cube_Core_SegmentClip_Clip_System_Numerics_Vector3__System_Numerics_Vector3__System_Numerics_Vector3_System_Int32_System_Int32_"></a> Clip\(ref Vector3, ref Vector3, Vector3, int, int\)

As [Clip](Cube.Core.SegmentClip.md#Cube_Core_SegmentClip_Clip) when the ends' outcodes are already known.

```csharp
public static bool Clip(ref Vector3 a, ref Vector3 b, Vector3 half, int codeA, int codeB)
```

#### Parameters

`a` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

One end; replaced by the clipped end.

`b` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The other end; replaced by the clipped end.

`half` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The cube's half extents.

`codeA` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The outcode of <code class="paramref">a</code>.

`codeB` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The outcode of <code class="paramref">b</code>.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

False if no part of the segment is inside the box.

### <a id="Cube_Core_SegmentClip_Outcode_System_Numerics_Vector3_System_Numerics_Vector3_"></a> Outcode\(Vector3, Vector3\)

Which of the six faces' outsides a point is beyond, as bits (X low, X high, Y low, Y high, Z low, Z high).

```csharp
public static int Outcode(Vector3 p, Vector3 half)
```

#### Parameters

`p` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

A cube-local point.

`half` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The cube's half extents.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_SegmentClip_Slab_System_Single_System_Single_System_Single_System_Single__System_Single__"></a> Slab\(float, float, float, ref float, ref float\)

Narrows the parameter range to the part of the segment inside one axis's slab.

```csharp
private static bool Slab(float start, float delta, float half, ref float t0, ref float t1)
```

#### Parameters

`start` [float](https://learn.microsoft.com/dotnet/api/system.single)

The segment's start on this axis.

`delta` [float](https://learn.microsoft.com/dotnet/api/system.single)

The segment's change along this axis.

`half` [float](https://learn.microsoft.com/dotnet/api/system.single)

The slab's half extent.

`t0` [float](https://learn.microsoft.com/dotnet/api/system.single)

The range's start, raised as needed.

`t1` [float](https://learn.microsoft.com/dotnet/api/system.single)

The range's end, lowered as needed.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

False if the range became empty.
