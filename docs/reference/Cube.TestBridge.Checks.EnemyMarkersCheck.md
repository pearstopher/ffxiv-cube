# <a id="Cube_TestBridge_Checks_EnemyMarkersCheck"></a> Class EnemyMarkersCheck

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

Checks #111 end to end through the window: fake enemies arrive with the enemy relation, and the labels carry
<code>[T]</code> for the target and <code>[!]</code> for an attacking enemy, target first when both apply. The game's real target
and enemy-list reads are logged by the NPC probe and need a fight to exercise (docs/manual-tests.md).

```csharp
internal sealed class EnemyMarkersCheck : BridgeCheck
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BridgeCheck](Cube.TestBridge.BridgeCheck.md) ← 
[EnemyMarkersCheck](Cube.TestBridge.Checks.EnemyMarkersCheck.md)


## Fields

### <a id="Cube_TestBridge_Checks_EnemyMarkersCheck_Cases"></a> Cases

The fakes added: name, then the label text expected.

```csharp
private static readonly (string Name, PlayerRelation Relation, string Label)[] Cases
```

#### Field Value

 \([string](https://learn.microsoft.com/dotnet/api/system.string) [Name](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.string,cube.core.playerrelation,system.string\-.name), [PlayerRelation](Cube.Core.PlayerRelation.md) [Relation](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.string,cube.core.playerrelation,system.string\-.relation), [string](https://learn.microsoft.com/dotnet/api/system.string) [Label](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.string,cube.core.playerrelation,system.string\-.label)\)\[\]

### <a id="Cube_TestBridge_Checks_EnemyMarkersCheck_FramesToWait"></a> FramesToWait

Frames to wait for the window to draw.

```csharp
private const int FramesToWait = 30
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_EnemyMarkersCheck_hadNpcs"></a> hadNpcs

The NPC setting before the check.

```csharp
private bool hadNpcs
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_TestBridge_Checks_EnemyMarkersCheck_wasOpen"></a> wasOpen

The window's open state before the check.

```csharp
private bool wasOpen
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Properties

### <a id="Cube_TestBridge_Checks_EnemyMarkersCheck_Name"></a> Name

Short unique name, shown in the results, e.g. <code>window-opens-and-closes</code>.

```csharp
public override string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

## Methods

### <a id="Cube_TestBridge_Checks_EnemyMarkersCheck_Cleanup_Cube_TestBridge_BridgeContext_"></a> Cleanup\(BridgeContext\)

Puts back anything the check changed (e.g. the window's open state). Called after every run,
including when the check failed, threw or timed out, so a check never leaves the plugin altered.

```csharp
public override void Cleanup(BridgeContext context)
```

#### Parameters

`context` [BridgeContext](Cube.TestBridge.BridgeContext.md)

The same context the check ran with.

### <a id="Cube_TestBridge_Checks_EnemyMarkersCheck_Run_Cube_TestBridge_BridgeContext_"></a> Run\(BridgeContext\)

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
