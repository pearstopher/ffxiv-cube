using System;
using Cube.Core;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace Cube;

/// <summary>
/// Entry point. Registers the <c>/cube</c> command, the cube and settings windows, and logs which build loaded.
/// </summary>
/// <remarks>
/// The plugin only reads game state and draws it in its own window (PROJECT.md, "Goal"):
/// it never changes the game, hooks nothing, and sends nothing.
/// Everything registered here is released in <see cref="Dispose"/>.
/// </remarks>
public sealed class Plugin : IDalamudPlugin
{
    /// <summary>The slash command that toggles the cube window.</summary>
    internal const string CommandName = "/cube";

    /// <summary>Dalamud's interface for this plugin: UI callbacks, manifest, file locations.</summary>
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    /// <summary>The plugin's log, written to <c>/xllog</c> and <c>dalamud.log</c>.</summary>
    [PluginService] internal static IPluginLog Log { get; private set; } = null!;
    /// <summary>Registers the <c>/cube</c> command.</summary>
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    /// <summary>Login, GPose and PvP state: the cube is hidden when logged out, in GPose or in PvP (PROJECT.md, "When hidden").</summary>
    [PluginService] internal static IClientState ClientState { get; private set; } = null!;
    /// <summary>Condition flags: the cube is hidden during loading screens and cutscenes.</summary>
    [PluginService] internal static ICondition Condition { get; private set; } = null!;

#if DEBUG
    /// <summary>Drives the in-game test bridge's checks, one step per frame. Debug builds only.</summary>
    [PluginService] internal static IFramework Framework { get; private set; } = null!;

    /// <summary>
    /// The object table, read by the bridge's checks and probe as a second, independent source to compare the direct
    /// read in <see cref="Game.PlayerReader"/> against. Debug builds only: the plugin itself never uses it (#10).
    /// </summary>
    [PluginService] internal static IObjectTable ObjectTable { get; private set; } = null!;

    /// <summary>The build configuration, shown in the load line.</summary>
    private const string BuildConfiguration = "Debug";
#else
    /// <summary>The build configuration, shown in the load line.</summary>
    private const string BuildConfiguration = "Release";
#endif

    /// <summary>Dalamud's window manager for this plugin's windows.</summary>
    private readonly WindowSystem windowSystem = new("Cube");

    /// <summary>The cube window.</summary>
    private readonly CubeWindow cubeWindow;

    /// <summary>The settings window (<c>/cube config</c>).</summary>
    private readonly SettingsWindow settingsWindow;

    /// <summary>The saved settings, shared by both windows.</summary>
    private readonly Configuration configuration;

    /// <summary>Exceptions caught in the command and the Dalamud UI buttons.</summary>
    private readonly ErrorReporter uiErrors = new("Command or UI button");

    /// <summary>How many unload steps failed; reported in the unload line.</summary>
    private int unloadErrors;

#if DEBUG
    /// <summary>The in-game test bridge (PROJECT.md, "Automated testing"). Debug builds only.</summary>
    private readonly TestBridge.BridgeRunner? testBridge;
#endif

    /// <summary>Creates the window and command, then logs the build line.</summary>
    public Plugin()
    {
        this.configuration = Configuration.Load(PluginInterface);
        this.cubeWindow = new CubeWindow(this.configuration, new Game.GameStateReader(ClientState, Condition), this.SaveConfiguration);
        this.settingsWindow = new SettingsWindow(this.configuration, this.SaveConfiguration);
        this.windowSystem.AddWindow(this.cubeWindow);
        this.windowSystem.AddWindow(this.settingsWindow);

#if DEBUG
        // Debug builds open the cube window on every load, so after each automatic reload the owner sees the
        // change straight away in game. Release builds start closed; players open it with /cube.
        this.cubeWindow.IsOpen = true;
#endif

        PluginInterface.UiBuilder.Draw += this.windowSystem.Draw;
        PluginInterface.UiBuilder.OpenMainUi += this.ToggleCubeWindow;
        PluginInterface.UiBuilder.OpenConfigUi += this.ToggleSettingsWindow;

        CommandManager.AddHandler(CommandName, new CommandInfo(this.OnCommand)
        {
            HelpMessage = "Show or hide the cube window. \"/cube config\" opens the settings.",
        });

        // Logs which build is running, so a stale or wrong-configuration build is obvious in /xllog.
        var dll = PluginInterface.AssemblyLocation.FullName;
        var builtAt = BuildInfo.ReadBuildTime(dll);
        var build = BuildInfo.Describe(
            BuildConfiguration,
            typeof(Plugin).Assembly.GetName().Version ?? new Version(0, 0),
            builtAt,
            dll);
        Log.Information($"{PluginInterface.Manifest.Name} loaded ({build})");

#if DEBUG
        // A broken bridge mustn't stop the plugin loading; the missing results file shows it.
        try
        {
            this.testBridge = new TestBridge.BridgeRunner(
                Framework,
                Log,
                PluginInterface.ConfigDirectory.FullName,
                new TestBridge.BridgeContext(this.cubeWindow, dll, builtAt));
            this.testBridge.Register(new TestBridge.Checks.BuildStampCheck());
            this.testBridge.Register(new TestBridge.Checks.WindowOpenCloseCheck());
            this.testBridge.Register(new TestBridge.Checks.DrawListMatchesCoreCheck());
            this.testBridge.Register(new TestBridge.Checks.PlayersMatchObjectTableCheck(ObjectTable));
            this.testBridge.Register(new TestBridge.Checks.FakePlayersCheck());
            this.testBridge.Register(new TestBridge.Checks.PlayerCubesCheck());
            this.testBridge.Register(new TestBridge.Checks.LabelsCheck());
            this.testBridge.Register(new TestBridge.Checks.OrientationCheck());
            this.testBridge.Register(new TestBridge.Checks.ViewControlCheck(this.testBridge.Folder));
            this.testBridge.Register(new TestBridge.Checks.SettingsCheck());
            this.testBridge.Register(new TestBridge.Checks.BoxSizeCheck());
            this.testBridge.Register(new TestBridge.Checks.PlayerHeightCheck());
            this.testBridge.Register(new TestBridge.Checks.HiddenStatesCheck());
            this.testBridge.Register(new TestBridge.Checks.SettingsWindowCheck(this.settingsWindow, this.configuration, this.SaveConfiguration, PluginInterface.ConfigFile.FullName));
            this.testBridge.Register(new TestBridge.Checks.DrawBudgetCheck());
            this.testBridge.Register(new TestBridge.Checks.FrameSnapshotCheck(this.testBridge.Folder));
            this.testBridge.Register(new TestBridge.Checks.GameStateProbeCheck(ObjectTable, ClientState, Condition, Log));
            this.testBridge.Register(new TestBridge.Checks.CollisionProbeCheck(Log));
            this.testBridge.Register(new TestBridge.Checks.CollisionReaderCheck());
            this.testBridge.Register(new TestBridge.Checks.CollisionDrawCheck());
            this.testBridge.Register(new TestBridge.Checks.CollisionInsideCubeCheck());
            this.testBridge.Register(new TestBridge.Checks.CollisionSettingsCheck());
            this.testBridge.Register(new TestBridge.Checks.CollisionZoneChangesCheck());
            this.testBridge.Register(new TestBridge.Checks.SoakCheck());
            this.testBridge.Register(new TestBridge.Checks.NoErrorsCaughtCheck(this.cubeWindow.Errors, this.settingsWindow.Errors, this.uiErrors));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Test bridge failed to start");
        }
#endif
    }

    /// <summary>
    /// Unregisters everything the constructor registered (the command, the three UI callbacks, both windows, and in
    /// Debug the bridge's framework hook and file watcher), each part even if an earlier one fails. The plugin changes
    /// nothing in the game, so there is nothing else to restore.
    /// </summary>
    /// <remarks>
    /// The last line is <c>Cube unloaded</c>, or <c>Cube unloaded with N error(s)</c> if any step failed; the in-game
    /// test script fails on the latter.
    /// </remarks>
    public void Dispose()
    {
#if DEBUG
        this.DisposeSafely(() => this.testBridge?.Dispose(), "test bridge");
#endif
        this.DisposeSafely(() => CommandManager.RemoveHandler(CommandName), "command");
        this.DisposeSafely(() => PluginInterface.UiBuilder.OpenMainUi -= this.ToggleCubeWindow, "main UI callback");
        this.DisposeSafely(() => PluginInterface.UiBuilder.OpenConfigUi -= this.ToggleSettingsWindow, "settings UI callback");
        this.DisposeSafely(() => PluginInterface.UiBuilder.Draw -= this.windowSystem.Draw, "draw callback");
        this.DisposeSafely(this.windowSystem.RemoveAllWindows, "windows");
        Log.Information(this.unloadErrors == 0
            ? $"{PluginInterface.Manifest.Name} unloaded"
            : $"{PluginInterface.Manifest.Name} unloaded with {this.unloadErrors} error(s)");
    }

    /// <summary>Handles <c>/cube</c> (toggles the cube window) and <c>/cube config</c> (toggles the settings).</summary>
    /// <param name="command">The command typed.</param>
    /// <param name="args">Anything typed after the command.</param>
    private void OnCommand(string command, string args)
    {
        try
        {
            if (CommandArgs.IsConfig(args))
            {
                this.settingsWindow.Toggle();
            }
            else
            {
                this.cubeWindow.Toggle();
            }
        }
        catch (Exception ex)
        {
            this.uiErrors.Report(ex);
        }
    }

    /// <summary>Shows the cube window if hidden, hides it if shown (Dalamud's "open" button for the plugin).</summary>
    private void ToggleCubeWindow()
    {
        try
        {
            this.cubeWindow.Toggle();
        }
        catch (Exception ex)
        {
            this.uiErrors.Report(ex);
        }
    }

    /// <summary>Shows or hides the settings window (Dalamud's settings button for the plugin).</summary>
    private void ToggleSettingsWindow()
    {
        try
        {
            this.settingsWindow.Toggle();
        }
        catch (Exception ex)
        {
            this.uiErrors.Report(ex);
        }
    }

    /// <summary>Writes the settings to disk, logging rather than throwing if that fails.</summary>
    private void SaveConfiguration()
    {
        try
        {
            this.configuration.Save(PluginInterface);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Couldn't save settings");
        }
    }

    /// <summary>Runs one cleanup step and logs and counts, rather than throws, if it fails.</summary>
    /// <param name="step">The cleanup to run.</param>
    /// <param name="name">What is being cleaned up, for the log.</param>
    private void DisposeSafely(Action step, string name)
    {
        try
        {
            step();
        }
        catch (Exception ex)
        {
            this.unloadErrors++;
            Log.Error(ex, "Error unloading {Part}", name);
        }
    }
}
