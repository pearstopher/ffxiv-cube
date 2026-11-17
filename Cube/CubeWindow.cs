using System.Numerics;
using Dalamud.Interface.Windowing;
#if DEBUG
using System;
using System.Diagnostics;
using Cube.Core;
#endif

namespace Cube;

/// <summary>
/// The window the cube is drawn in (PROJECT.md, "Features": a Dalamud window toggled by <c>/cube</c>).
/// </summary>
/// <remarks>Empty for now; drawing arrives with the projection and draw list work.</remarks>
internal sealed class CubeWindow : Window
{
    /// <summary>Creates the window, closed, with a sensible minimum size.</summary>
    public CubeWindow()
        : base("Cube###CubeMain")
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(200, 200),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue),
        };
    }

#if DEBUG
    /// <summary>
    /// Time and allocations of each <see cref="Draw"/> call, read by the in-game test bridge to check
    /// PROJECT.md's budgets (under 0.5 ms average, zero bytes per frame). Debug builds only.
    /// </summary>
    /// <remarks>Holds the last 240 frames (about 4 seconds at 60 fps); fixed size, so measuring allocates nothing.</remarks>
    internal DrawCostSampler DrawCost { get; } = new(240, Stopwatch.Frequency);
#endif

    /// <summary>Draws the window's contents, measuring the cost in Debug builds.</summary>
    public override void Draw()
    {
#if DEBUG
        var startBytes = GC.GetAllocatedBytesForCurrentThread();
        var startTicks = Stopwatch.GetTimestamp();
        this.DrawContents();
        var elapsed = Stopwatch.GetTimestamp() - startTicks;
        this.DrawCost.Record(elapsed, GC.GetAllocatedBytesForCurrentThread() - startBytes);
#else
        this.DrawContents();
#endif
    }

    /// <summary>The actual drawing: the plugin's per-frame draw path. Nothing yet.</summary>
    private void DrawContents()
    {
    }
}
