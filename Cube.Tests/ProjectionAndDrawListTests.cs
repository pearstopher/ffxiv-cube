using System.Numerics;
using Cube.Core;

namespace Cube.Tests;

/// <summary>
/// Checks projection and the draw list (PROJECT.md, "View": fixed-angle view from above and behind, default 30°;
/// "Draw order": far things first so near cubes cover far ones; "Cheap drawing": only visible faces, the big cube a
/// wireframe; "Cube on screen never turns by itself": the camera turns the contents, never the wireframe;
/// "Efficiency and memory": reused buffers, no per-frame allocations).
/// </summary>
public class ProjectionAndDrawListTests
{
    private static readonly float Elevation = ViewOrientation.DefaultPitchDegrees * MathF.PI / 180f;
    private static readonly Vector2 Origin = new(200, 150);

    private static ViewProjection Projection() => new(ViewOrientation.Default, Origin, pixelsPerYalm: 10);

    private static CubeSpace Space(float yaw = 0) => new(Vector3.Zero, 40, yaw);

    [Fact]
    public void Default_elevation_is_30_degrees()
    {
        Assert.Equal(30f, ViewOrientation.DefaultPitchDegrees);
        // Looking down 30° below the horizontal, straight ahead (no heading, no roll).
        var forward = ViewOrientation.Default.Forward;
        Assert.Equal(-MathF.Sin(Elevation), forward.Y, 5);
        Assert.Equal(MathF.Cos(Elevation), forward.Z, 5);
        Assert.Equal(0f, forward.X, 5);
        Assert.Equal(1f, ViewOrientation.Default.Right.X, 5);
    }

    [Fact]
    public void Cube_centre_projects_to_the_origin()
    {
        Assert.Equal(Origin, Projection().ToScreen(Vector3.Zero));
    }

    [Fact]
    public void Right_in_view_is_right_on_screen()
    {
        Assert.Equal(new Vector2(250, 150), Projection().ToScreen(new Vector3(5, 0, 0)));
    }

    [Fact]
    public void Up_is_up_on_screen_foreshortened_by_the_elevation()
    {
        var screen = Projection().ToScreen(new Vector3(0, 10, 0));

        Assert.Equal(200, screen.X, 3);
        Assert.Equal(150 - (100 * MathF.Cos(Elevation)), screen.Y, 3);
    }

    [Fact]
    public void Forward_appears_higher_on_screen_and_further_away()
    {
        var projection = Projection();
        var near = new Vector3(0, 0, -5);
        var far = new Vector3(0, 0, 5);

        Assert.True(projection.ToScreen(far).Y < projection.ToScreen(near).Y);
        Assert.True(projection.Depth(far) > projection.Depth(near));
    }

    [Fact]
    public void Higher_points_are_nearer_the_viewer_above()
    {
        var projection = Projection();

        Assert.True(projection.Depth(new Vector3(0, 5, 0)) < projection.Depth(new Vector3(0, -5, 0)));
    }

    [Theory]
    [InlineData(0f)]
    [InlineData(0.785f)]
    [InlineData(2f)]
    public void Fitted_cube_stays_inside_the_region_at_any_yaw(float yaw)
    {
        var regionMin = new Vector2(10, 20);
        var regionSize = new Vector2(300, 200);
        var space = Space(yaw);
        var projection = ViewProjection.Fit(regionMin, regionSize, space.HalfEdge, ViewOrientation.Default);

        for (var i = 0; i < 8; i++)
        {
            var corner = new Vector3((i & 1) != 0 ? 20 : -20, (i & 2) != 0 ? 20 : -20, (i & 4) != 0 ? 20 : -20);
            var screen = projection.ToScreen(space.RotateOffset(corner));
            Assert.InRange(screen.X, regionMin.X, regionMin.X + regionSize.X);
            Assert.InRange(screen.Y, regionMin.Y, regionMin.Y + regionSize.Y);
        }
    }

    [Fact]
    public void Fitted_projection_is_centred_in_the_region()
    {
        var projection = ViewProjection.Fit(new Vector2(10, 10), new Vector2(300, 300), 20, ViewOrientation.Default);

        Assert.Equal(new Vector2(160, 160), projection.Origin);
        Assert.True(projection.PixelsPerYalm > 0);
    }

    [Fact]
    public void Big_cube_is_a_wireframe_of_12_edges()
    {
        var list = new DrawList(64);

        CubeGeometry.AddWireCube(list, Space(0.3f), Projection(), 0xFFFFFFFF);

        Assert.Equal(12, list.Count);
        foreach (var primitive in list.Items)
        {
            Assert.Equal(PrimitiveKind.Line, primitive.Kind);
        }
    }

    [Fact]
    public void Wireframe_is_identical_for_any_camera_yaw()
    {
        // PROJECT.md: "Cube on screen never turns by itself": only the contents turn with the camera.
        var projection = Projection();
        var reference = new DrawList(16);
        CubeGeometry.AddWireCube(reference, Space(0), projection, 0xFFFFFFFF);

        foreach (var yaw in new[] { 0.3f, MathF.PI / 4, 2f, -2.7f })
        {
            var turned = new DrawList(16);
            CubeGeometry.AddWireCube(turned, Space(yaw), projection, 0xFFFFFFFF);
            Assert.Equal(reference.Items.ToArray(), turned.Items.ToArray());
        }
    }

    [Fact]
    public void Contents_turn_with_the_camera_while_the_wireframe_stays()
    {
        var projection = Projection();
        var east = new Vector3(10, 0, 0);

        var atNorth = projection.ToScreen(Space(0).ToView(east));
        var atEast = projection.ToScreen(Space(MathF.PI / 2).ToView(east));

        Assert.True(Vector2.Distance(atNorth, atEast) > 10, $"{atNorth} vs {atEast}");
    }

    [Fact]
    public void Solid_cube_facing_north_shows_top_and_the_face_towards_the_viewer()
    {
        // Yaw 0: the east and west faces are edge-on, so only the top and the south face are drawn.
        var list = new DrawList(16);

        CubeGeometry.AddSolidCube(list, Space(0), Projection(), Vector3.Zero, 1, 0xFF0000FF);

        Assert.Equal(2, list.Count);
        Assert.All(list.Items.ToArray(), p => Assert.Equal(PrimitiveKind.Quad, p.Kind));
    }

    [Fact]
    public void Solid_cube_at_an_angle_shows_three_faces()
    {
        var list = new DrawList(16);

        CubeGeometry.AddSolidCube(list, Space(0.6f), Projection(), Vector3.Zero, 1, 0xFF0000FF);

        Assert.Equal(3, list.Count);
    }

    [Fact]
    public void Top_face_is_brightest_and_sides_are_shaded()
    {
        var list = new DrawList(16);

        CubeGeometry.AddSolidCube(list, Space(0.6f), Projection(), Vector3.Zero, 1, Colour.Pack(1, 1, 1, 1));

        var colours = list.Items.ToArray().Select(p => p.Colour & 0xFF).Order().ToArray();
        Assert.Equal(255u, colours[^1]);
        Assert.True(colours[0] < 255);
    }

    [Fact]
    public void Far_cubes_are_drawn_before_near_cubes()
    {
        var list = new DrawList(32);
        var space = Space();
        var projection = Projection();
        CubeGeometry.AddSolidCube(list, space, projection, new Vector3(0, 0, -10), 1, 0xFF0000FF); // near
        CubeGeometry.AddSolidCube(list, space, projection, new Vector3(0, 0, 10), 1, 0xFF00FF00);  // far

        list.SortBackToFront();

        var items = list.Items;
        Assert.Equal(projection.Depth(new Vector3(0, 0, 10)), items[0].Depth);
        Assert.Equal(projection.Depth(new Vector3(0, 0, -10)), items[^1].Depth);
        for (var i = 1; i < items.Length; i++)
        {
            Assert.True(items[i - 1].Depth >= items[i].Depth);
        }
    }

    [Fact]
    public void Equal_depths_keep_the_order_they_were_added_in()
    {
        var list = new DrawList(8);
        list.AddLine(Vector2.Zero, Vector2.One, 1, 1);
        list.AddLine(Vector2.Zero, Vector2.One, 1, 2);
        list.AddLine(Vector2.Zero, Vector2.One, 1, 3);

        list.SortBackToFront();

        Assert.Equal([1u, 2u, 3u], list.Items.ToArray().Select(p => p.Colour));
    }

    [Fact]
    public void Sort_key_orders_exactly_like_depth_then_insertion_order()
    {
        float[] depths = [-1e6f, -12.5f, -1f, -1e-7f, -0f, 0f, 1e-7f, 0.5f, 1f, 12.5f, 3e5f, float.MaxValue, float.MinValue];
        var primitives = new List<DrawPrimitive>();
        for (var i = 0; i < depths.Length * 2; i++)
        {
            primitives.Add(new DrawPrimitive { Depth = depths[i % depths.Length], Sequence = i });
        }

        foreach (var x in primitives)
        {
            foreach (var y in primitives)
            {
                Assert.Equal(Math.Sign(DrawList.CompareBackToFront(x, y)), Math.Sign(DrawList.SortKey(x).CompareTo(DrawList.SortKey(y))));
            }
        }
    }

    [Fact]
    public void Full_list_drops_extra_primitives_instead_of_growing()
    {
        var list = new DrawList(2);

        list.AddLine(Vector2.Zero, Vector2.One, 0, 1);
        list.AddLine(Vector2.Zero, Vector2.One, 0, 2);
        list.AddLine(Vector2.Zero, Vector2.One, 0, 3);

        Assert.Equal(2, list.Count);
        Assert.Equal(1, list.Dropped);
        Assert.Equal(2, list.Capacity);
    }

    [Fact]
    public void Clear_empties_the_list_for_the_next_frame()
    {
        var list = new DrawList(2);
        list.AddLine(Vector2.Zero, Vector2.One, 0, 1);
        list.AddLine(Vector2.Zero, Vector2.One, 0, 1);
        list.AddLine(Vector2.Zero, Vector2.One, 0, 1);

        list.Clear();

        Assert.Equal(0, list.Count);
        Assert.Equal(0, list.Dropped);
    }

    [Fact]
    public void Building_and_sorting_a_frame_does_not_allocate()
    {
        var list = new DrawList(256);
        var space = Space(0.4f);
        var projection = ViewProjection.Fit(Vector2.Zero, new Vector2(400, 300), space.HalfEdge, ViewOrientation.Default);

        Allocations.AssertNone(() =>
        {
            list.Clear();
            CubeGeometry.AddWireCube(list, space, projection, 0xFFFFFFFF);
            for (var i = 0; i < 40; i++)
            {
                CubeGeometry.AddSolidCube(list, space, projection, new Vector3(i - 20, 0, 20 - i), 0.5f, 0xFF00FF00);
            }

            list.SortBackToFront();
        });
    }

    [Fact]
    public void Shade_darkens_colour_but_keeps_alpha()
    {
        var shaded = Colour.Shade(Colour.Pack(1, 0.5f, 0, 0.5f), 0.5f);

        Assert.Equal(Colour.Pack(0.5f, 0.25f, 0, 0.5f), shaded, new ChannelTolerance());
    }

    [Fact]
    public void Pack_uses_the_imgui_layout()
    {
        Assert.Equal(0xFF0000FFu, Colour.Pack(1, 0, 0, 1)); // red is the low byte
        Assert.Equal(0x80FF0000u, Colour.Pack(0, 0, 1, 0.5f));
    }

    /// <summary>Compares packed colours allowing one step of rounding per channel.</summary>
    private sealed class ChannelTolerance : IEqualityComparer<uint>
    {
        public bool Equals(uint x, uint y)
        {
            for (var shift = 0; shift < 32; shift += 8)
            {
                if (Math.Abs((int)((x >> shift) & 0xFF) - (int)((y >> shift) & 0xFF)) > 1)
                {
                    return false;
                }
            }

            return true;
        }

        public int GetHashCode(uint obj) => 0;
    }
}
