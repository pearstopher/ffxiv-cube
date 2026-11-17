using System;
using Cube.Core;
using Dalamud.Configuration;
using Dalamud.Plugin;

namespace Cube;

/// <summary>
/// The saved settings: Core's <see cref="CubeSettings"/> plus the version number Dalamud's configuration system
/// needs. Stored by Dalamud in <c>pluginConfigs\Cube.json</c>; contains no names or IDs (PROJECT.md, "Privacy").
/// </summary>
[Serializable]
internal sealed class Configuration : CubeSettings, IPluginConfiguration
{
    /// <summary>The current file format. Bump it and migrate in <see cref="Load"/> if a setting's meaning changes.</summary>
    public const int CurrentVersion = 1;

    /// <inheritdoc/>
    public int Version { get; set; } = CurrentVersion;

    /// <summary>Loads the saved settings, or the defaults if there are none, and repairs any bad values.</summary>
    /// <param name="pluginInterface">Dalamud's interface for this plugin.</param>
    public static Configuration Load(IDalamudPluginInterface pluginInterface)
    {
        var config = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        config.Version = CurrentVersion;
        config.Changed();
        return config;
    }

    /// <summary>Writes the settings to disk. Called when the user finishes a change, not every frame.</summary>
    /// <param name="pluginInterface">Dalamud's interface for this plugin.</param>
    public void Save(IDalamudPluginInterface pluginInterface) => pluginInterface.SavePluginConfig(this);
}
