namespace Cube.Core;

/// <summary>
/// Draws each player in the cube as a small solid cube in their relation's colour (PROJECT.md, "Players": player cube
/// size fixed relative to the big cube, about 1/40 of its edge, minimum a few pixels; "Features": the local player is a
/// cube at the centre in its own colour).
/// </summary>
/// <remarks>
/// Player cubes go into the same <see cref="DrawList"/> as the wireframe and are sorted with it, so near cubes cover far
/// ones (PROJECT.md, "Draw order"). Each player's faces are tagged with the player's index + 1.
/// </remarks>
public static class PlayerCubes
{
    /// <summary>Player cube edge as a fraction of the big cube's edge (PROJECT.md: about 1/40).</summary>
    public const float EdgeFraction = 1f / 40f;

    /// <summary>Smallest on-screen edge of a player cube, in pixels, so players stay visible in a small window.</summary>
    public const float MinEdgePixels = 6f;

    /// <summary>Half the edge of a player cube, in yalms, for this frame's cube and scale.</summary>
    /// <param name="space">The big cube.</param>
    /// <param name="projection">The projection, for the minimum on-screen size.</param>
    public static float HalfSize(in CubeSpace space, in ViewProjection projection)
    {
        var edge = space.Edge * EdgeFraction;
        if (projection.PixelsPerYalm > 0)
        {
            edge = MathF.Max(edge, MinEdgePixels / projection.PixelsPerYalm);
        }

        return edge / 2f;
    }

    /// <summary>Adds a cube for every player, in their colour.</summary>
    /// <param name="list">The frame's draw list.</param>
    /// <param name="space">The big cube.</param>
    /// <param name="projection">The projection.</param>
    /// <param name="players">This frame's players inside the cube.</param>
    /// <param name="palette">Colours per relation.</param>
    public static void Add(DrawList list, in CubeSpace space, in ViewProjection projection, ReadOnlySpan<NearbyPlayer> players, in PlayerPalette palette)
    {
        var half = HalfSize(space, projection);
        for (var i = 0; i < players.Length; i++)
        {
            ref readonly var player = ref players[i];
            CubeGeometry.AddSolidCube(list, space, projection, space.ToView(player.Position), half, palette.For(player.Relation), tag: i + 1);
        }
    }
}
