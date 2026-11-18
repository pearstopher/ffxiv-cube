# <a id="Cube_Game_GameStateReader"></a> Class GameStateReader

Namespace: [Cube.Game](Cube.Game.md)  
Assembly: Cube.dll  

Reads the states that hide the cube (PROJECT.md, "When hidden") from Dalamud, using the sources chosen in #10
(<code>docs/research-notes.md</code>, "When to hide"): `Dalamud.Plugin.Services.IClientState.IsLoggedIn`,
`Dalamud.Plugin.Services.IClientState.IsGPosing` and `Dalamud.Plugin.Services.IClientState.IsPvP`, and the loading-screen and cutscene
condition flags.

```csharp
internal sealed class GameStateReader
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[GameStateReader](Cube.Game.GameStateReader.md)


## Remarks

Dalamud can also hide plugin windows in cutscenes and GPose, but only if the user's Dalamud settings say so, so
the plugin checks for itself. `Dalamud.Plugin.Services.IClientState.IsPvP` is used rather than the variant that excludes the
Wolves' Den: the lobby isn't a match, but hiding there too is simpler and costs nothing (#69). Each read is a
property or flag lookup: no allocation.

## Constructors

### <a id="Cube_Game_GameStateReader__ctor_Dalamud_Plugin_Services_IClientState_Dalamud_Plugin_Services_ICondition_"></a> GameStateReader\(IClientState, ICondition\)

Reads the states that hide the cube (PROJECT.md, "When hidden") from Dalamud, using the sources chosen in #10
(<code>docs/research-notes.md</code>, "When to hide"): `Dalamud.Plugin.Services.IClientState.IsLoggedIn`,
`Dalamud.Plugin.Services.IClientState.IsGPosing` and `Dalamud.Plugin.Services.IClientState.IsPvP`, and the loading-screen and cutscene
condition flags.

```csharp
public GameStateReader(IClientState clientState, ICondition condition)
```

#### Parameters

`clientState` [IClientState](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Plugin/Services/IClientState.cs)

Login, GPose and PvP state.

`condition` [ICondition](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Plugin/Services/ICondition.cs)

Condition flags.

#### Remarks

Dalamud can also hide plugin windows in cutscenes and GPose, but only if the user's Dalamud settings say so, so
the plugin checks for itself. `Dalamud.Plugin.Services.IClientState.IsPvP` is used rather than the variant that excludes the
Wolves' Den: the lobby isn't a match, but hiding there too is simpler and costs nothing (#69). Each read is a
property or flag lookup: no allocation.

## Fields

### <a id="Cube_Game_GameStateReader_lastFlagTicks"></a> lastFlagTicks

When a jump flag was last seen set, in stopwatch ticks; 0 before the first. Debug builds only.

```csharp
private long lastFlagTicks
```

#### Field Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

## Methods

### <a id="Cube_Game_GameStateReader_IsJumping"></a> IsJumping\(\)

Whether the game says the local player is jumping (PROJECT.md, "Jump"; #105): the cube holds its height while
this is true. Both jump flags are accepted, as the cutscene and loading flags are, because which one the game
raises isn't known until the owner's manual session (<code>docs/manual-tests.md</code>, "Jump"). A flag lookup:
read-only, no allocation.

```csharp
public bool IsJumping()
```

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Game_GameStateReader_Read_System_Boolean_"></a> Read\(bool\)

Reads this frame's state.

```csharp
public GameState Read(bool hasLocalPlayer)
```

#### Parameters

`hasLocalPlayer` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether the local player object was found this frame.

#### Returns

 [GameState](Cube.Core.GameState.md)

### <a id="Cube_Game_GameStateReader_TraceJump_System_Single_"></a> TraceJump\(float\)

Debug logging for the jump research (#105): while a jump flag is set, and for half a second after it clears, one
line per frame with both flags and the player's height. Allocates only while logging, which is never in
steady state, so the draw budget is unaffected. Debug builds only.

```csharp
public void TraceJump(float playerY)
```

#### Parameters

`playerY` [float](https://learn.microsoft.com/dotnet/api/system.single)

The local player's real height this frame.
