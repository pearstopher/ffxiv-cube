namespace Cube.Core;

/// <summary>
/// How someone in the cube relates to the local player, which picks their colour (PROJECT.md, "Colours": self,
/// party/alliance members, everyone else, NPCs; one colour each).
/// </summary>
public enum PlayerRelation : byte
{
    /// <summary>Every other player.</summary>
    Other = 0,

    /// <summary>A member of the local player's party or alliance.</summary>
    Party = 1,

    /// <summary>The local player, at the centre of the cube.</summary>
    Self = 2,

    /// <summary>
    /// A named NPC (PROJECT.md, "NPCs"): drawn like a player, in its own colour. Never self or party, even when the NPC
    /// fights alongside the player.
    /// </summary>
    Npc = 3,

    /// <summary>
    /// A hostile battle NPC (#111): drawn in its own colour so enemies stand apart from friendly NPCs. Counts as an NPC
    /// everywhere else (NPC setting, NPC cap).
    /// </summary>
    Enemy = 4,

    /// <summary>
    /// An in-world interactable the game labels with words (#112): an event object such as "Entrance" or "Placard". Drawn
    /// like an NPC in its own colour, with its own cap and toggle.
    /// </summary>
    Interactable = 5,
}
