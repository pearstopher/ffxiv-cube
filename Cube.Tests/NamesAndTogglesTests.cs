using System.Numerics;
using System.Text;
using System.Text.Json;
using Cube.Core;

namespace Cube.Tests;

/// <summary>
/// Checks #112 (GitHub issue text): the own-name setting (on by default), name position (side by default, or centred
/// above), a toggle per kind of cube (all on by default), and the text-labelled interactable kind (own colour, own cap,
/// nameless objects left out).
/// </summary>
public class NamesAndTogglesTests
{
    private static readonly Vector3 Centre = new(10, 0, 10);
    private static readonly byte[] Name = Encoding.UTF8.GetBytes("Entrance");
    private static readonly CubeSpace Space = new(Vector3.Zero, 40, 0.4f);
    private static readonly ViewProjection Projection = ViewProjection.Fit(Vector2.Zero, new Vector2(400, 400), Space.Half, ViewOrientation.Default);

    private static PlayerSet Begin(bool npcs = true, bool players = true, bool enemies = true, bool interactables = true)
    {
        var set = new PlayerSet(8, new NameCache(64), 4, 3);
        set.Begin(new CubeSpace(Centre, new Vector3(40), 0), npcs, players, enemies, interactables);
        return set;
    }

    private static DrawPrimitive[] Labels(NearbyPlayer[] players, bool labels = true, bool selfLabel = false, NamePosition position = NamePosition.Side)
    {
        var list = new DrawList(256);
        PlayerCubes.Add(list, Space, Projection, players, PlayerPalette.Default, labels, selfLabel, position);
        return list.Items.ToArray().Where(p => p.Kind == PrimitiveKind.Label).ToArray();
    }

    [Fact]
    public void Defaults_show_everything_with_names_at_the_side()
    {
        var settings = new CubeSettings();

        Assert.True(settings.ShowOwnName);
        Assert.Equal(NamePosition.Side, settings.NamePosition);
        Assert.True(settings.ShowPlayers && settings.ShowNpcs && settings.ShowEnemies && settings.ShowInteractables);
    }

    [Fact]
    public void Old_files_load_the_new_settings_at_their_defaults()
    {
        var settings = JsonSerializer.Deserialize<CubeSettings>("""{ "ShowNpcs": false }""", new JsonSerializerOptions { IncludeFields = true })!;
        settings.Upgrade(CubeSettings.PlayerHeightVersion);

        Assert.True(settings.ShowOwnName && settings.ShowPlayers && settings.ShowEnemies && settings.ShowInteractables);
        Assert.Equal(CubeSettings.DefaultInteractableColour, settings.InteractableColour);
        Assert.False(settings.ShowNpcs);
    }

    [Fact]
    public void A_bad_name_position_is_repaired_to_side()
    {
        var settings = new CubeSettings { NamePosition = (NamePosition)99 };

        settings.Changed();

        Assert.Equal(NamePosition.Side, settings.NamePosition);
    }

    [Fact]
    public void CopyFrom_copies_the_new_settings()
    {
        var source = new CubeSettings { ShowOwnName = false, NamePosition = NamePosition.Above, ShowPlayers = false, ShowEnemies = false, ShowInteractables = false, InteractableColour = new Vector3(0.1f) };
        var target = new CubeSettings();

        target.CopyFrom(source);

        Assert.False(target.ShowOwnName || target.ShowPlayers || target.ShowEnemies || target.ShowInteractables);
        Assert.Equal(NamePosition.Above, target.NamePosition);
        Assert.Equal(source.InteractableColour, target.InteractableColour);
    }

    [Fact]
    public void Interactable_colour_is_in_the_palette_and_differs_from_the_others()
    {
        var palette = new CubeSettings().Palette();

        Assert.Equal(palette.Interactable, palette.For(PlayerRelation.Interactable));
        Assert.Equal(6, new[] { palette.Self, palette.Party, palette.Other, palette.Npc, palette.Enemy, palette.Interactable }.Distinct().Count());
    }

    [Theory]
    [InlineData("Entrance", true)]
    [InlineData("", false)]
    public void OnlyNamedObjectsAreTextInteractables(string name, bool expected)
    {
        var bytes = Encoding.UTF8.GetBytes(name + "\0");

        Assert.Equal(expected, PlayerSet.IsTextInteractable(bytes));
    }

    [Fact]
    public void Interactable_inside_the_cube_is_kept_as_an_interactable()
    {
        var set = Begin();

        Assert.True(set.OfferInteractable(30, 5, Centre, Name));

        Assert.Equal(PlayerRelation.Interactable, Assert.Single(set.Items.ToArray()).Relation);
        Assert.Equal(1, set.InteractableCount);
    }

    [Fact]
    public void Interactable_outside_the_cube_is_not_kept()
    {
        var set = Begin();

        Assert.False(set.OfferInteractable(30, 5, Centre + new Vector3(100, 0, 0), Name));
    }

    [Fact]
    public void Interactables_off_keeps_none_and_asks_the_reader_to_skip_them()
    {
        var set = Begin(interactables: false);

        Assert.False(set.ShowsInteractables);
        Assert.False(set.OfferInteractable(30, 5, Centre, Name));
    }

    [Fact]
    public void Interactable_cap_drops_extras_without_pushing_out_players_or_npcs()
    {
        var set = Begin();

        for (var i = 0; i < 6; i++)
        {
            set.OfferInteractable(30 + i, (uint)i, Centre, Name);
        }

        Assert.Equal(3, set.InteractableCount);
        Assert.Equal(3, set.Dropped);
        Assert.True(set.Offer(0, 1, Centre, true, false, false, Name));
        Assert.True(set.OfferNpc(40, 9, Centre, Name));
    }

    [Fact]
    public void Players_off_drops_other_players_but_never_the_local_player()
    {
        var set = Begin(players: false);

        Assert.False(set.Offer(0, 1, Centre, false, false, false, Name));
        Assert.False(set.Offer(2, 2, Centre, false, true, false, Name));
        Assert.True(set.Offer(4, 3, Centre, true, false, false, Name));
        Assert.Equal(PlayerRelation.Self, Assert.Single(set.Items.ToArray()).Relation);
    }

    [Fact]
    public void Enemies_off_drops_enemies_but_keeps_friendly_npcs()
    {
        var set = Begin(enemies: false);

        Assert.False(set.OfferNpc(30, 1, Centre, Name, isEnemy: true));
        Assert.True(set.OfferNpc(31, 2, Centre, Name));
        Assert.Equal(PlayerRelation.Npc, Assert.Single(set.Items.ToArray()).Relation);
    }

    [Fact]
    public void Npcs_off_drops_friendly_npcs_but_keeps_enemies()
    {
        var set = Begin(npcs: false);

        Assert.True(set.ShowsNpcs);
        Assert.False(set.OfferNpc(30, 1, Centre, Name));
        Assert.True(set.OfferNpc(31, 2, Centre, Name, isEnemy: true));
        Assert.Equal(PlayerRelation.Enemy, Assert.Single(set.Items.ToArray()).Relation);
    }

    [Fact]
    public void Npcs_and_enemies_both_off_makes_the_reader_skip_npcs()
    {
        var set = Begin(npcs: false, enemies: false);

        Assert.False(set.ShowsNpcs);
    }

    [Fact]
    public void Own_name_is_labelled_only_when_asked()
    {
        var self = new NearbyPlayer(0, 1, Vector3.Zero, PlayerRelation.Self, "Me", "Me");

        Assert.Empty(Labels([self]));
        Assert.Single(Labels([self], selfLabel: true));
    }

    [Fact]
    public void Own_name_setting_is_independent_of_the_names_setting()
    {
        var self = new NearbyPlayer(0, 1, Vector3.Zero, PlayerRelation.Self, "Me", "Me");
        var other = new NearbyPlayer(0, 2, new Vector3(5, 0, 0), PlayerRelation.Other, "You", "You");

        var items = Labels([self, other], labels: false, selfLabel: true);

        Assert.Equal(1, Assert.Single(items).Tag);
    }

    [Fact]
    public void Centred_label_is_over_the_cube_and_flagged_for_centring()
    {
        var other = new NearbyPlayer(0, 2, new Vector3(5, 0, 0), PlayerRelation.Other, "You", "You");
        var projector = new BoxProjector(Space, Projection);
        var cube = projector.Screen(other.Position);

        var side = Assert.Single(Labels([other], position: NamePosition.Side));
        var above = Assert.Single(Labels([other], position: NamePosition.Above));

        Assert.Equal(0f, side.B.X);
        Assert.Equal(1f, above.B.X);
        Assert.Equal(cube.X, above.A.X, 3);
        Assert.True(above.A.Y < cube.Y, "the centred label sits above the cube (smaller screen y)");
        Assert.True(side.A.X > cube.X);
    }

    [Fact]
    public void Interactable_cube_is_drawn_in_the_interactable_colour()
    {
        var list = new DrawList(256);
        var thing = new NearbyPlayer(0, 2, new Vector3(5, 0, 0), PlayerRelation.Interactable, "Entrance", "Entrance");

        PlayerCubes.Add(list, Space, Projection, [thing], PlayerPalette.Default, labels: false);

        var quads = list.Items.ToArray().Where(p => p.Kind == PrimitiveKind.Quad).ToArray();
        Assert.NotEmpty(quads);
        Assert.All(quads, p => Assert.NotEqual(PlayerPalette.Default.Npc, p.Colour));
    }
}
