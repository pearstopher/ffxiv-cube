namespace Cube.Core;

/// <summary>
/// The game states that decide whether the cube is drawn (PROJECT.md, "When hidden"). Read once per frame by the
/// plugin from Dalamud's client state and condition flags (<c>docs/research-notes.md</c>, "When to hide").
/// </summary>
/// <param name="LoggedIn">Logged in to a character (false on the title screen and character select).</param>
/// <param name="HasLocalPlayer">The local player object exists; briefly false while logging in or zoning.</param>
/// <param name="BetweenAreas">A loading screen is up.</param>
/// <param name="InCutscene">A cutscene is playing.</param>
/// <param name="InGPose">Group pose is active.</param>
public readonly record struct GameState(bool LoggedIn, bool HasLocalPlayer, bool BetweenAreas, bool InCutscene, bool InGPose)
{
    /// <summary>A normal in-game state: logged in, with a character, nothing in the way.</summary>
    public static GameState Playing { get; } = new(true, true, false, false, false);
}
