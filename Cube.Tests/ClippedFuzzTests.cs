using System.Numerics;
using Cube.Core;

namespace Cube.Tests;

/// <summary>
/// Fuzzes the clipper at 45° (#115; PROJECT.md, "Collision (phase 2)": clipped to the cube): whatever the box, a box
/// drawn as a plain rectangle must lie wholly inside the cube.
/// </summary>
public class ClippedFuzzTests
{
    [Fact]
    public void SliverAtCubeCorner_IsNotDrawnAsItsBoundingRectangle()
    {
        var space = new CubeSpace(new Vector3(-693, 25, -712), 40, 45f * MathF.PI / 180f);
        var box = new CollisionBox(new Vector3(-664.73975f, 24, -724.6042f), new Vector3(-652.14264f, 26, -704.91327f));

        Assert.True(CollisionClipper.TryClip(space, box, out var clipped));
        Assert.False(clipped.IsRectangle);
    }

    [Fact]
    public void PlainRectangleResult_IsInsideCube_ForRandomBoxes()
    {
        var rng = new Random(115);
        var space = new CubeSpace(new Vector3(-693, 25, -712), 40, 45f * MathF.PI / 180f);
        var bad = new List<string>();
        for (var i = 0; i < 200000; i++)
        {
            var c = space.Centre + new Vector3(((float)rng.NextDouble() - 0.5f) * 80, 0, ((float)rng.NextDouble() - 0.5f) * 80);
            var w = (float)rng.NextDouble() * (rng.Next(3) == 0 ? 0.001f : 30f);
            var d = (float)rng.NextDouble() * (rng.Next(3) == 0 ? 0.001f : 30f);
            var box = new CollisionBox(new Vector3(c.X, space.Centre.Y - 1, c.Z), new Vector3(c.X + w, space.Centre.Y + 1, c.Z + d));
            if (!CollisionClipper.TryClip(space, box, out var clipped) || !clipped.IsRectangle)
            {
                continue;
            }

            var b = clipped.Bounds;
            foreach (var x in new[] { b.Min.X, b.Max.X })
            {
                foreach (var z in new[] { b.Min.Z, b.Max.Z })
                {
                    var v = space.ToView(new Vector3(x, space.Centre.Y, z));
                    if (MathF.Abs(v.X) > space.Half.X + 1e-3f || MathF.Abs(v.Z) > space.Half.Z + 1e-3f)
                    {
                        bad.Add($"box {box.Min}..{box.Max} -> corner {x},{z} view {v.X},{v.Z} half {space.Half}");
                    }
                }
            }
        }

        Assert.True(bad.Count == 0, $"{bad.Count} bad, first: {(bad.Count > 0 ? bad[0] : "")}");
    }
}
