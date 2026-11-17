namespace Cube.TestBridge;

/// <summary>Whether an in-game check is still going, or how it ended.</summary>
internal enum CheckState
{
    /// <summary>Not finished: run the check again next frame.</summary>
    Running,

    /// <summary>The behaviour matched the spec.</summary>
    Passed,

    /// <summary>The behaviour didn't match, or the check couldn't run.</summary>
    Failed,
}
