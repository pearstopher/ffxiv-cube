namespace Cube.Core;

/// <summary>
/// Keeps the cost of the last few frames of the plugin's draw path: time taken and bytes allocated.
/// The in-game test bridge reads it to check the budgets in PROJECT.md ("Efficiency and memory"):
/// under 0.5 ms per frame on average and zero bytes allocated per frame in steady state.
/// </summary>
/// <remarks>
/// Samples go into fixed-size arrays allocated once, so recording a frame allocates nothing and the
/// memory used never grows (PROJECT.md: bounded memory). When full, the oldest sample is overwritten.
/// Times are kept as raw <see cref="System.Diagnostics.Stopwatch"/> ticks and converted only when read.
/// </remarks>
public sealed class DrawCostSampler
{
    /// <summary>Elapsed ticks per recorded frame, used as a ring buffer.</summary>
    private readonly long[] ticks;

    /// <summary>Bytes allocated per recorded frame, parallel to <see cref="ticks"/>.</summary>
    private readonly long[] bytes;

    /// <summary>Ticks per second of the clock the times were measured with.</summary>
    private readonly long ticksPerSecond;

    /// <summary>Where the next sample is written.</summary>
    private int next;

    /// <summary>Creates an empty sampler.</summary>
    /// <param name="capacity">How many frames to keep; the newest win.</param>
    /// <param name="ticksPerSecond">The clock's frequency, normally <see cref="System.Diagnostics.Stopwatch.Frequency"/>.</param>
    public DrawCostSampler(int capacity, long ticksPerSecond)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(capacity, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(ticksPerSecond, 1);
        this.ticks = new long[capacity];
        this.bytes = new long[capacity];
        this.ticksPerSecond = ticksPerSecond;
    }

    /// <summary>How many frames the sampler can hold.</summary>
    public int Capacity => this.ticks.Length;

    /// <summary>How many frames are currently held (at most <see cref="Capacity"/>).</summary>
    public int Count { get; private set; }

    /// <summary>
    /// How many frames were recorded since creation, never reset. A check compares it before and after
    /// to tell whether the window drew at all.
    /// </summary>
    public long TotalRecorded { get; private set; }

    /// <summary>Average time per held frame, in milliseconds; 0 when empty.</summary>
    public double AverageMilliseconds
    {
        get
        {
            if (this.Count == 0)
            {
                return 0;
            }

            long sum = 0;
            for (var i = 0; i < this.Count; i++)
            {
                sum += this.ticks[i];
            }

            return this.ToMilliseconds(sum) / this.Count;
        }
    }

    /// <summary>The slowest held frame, in milliseconds; 0 when empty.</summary>
    public double MaxMilliseconds
    {
        get
        {
            long max = 0;
            for (var i = 0; i < this.Count; i++)
            {
                max = Math.Max(max, this.ticks[i]);
            }

            return this.ToMilliseconds(max);
        }
    }

    /// <summary>Bytes allocated over all held frames.</summary>
    public long TotalAllocatedBytes
    {
        get
        {
            long sum = 0;
            for (var i = 0; i < this.Count; i++)
            {
                sum += this.bytes[i];
            }

            return sum;
        }
    }

    /// <summary>How many held frames allocated anything. The phase 1 budget is zero.</summary>
    public int FramesWithAllocations
    {
        get
        {
            var frames = 0;
            for (var i = 0; i < this.Count; i++)
            {
                if (this.bytes[i] != 0)
                {
                    frames++;
                }
            }

            return frames;
        }
    }

    /// <summary>Records one frame's cost. Allocation-free, so it can run inside the measured path's caller every frame.</summary>
    /// <param name="elapsedTicks">Time the draw path took, in clock ticks.</param>
    /// <param name="allocatedBytes">Bytes the draw path allocated on its thread.</param>
    public void Record(long elapsedTicks, long allocatedBytes)
    {
        this.ticks[this.next] = elapsedTicks;
        this.bytes[this.next] = allocatedBytes;
        this.next = (this.next + 1) % this.ticks.Length;
        this.Count = Math.Min(this.Count + 1, this.ticks.Length);
        this.TotalRecorded++;
    }

    /// <summary>
    /// Forgets the held frames (but not <see cref="TotalRecorded"/>), so a check can measure steady state
    /// after a warm-up period.
    /// </summary>
    public void Reset()
    {
        this.Count = 0;
        this.next = 0;
    }

    /// <summary>Converts clock ticks to milliseconds.</summary>
    /// <param name="value">A tick count.</param>
    private double ToMilliseconds(long value) => value * 1000.0 / this.ticksPerSecond;
}
