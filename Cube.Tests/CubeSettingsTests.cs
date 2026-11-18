using System.Numerics;
using System.Text.Json;
using Cube.Core;

namespace Cube.Tests;

/// <summary>
/// Checks the settings' defaults and repair rules (PROJECT.md, "Features": settings for cube size, orientation mode,
/// labels on/off, colours and opacity; "NPCs": on by default, purple; "Cube": width, height and depth each 10–100, default 40, zoom keeping their ratios, old single-edge files migrating (#90); "View": Camera
/// mode by default; #32: collision on, full detail by default; saved files from older versions still load) and the
/// <c>/cube config</c> command.
/// </summary>
public class CubeSettingsTests
{
    /// <summary>
    /// Serialises public fields, as Dalamud's configuration serialiser does, so these tests see the same saved shape.
    /// </summary>
    private static readonly JsonSerializerOptions JsonOptions = new() { IncludeFields = true };

    [Fact]
    public void Defaults_match_the_spec()
    {
        var settings = new CubeSettings();

        Assert.Equal(new Vector3(40f), settings.CubeSize);
        Assert.Equal(OrientationMode.Camera, settings.Orientation);
        Assert.True(settings.ShowLabels);
        Assert.True(settings.ShowNpcs);
        Assert.Equal(new Vector3(0.8f, 0.4f, 1f), settings.NpcColour);
        Assert.Equal(1f, settings.Opacity);
        Assert.True(settings.ShowCollision);
        Assert.Equal(CollisionDetail.Full, settings.CollisionDetail);
    }

    [Fact]
    public void Config_saved_before_collision_settings_existed_loads_with_full_collision_on()
    {
        const string old = """{ "CubeEdge": 60, "ShowNpcs": false }""";

        var settings = JsonSerializer.Deserialize<CubeSettings>(old, JsonOptions)!;
        settings.Changed();

        Assert.True(settings.ShowCollision);
        Assert.Equal(CollisionDetail.Full, settings.CollisionDetail);
    }

    [Fact]
    public void Collision_settings_survive_a_save_and_load()
    {
        var saved = new CubeSettings { ShowCollision = false, CollisionDetail = CollisionDetail.Coarse };

        var loaded = JsonSerializer.Deserialize<CubeSettings>(JsonSerializer.Serialize(saved, JsonOptions), JsonOptions)!;

        Assert.False(loaded.ShowCollision);
        Assert.Equal(CollisionDetail.Coarse, loaded.CollisionDetail);
    }

    [Fact]
    public void Unknown_collision_detail_falls_back_to_full()
    {
        var settings = new CubeSettings { CollisionDetail = (CollisionDetail)9 };

        settings.Changed();

        Assert.Equal(CollisionDetail.Full, settings.CollisionDetail);
    }

    [Fact]
    public void Default_colours_are_four_different_colours()
    {
        var palette = new CubeSettings().Palette();

        Assert.Equal(4, new[] { palette.Self, palette.Party, palette.Other, palette.Npc }.Distinct().Count());
        Assert.Equal(PlayerPalette.Default, palette);
    }

    [Fact]
    public void Config_saved_before_npc_settings_existed_loads_with_npcs_on_in_purple()
    {
        // A version 1 file from before #74: no ShowNpcs or NpcColour.
        const string old = """{ "CubeEdge": 60, "ShowLabels": false, "Opacity": 0.5 }""";

        var settings = JsonSerializer.Deserialize<CubeSettings>(old, JsonOptions)!;
        settings.Upgrade(1);

        Assert.Equal(new Vector3(60f), settings.CubeSize);
        Assert.False(settings.ShowLabels);
        Assert.True(settings.ShowNpcs);
        Assert.Equal(CubeSettings.DefaultNpcColour, settings.NpcColour);
    }

    [Fact]
    public void Npc_settings_survive_a_save_and_load()
    {
        var saved = new CubeSettings { ShowNpcs = false, NpcColour = new Vector3(0.1f, 0.2f, 0.3f) };

        var loaded = JsonSerializer.Deserialize<CubeSettings>(JsonSerializer.Serialize(saved, JsonOptions), JsonOptions)!;

        Assert.False(loaded.ShowNpcs);
        Assert.Equal(saved.NpcColour, loaded.NpcColour);
    }

    [Fact]
    public void Corrupt_npc_colour_falls_back_to_purple()
    {
        var settings = new CubeSettings { NpcColour = new Vector3(0, float.NaN, 0) };

        settings.Changed();

        Assert.Equal(CubeSettings.DefaultNpcColour, settings.NpcColour);
    }

    [Theory]
    [InlineData(5f, 10f)]
    [InlineData(500f, 100f)]
    [InlineData(float.NaN, 40f)]
    [InlineData(65f, 65f)]
    public void Cube_size_is_kept_within_10_to_100(float stored, float expected)
    {
        var settings = new CubeSettings { CubeSize = new Vector3(stored, 50, stored) };

        settings.Changed();

        Assert.Equal(new Vector3(expected, 50, expected), settings.CubeSize);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void Old_single_edge_becomes_all_three_extents(int fileVersion)
    {
        const string old = """{ "CubeEdge": 72, "Version": 2 }""";
        var settings = JsonSerializer.Deserialize<CubeSettings>(old, JsonOptions)!;

        settings.Upgrade(fileVersion);

        Assert.Equal(new Vector3(72f), settings.CubeSize);
    }

    [Fact]
    public void Old_out_of_range_edge_is_clamped_when_migrated()
    {
        var settings = new CubeSettings { CubeEdge = 400 };

        settings.Upgrade(CubeSettings.RotationViewVersion);

        Assert.Equal(new Vector3(100f), settings.CubeSize);
    }

    [Fact]
    public void Current_files_keep_their_box_size_and_ignore_any_old_edge()
    {
        var settings = new CubeSettings { CubeSize = new Vector3(20, 80, 30), CubeEdge = 60 };

        settings.Upgrade(CubeSettings.BoxSizeVersion);

        Assert.Equal(new Vector3(20, 80, 30), settings.CubeSize);
    }

    [Fact]
    public void Box_size_survives_a_save_and_load()
    {
        var saved = new CubeSettings { CubeSize = new Vector3(15, 90, 45) };

        var loaded = JsonSerializer.Deserialize<CubeSettings>(JsonSerializer.Serialize(saved, JsonOptions), JsonOptions)!;
        loaded.Upgrade(CubeSettings.BoxSizeVersion);

        Assert.Equal(saved.CubeSize, loaded.CubeSize);
    }

    [Fact]
    public void Old_edge_is_never_written_again()
    {
        Assert.False(new CubeSettings().ShouldSerializeCubeEdge());
    }

    [Fact]
    public void Zoom_scales_every_extent_and_keeps_the_ratios()
    {
        var settings = new CubeSettings { CubeSize = new Vector3(20, 40, 30) };
        var before = settings.Revision;

        settings.Zoom(1.5f);

        Assert.Equal(new Vector3(30, 60, 45), settings.CubeSize);
        Assert.True(settings.Revision > before);
    }

    [Fact]
    public void Reset_size_gives_40_by_40_by_40()
    {
        var settings = new CubeSettings { CubeSize = new Vector3(20, 90, 30) };

        settings.ResetSize();

        Assert.Equal(new Vector3(40f), settings.CubeSize);
    }

    [Fact]
    public void Reset_to_defaults_gives_40_by_40_by_40()
    {
        var settings = new CubeSettings { CubeSize = new Vector3(20, 90, 30) };

        settings.CopyFrom(new CubeSettings());

        Assert.Equal(new Vector3(40f), settings.CubeSize);
    }

    [Fact]
    public void Set_size_clamps_each_extent()
    {
        var settings = new CubeSettings();

        settings.SetSize(new Vector3(2, 55, 300));

        Assert.Equal(new Vector3(10, 55, 100), settings.CubeSize);
    }

    [Theory]
    [InlineData(0f, CubeSettings.MinOpacity)]
    [InlineData(2f, 1f)]
    [InlineData(float.NaN, 1f)]
    public void Opacity_is_kept_visible_and_at_most_full(float stored, float expected)
    {
        var settings = new CubeSettings { Opacity = stored };

        settings.Changed();

        Assert.Equal(expected, settings.Opacity);
    }

    [Fact]
    public void Unknown_orientation_falls_back_to_camera()
    {
        var settings = new CubeSettings { Orientation = (OrientationMode)42 };

        settings.Changed();

        Assert.Equal(OrientationMode.Camera, settings.Orientation);
    }

    [Fact]
    public void Corrupt_colour_falls_back_to_the_default_and_out_of_range_is_clamped()
    {
        var settings = new CubeSettings { SelfColour = new Vector3(float.NaN, 0, 0), OtherColour = new Vector3(2, -1, 0.5f) };

        settings.Changed();

        Assert.Equal(CubeSettings.DefaultSelfColour, settings.SelfColour);
        Assert.Equal(new Vector3(1, 0, 0.5f), settings.OtherColour);
    }

    [Fact]
    public void Opacity_applies_to_every_player_colour()
    {
        var settings = new CubeSettings { Opacity = 0.5f };
        settings.Changed();

        var palette = settings.Palette();

        Assert.All(new[] { palette.Self, palette.Party, palette.Other, palette.Npc }, c => Assert.Equal(128u, c >> 24));
    }

    [Fact]
    public void Chosen_colour_is_used_for_its_relation()
    {
        var settings = new CubeSettings { PartyColour = new Vector3(1, 0, 1) };
        settings.Changed();

        Assert.Equal(Colour.Pack(1, 0, 1, 1), settings.Palette().For(PlayerRelation.Party));
    }

    [Fact]
    public void Every_change_bumps_the_revision()
    {
        var settings = new CubeSettings();
        var before = settings.Revision;

        settings.Changed();
        settings.Changed();

        Assert.Equal(before + 2, settings.Revision);
    }

    [Fact]
    public void Copying_settings_copies_every_value_and_counts_as_a_change()
    {
        var source = new CubeSettings
        {
            CubeSize = new Vector3(70, 20, 30), Orientation = OrientationMode.NorthUp, ShowLabels = false,
            SelfColour = Vector3.One, PartyColour = Vector3.Zero, OtherColour = new Vector3(0.5f), Opacity = 0.3f,
            ShowNpcs = false, NpcColour = new Vector3(0.2f), ShowCollision = false, CollisionDetail = CollisionDetail.Medium,
        };
        var target = new CubeSettings();
        var before = target.Revision;

        target.CopyFrom(source);

        Assert.Equal(source.CubeSize, target.CubeSize);
        Assert.Equal(source.Orientation, target.Orientation);
        Assert.Equal(source.ShowLabels, target.ShowLabels);
        Assert.Equal(source.SelfColour, target.SelfColour);
        Assert.Equal(source.PartyColour, target.PartyColour);
        Assert.Equal(source.OtherColour, target.OtherColour);
        Assert.Equal(source.Opacity, target.Opacity);
        Assert.Equal(source.ShowNpcs, target.ShowNpcs);
        Assert.Equal(source.NpcColour, target.NpcColour);
        Assert.Equal(source.ShowCollision, target.ShowCollision);
        Assert.Equal(source.CollisionDetail, target.CollisionDetail);
        Assert.True(target.Revision > before);
    }

    [Theory]
    [InlineData("config", true)]
    [InlineData("  Config ", true)]
    [InlineData("", false)]
    [InlineData(null, false)]
    [InlineData("configure", false)]
    public void Cube_config_opens_settings_and_anything_else_toggles_the_cube(string? args, bool isConfig)
    {
        Assert.Equal(isConfig, CommandArgs.IsConfig(args));
    }
}
