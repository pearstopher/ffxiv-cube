# <a id="Cube_Core_GameState"></a> Struct GameState

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

The game states that decide whether the cube is drawn (PROJECT.md, "When hidden"). Read once per frame by the
plugin from Dalamud's client state and condition flags (<code>docs/research-notes.md</code>, "When to hide").

```csharp
public readonly record struct GameState : IEquatable<GameState>
```

#### Implements

[IEquatable<GameState\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)


## Constructors

### <a id="Cube_Core_GameState__ctor_System_Boolean_System_Boolean_System_Boolean_System_Boolean_System_Boolean_System_Boolean_"></a> GameState\(bool, bool, bool, bool, bool, bool\)

The game states that decide whether the cube is drawn (PROJECT.md, "When hidden"). Read once per frame by the
plugin from Dalamud's client state and condition flags (<code>docs/research-notes.md</code>, "When to hide").

```csharp
public GameState(bool LoggedIn, bool HasLocalPlayer, bool BetweenAreas, bool InCutscene, bool InGPose, bool InPvP)
```

#### Parameters

`LoggedIn` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Logged in to a character (false on the title screen and character select).

`HasLocalPlayer` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

The local player object exists; briefly false while logging in or zoning.

`BetweenAreas` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

A loading screen is up.

`InCutscene` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

A cutscene is playing.

`InGPose` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Group pose is active.

`InPvP` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

In a PvP area (Crystalline Conflict, Frontline, Rival Wings, and the Wolves' Den lobby). The official Dalamud rules
ban any PvP advantage, and the cube would work like a radar there (<code>docs/dalamud-repo-standards.md</code>).

## Properties

### <a id="Cube_Core_GameState_BetweenAreas"></a> BetweenAreas

A loading screen is up.

```csharp
public bool BetweenAreas { get; init; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_GameState_HasLocalPlayer"></a> HasLocalPlayer

The local player object exists; briefly false while logging in or zoning.

```csharp
public bool HasLocalPlayer { get; init; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_GameState_InCutscene"></a> InCutscene

A cutscene is playing.

```csharp
public bool InCutscene { get; init; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_GameState_InGPose"></a> InGPose

Group pose is active.

```csharp
public bool InGPose { get; init; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_GameState_InPvP"></a> InPvP

In a PvP area (Crystalline Conflict, Frontline, Rival Wings, and the Wolves' Den lobby). The official Dalamud rules
ban any PvP advantage, and the cube would work like a radar there (<code>docs/dalamud-repo-standards.md</code>).

```csharp
public bool InPvP { get; init; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_GameState_LoggedIn"></a> LoggedIn

Logged in to a character (false on the title screen and character select).

```csharp
public bool LoggedIn { get; init; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_GameState_Playing"></a> Playing

A normal in-game state: logged in, with a character, nothing in the way.

```csharp
public static GameState Playing { get; }
```

#### Property Value

 [GameState](Cube.Core.GameState.md)
