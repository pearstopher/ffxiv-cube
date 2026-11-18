using System.Numerics;
using Cube.Core;
using Control = FFXIVClientStructs.FFXIV.Client.Game.Control.Control;
using CsCharacter = FFXIVClientStructs.FFXIV.Client.Game.Character.Character;
using CsObjectKind = FFXIVClientStructs.FFXIV.Client.Game.Object.ObjectKind;
using GameObjectManager = FFXIVClientStructs.FFXIV.Client.Game.Object.GameObjectManager;

namespace Cube.Game;

/// <summary>
/// Reads the local player and the players around them straight from the game's object manager and offers each to
/// <see cref="PlayerSet"/> (PROJECT.md, "Players"). The approach chosen in #10 (<c>docs/research-notes.md</c>):
/// FFXIVClientStructs, not Dalamud's object table, because it allocates nothing per frame.
/// </summary>
/// <remarks>
/// Only the 200 character slots are scanned; players never live above them. Culling, colour relation and name
/// caching happen in Core. A plain memory read: no hooks, no game functions, nothing sent.
/// </remarks>
internal static class PlayerReader
{
    /// <summary>The object table slots holding characters (0–199).</summary>
    public const int CharacterSlots = 200;

    /// <summary>
    /// The most players that can be around at once: one per even character slot (odd slots hold their mounts and
    /// minions; <c>docs/research-notes.md</c>, #10). The efficiency checks measure at this cap.
    /// </summary>
    public const int MaxPlayers = CharacterSlots / 2;

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

    /// <summary>Offers every player character in the object table to the set; the set keeps those inside the cube.</summary>
    /// <param name="set">This frame's player set, already begun with the frame's cube.</param>
    public static unsafe void Read(PlayerSet set)
    {
        var manager = GameObjectManager.Instance();
        var control = Control.Instance();
        if (manager == null || control == null)
        {
            return;
        }

        var local = (FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*)control->LocalPlayer;
        for (var slot = 0; slot < CharacterSlots; slot++)
        {
            var obj = manager->Objects.IndexSorted[slot].Value;
            if (obj == null || obj->ObjectKind != CsObjectKind.Pc)
            {
                continue;
            }

            var character = (CsCharacter*)obj;
            set.Offer(slot, obj->EntityId, obj->Position, obj == local, character->IsPartyMember, character->IsAllianceMember, obj->Name);
        }
    }
}
