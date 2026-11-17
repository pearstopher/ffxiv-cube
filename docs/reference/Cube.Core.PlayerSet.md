# <a id="Cube_Core_PlayerSet"></a> Class PlayerSet

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

The players inside the cube this frame (PROJECT.md, "Players": every player character in the object table whose
position is inside the cube; players outside are not shown at all). The game reader offers every player it
finds; this class culls, classifies and names them.

```csharp
public sealed class PlayerSet
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[PlayerSet](Cube.Core.PlayerSet.md)


## Remarks

Keeping the rules here, not in the game reader, means they are unit tested without the game, and the Debug
fake players (#16) go through exactly the same path. The buffer is fixed-size and reused (PROJECT.md,
"Efficiency and memory"); a full set drops extra players and counts them in [Dropped](Cube.Core.PlayerSet.md#Cube_Core_PlayerSet_Dropped).

## Constructors

### <a id="Cube_Core_PlayerSet__ctor_System_Int32_Cube_Core_NameCache_"></a> PlayerSet\(int, NameCache\)

Creates an empty set.

```csharp
public PlayerSet(int capacity, NameCache names)
```

#### Parameters

`capacity` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Most players held per frame.

`names` [NameCache](Cube.Core.NameCache.md)

The name cache; its slot count bounds the slots that can be offered.

## Fields

### <a id="Cube_Core_PlayerSet_names"></a> names

Names, cached per slot.

```csharp
private readonly NameCache names
```

#### Field Value

 [NameCache](Cube.Core.NameCache.md)

### <a id="Cube_Core_PlayerSet_players"></a> players

The reused buffer.

```csharp
private readonly NearbyPlayer[] players
```

#### Field Value

 [NearbyPlayer](Cube.Core.NearbyPlayer.md)\[\]

### <a id="Cube_Core_PlayerSet_space"></a> space

The cube for this frame, set by [Begin](Cube.Core.PlayerSet.md#Cube_Core_PlayerSet_Begin).

```csharp
private CubeSpace space
```

#### Field Value

 [CubeSpace](Cube.Core.CubeSpace.md)

## Properties

### <a id="Cube_Core_PlayerSet_Count"></a> Count

How many players are inside the cube this frame.

```csharp
public int Count { get; private set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_PlayerSet_Dropped"></a> Dropped

How many players inside the cube didn't fit this frame.

```csharp
public int Dropped { get; private set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_PlayerSet_Items"></a> Items

The players inside the cube this frame.

```csharp
public ReadOnlySpan<NearbyPlayer> Items { get; }
```

#### Property Value

 [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[NearbyPlayer](Cube.Core.NearbyPlayer.md)\>

### <a id="Cube_Core_PlayerSet_Names"></a> Names

The name cache, for the bridge's checks.

```csharp
public NameCache Names { get; }
```

#### Property Value

 [NameCache](Cube.Core.NameCache.md)

### <a id="Cube_Core_PlayerSet_Offered"></a> Offered

How many players were offered this frame, inside the cube or not.

```csharp
public int Offered { get; private set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Methods

### <a id="Cube_Core_PlayerSet_Begin_Cube_Core_CubeSpace__"></a> Begin\(in CubeSpace\)

Starts a new frame: empties the set and sets the cube players are tested against.

```csharp
public void Begin(in CubeSpace cube)
```

#### Parameters

`cube` [CubeSpace](Cube.Core.CubeSpace.md)

This frame's cube.

### <a id="Cube_Core_PlayerSet_Classify_System_Boolean_System_Boolean_System_Boolean_"></a> Classify\(bool, bool, bool\)

The relation that picks a player's colour (PROJECT.md, "Colours"). Self wins over party: the local player
is always drawn in its own colour.

```csharp
public static PlayerRelation Classify(bool isLocal, bool isParty, bool isAlliance)
```

#### Parameters

`isLocal` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether this is the local player.

`isParty` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

The game's party-member flag.

`isAlliance` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

The game's alliance-member flag.

#### Returns

 [PlayerRelation](Cube.Core.PlayerRelation.md)

### <a id="Cube_Core_PlayerSet_Offer_System_Int32_System_UInt32_System_Numerics_Vector3_System_Boolean_System_Boolean_System_Boolean_System_ReadOnlySpan_System_Byte__"></a> Offer\(int, uint, Vector3, bool, bool, bool, ReadOnlySpan<byte\>\)

Offers one player. Outside the cube: rejected before anything else is done (no name lookup).
Inside: classified, named from the cache and kept.

```csharp
public bool Offer(int slot, uint entityId, Vector3 position, bool isLocal, bool isParty, bool isAlliance, ReadOnlySpan<byte> nameUtf8)
```

#### Parameters

`slot` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Object table index (must be below the name cache's slot count).

`entityId` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

The game's id for the player.

`position` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

World position.

`isLocal` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether this is the local player.

`isParty` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Party-member flag.

`isAlliance` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Alliance-member flag.

`nameUtf8` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

The game's name bytes.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

True if the player is inside the cube and was kept.
