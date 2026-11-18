using System.Numerics;

namespace Cube.Core;

/// <summary>
/// The rules for the cube's size (PROJECT.md, "Exact behaviour rules", Cube; changed 2026-11-18, #90): the cube is a
/// box with three extents in yalms, width (x), height (y) and depth (z), each 10–100, default 40. Zoom scales all three
/// together, keeping their ratios.
/// </summary>
/// <remarks>
/// <para>
/// A size is a <see cref="Vector3"/> of full extents in the cube's own axes: X across (towards the right face), Y up,
/// Z front to back. Keeping it a plain vector lets the settings window bind a slider to each component and the JSON
/// file store it like the colours.
/// </para>
/// <para>
/// Every reader goes through <see cref="Clamp(Vector3)"/>, so a corrupt or hand-edited value can never produce an empty
/// or infinite box. Pure maths, allocation-free, unit tested without the game.
/// </para>
/// </remarks>
public static class BoxExtents
{
    /// <summary>Default extent on every axis, in yalms (PROJECT.md: default 40).</summary>
    public const float Default = 40f;

    /// <summary>Smallest allowed extent, in yalms (PROJECT.md: 10–100).</summary>
    public const float Min = 10f;

    /// <summary>Largest allowed extent, in yalms (PROJECT.md: 10–100).</summary>
    public const float Max = 100f;

    /// <summary>
    /// How much one press of the settings window's zoom buttons scales the cube: "Zoom out" multiplies every extent by
    /// it, "Zoom in" divides by it.
    /// </summary>
    public const float ZoomStep = 1.25f;

    /// <summary>The default size: a 40 × 40 × 40 cube (PROJECT.md; "reset to default gives 40×40×40").</summary>
    public static Vector3 DefaultSize => new(Default);

    /// <summary>
    /// Clamps one extent to <see cref="Min"/>–<see cref="Max"/>; a non-number becomes <see cref="Default"/>.
    /// </summary>
    /// <param name="extent">The requested extent in yalms.</param>
    public static float Clamp(float extent) => float.IsNaN(extent) ? Default : Math.Clamp(extent, Min, Max);

    /// <summary>Clamps each extent of a size separately (see <see cref="Clamp(float)"/>).</summary>
    /// <param name="size">The requested width, height and depth.</param>
    public static Vector3 Clamp(Vector3 size) => new(Clamp(size.X), Clamp(size.Y), Clamp(size.Z));

    /// <summary>The largest of a size's three extents; marker sizes and the on-screen fit follow it.</summary>
    /// <param name="size">A size, already clamped.</param>
    public static float Largest(Vector3 size) => MathF.Max(size.X, MathF.Max(size.Y, size.Z));

    /// <summary>
    /// Zooms a size: multiplies all three extents by the same factor, so the box keeps its proportions (PROJECT.md:
    /// "Zoom scales all three extents together, keeping their ratios"). A factor below 1 zooms in (less space, so the
    /// contents look bigger).
    /// </summary>
    /// <remarks>
    /// The factor itself is clamped, not each extent, so no extent leaves 10–100 and the ratios still hold: zooming
    /// out a 20 × 80 × 40 box by 2 stops at 25 × 100 × 50. A factor that isn't a positive number leaves the size as
    /// it is.
    /// </remarks>
    /// <param name="size">The current size; clamped first.</param>
    /// <param name="factor">How much to scale every extent by.</param>
    public static Vector3 Zoom(Vector3 size, float factor)
    {
        size = Clamp(size);
        if (!(factor > 0f) || !float.IsFinite(factor))
        {
            return size;
        }

        var smallest = MathF.Min(size.X, MathF.Min(size.Y, size.Z));
        factor = Math.Clamp(factor, Min / smallest, Max / Largest(size));

        // Rounding can land a hair outside the range at a limit; the final clamp only trims that.
        return Clamp(size * factor);
    }
}
