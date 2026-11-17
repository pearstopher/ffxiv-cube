using System.Numerics;

namespace Cube.Core;

/// <summary>
/// Chooses the yaw of the view around the cube (PROJECT.md, "View"): in <b>Camera</b> mode it follows the
/// game camera so "forward on screen" matches the game view; in <b>North up</b> mode it is fixed with
/// north at the back.
/// </summary>
/// <remarks>
/// Yaw convention: 0 looks north (−Z), increasing clockwise seen from above (towards east, +X). The camera's
/// direction comes from the scene camera's look-at point minus its position (<c>docs/research-notes.md</c>, #10),
/// so no game angle convention is assumed.
/// </remarks>
public static class ViewYaw
{
    /// <summary>
    /// Below this horizontal length the camera is looking (almost) straight up or down and its heading is
    /// meaningless, so the previous yaw is kept rather than spinning the view.
    /// </summary>
    public const float MinHorizontalLength = 1e-4f;

    /// <summary>The yaw for north-up mode: north at the back of the view.</summary>
    public const float NorthUp = 0f;

    /// <summary>
    /// The yaw that looks along a world direction, using only its horizontal part.
    /// </summary>
    /// <param name="forward">A world direction, e.g. camera look-at minus camera position.</param>
    /// <param name="previous">The yaw to keep if the direction is (almost) vertical.</param>
    public static float FromForward(Vector3 forward, float previous)
    {
        var horizontal = MathF.Sqrt((forward.X * forward.X) + (forward.Z * forward.Z));
        return horizontal < MinHorizontalLength ? previous : MathF.Atan2(forward.X, -forward.Z);
    }

    /// <summary>The yaw for the given mode.</summary>
    /// <param name="mode">Camera or north up.</param>
    /// <param name="cameraForward">The game camera's world direction (used in Camera mode).</param>
    /// <param name="previous">The last yaw, kept if the camera direction is vertical.</param>
    public static float For(OrientationMode mode, Vector3 cameraForward, float previous) =>
        mode == OrientationMode.NorthUp ? NorthUp : FromForward(cameraForward, previous);
}
