using System.Numerics;

namespace Cube.Core;

/// <summary>
/// The user's settings (PROJECT.md, "Features": cube size (width, height and depth since #90), player height in the cube (#91), orientation mode, labels on/off, colours and opacity; "NPCs":
/// NPCs on/off; "View orientation control": the saved view orientation and the control's corner; #32: collision on/off
/// and detail), with their defaults
/// and the rules for valid values. The plugin's saved configuration derives from this, so the rules are unit tested
/// without Dalamud.
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

    /// <summary>Default NPC colour: purple, so NPCs stand apart from players (PROJECT.md, "Colours").</summary>
    public static readonly Vector3 DefaultNpcColour = new(0.8f, 0.4f, 1f);

    /// <summary>Lowest opacity allowed, so the cubes can be made faint but never vanish entirely.</summary>
    public const float MinOpacity = 0.1f;

    /// <summary>
    /// The cube's width (X), height (Y) and depth (Z) in yalms (PROJECT.md, "Cube": each 10–100, default 40; #90).
    /// Change it with <see cref="SetSize"/> or <see cref="Zoom"/>, or bind a slider to a component and call
    /// <see cref="Changed"/>.
    /// </summary>
    public Vector3 CubeSize = BoxExtents.DefaultSize;

    /// <summary>
    /// The single edge files saved before <see cref="BoxSizeVersion"/> stored. Only read from those files, so their
    /// size carries over as a cube with that edge; never written again.
    /// </summary>
    public float CubeEdge = BoxExtents.Default;

    /// <summary>
    /// Where the player sits vertically in the box, as a fraction of its height from the bottom face (PROJECT.md,
    /// "Player height in cube", #91): 0–1, default 0.5 (centred). The settings window shows it as 0–100%. Files saved
    /// before <see cref="PlayerHeightVersion"/> load it as 0.5.
    /// </summary>
    public float PlayerHeight = Core.PlayerHeight.Default;

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

    /// <summary>
    /// Whether named NPCs are shown (PROJECT.md, "NPCs": a setting turns them on or off, default on). When off, the
    /// game reader doesn't even look at NPCs. A config file saved before this setting existed loads it as on.
    /// </summary>
    public bool ShowNpcs = true;

    /// <summary>Named NPCs' colour.</summary>
    public Vector3 NpcColour = DefaultNpcColour;

    /// <summary>
    /// Whether collision is shown (#32; PROJECT.md, "Collision (phase 2)"), default on. When off, the collision scene
    /// isn't read at all and the gathered boxes are dropped, like NPCs off. A config file saved before this setting
    /// existed loads it as on.
    /// </summary>
    public bool ShowCollision = true;

    /// <summary>How finely collision is drawn (#32); <see cref="CollisionDetail.Full"/> by default.</summary>
    public CollisionDetail CollisionDetail = CollisionDetail.Full;

    /// <summary>Opacity of everything drawn except labels (which stay readable), <see cref="MinOpacity"/>–1.</summary>
    public float Opacity = 1f;

    /// <summary>
    /// The settings file format that stores the view as <see cref="ViewRotation"/>. Files older than this stored the
    /// old control's heading and tilt instead, which <see cref="Upgrade"/> converts.
    /// </summary>
    public const int RotationViewVersion = 2;

    /// <summary>
    /// The settings file format that stores the cube as <see cref="CubeSize"/> (#90). Files older than this stored a
    /// single <see cref="CubeEdge"/>, which <see cref="Upgrade"/> turns into three equal extents.
    /// </summary>
    public const int BoxSizeVersion = 3;

    /// <summary>
    /// The settings file format that stores <see cref="PlayerHeight"/> (#91). Older files load it as centred (50%),
    /// as PROJECT.md asks.
    /// </summary>
    public const int PlayerHeightVersion = 4;

    /// <summary>
    /// The view control's rotation (PROJECT.md, "View orientation control": the chosen orientation is saved, any
    /// orientation including roll). Use <see cref="View"/> to read it.
    /// </summary>
    public Quaternion ViewRotation = ViewOrientation.Default.Rotation;

    /// <summary>
    /// The old yaw-and-tilt control's heading, in degrees (#63). Only read from files saved before
    /// <see cref="RotationViewVersion"/>, so their view carries over (PROJECT.md); never written again.
    /// </summary>
    public float ViewYawDegrees;

    /// <summary>The old control's tilt, in degrees; read like <see cref="ViewYawDegrees"/>. Defaults to 30° (the default view).</summary>
    public float ViewPitchDegrees = ViewOrientation.DefaultPitchDegrees;

    /// <summary>Which corner of the cube window the view control is drawn in (PROJECT.md: bottom right by default).</summary>
    public WidgetCorner ViewWidgetCorner = WidgetCorner.BottomRight;

    /// <summary>The saved view orientation.</summary>
    [System.Runtime.Serialization.IgnoreDataMember]
    public ViewOrientation View => new(this.ViewRotation);

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
        this.CubeSize = BoxExtents.Clamp(this.CubeSize);
        this.PlayerHeight = Core.PlayerHeight.Clamp(this.PlayerHeight);
        if (!Enum.IsDefined(this.Orientation))
        {
            this.Orientation = OrientationMode.Camera;
        }

        this.SelfColour = ValidColour(this.SelfColour, DefaultSelfColour);
        this.PartyColour = ValidColour(this.PartyColour, DefaultPartyColour);
        this.OtherColour = ValidColour(this.OtherColour, DefaultOtherColour);
        this.NpcColour = ValidColour(this.NpcColour, DefaultNpcColour);
        this.Opacity = float.IsNaN(this.Opacity) ? 1f : Math.Clamp(this.Opacity, MinOpacity, 1f);
        // ViewOrientation normalises the rotation and replaces an unusable one with the default.
        this.ViewRotation = this.View.Rotation;
        if (!Enum.IsDefined(this.ViewWidgetCorner))
        {
            this.ViewWidgetCorner = WidgetCorner.BottomRight;
        }

        if (!Enum.IsDefined(this.CollisionDetail))
        {
            this.CollisionDetail = CollisionDetail.Full;
        }

        this.Revision++;
    }

    /// <summary>Stores a new view orientation (from the view control) and marks the settings changed.</summary>
    /// <param name="view">The orientation the user chose.</param>
    public void SetView(in ViewOrientation view)
    {
        this.ViewRotation = view.Rotation;
        this.Changed();
    }

    /// <summary>Puts the view back to the default (PROJECT.md: "There's a way to reset to the default").</summary>
    public void ResetView()
    {
        this.ViewRotation = ViewOrientation.Default.Rotation;
        this.Changed();
    }

    /// <summary>Stores a new cube size (from the scale gizmo's axis handles), clamped, and marks the settings changed.</summary>
    /// <param name="size">The width, height and depth the user chose.</param>
    public void SetSize(Vector3 size)
    {
        this.CubeSize = size;
        this.Changed();
    }

    /// <summary>
    /// Zooms the cube (PROJECT.md, "Zoom": all three extents scale together, keeping their ratios, clamped so none
    /// leaves 10–100) and marks the settings changed.
    /// </summary>
    /// <param name="factor">How much to scale every extent by; below 1 zooms in (less space, contents look bigger).</param>
    public void Zoom(float factor)
    {
        this.CubeSize = BoxExtents.Zoom(this.CubeSize, factor);
        this.Changed();
    }

    /// <summary>Puts the cube back to its default size, 40 × 40 × 40 (the settings window's "Reset size").</summary>
    public void ResetSize()
    {
        this.CubeSize = BoxExtents.DefaultSize;
        this.Changed();
    }

    /// <summary>
    /// Brings settings loaded from an older file format up to date, then repairs them with <see cref="Changed"/>.
    /// Files before <see cref="RotationViewVersion"/> stored the view as a heading and tilt; they become the same
    /// orientation (PROJECT.md: orientations saved by the old control carry over). A file from before the view
    /// control existed has neither, so the old fields' defaults give the default view. Files before
    /// <see cref="BoxSizeVersion"/> stored one cube edge; it becomes all three extents (#90). Files before
    /// <see cref="PlayerHeightVersion"/> had no player height; it becomes 50%, the centred cube they showed (#91).
    /// </summary>
    /// <param name="fileVersion">The format version the settings were saved with.</param>
    public void Upgrade(int fileVersion)
    {
        if (fileVersion < RotationViewVersion)
        {
            var yaw = float.IsFinite(this.ViewYawDegrees) ? this.ViewYawDegrees : 0f;
            var pitch = float.IsFinite(this.ViewPitchDegrees) ? this.ViewPitchDegrees : ViewOrientation.DefaultPitchDegrees;
            this.ViewRotation = ViewOrientation.FromDegrees(yaw, pitch).Rotation;
        }

        if (fileVersion < BoxSizeVersion)
        {
            this.CubeSize = new Vector3(BoxExtents.Clamp(this.CubeEdge));
        }

        if (fileVersion < PlayerHeightVersion)
        {
            this.PlayerHeight = Core.PlayerHeight.Default;
        }

        this.Changed();
    }

    /// <summary>Never writes the old single edge, so saved files only hold <see cref="CubeSize"/>; see <see cref="ShouldSerializeViewYawDegrees"/>.</summary>
    /// <returns>Always false.</returns>
    public bool ShouldSerializeCubeEdge() => false;

    /// <summary>
    /// Tells the JSON serialiser Dalamud uses (Newtonsoft, by naming convention) never to write the old heading, so
    /// saved files only hold <see cref="ViewRotation"/>.
    /// </summary>
    /// <returns>Always false.</returns>
    public bool ShouldSerializeViewYawDegrees() => false;

    /// <summary>Never writes the old tilt; see <see cref="ShouldSerializeViewYawDegrees"/>.</summary>
    /// <returns>Always false.</returns>
    public bool ShouldSerializeViewPitchDegrees() => false;

    /// <summary>Copies every setting from another instance, then marks them changed. Used to undo test changes.</summary>
    /// <param name="other">The settings to copy.</param>
    public void CopyFrom(CubeSettings other)
    {
        this.CubeSize = other.CubeSize;
        this.PlayerHeight = other.PlayerHeight;
        this.Orientation = other.Orientation;
        this.ShowLabels = other.ShowLabels;
        this.SelfColour = other.SelfColour;
        this.PartyColour = other.PartyColour;
        this.OtherColour = other.OtherColour;
        this.ShowNpcs = other.ShowNpcs;
        this.NpcColour = other.NpcColour;
        this.ShowCollision = other.ShowCollision;
        this.CollisionDetail = other.CollisionDetail;
        this.Opacity = other.Opacity;
        this.ViewRotation = other.ViewRotation;
        this.ViewWidgetCorner = other.ViewWidgetCorner;
        this.Changed();
    }

    /// <summary>The player and NPC colours at the chosen opacity, packed for drawing.</summary>
    public PlayerPalette Palette() => new(
        Pack(this.SelfColour, this.Opacity),
        Pack(this.PartyColour, this.Opacity),
        Pack(this.OtherColour, this.Opacity),
        Pack(this.NpcColour, this.Opacity));

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
