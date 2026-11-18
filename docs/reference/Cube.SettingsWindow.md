# <a id="Cube_SettingsWindow"></a> Class SettingsWindow

Namespace: [Cube](Cube.md)  
Assembly: Cube.dll  

The settings window, opened with <code>/cube config</code> or Dalamud's settings button (PROJECT.md, "Features": cube
size, orientation mode, labels on/off, colours and opacity; "View orientation control": the control's corner and
a way to reset the view).

```csharp
internal sealed class SettingsWindow : Window, IWindow
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[Window](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs) ← 
[SettingsWindow](Cube.SettingsWindow.md)

#### Implements

[IWindow](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/IWindow.cs)


## Remarks

Every control edits [Configuration](Cube.Configuration.md#Cube_Configuration) directly, so changes show in the cube on the next frame. The file
is written when an edit is finished (slider or colour picker released, box ticked), not on every frame of a drag.
Not part of the per-frame cube path: it only draws while open.

## Constructors

### <a id="Cube_SettingsWindow__ctor_Cube_Configuration_System_Action_"></a> SettingsWindow\(Configuration, Action\)

Creates the window, closed.

```csharp
public SettingsWindow(Configuration config, Action save)
```

#### Parameters

`config` [Configuration](Cube.Configuration.md)

The settings to edit.

`save` [Action](https://learn.microsoft.com/dotnet/api/system.action)

Saves them.

## Fields

### <a id="Cube_SettingsWindow_CornerNames"></a> CornerNames

The corner choices, in [WidgetCorner](Cube.Core.WidgetCorner.md#Cube_Core_WidgetCorner) order.

```csharp
private static readonly string[] CornerNames
```

#### Field Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)\[\]

### <a id="Cube_SettingsWindow_config"></a> config

The settings being edited.

```csharp
private readonly Configuration config
```

#### Field Value

 [Configuration](Cube.Configuration.md)

### <a id="Cube_SettingsWindow_save"></a> save

Writes the settings to disk.

```csharp
private readonly Action save
```

#### Field Value

 [Action](https://learn.microsoft.com/dotnet/api/system.action)

## Properties

### <a id="Cube_SettingsWindow_DrawCount"></a> DrawCount

How many times the window has drawn; the bridge checks it draws without errors. Debug builds only.

```csharp
internal long DrawCount { get; private set; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Cube_SettingsWindow_Errors"></a> Errors

Exceptions caught while drawing; the bridge checks there were none.

```csharp
internal ErrorReporter Errors { get; }
```

#### Property Value

 [ErrorReporter](Cube.ErrorReporter.md)

## Methods

### <a id="Cube_SettingsWindow_ApplyAndSave"></a> ApplyAndSave\(\)

For one-click controls: applies and saves.

```csharp
private void ApplyAndSave()
```

### <a id="Cube_SettingsWindow_Draw"></a> Draw\(\)

Draws the controls, catching and logging any exception so it never reaches the game.

```csharp
public override void Draw()
```

### <a id="Cube_SettingsWindow_DrawControls"></a> DrawControls\(\)

Draws the controls and applies and saves changes.

```csharp
private void DrawControls()
```

### <a id="Cube_SettingsWindow_Track_System_Boolean_"></a> Track\(bool\)

For controls that are dragged: applies every change at once, saves when the edit is finished.

```csharp
private void Track(bool changed)
```

#### Parameters

`changed` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether the control changed its value this frame.
