using Cube.Core;

namespace Cube.Tests;

/// <summary>
/// Checks when the cube is hidden (PROJECT.md, "When hidden": not logged in, during loading screens, cutscenes and
/// GPose; shown otherwise).
/// </summary>
public class VisibilityTests
{
    [Fact]
    public void Normal_play_shows_the_cube()
    {
        Assert.Equal(Visibility.Shown, VisibilityRules.For(GameState.Playing));
    }

    [Fact]
    public void Logged_out_hides_the_cube()
    {
        Assert.Equal(Visibility.NotLoggedIn, VisibilityRules.For(GameState.Playing with { LoggedIn = false }));
    }

    [Fact]
    public void Logged_in_without_a_character_yet_hides_the_cube()
    {
        Assert.Equal(Visibility.NotLoggedIn, VisibilityRules.For(GameState.Playing with { HasLocalPlayer = false }));
    }

    [Fact]
    public void Loading_screen_hides_the_cube()
    {
        Assert.Equal(Visibility.Loading, VisibilityRules.For(GameState.Playing with { BetweenAreas = true }));
    }

    [Fact]
    public void Cutscene_hides_the_cube()
    {
        Assert.Equal(Visibility.Cutscene, VisibilityRules.For(GameState.Playing with { InCutscene = true }));
    }

    [Fact]
    public void GPose_hides_the_cube()
    {
        Assert.Equal(Visibility.GPose, VisibilityRules.For(GameState.Playing with { InGPose = true }));
    }

    [Fact]
    public void Several_reasons_at_once_still_hide_the_cube()
    {
        var state = new GameState(LoggedIn: true, HasLocalPlayer: true, BetweenAreas: true, InCutscene: true, InGPose: true);

        Assert.NotEqual(Visibility.Shown, VisibilityRules.For(state));
    }
}
