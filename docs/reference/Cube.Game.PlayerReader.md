# <a id="Cube_Game_PlayerReader"></a> Class PlayerReader

Namespace: [Cube.Game](Cube.Game.md)  
Assembly: Cube.dll  

Reads the local player, the players around them and the named NPCs straight from the game's object manager and
offers each to [PlayerSet](Cube.Core.PlayerSet.md#Cube_Core_PlayerSet) (PROJECT.md, "Players" and "NPCs"). The approach chosen in #10
(<code>docs/research-notes.md</code>): FFXIVClientStructs, not Dalamud's object table, because it allocates nothing per
frame.

```csharp
internal static class PlayerReader
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[PlayerReader](Cube.Game.PlayerReader.md)


## Remarks

Players and battle NPCs live in the 200 character slots; event NPCs live above them (#73), so the whole table is
walked only when NPCs are shown. Culling, colour relation and name caching happen in Core. A plain memory read: no
hooks, no game functions, nothing sent.

## Fields

### <a id="Cube_Game_PlayerReader_CharacterSlots"></a> CharacterSlots

The object table slots holding characters (0–199).

```csharp
public const int CharacterSlots = 200
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Game_PlayerReader_MaxInteractables"></a> MaxInteractables

The most text-labelled interactable objects drawn per frame (#112). A housing ward put a few dozen entrances and
placards in range; the cap keeps the draw cost bounded and extras are dropped and counted.

```csharp
public const int MaxInteractables = 30
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Game_PlayerReader_MaxNpcs"></a> MaxNpcs

The most named NPCs drawn per frame (#73: a busy city put 22 in the largest cube; 50 players' worth of extra
drawing still fits the 0.5 ms budget alongside a full crowd of players). Extras are dropped and counted.

```csharp
public const int MaxNpcs = 50
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Game_PlayerReader_MaxPlayers"></a> MaxPlayers

The most players that can be around at once: one per even character slot (odd slots hold their mounts and
minions; <code>docs/research-notes.md</code>, #10). The efficiency checks measure at this cap.

```csharp
public const int MaxPlayers = 100
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Game_PlayerReader_ObjectSlots"></a> ObjectSlots

The length of the game's index-sorted object table, measured in #73 (<code>docs/research-notes.md</code>). The name
cache has one slot per table index, so its size is fixed at load (PROJECT.md: bounded memory).

```csharp
public const int ObjectSlots = 819
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Methods

### <a id="Cube_Game_PlayerReader_IsHater_FFXIVClientStructs_FFXIV_Client_Game_UI_UIState__System_UInt32_"></a> IsHater\(UIState\*, uint\)

Whether an entity is on the game's list of enemies attacking the player (the "hater" list the enmity UI uses).

```csharp
private static bool IsHater(UIState* state, uint entityId)
```

#### Parameters

`state` UIState\*

The game's UI state; null is treated as an empty list.

`entityId` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

The enemy's entity id.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Game_PlayerReader_Read_Cube_Core_PlayerSet_System_Single_"></a> Read\(PlayerSet, float\)

Offers every player character, and every named NPC if the set shows NPCs, to the set; the set keeps those
inside the cube.

```csharp
public static void Read(PlayerSet set, float localY)
```

#### Parameters

`set` [PlayerSet](Cube.Core.PlayerSet.md)

This frame's player set, already begun with the frame's cube.

`localY` [float](https://learn.microsoft.com/dotnet/api/system.single)

The local player's height this frame, as [CubeWindow](Cube.CubeWindow.md#Cube_CubeWindow) read it. Used instead of the object's own so the
marker and the cube agree on the same frame's value (and so the bridge's simulated jump can move the marker, #105).

#### Remarks

With NPCs off (PROJECT.md, "NPCs": the setting; [ShowsNpcs](Cube.Core.PlayerSet.md#Cube_Core_PlayerSet_ShowsNpcs)) only the character slots
are read and no NPC is looked at.

### <a id="Cube_Game_PlayerReader_TryGetLocalPosition_System_Numerics_Vector3__"></a> TryGetLocalPosition\(out Vector3\)

Gets the local player's world position: the cube's centre.

```csharp
public static bool TryGetLocalPosition(out Vector3 position)
```

#### Parameters

`position` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The position, if logged in.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

False when there is no local player (logged out, title screen, some loading screens).
