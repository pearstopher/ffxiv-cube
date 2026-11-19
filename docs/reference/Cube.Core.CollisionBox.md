# <a id="Cube_Core_CollisionBox"></a> Struct CollisionBox

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

One piece of collision as a simplified box (PROJECT.md, "Collision (phase 2)": each collider inside or crossing the
cube is shown as a simplified box, not the game's model). Upright, but turned about the vertical axis to fit its
object when the game's matrix says so (#123).

```csharp
public readonly struct CollisionBox
```


## Remarks

<p>
The research in #28 (<code>docs/research-notes.md</code>, "Where to get collision data") picked one box per collision
mesh node: the node's local bounds taken through its collider's world matrix ([FromTransformed](Cube.Core.CollisionBox.md#Cube_Core_CollisionBox_FromTransformed)), and
one box per primitive collider (a unit shape placed by its matrix). Those follow walls and floors closely enough to
navigate by, cost one memory read each, and need no triangles.
</p>
<p>
The box is stored as a centre, a half-size along its own axes and the turn as a (cos, sin) pair (32 bytes, so the
bounded cache in [CollisionBoxSet](Cube.Core.CollisionBoxSet.md#Cube_Core_CollisionBoxSet) stays small). Wrapping a turned object in a world-axis box made a
thin diagonal fence about 15-20 times too big (#110, <code>docs/research-notes.md</code>), so a box turned by a pure yaw
keeps its turn. [Min](Cube.Core.CollisionBox.md#Cube_Core_CollisionBox_Min) and [Max](Cube.Core.CollisionBox.md#Cube_Core_CollisionBox_Max) are still the world-axis box around it: culling, merging and
the cheap rejects all use that, so they are no worse than before. A matrix that tilts or shears falls back to that
axis box ([FromTransformed](Cube.Core.CollisionBox.md#Cube_Core_CollisionBox_FromTransformed)). The turn is independent of the camera, so the cached list never has to be
rebuilt for a yaw change.
</p>

## Constructors

### <a id="Cube_Core_CollisionBox__ctor_System_Numerics_Vector3_System_Numerics_Vector3_"></a> CollisionBox\(Vector3, Vector3\)

Creates a world-axis box from its lowest and highest corners.

```csharp
public CollisionBox(Vector3 min, Vector3 max)
```

#### Parameters

`min` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The corner with the smallest X, Y and Z.

`max` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The corner with the largest X, Y and Z.

### <a id="Cube_Core_CollisionBox__ctor_System_Numerics_Vector3_System_Numerics_Vector3_System_Single_System_Single_"></a> CollisionBox\(Vector3, Vector3, float, float\)

Creates a box turned about the vertical axis.

```csharp
public CollisionBox(Vector3 centre, Vector3 half, float cos, float sin)
```

#### Parameters

`centre` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The box's centre in world coordinates.

`half` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

Half the size along the box's own X, Y and Z (its Z is the world Z when not turned).

`cos` [float](https://learn.microsoft.com/dotnet/api/system.single)

Cosine of the turn: the world X and Z of the box's own X axis are (cos, sin).

`sin` [float](https://learn.microsoft.com/dotnet/api/system.single)

Sine of the turn.

## Fields

### <a id="Cube_Core_CollisionBox_PureTurnTolerance"></a> PureTurnTolerance

How far a matrix may be from a pure turn (relative, per axis) before it counts as tilted or sheared.

```csharp
private const float PureTurnTolerance = 0.001
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_CollisionBox_SnapTolerance"></a> SnapTolerance

How far from a quarter turn a yaw may be and still count as one (sine or cosine rounding).

```csharp
private const float SnapTolerance = 0.0001
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_CollisionBox_centre"></a> centre

The box's centre in world coordinates.

```csharp
private readonly Vector3 centre
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_CollisionBox_half"></a> half

Half the size along the box's own axes.

```csharp
private readonly Vector3 half
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

## Properties

### <a id="Cube_Core_CollisionBox_Centre"></a> Centre

The box's centre in world coordinates.

```csharp
public Vector3 Centre { get; }
```

#### Property Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_CollisionBox_Cos"></a> Cos

Cosine of the turn about the vertical axis (1 when not turned).

```csharp
public float Cos { get; }
```

#### Property Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_CollisionBox_HalfSize"></a> HalfSize

Half the size of the world-axis box around this one (the same as [OwnHalfSize](Cube.Core.CollisionBox.md#Cube_Core_CollisionBox_OwnHalfSize) when not turned).

```csharp
public Vector3 HalfSize { get; }
```

#### Property Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_CollisionBox_IsTurned"></a> IsTurned

Whether the box is turned off the world axes, so its footprint is not its world-axis box.

```csharp
public bool IsTurned { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_CollisionBox_IsValid"></a> IsValid

Whether the box is usable: every number finite and no negative size. A zero thickness is allowed: a flat floor
or a thin wall is still something you bump into.

```csharp
public bool IsValid { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

#### Remarks

Memory read from the game can be mid-update or garbage; an invalid box is dropped rather than drawn as a
screen-filling shape (CLAUDE.md: never let bad game data become an exception or a broken frame).

### <a id="Cube_Core_CollisionBox_Max"></a> Max

The corner of the world-axis box around this one with the largest X, Y and Z.

```csharp
public Vector3 Max { get; }
```

#### Property Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_CollisionBox_Min"></a> Min

The corner of the world-axis box around this one with the smallest X, Y and Z.

```csharp
public Vector3 Min { get; }
```

#### Property Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_CollisionBox_OwnHalfSize"></a> OwnHalfSize

Half the box's size along its own axes (the world axes when not turned).

```csharp
public Vector3 OwnHalfSize { get; }
```

#### Property Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_CollisionBox_Sin"></a> Sin

Sine of the turn (0 when not turned; exactly 0 for a box that lines up with the world axes).

```csharp
public float Sin { get; }
```

#### Property Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

## Methods

### <a id="Cube_Core_CollisionBox_Contains_Cube_Core_CollisionBox__"></a> Contains\(in CollisionBox\)

Whether this box wholly contains another (inclusive).

```csharp
public bool Contains(in CollisionBox other)
```

#### Parameters

`other` [CollisionBox](Cube.Core.CollisionBox.md)

The other box.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_CollisionBox_FromTransformed_System_Numerics_Vector3_System_Numerics_Vector3_System_Numerics_Matrix4x4__"></a> FromTransformed\(Vector3, Vector3, in Matrix4x4\)

The box a local box becomes when placed by a world matrix: what a collision mesh node's local bounds or a
primitive collider's unit shape becomes in the world (<code>docs/research-notes.md</code>, #28 and #110).

```csharp
public static CollisionBox FromTransformed(Vector3 localMin, Vector3 localMax, in Matrix4x4 world)
```

#### Parameters

`localMin` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The local box's lowest corner.

`localMax` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The local box's highest corner.

`world` [Matrix4x4](https://learn.microsoft.com/dotnet/api/system.numerics.matrix4x4)

The local-to-world matrix.

#### Returns

 [CollisionBox](Cube.Core.CollisionBox.md)

#### Remarks

Uses the `System.Numerics.Vector3.Transform(System.Numerics.Vector3%2cSystem.Numerics.Matrix4x4)` convention (row vectors, translation in the
fourth row), which is how the game's matrices are laid out. A matrix that is a pure turn about the vertical axis
(with any scale, mirrors included, since a box is its own mirror image) gives a turned box with the scale folded
into its half-size. Any other matrix (tilted or sheared) gives the world-axis box around the transformed
corners: the local centre is transformed and each world half-extent is the sum of the local half-extents
weighted by the absolute matrix entries (Arvo's method), exact for the corners without looping over all eight.

### <a id="Cube_Core_CollisionBox_Intersects_Cube_Core_CollisionBox__"></a> Intersects\(in CollisionBox\)

Whether two boxes overlap or touch (inclusive, like [Contains](Cube.Core.CubeSpace.md#Cube_Core_CubeSpace_Contains)).

```csharp
public bool Intersects(in CollisionBox other)
```

#### Parameters

`other` [CollisionBox](Cube.Core.CollisionBox.md)

The other box.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_CollisionBox_TryYaw_System_Numerics_Matrix4x4__System_Single__System_Single__System_Numerics_Vector3__"></a> TryYaw\(in Matrix4x4, out float, out float, out Vector3\)

Reads the turn out of a matrix if it is a pure turn about the vertical axis, possibly scaled and mirrored.

```csharp
private static bool TryYaw(in Matrix4x4 m, out float cos, out float sin, out Vector3 scale)
```

#### Parameters

`m` [Matrix4x4](https://learn.microsoft.com/dotnet/api/system.numerics.matrix4x4)

The matrix.

`cos` [float](https://learn.microsoft.com/dotnet/api/system.single)

Cosine of the turn of the local X axis.

`sin` [float](https://learn.microsoft.com/dotnet/api/system.single)

Sine of the turn of the local X axis.

`scale` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

How much each local axis is stretched.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

False for a tilted, sheared or degenerate matrix.

#### Remarks

The matrix's rows are the images of the local X, Y and Z axes. A pure yaw keeps Y vertical (row 2 has only an
M22), keeps X and Z horizontal (M12 and M32 zero) and keeps them at right angles. Everything is compared
relative to the axis length so a big scale doesn't loosen the test.

### <a id="Cube_Core_CollisionBox_Union_Cube_Core_CollisionBox__"></a> Union\(in CollisionBox\)

The smallest box holding both boxes.

```csharp
public CollisionBox Union(in CollisionBox other)
```

#### Parameters

`other` [CollisionBox](Cube.Core.CollisionBox.md)

The other box.

#### Returns

 [CollisionBox](Cube.Core.CollisionBox.md)
