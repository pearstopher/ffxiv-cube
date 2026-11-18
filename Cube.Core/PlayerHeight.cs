using System.Numerics;

namespace Cube.Core;

/// <summary>
/// The rules for where the player sits vertically in the box (PROJECT.md, "Exact behaviour rules", Cube, "Player height
/// in cube"; #91): a fraction <c>p</c> of the box's height, from 0 (player on the bottom face) to 1 (player on the top
/// face), default 0.5 (centred). The box spans <c>player.y - height × p</c> to <c>player.y + height × (1 - p)</c>;
/// x and z stay centred on the player.
/// </summary>
/// <remarks>
/// <para>
/// Rather than teach every reader about an off-centre player, the shift is applied once: <see cref="BoxCentre"/> turns
/// the player's position into the box's centre, and <see cref="CubeSpace"/> is built around that centre. Culling,
/// projection and the collision region then all use the shifted box with no further changes, and the projection keeps
/// drawing the box centred in the widget and turning it around its own centre, as PROJECT.md asks.
/// </para>
/// <para>
/// Because <c>p</c> is a fraction of the height, it keeps working when #90's height slider or zoom changes the height.
/// Pure maths, allocation-free, unit tested without the game.
/// </para>
/// </remarks>
public static class PlayerHeight
{
    /// <summary>Default: the player is centred vertically, as before #91.</summary>
    public const float Default = 0.5f;

    /// <summary>Lowest value: the player is on the bottom face, so the box shows only what's above them.</summary>
    public const float Min = 0f;

    /// <summary>Highest value: the player is on the top face, so the box shows only what's below them.</summary>
    public const float Max = 1f;

    /// <summary>Clamps a fraction to 0–1; a non-number becomes <see cref="Default"/>.</summary>
    /// <param name="fraction">The requested player height, as a fraction of the box's height.</param>
    public static float Clamp(float fraction) => float.IsNaN(fraction) ? Default : Math.Clamp(fraction, Min, Max);

    /// <summary>
    /// The box's centre for a player position: the player's x and z, and a y shifted so the player sits at
    /// <paramref name="fraction"/> of the height from the bottom face.
    /// </summary>
    /// <remarks>
    /// The box spans centre ± height/2, so a centre of <c>player.y + height × (0.5 - p)</c> gives a bottom face at
    /// <c>player.y - height × p</c> and a top face at <c>player.y + height × (1 - p)</c> (PROJECT.md). At the default
    /// the shift is exactly zero, so nothing changes from the centred cube.
    /// </remarks>
    /// <param name="player">The local player's world position.</param>
    /// <param name="height">The box's height in yalms, already clamped (<see cref="CubeSpace.Size"/>.Y).</param>
    /// <param name="fraction">The player height setting; clamped first.</param>
    public static Vector3 BoxCentre(Vector3 player, float height, float fraction) =>
        new(player.X, player.Y + (height * (Default - Clamp(fraction))), player.Z);
}
