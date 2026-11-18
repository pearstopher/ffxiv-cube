# <a id="Cube_Core_BoxShades"></a> Struct BoxShades

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

The colours of a box's three visible faces, worked out once per colour per frame ([Shades](Cube.Core.BoxProjector.md#Cube_Core_BoxProjector_Shades)).

```csharp
public readonly record struct BoxShades : IEquatable<BoxShades>
```

#### Implements

[IEquatable<BoxShades\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)


## Constructors

### <a id="Cube_Core_BoxShades__ctor_System_UInt32_System_UInt32_System_UInt32_"></a> BoxShades\(uint, uint, uint\)

The colours of a box's three visible faces, worked out once per colour per frame ([Shades](Cube.Core.BoxProjector.md#Cube_Core_BoxProjector_Shades)).

```csharp
public BoxShades(uint Horizontal, uint EastWest, uint NorthSouth)
```

#### Parameters

`Horizontal` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

The top face's shade (or the bottom's, seen from below).

`EastWest` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

The shade of the faces whose normals point east or west.

`NorthSouth` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

The shade of the faces whose normals point north or south.

## Properties

### <a id="Cube_Core_BoxShades_EastWest"></a> EastWest

The shade of the faces whose normals point east or west.

```csharp
public uint EastWest { get; init; }
```

#### Property Value

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

### <a id="Cube_Core_BoxShades_Horizontal"></a> Horizontal

The top face's shade (or the bottom's, seen from below).

```csharp
public uint Horizontal { get; init; }
```

#### Property Value

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

### <a id="Cube_Core_BoxShades_NorthSouth"></a> NorthSouth

The shade of the faces whose normals point north or south.

```csharp
public uint NorthSouth { get; init; }
```

#### Property Value

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)
