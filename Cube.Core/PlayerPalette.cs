namespace Cube.Core;

/// <summary>
/// One colour per relation (PROJECT.md, "Colours": self, party/alliance members, everyone else, NPCs; one colour
/// each; #111: enemies too). Built from the user's <see cref="CubeSettings"/>.
/// </summary>
/// <param name="Self">The local player's colour.</param>
/// <param name="Party">Party and alliance members' colour.</param>
/// <param name="Other">Every other player's colour.</param>
/// <param name="Npc">Named NPCs' colour.</param>
/// <param name="Enemy">Hostile battle NPCs' colour (#111).</param>
public readonly record struct PlayerPalette(uint Self, uint Party, uint Other, uint Npc, uint Enemy)
{
    /// <summary>
    /// Defaults: blue for self, green for party/alliance, orange for others, purple for NPCs, red for enemies, fully
    /// opaque, chosen to stay distinct from each other and from the grey wireframe
    /// (<see cref="CubeSettings.DefaultSelfColour"/> and siblings).
    /// </summary>
    public static PlayerPalette Default { get; } = new CubeSettings().Palette();

    /// <summary>The colour for a relation.</summary>
    /// <param name="relation">Self, party, other, NPC or enemy.</param>
    public uint For(PlayerRelation relation) => relation switch
    {
        PlayerRelation.Self => this.Self,
        PlayerRelation.Party => this.Party,
        PlayerRelation.Npc => this.Npc,
        PlayerRelation.Enemy => this.Enemy,
        _ => this.Other,
    };
}
