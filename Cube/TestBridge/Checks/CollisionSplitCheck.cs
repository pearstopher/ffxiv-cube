using System;
using System.Collections.Generic;
using System.Globalization;
using Cube.Core;

namespace Cube.TestBridge.Checks;

/// <summary>
/// Checks collision node boxes are split at gaps in the running game, lazily and within the budget (#127; PROJECT.md,
/// "Collision (phase 2)" and "Efficiency and memory").
/// </summary>
/// <remarks>
/// <para>
/// Starts from an empty piece cache and keeps gathering (the reader asks for another gather by itself while nodes are
/// still coarse) until a gather leaves the cache unchanged, at the largest cube and full detail. Pass criteria: the
/// refining settles within <see cref="MaxGathers"/> gathers; the piece cache stays within its fixed capacity; the floor
/// under the player (the game's own downward raycast, as in <see cref="CollisionDrawCheck"/>) is still inside a
/// gathered box after refining, so splitting left no hole under the player; and no gathering frame allocates.
/// </para>
/// <para>
/// The box count and the piece counts are reported as metrics. Comparing refined boxes with ray hits over a whole
/// region (the #125 probe's measure) is left to <see cref="CollisionFitProbeCheck"/>; the visual check is a manual step
/// in <c>docs/manual-tests.md</c>.
/// </para>
/// </remarks>
internal sealed class CollisionSplitCheck : BridgeCheck
{
    /// <summary>Most gathers the refining may take to settle (300 triangles a frame; a busy place needs a few dozen).</summary>
    private const int MaxGathers = 60;

    /// <summary>The user's settings before the check, restored by <see cref="Cleanup"/>.</summary>
    private readonly CubeSettings saved = new();

    /// <summary>The window's open state before the check, restored by <see cref="Cleanup"/>.</summary>
    private bool wasOpen;

    /// <inheritdoc/>
    public override string Name => "collision-split";

    /// <inheritdoc/>
    public override TimeSpan Timeout => TimeSpan.FromSeconds(90);

    /// <inheritdoc/>
    public override IEnumerable<CheckStep> Run(BridgeContext context)
    {
        var window = context.Window;
        var cache = window.Collision.Cache;
        var pieces = window.Collision.NodePieces;
        this.saved.CopyFrom(window.Settings);
        this.wasOpen = window.IsOpen;
        window.IsOpen = true;
        window.Settings.ShowCollision = true;
        window.Settings.CollisionDetail = CollisionDetail.Full;
        window.Settings.CubeSize = new System.Numerics.Vector3(BoxExtents.Max);
        window.Settings.Changed();

        pieces.Clear();
        var startBytes = window.GatherCost.LifetimeAllocatedBytes;
        var gathers = 0;
        var settled = false;
        var coarseBoxes = 0;
        while (gathers < MaxGathers && !settled)
        {
            var before = pieces.Pieces;
            foreach (var step in WorstCase.WaitForCollision(cache))
            {
                yield return step;
            }

            gathers++;
            coarseBoxes = gathers == 1 ? cache.Ready.Count : coarseBoxes;

            // Let the reader's own follow-up gather (if it asked for one) begin before judging.
            yield return CheckStep.NextFrame;
            yield return CheckStep.NextFrame;
            settled = pieces.Pieces == before && !cache.IsGathering;
        }

        var gatherBytes = window.GatherCost.LifetimeAllocatedBytes - startBytes;
        context.Metric("gathersToSettle", gathers);
        context.Metric("nodesSplit", pieces.Nodes);
        context.Metric("piecesKept", pieces.Pieces);
        context.Metric("pieceCapacity", pieces.Capacity);
        context.Metric("firstGatherBoxes", coarseBoxes);
        context.Metric("settledBoxes", cache.Ready.Count);
        context.Metric("gatherBytes", gatherBytes);

        if (!settled)
        {
            yield return CheckStep.Fail($"Refining had not settled after {MaxGathers} gathers ({pieces.Pieces} pieces).");
            yield break;
        }

        if (pieces.Pieces > pieces.Capacity)
        {
            yield return CheckStep.Fail($"{pieces.Pieces} pieces kept, over the capacity of {pieces.Capacity}.");
            yield break;
        }

        if (!Game.PlayerReader.TryGetLocalPosition(out var feet))
        {
            yield return CheckStep.Fail("Not logged in: no local player. Log in and leave the game running.");
            yield break;
        }

        var floor = CollisionReaderCheck.FindFloor(feet, cache.Ready.Items, out var hit, out _);
        var summary = string.Create(
            CultureInfo.InvariantCulture,
            $"{pieces.Nodes} nodes refined into {pieces.Pieces} pieces within {gathers} gathers; boxes {coarseBoxes} on the first gather, {cache.Ready.Count} settled; {gatherBytes} bytes allocated gathering (budget 0).");
        if (floor == CollisionReaderCheck.Floor.NoRayHit)
        {
            yield return CheckStep.Fail("The game's raycast found no floor under the player (flying or swimming?). Stand on the ground and run again.");
        }
        else if (floor != CollisionReaderCheck.Floor.InABox)
        {
            yield return CheckStep.Fail(string.Create(CultureInfo.InvariantCulture, $"After splitting, the floor under the player ({hit}) is in none of the {cache.Ready.Count} boxes. {summary}"));
        }
        else
        {
            yield return gatherBytes == 0 ? CheckStep.Pass(summary) : CheckStep.Fail(summary);
        }
    }

    /// <inheritdoc/>
    public override void Cleanup(BridgeContext context)
    {
        context.Window.Settings.CopyFrom(this.saved);
        context.Window.IsOpen = this.wasOpen;
    }
}
