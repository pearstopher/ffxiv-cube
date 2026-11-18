# <a id="Cube_Core_CollisionBox"></a> Struct CollisionBox

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

One piece of collision as a simplified, world-axis-aligned box (PROJECT.md, "Collision (phase 2)": each collider
inside or crossing the cube is shown as a simplified box, not the game's model).

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
World-axis-aligned, like the player cubes (both drawn by [BoxProjector](Cube.Core.BoxProjector.md#Cube_Core_BoxProjector)), so a box turns with the rest
of the cube's contents when the camera turns and the cached list never has to be rebuilt for a yaw change. Two
`System.Numerics.Vector3`s, 24 bytes, so the bounded cache in [CollisionBoxSet](Cube.Core.CollisionBoxSet.md#Cube_Core_CollisionBoxSet) stays small.
</p>

## Constructors

### <a id="Cube_Core_CollisionBox__ctor_System_Numerics_Vector3_System_Numerics_Vector3_"></a> CollisionBox\(Vector3, Vector3\)

Creates a box from its lowest and highest corners.

```csharp
public CollisionBox(Vector3 min, Vector3 max)
```

#### Parameters

`min` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The corner with the smallest X, Y and Z.

`max` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The corner with the largest X, Y and Z.

## Properties

### <a id="Cube_Core_CollisionBox_Centre"></a> Centre

The box's centre in world coordinates.

```csharp
public Vector3 Centre { get; }
```

#### Property Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_CollisionBox_HalfSize"></a> HalfSize

Half the box's size along each world axis.

```csharp
public Vector3 HalfSize { get; }
```

#### Property Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_CollisionBox_IsValid"></a> IsValid

Whether the box is usable: every coordinate a finite number and [Min](Cube.Core.CollisionBox.md#Cube_Core_CollisionBox_Min) not above [Max](Cube.Core.CollisionBox.md#Cube_Core_CollisionBox_Max)
on any axis. A zero thickness is allowed: a flat floor or a thin wall is still something you bump into.

```csharp
public bool IsValid { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

#### Remarks

Memory read from the game can be mid-update or garbage; an invalid box is dropped rather than drawn as a
screen-filling shape (CLAUDE.md: never let bad game data become an exception or a broken frame).

### <a id="Cube_Core_CollisionBox_Max"></a> Max

The corner with the largest world X, Y and Z.

```csharp
public Vector3 Max { get; }
```

#### Property Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_CollisionBox_Min"></a> Min

The corner with the smallest world X, Y and Z.

```csharp
public Vector3 Min { get; }
```

#### Property Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

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

The world box around a local box placed by a world matrix: what a collision mesh node's local bounds or a
primitive collider's unit shape becomes in the world (<code>docs/research-notes.md</code>, #28 recommendation).

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
fourth row), which is how the game's matrices are laid out. The local centre is transformed and each world
half-extent is the sum of the local half-extents weighted by the absolute matrix entries (Arvo's method):
exact for the corners, no loop over the eight of them.

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
