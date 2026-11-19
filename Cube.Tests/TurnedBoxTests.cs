using System.Numerics;
using Cube.Core;

namespace Cube.Tests;

/// <summary>
/// Checks that collision boxes turn to fit their objects (PROJECT.md, "Collision (phase 2)": simplified boxes clipped to
/// the cube; "Efficiency and memory": no allocations in the per-frame path). A box from a pure yaw keeps its turn, a
/// tilted or sheared matrix falls back to the world-axis box, and clipping a turned box to a turned cube stays inside
/// both shapes (#110 research, #123).
/// </summary>
public class TurnedBoxTests
{
    private const float Tolerance = 1e-3f;
    private static readonly Vector3 Centre = new(120, 10, -40);

    private static CollisionBox Fence(float yaw)
    {
        // A 10 x 0.3 yalm fence, 2 tall, placed at Centre.
        var world = Matrix4x4.CreateRotationY(yaw) * Matrix4x4.CreateTranslation(Centre);
        return CollisionBox.FromTransformed(new Vector3(-5, -1, -0.15f), new Vector3(5, 1, 0.15f), world);
    }

    // Whether a world X/Z point lies inside a turned box's footprint.
    private static bool InsideBox(in CollisionBox box, Vector2 p)
    {
        var d = p - new Vector2(box.Centre.X, box.Centre.Z);
        var x = (d.X * box.Cos) + (d.Y * box.Sin);
        var z = (d.Y * box.Cos) - (d.X * box.Sin);
        return MathF.Abs(x) <= box.OwnHalfSize.X + Tolerance && MathF.Abs(z) <= box.OwnHalfSize.Z + Tolerance;
    }

    private static bool InsideCube(in CubeSpace cube, Vector2 p)
    {
        var v = cube.ToView(new Vector3(p.X, cube.Centre.Y, p.Y));
        return MathF.Abs(v.X) <= cube.Half.X + Tolerance && MathF.Abs(v.Z) <= cube.Half.Z + Tolerance;
    }

    [Theory]
    [InlineData(0.4f)]
    [InlineData(MathF.PI / 4)]
    [InlineData(2.2f)]
    [InlineData(-1.0f)]
    public void PureYawMatrix_GivesATurnedBoxKeepingTheLocalSize(float yaw)
    {
        var box = Fence(yaw);

        Assert.True(box.IsTurned);
        Assert.Equal(5f, box.OwnHalfSize.X, 1e-3f);
        Assert.Equal(1f, box.OwnHalfSize.Y, 1e-3f);
        Assert.Equal(0.15f, box.OwnHalfSize.Z, 1e-3f);
        Assert.True(Vector3.Distance(Centre, box.Centre) < Tolerance);
        Assert.Equal(1f, (box.Cos * box.Cos) + (box.Sin * box.Sin), 1e-4f);
    }

    [Fact]
    public void TurnedFence_WorldAxisBoxIsStillTheBoundsAround()
    {
        var box = Fence(MathF.PI / 4);

        var reach = (5f + 0.15f) * MathF.Sqrt(0.5f);
        Assert.True(Vector3.Distance(new Vector3(Centre.X - reach, 9, Centre.Z - reach), box.Min) < Tolerance);
        Assert.True(Vector3.Distance(new Vector3(Centre.X + reach, 11, Centre.Z + reach), box.Max) < Tolerance);
    }

    [Theory]
    [InlineData(0f)]
    [InlineData(MathF.PI / 2)]
    [InlineData(MathF.PI)]
    [InlineData(-MathF.PI / 2)]
    public void QuarterTurns_LineUpWithTheWorldAxes(float yaw)
    {
        var box = Fence(yaw);

        Assert.False(box.IsTurned);
        var quarter = Math.Abs(Math.Round(yaw / (MathF.PI / 2))) % 2 == 1;
        var expected = quarter ? new Vector3(0.15f, 1, 5) : new Vector3(5, 1, 0.15f);
        Assert.True(Vector3.Distance(expected, box.HalfSize) < Tolerance);
    }

    [Fact]
    public void ScaleFoldsIntoTheHalfSize_AndMirroringChangesNothing()
    {
        var world = Matrix4x4.CreateScale(2, 0.5f, -3) * Matrix4x4.CreateRotationY(0.7f) * Matrix4x4.CreateTranslation(Centre);

        var box = CollisionBox.FromTransformed(-Vector3.One, Vector3.One, world);

        Assert.True(box.IsTurned);
        Assert.True(Vector3.Distance(new Vector3(2, 0.5f, 3), box.OwnHalfSize) < Tolerance);
        Assert.True(Box_CornersMatchTheMatrix(box, world));
    }

    private static bool Box_CornersMatchTheMatrix(in CollisionBox box, in Matrix4x4 world)
    {
        for (var i = 0; i < 8; i++)
        {
            var corner = new Vector3((i & 1) == 0 ? -1 : 1, (i & 2) == 0 ? -1 : 1, (i & 4) == 0 ? -1 : 1);
            var p = Vector3.Transform(corner, world);
            if (!InsideBox(box, new Vector2(p.X, p.Z)) || p.Y < box.Min.Y - Tolerance || p.Y > box.Max.Y + Tolerance)
            {
                return false;
            }
        }

        return true;
    }

    [Fact]
    public void TiltedMatrix_FallsBackToTheWorldAxisBox()
    {
        var world = Matrix4x4.CreateRotationX(0.3f) * Matrix4x4.CreateRotationY(0.6f) * Matrix4x4.CreateTranslation(Centre);

        var box = CollisionBox.FromTransformed(-Vector3.One, Vector3.One, world);

        Assert.False(box.IsTurned);
        for (var i = 0; i < 8; i++)
        {
            var corner = new Vector3((i & 1) == 0 ? -1 : 1, (i & 2) == 0 ? -1 : 1, (i & 4) == 0 ? -1 : 1);
            var p = Vector3.Transform(corner, world);
            Assert.True(p.X >= box.Min.X - Tolerance && p.X <= box.Max.X + Tolerance);
            Assert.True(p.Y >= box.Min.Y - Tolerance && p.Y <= box.Max.Y + Tolerance);
            Assert.True(p.Z >= box.Min.Z - Tolerance && p.Z <= box.Max.Z + Tolerance);
        }
    }

    [Fact]
    public void ShearedMatrix_FallsBackToTheWorldAxisBox()
    {
        var world = Matrix4x4.Identity;
        world.M31 = 0.5f;

        Assert.False(CollisionBox.FromTransformed(-Vector3.One, Vector3.One, world).IsTurned);
    }

    [Fact]
    public void DegenerateMatrix_DoesNotThrowAndIsNotTurned()
    {
        var box = CollisionBox.FromTransformed(-Vector3.One, Vector3.One, new Matrix4x4());

        Assert.False(box.IsTurned);
    }

    [Fact]
    public void TurnedBoxes_AreNeverMerged()
    {
        var a = Fence(0.5f);

        Assert.False(CollisionBoxSet.TryMerge(a, a, out _));
    }

    [Fact]
    public void FenceAt45_ClippedToANorthUpCube_KeepsItsThinFootprint()
    {
        var cube = new CubeSpace(Centre, 40, 0);

        Assert.True(CollisionClipper.TryClip(cube, Fence(MathF.PI / 4), out var clipped));

        Assert.False(clipped.IsRectangle);
        Assert.Equal(4, clipped.CornerCount);
        var area = Area(clipped);
        Assert.Equal(10f * 0.3f, area, 1e-2f);
    }

    [Fact]
    public void TurnedBoxWhollyOutsideTheCube_IsDropped_EvenIfItsWorldAxisBoxReachesIn()
    {
        // The cube is 40 wide, so reaches 20 from the centre. A long fence turned 45° ending just past the corner
        // has a world-axis box that overlaps the cube's corner while the fence itself does not.
        var cube = new CubeSpace(Centre, 40, 0);
        var world = Matrix4x4.CreateRotationY(MathF.PI / 4) * Matrix4x4.CreateTranslation(Centre + new Vector3(25, 0, 20));
        var fence = CollisionBox.FromTransformed(new Vector3(-12, -1, -0.15f), new Vector3(12, 1, 0.15f), world);

        Assert.True(fence.Min.X < Centre.X + 20 && fence.Min.Z < Centre.Z + 20, "Test set-up: the world-axis box must reach the cube.");
        Assert.False(CollisionClipper.TryClip(cube, fence, out _));
    }

    [Theory]
    [InlineData(0f, 0.3f)]
    [InlineData(0.5f, 0f)]
    [InlineData(0.5f, 0.9f)]
    [InlineData(0.785f, 0.785f)]
    [InlineData(1.2f, -0.4f)]
    public void TurnedBoxClippedToTurnedCube_EveryCornerIsInsideBothShapes(float boxYaw, float cubeYaw)
    {
        var cube = new CubeSpace(Centre, 40, cubeYaw);
        var box = new CollisionBox(
            Centre + new Vector3(8, 0, 3),
            new Vector3(14, 3, 9),
            MathF.Cos(boxYaw),
            MathF.Sin(boxYaw));

        Assert.True(CollisionClipper.TryClip(cube, box, out var clipped));

        for (var i = 0; i < clipped.CornerCount; i++)
        {
            var corner = clipped.Corner(i);
            Assert.True(InsideCube(cube, corner), $"corner {i} {corner} outside the cube");
            Assert.True(InsideBox(box, corner), $"corner {i} {corner} outside the box");
        }
    }

    [Fact]
    public void TurnedBoxWhollyInsideTheCube_KeepsItsFourTurnedCorners()
    {
        var cube = new CubeSpace(Centre, 40, 0.3f);
        var box = new CollisionBox(Centre, new Vector3(3, 1, 1), MathF.Cos(0.8f), MathF.Sin(0.8f));

        Assert.True(CollisionClipper.TryClip(cube, box, out var clipped));

        Assert.Equal(4, clipped.CornerCount);
        Assert.Equal(4f * 3f * 1f, Area(clipped), 1e-2f);
    }

    [Fact]
    public void TwoTurnedRectangles_CanMakeAnEightCornerFootprint()
    {
        // A square box and a square cube turned 45° to each other overlap in an octagon.
        var cube = new CubeSpace(Centre, 20, MathF.PI / 4);
        var box = new CollisionBox(Centre, new Vector3(10, 2, 10), 1f, 1e-3f);

        Assert.True(CollisionClipper.TryClip(cube, box, out var clipped));

        Assert.Equal(8, clipped.CornerCount);
    }

    [Fact]
    public void TurnedBoxFuzz_EveryClippedCornerIsInsideBothShapes()
    {
        var rng = new Random(123);
        for (var n = 0; n < 2000; n++)
        {
            var cube = new CubeSpace(Centre, 40, (float)(rng.NextDouble() * 6.28));
            var yaw = (float)(rng.NextDouble() * 6.28);
            var box = new CollisionBox(
                Centre + new Vector3(((float)rng.NextDouble() - 0.5f) * 60, 0, ((float)rng.NextDouble() - 0.5f) * 60),
                new Vector3(0.1f + ((float)rng.NextDouble() * 15), 5, 0.1f + ((float)rng.NextDouble() * 15)),
                MathF.Cos(yaw),
                MathF.Sin(yaw));

            if (!CollisionClipper.TryClip(cube, box, out var clipped))
            {
                continue;
            }

            Assert.InRange(clipped.CornerCount, 3, ClippedBox.MaxCorners);
            for (var i = 0; i < clipped.CornerCount; i++)
            {
                Assert.True(InsideCube(cube, clipped.Corner(i)), $"case {n}: corner outside the cube");
                if (!clipped.IsRectangle)
                {
                    Assert.True(InsideBox(box, clipped.Corner(i)), $"case {n}: corner outside the box");
                }
            }
        }
    }

    [Fact]
    public void ClippingTurnedBoxes_DoesNotAllocate()
    {
        var cube = new CubeSpace(Centre, 40, 0.3f);
        var source = new CollisionBox[200];
        for (var i = 0; i < source.Length; i++)
        {
            source[i] = new CollisionBox(Centre + new Vector3((i % 20) - 10, 0, (i / 20) - 5), new Vector3(3, 1, 0.2f), MathF.Cos(i), MathF.Sin(i));
        }

        var destination = new ClippedBox[source.Length];

        Allocations.AssertNone(() => CollisionClipper.Clip(cube, source, destination));
    }

    private static float Area(in ClippedBox box)
    {
        var sum = 0f;
        var origin = box.Corner(0);
        for (var i = 0; i < box.CornerCount; i++)
        {
            var p = box.Corner(i) - origin;
            var q = box.Corner((i + 1) % box.CornerCount) - origin;
            sum += (p.X * q.Y) - (q.X * p.Y);
        }

        return MathF.Abs(sum) / 2f;
    }
}
