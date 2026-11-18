# <a id="Cube_TestBridge_Checks_InteractableProbeCheck"></a> Class InteractableProbeCheck

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

Research probe for #112 (which in-world objects does the game label with words, and which with icons?): walks the
object table for a few seconds and logs every distinct (kind, name, nameplate icon, targetable) combination of the
objects that are neither characters nor NPCs, so a single session in a housing area, a city and a dungeon answers
which fields tell a text label from an icon-only one.

```csharp
internal sealed class InteractableProbeCheck : BridgeCheck
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BridgeCheck](Cube.TestBridge.BridgeCheck.md) ← 
[InteractableProbeCheck](Cube.TestBridge.Checks.InteractableProbeCheck.md)


## Remarks

Only objects within 100 yalms of the player are listed (the largest cube is 100 across). The names logged are
object names such as "Entrance", never player names. The check passes whenever the game is reachable; the findings
are in <code>dalamud.log</code> as <code>Probe #112</code> lines and are recorded in <code>docs/research-notes.md</code>.

## Constructors

### <a id="Cube_TestBridge_Checks_InteractableProbeCheck__ctor_Dalamud_Plugin_Services_IPluginLog_"></a> InteractableProbeCheck\(IPluginLog\)

Research probe for #112 (which in-world objects does the game label with words, and which with icons?): walks the
object table for a few seconds and logs every distinct (kind, name, nameplate icon, targetable) combination of the
objects that are neither characters nor NPCs, so a single session in a housing area, a city and a dungeon answers
which fields tell a text label from an icon-only one.

```csharp
public InteractableProbeCheck(IPluginLog log)
```

#### Parameters

`log` [IPluginLog](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Plugin/Services/IPluginLog.cs)

Where the findings are logged, at Information.

#### Remarks

Only objects within 100 yalms of the player are listed (the largest cube is 100 across). The names logged are
object names such as "Entrance", never player names. The check passes whenever the game is reachable; the findings
are in <code>dalamud.log</code> as <code>Probe #112</code> lines and are recorded in <code>docs/research-notes.md</code>.

## Fields

### <a id="Cube_TestBridge_Checks_InteractableProbeCheck_Reach"></a> Reach

Furthest an object can be on any axis and still be listed.

```csharp
private const float Reach = 100
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_InteractableProbeCheck_SampleSeconds"></a> SampleSeconds

How long the probe watches, in seconds.

```csharp
private const double SampleSeconds = 3
```

#### Field Value

 [double](https://learn.microsoft.com/dotnet/api/system.double)

## Properties

### <a id="Cube_TestBridge_Checks_InteractableProbeCheck_Name"></a> Name

Short unique name, shown in the results, e.g. <code>window-opens-and-closes</code>.

```csharp
public override string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

## Methods

### <a id="Cube_TestBridge_Checks_InteractableProbeCheck_Run_Cube_TestBridge_BridgeContext_"></a> Run\(BridgeContext\)

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

### <a id="Cube_TestBridge_Checks_InteractableProbeCheck_Sample_System_Collections_Generic_Dictionary_System_String_System_Int32__"></a> Sample\(Dictionary<string, int\>\)

Records one frame's objects.

```csharp
private static void Sample(Dictionary<string, int> seen)
```

#### Parameters

`seen` [Dictionary](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary\-2)<[string](https://learn.microsoft.com/dotnet/api/system.string), [int](https://learn.microsoft.com/dotnet/api/system.int32)\>

Distinct combinations and the largest per-frame count is not needed: the count is frames seen.
