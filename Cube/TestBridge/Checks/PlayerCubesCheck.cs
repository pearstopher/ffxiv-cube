using System.Collections.Generic;
using Cube.Core;

namespace Cube.TestBridge.Checks;

/// <summary>
/// Checks every player in the cube is drawn as a cube in their relation's colour (PROJECT.md, "Players" and "Colours";
/// "Draw order": far first), using the standard fake players (#16) so it works with nobody around.
/// </summary>
/// <remarks>
/// Each player's faces carry the player's index + 1 as their <see cref="DrawPrimitive.Tag"/>. For each player the
/// check expects one to three visible faces, every one a shade of the palette colour for the player's relation, and
/// the whole list in far-to-near order.
/// </remarks>
internal sealed class PlayerCubesCheck : BridgeCheck
{
    /// <summary>Frames to wait for the window to draw.</summary>
    private const int FramesToWait = 30;

    /// <summary>The window's open state before the check.</summary>
    private bool wasOpen;

    /// <inheritdoc/>
    public override string Name => "player-cubes-drawn-in-colour";

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

    /// <summary>Whether a face colour is one of the cube shades of a base colour.</summary>
    /// <param name="face">The face's colour.</param>
    /// <param name="baseColour">The player's palette colour.</param>
    private static bool IsShadeOf(uint face, uint baseColour) =>
        face == Colour.Shade(baseColour, CubeGeometry.TopShade)
        || face == Colour.Shade(baseColour, CubeGeometry.NorthSouthShade)
        || face == Colour.Shade(baseColour, CubeGeometry.EastWestShade)
        || face == Colour.Shade(baseColour, CubeGeometry.BottomShade);

    /// <summary>Checks the last frame's player cubes.</summary>
    /// <param name="context">For metrics.</param>
    /// <param name="window">The cube window.</param>
    private static CheckStep Verify(BridgeContext context, CubeWindow window)
    {
        var players = window.Players.Items;
        var faces = new int[players.Length];
        var items = window.DrawList.Items;
        for (var i = 0; i < items.Length; i++)
        {
            var p = items[i];
            if (i > 0 && items[i - 1].Depth < p.Depth)
            {
                return CheckStep.Fail($"Draw list isn't far-to-near at primitive {i}.");
            }

            // Labels (#18) carry their player's tag too; LabelsCheck covers them.
            if (p.Tag == 0 || p.Kind == PrimitiveKind.Label)
            {
                continue;
            }

            var index = p.Tag - 1;
            if (index >= players.Length)
            {
                return CheckStep.Fail($"A face is tagged for player {index}, but only {players.Length} are in the cube.");
            }

            var want = window.Palette.For(players[index].Relation);
            if (p.Kind != PrimitiveKind.Quad || !IsShadeOf(p.Colour, want))
            {
                return CheckStep.Fail($"Player {index} ({players[index].Relation}) has a face coloured {p.Colour:X8}; expected a shade of {want:X8}.");
            }

            faces[index]++;
        }

        for (var i = 0; i < faces.Length; i++)
        {
            if (faces[i] is < 1 or > 3)
            {
                return CheckStep.Fail($"Player {i} has {faces[i]} visible faces; a cube shows 1 to 3.");
            }
        }

        context.Metric("players", players.Length);
        context.Metric("primitives", items.Length);
        context.Metric("halfSizeYalms", PlayerCubes.HalfSize(window.LastSpace, window.LastProjection));
        return players.Length >= FakePlayers.StandardInside
            ? CheckStep.Pass($"{players.Length} player cubes drawn, each 1-3 faces in its relation's colour, far to near.")
            : CheckStep.Fail($"Only {players.Length} players in the cube; expected at least the {FakePlayers.StandardInside} fakes.");
    }
}
