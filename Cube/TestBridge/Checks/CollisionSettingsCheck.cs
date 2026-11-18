using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using Cube.Core;

namespace Cube.TestBridge.Checks;

/// <summary>
/// Checks the collision settings in the running game (#32: collision on/off and detail level, saved and applied
/// immediately; PROJECT.md, "Collision (phase 2)").
/// </summary>
/// <remarks>
/// <para>
/// From the defaults (full detail) it gathers once, then for each coarser level: changing the setting starts a new
/// gather at once while the old boxes stay drawn, and when it completes every full-detail box lies inside one of the
/// new, bigger boxes. That proves the reader's assumption (<see cref="CollisionDetail"/>) that a mesh node's box holds
/// its children's, so a coarser level never hides anything the player can bump into. Coarse must also give no more
/// boxes than full.
/// </para>
/// <para>
/// Then collision off: no boxes are clipped or drawn and no gather runs; and back on: boxes are gathered and drawn
/// again. Settings change in memory only (saving is the settings window's job, covered by <c>docs/manual-tests.md</c>)
/// and are restored in <see cref="Cleanup"/>.
/// </para>
/// </remarks>
internal sealed class CollisionSettingsCheck : BridgeCheck
{
    /// <summary>How far a full-detail box's centre may sit outside a coarser box, in yalms (rounding).</summary>
    private const float Tolerance = 0.05f;

    /// <summary>Frames collision stays off before checking that no gather ran.</summary>
    private const int FramesOff = 10;

    /// <summary>The user's settings before the check, restored by <see cref="Cleanup"/>.</summary>
    private readonly CubeSettings saved = new();

    /// <summary>The window's open state before the check, restored by <see cref="Cleanup"/>.</summary>
    private bool wasOpen;

    /// <inheritdoc/>
    public override string Name => "collision-settings";

    /// <inheritdoc/>
    /// <remarks>Four gathers, each possibly spread over a few frames at background frame rates (about 10 fps).</remarks>
    public override TimeSpan Timeout => TimeSpan.FromSeconds(60);

    /// <inheritdoc/>
    public override IEnumerable<CheckStep> Run(BridgeContext context)
    {
        var window = context.Window;
        var settings = window.Settings;
        var cache = window.Collision.Cache;
        this.saved.CopyFrom(settings);
        this.wasOpen = window.IsOpen;
        settings.CopyFrom(new CubeSettings());
        window.IsOpen = true;

        // A fresh full-detail gather around where the player stands now, even if the defaults were already in use.
        cache.Invalidate();
        foreach (var step in WaitForGather(cache, cache.Completed))
        {
            yield return step;
        }

        var full = cache.Ready.Items.ToArray();
        context.Metric("fullBoxes", full.Length);
        if (full.Length == 0)
        {
            yield return CheckStep.Fail("No collision gathered at full detail. Stand somewhere with walls or floor and run again.");
            yield break;
        }

        var counts = new int[3];
        counts[(int)CollisionDetail.Full] = full.Length;
        foreach (var detail in new[] { CollisionDetail.Medium, CollisionDetail.Coarse })
        {
            var before = cache.Completed;
            settings.CollisionDetail = detail;
            settings.Changed();
            foreach (var step in WaitForDraws(window))
            {
                yield return step;
            }

            // Applied at once: the new level is gathering (or already done), and the cube never went blank meanwhile.
            if (!cache.IsGathering && cache.Completed == before)
            {
                yield return CheckStep.Fail($"Detail {detail} didn't start a new gather.");
                yield break;
            }

            if (!cache.Ready.HasData)
            {
                yield return CheckStep.Fail($"Changing to detail {detail} dropped the drawn boxes before the new ones were ready.");
                yield break;
            }

            foreach (var step in WaitForGather(cache, before))
            {
                yield return step;
            }

            var coarser = cache.Ready;
            counts[(int)detail] = coarser.Count;
            context.Metric(detail + "Boxes", coarser.Count);
            var problem = CheckCovered(full, coarser.Items, coarser.Region, detail);
            if (problem is not null)
            {
                yield return CheckStep.Fail(problem);
                yield break;
            }
        }

        if (counts[(int)CollisionDetail.Coarse] > counts[(int)CollisionDetail.Full])
        {
            yield return CheckStep.Fail($"Coarse detail gave {counts[(int)CollisionDetail.Coarse]} boxes, more than full's {counts[(int)CollisionDetail.Full]}.");
            yield break;
        }

        // Collision off: nothing gathered, clipped or drawn.
        settings.ShowCollision = false;
        settings.Changed();
        foreach (var step in WaitForDraws(window))
        {
            yield return step;
        }

        var completedWhileOff = cache.Completed;
        var start = window.DrawCost.TotalRecorded;
        while (window.DrawCost.TotalRecorded - start < FramesOff)
        {
            yield return CheckStep.NextFrame;
        }

        var drawnWhileOff = CountCollisionFaces(window);
        if (window.ClippedCollision.Count != 0 || drawnWhileOff != 0 || cache.Ready.HasData || cache.IsGathering || cache.Completed != completedWhileOff)
        {
            yield return CheckStep.Fail($"Collision off but {window.ClippedCollision.Count} boxes clipped, {drawnWhileOff} faces drawn, gathering {cache.IsGathering}, {cache.Completed - completedWhileOff} gathers completed.");
            yield break;
        }

        // Back on: gathered and drawn again.
        settings.ShowCollision = true;
        settings.Changed();
        foreach (var step in WaitForGather(cache, cache.Completed))
        {
            yield return step;
        }

        foreach (var step in WaitForDraws(window))
        {
            yield return step;
        }

        var facesOn = CountCollisionFaces(window);
        context.Metric("facesBackOn", facesOn);
        yield return window.ClippedCollision.Count > 0 && facesOn > 0
            ? CheckStep.Pass(string.Create(
                CultureInfo.InvariantCulture,
                $"Boxes full {counts[0]}, medium {counts[1]}, coarse {counts[2]}; each level gathered at once and covers every full-detail box. Off: nothing drawn or gathered; on again: {window.ClippedCollision.Count} boxes, {facesOn} faces."))
            : CheckStep.Fail("Collision turned back on but nothing was drawn.");
    }

    /// <inheritdoc/>
    public override void Cleanup(BridgeContext context)
    {
        context.Window.Settings.CopyFrom(this.saved);
        context.Window.IsOpen = this.wasOpen;
    }

    /// <summary>Waits until a gather begun after <paramref name="completedBefore"/> gathers has finished.</summary>
    /// <param name="cache">The collision cache.</param>
    /// <param name="completedBefore">The cache's completed count before the change.</param>
    private static IEnumerable<CheckStep> WaitForGather(CollisionCache cache, long completedBefore)
    {
        while (cache.Completed == completedBefore || cache.IsGathering || !cache.Ready.HasData)
        {
            yield return CheckStep.NextFrame;
        }
    }

    /// <summary>
    /// Checks every full-detail box whose centre is inside the coarser gather's region has its centre inside one of
    /// the coarser boxes: a coarser level only loosens the shape, it never loses anything.
    /// </summary>
    /// <param name="full">The full-detail boxes.</param>
    /// <param name="coarser">The coarser level's boxes.</param>
    /// <param name="region">The region the coarser level was gathered for.</param>
    /// <param name="detail">The coarser level, for the message.</param>
    /// <returns>What was wrong, or null.</returns>
    private static string? CheckCovered(CollisionBox[] full, ReadOnlySpan<CollisionBox> coarser, CollisionBox region, CollisionDetail detail)
    {
        foreach (var box in full)
        {
            var centre = box.Centre;
            if (!Inside(centre, region, 0f))
            {
                continue;
            }

            var covered = false;
            for (var i = 0; i < coarser.Length && !covered; i++)
            {
                covered = Inside(centre, coarser[i], Tolerance);
            }

            if (!covered)
            {
                return string.Create(CultureInfo.InvariantCulture, $"Full-detail box {box.Min}-{box.Max} is inside none of the {coarser.Length} boxes at {detail} detail.");
            }
        }

        return null;
    }

    /// <summary>Whether a point is inside a box, allowing a tolerance.</summary>
    /// <param name="point">The point.</param>
    /// <param name="box">The box.</param>
    /// <param name="tolerance">How far outside still counts, in yalms.</param>
    private static bool Inside(Vector3 point, in CollisionBox box, float tolerance) =>
        point.X >= box.Min.X - tolerance && point.X <= box.Max.X + tolerance &&
        point.Y >= box.Min.Y - tolerance && point.Y <= box.Max.Y + tolerance &&
        point.Z >= box.Min.Z - tolerance && point.Z <= box.Max.Z + tolerance;

    /// <summary>Counts the collision faces in the last frame.</summary>
    /// <param name="window">The cube window.</param>
    private static int CountCollisionFaces(CubeWindow window)
    {
        var count = 0;
        foreach (var p in window.DrawList.Items)
        {
            if (CollisionCubes.IsCollisionTag(p.Tag))
            {
                count++;
            }
        }

        return count;
    }
}
