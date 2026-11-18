# <a id="Cube_Core_CollisionClipper_Footprint"></a> Struct CollisionClipper.Footprint

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

The cube's rectangular footprint in world X/Z (stored as `System.Numerics.Vector2` X and Y), with its bounds for the
cheap reject. Computed once per clip pass.

```csharp
private readonly struct CollisionClipper.Footprint
```


## Constructors

### <a id="Cube_Core_CollisionClipper_Footprint__ctor_Cube_Core_CubeSpace__"></a> Footprint\(in CubeSpace\)

Works out the four corners from the cube's alignment.

```csharp
public Footprint(in CubeSpace cube)
```

#### Parameters

`cube` [CubeSpace](Cube.Core.CubeSpace.md)

The cube.

## Properties

### <a id="Cube_Core_CollisionClipper_Footprint_C0"></a> C0

First corner, in order around the rectangle.

```csharp
public Vector2 C0 { get; }
```

#### Property Value

 [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

### <a id="Cube_Core_CollisionClipper_Footprint_C1"></a> C1

Second corner.

```csharp
public Vector2 C1 { get; }
```

#### Property Value

 [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

### <a id="Cube_Core_CollisionClipper_Footprint_C2"></a> C2

Third corner.

```csharp
public Vector2 C2 { get; }
```

#### Property Value

 [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

### <a id="Cube_Core_CollisionClipper_Footprint_C3"></a> C3

Fourth corner.

```csharp
public Vector2 C3 { get; }
```

#### Property Value

 [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

### <a id="Cube_Core_CollisionClipper_Footprint_IsAxisAligned"></a> IsAxisAligned

Whether the footprint's sides run along the world axes (the yaw is a multiple of 90°).

```csharp
public bool IsAxisAligned { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_CollisionClipper_Footprint_Max"></a> Max

Highest world X and Z of the footprint.

```csharp
public Vector2 Max { get; }
```

#### Property Value

 [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

### <a id="Cube_Core_CollisionClipper_Footprint_Min"></a> Min

Lowest world X and Z of the footprint.

```csharp
public Vector2 Min { get; }
```

#### Property Value

 [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

## Methods

### <a id="Cube_Core_CollisionClipper_Footprint_Flat_System_Numerics_Vector3_"></a> Flat\(Vector3\)

A world point's X and Z.

```csharp
private static Vector2 Flat(Vector3 world)
```

#### Parameters

`world` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The point.

#### Returns

 [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)
