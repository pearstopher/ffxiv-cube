namespace Cube.Core;

/// <summary>What a <see cref="DrawPrimitive"/> draws.</summary>
public enum PrimitiveKind : byte
{
    /// <summary>A line from <see cref="DrawPrimitive.A"/> to <see cref="DrawPrimitive.B"/>: an edge of the big wireframe cube.</summary>
    Line,

    /// <summary>A filled quadrilateral <see cref="DrawPrimitive.A"/>–<see cref="DrawPrimitive.D"/>: one visible face of a solid cube.</summary>
    Quad,
}
