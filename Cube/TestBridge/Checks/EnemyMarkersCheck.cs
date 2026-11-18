using System.Collections.Generic;

namespace Cube.TestBridge.Checks;

/// <summary>
/// Checks #111 end to end through the window: fake enemies arrive with the enemy relation, and the labels carry
/// <c>[T]</c> for the target and <c>[!]</c> for an attacking enemy, target first when both apply. The game's real target
/// and enemy-list reads are logged by the NPC probe and need a fight to exercise (docs/manual-tests.md).
/// </summary>
internal sealed class EnemyMarkersCheck : BridgeCheck
{
    /// <summary>Frames to wait for the window to draw.</summary>
    private const int FramesToWait = 30;

    /// <summary>The fakes added: name, then the label text expected.</summary>
    private static readonly (string Name, Core.PlayerRelation Relation, string Label)[] Cases =
    [
        ("Fake Enemy Plain", Core.PlayerRelation.Enemy, "Fake Enemy Plain"),
        ("Fake Enemy Attacker", Core.PlayerRelation.Enemy, "[!] Fake Enemy Attacker"),
        ("Fake Enemy Target", Core.PlayerRelation.Enemy, "[T] Fake Enemy Target"),
        ("Fake Enemy Target Attacker", Core.PlayerRelation.Enemy, "[T] [!] Fake Enemy Target Attacker"),
        ("Fake Friendly Target", Core.PlayerRelation.Npc, "[T] Fake Friendly Target"),
        ("Fake Friendly Attacker", Core.PlayerRelation.Npc, "Fake Friendly Attacker"),
    ];

    /// <summary>The window's open state before the check.</summary>
    private bool wasOpen;

    /// <summary>The NPC setting before the check.</summary>
    private bool hadNpcs;

    /// <inheritdoc/>
    public override string Name => "enemy-markers";

    /// <inheritdoc/>
    public override IEnumerable<CheckStep> Run(BridgeContext context)
    {
        var window = context.Window;
        this.wasOpen = window.IsOpen;
        this.hadNpcs = window.Settings.ShowNpcs;
        window.IsOpen = true;
        window.Settings.ShowNpcs = true;
        window.Settings.Changed();
        window.Fakes.Clear();
        for (var i = 0; i < Cases.Length; i++)
        {
            window.Fakes.Add(Cases[i].Name, new System.Numerics.Vector3(-0.8f + (i * 0.3f), 0, 0.2f), Cases[i].Relation);
        }

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

        var seen = 0;
        foreach (var player in window.Players.Items)
        {
            if (player.Slot < FakePlayers.FirstSlot)
            {
                continue;
            }

            var expected = Cases[player.Slot - FakePlayers.FirstSlot];
            if (player.Relation != expected.Relation || player.Label != expected.Label)
            {
                yield return CheckStep.Fail($"'{expected.Name}' arrived as {player.Relation} '{player.Label}', expected {expected.Relation} '{expected.Label}'.");
                yield break;
            }

            seen++;
        }

        yield return seen == Cases.Length
            ? CheckStep.Pass($"All {seen} fakes had the right colour relation and marker text.")
            : CheckStep.Fail($"{seen} of {Cases.Length} fakes arrived.");
    }

    /// <inheritdoc/>
    public override void Cleanup(BridgeContext context)
    {
        context.Window.Fakes.Clear();
        context.Window.Settings.ShowNpcs = this.hadNpcs;
        context.Window.Settings.Changed();
        context.Window.IsOpen = this.wasOpen;
    }
}
