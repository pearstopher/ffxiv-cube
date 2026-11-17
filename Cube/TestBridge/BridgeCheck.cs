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
}
