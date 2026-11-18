namespace Cube.Core;

/// <summary>
/// One colour per relation (PROJECT.md, "Colours": self, party/alliance members, everyone else, NPCs; one colour
/// each). Built from the user's <see cref="CubeSettings"/>.
/// </summary>
/// <param name="Self">The local player's colour.</param>
/// <param name="Party">Party and alliance members' colour.</param>
/// <param name="Other">Every other player's colour.</param>
/// <param name="Npc">Named NPCs' colour.</param>
public readonly record struct PlayerPalette(uint Self, uint Party, uint Other, uint Npc)
{
    /// <summary>
    /// Defaults: blue for self, green for party/alliance, orange for others, purple for NPCs, fully opaque, chosen to
    /// stay distinct from each other and from the grey wireframe (<see cref="CubeSettings.DefaultSelfColour"/> and
    /// siblings).
    /// </summary>
    public static PlayerPalette Default { get; } = new CubeSettings().Palette();

    /// <summary>The colour for a relation.</summary>
    /// <param name="relation">Self, party, other or NPC.</param>
    public uint For(PlayerRelation relation) => relation switch
    {
        PlayerRelation.Self => this.Self,
        PlayerRelation.Party => this.Party,
        PlayerRelation.Npc => this.Npc,
        _ => this.Other,
    };
}
