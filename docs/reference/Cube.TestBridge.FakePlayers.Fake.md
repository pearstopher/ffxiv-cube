# <a id="Cube_TestBridge_FakePlayers_Fake"></a> Struct FakePlayers.Fake

Namespace: [Cube.TestBridge](Cube.TestBridge.md)  
Assembly: Cube.dll  

One fake player.

```csharp
private readonly record struct FakePlayers.Fake : IEquatable<FakePlayers.Fake>
```

#### Implements

[IEquatable<FakePlayers.Fake\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)


## Constructors

### <a id="Cube_TestBridge_FakePlayers_Fake__ctor_System_Byte___System_Numerics_Vector3_System_Boolean_"></a> Fake\(byte\[\], Vector3, bool\)

One fake player.

```csharp
public Fake(byte[] Name, Vector3 Offset, bool IsParty)
```

#### Parameters

`Name` [byte](https://learn.microsoft.com/dotnet/api/system.byte)\[\]

UTF-8 name bytes, encoded once.

`Offset` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

Position as fractions of the half-edge.

`IsParty` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether it is flagged as a party member.

## Properties

### <a id="Cube_TestBridge_FakePlayers_Fake_IsParty"></a> IsParty

Whether it is flagged as a party member.

```csharp
public bool IsParty { get; init; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_TestBridge_FakePlayers_Fake_Name"></a> Name

UTF-8 name bytes, encoded once.

```csharp
public byte[] Name { get; init; }
```

#### Property Value

 [byte](https://learn.microsoft.com/dotnet/api/system.byte)\[\]

### <a id="Cube_TestBridge_FakePlayers_Fake_Offset"></a> Offset

Position as fractions of the half-edge.

```csharp
public Vector3 Offset { get; init; }
```

#### Property Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)
