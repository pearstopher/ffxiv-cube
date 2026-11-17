using System.Numerics;
using Cube.Core;

namespace Cube.Tests;

/// <summary>
/// Checks the view's orientation modes (PROJECT.md, "View"): Camera mode follows the game camera's heading so
/// "forward on screen" matches the game view; North up is fixed with north at the back. The heading comes from
/// the camera's direction vector (docs/research-notes.md, #10).
/// </summary>
public class ViewYawTests
{
    [Theory]
    [InlineData(0, -1, 0)]                 // looking north
    [InlineData(1, 0, MathF.PI / 2)]       // looking east
    [InlineData(0, 1, MathF.PI)]           // looking south
    [InlineData(-1, 0, -MathF.PI / 2)]     // looking west
    public void Camera_heading_becomes_yaw_clockwise_from_north(float x, float z, float expected)
    {
        Assert.Equal(expected, ViewYaw.FromForward(new Vector3(x, -0.5f, z), previous: 0), 4);
    }

    [Fact]
    public void Camera_pitch_does_not_change_yaw()
    {
        var level = ViewYaw.FromForward(new Vector3(1, 0, -1), 0);
        var steep = ViewYaw.FromForward(new Vector3(1, -20, -1), 0);

        Assert.Equal(level, steep, 5);
    }

    [Fact]
    public void Camera_looking_straight_down_keeps_the_previous_yaw()
    {
        Assert.Equal(1.25f, ViewYaw.FromForward(new Vector3(0, -1, 0), previous: 1.25f));
    }

    [Fact]
    public void North_up_mode_ignores_the_camera()
    {
        Assert.Equal(0f, ViewYaw.For(OrientationMode.NorthUp, new Vector3(1, 0, 0), previous: 2f));
    }

    [Fact]
    public void Camera_mode_follows_the_camera()
    {
        Assert.Equal(MathF.PI / 2, ViewYaw.For(OrientationMode.Camera, new Vector3(1, 0, 0), previous: 0), 4);
    }

    [Fact]
    public void Camera_is_the_default_mode()
    {
        Assert.Equal(OrientationMode.Camera, default(OrientationMode));
    }
}
