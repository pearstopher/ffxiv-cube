namespace Cube.Core;

/// <summary>How the view around the cube is turned (PROJECT.md, "View": orientation mode setting).</summary>
public enum OrientationMode
{
    /// <summary>The view's yaw follows the game camera, so "forward on screen" matches the game view. The default.</summary>
    Camera = 0,

    /// <summary>Fixed yaw with north at the back.</summary>
    NorthUp = 1,
}
