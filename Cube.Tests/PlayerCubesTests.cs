using System.Numerics;
using Cube.Core;

namespace Cube.Tests;

/// <summary>
/// Checks players are drawn as small coloured cubes (PROJECT.md, "Players": about 1/40 of the big cube's edge, minimum
/// a few pixels; "Colours": one colour each for self, party/alliance and others; "Draw order": near cubes cover far
/// ones; "Efficiency and memory": no per-frame allocations).
/// </summary>
public class PlayerCubesTests
{
    private static readonly float Elevation = MathF.PI / 6;

    private static CubeSpace Space(float edge = 40) => new(Vector3.Zero, edge, 0.4f);

    private static NearbyPlayer Player(Vector3 position, PlayerRelation relation) => new(0, 1, position, relation, "Someone");

    [Fact]
    public void Player_cube_is_one_fortieth_of_the_big_cube()
    {
        var space = Space(40);
        var projection = new ViewProjection(Elevation, Vector2.Zero, pixelsPerYalm: 20);

        Assert.Equal(0.5f, PlayerCubes.HalfSize(space, projection), 5);
    }

    [Fact]
    public void Player_cube_scales_with_the_big_cube()
    {
        var projection = new ViewProjection(Elevation, Vector2.Zero, pixelsPerYalm: 20);

        Assert.Equal(1.25f, PlayerCubes.HalfSize(Space(100), projection), 5);
    }

    [Fact]
    public void Player_cube_never_gets_smaller_than_the_minimum_on_screen()
    {
        // At 1 pixel per yalm a 1-yalm cube would be 1 pixel; the minimum keeps it 6 pixels wide.
        var projection = new ViewProjection(Elevation, Vector2.Zero, pixelsPerYalm: 1);

        Assert.Equal(PlayerCubes.MinEdgePixels / 2f, PlayerCubes.HalfSize(Space(40), projection), 5);
    }

    [Theory]
    [InlineData(PlayerRelation.Self)]
    [InlineData(PlayerRelation.Party)]
    [InlineData(PlayerRelation.Other)]
    public void Each_relation_has_its_own_colour(PlayerRelation relation)
    {
        var palette = PlayerPalette.Default;
        var colours = new[] { palette.Self, palette.Party, palette.Other };

        Assert.Single(colours, c => c == palette.For(relation));
    }

    [Fact]
    public void Each_player_gets_faces_in_their_colour_tagged_with_their_index()
    {
        var space = Space();
        var projection = ViewProjection.Fit(Vector2.Zero, new Vector2(400, 400), space.HalfEdge, Elevation);
        var list = new DrawList(64);
        NearbyPlayer[] players = [Player(Vector3.Zero, PlayerRelation.Self), Player(new Vector3(5, 0, 5), PlayerRelation.Party)];

        PlayerCubes.Add(list, space, projection, players, PlayerPalette.Default);

        var items = list.Items.ToArray().Where(p => p.Kind == PrimitiveKind.Quad).ToArray();
        Assert.InRange(items.Count(p => p.Tag == 1), 1, 3);
        Assert.InRange(items.Count(p => p.Tag == 2), 1, 3);
        Assert.Contains(items, p => p.Tag == 1 && p.Colour == PlayerPalette.Default.Self);
        Assert.Contains(items, p => p.Tag == 2 && p.Colour == PlayerPalette.Default.Party);
    }

    [Fact]
    public void Local_player_cube_is_at_the_centre_of_the_window()
    {
        var space = new CubeSpace(new Vector3(100, 5, -30), 40, 0f);
        var projection = ViewProjection.Fit(Vector2.Zero, new Vector2(400, 400), space.HalfEdge, Elevation);
        var list = new DrawList(16);

        PlayerCubes.Add(list, space, projection, [Player(space.Centre, PlayerRelation.Self)], PlayerPalette.Default);

        var top = list.Items.ToArray().First(p => p.Colour == PlayerPalette.Default.Self);
        var centre = (top.A + top.B + top.C + top.D) / 4;
        Assert.True(Vector2.Distance(projection.Origin, centre) < 10, $"Self cube top at {centre}, window centre {projection.Origin}");
    }

    [Fact]
    public void Nearer_player_is_drawn_after_farther_player()
    {
        var space = new CubeSpace(Vector3.Zero, 40, 0f);
        var projection = ViewProjection.Fit(Vector2.Zero, new Vector2(400, 400), space.HalfEdge, Elevation);
        var list = new DrawList(32);

        // North is forward (away from the viewer) at yaw 0.
        PlayerCubes.Add(list, space, projection, [Player(new Vector3(0, 0, 10), PlayerRelation.Party), Player(new Vector3(0, 0, -10), PlayerRelation.Other)], PlayerPalette.Default);
        list.SortBackToFront();

        var items = list.Items.ToArray();
        var lastFar = Array.FindLastIndex(items, p => p.Tag == 2);
        var firstNear = Array.FindIndex(items, p => p.Tag == 1);
        Assert.True(lastFar < firstNear, "The northern (far) player must be painted before the southern (near) one.");
    }

    [Fact]
    public void Drawing_players_does_not_allocate()
    {
        var space = Space();
        var projection = ViewProjection.Fit(Vector2.Zero, new Vector2(400, 400), space.HalfEdge, Elevation);
        var list = new DrawList(512);
        var players = new NearbyPlayer[60];
        for (var i = 0; i < players.Length; i++)
        {
            players[i] = Player(new Vector3(i % 20 - 10, 0, i / 3 - 10), (PlayerRelation)(i % 3));
        }

        Allocations.AssertNone(() =>
        {
            list.Clear();
            CubeGeometry.AddWireCube(list, space, projection, 0xFFFFFFFF);
            PlayerCubes.Add(list, space, projection, players, PlayerPalette.Default);
            list.SortBackToFront();
        });
    }
}
