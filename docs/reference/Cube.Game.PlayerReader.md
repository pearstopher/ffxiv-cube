# <a id="Cube_Game_PlayerReader"></a> Class PlayerReader

Namespace: [Cube.Game](Cube.Game.md)  
Assembly: Cube.dll  

Reads the local player and the players around them straight from the game's object manager and offers each to
[PlayerSet](Cube.Core.PlayerSet.md#Cube_Core_PlayerSet) (PROJECT.md, "Players"). The approach chosen in #10 (<code>docs/research-notes.md</code>):
FFXIVClientStructs, not Dalamud's object table, because it allocates nothing per frame.

```csharp
internal static class PlayerReader
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[PlayerReader](Cube.Game.PlayerReader.md)


## Remarks

Only the 200 character slots are scanned; players never live above them. Culling, colour relation and name
caching happen in Core. A plain memory read: no hooks, no game functions, nothing sent.

## Fields

### <a id="Cube_Game_PlayerReader_CharacterSlots"></a> CharacterSlots

The object table slots holding characters (0–199).

```csharp
public const int CharacterSlots = 200
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Methods

### <a id="Cube_Game_PlayerReader_Read_Cube_Core_PlayerSet_"></a> Read\(PlayerSet\)

Offers every player character in the object table to the set; the set keeps those inside the cube.

```csharp
public static void Read(PlayerSet set)
```

#### Parameters

`set` [PlayerSet](Cube.Core.PlayerSet.md)

This frame's player set, already begun with the frame's cube.

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
