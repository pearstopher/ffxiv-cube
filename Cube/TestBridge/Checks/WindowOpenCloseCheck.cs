using System.Collections.Generic;

namespace Cube.TestBridge.Checks;

/// <summary>
/// Checks the cube window draws when opened and stops drawing when closed (PROJECT.md, "Features":
/// <c>/cube</c> toggles the window; "Efficiency": skip everything when the window is closed).
/// </summary>
/// <remarks>
/// Uses the window's draw counter (<see cref="Core.DrawCostSampler.TotalRecorded"/>) rather than ImGui
/// state, so it observes what actually ran. Dalamud hides plugin windows during cutscenes and GPose by
/// default, so this fails there; the message says so.
/// </remarks>
internal sealed class WindowOpenCloseCheck : BridgeCheck
{
    /// <summary>How many frames to wait for the window to draw after opening it.</summary>
    private const int FramesToWait = 30;

    /// <summary>The window's open state before the check, restored by <see cref="Cleanup"/>.</summary>
    private bool wasOpen;

    /// <inheritdoc/>
    public override string Name => "window-opens-and-closes";

    /// <inheritdoc/>
    public override IEnumerable<CheckStep> Run(BridgeContext context)
    {
        var window = context.Window;
        this.wasOpen = window.IsOpen;

        window.IsOpen = true;
        var before = window.DrawCost.TotalRecorded;
        var waited = 0;
        while (window.DrawCost.TotalRecorded == before)
        {
            if (++waited > FramesToWait)
            {
                yield return CheckStep.Fail($"Window opened but didn't draw within {FramesToWait} frames (game UI hidden? cutscene, GPose or loading screen).");
                yield break;
            }

            yield return CheckStep.NextFrame;
        }

        context.Metric("framesUntilDrawn", waited);

        window.IsOpen = false;

        // One frame for the close to take effect, then a few frames that must not draw.
        yield return CheckStep.NextFrame;
        var afterClose = window.DrawCost.TotalRecorded;
        for (var i = 0; i < 5; i++)
        {
            yield return CheckStep.NextFrame;
        }

        var drawnWhileClosed = window.DrawCost.TotalRecorded - afterClose;
        yield return drawnWhileClosed == 0
            ? CheckStep.Pass($"Drew {waited} frame(s) after opening; nothing drawn while closed.")
            : CheckStep.Fail($"Window drew {drawnWhileClosed} frame(s) while closed.");
    }

    /// <inheritdoc/>
    public override void Cleanup(BridgeContext context) => context.Window.IsOpen = this.wasOpen;
}
