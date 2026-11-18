using Cube.Core;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;

namespace Cube.Game;

/// <summary>
/// Reads the states that hide the cube (PROJECT.md, "When hidden") from Dalamud, using the sources chosen in #10
/// (<c>docs/research-notes.md</c>, "When to hide"): <see cref="IClientState.IsLoggedIn"/>,
/// <see cref="IClientState.IsGPosing"/> and <see cref="IClientState.IsPvP"/>, and the loading-screen and cutscene
/// condition flags.
/// </summary>
/// <remarks>
/// Dalamud can also hide plugin windows in cutscenes and GPose, but only if the user's Dalamud settings say so, so
/// the plugin checks for itself. <see cref="IClientState.IsPvP"/> is used rather than the variant that excludes the
/// Wolves' Den: the lobby isn't a match, but hiding there too is simpler and costs nothing (#69). Each read is a
/// property or flag lookup: no allocation.
/// </remarks>
/// <param name="clientState">Login, GPose and PvP state.</param>
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
        clientState.IsGPosing,
        clientState.IsPvP);

    /// <summary>
    /// Whether the game says the local player is jumping (PROJECT.md, "Jump"; #105): the cube holds its height while
    /// this is true. Both jump flags are accepted, as the cutscene and loading flags are, because which one the game
    /// raises isn't known until the owner's manual session (<c>docs/manual-tests.md</c>, "Jump"). A flag lookup:
    /// read-only, no allocation.
    /// </summary>
    public bool IsJumping() => condition[ConditionFlag.Jumping] || condition[ConditionFlag.Jumping61];

#if DEBUG
    /// <summary>
    /// Debug logging for the jump research (#105): while a jump flag is set, and for half a second after it clears, one
    /// line per frame with both flags and the player's height. Allocates only while logging, which is never in
    /// steady state, so the draw budget is unaffected. Debug builds only.
    /// </summary>
    /// <param name="playerY">The local player's real height this frame.</param>
    public void TraceJump(float playerY)
    {
        var flags = (condition[ConditionFlag.Jumping] ? 1 : 0) | (condition[ConditionFlag.Jumping61] ? 2 : 0);
        var now = System.Diagnostics.Stopwatch.GetTimestamp();
        if (flags != 0)
        {
            this.lastFlagTicks = now;
        }

        var window = System.Diagnostics.Stopwatch.Frequency / 2;
        if (this.lastFlagTicks != 0 && now - this.lastFlagTicks <= window)
        {
            Plugin.Log.Debug("Jump trace: Jumping={Jumping} Jumping61={Jumping61} y={Y:0.000}", (flags & 1) != 0, (flags & 2) != 0, playerY);
        }
    }

    /// <summary>When a jump flag was last seen set, in stopwatch ticks; 0 before the first. Debug builds only.</summary>
    private long lastFlagTicks;
#endif
}
