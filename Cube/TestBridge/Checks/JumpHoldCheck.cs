using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using Cube.Core;

namespace Cube.TestBridge.Checks;

/// <summary>
/// Checks "Jump" (#105; PROJECT.md, "Exact behaviour rules", Cube): with a simulated jump flag and a scripted height for
/// the local player (no game input, so no automation), the cube's height stays at the take-off height, a thing fixed in
/// the world doesn't move vertically in the cube or on screen, the player's marker rises and falls, x and z still
/// follow, the marker is kept at the top face when the jump leaves the box, and landing on a higher spot snaps the
/// cube to the new height in the first frame.
/// </summary>
/// <remarks>
/// The real jump flag can't be tested here (the bridge can't make the character jump); that is the owner's manual
/// session in <c>docs/manual-tests.md</c> ("Jump"). Everything changed here is in memory and restored in
/// <see cref="Cleanup"/>.
/// </remarks>
internal sealed class JumpHoldCheck : BridgeCheck
{
    /// <summary>The scripted rise and fall above take-off, in yalms.</summary>
    private static readonly float[] Path = [1f, 3f, 5f, 3f, 1f];

    /// <summary>How far the held cube's centre or a fixed thing's view height may drift, in yalms (rounding only).</summary>
    private const float Tolerance = 0.01f;

    /// <summary>The user's settings before the check, restored by <see cref="Cleanup"/>.</summary>
    private readonly CubeSettings saved = new();

    /// <summary>The window's open state before the check, restored by <see cref="Cleanup"/>.</summary>
    private bool wasOpen;

    /// <inheritdoc/>
    public override string Name => "jump-holds-cube-height";

    /// <inheritdoc/>
    public override TimeSpan Timeout => TimeSpan.FromSeconds(30);

    /// <inheritdoc/>
    public override IEnumerable<CheckStep> Run(BridgeContext context)
    {
        var window = context.Window;
        var settings = window.Settings;
        this.saved.CopyFrom(settings);
        this.wasOpen = window.IsOpen;
        window.IsOpen = true;
        settings.CopyFrom(new CubeSettings());
        settings.ShowCollision = false;
        settings.Changed();

        foreach (var step in WaitForDraws(window))
        {
            yield return step;
        }

        // Stand on the ground at the player's real height, with something fixed in the world just beside them.
        var ground = window.LastSpace.Player;
        window.SimulatedJump = false;
        window.SimulatedPlayerY = ground.Y;
        var fixedThing = ground + new Vector3(3f, 2f, 0f);
        window.Fakes.Clear();
        window.Fakes.AddAtWorld("Fake Fixed", fixedThing, PlayerRelation.Other);
        foreach (var step in WaitForDraws(window))
        {
            yield return step;
        }

        var standing = Measure(window, fixedThing);
        window.SimulatedJump = true;

        var maxCentreDrift = 0f;
        var maxThingDrift = 0f;
        var maxThingScreenDrift = 0f;
        var markerRise = 0f;
        var markerScreenRise = 0f;
        var thingDrawn = true;
        foreach (var rise in Path)
        {
            window.SimulatedPlayerY = ground.Y + rise;
            foreach (var step in WaitForDraws(window))
            {
                yield return step;
            }

            var now = Measure(window, fixedThing);
            thingDrawn &= now.ThingDrawn;
            maxCentreDrift = MathF.Max(maxCentreDrift, MathF.Abs(now.CentreY - standing.CentreY));
            maxThingDrift = MathF.Max(maxThingDrift, MathF.Abs(now.ThingViewY - standing.ThingViewY));
            maxThingScreenDrift = MathF.Max(maxThingScreenDrift, MathF.Abs(now.ThingScreenY - standing.ThingScreenY));
            markerRise = MathF.Max(markerRise, now.MarkerViewY - standing.MarkerViewY);
            markerScreenRise = MathF.Max(markerScreenRise, MathF.Abs(now.MarkerScreenY - standing.MarkerScreenY));
        }

        // A jump that leaves the box: the marker is kept at the top face, not dropped.
        window.SimulatedPlayerY = ground.Y + (window.LastSpace.Size.Y * 2f);
        foreach (var step in WaitForDraws(window))
        {
            yield return step;
        }

        var high = Measure(window, fixedThing);
        var topFace = window.LastSpace.Half.Y;

        // Landing on a ledge 4 yalms higher: the flag clears and the cube follows straight away.
        window.SimulatedJump = false;
        window.SimulatedPlayerY = ground.Y + 4f;
        foreach (var step in WaitForDraws(window))
        {
            yield return step;
        }

        var landed = Measure(window, fixedThing);
        context.Metric("centreDrift", maxCentreDrift);
        context.Metric("thingScreenDrift", maxThingScreenDrift);
        context.Metric("markerRise", markerRise);

        if (!thingDrawn || !standing.ThingDrawn || !standing.HasMarker || !high.HasMarker)
        {
            yield return CheckStep.Fail($"The fixed thing or the player's marker wasn't drawn in every frame (thing {thingDrawn}, marker at the top {high.HasMarker}).");
        }
        else if (maxCentreDrift > Tolerance || maxThingDrift > Tolerance || maxThingScreenDrift > 0.5f)
        {
            yield return CheckStep.Fail(string.Create(
                CultureInfo.InvariantCulture,
                $"During the jump the cube moved: centre drift {maxCentreDrift:0.000} yalms, fixed thing {maxThingDrift:0.000} yalms / {maxThingScreenDrift:0.0} px."));
        }
        else if (MathF.Abs(markerRise - Path[2]) > Tolerance || markerScreenRise < 5f)
        {
            yield return CheckStep.Fail(string.Create(
                CultureInfo.InvariantCulture,
                $"The player's marker should rise {Path[2]:0.0} yalms inside the cube (and move on screen); rose {markerRise:0.00} yalms, {markerScreenRise:0.0} px."));
        }
        else if (high.MarkerViewY < topFace * 0.99f || high.MarkerViewY > topFace)
        {
            yield return CheckStep.Fail(string.Create(
                CultureInfo.InvariantCulture,
                $"A jump out of the box should keep the marker at the top face ({topFace:0.00}); it was at {high.MarkerViewY:0.00}."));
        }
        else if (MathF.Abs(landed.CentreY - (ground.Y + 4f)) > Tolerance || window.Anchor.IsHeld)
        {
            yield return CheckStep.Fail(string.Create(
                CultureInfo.InvariantCulture,
                $"Landing 4 yalms higher should move the cube there in the first frame; its centre was {landed.CentreY - ground.Y:0.00} above take-off, held {window.Anchor.IsHeld}."));
        }
        else
        {
            yield return CheckStep.Pass(string.Create(
                CultureInfo.InvariantCulture,
                $"Cube held through the jump (drift {maxCentreDrift:0.000} yalms, fixed thing {maxThingScreenDrift:0.0} px); marker rose {markerRise:0.0} yalms ({markerScreenRise:0.0} px), held at the top face when out of the box; landing 4 higher snapped the cube."));
        }
    }

    /// <inheritdoc/>
    public override void Cleanup(BridgeContext context)
    {
        var window = context.Window;
        window.SimulatedJump = null;
        window.SimulatedPlayerY = null;
        window.Fakes.Clear();
        window.Settings.CopyFrom(this.saved);
        window.IsOpen = this.wasOpen;
    }

    /// <summary>
    /// Reads the last frame: the cube's centre height, the fixed thing's height in the cube and on screen, whether it
    /// was drawn, and the player's own marker's height in the cube and on screen.
    /// </summary>
    /// <param name="window">The cube window.</param>
    /// <param name="thing">The fixed thing's world position.</param>
    private static Sample Measure(CubeWindow window, Vector3 thing)
    {
        var space = window.LastSpace;
        var projection = window.LastProjection;
        var players = window.Players.Items;
        var hasMarker = false;
        var markerView = 0f;
        var markerScreen = 0f;
        var thingDrawn = false;
        for (var i = 0; i < players.Length; i++)
        {
            if (players[i].Relation == PlayerRelation.Self)
            {
                var view = space.ToView(players[i].Position);
                hasMarker = true;
                markerView = view.Y;
                markerScreen = projection.ToScreen(view).Y;
            }
            else
            {
                thingDrawn = true;
            }
        }

        var thingView = space.ToView(thing);
        return new Sample(space.Centre.Y, thingView.Y, projection.ToScreen(thingView).Y, thingDrawn, hasMarker, markerView, markerScreen);
    }

    /// <summary>One frame's measurements.</summary>
    /// <param name="CentreY">The cube's centre height in the world.</param>
    /// <param name="ThingViewY">The fixed thing's height in the cube.</param>
    /// <param name="ThingScreenY">The fixed thing's projected screen y, in pixels.</param>
    /// <param name="ThingDrawn">Whether the fixed thing is in the frame's players.</param>
    /// <param name="HasMarker">Whether the player's own marker is in the frame's players.</param>
    /// <param name="MarkerViewY">The marker's height in the cube.</param>
    /// <param name="MarkerScreenY">The marker's projected screen y, in pixels.</param>
    private readonly record struct Sample(float CentreY, float ThingViewY, float ThingScreenY, bool ThingDrawn, bool HasMarker, float MarkerViewY, float MarkerScreenY);
}
