using System.Numerics;

namespace Cube.Core;

/// <summary>
/// Chooses the cube's alignment yaw (PROJECT.md, "Cube"): in <b>Camera</b> mode it follows the game camera's
/// horizontal direction so "forward in the cube" matches the game view; in <b>North up</b> mode it is fixed with
/// north at the front. The yaw turns the contents, never the wireframe (see <see cref="CubeSpace"/>).
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
    /// meaningless, so the previous yaw is kept rather than spinning the contents.
    /// </summary>
    public const float MinHorizontalLength = 1e-4f;

    /// <summary>The yaw for north-up mode: the cube's front face points north.</summary>
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

    /// <summary>
    /// Smallest yaw change, in radians (about 0.1°), that turns the contents. Smaller changes, such as camera jitter or
    /// float noise while the camera is still, keep the previous yaw so the rotation isn't recomputed (PROJECT.md,
    /// "Efficiency and memory": recompute only when inputs change). Far below what a pixel shows at any window size.
    /// </summary>
    public const float ChangeThreshold = 0.002f;

    /// <summary>
    /// Keeps the previous yaw unless the new one differs by at least <see cref="ChangeThreshold"/>, measured the
    /// short way round (so −π and π count as equal).
    /// </summary>
    /// <param name="previous">The yaw in use.</param>
    /// <param name="next">This frame's wanted yaw.</param>
    /// <returns><paramref name="previous"/> or <paramref name="next"/>.</returns>
    public static float Settle(float previous, float next)
    {
        var difference = MathF.IEEERemainder(next - previous, 2 * MathF.PI);
        return MathF.Abs(difference) < ChangeThreshold ? previous : next;
    }

    /// <summary>The yaw for the given mode.</summary>
    /// <param name="mode">Camera or north up.</param>
    /// <param name="cameraForward">The game camera's world direction (used in Camera mode).</param>
    /// <param name="previous">The last yaw, kept if the camera direction is vertical.</param>
    public static float For(OrientationMode mode, Vector3 cameraForward, float previous) =>
        mode == OrientationMode.NorthUp ? NorthUp : FromForward(cameraForward, previous);
}
