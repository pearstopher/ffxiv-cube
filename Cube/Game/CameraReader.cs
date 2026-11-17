using System.Numerics;
using CameraManager = FFXIVClientStructs.FFXIV.Client.Game.Control.CameraManager;

namespace Cube.Game;

/// <summary>
/// Reads which way the game camera is looking, for the Camera orientation mode (PROJECT.md, "View": the view's
/// yaw follows the game camera so "forward on screen" matches the game view).
/// </summary>
/// <remarks>
/// Uses the scene camera's look-at point minus its position, a world-space direction with no angle convention
/// to guess (<c>docs/research-notes.md</c>, #10). A plain memory read on the framework/draw thread: no hooks,
/// no game functions called, no allocation.
/// </remarks>
internal static class CameraReader
{
    /// <summary>
    /// Gets the camera's world-space forward direction (not normalised).
    /// </summary>
    /// <param name="forward">The direction, if a camera exists.</param>
    /// <returns>False when there is no game camera (e.g. not logged in); the caller keeps its previous yaw.</returns>
    public static unsafe bool TryGetForward(out Vector3 forward)
    {
        forward = default;
        var manager = CameraManager.Instance();
        if (manager == null || manager->Camera == null)
        {
            return false;
        }

        ref var scene = ref manager->Camera->SceneCamera;
        Vector3 eye = scene.Position;
        Vector3 lookAt = scene.LookAtVector;
        forward = lookAt - eye;
        return true;
    }
}
