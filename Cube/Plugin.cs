using System;
using Cube.Core;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace Cube;

/// <summary>
/// Entry point. Registers the <c>/cube</c> command and the cube window, and logs which build loaded.
/// </summary>
/// <remarks>
/// The plugin only reads game state and draws it in its own window (PROJECT.md, "Goal"):
/// it never changes the game, hooks nothing it doesn't need, and sends nothing.
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

#if DEBUG
    /// <summary>Drives the in-game test bridge's checks, one step per frame. Debug builds only.</summary>
    [PluginService] internal static IFramework Framework { get; private set; } = null!;

    /// <summary>The object table, read by the bridge's probe as an independent source. Debug builds only, for now.</summary>
    [PluginService] internal static IObjectTable ObjectTable { get; private set; } = null!;

    /// <summary>Login and GPose state, read by the bridge's probe. Debug builds only, for now.</summary>
    [PluginService] internal static IClientState ClientState { get; private set; } = null!;

    /// <summary>Condition flags (loading screens, cutscenes), read by the bridge's probe. Debug builds only, for now.</summary>
    [PluginService] internal static ICondition Condition { get; private set; } = null!;

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

#if DEBUG
    /// <summary>The in-game test bridge (PROJECT.md, "Automated testing"). Debug builds only.</summary>
    private readonly TestBridge.BridgeRunner? testBridge;
#endif

    /// <summary>Creates the window and command, then logs the build line.</summary>
    public Plugin()
    {
        this.cubeWindow = new CubeWindow();
        this.windowSystem.AddWindow(this.cubeWindow);

        PluginInterface.UiBuilder.Draw += this.windowSystem.Draw;
        PluginInterface.UiBuilder.OpenMainUi += this.ToggleCubeWindow;

        CommandManager.AddHandler(CommandName, new CommandInfo(this.OnCommand)
        {
            HelpMessage = "Show or hide the cube window.",
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
            this.testBridge.Register(new TestBridge.Checks.DrawBudgetCheck());
            this.testBridge.Register(new TestBridge.Checks.FrameSnapshotCheck(this.testBridge.Folder));
            this.testBridge.Register(new TestBridge.Checks.GameStateProbeCheck(ObjectTable, ClientState, Condition, Log));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Test bridge failed to start");
        }
#endif
    }

    /// <summary>Unregisters everything, each part even if an earlier one fails.</summary>
    public void Dispose()
    {
#if DEBUG
        DisposeSafely(() => this.testBridge?.Dispose(), "test bridge");
#endif
        DisposeSafely(() => CommandManager.RemoveHandler(CommandName), "command");
        DisposeSafely(() => PluginInterface.UiBuilder.OpenMainUi -= this.ToggleCubeWindow, "main UI callback");
        DisposeSafely(() => PluginInterface.UiBuilder.Draw -= this.windowSystem.Draw, "draw callback");
        DisposeSafely(this.windowSystem.RemoveAllWindows, "windows");
        Log.Information($"{PluginInterface.Manifest.Name} unloaded");
    }

    /// <summary>Handles <c>/cube</c>: toggles the window.</summary>
    /// <param name="command">The command typed.</param>
    /// <param name="args">Anything typed after the command (unused for now).</param>
    private void OnCommand(string command, string args) => this.ToggleCubeWindow();

    /// <summary>Shows the window if hidden, hides it if shown.</summary>
    private void ToggleCubeWindow() => this.cubeWindow.Toggle();

    /// <summary>Runs one cleanup step and logs, rather than throws, if it fails.</summary>
    /// <param name="step">The cleanup to run.</param>
    /// <param name="name">What is being cleaned up, for the log.</param>
    private static void DisposeSafely(Action step, string name)
    {
        try
        {
            step();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error unloading {Part}", name);
        }
    }
}
