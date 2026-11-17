using System.Numerics;

namespace Cube.Core;

/// <summary>
/// The user's settings (PROJECT.md, "Features": cube size, orientation mode, labels on/off, colours and opacity),
/// with their defaults and the rules for valid values. The plugin's saved configuration derives from this, so the
/// rules are unit tested without Dalamud.
/// </summary>
/// <remarks>
/// Public fields, not properties, so the settings window can bind ImGui controls to them by reference. After any
/// change, call <see cref="Changed"/>: it repairs bad values and bumps <see cref="Revision"/>, which tells the window
/// to rebuild its cached colours (PROJECT.md, "Efficiency and memory": recompute only when inputs change).
/// </remarks>
public class CubeSettings
{
    /// <summary>Default player colours: blue for self, green for party/alliance, orange for others.</summary>
    public static readonly Vector3 DefaultSelfColour = new(0.25f, 0.6f, 1f);

    /// <summary>Default party and alliance colour.</summary>
    public static readonly Vector3 DefaultPartyColour = new(0.3f, 0.95f, 0.4f);

    /// <summary>Default colour for everyone else.</summary>
    public static readonly Vector3 DefaultOtherColour = new(1f, 0.6f, 0.2f);

    /// <summary>Lowest opacity allowed, so the cubes can be made faint but never vanish entirely.</summary>
    public const float MinOpacity = 0.1f;

    /// <summary>The cube's edge in yalms (PROJECT.md: 10–100, default 40).</summary>
    public float CubeEdge = CubeSpace.DefaultEdge;

    /// <summary>How the view turns (PROJECT.md, "View": Camera by default).</summary>
    public OrientationMode Orientation = OrientationMode.Camera;

    /// <summary>Whether other players' names are drawn (PROJECT.md: labels on/off).</summary>
    public bool ShowLabels = true;

    /// <summary>The local player's colour (red, green, blue, each 0–1).</summary>
    public Vector3 SelfColour = DefaultSelfColour;

    /// <summary>Party and alliance members' colour.</summary>
    public Vector3 PartyColour = DefaultPartyColour;

    /// <summary>Everyone else's colour.</summary>
    public Vector3 OtherColour = DefaultOtherColour;

    /// <summary>Opacity of everything drawn except labels (which stay readable), <see cref="MinOpacity"/>–1.</summary>
    public float Opacity = 1f;

    /// <summary>
    /// Goes up by one on every <see cref="Changed"/>; readers compare it with the revision they last built from.
    /// Not saved (<see cref="System.Runtime.Serialization.IgnoreDataMemberAttribute"/> is honoured by the JSON
    /// serialiser Dalamud uses for configurations, without Core referencing it).
    /// </summary>
    [System.Runtime.Serialization.IgnoreDataMember]
    public int Revision { get; private set; }

    /// <summary>
    /// Repairs out-of-range or corrupt values (e.g. from a hand-edited config file) and marks the settings as
    /// changed. A bad file can never produce a broken cube or an invisible one.
    /// </summary>
    public void Changed()
    {
        this.CubeEdge = CubeSpace.ClampEdge(this.CubeEdge);
        if (!Enum.IsDefined(this.Orientation))
        {
            this.Orientation = OrientationMode.Camera;
        }

        this.SelfColour = ValidColour(this.SelfColour, DefaultSelfColour);
        this.PartyColour = ValidColour(this.PartyColour, DefaultPartyColour);
        this.OtherColour = ValidColour(this.OtherColour, DefaultOtherColour);
        this.Opacity = float.IsNaN(this.Opacity) ? 1f : Math.Clamp(this.Opacity, MinOpacity, 1f);
        this.Revision++;
    }

    /// <summary>Copies every setting from another instance, then marks them changed. Used to undo test changes.</summary>
    /// <param name="other">The settings to copy.</param>
    public void CopyFrom(CubeSettings other)
    {
        this.CubeEdge = other.CubeEdge;
        this.Orientation = other.Orientation;
        this.ShowLabels = other.ShowLabels;
        this.SelfColour = other.SelfColour;
        this.PartyColour = other.PartyColour;
        this.OtherColour = other.OtherColour;
        this.Opacity = other.Opacity;
        this.Changed();
    }

    /// <summary>The player colours at the chosen opacity, packed for drawing.</summary>
    public PlayerPalette Palette() => new(
        Pack(this.SelfColour, this.Opacity),
        Pack(this.PartyColour, this.Opacity),
        Pack(this.OtherColour, this.Opacity));

    /// <summary>A colour at the chosen opacity, packed for drawing; used for the wireframe.</summary>
    /// <param name="rgb">Red, green and blue, 0–1.</param>
    /// <param name="alpha">The colour's own opacity before the setting is applied.</param>
    public uint WithOpacity(Vector3 rgb, float alpha) => Colour.Pack(rgb.X, rgb.Y, rgb.Z, alpha * this.Opacity);

    /// <summary>Packs an RGB colour with an alpha.</summary>
    /// <param name="rgb">Red, green and blue.</param>
    /// <param name="alpha">Opacity.</param>
    private static uint Pack(Vector3 rgb, float alpha) => Colour.Pack(rgb.X, rgb.Y, rgb.Z, alpha);

    /// <summary>Clamps each channel to 0–1; a colour with a non-number channel is replaced by the default.</summary>
    /// <param name="colour">The stored colour.</param>
    /// <param name="fallback">The default to use if it's corrupt.</param>
    private static Vector3 ValidColour(Vector3 colour, Vector3 fallback) =>
        float.IsNaN(colour.X) || float.IsNaN(colour.Y) || float.IsNaN(colour.Z)
            ? fallback
            : Vector3.Clamp(colour, Vector3.Zero, Vector3.One);
}
