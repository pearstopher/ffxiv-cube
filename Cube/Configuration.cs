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
    /// <summary>
    /// The current file format. Bump it and migrate in <see cref="CubeSettings.Upgrade"/> if a setting's meaning
    /// changes. 2: the view is a rotation instead of a heading and tilt (#79). 3: the cube is a width, height and
    /// depth instead of one edge (#90). 4: adds the player height in the cube; older files load it as 50% (#91).
    /// 5: adds the collision style; older files load Boxes (#100).
    /// </summary>
    public const int CurrentVersion = CollisionStyleVersion;

    /// <inheritdoc/>
    public int Version { get; set; } = CurrentVersion;

    /// <summary>
    /// Loads the saved settings, or the defaults if there are none, converts an older file format and repairs any
    /// bad values.
    /// </summary>
    /// <param name="pluginInterface">Dalamud's interface for this plugin.</param>
    public static Configuration Load(IDalamudPluginInterface pluginInterface)
    {
        var config = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        config.Upgrade(config.Version);
        config.Version = CurrentVersion;
        return config;
    }

    /// <summary>Writes the settings to disk. Called when the user finishes a change, not every frame.</summary>
    /// <param name="pluginInterface">Dalamud's interface for this plugin.</param>
    public void Save(IDalamudPluginInterface pluginInterface) => pluginInterface.SavePluginConfig(this);
}
