using System;
using System.Collections.Generic;

namespace Cube.TestBridge;

/// <summary>
/// A named in-game check, registered with <see cref="BridgeRunner.Register"/>. Each feature that touches
/// the game adds checks for its behaviour (CLAUDE.md, "Building and testing"), so the running game
/// verifies it with no human in game.
/// </summary>
internal abstract class BridgeCheck
{
    /// <summary>Short unique name, shown in the results, e.g. <c>window-opens-and-closes</c>.</summary>
    public abstract string Name { get; }

    /// <summary>The longest the check may run before it fails as timed out.</summary>
    /// <remarks>Ten seconds covers checks that wait a few hundred frames; override for longer ones.</remarks>
    public virtual TimeSpan Timeout => TimeSpan.FromSeconds(10);

    /// <summary>
    /// Runs the check, one step per frame, on the framework thread. Yield <see cref="CheckStep.NextFrame"/>
    /// to wait a frame; finish with <see cref="CheckStep.Pass"/> or <see cref="CheckStep.Fail"/>.
    /// </summary>
    /// <param name="context">The plugin parts the check can look at, and where it records metrics.</param>
    public abstract IEnumerable<CheckStep> Run(BridgeContext context);

    /// <summary>
    /// Puts back anything the check changed (e.g. the window's open state). Called after every run,
    /// including when the check failed, threw or timed out, so a check never leaves the plugin altered.
    /// </summary>
    /// <param name="context">The same context the check ran with.</param>
    public virtual void Cleanup(BridgeContext context)
    {
    }

    /// <summary>
    /// Waits until the cube window has drawn two more frames, so a change made before the call is in the last
    /// frame. Ends with a failure step if it doesn't draw within <paramref name="maxFrames"/> frames (e.g. the
    /// game UI is hidden); the runner stops the check at that step.
    /// </summary>
    /// <param name="window">The cube window.</param>
    /// <param name="maxFrames">How many frames to wait at most.</param>
    protected static IEnumerable<CheckStep> WaitForDraws(CubeWindow window, int maxFrames = 30)
    {
        var start = window.DrawCost.TotalRecorded;
        var waited = 0;
        while (window.DrawCost.TotalRecorded - start < 2)
        {
            if (++waited > maxFrames)
            {
                yield return CheckStep.Fail($"Window didn't draw within {maxFrames} frames (game UI hidden?).");
                yield break;
            }

            yield return CheckStep.NextFrame;
        }
    }
}
