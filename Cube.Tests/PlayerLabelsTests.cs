using System.Numerics;
using Cube.Core;

namespace Cube.Tests;

/// <summary>
/// Checks name labels (PROJECT.md, "Features": other players are labelled with their names, labels on/off;
/// "Labels": the name drawn next to the cube, overlaps are fine; "Draw order": near cubes cover far ones;
/// "Efficiency and memory": labels cached, no string building per frame).
/// </summary>
public class PlayerLabelsTests
{
    private static readonly float Elevation = MathF.PI / 6;

    private static readonly CubeSpace Space = new(Vector3.Zero, 40, 0.4f);

    private static readonly ViewProjection Projection = ViewProjection.Fit(Vector2.Zero, new Vector2(400, 400), Space.HalfEdge, Elevation);

    private static NearbyPlayer Player(Vector3 position, PlayerRelation relation, string name = "Someone") => new(0, 1, position, relation, name);

    private static DrawPrimitive[] Build(NearbyPlayer[] players, bool labels = true)
    {
        var list = new DrawList(256);
        PlayerCubes.Add(list, Space, Projection, players, PlayerPalette.Default, labels);
        list.SortBackToFront();
        return list.Items.ToArray();
    }

    [Fact]
    public void Each_other_player_gets_one_label_tagged_with_them()
    {
        var items = Build([Player(new Vector3(3, 0, 3), PlayerRelation.Party), Player(new Vector3(-5, 0, 2), PlayerRelation.Other)]);

        var labels = items.Where(p => p.Kind == PrimitiveKind.Label).Select(p => p.Tag).Order().ToArray();
        Assert.Equal([1, 2], labels);
    }

    [Fact]
    public void Local_player_is_not_labelled()
    {
        var items = Build([Player(Vector3.Zero, PlayerRelation.Self), Player(new Vector3(5, 0, 0), PlayerRelation.Other)]);

        Assert.DoesNotContain(items, p => p.Kind == PrimitiveKind.Label && p.Tag == 1);
    }

    [Fact]
    public void Labels_off_draws_no_labels_but_keeps_the_cubes()
    {
        var items = Build([Player(new Vector3(5, 0, 0), PlayerRelation.Other)], labels: false);

        Assert.DoesNotContain(items, p => p.Kind == PrimitiveKind.Label);
        Assert.Contains(items, p => p.Kind == PrimitiveKind.Quad);
    }

    [Theory]
    [InlineData(0f)]
    [InlineData(0.785f)]
    [InlineData(2f)]
    public void Label_sits_to_the_right_of_its_cube_at_any_yaw(float yaw)
    {
        var space = new CubeSpace(Vector3.Zero, 40, yaw);
        var projection = ViewProjection.Fit(Vector2.Zero, new Vector2(400, 400), space.HalfEdge, Elevation);
        var list = new DrawList(16);

        PlayerCubes.Add(list, space, projection, [Player(new Vector3(4, 0, -6), PlayerRelation.Other)], PlayerPalette.Default);

        var items = list.Items.ToArray();
        var label = items.Single(p => p.Kind == PrimitiveKind.Label);
        var rightEdge = items.Where(p => p.Kind == PrimitiveKind.Quad).Max(p => Math.Max(Math.Max(p.A.X, p.B.X), Math.Max(p.C.X, p.D.X)));
        var top = items.Where(p => p.Kind == PrimitiveKind.Quad).Min(p => Math.Min(Math.Min(p.A.Y, p.B.Y), Math.Min(p.C.Y, p.D.Y)));
        var bottom = items.Where(p => p.Kind == PrimitiveKind.Quad).Max(p => Math.Max(Math.Max(p.A.Y, p.B.Y), Math.Max(p.C.Y, p.D.Y)));
        Assert.InRange(label.A.X, rightEdge, rightEdge + 20);
        Assert.InRange(label.A.Y, top, bottom);
    }

    [Fact]
    public void Label_is_painted_after_its_own_cube()
    {
        var items = Build([Player(new Vector3(2, 0, 2), PlayerRelation.Other)]);

        var labelIndex = Array.FindIndex(items, p => p.Kind == PrimitiveKind.Label);
        var lastFace = Array.FindLastIndex(items, p => p.Kind == PrimitiveKind.Quad);
        Assert.True(labelIndex > lastFace);
    }

    [Fact]
    public void Nearer_cube_is_painted_over_a_farther_players_label()
    {
        // North (−Z) is away from the viewer at yaw 0, so the northern player is far and the southern one near.
        var space = new CubeSpace(Vector3.Zero, 40, 0f);
        var projection = ViewProjection.Fit(Vector2.Zero, new Vector2(400, 400), space.HalfEdge, Elevation);
        var list = new DrawList(32);
        PlayerCubes.Add(list, space, projection, [Player(new Vector3(0, 0, -10), PlayerRelation.Other), Player(new Vector3(0, 0, 10), PlayerRelation.Other)], PlayerPalette.Default);
        list.SortBackToFront();

        var items = list.Items.ToArray();
        var farLabel = Array.FindIndex(items, p => p.Kind == PrimitiveKind.Label && p.Tag == 1);
        var nearFirstFace = Array.FindIndex(items, p => p.Kind == PrimitiveKind.Quad && p.Tag == 2);
        Assert.True(farLabel < nearFirstFace, "The far player's label must be painted before the near player's cube.");
    }

    [Fact]
    public void Overlapping_players_each_keep_their_label()
    {
        // PROJECT.md: labels may overlap when players stand close together; neither is hidden.
        var items = Build([Player(new Vector3(1, 0, 1), PlayerRelation.Other), Player(new Vector3(1.05f, 0, 1), PlayerRelation.Party)]);

        Assert.Equal(2, items.Count(p => p.Kind == PrimitiveKind.Label));
    }

    [Fact]
    public void Building_labels_does_not_allocate()
    {
        var list = new DrawList(1024);
        var players = new NearbyPlayer[100];
        for (var i = 0; i < players.Length; i++)
        {
            players[i] = Player(new Vector3(i % 20 - 10, 0, i / 5 - 10), (PlayerRelation)(i % 3), "Name " + i);
        }

        Allocations.AssertNone(() =>
        {
            list.Clear();
            PlayerCubes.Add(list, Space, Projection, players, PlayerPalette.Default);
            list.SortBackToFront();
        });
    }
}
