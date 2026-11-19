using System.Numerics;

namespace Cube.Core;

/// <summary>
/// Draws each player in the cube as a small solid cube in their relation's colour, with a name label for other
/// players (PROJECT.md, "Players": player cube size fixed relative to the big cube, about 1/40 of its largest extent,
/// minimum a few pixels; "Features": the local player is a cube at the centre in its own colour, other players are labelled
/// with their names; "Labels": the name drawn next to the cube, overlaps are fine; "NPCs": named NPCs are shown the
/// same way, in the NPC colour).
/// </summary>
/// <remarks>
/// Player cubes and labels go into the same <see cref="DrawList"/> as the wireframe and are sorted with it, so near
/// cubes cover far ones (PROJECT.md, "Draw order"). A label shares its cube's depth and is added after the cube's
/// faces, so it is painted right on top of its own cube and under anything nearer. Each player's faces and label are
/// tagged with the player's index + 1.
/// </remarks>
public static class PlayerCubes
{
    /// <summary>Player cube edge as a fraction of the big cube's largest extent (PROJECT.md: about 1/40).</summary>
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
    /// <remarks>
    /// Follows the <see cref="CubeSpace.Largest">largest extent</see> (PROJECT.md, changed in #90), so a long thin box
    /// doesn't shrink its markers to dots along the short axes.
    /// </remarks>
    public static float HalfSize(in CubeSpace space, in ViewProjection projection)
    {
        var edge = space.Largest * EdgeFraction;
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
    /// <param name="labels">Whether to add name labels for everyone but the local player (PROJECT.md, "Features": labels on/off setting).</param>
    /// <param name="selfLabel">Whether the local player's own name is labelled too (#112).</param>
    /// <param name="position">Whether labels sit beside or centred above their cubes (#112).</param>
    /// <remarks>
    /// One <see cref="BoxProjector"/> and one set of face shades per relation are worked out per call, so each player
    /// costs one projection of their position (PROJECT.md, "Efficiency and memory").
    /// </remarks>
    public static void Add(DrawList list, in CubeSpace space, in ViewProjection projection, ReadOnlySpan<NearbyPlayer> players, in PlayerPalette palette, bool labels = true, bool selfLabel = false, NamePosition position = NamePosition.Side)
    {
        if (players.IsEmpty)
        {
            return;
        }

        var projector = new BoxProjector(space, projection);
        var self = projector.Shades(palette.Self);
        var party = projector.Shades(palette.Party);
        var other = projector.Shades(palette.Other);
        var npc = projector.Shades(palette.Npc);
        var enemy = projector.Shades(palette.Enemy);
        var interactable = projector.Shades(palette.Interactable);
        var half = HalfSize(space, projection);
        var size = new Vector3(half);
        var labelOffset = (half * WidestReach * projection.PixelsPerYalm) + LabelGap;
        var above = position == NamePosition.Above;
        for (var i = 0; i < players.Length; i++)
        {
            ref readonly var player = ref players[i];
            var screen = projector.Screen(player.Position);
            var depth = projector.Depth(player.Position);
            var shades = player.Relation switch
            {
                PlayerRelation.Self => self,
                PlayerRelation.Party => party,
                PlayerRelation.Npc => npc,
                PlayerRelation.Enemy => enemy,
                PlayerRelation.Interactable => interactable,
                _ => other,
            };
            projector.AddFaces(list, screen, size, depth, shades, tag: i + 1);

            // PROJECT.md labels the *other* players (and NPCs); the local player's own name is a separate setting (#112).
            if (player.Relation == PlayerRelation.Self ? selfLabel : labels)
            {
                // Above: the anchor is the middle of the text's bottom edge, a gap over the cube's top corner; the
                // window centres the text on it, since only it knows the text's width.
                list.AddLabel(
                    above ? new Vector2(screen.X, screen.Y - labelOffset) : new Vector2(screen.X + labelOffset, screen.Y),
                    depth,
                    LabelColour,
                    i + 1,
                    above);
            }
        }
    }
}
