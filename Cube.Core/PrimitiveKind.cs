namespace Cube.Core;

/// <summary>What a <see cref="DrawPrimitive"/> draws.</summary>
public enum PrimitiveKind : byte
{
    /// <summary>A line from <see cref="DrawPrimitive.A"/> to <see cref="DrawPrimitive.B"/>: an edge of the big wireframe cube.</summary>
    Line,

    /// <summary>A filled quadrilateral <see cref="DrawPrimitive.A"/>–<see cref="DrawPrimitive.D"/>: one visible face of a solid cube.</summary>
    Quad,

    /// <summary>
    /// A player's name label, anchored at <see cref="DrawPrimitive.A"/> (left end, vertically centred; or, when
    /// <see cref="DrawPrimitive.B"/>.X is 1 (#112), the middle of the text's bottom edge). The text is
    /// not stored here: it is the name of the player <see cref="DrawPrimitive.Tag"/> points to, so the draw list stays
    /// a plain struct array and names are never copied (PROJECT.md, "Labels"; "Efficiency and memory").
    /// </summary>
    Label,

    /// <summary>
    /// A filled convex polygon of 3 to <see cref="ClippedBox.MaxCorners"/> corners, stored in the draw list's point
    /// buffer (<see cref="DrawList.Points"/>): the top or bottom of a collision box clipped to a turned cube (#93),
    /// whose footprint isn't a quad. One ImGui call, so a see-through face shows no seams where quads would meet.
    /// </summary>
    Polygon,
}
