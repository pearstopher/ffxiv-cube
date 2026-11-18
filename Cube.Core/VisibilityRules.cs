namespace Cube.Core;

/// <summary>
/// Decides whether to draw (PROJECT.md, "When hidden": not logged in, loading screens, cutscenes, GPose and PvP
/// areas; the window is just empty then, never an error).
/// </summary>
/// <remarks>
/// When hidden, the window skips everything else too: no player reading, no geometry (PROJECT.md, "Efficiency and
/// memory": do work only when needed). If several apply, the first in the order below is reported.
/// </remarks>
public static class VisibilityRules
{
    /// <summary>Whether the cube is drawn in this state, and if not, why.</summary>
    /// <param name="state">This frame's game state.</param>
    public static Visibility For(in GameState state) =>
        !state.LoggedIn || !state.HasLocalPlayer ? Visibility.NotLoggedIn
        : state.BetweenAreas ? Visibility.Loading
        : state.InCutscene ? Visibility.Cutscene
        : state.InGPose ? Visibility.GPose
        : state.InPvP ? Visibility.PvP
        : Visibility.Shown;
}
