# <a id="Cube_Configuration"></a> Class Configuration

Namespace: [Cube](Cube.md)  
Assembly: Cube.dll  

The saved settings: Core's [CubeSettings](Cube.Core.CubeSettings.md#Cube_Core_CubeSettings) plus the version number Dalamud's configuration system
needs. Stored by Dalamud in <code>pluginConfigs\Cube.json</code>; contains no names or IDs (PROJECT.md, "Privacy").

```csharp
[Serializable]
internal sealed class Configuration : CubeSettings, IPluginConfiguration
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[CubeSettings](Cube.Core.CubeSettings.md) ← 
[Configuration](Cube.Configuration.md)

#### Implements

[IPluginConfiguration](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Configuration/IPluginConfiguration.cs)


## Fields

### <a id="Cube_Configuration_CurrentVersion"></a> CurrentVersion

The current file format. Bump it and migrate in [Upgrade](Cube.Core.CubeSettings.md#Cube_Core_CubeSettings_Upgrade) if a setting's meaning
changes. 2: the view is a rotation instead of a heading and tilt (#79). 3: the cube is a width, height and
depth instead of one edge (#90).

```csharp
public const int CurrentVersion = 3
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Properties

### <a id="Cube_Configuration_Version"></a> Version

Gets or sets configuration version.

```csharp
public int Version { get; set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Methods

### <a id="Cube_Configuration_Load_Dalamud_Plugin_IDalamudPluginInterface_"></a> Load\(IDalamudPluginInterface\)

Loads the saved settings, or the defaults if there are none, converts an older file format and repairs any
bad values.

```csharp
public static Configuration Load(IDalamudPluginInterface pluginInterface)
```

#### Parameters

`pluginInterface` [IDalamudPluginInterface](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Plugin/IDalamudPluginInterface.cs)

Dalamud's interface for this plugin.

#### Returns

 [Configuration](Cube.Configuration.md)

### <a id="Cube_Configuration_Save_Dalamud_Plugin_IDalamudPluginInterface_"></a> Save\(IDalamudPluginInterface\)

Writes the settings to disk. Called when the user finishes a change, not every frame.

```csharp
public void Save(IDalamudPluginInterface pluginInterface)
```

#### Parameters

`pluginInterface` [IDalamudPluginInterface](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Plugin/IDalamudPluginInterface.cs)

Dalamud's interface for this plugin.
