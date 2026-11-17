using Cube.Core;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;

namespace Cube.Game;

/// <summary>
/// Reads the states that hide the cube (PROJECT.md, "When hidden") from Dalamud, using the sources chosen in #10
/// (<c>docs/research-notes.md</c>, "When to hide"): <see cref="IClientState.IsLoggedIn"/> and
/// <see cref="IClientState.IsGPosing"/>, and the loading-screen and cutscene condition flags.
/// </summary>
/// <remarks>
/// Dalamud can also hide plugin windows in cutscenes and GPose, but only if the user's Dalamud settings say so, so
/// the plugin checks for itself. Each read is a property or flag lookup: no allocation.
/// </remarks>
/// <param name="clientState">Login and GPose state.</param>
/// <param name="condition">Condition flags.</param>
internal sealed class GameStateReader(IClientState clientState, ICondition condition)
{
    /// <summary>Reads this frame's state.</summary>
    /// <param name="hasLocalPlayer">Whether the local player object was found this frame.</param>
    public GameState Read(bool hasLocalPlayer) => new(
        clientState.IsLoggedIn,
        hasLocalPlayer,
        condition[ConditionFlag.BetweenAreas] || condition[ConditionFlag.BetweenAreas51],
        condition[ConditionFlag.OccupiedInCutSceneEvent] || condition[ConditionFlag.WatchingCutscene] || condition[ConditionFlag.WatchingCutscene78],
        clientState.IsGPosing);
}
