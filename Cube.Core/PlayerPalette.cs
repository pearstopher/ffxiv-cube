namespace Cube.Core;

/// <summary>
/// One colour per player relation (PROJECT.md, "Colours": self, party/alliance members, everyone else; one colour
/// each). Built from the user's <see cref="CubeSettings"/>.
/// </summary>
/// <param name="Self">The local player's colour.</param>
/// <param name="Party">Party and alliance members' colour.</param>
/// <param name="Other">Everyone else's colour.</param>
public readonly record struct PlayerPalette(uint Self, uint Party, uint Other)
{
    /// <summary>
    /// Defaults: blue for self, green for party/alliance, orange for others, fully opaque, chosen to stay distinct
    /// from each other and from the grey wireframe (<see cref="CubeSettings.DefaultSelfColour"/> and siblings).
    /// </summary>
    public static PlayerPalette Default { get; } = new CubeSettings().Palette();

    /// <summary>The colour for a relation.</summary>
    /// <param name="relation">Self, party or other.</param>
    public uint For(PlayerRelation relation) => relation switch
    {
        PlayerRelation.Self => this.Self,
        PlayerRelation.Party => this.Party,
        _ => this.Other,
    };
}
