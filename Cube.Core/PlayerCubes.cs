namespace Cube.Core;

/// <summary>
/// Draws each player in the cube as a small solid cube in their relation's colour, with a name label for other
/// players (PROJECT.md, "Players": player cube size fixed relative to the big cube, about 1/40 of its edge, minimum a
/// few pixels; "Features": the local player is a cube at the centre in its own colour, other players are labelled
/// with their names; "Labels": the name drawn next to the cube, overlaps are fine).
/// </summary>
/// <remarks>
/// Player cubes and labels go into the same <see cref="DrawList"/> as the wireframe and are sorted with it, so near
/// cubes cover far ones (PROJECT.md, "Draw order"). A label shares its cube's depth and is added after the cube's
/// faces, so it is painted right on top of its own cube and under anything nearer. Each player's faces and label are
/// tagged with the player's index + 1.
/// </remarks>
public static class PlayerCubes
{
    /// <summary>Player cube edge as a fraction of the big cube's edge (PROJECT.md: about 1/40).</summary>
    public const float EdgeFraction = 1f / 40f;

    /// <summary>Smallest on-screen edge of a player cube, in pixels, so players stay visible in a small window.</summary>
    public const float MinEdgePixels = 6f;

    /// <summary>Space between a cube's widest possible outline and its label, in pixels.</summary>
    public const float LabelGap = 3f;

    /// <summary>
    /// Furthest a cube's outline reaches sideways from its centre, per unit of half-edge: half its horizontal
    /// diagonal (√2) once the cube's alignment has turned it 45°. Using the worst case keeps the label clear of the
    /// cube at every yaw without measuring the faces.
    /// </summary>
    private const float WidestReach = 1.4143f;

    /// <summary>Label text colour: near-white, readable over the dark window and every palette colour.</summary>
    public static uint LabelColour { get; } = Colour.Pack(0.95f, 0.95f, 0.95f, 1f);

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

    /// <summary>Adds a cube for every player, in their colour, and a name label for everyone but the local player.</summary>
    /// <param name="list">The frame's draw list.</param>
    /// <param name="space">The big cube.</param>
    /// <param name="projection">The projection.</param>
    /// <param name="players">This frame's players inside the cube.</param>
    /// <param name="palette">Colours per relation.</param>
    /// <param name="labels">Whether to add name labels (PROJECT.md, "Features": labels on/off setting).</param>
    public static void Add(DrawList list, in CubeSpace space, in ViewProjection projection, ReadOnlySpan<NearbyPlayer> players, in PlayerPalette palette, bool labels = true)
    {
        var half = HalfSize(space, projection);
        var labelOffset = (half * WidestReach * projection.PixelsPerYalm) + LabelGap;
        for (var i = 0; i < players.Length; i++)
        {
            ref readonly var player = ref players[i];
            var centre = space.ToView(player.Position);
            CubeGeometry.AddSolidCube(list, space, projection, centre, half, palette.For(player.Relation), tag: i + 1);

            // The local player isn't labelled: PROJECT.md labels the *other* players.
            if (labels && player.Relation != PlayerRelation.Self)
            {
                var anchor = projection.ToScreen(centre);
                anchor.X += labelOffset;
                list.AddLabel(anchor, projection.Depth(centre), LabelColour, i + 1);
            }
        }
    }
}
