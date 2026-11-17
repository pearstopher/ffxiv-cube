using System.Collections.Generic;
using Cube.Core;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin.Services;

namespace Cube.TestBridge.Checks;

/// <summary>
/// Checks the players the window found match an independent read of Dalamud's object table (PROJECT.md,
/// "Automated testing": the players drawn match an independent read of the object table; "Players": every player
/// character inside the cube, nobody outside; "Colours": self, party/alliance, other).
/// </summary>
/// <remarks>
/// The window reads FFXIVClientStructs directly (allocation-free); this check reads Dalamud's
/// <see cref="IObjectTable"/> and its <see cref="StatusFlags"/> instead, so a mistake in either shows up as a
/// difference. Both are taken within one frame of each other, so players within half a yalm of a face may cross it
/// in between; they are ignored. Fake players (Debug, #16) are left out of the comparison.
/// </remarks>
/// <param name="objects">Dalamud's object table.</param>
internal sealed class PlayersMatchObjectTableCheck(IObjectTable objects) : BridgeCheck
{
    /// <summary>Players this close to a face may legitimately differ between the two reads.</summary>
    private const float BoundaryTolerance = 0.5f;

    /// <summary>Frames to wait for the window to draw after opening it.</summary>
    private const int FramesToWait = 30;

    /// <summary>The window's open state before the check, restored by <see cref="Cleanup"/>.</summary>
    private bool wasOpen;

    /// <inheritdoc/>
    public override string Name => "players-match-object-table";

    /// <inheritdoc/>
    public override IEnumerable<CheckStep> Run(BridgeContext context)
    {
        var window = context.Window;
        this.wasOpen = window.IsOpen;
        window.IsOpen = true;

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

        if (!window.LastHadLocalPlayer || objects.LocalPlayer is null)
        {
            yield return CheckStep.Fail("Not logged in: no local player, so no cube to read players into.");
            yield break;
        }

        yield return this.Compare(context, window);
    }

    /// <inheritdoc/>
    public override void Cleanup(BridgeContext context) => context.Window.IsOpen = this.wasOpen;

    /// <summary>Whether a point is within a given half-size of the cube's centre on every axis (unclamped, unlike <see cref="CubeSpace"/>).</summary>
    /// <param name="space">The cube.</param>
    /// <param name="point">A world position.</param>
    /// <param name="half">Half-size to test against.</param>
    private static bool Within(in CubeSpace space, System.Numerics.Vector3 point, float half)
    {
        var d = point - space.Centre;
        return System.MathF.Abs(d.X) <= half && System.MathF.Abs(d.Y) <= half && System.MathF.Abs(d.Z) <= half;
    }

    /// <summary>Compares the window's players with the object table, one frame's worth.</summary>
    /// <param name="context">For metrics.</param>
    /// <param name="window">The cube window.</param>
    private CheckStep Compare(BridgeContext context, CubeWindow window)
    {
        var space = window.LastSpace;
        var localId = objects.LocalPlayer!.EntityId;

        var drawn = new Dictionary<uint, PlayerRelation>();
        foreach (var player in window.Players.Items)
        {
            if (player.Slot < Game.PlayerReader.CharacterSlots)
            {
                drawn[player.EntityId] = player.Relation;
            }
        }

        var expected = 0;
        foreach (var obj in objects)
        {
            if (obj is not IPlayerCharacter pc)
            {
                continue;
            }

            if (Within(space, pc.Position, space.HalfEdge - BoundaryTolerance))
            {
                expected++;
                if (!drawn.TryGetValue(pc.EntityId, out var relation))
                {
                    return CheckStep.Fail($"A player inside the cube (entity {pc.EntityId:X8}) is missing from the window's players.");
                }

                var flags = pc.StatusFlags;
                var want = PlayerSet.Classify(
                    pc.EntityId == localId,
                    (flags & StatusFlags.PartyMember) != 0,
                    (flags & StatusFlags.AllianceMember) != 0);
                if (relation != want)
                {
                    return CheckStep.Fail($"Player {pc.EntityId:X8} is classed {relation}; the object table says {want}.");
                }
            }
            else if (!Within(space, pc.Position, space.HalfEdge + BoundaryTolerance) && drawn.ContainsKey(pc.EntityId))
            {
                return CheckStep.Fail($"A player outside the cube (entity {pc.EntityId:X8}) is in the window's players.");
            }
        }

        var self = 0;
        var party = 0;
        foreach (var relation in drawn.Values)
        {
            self += relation == PlayerRelation.Self ? 1 : 0;
            party += relation == PlayerRelation.Party ? 1 : 0;
        }

        context.Metric("playersInCube", drawn.Count);
        context.Metric("expectedInCube", expected);
        context.Metric("party", party);
        context.Metric("offered", window.Players.Offered);
        context.Metric("dropped", window.Players.Dropped);
        context.Metric("nameBuilds", window.Players.Names.Builds);

        if (self != 1)
        {
            return CheckStep.Fail($"Expected exactly one Self player, found {self}.");
        }

        return CheckStep.Pass($"{drawn.Count} player(s) in the cube match the object table ({party} party/alliance, 1 self).");
    }
}
