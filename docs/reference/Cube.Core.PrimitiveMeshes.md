# <a id="Cube_Core_PrimitiveMeshes"></a> Class PrimitiveMeshes

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

Turns the game's primitive colliders (box, cylinder, sphere, plane) into triangles for the Mesh style (#100;
PROJECT.md, "Collision style": primitives are shown as their real shapes, in triangles).

```csharp
public static class PrimitiveMeshes
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[PrimitiveMeshes](Cube.Core.PrimitiveMeshes.md)


## Remarks

<p>
Each primitive is a unit shape placed by its collider's world matrix (<code>docs/research-notes.md</code>, #28): a box
from (-1,-1,-1) to (1,1,1), a plane from (-1,-1,0) to (1,1,0), a cylinder of radius 1 from y = -1 to 1, and a sphere
of radius 1. A box and a plane are exact; a cylinder and a sphere are curved, so they are drawn as a
[CylinderSides](Cube.Core.PrimitiveMeshes.md#Cube_Core_PrimitiveMeshes_CylinderSides)-sided prism and a sphere cut into [SphereTriangles](Cube.Core.PrimitiveMeshes.md#Cube_Core_PrimitiveMeshes_SphereTriangles) triangles, which is what
any triangle wireframe of a curve is. The matrix uses the `System.Numerics.Vector3.Transform(System.Numerics.Vector3%2cSystem.Numerics.Matrix4x4)`
convention like [FromTransformed](Cube.Core.CollisionBox.md#Cube_Core_CollisionBox_FromTransformed). Nothing is allocated: corners live on the stack.
</p>

## Fields

### <a id="Cube_Core_PrimitiveMeshes_BoxTriangles"></a> BoxTriangles

Triangles in a box: two per face.

```csharp
public const int BoxTriangles = 12
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_PrimitiveMeshes_CylinderSides"></a> CylinderSides

Sides of a cylinder's prism.

```csharp
public const int CylinderSides = 16
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_PrimitiveMeshes_CylinderTriangles"></a> CylinderTriangles

Triangles in a cylinder: two per side wall and one per side for each cap.

```csharp
public const int CylinderTriangles = 64
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_PrimitiveMeshes_PlaneTriangles"></a> PlaneTriangles

Triangles in a plane: one quad.

```csharp
public const int PlaneTriangles = 2
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_PrimitiveMeshes_SphereTriangles"></a> SphereTriangles

Triangles in a sphere: an icosahedron's 20 faces, each cut into four.

```csharp
public const int SphereTriangles = 80
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Properties

### <a id="Cube_Core_PrimitiveMeshes_BoxQuads"></a> BoxQuads

The box's faces as corner quads; corner bits are X, Y, Z (0 low, 1 high).

```csharp
private static ReadOnlySpan<byte> BoxQuads { get; }
```

#### Property Value

 [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

### <a id="Cube_Core_PrimitiveMeshes_IcosahedronFaces"></a> IcosahedronFaces

The icosahedron's corner order, three per face.

```csharp
private static ReadOnlySpan<byte> IcosahedronFaces { get; }
```

#### Property Value

 [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

## Methods

### <a id="Cube_Core_PrimitiveMeshes_AddBox_Cube_Core_TriangleSet_System_Numerics_Matrix4x4__"></a> AddBox\(TriangleSet, in Matrix4x4\)

Adds a box collider's 12 triangles.

```csharp
public static void AddBox(TriangleSet set, in Matrix4x4 world)
```

#### Parameters

`set` [TriangleSet](Cube.Core.TriangleSet.md)

The set to add to.

`world` [Matrix4x4](https://learn.microsoft.com/dotnet/api/system.numerics.matrix4x4)

The collider's world matrix.

### <a id="Cube_Core_PrimitiveMeshes_AddCylinder_Cube_Core_TriangleSet_System_Numerics_Matrix4x4__"></a> AddCylinder\(TriangleSet, in Matrix4x4\)

Adds a cylinder collider's triangles: a prism with a fan on each cap.

```csharp
public static void AddCylinder(TriangleSet set, in Matrix4x4 world)
```

#### Parameters

`set` [TriangleSet](Cube.Core.TriangleSet.md)

The set to add to.

`world` [Matrix4x4](https://learn.microsoft.com/dotnet/api/system.numerics.matrix4x4)

The collider's world matrix.

### <a id="Cube_Core_PrimitiveMeshes_AddPlane_Cube_Core_TriangleSet_System_Numerics_Matrix4x4__"></a> AddPlane\(TriangleSet, in Matrix4x4\)

Adds a plane collider's 2 triangles (a flat square in its own X and Y).

```csharp
public static void AddPlane(TriangleSet set, in Matrix4x4 world)
```

#### Parameters

`set` [TriangleSet](Cube.Core.TriangleSet.md)

The set to add to.

`world` [Matrix4x4](https://learn.microsoft.com/dotnet/api/system.numerics.matrix4x4)

The collider's world matrix.

### <a id="Cube_Core_PrimitiveMeshes_AddSphere_Cube_Core_TriangleSet_System_Numerics_Matrix4x4__"></a> AddSphere\(TriangleSet, in Matrix4x4\)

Adds a sphere collider's [SphereTriangles](Cube.Core.PrimitiveMeshes.md#Cube_Core_PrimitiveMeshes_SphereTriangles) triangles.

```csharp
public static void AddSphere(TriangleSet set, in Matrix4x4 world)
```

#### Parameters

`set` [TriangleSet](Cube.Core.TriangleSet.md)

The set to add to.

`world` [Matrix4x4](https://learn.microsoft.com/dotnet/api/system.numerics.matrix4x4)

The collider's world matrix.

### <a id="Cube_Core_PrimitiveMeshes_RingPoint_System_Int32_System_Single_System_Numerics_Matrix4x4__"></a> RingPoint\(int, float, in Matrix4x4\)

A point on a cylinder's rim, in the world.

```csharp
private static Vector3 RingPoint(int index, float y, in Matrix4x4 world)
```

#### Parameters

`index` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Which of [CylinderSides](Cube.Core.PrimitiveMeshes.md#Cube_Core_PrimitiveMeshes_CylinderSides) points round the rim.

`y` [float](https://learn.microsoft.com/dotnet/api/system.single)

1 for the top rim, -1 for the bottom.

`world` [Matrix4x4](https://learn.microsoft.com/dotnet/api/system.numerics.matrix4x4)

The collider's world matrix.

#### Returns

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)
