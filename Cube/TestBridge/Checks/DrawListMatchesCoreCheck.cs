using System;
using System.Collections.Generic;
using Cube.Core;

namespace Cube.TestBridge.Checks;

/// <summary>
/// Checks the window drew exactly what Core produced (PROJECT.md, "Automated testing": the primitives drawn
/// match Core's output): rebuilds the last frame independently from the same cube space and projection,
/// compares it primitive by primitive (and each polygon's corners, #93), and checks one ImGui call was issued per
/// primitive.
/// </summary>
/// <remarks>
/// The frame is the wireframe (12 edges) plus one solid cube per player in the window's player set (#17) and a
/// label per other player (#18), and the collision boxes clipped to the cube (#31).
/// Later features extend what <see cref="CubeWindow.BuildFrame"/> builds, not this comparison.
/// </remarks>
internal sealed class DrawListMatchesCoreCheck : BridgeCheck
{
    /// <summary>Frames to wait for the window to draw after opening it.</summary>
    private const int FramesToWait = 30;

    /// <summary>An independent list to rebuild the frame into; allocated once per check run.</summary>
    private readonly DrawList expected = new(CubeWindow.DrawListCapacity, CubeWindow.DrawListPointCapacity);

    /// <summary>The window's open state before the check, restored by <see cref="Cleanup"/>.</summary>
    private bool wasOpen;

    /// <inheritdoc/>
    public override string Name => "drawn-primitives-match-core";

    /// <inheritdoc/>
    public override IEnumerable<CheckStep> Run(BridgeContext context)
    {
        var window = context.Window;
        this.wasOpen = window.IsOpen;
        window.IsOpen = true;

        // Wait for two frames drawn after opening, so the comparison isn't against a frame from before.
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

        var actual = window.DrawList;
        CubeWindow.BuildFrame(this.expected, window.LastSpace, window.LastProjection, window.Players.Items, window.Palette, window.ShowLabels, window.WireColour, window.ClippedCollision.Items, window.CollisionColour, window.ShowOwnName, window.NamePosition);

        context.Metric("primitives", actual.Count);
        context.Metric("submitted", window.LastSubmitted);
        context.Metric("dropped", actual.Dropped);
        context.Metric("pixelsPerYalm", window.LastProjection.PixelsPerYalm);

        if (window.LastSubmitted != actual.Count)
        {
            yield return CheckStep.Fail($"Issued {window.LastSubmitted} ImGui calls for {actual.Count} primitives.");
            yield break;
        }

        if (actual.Count != this.expected.Count)
        {
            yield return CheckStep.Fail($"Window drew {actual.Count} primitives; Core builds {this.expected.Count} from the same inputs.");
            yield break;
        }

        var actualItems = actual.Items;
        var expectedItems = this.expected.Items;
        for (var i = 0; i < actualItems.Length; i++)
        {
            if (!actualItems[i].Equals(expectedItems[i]) || !actual.Points(actualItems[i]).SequenceEqual(this.expected.Points(expectedItems[i])))
            {
                yield return CheckStep.Fail($"Primitive {i} differs from Core's: {actualItems[i].Kind} at {actualItems[i].A} vs {expectedItems[i].Kind} at {expectedItems[i].A}.");
                yield break;
            }
        }

        if (window.LastProjection.PixelsPerYalm <= 0)
        {
            yield return CheckStep.Fail("The cube was fitted to an empty region (scale 0).");
            yield break;
        }

        yield return CheckStep.Pass($"{actual.Count} primitives drawn, identical to Core's, one ImGui call each.");
    }

    /// <inheritdoc/>
    public override void Cleanup(BridgeContext context) => context.Window.IsOpen = this.wasOpen;
}
