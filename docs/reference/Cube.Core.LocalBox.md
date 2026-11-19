# <a id="Cube_Core_LocalBox"></a> Struct LocalBox

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

An axis-aligned box in a mesh's own (local) space: a triangle's bounds, or one piece of a split node.

```csharp
public readonly record struct LocalBox : IEquatable<LocalBox>
```

#### Implements

[IEquatable<LocalBox\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)


## Constructors

### <a id="Cube_Core_LocalBox__ctor_System_Numerics_Vector3_System_Numerics_Vector3_"></a> LocalBox\(Vector3, Vector3\)

An axis-aligned box in a mesh's own (local) space: a triangle's bounds, or one piece of a split node.

```csharp
public LocalBox(Vector3 Min, Vector3 Max)
```

#### Parameters

`Min` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The corner with the smallest X, Y and Z.

`Max` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The corner with the largest X, Y and Z.

## Properties

### <a id="Cube_Core_LocalBox_Max"></a> Max

The corner with the largest X, Y and Z.

```csharp
public Vector3 Max { get; init; }
```

#### Property Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_LocalBox_Min"></a> Min

The corner with the smallest X, Y and Z.

```csharp
public Vector3 Min { get; init; }
```

#### Property Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)
