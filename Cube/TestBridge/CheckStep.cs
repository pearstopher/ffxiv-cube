namespace Cube.TestBridge;

/// <summary>
/// One step of an in-game check. A check is written as an iterator that yields <see cref="NextFrame"/>
/// to wait a frame and ends with <see cref="Pass"/> or <see cref="Fail"/>, so a check that needs
/// several frames (open the window, wait for it to draw, close it) reads top to bottom.
/// </summary>
/// <param name="State">Running, passed or failed.</param>
/// <param name="Message">What was seen; required when finished, so the results explain themselves.</param>
internal readonly record struct CheckStep(CheckState State, string? Message)
{
    /// <summary>Wait for the next frame, then continue the check.</summary>
    public static CheckStep NextFrame => new(CheckState.Running, null);

    /// <summary>Ends the check as passed.</summary>
    /// <param name="message">What was seen, e.g. the measured value.</param>
    public static CheckStep Pass(string message) => new(CheckState.Passed, message);

    /// <summary>Ends the check as failed.</summary>
    /// <param name="message">What was expected and what was seen instead.</param>
    public static CheckStep Fail(string message) => new(CheckState.Failed, message);
}
