# <a id="Cube_TestBridge_Checks_GameStateProbeCheck"></a> Class GameStateProbeCheck

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

Research probe for #10 (reading players, party and camera): reads the same facts two ways and records
both, so one in-game run settles the open questions in <code>docs/research-notes.md</code>.

```csharp
internal sealed class GameStateProbeCheck : BridgeCheck
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BridgeCheck](Cube.TestBridge.BridgeCheck.md) ← 
[GameStateProbeCheck](Cube.TestBridge.Checks.GameStateProbeCheck.md)


## Remarks

<ul><li>Player count from Dalamud's `Dalamud.Plugin.Services.IObjectTable` versus a direct, allocation-free walk of
FFXIVClientStructs' `FFXIVClientStructs.FFXIV.Client.Game.Object.GameObjectManager` (the planned hot-path reader). They must agree.</li><li>Party and alliance counts from the `FFXIVClientStructs.FFXIV.Client.Game.Character.Character` relation flags.</li><li>The camera's yaw from <code>Camera.DirH</code> and, independently, from the scene camera's position and
look-at point, which fixes the angle convention against the world axes.</li><li>The hide conditions (logged in, between areas, cutscene, GPose) as seen right now.</li></ul>

Fails only if the player isn't logged in (nothing to read) or the two player counts disagree.
Read-only: it only reads memory (PROJECT.md, "Read-only game access").

## Constructors

### <a id="Cube_TestBridge_Checks_GameStateProbeCheck__ctor_Dalamud_Plugin_Services_IObjectTable_Dalamud_Plugin_Services_IClientState_Dalamud_Plugin_Services_ICondition_Dalamud_Plugin_Services_IPluginLog_"></a> GameStateProbeCheck\(IObjectTable, IClientState, ICondition, IPluginLog\)

Research probe for #10 (reading players, party and camera): reads the same facts two ways and records
both, so one in-game run settles the open questions in <code>docs/research-notes.md</code>.

```csharp
public GameStateProbeCheck(IObjectTable objects, IClientState clientState, ICondition condition, IPluginLog log)
```

#### Parameters

`objects` [IObjectTable](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Plugin/Services/IObjectTable.cs)

Dalamud's object table: the independent read.

`clientState` [IClientState](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Plugin/Services/IClientState.cs)

Login and GPose state.

`condition` [ICondition](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Plugin/Services/ICondition.cs)

Loading-screen and cutscene condition flags.

`log` [IPluginLog](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Plugin/Services/IPluginLog.cs)

Where the probe's findings are logged, at Information so they show without Debug+.

#### Remarks

<ul><li>Player count from Dalamud's `Dalamud.Plugin.Services.IObjectTable` versus a direct, allocation-free walk of
FFXIVClientStructs' `FFXIVClientStructs.FFXIV.Client.Game.Object.GameObjectManager` (the planned hot-path reader). They must agree.</li><li>Party and alliance counts from the `FFXIVClientStructs.FFXIV.Client.Game.Character.Character` relation flags.</li><li>The camera's yaw from <code>Camera.DirH</code> and, independently, from the scene camera's position and
look-at point, which fixes the angle convention against the world axes.</li><li>The hide conditions (logged in, between areas, cutscene, GPose) as seen right now.</li></ul>

Fails only if the player isn't logged in (nothing to read) or the two player counts disagree.
Read-only: it only reads memory (PROJECT.md, "Read-only game access").

## Fields

### <a id="Cube_TestBridge_Checks_GameStateProbeCheck_CharacterSlots"></a> CharacterSlots

The object table indexes holding characters: 0–199, players and battle NPCs at even indexes (FFXIVClientStructs' notes).

```csharp
private const int CharacterSlots = 200
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Properties

### <a id="Cube_TestBridge_Checks_GameStateProbeCheck_Name"></a> Name

Short unique name, shown in the results, e.g. <code>window-opens-and-closes</code>.

```csharp
public override string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

## Methods

### <a id="Cube_TestBridge_Checks_GameStateProbeCheck_Probe_Cube_TestBridge_BridgeContext_"></a> Probe\(BridgeContext\)

Reads everything in one frame, records it as metrics and logs a summary line.

```csharp
private CheckStep Probe(BridgeContext context)
```

#### Parameters

`context` [BridgeContext](Cube.TestBridge.BridgeContext.md)

Where the metrics go.

#### Returns

 [CheckStep](Cube.TestBridge.CheckStep.md)

### <a id="Cube_TestBridge_Checks_GameStateProbeCheck_Run_Cube_TestBridge_BridgeContext_"></a> Run\(BridgeContext\)

Runs the check, one step per frame, on the framework thread. Yield [NextFrame](Cube.TestBridge.CheckStep.md#Cube_TestBridge_CheckStep_NextFrame)
to wait a frame; finish with [Pass](Cube.TestBridge.CheckStep.md#Cube_TestBridge_CheckStep_Pass) or [Fail](Cube.TestBridge.CheckStep.md#Cube_TestBridge_CheckStep_Fail).

```csharp
public override IEnumerable<CheckStep> Run(BridgeContext context)
```

#### Parameters

`context` [BridgeContext](Cube.TestBridge.BridgeContext.md)

The plugin parts the check can look at, and where it records metrics.

#### Returns

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[CheckStep](Cube.TestBridge.CheckStep.md)\>
