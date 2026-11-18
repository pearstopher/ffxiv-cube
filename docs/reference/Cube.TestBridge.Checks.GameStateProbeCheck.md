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
look-at point, which fixes the angle convention against the world axes.</li><li>The hide conditions (logged in, between areas, cutscene, GPose) as seen right now.</li><li>For #73 (named NPCs), the whole object table watched for about five seconds: where event and
battle NPCs sit, how many there are by kind, sub-kind and owner, how many have names, how many would be
inside the largest cube, and whether any name changed. Totals are kept in [NpcProbe](Cube.TestBridge.Checks.NpcProbe.md#Cube_TestBridge_Checks_NpcProbe).</li></ul>

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
look-at point, which fixes the angle convention against the world axes.</li><li>The hide conditions (logged in, between areas, cutscene, GPose) as seen right now.</li><li>For #73 (named NPCs), the whole object table watched for about five seconds: where event and
battle NPCs sit, how many there are by kind, sub-kind and owner, how many have names, how many would be
inside the largest cube, and whether any name changed. Totals are kept in [NpcProbe](Cube.TestBridge.Checks.NpcProbe.md#Cube_TestBridge_Checks_NpcProbe).</li></ul>

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

### <a id="Cube_TestBridge_Checks_GameStateProbeCheck_MaxHalfEdge"></a> MaxHalfEdge

Half the largest cube edge ([Max](Cube.Core.BoxExtents.md#Cube_Core_BoxExtents_Max)): NPCs within this many yalms on every
axis would be inside the biggest cube the settings allow, which is the worst case for the NPC cap.

```csharp
private const float MaxHalfEdge = 50
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_GameStateProbeCheck_NoOwner"></a> NoOwner

The <code>OwnerId</code> value meaning "no owner" (the game's invalid entity id).

```csharp
private const uint NoOwner = 3758096384
```

#### Field Value

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

### <a id="Cube_TestBridge_Checks_GameStateProbeCheck_NpcSampleSeconds"></a> NpcSampleSeconds

How long the NPC part of the probe (#73) watches, in seconds: long enough to see NPCs spawn and
despawn, and a name change if one happens while an NPC is in view. Time-based, not frame-based,
because the game drops its frame rate in the background and the bridge stops a check after 10 s.

```csharp
private const double NpcSampleSeconds = 4
```

#### Field Value

 [double](https://learn.microsoft.com/dotnet/api/system.double)

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

### <a id="Cube_TestBridge_Checks_GameStateProbeCheck_ProbeNpcs_Cube_TestBridge_Checks_NpcProbe_"></a> ProbeNpcs\(NpcProbe\)

One frame of the NPC research for #73: walks the whole object table and counts every NPC by slot,
kind, sub-kind, owner and whether it has a name, and remembers each NPC's name bytes to spot renames.

```csharp
private void ProbeNpcs(NpcProbe npcs)
```

#### Parameters

`npcs` [NpcProbe](Cube.TestBridge.Checks.NpcProbe.md)

The running totals.

#### Remarks

The proposed filter for #74 is counted as "named": an <code>EventNpc</code> or <code>BattleNpc</code> with a
non-empty name, no owner, and not a pet or chocobo companion (PROJECT.md, "NPCs": player-owned objects
are left out). Names are compared, never logged.

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
