namespace Cube.Core;

/// <summary>
/// Packs and shades colours in the 32-bit layout ImGui uses (<c>0xAABBGGRR</c>), without Core depending on ImGui.
/// </summary>
public static class Colour
{
    /// <summary>Packs red, green, blue and alpha (each 0–1, clamped) as <c>0xAABBGGRR</c>.</summary>
    /// <param name="r">Red.</param>
    /// <param name="g">Green.</param>
    /// <param name="b">Blue.</param>
    /// <param name="a">Alpha (opacity).</param>
    public static uint Pack(float r, float g, float b, float a) =>
        (ToByte(a) << 24) | (ToByte(b) << 16) | (ToByte(g) << 8) | ToByte(r);

    /// <summary>
    /// Darkens a colour's red, green and blue by a factor, keeping alpha. Side faces of a cube are drawn darker
    /// than the top so the cubes read as solid 3D shapes.
    /// </summary>
    /// <param name="colour">A packed colour.</param>
    /// <param name="factor">0 (black) to 1 (unchanged).</param>
    public static uint Shade(uint colour, float factor)
    {
        factor = Math.Clamp(factor, 0f, 1f);
        var r = (uint)((colour & 0xFF) * factor);
        var g = (uint)(((colour >> 8) & 0xFF) * factor);
        var b = (uint)(((colour >> 16) & 0xFF) * factor);
        return (colour & 0xFF000000) | (b << 16) | (g << 8) | r;
    }

    /// <summary>Converts 0–1 to a byte, clamped and rounded.</summary>
    /// <param name="value">A channel value.</param>
    private static uint ToByte(float value) => (uint)MathF.Round(Math.Clamp(value, 0f, 1f) * 255f);
}
