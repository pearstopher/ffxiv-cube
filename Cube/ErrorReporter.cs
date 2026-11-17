using System;
using System.Diagnostics;
using Cube.Core;

namespace Cube;

/// <summary>
/// Logs exceptions caught in one game-facing handler, throttled with <see cref="ErrorThrottle"/> so a handler that
/// fails every frame logs about once every ten seconds (CLAUDE.md, "Dalamud rules": wrap every game-facing handler in
/// try/catch and log; never let an exception reach the game).
/// </summary>
/// <param name="where">What failed, for the log line, e.g. "Cube window".</param>
internal sealed class ErrorReporter(string where)
{
    /// <summary>The throttle, also read by the bridge: its <see cref="ErrorThrottle.Total"/> must stay 0.</summary>
    public ErrorThrottle Throttle { get; } = new(Stopwatch.Frequency * 10);

    /// <summary>What this reporter covers.</summary>
    public string Where { get; } = where;

    /// <summary>Counts the exception and logs it unless one was logged in the last ten seconds.</summary>
    /// <param name="ex">The exception caught.</param>
    public void Report(Exception ex)
    {
        if (this.Throttle.ShouldReport(Stopwatch.GetTimestamp(), out var skipped))
        {
            Plugin.Log.Error(ex, skipped > 0 ? $"{this.Where} failed ({skipped} more since the last report)" : $"{this.Where} failed");
        }
    }
}
