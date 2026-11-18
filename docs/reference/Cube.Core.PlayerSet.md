# <a id="Cube_Core_PlayerSet"></a> Class PlayerSet

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

The players and named NPCs inside the cube this frame (PROJECT.md, "Players": every player character in the
object table whose position is inside the cube; "NPCs": every named NPC inside the cube, shown the same way;
anyone outside is not shown at all). The game reader offers everyone it finds; this class culls, classifies and
names them.

```csharp
public sealed class PlayerSet
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[PlayerSet](Cube.Core.PlayerSet.md)


## Remarks

<p>
Keeping the rules here, not in the game reader, means they are unit tested without the game, and the Debug
fake players (#16) go through exactly the same path. The buffer is fixed-size and reused (PROJECT.md,
"Efficiency and memory"); a full set drops extras and counts them in [Dropped](Cube.Core.PlayerSet.md#Cube_Core_PlayerSet_Dropped).
</p>
<p>
Players and NPCs share one buffer, so they share the culling, ordering and label code downstream, but each has its
own cap: a crowd of NPCs can never push a player out, and NPCs can't push the frame past the draw budget
(<code>docs/research-notes.md</code>, #73: NPC cap 50).
</p>

## Constructors

### <a id="Cube_Core_PlayerSet__ctor_System_Int32_Cube_Core_NameCache_System_Int32_"></a> PlayerSet\(int, NameCache, int\)

Creates an empty set.

```csharp
public PlayerSet(int capacity, NameCache names, int npcCapacity = 0)
```

#### Parameters

`capacity` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Most players held per frame.

`names` [NameCache](Cube.Core.NameCache.md)

The name cache; its slot count bounds the slots that can be offered.

`npcCapacity` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Most named NPCs held per frame, on top of the players; 0 keeps none.

## Fields

### <a id="Cube_Core_PlayerSet_NoOwner"></a> NoOwner

The game's "no owner" entity id (<code>0xE0000000</code>), which <code>GameObject.OwnerId</code> holds for objects nobody
owns (<code>docs/research-notes.md</code>, #73).

```csharp
public const uint NoOwner = 3758096384
```

#### Field Value

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

### <a id="Cube_Core_PlayerSet_markers"></a> markers

Label text with the target and attacking markers (#111), cached per slot.

```csharp
private readonly MarkerLabels markers
```

#### Field Value

 [MarkerLabels](Cube.Core.MarkerLabels.md)

### <a id="Cube_Core_PlayerSet_names"></a> names

Names, cached per slot.

```csharp
private readonly NameCache names
```

#### Field Value

 [NameCache](Cube.Core.NameCache.md)

### <a id="Cube_Core_PlayerSet_npcCapacity"></a> npcCapacity

Most NPCs kept per frame.

```csharp
private readonly int npcCapacity
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_PlayerSet_playerCapacity"></a> playerCapacity

Most players kept per frame.

```csharp
private readonly int playerCapacity
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_PlayerSet_playerCount"></a> playerCount

How many players (not NPCs) were kept this frame.

```csharp
private int playerCount
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_PlayerSet_players"></a> players

The reused buffer: players and NPCs, in the order offered.

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

How many players and NPCs are inside the cube this frame.

```csharp
public int Count { get; private set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_PlayerSet_Dropped"></a> Dropped

How many players or NPCs inside the cube didn't fit this frame.

```csharp
public int Dropped { get; private set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_PlayerSet_Items"></a> Items

The players and NPCs inside the cube this frame.

```csharp
public ReadOnlySpan<NearbyPlayer> Items { get; }
```

#### Property Value

 [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[NearbyPlayer](Cube.Core.NearbyPlayer.md)\>

### <a id="Cube_Core_PlayerSet_Markers"></a> Markers

The marker cache, for tests and the bridge.

```csharp
public MarkerLabels Markers { get; }
```

#### Property Value

 [MarkerLabels](Cube.Core.MarkerLabels.md)

### <a id="Cube_Core_PlayerSet_Names"></a> Names

The name cache, for the bridge's checks.

```csharp
public NameCache Names { get; }
```

#### Property Value

 [NameCache](Cube.Core.NameCache.md)

### <a id="Cube_Core_PlayerSet_NpcCount"></a> NpcCount

How many of [Count](Cube.Core.PlayerSet.md#Cube_Core_PlayerSet_Count) are NPCs.

```csharp
public int NpcCount { get; private set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_PlayerSet_Offered"></a> Offered

How many players and NPCs were offered this frame, inside the cube or not.

```csharp
public int Offered { get; private set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_PlayerSet_ShowsNpcs"></a> ShowsNpcs

Whether NPCs are taken this frame (PROJECT.md, "NPCs": the on/off setting). The game reader checks it to skip
the NPC part of the object table altogether; [OfferNpc](Cube.Core.PlayerSet.md#Cube_Core_PlayerSet_OfferNpc) rejects every NPC while it's off.

```csharp
public bool ShowsNpcs { get; private set; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Methods

### <a id="Cube_Core_PlayerSet_Accept_System_Int32_System_Numerics_Vector3_System_Boolean_"></a> Accept\(int, Vector3, bool\)

The shared first steps of an offer: counts it, culls it, and counts it as dropped if its kind's cap is full.
Culling comes first, so someone outside the cube costs no name lookup.

```csharp
private bool Accept(int slot, Vector3 position, bool full)
```

#### Parameters

`slot` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Object table index.

`position` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

World position.

`full` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether this kind's cap is already reached.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

True if the offer should be kept.

### <a id="Cube_Core_PlayerSet_Begin_Cube_Core_CubeSpace__System_Boolean_"></a> Begin\(in CubeSpace, bool\)

Starts a new frame: empties the set and sets the cube players are tested against.

```csharp
public void Begin(in CubeSpace cube, bool npcs = true)
```

#### Parameters

`cube` [CubeSpace](Cube.Core.CubeSpace.md)

This frame's cube.

`npcs` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether named NPCs are shown this frame (the user's setting).

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

### <a id="Cube_Core_PlayerSet_ClassifyNpc_System_Boolean_System_Boolean_"></a> ClassifyNpc\(bool, bool\)

The relation for a named NPC (#111): a hostile battle NPC is an enemy, anything else a plain NPC. Event NPCs are
never enemies, whatever their flags say.

```csharp
public static PlayerRelation ClassifyNpc(bool isBattleNpc, bool isHostile)
```

#### Parameters

`isBattleNpc` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether the object is a battle NPC (not an event NPC).

`isHostile` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

The game's hostile flag (<code>Character.IsHostile</code>).

#### Returns

 [PlayerRelation](Cube.Core.PlayerRelation.md)

### <a id="Cube_Core_PlayerSet_IsNamedNpc_System_UInt32_System_Boolean_System_ReadOnlySpan_System_Byte__"></a> IsNamedNpc\(uint, bool, ReadOnlySpan<byte\>\)

Whether an event or battle NPC counts as a named NPC (PROJECT.md, "NPCs"; the filter measured in #73,
<code>docs/research-notes.md</code>).

```csharp
public static bool IsNamedNpc(uint ownerId, bool isPetOrBuddy, ReadOnlySpan<byte> nameUtf8)
```

#### Parameters

`ownerId` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

The object's <code>OwnerId</code>; anything but [NoOwner](Cube.Core.PlayerSet.md#Cube_Core_PlayerSet_NoOwner) means it is owned.

`isPetOrBuddy` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether it is a battle NPC of sub-kind <code>Pet</code> or <code>Buddy</code>.

`nameUtf8` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

The game's zero-terminated name bytes.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

#### Remarks

Objects owned by a player (pets, chocobo companions) are left out by both their owner and their battle NPC
sub-kind, so either field being unset still leaves them out. Nameless NPCs are the game's invisible event
helpers. The caller has already checked the object kind is an event or battle NPC.

### <a id="Cube_Core_PlayerSet_Offer_System_Int32_System_UInt32_System_Numerics_Vector3_System_Boolean_System_Boolean_System_Boolean_System_ReadOnlySpan_System_Byte__System_Boolean_"></a> Offer\(int, uint, Vector3, bool, bool, bool, ReadOnlySpan<byte\>, bool\)

Offers one player. Outside the cube: rejected before anything else is done (no name lookup).
Inside: classified, named from the cache and kept.

```csharp
public bool Offer(int slot, uint entityId, Vector3 position, bool isLocal, bool isParty, bool isAlliance, ReadOnlySpan<byte> nameUtf8, bool isTarget = false)
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

`isTarget` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether this is the local player's current target (#111: gets a <code>[T]</code> marker).

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

True if the player is inside the cube and was kept.

### <a id="Cube_Core_PlayerSet_OfferNpc_System_Int32_System_UInt32_System_Numerics_Vector3_System_ReadOnlySpan_System_Byte__System_Boolean_System_Boolean_System_Boolean_"></a> OfferNpc\(int, uint, Vector3, ReadOnlySpan<byte\>, bool, bool, bool\)

Offers one named NPC (the caller has applied [IsNamedNpc](Cube.Core.PlayerSet.md#Cube_Core_PlayerSet_IsNamedNpc)). It goes through the same culling as a
player and is kept with the [Npc](Cube.Core.PlayerRelation.md#Cube_Core_PlayerRelation_Npc) relation, up to the NPC cap. With NPCs turned off
([ShowsNpcs](Cube.Core.PlayerSet.md#Cube_Core_PlayerSet_ShowsNpcs)) it is rejected straight away and not even counted as offered.

```csharp
public bool OfferNpc(int slot, uint entityId, Vector3 position, ReadOnlySpan<byte> nameUtf8, bool isEnemy = false, bool isTarget = false, bool isAttacking = false)
```

#### Parameters

`slot` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Object table index (must be below the name cache's slot count).

`entityId` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

The game's id for the NPC.

`position` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

World position.

`nameUtf8` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

The game's name bytes.

`isEnemy` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether it is a hostile battle NPC (#111: kept as [Enemy](Cube.Core.PlayerRelation.md#Cube_Core_PlayerRelation_Enemy)).

`isTarget` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether it is the local player's current target (<code>[T]</code> marker).

`isAttacking` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether it is attacking the local player (<code>[!]</code> marker); only enemies are marked.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

True if the NPC is inside the cube and was kept.
