using System.Collections.Generic;

namespace Cube.TestBridge.Checks;

/// <summary>
/// Checks no game-facing handler has caught an exception since the plugin loaded (CLAUDE.md, "Dalamud rules": wrap
/// every handler in try/catch and log). Runs after the other checks, so it also covers everything they exercised.
/// </summary>
/// <remarks>
/// Handlers swallow exceptions so the game is never affected, which also means a bug could go unnoticed; this check
/// turns any caught exception into a failed run. Errors during unload and reload are checked by the test script in
/// <c>dalamud.log</c>, since the plugin can't observe its own unload.
/// </remarks>
/// <param name="reporters">Every handler's error reporter.</param>
internal sealed class NoErrorsCaughtCheck(params ErrorReporter[] reporters) : BridgeCheck
{
    /// <inheritdoc/>
    public override string Name => "no-errors-caught";

    /// <inheritdoc/>
    public override IEnumerable<CheckStep> Run(BridgeContext context)
    {
        foreach (var reporter in reporters)
        {
            if (reporter.Throttle.Total != 0)
            {
                yield return CheckStep.Fail($"{reporter.Where} caught {reporter.Throttle.Total} exception(s) since load; see dalamud.log.");
                yield break;
            }
        }

        yield return CheckStep.Pass($"No exceptions caught in {reporters.Length} handler groups since load.");
    }
}
