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
/// <param name="InPvP">
/// In a PvP area (Crystalline Conflict, Frontline, Rival Wings, and the Wolves' Den lobby). The official Dalamud rules
/// ban any PvP advantage, and the cube would work like a radar there (<c>docs/dalamud-repo-standards.md</c>).
/// </param>
public readonly record struct GameState(bool LoggedIn, bool HasLocalPlayer, bool BetweenAreas, bool InCutscene, bool InGPose, bool InPvP)
{
    /// <summary>A normal in-game state: logged in, with a character, nothing in the way.</summary>
    public static GameState Playing { get; } = new(true, true, false, false, false, false);
}
