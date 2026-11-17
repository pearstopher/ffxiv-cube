# <a id="Cube_Game_GameStateReader"></a> Class GameStateReader

Namespace: [Cube.Game](Cube.Game.md)  
Assembly: Cube.dll  

Reads the states that hide the cube (PROJECT.md, "When hidden") from Dalamud, using the sources chosen in #10
(<code>docs/research-notes.md</code>, "When to hide"): `Dalamud.Plugin.Services.IClientState.IsLoggedIn` and
`Dalamud.Plugin.Services.IClientState.IsGPosing`, and the loading-screen and cutscene condition flags.

```csharp
internal sealed class GameStateReader
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[GameStateReader](Cube.Game.GameStateReader.md)


## Remarks

Dalamud can also hide plugin windows in cutscenes and GPose, but only if the user's Dalamud settings say so, so
the plugin checks for itself. Each read is a property or flag lookup: no allocation.

## Constructors

### <a id="Cube_Game_GameStateReader__ctor_Dalamud_Plugin_Services_IClientState_Dalamud_Plugin_Services_ICondition_"></a> GameStateReader\(IClientState, ICondition\)

Reads the states that hide the cube (PROJECT.md, "When hidden") from Dalamud, using the sources chosen in #10
(<code>docs/research-notes.md</code>, "When to hide"): `Dalamud.Plugin.Services.IClientState.IsLoggedIn` and
`Dalamud.Plugin.Services.IClientState.IsGPosing`, and the loading-screen and cutscene condition flags.

```csharp
public GameStateReader(IClientState clientState, ICondition condition)
```

#### Parameters

`clientState` [IClientState](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Plugin/Services/IClientState.cs)

Login and GPose state.

`condition` [ICondition](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Plugin/Services/ICondition.cs)

Condition flags.

#### Remarks

Dalamud can also hide plugin windows in cutscenes and GPose, but only if the user's Dalamud settings say so, so
the plugin checks for itself. Each read is a property or flag lookup: no allocation.

## Methods

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
