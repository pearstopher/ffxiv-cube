# <a id="Cube_Core_PlayerPalette"></a> Struct PlayerPalette

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

One colour per relation (PROJECT.md, "Colours": self, party/alliance members, everyone else, NPCs; one colour
each). Built from the user's [CubeSettings](Cube.Core.CubeSettings.md#Cube_Core_CubeSettings).

```csharp
public readonly record struct PlayerPalette : IEquatable<PlayerPalette>
```

#### Implements

[IEquatable<PlayerPalette\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)


## Constructors

### <a id="Cube_Core_PlayerPalette__ctor_System_UInt32_System_UInt32_System_UInt32_System_UInt32_"></a> PlayerPalette\(uint, uint, uint, uint\)

One colour per relation (PROJECT.md, "Colours": self, party/alliance members, everyone else, NPCs; one colour
each). Built from the user's [CubeSettings](Cube.Core.CubeSettings.md#Cube_Core_CubeSettings).

```csharp
public PlayerPalette(uint Self, uint Party, uint Other, uint Npc)
```

#### Parameters

`Self` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

The local player's colour.

`Party` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

Party and alliance members' colour.

`Other` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

Every other player's colour.

`Npc` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

Named NPCs' colour.

## Properties

### <a id="Cube_Core_PlayerPalette_Default"></a> Default

Defaults: blue for self, green for party/alliance, orange for others, purple for NPCs, fully opaque, chosen to
stay distinct from each other and from the grey wireframe ([DefaultSelfColour](Cube.Core.CubeSettings.md#Cube_Core_CubeSettings_DefaultSelfColour) and
siblings).

```csharp
public static PlayerPalette Default { get; }
```

#### Property Value

 [PlayerPalette](Cube.Core.PlayerPalette.md)

### <a id="Cube_Core_PlayerPalette_Npc"></a> Npc

Named NPCs' colour.

```csharp
public uint Npc { get; init; }
```

#### Property Value

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

### <a id="Cube_Core_PlayerPalette_Other"></a> Other

Every other player's colour.

```csharp
public uint Other { get; init; }
```

#### Property Value

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

### <a id="Cube_Core_PlayerPalette_Party"></a> Party

Party and alliance members' colour.

```csharp
public uint Party { get; init; }
```

#### Property Value

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

### <a id="Cube_Core_PlayerPalette_Self"></a> Self

The local player's colour.

```csharp
public uint Self { get; init; }
```

#### Property Value

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

## Methods

### <a id="Cube_Core_PlayerPalette_For_Cube_Core_PlayerRelation_"></a> For\(PlayerRelation\)

The colour for a relation.

```csharp
public uint For(PlayerRelation relation)
```

#### Parameters

`relation` [PlayerRelation](Cube.Core.PlayerRelation.md)

Self, party, other or NPC.

#### Returns

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)
