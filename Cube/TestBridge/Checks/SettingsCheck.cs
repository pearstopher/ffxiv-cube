using System.Collections.Generic;
using System.Numerics;
using Cube.Core;

namespace Cube.TestBridge.Checks;

/// <summary>
/// Checks each setting changes the drawn frame straight away, and that putting the settings back restores it
/// (PROJECT.md, "Features": settings for cube size, orientation mode, labels on/off, colours and opacity; "NPCs": NPCs
/// on/off and the NPC colour).
/// </summary>
/// <remarks>
/// Changes the settings in memory only, never saving them, and restores the user's values in
/// <see cref="Cleanup"/> even if a step fails. Uses the standard fakes (#16, #74): the colour step changes the
/// "others" colour, the NPC steps change the NPC colour and then turn NPCs off.
/// </remarks>
internal sealed class SettingsCheck : BridgeCheck
{
    /// <summary>A colour no default uses, so a face in it can only come from the changed setting.</summary>
    private static readonly Vector3 TestColour = new(1f, 0f, 1f);

    /// <summary>A second unused colour for the NPC colour step, different from <see cref="TestColour"/>.</summary>
    private static readonly Vector3 NpcTestColour = new(0f, 1f, 1f);

    /// <summary>The user's settings before the check.</summary>
    private readonly CubeSettings saved = new();

    /// <summary>The window's open state before the check.</summary>
    private bool wasOpen;

    /// <inheritdoc/>
    public override string Name => "settings-change-the-frame";

    /// <inheritdoc/>
    public override System.TimeSpan Timeout => System.TimeSpan.FromSeconds(20);

    /// <inheritdoc/>
    public override IEnumerable<CheckStep> Run(BridgeContext context)
    {
        var window = context.Window;
        var settings = window.Settings;
        this.saved.CopyFrom(settings);
        this.wasOpen = window.IsOpen;
        window.IsOpen = true;
        window.Fakes.UseStandardSet();

        // Start from the defaults so the expectations below don't depend on the user's choices.
        settings.CopyFrom(new CubeSettings());
        foreach (var step in WaitForDraws(window))
        {
            yield return step;
        }

        var labelsBefore = CountLabels(window);
        if (labelsBefore == 0)
        {
            yield return CheckStep.Fail("No labels with default settings; can't test the labels setting.");
            yield break;
        }

        // Cube size (#90: width, height and depth separately).
        var wantSize = new Vector3(80, 30, 60);
        settings.CubeSize = wantSize;
        settings.Changed();
        foreach (var step in WaitForDraws(window))
        {
            yield return step;
        }

        if (window.LastSpace.Size != wantSize)
        {
            yield return CheckStep.Fail($"Cube size {wantSize} drew a box of {window.LastSpace.Size} yalms.");
            yield break;
        }

        // Labels off.
        settings.ResetSize();
        settings.ShowLabels = false;
        settings.ShowOwnName = false;
        settings.Changed();
        foreach (var step in WaitForDraws(window))
        {
            yield return step;
        }

        if (CountLabels(window) != 0)
        {
            yield return CheckStep.Fail("Labels still drawn with labels turned off.");
            yield break;
        }

        // Colour and opacity together: other players' faces must be shades of the test colour at half opacity.
        settings.ShowLabels = true;
        settings.ShowOwnName = true;
        settings.OtherColour = TestColour;
        settings.Opacity = 0.5f;
        settings.Changed();
        foreach (var step in WaitForDraws(window))
        {
            yield return step;
        }

        var colourProblem = CheckColours(window, Colour.Pack(TestColour.X, TestColour.Y, TestColour.Z, 0.5f), out var others);
        if (colourProblem is not null)
        {
            yield return CheckStep.Fail(colourProblem);
            yield break;
        }

        // Orientation.
        settings.Orientation = OrientationMode.NorthUp;
        settings.Changed();
        foreach (var step in WaitForDraws(window))
        {
            yield return step;
        }

        if (window.LastSpace.Yaw != ViewYaw.NorthUp)
        {
            yield return CheckStep.Fail($"North up drew yaw {window.LastSpace.Yaw}.");
            yield break;
        }

        // NPC colour (#74): the fake NPC's faces must be shades of the new NPC colour.
        settings.Orientation = OrientationMode.Camera;
        settings.Opacity = 1f;
        settings.NpcColour = NpcTestColour;
        settings.Changed();
        foreach (var step in WaitForDraws(window))
        {
            yield return step;
        }

        var npcFaces = CountFaces(window, PlayerRelation.Npc, Colour.Pack(NpcTestColour.X, NpcTestColour.Y, NpcTestColour.Z, 1f), out var wrongNpcFace);
        if (wrongNpcFace is not null || npcFaces == 0)
        {
            yield return CheckStep.Fail(wrongNpcFace ?? "No NPC faces drawn to check the NPC colour on.");
            yield break;
        }

        // NPCs off: none in the set, so none drawn or labelled; players stay.
        settings.ShowNpcs = false;
        settings.Changed();
        foreach (var step in WaitForDraws(window))
        {
            yield return step;
        }

        if (window.Players.NpcCount != 0 || CountFaces(window, PlayerRelation.Npc, 0, out _) != 0)
        {
            yield return CheckStep.Fail($"NPCs turned off but {window.Players.NpcCount} NPCs are still in the cube.");
            yield break;
        }

        if (window.Players.Count < FakePlayers.StandardArriving(false))
        {
            yield return CheckStep.Fail($"Turning NPCs off also removed players: {window.Players.Count} left.");
            yield break;
        }

        // Back to the defaults: the frame returns to how it started.
        settings.CopyFrom(new CubeSettings());
        foreach (var step in WaitForDraws(window))
        {
            yield return step;
        }

        var labelsAfter = CountLabels(window);
        context.Metric("labels", labelsAfter);
        context.Metric("otherFacesChecked", others);
        context.Metric("npcFacesChecked", npcFaces);
        yield return labelsAfter == labelsBefore && window.Palette == PlayerPalette.Default && window.LastSpace.Size == BoxExtents.DefaultSize
            && window.Players.NpcCount >= FakePlayers.StandardNpcsInside
            ? CheckStep.Pass($"Size, labels, colour, opacity, orientation, NPC colour and NPCs on/off each changed the next frame; defaults restored it ({labelsAfter} labels).")
            : CheckStep.Fail("Restoring the defaults didn't restore the frame.");
    }

    /// <inheritdoc/>
    public override void Cleanup(BridgeContext context)
    {
        context.Window.Settings.CopyFrom(this.saved);
        context.Window.Fakes.Clear();
        context.Window.IsOpen = this.wasOpen;
    }

    /// <summary>
    /// Checks other players' faces are shades of the wanted colour (alpha included) and the edges carry the
    /// opacity too.
    /// </summary>
    /// <param name="window">The cube window.</param>
    /// <param name="want">The packed colour other players should have.</param>
    /// <param name="others">How many other players' faces were checked.</param>
    /// <returns>What was wrong, or null if all is well.</returns>
    private static string? CheckColours(CubeWindow window, uint want, out int others)
    {
        others = 0;
        var edgeAlpha = Colour.Pack(0, 0, 0, 0.8f * 0.5f) >> 24;
        var players = window.Players.Items;
        foreach (var p in window.DrawList.Items)
        {
            if (p.Kind == PrimitiveKind.Quad && p.Tag > 0 && players[p.Tag - 1].Relation == PlayerRelation.Other)
            {
                if (p.Colour >> 24 != want >> 24 || !IsShadeOf(p.Colour, want))
                {
                    return $"An other player's face is {p.Colour:X8}; expected a shade of {want:X8}.";
                }

                others++;
            }
            else if (p.Kind == PrimitiveKind.Line && p.Colour >> 24 != edgeAlpha)
            {
                return $"Edge alpha is {p.Colour >> 24}; expected {edgeAlpha} (the edges' 0.8 times opacity 0.5).";
            }
        }

        return others == 0 ? "No other players' faces to check the colour on." : null;
    }

    /// <summary>Counts the faces drawn for one relation and checks each is a shade of the wanted colour.</summary>
    /// <param name="window">The cube window.</param>
    /// <param name="relation">Whose faces to count.</param>
    /// <param name="want">The packed colour they should be shades of; 0 to only count.</param>
    /// <param name="problem">What was wrong with a face, or null.</param>
    /// <returns>How many faces that relation has.</returns>
    private static int CountFaces(CubeWindow window, PlayerRelation relation, uint want, out string? problem)
    {
        problem = null;
        var count = 0;
        var players = window.Players.Items;
        foreach (var p in window.DrawList.Items)
        {
            if (p.Kind != PrimitiveKind.Quad || p.Tag <= 0 || players[p.Tag - 1].Relation != relation)
            {
                continue;
            }

            if (want != 0 && !IsShadeOf(p.Colour, want))
            {
                problem ??= $"A {relation} face is {p.Colour:X8}; expected a shade of {want:X8}.";
            }

            count++;
        }

        return count;
    }

    /// <summary>Counts label primitives in the last frame.</summary>
    /// <param name="window">The cube window.</param>
    private static int CountLabels(CubeWindow window)
    {
        var count = 0;
        foreach (var p in window.DrawList.Items)
        {
            if (p.Kind == PrimitiveKind.Label)
            {
                count++;
            }
        }

        return count;
    }

    /// <summary>Whether a face colour is one of the cube shades of a base colour.</summary>
    /// <param name="face">The face's colour.</param>
    /// <param name="baseColour">The player's colour.</param>
    private static bool IsShadeOf(uint face, uint baseColour) =>
        face == Colour.Shade(baseColour, CubeGeometry.TopShade)
        || face == Colour.Shade(baseColour, CubeGeometry.NorthSouthShade)
        || face == Colour.Shade(baseColour, CubeGeometry.EastWestShade)
        || face == Colour.Shade(baseColour, CubeGeometry.BottomShade);
}
