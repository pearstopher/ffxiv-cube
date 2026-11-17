using System.Numerics;
using Dalamud.Interface.Windowing;

namespace Cube;

/// <summary>
/// The window the cube is drawn in (PROJECT.md, "Features": a Dalamud window toggled by <c>/cube</c>).
/// </summary>
/// <remarks>Empty for now; drawing arrives with the projection and draw list work.</remarks>
internal sealed class CubeWindow : Window
{
    /// <summary>Creates the window, closed, with a sensible minimum size.</summary>
    public CubeWindow()
        : base("Cube###CubeMain")
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(200, 200),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue),
        };
    }

    /// <summary>Draws the window's contents. Nothing yet.</summary>
    public override void Draw()
    {
    }
}
