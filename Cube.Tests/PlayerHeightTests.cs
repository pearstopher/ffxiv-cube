using System.Numerics;
using System.Text.Json;
using Cube.Core;

namespace Cube.Tests;

/// <summary>
/// Checks "Player height in cube" (PROJECT.md, "Exact behaviour rules", Cube; #91): a fraction p of the box's height,
/// 0–1, default 0.5; the box spans <c>player.y - height × p</c> to <c>player.y + height × (1 - p)</c> with x and z
/// centred on the player; culling uses the shifted box; on screen the box stays centred, so the player's marker moves
/// off the middle; the setting is saved, older configs load as 50%, and reset to defaults restores it. Also the
/// zero-allocation rule ("Efficiency and memory").
/// </summary>
public class PlayerHeightTests
{
    private static readonly Vector3 Player = new(100, 20, -50);

    private static readonly JsonSerializerOptions JsonOptions = new() { IncludeFields = true };

    private static CubeSpace Box(float fraction, Vector3? size = null) =>
        new(Player, size ?? BoxExtents.DefaultSize, ViewYaw.NorthUp, fraction);

    [Fact]
    public void At_50_percent_the_box_is_the_centred_cube()
    {
        var centred = new CubeSpace(Player, new Vector3(30, 60, 50), 0.4f);
        var shifted = new CubeSpace(Player, new Vector3(30, 60, 50), 0.4f, PlayerHeight.Default);
        var point = Player + new Vector3(3, 25, -7);

        Assert.Equal(centred.Centre, shifted.Centre);
        Assert.Equal(Player, shifted.Player);
        Assert.Equal(centred.ToView(point), shifted.ToView(point));
        Assert.Equal(centred.Contains(point), shifted.Contains(point));
    }

    [Fact]
    public void At_0_percent_things_just_below_are_culled_and_things_near_the_top_kept()
    {
        var box = Box(0f);

        Assert.False(box.Contains(Player - new Vector3(0, 1, 0)));
        Assert.True(box.Contains(Player + new Vector3(0, 39, 0)));
        Assert.True(box.Contains(Player));
    }

    [Fact]
    public void At_100_percent_things_just_above_are_culled_and_things_near_the_bottom_kept()
    {
        var box = Box(1f);

        Assert.False(box.Contains(Player + new Vector3(0, 1, 0)));
        Assert.True(box.Contains(Player - new Vector3(0, 39, 0)));
        Assert.True(box.Contains(Player));
    }

    [Theory]
    [InlineData(0f)]
    [InlineData(0.25f)]
    [InlineData(0.5f)]
    [InlineData(1f)]
    public void Bounds_span_height_times_p_below_and_height_times_one_minus_p_above(float p)
    {
        var box = Box(p, new Vector3(40, 80, 40));

        var bottom = box.Centre.Y - box.Half.Y;
        var top = box.Centre.Y + box.Half.Y;

        Assert.Equal(Player.Y - (80 * p), bottom, 3);
        Assert.Equal(Player.Y + (80 * (1 - p)), top, 3);
    }

    [Fact]
    public void Shift_is_vertical_only()
    {
        var box = Box(0.1f);

        Assert.Equal(Player.X, box.Centre.X);
        Assert.Equal(Player.Z, box.Centre.Z);
    }

    [Fact]
    public void Bounds_follow_a_zoomed_height()
    {
        var zoomed = BoxExtents.Zoom(new Vector3(20, 40, 30), 2f);
        var box = Box(0.25f, zoomed);

        Assert.Equal(Player.Y - (zoomed.Y * 0.25f), box.Centre.Y - box.Half.Y, 3);
        Assert.Equal(Player.Y + (zoomed.Y * 0.75f), box.Centre.Y + box.Half.Y, 3);
    }

    [Fact]
    public void Bounds_use_the_clamped_height()
    {
        var box = Box(0f, new Vector3(40, 500, 40));

        Assert.Equal(Player.Y, box.Centre.Y - box.Half.Y, 3);
        Assert.Equal(Player.Y + BoxExtents.Max, box.Centre.Y + box.Half.Y, 3);
    }

    [Theory]
    [InlineData(-0.5f, 0f)]
    [InlineData(1.5f, 1f)]
    [InlineData(float.NaN, 0.5f)]
    [InlineData(0.3f, 0.3f)]
    public void Fraction_is_clamped_to_0_to_1(float requested, float expected)
    {
        Assert.Equal(expected, PlayerHeight.Clamp(requested));
    }

    [Fact]
    public void Player_marker_moves_off_the_middle_on_screen()
    {
        var region = new Vector2(400, 400);
        var view = ViewOrientation.FromDegrees(0, 0);
        var centred = Box(0.5f);
        var bottom = Box(0f);
        var top = Box(1f);

        float ScreenY(in CubeSpace space)
        {
            var projection = ViewProjection.Fit(Vector2.Zero, region, space.Half, view);
            return projection.ToScreen(space.ToView(space.Player)).Y - projection.ToScreen(Vector3.Zero).Y;
        }

        Assert.Equal(0f, ScreenY(centred), 3);
        Assert.True(ScreenY(bottom) > 10f, "At 0% the player is drawn below the middle.");
        Assert.True(ScreenY(top) < -10f, "At 100% the player is drawn above the middle.");
    }

    [Fact]
    public void Recentred_box_matches_a_fresh_one()
    {
        var moved = Player + new Vector3(4, -3, 2);
        var size = new Vector3(30, 70, 50);
        var fresh = new CubeSpace(moved, size, 0.9f, 0.2f);

        var recentred = new CubeSpace(Player, 40, 0.9f).Recentred(moved, size, 0.2f);

        Assert.Equal(fresh.Centre, recentred.Centre);
        Assert.Equal(fresh.Player, recentred.Player);
        Assert.Equal(fresh.ToView(moved), recentred.ToView(moved));
    }

    [Fact]
    public void Setting_defaults_to_50_percent_and_is_repaired()
    {
        Assert.Equal(0.5f, new CubeSettings().PlayerHeight);

        var settings = new CubeSettings { PlayerHeight = 3f };
        settings.Changed();
        Assert.Equal(1f, settings.PlayerHeight);
    }

    [Fact]
    public void Setting_is_saved_and_loaded()
    {
        var saved = new CubeSettings { PlayerHeight = 0.2f };

        var loaded = JsonSerializer.Deserialize<CubeSettings>(JsonSerializer.Serialize(saved, JsonOptions), JsonOptions)!;
        loaded.Upgrade(CubeSettings.PlayerHeightVersion);

        Assert.Equal(0.2f, loaded.PlayerHeight);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Older_configs_load_as_50_percent(int fileVersion)
    {
        var settings = JsonSerializer.Deserialize<CubeSettings>("""{ "PlayerHeight": 0.9 }""", JsonOptions)!;

        settings.Upgrade(fileVersion);

        Assert.Equal(PlayerHeight.Default, settings.PlayerHeight);
    }

    [Fact]
    public void Reset_to_defaults_restores_50_percent()
    {
        var settings = new CubeSettings { PlayerHeight = 0f };

        settings.CopyFrom(new CubeSettings());

        Assert.Equal(PlayerHeight.Default, settings.PlayerHeight);
    }

    [Fact]
    public void Copying_settings_keeps_the_player_height()
    {
        var settings = new CubeSettings();

        settings.CopyFrom(new CubeSettings { PlayerHeight = 0.7f });

        Assert.Equal(0.7f, settings.PlayerHeight);
    }

    [Fact]
    public void Building_and_culling_with_a_player_height_do_not_allocate()
    {
        var space = Box(0.2f);
        var point = Player + new Vector3(3, 10, -4);
        var sink = Vector3.Zero;

        Allocations.AssertNone(() =>
        {
            space = space.Recentred(Player, BoxExtents.DefaultSize, 0.2f);
            if (space.TryToView(point, out var view))
            {
                sink += view;
            }
        });

        Assert.NotEqual(Vector3.Zero, sink);
    }
}
