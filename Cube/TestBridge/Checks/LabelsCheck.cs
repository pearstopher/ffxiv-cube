using System.Collections.Generic;
using Cube.Core;

namespace Cube.TestBridge.Checks;

/// <summary>
/// Checks every other player in the cube gets exactly one name label showing their name, next to their cube and on
/// top of it, and the local player gets none (PROJECT.md, "Features": other players are labelled with their names;
/// "Labels": the name drawn next to the cube). Uses the standard fake players (#16) so it works with nobody around.
/// </summary>
/// <remarks>
/// The text is checked against what the window actually passed to ImGui (<see cref="CubeWindow.LastLabelText"/>),
/// not just against the draw list, so a label showing the wrong player's name fails. Names stay in memory: the
/// check reports counts only (PROJECT.md, "Privacy").
/// </remarks>
internal sealed class LabelsCheck : BridgeCheck
{
    /// <summary>Frames to wait for the window to draw.</summary>
    private const int FramesToWait = 30;

    /// <summary>The window's open state before the check.</summary>
    private bool wasOpen;

    /// <inheritdoc/>
    public override string Name => "labels-name-each-other-player";

    /// <inheritdoc/>
    public override IEnumerable<CheckStep> Run(BridgeContext context)
    {
        var window = context.Window;
        this.wasOpen = window.IsOpen;
        window.IsOpen = true;
        window.Fakes.UseStandardSet();

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

        yield return Verify(context, window);
    }

    /// <inheritdoc/>
    public override void Cleanup(BridgeContext context)
    {
        context.Window.Fakes.Clear();
        context.Window.IsOpen = this.wasOpen;
    }

    /// <summary>Checks the last frame's labels.</summary>
    /// <param name="context">For metrics.</param>
    /// <param name="window">The cube window.</param>
    private static CheckStep Verify(BridgeContext context, CubeWindow window)
    {
        var players = window.Players.Items;
        var items = window.DrawList.Items;
        var labelsPerPlayer = new int[players.Length];
        var rightmostFace = new float[players.Length];
        var lastFaceIndex = new int[players.Length];
        var drawn = 0;

        for (var i = 0; i < items.Length; i++)
        {
            var p = items[i];
            if (p.Tag <= 0)
            {
                continue;
            }

            var index = p.Tag - 1;
            if (p.Kind == PrimitiveKind.Quad)
            {
                rightmostFace[index] = System.MathF.Max(rightmostFace[index], System.MathF.Max(System.MathF.Max(p.A.X, p.B.X), System.MathF.Max(p.C.X, p.D.X)));
                lastFaceIndex[index] = i;
                continue;
            }

            if (p.Kind != PrimitiveKind.Label)
            {
                continue;
            }

            labelsPerPlayer[index]++;
            if (drawn >= window.LastLabelCount || window.LastLabelText[drawn] != players[index].Label)
            {
                return CheckStep.Fail($"Label {drawn} (player {index}) wasn't drawn with that player's name.");
            }

            if (p.A.X <= rightmostFace[index] || i < lastFaceIndex[index])
            {
                return CheckStep.Fail($"Player {index}'s label isn't to the right of and on top of their cube.");
            }

            drawn++;
        }

        if (drawn != window.LastLabelCount)
        {
            return CheckStep.Fail($"The draw list has {drawn} labels but the window drew {window.LastLabelCount}.");
        }

        var others = 0;
        for (var i = 0; i < players.Length; i++)
        {
            var want = players[i].Relation == PlayerRelation.Self ? 0 : 1;
            others += want;
            if (labelsPerPlayer[i] != want)
            {
                return CheckStep.Fail($"Player {i} ({players[i].Relation}) has {labelsPerPlayer[i]} labels; expected {want}.");
            }
        }

        context.Metric("players", players.Length);
        context.Metric("labels", drawn);
        return others >= FakePlayers.StandardArriving(window.Settings.ShowNpcs)
            ? CheckStep.Pass($"{drawn} labels, one per other player, each with that player's name, beside and above their cube; none on self.")
            : CheckStep.Fail($"Only {others} other players in the cube; expected the standard fakes.");
    }
}
