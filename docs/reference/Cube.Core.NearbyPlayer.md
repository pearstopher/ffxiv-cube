# <a id="Cube_Core_NearbyPlayer"></a> Struct NearbyPlayer

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

One player inside the cube this frame: where they are, how they relate to the local player, and their
(cached) name for the label.

```csharp
public readonly record struct NearbyPlayer : IEquatable<NearbyPlayer>
```

#### Implements

[IEquatable<NearbyPlayer\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)


## Constructors

### <a id="Cube_Core_NearbyPlayer__ctor_System_Int32_System_UInt32_System_Numerics_Vector3_Cube_Core_PlayerRelation_System_String_"></a> NearbyPlayer\(int, uint, Vector3, PlayerRelation, string\)

One player inside the cube this frame: where they are, how they relate to the local player, and their
(cached) name for the label.

```csharp
public NearbyPlayer(int Slot, uint EntityId, Vector3 Position, PlayerRelation Relation, string Name)
```

#### Parameters

`Slot` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Object table index the player was read from; keys the name cache.

`EntityId` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

The game's id for the player, used by the bridge to compare with an independent read.

`Position` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

World position in yalms.

`Relation` [PlayerRelation](Cube.Core.PlayerRelation.md)

Self, party/alliance or other.

`Name` [string](https://learn.microsoft.com/dotnet/api/system.string)

The player's name, shared from [NameCache](Cube.Core.NameCache.md#Cube_Core_NameCache): only drawn on screen, never stored or sent (PROJECT.md, "Privacy").

## Properties

### <a id="Cube_Core_NearbyPlayer_EntityId"></a> EntityId

The game's id for the player, used by the bridge to compare with an independent read.

```csharp
public uint EntityId { get; init; }
```

#### Property Value

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

### <a id="Cube_Core_NearbyPlayer_Name"></a> Name

The player's name, shared from [NameCache](Cube.Core.NameCache.md#Cube_Core_NameCache): only drawn on screen, never stored or sent (PROJECT.md, "Privacy").

```csharp
public string Name { get; init; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Cube_Core_NearbyPlayer_Position"></a> Position

World position in yalms.

```csharp
public Vector3 Position { get; init; }
```

#### Property Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_NearbyPlayer_Relation"></a> Relation

Self, party/alliance or other.

```csharp
public PlayerRelation Relation { get; init; }
```

#### Property Value

 [PlayerRelation](Cube.Core.PlayerRelation.md)

### <a id="Cube_Core_NearbyPlayer_Slot"></a> Slot

Object table index the player was read from; keys the name cache.

```csharp
public int Slot { get; init; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)
