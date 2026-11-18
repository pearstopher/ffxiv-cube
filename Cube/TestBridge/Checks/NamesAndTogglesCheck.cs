using System.Collections.Generic;
using System.Numerics;
using Cube.Core;

namespace Cube.TestBridge.Checks;

/// <summary>
/// Checks #112 end to end through the window: each per-kind toggle removes exactly its kind of cube from the next frame
/// (and the local player is never removed), the own-name setting adds or removes the local player's label, and the
/// name position setting makes every label centred above its cube. Uses fakes of every kind so it works with nobody
/// around; the game's real reads of event objects are covered by the research probe and
/// <c>docs/manual-tests.md</c>.
/// </summary>
internal sealed class NamesAndTogglesCheck : BridgeCheck
{
    /// <summary>Frames to wait for the window to draw.</summary>
    private const int FramesToWait = 30;

    /// <summary>The fakes: one of every kind, all inside the cube.</summary>
    private static readonly (string Name, PlayerRelation Relation)[] Kinds =
    [
        ("Fake Toggle Other", PlayerRelation.Other),
        ("Fake Toggle Party", PlayerRelation.Party),
        ("Fake Toggle Npc", PlayerRelation.Npc),
        ("Fake Toggle Enemy", PlayerRelation.Enemy),
        ("Fake Toggle Interactable", PlayerRelation.Interactable),
    ];

    /// <summary>The window's open state before the check.</summary>
    private bool wasOpen;

    /// <summary>The settings before the check, restored afterwards.</summary>
    private CubeSettings? saved;

    /// <summary>Set when the window didn't draw in time.</summary>
    private bool timedOut;

    /// <inheritdoc/>
    public override string Name => "names-and-toggles";

    /// <inheritdoc/>
    public override IEnumerable<CheckStep> Run(BridgeContext context)
    {
        var window = context.Window;
        var settings = window.Settings;
        this.wasOpen = window.IsOpen;
        this.saved = new CubeSettings();
        this.saved.CopyFrom(settings);
        window.IsOpen = true;
        settings.ShowLabels = true;
        settings.ShowOwnName = true;
        settings.NamePosition = NamePosition.Side;
        settings.ShowPlayers = settings.ShowNpcs = settings.ShowEnemies = settings.ShowInteractables = true;
        settings.Changed();
        window.Fakes.Clear();
        for (var i = 0; i < Kinds.Length; i++)
        {
            window.Fakes.Add(Kinds[i].Name, new Vector3(-0.8f + (i * 0.3f), 0, 0.2f), Kinds[i].Relation);
        }

        foreach (var step in this.Draw(window))
        {
            yield return step;
        }

        if (this.timedOut)
        {
            yield return CheckStep.Fail($"Window didn't draw within {FramesToWait} frames (game UI hidden?).");
            yield break;
        }

        // Everything on: every fake arrived, and each is labelled (plus the local player's own name).
        if (CountFakes(window, null) != Kinds.Length)
        {
            yield return CheckStep.Fail($"With every toggle on, {CountFakes(window, null)} of {Kinds.Length} fakes arrived.");
            yield break;
        }

        var baseline = window.LastLabelCount;
        var self = HasSelf(window) ? 1 : 0;
        if (baseline != window.Players.Count)
        {
            yield return CheckStep.Fail($"Every cube should be labelled with own name on: {baseline} labels for {window.Players.Count} cubes.");
            yield break;
        }

        // Each toggle off removes only its kind.
        var toggles = new (string Label, System.Action<CubeSettings> Off, PlayerRelation[] Gone)[]
        {
            ("players", s => s.ShowPlayers = false, [PlayerRelation.Other, PlayerRelation.Party]),
            ("NPCs", s => s.ShowNpcs = false, [PlayerRelation.Npc]),
            ("enemies", s => s.ShowEnemies = false, [PlayerRelation.Enemy]),
            ("interactables", s => s.ShowInteractables = false, [PlayerRelation.Interactable]),
        };
        foreach (var (label, off, gone) in toggles)
        {
            off(settings);
            settings.Changed();
            foreach (var step in this.Draw(window))
            {
                yield return step;
            }

            var remaining = 0;
            foreach (var relation in new[] { PlayerRelation.Other, PlayerRelation.Party, PlayerRelation.Npc, PlayerRelation.Enemy, PlayerRelation.Interactable })
            {
                remaining += CountFakes(window, relation);
            }

            if (this.timedOut || remaining != Kinds.Length - gone.Length || HasSelf(window) != (self == 1))
            {
                yield return CheckStep.Fail($"With {label} off, {remaining} fakes remained (expected {Kinds.Length - gone.Length}) or the local player's cube changed.");
                yield break;
            }

            foreach (var relation in gone)
            {
                if (CountFakes(window, relation) != 0)
                {
                    yield return CheckStep.Fail($"With {label} off, a {relation} cube was still drawn.");
                    yield break;
                }
            }

            settings.CopyFrom(this.saved);
            settings.ShowLabels = true;
            settings.ShowOwnName = true;
            settings.NamePosition = NamePosition.Side;
            settings.ShowPlayers = settings.ShowNpcs = settings.ShowEnemies = settings.ShowInteractables = true;
            settings.Changed();
        }

        // Own name off: one label fewer, when the local player is in the cube.
        settings.ShowOwnName = false;
        settings.Changed();
        foreach (var step in this.Draw(window))
        {
            yield return step;
        }

        if (window.LastLabelCount != window.Players.Count - self)
        {
            yield return CheckStep.Fail($"With own name off, {window.LastLabelCount} labels for {window.Players.Count} cubes ({self} of them yours).");
            yield break;
        }

        // Centred above: every label is flagged for centring and sits above the cube it belongs to.
        settings.ShowOwnName = true;
        settings.NamePosition = NamePosition.Above;
        settings.Changed();
        foreach (var step in this.Draw(window))
        {
            yield return step;
        }

        var items = window.DrawList.Items;
        var labels = 0;
        for (var i = 0; i < items.Length; i++)
        {
            if (items[i].Kind != PrimitiveKind.Label)
            {
                continue;
            }

            labels++;
            if (items[i].B.X != 1f)
            {
                yield return CheckStep.Fail("A label wasn't flagged for centring with the name position set to above.");
                yield break;
            }
        }

        if (labels != window.Players.Count || window.LastLabelCount != labels)
        {
            yield return CheckStep.Fail($"Above: {labels} labels in the draw list, {window.LastLabelCount} drawn, {window.Players.Count} cubes.");
            yield break;
        }

        yield return CheckStep.Pass($"Each toggle removed only its kind of cube (players, NPCs, enemies, interactables); own name off dropped one label (own cube {(self == 1 ? "present" : "absent")}); above-position flagged all {labels} labels for centring.");
    }

    /// <inheritdoc/>
    public override void Cleanup(BridgeContext context)
    {
        context.Window.Fakes.Clear();
        if (this.saved is not null)
        {
            context.Window.Settings.CopyFrom(this.saved);
        }

        context.Window.IsOpen = this.wasOpen;
    }

    /// <summary>How many of this check's fakes are in the frame, of one relation or (null) of any.</summary>
    /// <param name="window">The cube window.</param>
    /// <param name="relation">The relation to count, or null for all.</param>
    private static int CountFakes(CubeWindow window, PlayerRelation? relation)
    {
        var count = 0;
        foreach (var player in window.Players.Items)
        {
            if (player.Slot >= FakePlayers.FirstSlot && (relation is null || player.Relation == relation))
            {
                count++;
            }
        }

        return count;
    }

    /// <summary>Whether the local player is in the frame.</summary>
    /// <param name="window">The cube window.</param>
    private static bool HasSelf(CubeWindow window)
    {
        foreach (var player in window.Players.Items)
        {
            if (player.Relation == PlayerRelation.Self)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Waits for the window to draw two more frames, setting <see cref="timedOut"/> if it doesn't.</summary>
    /// <param name="window">The cube window.</param>
    private IEnumerable<CheckStep> Draw(CubeWindow window)
    {
        var start = window.DrawCost.TotalRecorded;
        var waited = 0;
        this.timedOut = false;
        while (window.DrawCost.TotalRecorded - start < 2)
        {
            if (++waited > FramesToWait)
            {
                this.timedOut = true;
                yield break;
            }

            yield return CheckStep.NextFrame;
        }
    }
}
