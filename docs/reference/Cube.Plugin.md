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

```csharp
private const string BuildConfiguration = "Release"
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

### <a id="Cube_Plugin_cubeWindow"></a> cubeWindow

```csharp
private readonly CubeWindow cubeWindow
```

#### Field Value

 [CubeWindow](Cube.CubeWindow.md)

### <a id="Cube_Plugin_windowSystem"></a> windowSystem

```csharp
private readonly WindowSystem windowSystem
```

#### Field Value

 [WindowSystem](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/WindowSystem.cs)

## Properties

### <a id="Cube_Plugin_CommandManager"></a> CommandManager

Registers the <code>/cube</code> command.

```csharp
[PluginService]
internal static ICommandManager CommandManager { get; private set; }
```

#### Property Value

 [ICommandManager](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Plugin/Services/ICommandManager.cs)

### <a id="Cube_Plugin_Log"></a> Log

The plugin's log, written to <code>/xllog</code> and <code>dalamud.log</code>.

```csharp
[PluginService]
internal static IPluginLog Log { get; private set; }
```

#### Property Value

 [IPluginLog](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Plugin/Services/IPluginLog.cs)

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

Unregisters everything, each part even if an earlier one fails.

```csharp
public void Dispose()
```

### <a id="Cube_Plugin_DisposeSafely_System_Action_System_String_"></a> DisposeSafely\(Action, string\)

Runs one cleanup step and logs, rather than throws, if it fails.

```csharp
private static void DisposeSafely(Action step, string name)
```

#### Parameters

`step` [Action](https://learn.microsoft.com/dotnet/api/system.action)

The cleanup to run.

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)

What is being cleaned up, for the log.

### <a id="Cube_Plugin_OnCommand_System_String_System_String_"></a> OnCommand\(string, string\)

Handles <code>/cube</code>: toggles the window.

```csharp
private void OnCommand(string command, string args)
```

#### Parameters

`command` [string](https://learn.microsoft.com/dotnet/api/system.string)

The command typed.

`args` [string](https://learn.microsoft.com/dotnet/api/system.string)

Anything typed after the command (unused for now).

### <a id="Cube_Plugin_ToggleCubeWindow"></a> ToggleCubeWindow\(\)

Shows the window if hidden, hides it if shown.

```csharp
private void ToggleCubeWindow()
```
