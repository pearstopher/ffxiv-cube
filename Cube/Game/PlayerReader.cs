using System;
using System.Numerics;
using Cube.Core;
using BattleNpcSubKind = FFXIVClientStructs.FFXIV.Client.Game.Object.BattleNpcSubKind;
using Control = FFXIVClientStructs.FFXIV.Client.Game.Control.Control;
using CsCharacter = FFXIVClientStructs.FFXIV.Client.Game.Character.Character;
using CsObjectKind = FFXIVClientStructs.FFXIV.Client.Game.Object.ObjectKind;
using GameObjectManager = FFXIVClientStructs.FFXIV.Client.Game.Object.GameObjectManager;
using TargetSystem = FFXIVClientStructs.FFXIV.Client.Game.Control.TargetSystem;
using UIState = FFXIVClientStructs.FFXIV.Client.Game.UI.UIState;

namespace Cube.Game;

/// <summary>
/// Reads the local player, the players around them and the named NPCs straight from the game's object manager and
/// offers each to <see cref="PlayerSet"/> (PROJECT.md, "Players" and "NPCs"). The approach chosen in #10
/// (<c>docs/research-notes.md</c>): FFXIVClientStructs, not Dalamud's object table, because it allocates nothing per
/// frame.
/// </summary>
/// <remarks>
/// Players and battle NPCs live in the 200 character slots; event NPCs live above them (#73), so the whole table is
/// walked only when NPCs are shown. Culling, colour relation and name caching happen in Core. A plain memory read: no
/// hooks, no game functions, nothing sent.
/// </remarks>
internal static class PlayerReader
{
    /// <summary>The object table slots holding characters (0–199).</summary>
    public const int CharacterSlots = 200;

    /// <summary>
    /// The length of the game's index-sorted object table, measured in #73 (<c>docs/research-notes.md</c>). The name
    /// cache has one slot per table index, so its size is fixed at load (PROJECT.md: bounded memory).
    /// </summary>
    public const int ObjectSlots = 819;

    /// <summary>
    /// The most players that can be around at once: one per even character slot (odd slots hold their mounts and
    /// minions; <c>docs/research-notes.md</c>, #10). The efficiency checks measure at this cap.
    /// </summary>
    public const int MaxPlayers = CharacterSlots / 2;

    /// <summary>
    /// The most named NPCs drawn per frame (#73: a busy city put 22 in the largest cube; 50 players' worth of extra
    /// drawing still fits the 0.5 ms budget alongside a full crowd of players). Extras are dropped and counted.
    /// </summary>
    public const int MaxNpcs = 50;

    /// <summary>
    /// The most text-labelled interactable objects drawn per frame (#112). A housing ward put a few dozen entrances and
    /// placards in range; the cap keeps the draw cost bounded and extras are dropped and counted.
    /// </summary>
    public const int MaxInteractables = 30;

    /// <summary>Whether an entity is on the game's list of enemies attacking the player (the "hater" list the enmity UI uses).</summary>
    /// <param name="state">The game's UI state; null is treated as an empty list.</param>
    /// <param name="entityId">The enemy's entity id.</param>
    private static unsafe bool IsHater(UIState* state, uint entityId)
    {
        if (state == null)
        {
            return false;
        }

        var haters = state->Hater.Haters;
        var count = Math.Min(state->Hater.HaterCount, haters.Length);
        for (var i = 0; i < count; i++)
        {
            if (haters[i].EntityId == entityId)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Gets the local player's world position: the cube's centre.</summary>
    /// <param name="position">The position, if logged in.</param>
    /// <returns>False when there is no local player (logged out, title screen, some loading screens).</returns>
    public static unsafe bool TryGetLocalPosition(out Vector3 position)
    {
        position = default;
        var control = Control.Instance();
        if (control == null || control->LocalPlayer == null)
        {
            return false;
        }

        position = control->LocalPlayer->Position;
        return true;
    }

    /// <summary>
    /// Offers every player character, and every named NPC if the set shows NPCs, to the set; the set keeps those
    /// inside the cube.
    /// </summary>
    /// <remarks>
    /// With NPCs off (PROJECT.md, "NPCs": the setting; <see cref="PlayerSet.ShowsNpcs"/>) only the character slots
    /// are read and no NPC is looked at.
    /// </remarks>
    /// <param name="set">This frame's player set, already begun with the frame's cube.</param>
    /// <param name="localY">
    /// The local player's height this frame, as <see cref="CubeWindow"/> read it. Used instead of the object's own so the
    /// marker and the cube agree on the same frame's value (and so the bridge's simulated jump can move the marker, #105).
    /// </param>
    public static unsafe void Read(PlayerSet set, float localY)
    {
        var npcs = set.ShowsNpcs;
        var objects = set.ShowsInteractables;
        var manager = GameObjectManager.Instance();
        var control = Control.Instance();
        if (manager == null || control == null)
        {
            return;
        }

        var local = (FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*)control->LocalPlayer;
        var targetSystem = TargetSystem.Instance();
        var target = targetSystem == null ? null : targetSystem->GetTargetObject();
        var hate = UIState.Instance();
        var table = manager->Objects.IndexSorted;
        var end = npcs || objects ? Math.Min(table.Length, ObjectSlots) : CharacterSlots;
        for (var slot = 0; slot < end; slot++)
        {
            var obj = table[slot].Value;
            if (obj == null)
            {
                continue;
            }

            var kind = obj->ObjectKind;
            if (kind == CsObjectKind.Pc)
            {
                var character = (CsCharacter*)obj;
                var position = obj->Position;
                var isLocal = obj == local;
                if (isLocal)
                {
                    position.Y = localY;
                }

                set.Offer(slot, obj->EntityId, position, isLocal, character->IsPartyMember, character->IsAllianceMember, obj->Name, obj == target);
            }
            else if (objects && kind == CsObjectKind.EventObj)
            {
                // A text-labelled interactable (#112): an event object with a name. Nameless ones are invisible
                // triggers and icon-only markers (docs/research-notes.md).
                if (PlayerSet.IsTextInteractable(obj->Name))
                {
                    set.OfferInteractable(slot, obj->EntityId, obj->Position, obj->Name, obj == target);
                }
            }
            else if (npcs && (kind == CsObjectKind.EventNpc || kind == CsObjectKind.BattleNpc))
            {
                var petOrBuddy = kind == CsObjectKind.BattleNpc
                    && (obj->BattleNpcSubKind == BattleNpcSubKind.Pet || obj->BattleNpcSubKind == BattleNpcSubKind.Buddy);
                if (PlayerSet.IsNamedNpc(obj->OwnerId, petOrBuddy, obj->Name))
                {
                    // Enemy = a hostile battle NPC; attacking = on the game's enemy list, the one the game keeps of
                    // everything fighting the player (#111; docs/research-notes.md).
                    var enemy = PlayerSet.ClassifyNpc(kind == CsObjectKind.BattleNpc, ((CsCharacter*)obj)->IsHostile) == PlayerRelation.Enemy;
                    set.OfferNpc(slot, obj->EntityId, obj->Position, obj->Name, enemy, obj == target, enemy && IsHater(hate, obj->EntityId));
                }
            }
        }
    }
}
