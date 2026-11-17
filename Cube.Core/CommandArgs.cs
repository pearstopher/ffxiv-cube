namespace Cube.Core;

/// <summary>Reads what was typed after <c>/cube</c> (PROJECT.md, "Features": <c>/cube</c> toggles the window, <c>/cube config</c> opens the settings).</summary>
public static class CommandArgs
{
    /// <summary>Whether the arguments ask for the settings: <c>config</c>, in any case, ignoring surrounding spaces.</summary>
    /// <param name="args">Everything after the command.</param>
    public static bool IsConfig(string? args) =>
        args is not null && args.AsSpan().Trim().Equals("config", StringComparison.OrdinalIgnoreCase);
}
