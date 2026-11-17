namespace Cube.Core;

/// <summary>What a <see cref="DrawPrimitive"/> draws.</summary>
public enum PrimitiveKind : byte
{
    /// <summary>A line from <see cref="DrawPrimitive.A"/> to <see cref="DrawPrimitive.B"/>: an edge of the big wireframe cube.</summary>
    Line,

    /// <summary>A filled quadrilateral <see cref="DrawPrimitive.A"/>–<see cref="DrawPrimitive.D"/>: one visible face of a solid cube.</summary>
    Quad,

    /// <summary>
    /// A player's name label, anchored at <see cref="DrawPrimitive.A"/> (left end, vertically centred). The text is
    /// not stored here: it is the name of the player <see cref="DrawPrimitive.Tag"/> points to, so the draw list stays
    /// a plain struct array and names are never copied (PROJECT.md, "Labels"; "Efficiency and memory").
    /// </summary>
    Label,
}
