using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using Cube.Core;

namespace Cube.TestBridge.Checks;

/// <summary>
/// Checks the collision boxes are drawn in the running game (#31; PROJECT.md, "Collision (phase 2)": each collider
/// inside or crossing the cube is shown as a simplified box, clipped to the cube; "Draw order": far things first, with
/// the players).
/// </summary>
/// <remarks>
/// <para>
/// With the default settings (default view, from above), once a gather has completed: the cube holds clipped boxes,
/// every one inside the cube's height and footprint; every collision face belongs to one of them, at most three per
/// box, in the collision colour; and the floor under the player (found with the game's own downward raycast, as in
/// <see cref="CollisionReaderCheck"/>) is painted before the player's own cube, so the player stands on it rather than
/// under it. That the whole frame matches Core is <see cref="DrawListMatchesCoreCheck"/>'s job, and the snapshot
/// (<see cref="FrameSnapshotCheck"/>) shows the boxes for the side-by-side with vnavmesh's debug view in
/// <c>docs/manual-tests.md</c>.
/// </para>
/// </remarks>
internal sealed class CollisionDrawCheck : BridgeCheck
{
    /// <summary>Frames drawn after the gather before the frame is checked.</summary>
    private const int FramesAfterGather = 2;

    /// <summary>How far a clipped box may reach past the cube, in yalms (rounding).</summary>
    private const float Tolerance = 0.01f;

    /// <summary>The user's settings before the check, restored by <see cref="Cleanup"/>.</summary>
    private readonly CubeSettings saved = new();

    /// <summary>The window's open state before the check, restored by <see cref="Cleanup"/>.</summary>
    private bool wasOpen;

    /// <inheritdoc/>
    public override string Name => "collision-drawn";

    /// <inheritdoc/>
    /// <remarks>Room for a first gather spread over a few frames at background frame rates (about 10 fps).</remarks>
    public override TimeSpan Timeout => TimeSpan.FromSeconds(30);

    /// <inheritdoc/>
    public override IEnumerable<CheckStep> Run(BridgeContext context)
    {
        var window = context.Window;
        var cache = window.Collision.Cache;
        this.saved.CopyFrom(window.Settings);
        this.wasOpen = window.IsOpen;
        window.Settings.CopyFrom(new CubeSettings());
        window.IsOpen = true;

        while (!cache.Ready.HasData || cache.IsGathering)
        {
            yield return CheckStep.NextFrame;
        }

        var start = window.DrawCost.TotalRecorded;
        while (window.DrawCost.TotalRecorded - start < FramesAfterGather)
        {
            yield return CheckStep.NextFrame;
        }

        var clipped = window.ClippedCollision.Items;
        context.Metric("readyBoxes", cache.Ready.Count);
        context.Metric("clippedBoxes", clipped.Length);
        context.Metric("primitives", window.DrawList.Count);
        context.Metric("dropped", window.DrawList.Dropped);
        if (clipped.IsEmpty)
        {
            yield return CheckStep.Fail($"No collision boxes in the cube ({cache.Ready.Count} gathered around it). Stand on the ground and run again.");
            yield break;
        }

        var faces = 0;
        var boxesWithFaces = 0;
        var problem = CheckBoxesInCube(window.LastSpace, clipped);
        problem ??= CheckFaces(window, clipped.Length, out faces, out boxesWithFaces);
        if (problem is not null)
        {
            yield return CheckStep.Fail(problem);
            yield break;
        }

        context.Metric("collisionFaces", faces);
        context.Metric("boxesWithFaces", boxesWithFaces);

        if (!Game.PlayerReader.TryGetLocalPosition(out var feet))
        {
            yield return CheckStep.Fail("Not logged in: no local player. Log in and leave the game running.");
            yield break;
        }

        var floor = CollisionReaderCheck.FindFloor(feet, clipped, out var hit, out var floorBox);
        if (floor != CollisionReaderCheck.Floor.InABox)
        {
            yield return CheckStep.Fail(floor == CollisionReaderCheck.Floor.NoRayHit
                ? "The game's raycast found no floor under the player (flying or swimming?). Stand on the ground and run again."
                : string.Create(CultureInfo.InvariantCulture, $"The floor under the player ({hit}) is in none of the {clipped.Length} boxes drawn."));
            yield break;
        }

        problem = CheckFloorBeforeSelf(window, clipped, floorBox, out var floorLast, out var selfFirst);
        context.Metric("floorLastIndex", floorLast);
        context.Metric("selfFirstIndex", selfFirst);
        if (problem is not null)
        {
            yield return CheckStep.Fail(problem);
            yield break;
        }

        yield return CheckStep.Pass(string.Create(
            CultureInfo.InvariantCulture,
            $"{clipped.Length} boxes in the cube ({cache.Ready.Count} gathered), {faces} faces on {boxesWithFaces} boxes, all inside the cube; floor box {floorBox.Min}-{floorBox.Max} painted (last face {floorLast}) before the player (first face {selfFirst})."));
    }

    /// <inheritdoc/>
    public override void Cleanup(BridgeContext context)
    {
        context.Window.Settings.CopyFrom(this.saved);
        context.Window.IsOpen = this.wasOpen;
    }

    /// <summary>Checks every clipped box lies within the cube's height and the reach of its footprint at any yaw.</summary>
    /// <param name="space">The frame's cube.</param>
    /// <param name="boxes">The clipped boxes.</param>
    /// <returns>What was wrong, or null.</returns>
    private static string? CheckBoxesInCube(in CubeSpace space, ReadOnlySpan<CollisionBox> boxes)
    {
        var half = space.Half;
        var reach = new Vector2(half.X, half.Z).Length() + Tolerance;
        var centre = space.Centre;
        for (var i = 0; i < boxes.Length; i++)
        {
            var box = boxes[i];
            var outside =
                box.Min.Y < centre.Y - half.Y - Tolerance || box.Max.Y > centre.Y + half.Y + Tolerance ||
                box.Min.X < centre.X - reach || box.Max.X > centre.X + reach ||
                box.Min.Z < centre.Z - reach || box.Max.Z > centre.Z + reach;
            if (outside || !box.IsValid)
            {
                return string.Create(CultureInfo.InvariantCulture, $"Clipped box {i} ({box.Min}-{box.Max}) reaches outside the cube around {centre} (half size {half}).");
            }
        }

        return null;
    }

    /// <summary>Checks every collision face belongs to a clipped box, in the collision colour, at most three per box.</summary>
    /// <param name="window">The cube window.</param>
    /// <param name="boxCount">How many boxes were clipped.</param>
    /// <param name="faces">How many collision faces were drawn.</param>
    /// <param name="boxesWithFaces">How many boxes had at least one face.</param>
    /// <returns>What was wrong, or null.</returns>
    private static string? CheckFaces(CubeWindow window, int boxCount, out int faces, out int boxesWithFaces)
    {
        faces = 0;
        boxesWithFaces = 0;
        var perBox = new int[boxCount];
        var alpha = window.CollisionColour >> 24;
        foreach (var p in window.DrawList.Items)
        {
            if (!CollisionCubes.IsCollisionTag(p.Tag))
            {
                continue;
            }

            var index = CollisionCubes.IndexOf(p.Tag);
            if (p.Kind != PrimitiveKind.Quad || index >= boxCount)
            {
                return $"A {p.Kind} is tagged for collision box {index}, but {boxCount} boxes are in the cube.";
            }

            if (p.Colour >> 24 != alpha)
            {
                return $"Collision face of box {index} is {p.Colour:X8}; expected the collision colour's alpha {alpha}.";
            }

            if (++perBox[index] > 3)
            {
                return $"Collision box {index} has more than three faces drawn.";
            }

            faces++;
        }

        foreach (var count in perBox)
        {
            boxesWithFaces += count > 0 ? 1 : 0;
        }

        return boxesWithFaces == 0 ? $"None of the {boxCount} boxes in the cube has a face drawn." : null;
    }

    /// <summary>Checks the floor box's faces are all painted before the local player's cube.</summary>
    /// <param name="window">The cube window.</param>
    /// <param name="boxes">The clipped boxes.</param>
    /// <param name="floor">The box holding the floor under the player.</param>
    /// <param name="floorLast">Draw-list index of the floor box's last face, or -1.</param>
    /// <param name="selfFirst">Draw-list index of the player's first face, or -1.</param>
    /// <returns>What was wrong, or null.</returns>
    private static string? CheckFloorBeforeSelf(CubeWindow window, ReadOnlySpan<CollisionBox> boxes, CollisionBox floor, out int floorLast, out int selfFirst)
    {
        floorLast = -1;
        selfFirst = -1;
        var floorIndex = -1;
        for (var i = 0; i < boxes.Length && floorIndex < 0; i++)
        {
            if (boxes[i].Min == floor.Min && boxes[i].Max == floor.Max)
            {
                floorIndex = i;
            }
        }

        var selfTag = 0;
        var players = window.Players.Items;
        for (var i = 0; i < players.Length && selfTag == 0; i++)
        {
            if (players[i].Relation == PlayerRelation.Self)
            {
                selfTag = i + 1;
            }
        }

        var floorTag = CollisionCubes.TagFor(floorIndex);
        var items = window.DrawList.Items;
        for (var i = 0; i < items.Length; i++)
        {
            if (items[i].Kind != PrimitiveKind.Quad)
            {
                continue;
            }

            if (items[i].Tag == floorTag)
            {
                floorLast = i;
            }
            else if (items[i].Tag == selfTag && selfFirst < 0)
            {
                selfFirst = i;
            }
        }

        if (selfTag == 0 || selfFirst < 0)
        {
            return "The local player's cube wasn't drawn.";
        }

        if (floorLast < 0)
        {
            return $"The floor box ({floor.Min}-{floor.Max}) has no face drawn.";
        }

        return floorLast < selfFirst
            ? null
            : $"The floor box's face {floorLast} is painted after the player's first face {selfFirst}: the floor covers the player.";
    }
}
