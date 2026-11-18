using System.Collections.Generic;
using Cube.Core;

namespace Cube.TestBridge.Checks;

/// <summary>
/// The most expensive settings and contents the cube can have, shared by the budget and soak checks so both measure
/// the same thing (PROJECT.md, "Efficiency and memory": the phase 2 budgets hold with collision on; #33).
/// </summary>
/// <remarks>
/// Labels and NPCs on, the full crowd of fakes at the player and NPC caps (#23, #74), and collision on at full detail
/// in the largest cube (#32), whatever the user chose. Settings change in memory only; each check saves them first with
/// <see cref="CubeSettings.CopyFrom"/> and restores them in its cleanup.
/// </remarks>
internal static class WorstCase
{
    /// <summary>Switches the window to the worst case.</summary>
    /// <param name="window">The cube window.</param>
    public static void Apply(CubeWindow window)
    {
        var settings = window.Settings;
        settings.ShowLabels = true;
        settings.ShowNpcs = true;
        settings.ShowCollision = true;
        settings.CollisionDetail = CollisionDetail.Full;
        settings.CubeSize = new System.Numerics.Vector3(BoxExtents.Max);
        settings.Changed();
        window.Fakes.UseCrowd();
    }

    /// <summary>
    /// Starts a fresh collision gather for the worst-case settings and waits until it has completed, so measuring
    /// starts with the boxes drawn and gathering idle (gathering frames are checked by <see cref="CollisionReaderCheck"/>).
    /// </summary>
    /// <param name="cache">The collision cache.</param>
    public static IEnumerable<CheckStep> WaitForCollision(CollisionCache cache)
    {
        var before = cache.Completed;
        cache.Invalidate();
        while (cache.Completed == before || cache.IsGathering || !cache.Ready.HasData)
        {
            yield return CheckStep.NextFrame;
        }
    }
}
