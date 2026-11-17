namespace Cube.Core;

/// <summary>Whether the cube is drawn, and if not, why (PROJECT.md, "When hidden"). Decided by <see cref="VisibilityRules"/>.</summary>
public enum Visibility
{
    /// <summary>Drawn normally.</summary>
    Shown = 0,

    /// <summary>Not logged in, or no character yet: nothing to centre the cube on.</summary>
    NotLoggedIn,

    /// <summary>A loading screen is up.</summary>
    Loading,

    /// <summary>A cutscene is playing.</summary>
    Cutscene,

    /// <summary>Group pose is active.</summary>
    GPose,
}
