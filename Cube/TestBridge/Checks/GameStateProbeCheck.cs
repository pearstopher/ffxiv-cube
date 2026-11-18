using System;
using System.Collections.Generic;
using System.Globalization;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Plugin.Services;
using CameraManager = FFXIVClientStructs.FFXIV.Client.Game.Control.CameraManager;
using Control = FFXIVClientStructs.FFXIV.Client.Game.Control.Control;
using CsCharacter = FFXIVClientStructs.FFXIV.Client.Game.Character.Character;
using CsObjectKind = FFXIVClientStructs.FFXIV.Client.Game.Object.ObjectKind;
using GameObjectManager = FFXIVClientStructs.FFXIV.Client.Game.Object.GameObjectManager;

namespace Cube.TestBridge.Checks;

/// <summary>
/// Research probe for #10 (reading players, party and camera): reads the same facts two ways and records
/// both, so one in-game run settles the open questions in <c>docs/research-notes.md</c>.
/// </summary>
/// <remarks>
/// <list type="bullet">
/// <item>Player count from Dalamud's <see cref="IObjectTable"/> versus a direct, allocation-free walk of
/// FFXIVClientStructs' <see cref="GameObjectManager"/> (the planned hot-path reader). They must agree.</item>
/// <item>Party and alliance counts from the <see cref="CsCharacter"/> relation flags.</item>
/// <item>The camera's yaw from <c>Camera.DirH</c> and, independently, from the scene camera's position and
/// look-at point, which fixes the angle convention against the world axes.</item>
/// <item>The hide conditions (logged in, between areas, cutscene, GPose) as seen right now.</item>
/// <item>For #73 (named NPCs), the whole object table watched for about five seconds: where event and
/// battle NPCs sit, how many there are by kind, sub-kind and owner, how many have names, how many would be
/// inside the largest cube, and whether any name changed. Totals are kept in <see cref="NpcProbe"/>.</item>
/// </list>
/// Fails only if the player isn't logged in (nothing to read) or the two player counts disagree.
/// Read-only: it only reads memory (PROJECT.md, "Read-only game access").
/// </remarks>
/// <param name="objects">Dalamud's object table: the independent read.</param>
/// <param name="clientState">Login and GPose state.</param>
/// <param name="condition">Loading-screen and cutscene condition flags.</param>
/// <param name="log">Where the probe's findings are logged, at Information so they show without Debug+.</param>
internal sealed class GameStateProbeCheck(IObjectTable objects, IClientState clientState, ICondition condition, IPluginLog log)
    : BridgeCheck
{
    /// <summary>The object table indexes holding characters: 0–199, players and battle NPCs at even indexes (FFXIVClientStructs' notes).</summary>
    private const int CharacterSlots = 200;

    /// <inheritdoc/>
    public override string Name => "probe-players-party-camera";

    /// <summary>
    /// How long the NPC part of the probe (#73) watches, in seconds: long enough to see NPCs spawn and
    /// despawn, and a name change if one happens while an NPC is in view. Time-based, not frame-based,
    /// because the game drops its frame rate in the background and the bridge stops a check after 10 s.
    /// </summary>
    private const double NpcSampleSeconds = 4;

    /// <summary>
    /// Half the largest cube edge (<see cref="Core.BoxExtents.Max"/>): NPCs within this many yalms on every
    /// axis would be inside the biggest cube the settings allow, which is the worst case for the NPC cap.
    /// </summary>
    private const float MaxHalfEdge = Core.BoxExtents.Max / 2f;

    /// <summary>The <c>OwnerId</c> value meaning "no owner" (the game's invalid entity id).</summary>
    private const uint NoOwner = 0xE0000000;

    /// <inheritdoc/>
    public override IEnumerable<CheckStep> Run(BridgeContext context)
    {
        var players = this.Probe(context);
        if (players.State != CheckState.Passed)
        {
            yield return players;
            yield break;
        }

        var npcs = new NpcProbe();
        var started = System.Diagnostics.Stopwatch.GetTimestamp();
        while (System.Diagnostics.Stopwatch.GetElapsedTime(started).TotalSeconds < NpcSampleSeconds)
        {
            this.ProbeNpcs(npcs);
            yield return CheckStep.NextFrame;
        }

        npcs.Record(context, log);
        yield return CheckStep.Pass($"{players.Message} NPCs (#73): up to {npcs.MaxNamedInMaxCube} named in the largest cube, {npcs.MaxNamed} named in the table.");
    }

    /// <summary>
    /// One frame of the NPC research for #73: walks the whole object table and counts every NPC by slot,
    /// kind, sub-kind, owner and whether it has a name, and remembers each NPC's name bytes to spot renames.
    /// </summary>
    /// <remarks>
    /// The proposed filter for #74 is counted as "named": an <c>EventNpc</c> or <c>BattleNpc</c> with a
    /// non-empty name, no owner, and not a pet or chocobo companion (PROJECT.md, "NPCs": player-owned objects
    /// are left out). Names are compared, never logged.
    /// </remarks>
    /// <param name="npcs">The running totals.</param>
    private unsafe void ProbeNpcs(NpcProbe npcs)
    {
        var manager = GameObjectManager.Instance();
        var control = Control.Instance();
        if (manager == null || control == null || control->LocalPlayer == null)
        {
            return;
        }

        var local = control->LocalPlayer;

        var centre = (System.Numerics.Vector3)local->Position;
        var table = manager->Objects.IndexSorted;
        npcs.TableLength = table.Length;
        npcs.BeginFrame();
        for (var i = 0; i < table.Length; i++)
        {
            var obj = table[i].Value;
            if (obj == null)
            {
                continue;
            }

            var kind = obj->ObjectKind;
            if (kind != CsObjectKind.EventNpc && kind != CsObjectKind.BattleNpc)
            {
                if (kind != CsObjectKind.Pc)
                {
                    npcs.CountOther(kind, i);
                }

                continue;
            }

            var name = obj->Name;
            var nameLength = name.IndexOf((byte)0);
            if (nameLength < 0)
            {
                nameLength = name.Length;
            }

            var subKind = kind == CsObjectKind.BattleNpc ? obj->BattleNpcSubKind : default;
            var owned = obj->OwnerId != NoOwner;
            System.Numerics.Vector3 position = obj->Position;
            var offset = position - centre;
            var inMaxCube = MathF.Abs(offset.X) <= MaxHalfEdge && MathF.Abs(offset.Y) <= MaxHalfEdge && MathF.Abs(offset.Z) <= MaxHalfEdge;
            npcs.CountNpc(i, kind, subKind, owned, nameLength, inMaxCube);

            if (nameLength > 0)
            {
                npcs.TrackName(((ulong)obj->EntityId << 32) | (uint)i, name[..nameLength]);
            }
        }

        npcs.EndFrame();
    }

    /// <summary>Reads everything in one frame, records it as metrics and logs a summary line.</summary>
    /// <param name="context">Where the metrics go.</param>
    private unsafe CheckStep Probe(BridgeContext context)
    {
        context.Metric("loggedIn", clientState.IsLoggedIn ? 1 : 0);
        context.Metric("gposing", clientState.IsGPosing ? 1 : 0);
        context.Metric("betweenAreas", condition[ConditionFlag.BetweenAreas] || condition[ConditionFlag.BetweenAreas51] ? 1 : 0);
        context.Metric("cutscene", condition[ConditionFlag.OccupiedInCutSceneEvent] || condition[ConditionFlag.WatchingCutscene] || condition[ConditionFlag.WatchingCutscene78] ? 1 : 0);

        var local = objects.LocalPlayer;
        if (!clientState.IsLoggedIn || local is null)
        {
            return CheckStep.Fail("Not logged in: no local player to read. Log in and leave the game running.");
        }

        // Independent read: Dalamud's object table (allocates wrappers; fine in a check).
        var dalamudPlayers = 0;
        foreach (var obj in objects)
        {
            if (obj.ObjectKind == ObjectKind.Pc)
            {
                dalamudPlayers++;
            }
        }

        // Planned hot-path read: walk the character slots of the game's object manager directly.
        var structPlayers = 0;
        var party = 0;
        var alliance = 0;
        var manager = GameObjectManager.Instance();
        for (var i = 0; i < CharacterSlots; i++)
        {
            var obj = manager->Objects.IndexSorted[i].Value;
            if (obj == null || obj->ObjectKind != CsObjectKind.Pc)
            {
                continue;
            }

            structPlayers++;
            var character = (CsCharacter*)obj;
            if (character->IsPartyMember)
            {
                party++;
            }
            else if (character->IsAllianceMember)
            {
                alliance++;
            }
        }

        context.Metric("playersDalamud", dalamudPlayers);
        context.Metric("playersStructs", structPlayers);
        context.Metric("partyFlagged", party);
        context.Metric("allianceFlagged", alliance);

        var position = local.Position;
        context.Metric("playerX", position.X);
        context.Metric("playerY", position.Y);
        context.Metric("playerZ", position.Z);
        context.Metric("playerRotation", local.Rotation);

        var dirH = float.NaN;
        var sceneYaw = float.NaN;
        var camera = CameraManager.Instance()->Camera;
        if (camera != null)
        {
            System.Numerics.Vector3 eye = camera->SceneCamera.Position;
            System.Numerics.Vector3 lookAt = camera->SceneCamera.LookAtVector;
            var forward = lookAt - eye;

            // atan2(x, z): 0 when looking along +Z, increasing towards +X. Compared with DirH in the log.
            dirH = camera->DirH;
            sceneYaw = MathF.Atan2(forward.X, forward.Z);
            context.Metric("cameraDirH", dirH);
            context.Metric("cameraDirV", camera->DirV);
            context.Metric("sceneYawAtan2XZ", sceneYaw);
            context.Metric("sceneForwardX", forward.X);
            context.Metric("sceneForwardZ", forward.Z);
        }

        log.Information(string.Create(
            CultureInfo.InvariantCulture,
            $"Probe #10: players Dalamud={dalamudPlayers} structs={structPlayers} party={party} alliance={alliance}; player at {position}, rotation {local.Rotation:0.###}; camera DirH {dirH:0.###}, scene yaw atan2(x,z) {sceneYaw:0.###}"));

        return dalamudPlayers == structPlayers
            ? CheckStep.Pass($"{structPlayers} player(s) read both ways; {party} party, {alliance} alliance. Values in metrics.")
            : CheckStep.Fail($"Player counts disagree: Dalamud object table {dalamudPlayers}, direct read {structPlayers}.");
    }
}
