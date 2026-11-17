namespace Cube.Core;

/// <summary>
/// One colour per player relation (PROJECT.md, "Colours": self, party/alliance members, everyone else; one colour
/// each). The settings window (#20) lets the user change them.
/// </summary>
/// <param name="Self">The local player's colour.</param>
/// <param name="Party">Party and alliance members' colour.</param>
/// <param name="Other">Everyone else's colour.</param>
public readonly record struct PlayerPalette(uint Self, uint Party, uint Other)
{
    /// <summary>
    /// Defaults: blue for self, green for party/alliance, orange for others, chosen to stay distinct from each other
    /// and from the grey wireframe.
    /// </summary>
    public static PlayerPalette Default { get; } = new(
        Colour.Pack(0.25f, 0.6f, 1f, 1f),
        Colour.Pack(0.3f, 0.95f, 0.4f, 1f),
        Colour.Pack(1f, 0.6f, 0.2f, 1f));

    /// <summary>The colour for a relation.</summary>
    /// <param name="relation">Self, party or other.</param>
    public uint For(PlayerRelation relation) => relation switch
    {
        PlayerRelation.Self => this.Self,
        PlayerRelation.Party => this.Party,
        _ => this.Other,
    };
}
