using System.Collections.Generic;
using Cube.Core;

namespace Cube.TestBridge.Checks;

/// <summary>
/// Checks the cube is hidden when logged out, during loading screens, cutscenes and GPose, and comes back
/// afterwards (PROJECT.md, "When hidden": the window is just empty then, never an error; "Efficiency and memory": no
/// work and no allocations while there's nothing to draw).
/// </summary>
/// <remarks>
/// A real cutscene or loading screen can't be started from a check, so each state is simulated through
/// <see cref="CubeWindow.SimulatedState"/>, which replaces only the game-state read; everything after it is the
/// real draw path. The real state is checked first: with the game in normal play the cube must be shown. The
/// condition flags behind the real read were confirmed by the #10 probe.
/// </remarks>
internal sealed class HiddenStatesCheck : BridgeCheck
{
    /// <summary>Frames measured in each hidden state for allocations.</summary>
    private const int FramesPerState = 5;

    /// <summary>The hidden states to simulate, with the reason each must report.</summary>
    private static readonly (GameState State, Visibility Expected)[] Hidden =
    [
        (GameState.Playing with { LoggedIn = false, HasLocalPlayer = false }, Visibility.NotLoggedIn),
        (GameState.Playing with { BetweenAreas = true }, Visibility.Loading),
        (GameState.Playing with { InCutscene = true }, Visibility.Cutscene),
        (GameState.Playing with { InGPose = true }, Visibility.GPose),
    ];

    /// <summary>The window's open state before the check.</summary>
    private bool wasOpen;

    /// <inheritdoc/>
    public override string Name => "hidden-when-logged-out-loading-cutscene-gpose";

    /// <inheritdoc/>
    public override IEnumerable<CheckStep> Run(BridgeContext context)
    {
        var window = context.Window;
        this.wasOpen = window.IsOpen;
        window.IsOpen = true;
        window.Fakes.UseStandardSet();
        foreach (var step in WaitForDraws(window))
        {
            yield return step;
        }

        if (window.LastVisibility != Visibility.Shown)
        {
            yield return CheckStep.Fail($"In normal play the cube is hidden ({window.LastVisibility}); expected it shown.");
            yield break;
        }

        foreach (var (state, expected) in Hidden)
        {
            window.SimulatedState = state;
            foreach (var step in WaitForDraws(window))
            {
                yield return step;
            }

            // Measure a few frames in the hidden state: nothing drawn, nothing allocated.
            window.DrawCost.Reset();
            while (window.DrawCost.Count < FramesPerState)
            {
                yield return CheckStep.NextFrame;
            }

            if (window.LastVisibility != expected)
            {
                yield return CheckStep.Fail($"Simulated {expected} reported {window.LastVisibility}.");
                yield break;
            }

            if (window.DrawList.Count != 0 || window.LastSubmitted != 0 || window.Players.Count != 0)
            {
                yield return CheckStep.Fail($"{expected}: still drew {window.LastSubmitted} primitives for {window.Players.Count} players.");
                yield break;
            }

            if (window.DrawCost.TotalAllocatedBytes != 0)
            {
                yield return CheckStep.Fail($"{expected}: allocated {window.DrawCost.TotalAllocatedBytes} bytes while hidden.");
                yield break;
            }
        }

        window.SimulatedState = null;
        foreach (var step in WaitForDraws(window))
        {
            yield return step;
        }

        context.Metric("primitivesAfter", window.DrawList.Count);
        yield return window.LastVisibility == Visibility.Shown && window.DrawList.Count > 12
            ? CheckStep.Pass($"Shown in normal play; empty and allocation-free when logged out, loading, in a cutscene or GPose; back afterwards ({window.DrawList.Count} primitives).")
            : CheckStep.Fail("The cube didn't come back after the hidden states ended.");
    }

    /// <inheritdoc/>
    public override void Cleanup(BridgeContext context)
    {
        context.Window.SimulatedState = null;
        context.Window.Fakes.Clear();
        context.Window.IsOpen = this.wasOpen;
    }
}
