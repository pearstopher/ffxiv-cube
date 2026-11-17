# <a id="Cube_Plugin"></a> Class Plugin

Namespace: [Cube](Cube.md)  
Assembly: Cube.dll  

Entry point. Registers the <code>/cube</code> command and the cube window, and logs which build loaded.

```csharp
public sealed class Plugin : IDalamudPlugin, IDisposable
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[Plugin](Cube.Plugin.md)

#### Implements

[IDalamudPlugin](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Plugin/IDalamudPlugin.cs), 
[IDisposable](https://learn.microsoft.com/dotnet/api/system.idisposable)


## Remarks

The plugin only reads game state and draws it in its own window (PROJECT.md, "Goal"):
it never changes the game, hooks nothing it doesn't need, and sends nothing.
Everything registered here is released in [Dispose](Cube.Plugin.md#Cube_Plugin_Dispose).

## Constructors

### <a id="Cube_Plugin__ctor"></a> Plugin\(\)

Creates the window and command, then logs the build line.

```csharp
public Plugin()
```

## Fields

### <a id="Cube_Plugin_BuildConfiguration"></a> BuildConfiguration

The build configuration, shown in the load line.

```csharp
private const string BuildConfiguration = "Debug"
```

#### Field Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Cube_Plugin_CommandName"></a> CommandName

The slash command that toggles the cube window.

```csharp
internal const string CommandName = "/cube"
```

#### Field Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Cube_Plugin_configuration"></a> configuration

The saved settings, shared by both windows.

```csharp
private readonly Configuration configuration
```

#### Field Value

 [Configuration](Cube.Configuration.md)

### <a id="Cube_Plugin_cubeWindow"></a> cubeWindow

The cube window.

```csharp
private readonly CubeWindow cubeWindow
```

#### Field Value

 [CubeWindow](Cube.CubeWindow.md)

### <a id="Cube_Plugin_settingsWindow"></a> settingsWindow

The settings window (<code>/cube config</code>).

```csharp
private readonly SettingsWindow settingsWindow
```

#### Field Value

 [SettingsWindow](Cube.SettingsWindow.md)

### <a id="Cube_Plugin_testBridge"></a> testBridge

The in-game test bridge (PROJECT.md, "Automated testing"). Debug builds only.

```csharp
private readonly BridgeRunner? testBridge
```

#### Field Value

 [BridgeRunner](Cube.TestBridge.BridgeRunner.md)?

### <a id="Cube_Plugin_uiErrors"></a> uiErrors

Exceptions caught in the command and the Dalamud UI buttons.

```csharp
private readonly ErrorReporter uiErrors
```

#### Field Value

 [ErrorReporter](Cube.ErrorReporter.md)

### <a id="Cube_Plugin_unloadErrors"></a> unloadErrors

How many unload steps failed; reported in the unload line.

```csharp
private int unloadErrors
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Plugin_windowSystem"></a> windowSystem

Dalamud's window manager for this plugin's windows.

```csharp
private readonly WindowSystem windowSystem
```

#### Field Value

 [WindowSystem](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/WindowSystem.cs)

## Properties

### <a id="Cube_Plugin_ClientState"></a> ClientState

Login and GPose state: the cube is hidden when logged out or in GPose (PROJECT.md, "When hidden").

```csharp
[PluginService]
internal static IClientState ClientState { get; private set; }
```

#### Property Value

 [IClientState](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Plugin/Services/IClientState.cs)

### <a id="Cube_Plugin_CommandManager"></a> CommandManager

Registers the <code>/cube</code> command.

```csharp
[PluginService]
internal static ICommandManager CommandManager { get; private set; }
```

#### Property Value

 [ICommandManager](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Plugin/Services/ICommandManager.cs)

### <a id="Cube_Plugin_Condition"></a> Condition

Condition flags: the cube is hidden during loading screens and cutscenes.

```csharp
[PluginService]
internal static ICondition Condition { get; private set; }
```

#### Property Value

 [ICondition](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Plugin/Services/ICondition.cs)

### <a id="Cube_Plugin_Framework"></a> Framework

Drives the in-game test bridge's checks, one step per frame. Debug builds only.

```csharp
[PluginService]
internal static IFramework Framework { get; private set; }
```

#### Property Value

 [IFramework](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Plugin/Services/IFramework.cs)

### <a id="Cube_Plugin_Log"></a> Log

The plugin's log, written to <code>/xllog</code> and <code>dalamud.log</code>.

```csharp
[PluginService]
internal static IPluginLog Log { get; private set; }
```

#### Property Value

 [IPluginLog](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Plugin/Services/IPluginLog.cs)

### <a id="Cube_Plugin_ObjectTable"></a> ObjectTable

The object table, read by the bridge's probe as an independent source. Debug builds only, for now.

```csharp
[PluginService]
internal static IObjectTable ObjectTable { get; private set; }
```

#### Property Value

 [IObjectTable](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Plugin/Services/IObjectTable.cs)

### <a id="Cube_Plugin_PluginInterface"></a> PluginInterface

Dalamud's interface for this plugin: UI callbacks, manifest, file locations.

```csharp
[PluginService]
internal static IDalamudPluginInterface PluginInterface { get; private set; }
```

#### Property Value

 [IDalamudPluginInterface](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Plugin/IDalamudPluginInterface.cs)

## Methods

### <a id="Cube_Plugin_Dispose"></a> Dispose\(\)

Unregisters everything the constructor registered (the command, the three UI callbacks, both windows, and in
Debug the bridge's framework hook and file watcher), each part even if an earlier one fails. The plugin changes
nothing in the game, so there is nothing else to restore.

```csharp
public void Dispose()
```

#### Remarks

The last line is <code>Cube unloaded</code>, or <code>Cube unloaded with N error(s)</code> if any step failed; the in-game
test script fails on the latter.

### <a id="Cube_Plugin_DisposeSafely_System_Action_System_String_"></a> DisposeSafely\(Action, string\)

Runs one cleanup step and logs and counts, rather than throws, if it fails.

```csharp
private void DisposeSafely(Action step, string name)
```

#### Parameters

`step` [Action](https://learn.microsoft.com/dotnet/api/system.action)

The cleanup to run.

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)

What is being cleaned up, for the log.

### <a id="Cube_Plugin_OnCommand_System_String_System_String_"></a> OnCommand\(string, string\)

Handles <code>/cube</code> (toggles the cube window) and <code>/cube config</code> (toggles the settings).

```csharp
private void OnCommand(string command, string args)
```

#### Parameters

`command` [string](https://learn.microsoft.com/dotnet/api/system.string)

The command typed.

`args` [string](https://learn.microsoft.com/dotnet/api/system.string)

Anything typed after the command.

### <a id="Cube_Plugin_SaveConfiguration"></a> SaveConfiguration\(\)

Writes the settings to disk, logging rather than throwing if that fails.

```csharp
private void SaveConfiguration()
```

### <a id="Cube_Plugin_ToggleCubeWindow"></a> ToggleCubeWindow\(\)

Shows the cube window if hidden, hides it if shown (Dalamud's "open" button for the plugin).

```csharp
private void ToggleCubeWindow()
```

### <a id="Cube_Plugin_ToggleSettingsWindow"></a> ToggleSettingsWindow\(\)

Shows or hides the settings window (Dalamud's settings button for the plugin).

```csharp
private void ToggleSettingsWindow()
```
