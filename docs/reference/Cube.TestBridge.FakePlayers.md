# <a id="Cube_TestBridge_FakePlayers"></a> Class FakePlayers

Namespace: [Cube.TestBridge](Cube.TestBridge.md)  
Assembly: Cube.dll  

Synthetic players and NPCs for in-game checks (PROJECT.md, "Automated testing": the bridge can inject synthetic
players so rendering checks work even when nobody is nearby). Debug builds only: this file lives in
<code>TestBridge/</code>, which Release builds exclude.

```csharp
internal sealed class FakePlayers
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[FakePlayers](Cube.TestBridge.FakePlayers.md)


## Remarks

Fakes are offered to the same [PlayerSet](Cube.Core.PlayerSet.md#Cube_Core_PlayerSet) as real players and NPCs, after them, so they go through
exactly the same culling, classification and naming; fake NPCs use [OfferNpc](Cube.Core.PlayerSet.md#Cube_Core_PlayerSet_OfferNpc), so they are
left out when the NPC setting is off, like real ones. They use object slots from [FirstSlot](Cube.TestBridge.FakePlayers.md#Cube_TestBridge_FakePlayers_FirstSlot) up (above the whole
real object table) and entity ids from [FirstEntityId](Cube.TestBridge.FakePlayers.md#Cube_TestBridge_FakePlayers_FirstEntityId) up, so checks can tell them apart. Positions are
fractions of the cube's half-edge in cube-local coordinates (see [FromView](Cube.Core.CubeSpace.md#Cube_Core_CubeSpace_FromView)), so the same set
works at any cube size and stays put in the cube however the camera turns. A fake added with
[AddAtWorld](Cube.TestBridge.FakePlayers.md#Cube_TestBridge_FakePlayers_AddAtWorld) is instead fixed in the world, like a real player, so it rotates with the contents.

## Fields

### <a id="Cube_TestBridge_FakePlayers_FirstEntityId"></a> FirstEntityId

Entity ids for fakes start here; the game never uses this range for players.

```csharp
public const uint FirstEntityId = 4194304000
```

#### Field Value

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

### <a id="Cube_TestBridge_FakePlayers_FirstSlot"></a> FirstSlot

First object slot used by fakes, just above the real object table.

```csharp
public const int FirstSlot = 819
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_FakePlayers_fakes"></a> fakes

The fakes currently enabled; empty means none are injected.

```csharp
private readonly List<FakePlayers.Fake> fakes
```

#### Field Value

 [List](https://learn.microsoft.com/dotnet/api/system.collections.generic.list\-1)<[FakePlayers](Cube.TestBridge.FakePlayers.md).[Fake](Cube.TestBridge.FakePlayers.Fake.md)\>

## Properties

### <a id="Cube_TestBridge_FakePlayers_Count"></a> Count

How many fakes are enabled.

```csharp
public int Count { get; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_FakePlayers_Standard"></a> Standard

The standard set covering the cases PROJECT.md cares about: near the centre, on a face (inside), on a corner
(inside), two overlapping, above and below, one just outside (never shown), and an NPC inside and outside
(PROJECT.md, "NPCs": shown like players, in the NPC colour).

```csharp
public static IReadOnlyList<(string Name, Vector3 Offset, PlayerRelation Relation)> Standard { get; }
```

#### Property Value

 [IReadOnlyList](https://learn.microsoft.com/dotnet/api/system.collections.generic.ireadonlylist\-1)<\([string](https://learn.microsoft.com/dotnet/api/system.string) [Name](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.string,system.numerics.vector3,cube.core.playerrelation\-.name), [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3) [Offset](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.string,system.numerics.vector3,cube.core.playerrelation\-.offset), [PlayerRelation](Cube.Core.PlayerRelation.md) [Relation](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.string,system.numerics.vector3,cube.core.playerrelation\-.relation)\)\>

### <a id="Cube_TestBridge_FakePlayers_StandardInside"></a> StandardInside

How many of [Standard](Cube.TestBridge.FakePlayers.md#Cube_TestBridge_FakePlayers_Standard) are inside the cube (all but the two named "... Outside").

```csharp
public static int StandardInside { get; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_FakePlayers_StandardNpcsInside"></a> StandardNpcsInside

How many of [Standard](Cube.TestBridge.FakePlayers.md#Cube_TestBridge_FakePlayers_Standard) are NPCs inside the cube.

```csharp
public static int StandardNpcsInside { get; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Methods

### <a id="Cube_TestBridge_FakePlayers_Add_System_String_System_Numerics_Vector3_Cube_Core_PlayerRelation_"></a> Add\(string, Vector3, PlayerRelation\)

Adds one fake.

```csharp
public void Add(string name, Vector3 offset, PlayerRelation relation)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)

Its label.

`offset` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

Position as fractions of the half-edge from the cube centre (1 = on a face).

`relation` [PlayerRelation](Cube.Core.PlayerRelation.md)

Party, other or NPC (fakes are never Self).

### <a id="Cube_TestBridge_FakePlayers_AddAtWorld_System_String_System_Numerics_Vector3_Cube_Core_PlayerRelation_"></a> AddAtWorld\(string, Vector3, PlayerRelation\)

Adds one fake at a fixed world position, like a real player standing still, so it rotates with the cube's
contents when the camera turns (#62).

```csharp
public void AddAtWorld(string name, Vector3 world, PlayerRelation relation)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)

Its label.

`world` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

Its world position.

`relation` [PlayerRelation](Cube.Core.PlayerRelation.md)

Party, other or NPC (fakes are never Self).

### <a id="Cube_TestBridge_FakePlayers_Clear"></a> Clear\(\)

Removes all fakes.

```csharp
public void Clear()
```

### <a id="Cube_TestBridge_FakePlayers_IsOutside_System_String_"></a> IsOutside\(string\)

Whether a standard fake's name marks it as outside the cube.

```csharp
public static bool IsOutside(string name)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)

The fake's name.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_TestBridge_FakePlayers_Offer_Cube_Core_PlayerSet_Cube_Core_CubeSpace__"></a> Offer\(PlayerSet, in CubeSpace\)

Offers every fake to the frame's player set. Allocation-free: names were encoded when added.

```csharp
public void Offer(PlayerSet set, in CubeSpace space)
```

#### Parameters

`set` [PlayerSet](Cube.Core.PlayerSet.md)

The frame's player set, already begun.

`space` [CubeSpace](Cube.Core.CubeSpace.md)

The frame's cube.

### <a id="Cube_TestBridge_FakePlayers_StandardArriving_System_Boolean_"></a> StandardArriving\(bool\)

How many of [Standard](Cube.TestBridge.FakePlayers.md#Cube_TestBridge_FakePlayers_Standard) reach the cube with the NPC setting on or off: the fake NPC is skipped when
it's off, like a real one.

```csharp
public static int StandardArriving(bool npcs)
```

#### Parameters

`npcs` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

The NPC setting.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_FakePlayers_UseCrowd_System_Int32_System_Int32_"></a> UseCrowd\(int, int\)

Enables a crowd of fakes inside the cube, replacing any: by default [MaxPlayers](Cube.Game.PlayerReader.md#Cube_Game_PlayerReader_MaxPlayers)
players and [MaxNpcs](Cube.Game.PlayerReader.md#Cube_Game_PlayerReader_MaxNpcs) NPCs, the most the cube can hold, so the efficiency checks
measure the worst case (#23, #74).

```csharp
public void UseCrowd(int count = 100, int npcs = 50)
```

#### Parameters

`count` [int](https://learn.microsoft.com/dotnet/api/system.int32)

How many fake players, at most [MaxPlayers](Cube.Game.PlayerReader.md#Cube_Game_PlayerReader_MaxPlayers).

`npcs` [int](https://learn.microsoft.com/dotnet/api/system.int32)

How many fake NPCs, at most [MaxNpcs](Cube.Game.PlayerReader.md#Cube_Game_PlayerReader_MaxNpcs).

#### Remarks

Players are spread on a 5×4×5 grid through the cube, alternating party and other; NPCs on a 5×2×5 grid offset
half a step from it. Each has a 21-character name (about the longest the game allows), so every one gets a cube
and a full-length label.

### <a id="Cube_TestBridge_FakePlayers_UseStandardSet"></a> UseStandardSet\(\)

Enables the [Standard](Cube.TestBridge.FakePlayers.md#Cube_TestBridge_FakePlayers_Standard) set, replacing any fakes.

```csharp
public void UseStandardSet()
```
