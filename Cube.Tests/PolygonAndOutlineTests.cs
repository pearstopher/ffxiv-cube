using System.Numerics;
using Cube.Core;

namespace Cube.Tests;

/// <summary>
/// Checks the draw list's polygons and the cube's screen outline (#93; PROJECT.md, "Cheap drawing": one ImGui call per
/// face; "Efficiency and memory": fixed buffers, nothing allocated per frame, bounded memory; "Collision (phase 2)":
/// collision clipped to the cube, which the outline is used to check).
/// </summary>
public class PolygonAndOutlineTests
{
    private static readonly Vector2[] Pentagon = [new(0, 0), new(10, 0), new(12, 6), new(5, 10), new(-2, 6)];

    [Fact]
    public void Polygon_keeps_its_corners_through_the_sort()
    {
        var list = new DrawList(8);
        list.AddQuad(Vector2.Zero, Vector2.One, Vector2.One, Vector2.Zero, depth: 1f, colour: 1);
        list.AddPolygon(Pentagon, depth: 5f, colour: 2, tag: -3);

        list.SortBackToFront();

        var polygon = list.Items[0];
        Assert.Equal(PrimitiveKind.Polygon, polygon.Kind);
        Assert.Equal(-3, polygon.Tag);
        Assert.Equal(Pentagon, list.Points(polygon).ToArray());
        Assert.True(list.Points(list.Items[1]).IsEmpty);
    }

    [Fact]
    public void Polygon_is_dropped_when_the_point_buffer_is_full_and_room_returns_after_clear()
    {
        var list = new DrawList(8, pointCapacity: 8);
        list.AddPolygon(Pentagon, 1f, 1);

        list.AddPolygon(Pentagon, 1f, 1);

        Assert.Equal(1, list.Count);
        Assert.Equal(1, list.Dropped);

        list.Clear();
        list.AddPolygon(Pentagon, 1f, 1);
        Assert.Equal(1, list.Count);
        Assert.Equal(0, list.Dropped);
    }

    [Fact]
    public void Adding_polygons_does_not_allocate()
    {
        var list = new DrawList(64);
        var corners = Pentagon;

        Allocations.AssertNone(() =>
        {
            list.Clear();
            for (var i = 0; i < 10; i++)
            {
                list.AddPolygon(corners, i, 1);
            }

            list.SortBackToFront();
        });
    }

    [Theory]
    [InlineData(0f, 0f)]
    [InlineData(0.7f, 0f)]
    [InlineData(0.7f, 70f)]
    public void Cube_outline_holds_its_own_corners_and_centre_but_not_points_beyond(float yaw, float viewYaw)
    {
        var space = new CubeSpace(new Vector3(10, 0, 10), new Vector3(40, 20, 30), yaw);
        var view = viewYaw == 0 ? ViewOrientation.Default : ViewOrientation.FromDegrees(viewYaw, 25f);
        var projection = ViewProjection.Fit(Vector2.Zero, new Vector2(400, 300), space.Half, view);

        var outline = new CubeOutline(space, projection);

        Assert.True(outline.Contains(projection.Origin, 0f));
        var lo = new Vector2(float.MaxValue);
        var hi = new Vector2(float.MinValue);
        for (var i = 0; i < 8; i++)
        {
            var corner = projection.ToScreen(new Vector3((i & 1) != 0 ? 20 : -20, (i & 2) != 0 ? 10 : -10, (i & 4) != 0 ? 15 : -15));
            Assert.True(outline.Contains(corner, 0.01f));
            lo = Vector2.Min(lo, corner);
            hi = Vector2.Max(hi, corner);
        }

        Assert.False(outline.Contains(new Vector2(hi.X + 1, projection.Origin.Y), 0.01f));
        Assert.False(outline.Contains(new Vector2(projection.Origin.X, lo.Y - 1), 0.01f));
        Assert.False(outline.Contains(hi + Vector2.One, 0.01f));
    }
}
