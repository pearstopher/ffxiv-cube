using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using Cube.Core;

namespace Cube.TestBridge.Checks;

/// <summary>
/// Checks the player height setting (#91; PROJECT.md, "Player height in cube"): it changes which fake players above or
/// below the player are inside, moves the player's drawn marker off the middle of the box, shifts the collision region,
/// and resetting to defaults brings back the centred cube.
/// </summary>
/// <remarks>
/// <para>
/// Two fakes are fixed in the world 15 yalms above and below the player. With the default 40-yalm height and the
/// player centred both are inside; at 0% (player on the bottom face) only the one above is, at 100% only the one below.
/// The default view looks from above and behind, so the box's up axis points up the screen: at 0% the player's marker
/// must be drawn below the box's projected centre, at 100% above it, and at 50% on it.
/// </para>
/// <para>
/// The collision part runs only if collision is gathered here: at 0% the next gather's region must be centred half the
/// height above the player. The settings, fakes and window state are changed in memory only and restored in
/// <see cref="Cleanup"/>.
/// </para>
/// </remarks>
internal sealed class PlayerHeightCheck : BridgeCheck
{
    /// <summary>How far above and below the player the two fakes are, in yalms: inside a centred 40 box, outside the other half.</summary>
    private const float FakeOffset = 15f;

    /// <summary>How far the marker must move off the box's centre on screen at 0% or 100%, in pixels.</summary>
    private const float MinMarkerShift = 5f;

    /// <summary>How close the marker must be to the box's centre on screen at 50%, in pixels.</summary>
    private const float CentredTolerance = 0.5f;

    /// <summary>How far the collision region's centre may be from where it should be, in yalms (the player moving).</summary>
    private const float RegionTolerance = 1f;

    /// <summary>The user's settings before the check, restored by <see cref="Cleanup"/>.</summary>
    private readonly CubeSettings saved = new();

    /// <summary>The window's open state before the check, restored by <see cref="Cleanup"/>.</summary>
    private bool wasOpen;

    /// <inheritdoc/>
    public override string Name => "player-height-in-cube";

    /// <inheritdoc/>
    /// <remarks>Room for a collision gather at background frame rates (about 10 fps).</remarks>
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
        foreach (var step in WaitForDraws(window))
        {
            yield return step;
        }

        var player = window.LastSpace.Player;
        window.Fakes.Clear();
        window.Fakes.AddAtWorld("Fake Above", player + new Vector3(0, FakeOffset, 0), PlayerRelation.Other);
        window.Fakes.AddAtWorld("Fake Below", player - new Vector3(0, FakeOffset, 0), PlayerRelation.Other);

        var results = new Sample[3];
        var fractions = new[] { PlayerHeight.Default, PlayerHeight.Min, PlayerHeight.Max };
        for (var i = 0; i < fractions.Length; i++)
        {
            settings.PlayerHeight = fractions[i];
            settings.Changed();
            foreach (var step in WaitForDraws(window))
            {
                yield return step;
            }

            results[i] = Measure(window);
        }

        // Collision: at 0% the next gather is centred half the height above the player.
        var regionOffset = float.NaN;
        var regionWant = float.NaN;
        if (settings.ShowCollision && window.Collision.Cache.Ready.HasData)
        {
            settings.PlayerHeight = PlayerHeight.Min;
            settings.Changed();
            var cache = window.Collision.Cache;
            var completed = cache.Completed;
            while (cache.Completed == completed || cache.IsGathering)
            {
                yield return CheckStep.NextFrame;
            }

            regionOffset = cache.Ready.Region.Centre.Y - window.LastSpace.Player.Y;
            regionWant = window.LastSpace.Size.Y / 2f;
            context.Metric("regionOffset", regionOffset);
        }

        // Reset to defaults restores 50%.
        settings.CopyFrom(new CubeSettings());
        foreach (var step in WaitForDraws(window))
        {
            yield return step;
        }

        var reset = Measure(window);
        var centred = results[0];
        var bottom = results[1];
        var top = results[2];
        context.Metric("centredMarkerShift", centred.MarkerShift);
        context.Metric("bottomMarkerShift", bottom.MarkerShift);
        context.Metric("topMarkerShift", top.MarkerShift);

        if (!centred.Above || !centred.Below || !bottom.Above || bottom.Below || top.Above || !top.Below)
        {
            yield return CheckStep.Fail($"Fakes 15 yalms above/below should be in/in at 50%, in/out at 0%, out/in at 100%; were {centred.Above}/{centred.Below}, {bottom.Above}/{bottom.Below}, {top.Above}/{top.Below}.");
        }
        else if (!centred.HasMarker || !bottom.HasMarker || !top.HasMarker)
        {
            yield return CheckStep.Fail("The player's own marker wasn't drawn in every frame.");
        }
        else if (MathF.Abs(centred.MarkerShift) > CentredTolerance || bottom.MarkerShift < MinMarkerShift || top.MarkerShift > -MinMarkerShift)
        {
            yield return CheckStep.Fail(string.Create(
                CultureInfo.InvariantCulture,
                $"The player's marker should be on the box's centre at 50%, below it at 0%, above it at 100%; screen-down shifts were {centred.MarkerShift:0.0}, {bottom.MarkerShift:0.0}, {top.MarkerShift:0.0} px."));
        }
        else if (!float.IsNaN(regionOffset) && MathF.Abs(regionOffset - regionWant) > RegionTolerance)
        {
            yield return CheckStep.Fail(string.Create(
                CultureInfo.InvariantCulture,
                $"At 0% the collision region was centred {regionOffset:0.0} yalms above the player, not {regionWant:0.0}."));
        }
        else if (settings.PlayerHeight != PlayerHeight.Default || !reset.Above || !reset.Below || MathF.Abs(reset.MarkerShift) > CentredTolerance)
        {
            yield return CheckStep.Fail($"Reset to defaults gave player height {settings.PlayerHeight}, fakes {reset.Above}/{reset.Below}, marker shift {reset.MarkerShift:0.0} px; want 50%, both inside, centred.");
        }
        else
        {
            var collision = float.IsNaN(regionOffset)
                ? "collision not gathered here, region part skipped"
                : string.Create(CultureInfo.InvariantCulture, $"collision region {regionOffset:0.0} yalms above the player at 0%");
            yield return CheckStep.Pass(string.Create(
                CultureInfo.InvariantCulture,
                $"Fakes above/below followed the setting; marker shift {centred.MarkerShift:0.0} / {bottom.MarkerShift:0.0} / {top.MarkerShift:0.0} px at 50/0/100%; {collision}; reset restored 50%."));
        }
    }

    /// <inheritdoc/>
    public override void Cleanup(BridgeContext context)
    {
        context.Window.Fakes.Clear();
        context.Window.Settings.CopyFrom(this.saved);
        context.Window.IsOpen = this.wasOpen;
    }

    /// <summary>
    /// Reads the last frame: whether each fake is drawn, whether the player's own marker is, and how far the player's
    /// projected position is below the box's projected centre on screen (screen Y grows downwards, so positive means
    /// below). The marker is drawn around that projected position.
    /// </summary>
    /// <param name="window">The cube window.</param>
    private static Sample Measure(CubeWindow window)
    {
        var space = window.LastSpace;
        var centre = window.LastProjection.ToScreen(Vector3.Zero);
        var marker = window.LastProjection.ToScreen(space.ToView(space.Player));
        var players = window.Players.Items;
        var drawn = false;
        for (var i = 0; i < players.Length && !drawn; i++)
        {
            if (players[i].Relation != PlayerRelation.Self)
            {
                continue;
            }

            foreach (var p in window.DrawList.Items)
            {
                if (p.Kind == PrimitiveKind.Quad && p.Tag == i + 1)
                {
                    drawn = true;
                    break;
                }
            }
        }

        return new Sample(OrientationCheck.HasFake(window, 0), OrientationCheck.HasFake(window, 1), drawn, marker.Y - centre.Y);
    }

    /// <summary>One frame's measurements.</summary>
    /// <param name="Above">Whether the fake above the player was drawn.</param>
    /// <param name="Below">Whether the fake below the player was drawn.</param>
    /// <param name="HasMarker">Whether the player's own marker was drawn.</param>
    /// <param name="MarkerShift">How far the player's projected position is below the box's centre on screen, in pixels.</param>
    private readonly record struct Sample(bool Above, bool Below, bool HasMarker, float MarkerShift);
}
