# <a id="Cube_TestBridge_FakePlayers_Fake"></a> Struct FakePlayers.Fake

Namespace: [Cube.TestBridge](Cube.TestBridge.md)  
Assembly: Cube.dll  

One fake player or NPC.

```csharp
private readonly record struct FakePlayers.Fake : IEquatable<FakePlayers.Fake>
```

#### Implements

[IEquatable<FakePlayers.Fake\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)


## Constructors

### <a id="Cube_TestBridge_FakePlayers_Fake__ctor_System_Byte___System_Numerics_Vector3_Cube_Core_PlayerRelation_System_Boolean_System_Boolean_System_Boolean_"></a> Fake\(byte\[\], Vector3, PlayerRelation, bool, bool, bool\)

One fake player or NPC.

```csharp
public Fake(byte[] Name, Vector3 Offset, PlayerRelation Relation, bool IsWorld, bool IsTarget, bool IsAttacker)
```

#### Parameters

`Name` [byte](https://learn.microsoft.com/dotnet/api/system.byte)\[\]

UTF-8 name bytes, encoded once.

`Offset` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

Position as fractions of the half-size on each axis in cube-local coordinates, or a world position if <code class="paramref">IsWorld</code>.

`Relation` [PlayerRelation](Cube.Core.PlayerRelation.md)

Party, other or NPC.

`IsWorld` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether <code class="paramref">Offset</code> is a fixed world position.

`IsTarget` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether it stands in for the current target (name contains "Target"; #111).

`IsAttacker` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether it stands in for an attacking enemy (name contains "Attacker"; #111).

## Properties

### <a id="Cube_TestBridge_FakePlayers_Fake_IsAttacker"></a> IsAttacker

Whether it stands in for an attacking enemy (name contains "Attacker"; #111).

```csharp
public bool IsAttacker { get; init; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_TestBridge_FakePlayers_Fake_IsTarget"></a> IsTarget

Whether it stands in for the current target (name contains "Target"; #111).

```csharp
public bool IsTarget { get; init; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_TestBridge_FakePlayers_Fake_IsWorld"></a> IsWorld

Whether <code class="paramref">Offset</code> is a fixed world position.

```csharp
public bool IsWorld { get; init; }
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

Position as fractions of the half-size on each axis in cube-local coordinates, or a world position if <code class="paramref">IsWorld</code>.

```csharp
public Vector3 Offset { get; init; }
```

#### Property Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_TestBridge_FakePlayers_Fake_Relation"></a> Relation

Party, other or NPC.

```csharp
public PlayerRelation Relation { get; init; }
```

#### Property Value

 [PlayerRelation](Cube.Core.PlayerRelation.md)
