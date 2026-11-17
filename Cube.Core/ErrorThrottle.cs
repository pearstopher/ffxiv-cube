namespace Cube.Core;

/// <summary>
/// Decides when a repeated error is worth logging. A handler that fails every frame would otherwise write 60 log
/// lines a second and bury everything else (CLAUDE.md, "Dalamud rules": catch and log, never let an exception reach
/// the game).
/// </summary>
/// <remarks>
/// The first error is always reported; after that at most one per <see cref="Interval"/>, and each report says how
/// many were skipped. Works on <see cref="System.Diagnostics.Stopwatch"/> timestamps passed in, so it is tested
/// without waiting. No allocation.
/// </remarks>
/// <param name="interval">Smallest time between reports, in timestamp ticks.</param>
public sealed class ErrorThrottle(long interval)
{
    /// <summary>When the last report was allowed; null before the first error.</summary>
    private long? lastReport;

    /// <summary>Smallest time between reports, in timestamp ticks.</summary>
    public long Interval { get; } = interval;

    /// <summary>Every error seen, reported or not. The bridge checks it stays 0.</summary>
    public long Total { get; private set; }

    /// <summary>Errors since the last report that weren't reported.</summary>
    public long Skipped { get; private set; }

    /// <summary>
    /// Counts an error and says whether to log it. When it returns true, <paramref name="skippedBefore"/> is how many
    /// errors were swallowed since the previous report.
    /// </summary>
    /// <param name="now">The current timestamp.</param>
    /// <param name="skippedBefore">Errors not reported since the last report.</param>
    public bool ShouldReport(long now, out long skippedBefore)
    {
        this.Total++;
        if (this.lastReport is { } last && now - last < this.Interval)
        {
            this.Skipped++;
            skippedBefore = 0;
            return false;
        }

        skippedBefore = this.Skipped;
        this.Skipped = 0;
        this.lastReport = now;
        return true;
    }
}
