using System;
using System.Numerics;
using Cube.Core;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;

namespace Cube;

/// <summary>
/// The settings window, opened with <c>/cube config</c> or Dalamud's settings button (PROJECT.md, "Features": cube
/// size, orientation mode, labels on/off, colours and opacity).
/// </summary>
/// <remarks>
/// Every control edits <see cref="Configuration"/> directly, so changes show in the cube on the next frame. The file
/// is written when an edit is finished (slider or colour picker released, box ticked), not on every frame of a drag.
/// Not part of the per-frame cube path: it only draws while open.
/// </remarks>
internal sealed class SettingsWindow : Window
{
    /// <summary>The settings being edited.</summary>
    private readonly Configuration config;

    /// <summary>Writes the settings to disk.</summary>
    private readonly Action save;

    /// <summary>Creates the window, closed.</summary>
    /// <param name="config">The settings to edit.</param>
    /// <param name="save">Saves them.</param>
    public SettingsWindow(Configuration config, Action save)
        : base("Cube settings###CubeSettings", ImGuiWindowFlags.AlwaysAutoResize)
    {
        this.config = config;
        this.save = save;
    }

#if DEBUG
    /// <summary>How many times the window has drawn; the bridge checks it draws without errors. Debug builds only.</summary>
    internal long DrawCount { get; private set; }
#endif

    /// <summary>Exceptions caught while drawing; the bridge checks there were none.</summary>
    internal ErrorReporter Errors { get; } = new("Settings window");

    /// <summary>Draws the controls, catching and logging any exception so it never reaches the game.</summary>
    public override void Draw()
    {
#if DEBUG
        this.DrawCount++;
#endif
        try
        {
            this.DrawControls();
        }
        catch (Exception ex)
        {
            this.Errors.Report(ex);
        }
    }

    /// <summary>Draws the controls and applies and saves changes.</summary>
    private void DrawControls()
    {
        var c = this.config;

        ImGui.SetNextItemWidth(200);
        this.Track(ImGui.SliderFloat("Cube size (yalms)", ref c.CubeEdge, CubeSpace.MinEdge, CubeSpace.MaxEdge, "%.0f"));

        ImGui.TextUnformatted("Orientation");
        ImGui.SameLine();
        var mode = (int)c.Orientation;
        var modeChanged = ImGui.RadioButton("Camera", ref mode, (int)OrientationMode.Camera);
        ImGui.SameLine();
        modeChanged |= ImGui.RadioButton("North up", ref mode, (int)OrientationMode.NorthUp);
        if (modeChanged)
        {
            c.Orientation = (OrientationMode)mode;
            this.ApplyAndSave();
        }

        if (ImGui.Checkbox("Show names", ref c.ShowLabels))
        {
            this.ApplyAndSave();
        }

        this.Track(ImGui.ColorEdit3("You", ref c.SelfColour, ImGuiColorEditFlags.NoInputs));
        ImGui.SameLine();
        this.Track(ImGui.ColorEdit3("Party / alliance", ref c.PartyColour, ImGuiColorEditFlags.NoInputs));
        ImGui.SameLine();
        this.Track(ImGui.ColorEdit3("Others", ref c.OtherColour, ImGuiColorEditFlags.NoInputs));

        ImGui.SetNextItemWidth(200);
        this.Track(ImGui.SliderFloat("Opacity", ref c.Opacity, CubeSettings.MinOpacity, 1f, "%.2f"));

        if (ImGui.Button("Reset to defaults"))
        {
            c.CopyFrom(new CubeSettings());
            this.save();
        }
    }

    /// <summary>
    /// For controls that are dragged: applies every change at once, saves when the edit is finished.
    /// </summary>
    /// <param name="changed">Whether the control changed its value this frame.</param>
    private void Track(bool changed)
    {
        if (changed)
        {
            this.config.Changed();
        }

        if (ImGui.IsItemDeactivatedAfterEdit())
        {
            this.save();
        }
    }

    /// <summary>For one-click controls: applies and saves.</summary>
    private void ApplyAndSave()
    {
        this.config.Changed();
        this.save();
    }
}
