using System.IO.Compression;
using System.Numerics;
using Cube.Snapshot;

namespace Cube.Tests;

/// <summary>
/// Checks the dev-only snapshot renderer (PROJECT.md, "Automated testing": a dev tool turns a dumped draw list
/// into a PNG) paints what the dump says, in order, and writes a valid PNG.
/// </summary>
public class SnapshotTests
{
    private const uint Red = 0xFF0000FF;
    private const uint Green = 0xFF00FF00;

    [Fact]
    public void Quad_fills_its_inside_only()
    {
        var canvas = new Canvas(10, 10, 0xFF000000);

        canvas.FillConvex([new(2, 2), new(8, 2), new(8, 8), new(2, 8)], Red);

        Assert.Equal(Red, canvas.GetPixel(5, 5));
        Assert.Equal(0xFF000000, canvas.GetPixel(0, 0));
        Assert.Equal(0xFF000000, canvas.GetPixel(9, 9));
    }

    [Fact]
    public void Winding_order_does_not_matter()
    {
        var canvas = new Canvas(10, 10, 0xFF000000);

        canvas.FillConvex([new(2, 8), new(8, 8), new(8, 2), new(2, 2)], Red);

        Assert.Equal(Red, canvas.GetPixel(5, 5));
    }

    [Fact]
    public void Half_transparent_colour_blends_with_the_background()
    {
        var canvas = new Canvas(4, 4, 0xFF000000);

        canvas.FillConvex([new(0, 0), new(4, 0), new(4, 4), new(0, 4)], 0x80FFFFFF);

        Assert.InRange(canvas.GetPixel(1, 1) & 0xFF, 127u, 129u);
    }

    [Fact]
    public void Line_is_drawn_between_its_ends()
    {
        var canvas = new Canvas(20, 10, 0xFF000000);

        canvas.DrawLine(new Vector2(2, 5), new Vector2(18, 5), Green, 2);

        Assert.Equal(Green, canvas.GetPixel(10, 5));
        Assert.Equal(0xFF000000, canvas.GetPixel(10, 1));
    }

    [Fact]
    public void Dump_is_painted_in_order_so_later_primitives_cover_earlier_ones()
    {
        // Draw lists are sorted far to near, so the second (nearer) quad must end up on top.
        const string json = """
            {
              "regionMin": [100, 50],
              "regionSize": [20, 20],
              "primitives": [
                { "kind": "quad", "points": [[100, 50], [120, 50], [120, 70], [100, 70]], "colour": 4278190335 },
                { "kind": "quad", "points": [[105, 55], [115, 55], [115, 65], [105, 65]], "colour": 4278255360 },
                { "kind": "line", "points": [[100, 69], [120, 69]], "colour": 4294967295, "thickness": 1 }
              ]
            }
            """;

        var canvas = SnapshotRenderer.Render(json);

        Assert.Equal(20, canvas.Width);
        Assert.Equal(20, canvas.Height);
        Assert.Equal(Green, canvas.GetPixel(10, 10));
        Assert.Equal(Red, canvas.GetPixel(2, 2));
        Assert.Equal(0xFFFFFFFF, canvas.GetPixel(10, 19));
    }

    [Fact]
    public void Core_frame_paints_the_wireframe_and_a_cube_at_the_centre()
    {
        var size = new Vector2(200, 200);
        var space = new Cube.Core.CubeSpace(Vector3.Zero, 40, 0.5f);
        var projection = Cube.Core.ViewProjection.Fit(Vector2.Zero, size, space.HalfEdge, MathF.PI / 6);
        var list = new Cube.Core.DrawList(64);
        Cube.Core.CubeGeometry.AddWireCube(list, space, projection, 0xFFFFFFFF);
        Cube.Core.CubeGeometry.AddSolidCube(list, space, projection, Vector3.Zero, 2, Green);
        list.SortBackToFront();

        var canvas = SnapshotRenderer.Paint(Vector2.Zero, size, list.Items);

        // The centre shows one of the cube's faces: green, possibly shaded darker, never red or blue.
        var centre = canvas.GetPixel(100, 100);
        Assert.True(((centre >> 8) & 0xFF) > 100, $"Centre pixel {centre:X8} isn't the green cube.");
        Assert.Equal(0u, centre & 0x00FF00FF);
        Assert.Equal(SnapshotRenderer.Background, canvas.GetPixel(0, 0));
    }

    [Fact]
    public void Png_has_the_signature_size_and_pixels()
    {
        var canvas = new Canvas(3, 2, Red);

        var png = PngWriter.Encode(canvas);

        Assert.Equal(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, png[..8]);
        Assert.Equal("IHDR", System.Text.Encoding.ASCII.GetString(png, 12, 4));
        Assert.Equal(3, System.Buffers.Binary.BinaryPrimitives.ReadInt32BigEndian(png.AsSpan(16)));
        Assert.Equal(2, System.Buffers.Binary.BinaryPrimitives.ReadInt32BigEndian(png.AsSpan(20)));

        var raw = Inflate(png);
        Assert.Equal(2 * ((3 * 4) + 1), raw.Length);
        Assert.Equal(new byte[] { 0, 255, 0, 0, 255 }, raw[..5]); // filter byte, then the first RGBA pixel
    }

    [Fact]
    public void Png_chunk_checksums_are_valid()
    {
        var png = PngWriter.Encode(new Canvas(2, 2, Green));

        // The IEND chunk's CRC is a fixed, well-known value.
        Assert.Equal(new byte[] { 0xAE, 0x42, 0x60, 0x82 }, png[^4..]);
    }

    private static byte[] Inflate(byte[] png)
    {
        // IDAT follows IHDR (8 signature + 25 IHDR bytes).
        var offset = 8 + 25;
        var length = System.Buffers.Binary.BinaryPrimitives.ReadInt32BigEndian(png.AsSpan(offset));
        Assert.Equal("IDAT", System.Text.Encoding.ASCII.GetString(png, offset + 4, 4));
        using var input = new MemoryStream(png, offset + 8, length);
        using var zlib = new ZLibStream(input, CompressionMode.Decompress);
        using var output = new MemoryStream();
        zlib.CopyTo(output);
        return output.ToArray();
    }
}
