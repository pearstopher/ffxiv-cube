using System.Collections.Generic;

namespace Cube.TestBridge.Checks;

/// <summary>
/// Checks fake players reach the cube like real ones (PROJECT.md, "Automated testing": synthetic players so rendering
/// checks work with nobody nearby; "Players": inside shown, outside not at all): with the standard fakes enabled, every
/// fake inside the cube is in the window's player set with its relation, and the one outside isn't.
/// </summary>
internal sealed class FakePlayersCheck : BridgeCheck
{
    /// <summary>Frames to wait for the window to draw.</summary>
    private const int FramesToWait = 30;

    /// <summary>The window's open state before the check.</summary>
    private bool wasOpen;

    /// <inheritdoc/>
    public override string Name => "fake-players-reach-the-cube";

    /// <inheritdoc/>
    public override IEnumerable<CheckStep> Run(BridgeContext context)
    {
        var window = context.Window;
        this.wasOpen = window.IsOpen;
        window.IsOpen = true;
        window.Fakes.UseStandardSet();

        var start = window.DrawCost.TotalRecorded;
        var waited = 0;
        while (window.DrawCost.TotalRecorded - start < 2)
        {
            if (++waited > FramesToWait)
            {
                yield return CheckStep.Fail($"Window didn't draw within {FramesToWait} frames (game UI hidden?).");
                yield break;
            }

            yield return CheckStep.NextFrame;
        }

        var found = 0;
        foreach (var player in window.Players.Items)
        {
            if (player.Slot < FakePlayers.FirstSlot)
            {
                continue;
            }

            var index = player.Slot - FakePlayers.FirstSlot;
            var (name, _, relation) = FakePlayers.Standard[index];
            if (name == "Fake Outside")
            {
                yield return CheckStep.Fail("The fake player outside the cube is in the window's players.");
                yield break;
            }

            if (player.Relation != relation || player.Name != name)
            {
                yield return CheckStep.Fail($"Fake {index} arrived as {player.Relation} '{player.Name}', expected {relation} '{name}'.");
                yield break;
            }

            found++;
        }

        context.Metric("fakesInside", found);
        yield return found == FakePlayers.StandardInside
            ? CheckStep.Pass($"All {found} fakes inside the cube arrived with the right relation and name; the outside one didn't.")
            : CheckStep.Fail($"{found} of {FakePlayers.StandardInside} fakes inside the cube arrived.");
    }

    /// <inheritdoc/>
    public override void Cleanup(BridgeContext context)
    {
        context.Window.Fakes.Clear();
        context.Window.IsOpen = this.wasOpen;
    }
}
