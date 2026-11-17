# <a id="Cube_TestBridge_Checks_NoErrorsCaughtCheck"></a> Class NoErrorsCaughtCheck

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

Checks no game-facing handler has caught an exception since the plugin loaded (CLAUDE.md, "Dalamud rules": wrap
every handler in try/catch and log). Runs after the other checks, so it also covers everything they exercised.

```csharp
internal sealed class NoErrorsCaughtCheck : BridgeCheck
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BridgeCheck](Cube.TestBridge.BridgeCheck.md) ← 
[NoErrorsCaughtCheck](Cube.TestBridge.Checks.NoErrorsCaughtCheck.md)


## Remarks

Handlers swallow exceptions so the game is never affected, which also means a bug could go unnoticed; this check
turns any caught exception into a failed run. Errors during unload and reload are checked by the test script in
<code>dalamud.log</code>, since the plugin can't observe its own unload.

## Constructors

### <a id="Cube_TestBridge_Checks_NoErrorsCaughtCheck__ctor_Cube_ErrorReporter___"></a> NoErrorsCaughtCheck\(params ErrorReporter\[\]\)

Checks no game-facing handler has caught an exception since the plugin loaded (CLAUDE.md, "Dalamud rules": wrap
every handler in try/catch and log). Runs after the other checks, so it also covers everything they exercised.

```csharp
public NoErrorsCaughtCheck(params ErrorReporter[] reporters)
```

#### Parameters

`reporters` [ErrorReporter](Cube.ErrorReporter.md)\[\]

Every handler's error reporter.

#### Remarks

Handlers swallow exceptions so the game is never affected, which also means a bug could go unnoticed; this check
turns any caught exception into a failed run. Errors during unload and reload are checked by the test script in
<code>dalamud.log</code>, since the plugin can't observe its own unload.

## Properties

### <a id="Cube_TestBridge_Checks_NoErrorsCaughtCheck_Name"></a> Name

Short unique name, shown in the results, e.g. <code>window-opens-and-closes</code>.

```csharp
public override string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

## Methods

### <a id="Cube_TestBridge_Checks_NoErrorsCaughtCheck_Run_Cube_TestBridge_BridgeContext_"></a> Run\(BridgeContext\)

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
