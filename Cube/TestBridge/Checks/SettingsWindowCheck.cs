using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Cube.TestBridge.Checks;

/// <summary>
/// Checks the settings window draws and the settings are saved in the expected file (PROJECT.md, "Features":
/// a settings window, <c>/cube config</c>; the acceptance rule that settings are saved on change).
/// </summary>
/// <remarks>
/// Opens the settings window for a couple of frames (an exception in its controls would stop it drawing), then
/// saves the user's current, unchanged settings and reads the file back: it must hold the current cube size and
/// file version, and not the in-memory-only revision counter.
/// </remarks>
/// <param name="window">The settings window.</param>
/// <param name="config">The live settings.</param>
/// <param name="save">The plugin's save action, the same one the window uses.</param>
/// <param name="configFile">Where Dalamud stores the settings.</param>
internal sealed class SettingsWindowCheck(SettingsWindow window, Configuration config, Action save, string configFile) : BridgeCheck
{
    /// <summary>Frames to wait for the settings window to draw.</summary>
    private const int FramesToWait = 30;

    /// <summary>The settings window's open state before the check.</summary>
    private bool wasOpen;

    /// <inheritdoc/>
    public override string Name => "settings-window-draws-and-saves";

    /// <inheritdoc/>
    public override IEnumerable<CheckStep> Run(BridgeContext context)
    {
        this.wasOpen = window.IsOpen;
        window.IsOpen = true;
        var start = window.DrawCount;
        var waited = 0;
        while (window.DrawCount - start < 2)
        {
            if (++waited > FramesToWait)
            {
                yield return CheckStep.Fail($"Settings window didn't draw within {FramesToWait} frames.");
                yield break;
            }

            yield return CheckStep.NextFrame;
        }

        window.IsOpen = this.wasOpen;
        save();
        yield return VerifyFile();
    }

    /// <inheritdoc/>
    public override void Cleanup(BridgeContext context) => window.IsOpen = this.wasOpen;

    /// <summary>Reads the saved file back and compares it with the live settings.</summary>
    private CheckStep VerifyFile()
    {
        if (!File.Exists(configFile))
        {
            return CheckStep.Fail($"Saving didn't write {configFile}.");
        }

        using var json = JsonDocument.Parse(File.ReadAllText(configFile));
        var root = json.RootElement;
        if (!root.TryGetProperty("CubeEdge", out var edge) || edge.GetSingle() != config.CubeEdge)
        {
            return CheckStep.Fail("The saved file doesn't hold the current cube size.");
        }

        if (!root.TryGetProperty("Version", out var version) || version.GetInt32() != Configuration.CurrentVersion)
        {
            return CheckStep.Fail("The saved file doesn't hold the current version.");
        }

        return root.TryGetProperty("Revision", out _)
            ? CheckStep.Fail("The in-memory revision counter was saved to the file.")
            : CheckStep.Pass($"Settings window drew; saved file holds cube size {config.CubeEdge} and version {Configuration.CurrentVersion}.");
    }
}
