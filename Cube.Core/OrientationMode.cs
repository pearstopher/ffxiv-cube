namespace Cube.Core;

/// <summary>
/// How the cube is aligned, and so what happens to its contents (PROJECT.md, "Cube": the orientation setting). The
/// cube's frame stays still on screen in both modes.
/// </summary>
public enum OrientationMode
{
    /// <summary>
    /// "Rotate contents with camera", the default: the cube is aligned to the camera's horizontal direction, so
    /// "forward in the cube" matches the game view and the contents rotate around the player as the camera turns.
    /// </summary>
    Camera = 0,

    /// <summary>"Keep contents fixed (north stays put)": the cube is aligned to the world with north at the front.</summary>
    NorthUp = 1,
}
