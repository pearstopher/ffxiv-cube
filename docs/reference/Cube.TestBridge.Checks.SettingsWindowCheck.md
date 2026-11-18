# <a id="Cube_TestBridge_Checks_SettingsWindowCheck"></a> Class SettingsWindowCheck

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

Checks the settings window draws and the settings are saved in the expected file (PROJECT.md, "Features":
a settings window, <code>/cube config</code>; the acceptance rule that settings are saved on change).

```csharp
internal sealed class SettingsWindowCheck : BridgeCheck
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BridgeCheck](Cube.TestBridge.BridgeCheck.md) ← 
[SettingsWindowCheck](Cube.TestBridge.Checks.SettingsWindowCheck.md)


## Remarks

Opens the settings window for a couple of frames (an exception in its controls would stop it drawing), then
saves the user's current, unchanged settings and reads the file back: it must hold the current cube size,
collision settings (#32) and file version, and not the in-memory-only revision counter.

## Constructors

### <a id="Cube_TestBridge_Checks_SettingsWindowCheck__ctor_Cube_SettingsWindow_Cube_Configuration_System_Action_System_String_"></a> SettingsWindowCheck\(SettingsWindow, Configuration, Action, string\)

Checks the settings window draws and the settings are saved in the expected file (PROJECT.md, "Features":
a settings window, <code>/cube config</code>; the acceptance rule that settings are saved on change).

```csharp
public SettingsWindowCheck(SettingsWindow window, Configuration config, Action save, string configFile)
```

#### Parameters

`window` [SettingsWindow](Cube.SettingsWindow.md)

The settings window.

`config` [Configuration](Cube.Configuration.md)

The live settings.

`save` [Action](https://learn.microsoft.com/dotnet/api/system.action)

The plugin's save action, the same one the window uses.

`configFile` [string](https://learn.microsoft.com/dotnet/api/system.string)

Where Dalamud stores the settings.

#### Remarks

Opens the settings window for a couple of frames (an exception in its controls would stop it drawing), then
saves the user's current, unchanged settings and reads the file back: it must hold the current cube size,
collision settings (#32) and file version, and not the in-memory-only revision counter.

## Fields

### <a id="Cube_TestBridge_Checks_SettingsWindowCheck_FramesToWait"></a> FramesToWait

Frames to wait for the settings window to draw.

```csharp
private const int FramesToWait = 30
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_SettingsWindowCheck_wasOpen"></a> wasOpen

The settings window's open state before the check.

```csharp
private bool wasOpen
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Properties

### <a id="Cube_TestBridge_Checks_SettingsWindowCheck_Name"></a> Name

Short unique name, shown in the results, e.g. <code>window-opens-and-closes</code>.

```csharp
public override string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

## Methods

### <a id="Cube_TestBridge_Checks_SettingsWindowCheck_Cleanup_Cube_TestBridge_BridgeContext_"></a> Cleanup\(BridgeContext\)

Puts back anything the check changed (e.g. the window's open state). Called after every run,
including when the check failed, threw or timed out, so a check never leaves the plugin altered.

```csharp
public override void Cleanup(BridgeContext context)
```

#### Parameters

`context` [BridgeContext](Cube.TestBridge.BridgeContext.md)

The same context the check ran with.

### <a id="Cube_TestBridge_Checks_SettingsWindowCheck_Run_Cube_TestBridge_BridgeContext_"></a> Run\(BridgeContext\)

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

### <a id="Cube_TestBridge_Checks_SettingsWindowCheck_VerifyFile"></a> VerifyFile\(\)

Reads the saved file back and compares it with the live settings.

```csharp
private CheckStep VerifyFile()
```

#### Returns

 [CheckStep](Cube.TestBridge.CheckStep.md)
