namespace Cube.Core;

/// <summary>
/// How a player relates to the local player, which picks their colour (PROJECT.md, "Colours": self,
/// party/alliance members, everyone else; one colour each).
/// </summary>
public enum PlayerRelation : byte
{
    /// <summary>Everyone else.</summary>
    Other = 0,

    /// <summary>A member of the local player's party or alliance.</summary>
    Party = 1,

    /// <summary>The local player, at the centre of the cube.</summary>
    Self = 2,
}
