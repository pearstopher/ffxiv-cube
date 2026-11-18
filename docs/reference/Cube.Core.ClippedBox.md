# <a id="Cube_Core_ClippedBox"></a> Struct ClippedBox

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

One collision box clipped to the cube (PROJECT.md, "Collision (phase 2)": each collider is shown as a simplified
box, clipped to the cube): an upright prism whose footprint is the part of the box's footprint inside the cube's
turned square, cut to the cube's height.

```csharp
public struct ClippedBox
```


## Remarks

<p>
The boxes are world-axis-aligned but the cube is turned by its yaw, so the overlap of the two footprints is a
convex polygon of up to eight corners, not a rectangle (#93). Drawing the polygon's world-aligned bounds instead
reached up to √2 past the cube's sides for a box bigger than the cube (a floor), so collision stuck out of the
cube and looked like a separate, still shape when the contents turned with the camera. The prism is exact.
</p>
<p>
When the overlap is still a world-aligned rectangle (the box lies inside the cube's footprint, or the cube is at a
multiple of 90°), [IsRectangle](Cube.Core.ClippedBox.md#Cube_Core_ClippedBox_IsRectangle) is true and the box is drawn exactly as before, from
[Bounds](Cube.Core.ClippedBox.md#Cube_Core_ClippedBox_Bounds): the common case stays the cheap one. A plain struct with its corners stored inline, so the
reused buffer in [ClippedCollision](Cube.Core.ClippedCollision.md#Cube_Core_ClippedCollision) never allocates (PROJECT.md, "Efficiency and memory").
</p>

## Constructors

### <a id="Cube_Core_ClippedBox__ctor_Cube_Core_CollisionBox__"></a> ClippedBox\(in CollisionBox\)

Creates a clipped box that is a world-aligned box: no turned edges.

```csharp
public ClippedBox(in CollisionBox bounds)
```

#### Parameters

`bounds` [CollisionBox](Cube.Core.CollisionBox.md)

The box.

### <a id="Cube_Core_ClippedBox__ctor_System_ReadOnlySpan_System_Numerics_Vector2__Cube_Core_CollisionBox__System_Boolean_"></a> ClippedBox\(ReadOnlySpan<Vector2\>, in CollisionBox, bool\)

Creates a prism from its footprint and height.

```csharp
internal ClippedBox(ReadOnlySpan<Vector2> footprint, in CollisionBox bounds, bool isRectangle)
```

#### Parameters

`footprint` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)\>

The footprint's corners in world X/Z, counter-clockwise; at most [MaxCorners](Cube.Core.ClippedBox.md#Cube_Core_ClippedBox_MaxCorners).

`bounds` [CollisionBox](Cube.Core.CollisionBox.md)

The prism's world-aligned bounds.

`isRectangle` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether the footprint is the bounds' own rectangle.

## Fields

### <a id="Cube_Core_ClippedBox_MaxCorners"></a> MaxCorners

Most corners a clipped footprint can have: a rectangle cut by the four sides of another.

```csharp
public const int MaxCorners = 8
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_ClippedBox_corners"></a> corners

The footprint's corners in world X/Z, counter-clockwise; only the first [CornerCount](Cube.Core.ClippedBox.md#Cube_Core_ClippedBox_CornerCount) are used.

```csharp
private FootprintCorners corners
```

#### Field Value

 [FootprintCorners](Cube.Core.FootprintCorners.md)

## Properties

### <a id="Cube_Core_ClippedBox_Bounds"></a> Bounds

The prism's world-aligned bounds: its exact height, and sideways the box around its footprint. Used for the
sorting depth ([FarDepth](Cube.Core.BoxProjector.md#Cube_Core_BoxProjector_FarDepth)), for drawing when [IsRectangle](Cube.Core.ClippedBox.md#Cube_Core_ClippedBox_IsRectangle), and by the
bridge to find the floor.

```csharp
public CollisionBox Bounds { readonly get; private set; }
```

#### Property Value

 [CollisionBox](Cube.Core.CollisionBox.md)

### <a id="Cube_Core_ClippedBox_CornerCount"></a> CornerCount

How many footprint corners there are: 4 for a rectangle, 3 to [MaxCorners](Cube.Core.ClippedBox.md#Cube_Core_ClippedBox_MaxCorners) for a prism.

```csharp
public int CornerCount { readonly get; private set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_ClippedBox_IsRectangle"></a> IsRectangle

Whether the footprint is a world-aligned rectangle equal to [Bounds](Cube.Core.ClippedBox.md#Cube_Core_ClippedBox_Bounds)' (or has no area), so the box
is drawn as a plain box.

```csharp
public bool IsRectangle { readonly get; private set; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Methods

### <a id="Cube_Core_ClippedBox_Corner_System_Int32_"></a> Corner\(int\)

One footprint corner in world X/Z (as `System.Numerics.Vector2` X and Y), counter-clockwise seen with +Z up.

```csharp
public readonly Vector2 Corner(int index)
```

#### Parameters

`index` [int](https://learn.microsoft.com/dotnet/api/system.int32)

0 to [CornerCount](Cube.Core.ClippedBox.md#Cube_Core_ClippedBox_CornerCount) − 1.

#### Returns

 [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

### <a id="Cube_Core_ClippedBox_CornerAt_System_Int32_System_Single_"></a> CornerAt\(int, float\)

One footprint corner as a world point at a height.

```csharp
public readonly Vector3 CornerAt(int index, float y)
```

#### Parameters

`index` [int](https://learn.microsoft.com/dotnet/api/system.int32)

0 to [CornerCount](Cube.Core.ClippedBox.md#Cube_Core_ClippedBox_CornerCount) − 1.

`y` [float](https://learn.microsoft.com/dotnet/api/system.single)

The world height.

#### Returns

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)
