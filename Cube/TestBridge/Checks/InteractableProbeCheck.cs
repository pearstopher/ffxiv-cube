using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Dalamud.Plugin.Services;
using Control = FFXIVClientStructs.FFXIV.Client.Game.Control.Control;
using CsObjectKind = FFXIVClientStructs.FFXIV.Client.Game.Object.ObjectKind;
using GameObjectManager = FFXIVClientStructs.FFXIV.Client.Game.Object.GameObjectManager;

namespace Cube.TestBridge.Checks;

/// <summary>
/// Research probe for #112 (which in-world objects does the game label with words, and which with icons?): walks the
/// object table for a few seconds and logs every distinct (kind, name, nameplate icon, targetable) combination of the
/// objects that are neither characters nor NPCs, so a single session in a housing area, a city and a dungeon answers
/// which fields tell a text label from an icon-only one.
/// </summary>
/// <remarks>
/// Only objects within 100 yalms of the player are listed (the largest cube is 100 across). The names logged are
/// object names such as "Entrance", never player names. The check passes whenever the game is reachable; the findings
/// are in <c>dalamud.log</c> as <c>Probe #112</c> lines and are recorded in <c>docs/research-notes.md</c>.
/// </remarks>
/// <param name="log">Where the findings are logged, at Information.</param>
internal sealed class InteractableProbeCheck(IPluginLog log) : BridgeCheck
{
    /// <summary>How long the probe watches, in seconds.</summary>
    private const double SampleSeconds = 3;

    /// <summary>Furthest an object can be on any axis and still be listed.</summary>
    private const float Reach = 100f;

    /// <inheritdoc/>
    public override string Name => "probe-interactables";

    /// <inheritdoc/>
    public override IEnumerable<CheckStep> Run(BridgeContext context)
    {
        var seen = new Dictionary<string, int>();
        var started = System.Diagnostics.Stopwatch.GetTimestamp();
        while (System.Diagnostics.Stopwatch.GetElapsedTime(started).TotalSeconds < SampleSeconds)
        {
            Sample(seen);
            yield return CheckStep.NextFrame;
        }

        var text = new StringBuilder();
        foreach (var (key, count) in seen)
        {
            text.Append(CultureInfo.InvariantCulture, $"\n  {key} count={count}");
        }

        log.Information("Probe #112: {Count} distinct object kinds/names/icons around the player:{List}", seen.Count, text.ToString());
        context.Metric("interactableCombos", seen.Count);
        yield return CheckStep.Pass($"{seen.Count} distinct non-character objects logged.");
    }

    /// <summary>Records one frame's objects.</summary>
    /// <param name="seen">Distinct combinations and the largest per-frame count is not needed: the count is frames seen.</param>
    private static unsafe void Sample(Dictionary<string, int> seen)
    {
        var manager = GameObjectManager.Instance();
        var control = Control.Instance();
        if (manager == null || control == null || control->LocalPlayer == null)
        {
            return;
        }

        System.Numerics.Vector3 centre = control->LocalPlayer->Position;
        var table = manager->Objects.IndexSorted;
        for (var i = 0; i < table.Length; i++)
        {
            var obj = table[i].Value;
            if (obj == null)
            {
                continue;
            }

            var kind = obj->ObjectKind;
            if (kind == CsObjectKind.Pc || kind == CsObjectKind.BattleNpc || kind == CsObjectKind.EventNpc)
            {
                continue;
            }

            System.Numerics.Vector3 position = obj->Position;
            var offset = position - centre;
            if (MathF.Abs(offset.X) > Reach || MathF.Abs(offset.Y) > Reach || MathF.Abs(offset.Z) > Reach)
            {
                continue;
            }

            var key = $"{kind} name='{obj->NameString}' icon={obj->NamePlateIconId} targetable={obj->GetIsTargetable()} targetStatus={obj->TargetableStatus}";
            seen[key] = seen.GetValueOrDefault(key) + 1;
        }
    }
}
