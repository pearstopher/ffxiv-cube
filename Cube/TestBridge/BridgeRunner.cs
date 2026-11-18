using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using Cube.Core;
using Dalamud.Plugin.Services;

namespace Cube.TestBridge;

/// <summary>
/// The Debug-only in-game test bridge (PROJECT.md, "Automated testing"): runs every registered
/// <see cref="BridgeCheck"/> on the framework thread when the plugin loads and whenever a request file
/// appears, and writes the results, stamped with the build time, for the test script to read.
/// </summary>
/// <remarks>
/// <para>
/// Files live in <c>pluginConfigs\Cube\test-bridge\</c> (see <c>docs/research-notes.md</c>, #7):
/// the script writes <c>request.json</c> (its text is echoed back as the request id), the bridge deletes it
/// when it starts and writes <c>results.json</c> atomically with <see cref="AtomicFile"/> when done.
/// </para>
/// <para>
/// When idle the per-frame cost is one bool read: a <see cref="FileSystemWatcher"/> sets
/// <see cref="requestSeen"/> from its own thread, so nothing polls the disk and nothing is allocated.
/// Allocations during a run are fine: the draw-path budget is measured inside
/// <see cref="CubeWindow.Draw"/> only, and the bridge runs in <see cref="IFramework.Update"/>.
/// </para>
/// </remarks>
internal sealed class BridgeRunner : IDisposable
{
    /// <summary>Name of the request file the script writes to ask for a run.</summary>
    public const string RequestFileName = "request.json";

    /// <summary>Name of the results file the bridge writes after each run.</summary>
    public const string ResultsFileName = "results.json";

    /// <summary>A request id starting with this asks for a soak run (<c>ingame_test.py --soak</c>).</summary>
    public const string SoakRequestPrefix = "soak-";

    /// <summary>The framework, whose update event drives the checks.</summary>
    private readonly IFramework framework;

    /// <summary>The plugin log; runs are summarised at Information, check details at Debug.</summary>
    private readonly IPluginLog log;

    /// <summary>What checks can see.</summary>
    private readonly BridgeContext context;

    /// <summary>The bridge's folder inside the plugin's config directory.</summary>
    private readonly string folder;

    /// <summary>Watches <see cref="folder"/> for the request file.</summary>
    private readonly FileSystemWatcher watcher;

    /// <summary>Registered checks, run in registration order.</summary>
    private readonly List<BridgeCheck> checks = [];

    /// <summary>Results of the run in progress.</summary>
    private readonly List<CheckResult> results = [];

    /// <summary>Set by the watcher thread when a request file appears; read each frame.</summary>
    private volatile bool requestSeen;

    /// <summary>True until the first run (the one on load) has started.</summary>
    private bool runOnLoad = true;

    /// <summary>Whether a run is in progress.</summary>
    private bool running;

    /// <summary>Index of the check being run.</summary>
    private int checkIndex;

    /// <summary>The running check's steps.</summary>
    private IEnumerator<CheckStep>? steps;

    /// <summary>When the running check started, as a <see cref="Stopwatch"/> timestamp.</summary>
    private long checkStarted;

    /// <summary>When the run started, local time.</summary>
    private DateTime runStarted;

    /// <summary>Why the run started: <c>load</c>, or the request file's text.</summary>
    private string request = "load";

    /// <summary>Creates the bridge, its folder and the request watcher, and hooks the framework update.</summary>
    /// <param name="framework">Dalamud's framework service.</param>
    /// <param name="log">The plugin log.</param>
    /// <param name="configDirectory">The plugin's config directory (<c>IDalamudPluginInterface.ConfigDirectory</c>).</param>
    /// <param name="context">What checks can see.</param>
    public BridgeRunner(IFramework framework, IPluginLog log, string configDirectory, BridgeContext context)
    {
        this.framework = framework;
        this.log = log;
        this.context = context;
        this.folder = Path.Combine(configDirectory, "test-bridge");
        Directory.CreateDirectory(this.folder);

        // A request left over from before this load is covered by the run on load.
        File.Delete(this.RequestPath);

        this.watcher = new FileSystemWatcher(this.folder, RequestFileName)
        {
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
        };
        this.watcher.Created += this.OnRequestFile;
        this.watcher.Changed += this.OnRequestFile;
        this.watcher.Renamed += this.OnRequestFile;
        this.watcher.EnableRaisingEvents = true;

        this.framework.Update += this.OnUpdate;
    }

    /// <summary>The bridge's folder, where checks may write their own output (e.g. the frame dump).</summary>
    public string Folder => this.folder;

    /// <summary>Full path of the request file.</summary>
    private string RequestPath => Path.Combine(this.folder, RequestFileName);

    /// <summary>Adds a check to every run. Call while setting the plugin up.</summary>
    /// <param name="check">The check.</param>
    public void Register(BridgeCheck check) => this.checks.Add(check);

    /// <summary>Unhooks the framework and stops watching. A run in progress is abandoned after cleaning up its check.</summary>
    public void Dispose()
    {
        this.framework.Update -= this.OnUpdate;
        this.watcher.EnableRaisingEvents = false;
        this.watcher.Dispose();
        if (this.running && this.checkIndex < this.checks.Count)
        {
            // Not running any more, so EndCheck cleans up without starting the next check.
            this.running = false;
            this.EndCheck(CheckState.Failed, "Plugin unloaded during the check.");
        }
    }

    /// <summary>Watcher callback (thread pool): only sets a flag; the framework thread does the work.</summary>
    /// <param name="sender">The watcher.</param>
    /// <param name="e">Which file changed.</param>
    private void OnRequestFile(object sender, FileSystemEventArgs e) => this.requestSeen = true;

    /// <summary>
    /// Runs once per frame on the framework thread. Idle: two bool reads. Running: advances the current check one step.
    /// Never lets an exception reach the game (CLAUDE.md, "Dalamud rules").
    /// </summary>
    /// <param name="fw">The framework.</param>
    private void OnUpdate(IFramework fw)
    {
        if (!this.running && !this.runOnLoad && !this.requestSeen)
        {
            return;
        }

        try
        {
            if (!this.running)
            {
                this.StartRun();
                return;
            }

            this.Step();
        }
        catch (Exception ex)
        {
            this.log.Error(ex, "Test bridge failed");
            this.running = false;
        }
    }

    /// <summary>Starts a run: reads and deletes the request, if any, and resets the results.</summary>
    private void StartRun()
    {
        if (this.runOnLoad)
        {
            this.runOnLoad = false;
            this.request = "load";
        }
        else
        {
            this.requestSeen = false;
            if (!File.Exists(this.RequestPath))
            {
                return;
            }

            this.request = File.ReadAllText(this.RequestPath).Trim();
            File.Delete(this.RequestPath);
        }

        this.log.Information($"Test bridge: running {this.checks.Count} checks (request: {this.request})");
        this.results.Clear();
        this.runStarted = DateTime.Now;
        this.checkIndex = -1;
        this.running = true;
        this.NextCheck();
    }

    /// <summary>
    /// Whether this run is a soak run: the request id starts with <see cref="SoakRequestPrefix"/>. Soak runs take
    /// minutes, so they run only the <see cref="BridgeCheck.SoakOnly"/> checks, and normal runs skip those.
    /// </summary>
    private bool IsSoakRun => this.request.StartsWith(SoakRequestPrefix, StringComparison.Ordinal);

    /// <summary>Moves to the next check that belongs in this kind of run, or finishes the run.</summary>
    private void NextCheck()
    {
        do
        {
            this.checkIndex++;
        }
        while (this.checkIndex < this.checks.Count && this.checks[this.checkIndex].SoakOnly != this.IsSoakRun);

        if (this.checkIndex >= this.checks.Count)
        {
            this.FinishRun();
            return;
        }

        this.context.Metrics.Clear();
        this.checkStarted = Stopwatch.GetTimestamp();
        this.steps = this.checks[this.checkIndex].Run(this.context).GetEnumerator();
    }

    /// <summary>Advances the current check one step, ending it on pass, fail, exception or timeout.</summary>
    private void Step()
    {
        var check = this.checks[this.checkIndex];
        if (Stopwatch.GetElapsedTime(this.checkStarted) > check.Timeout)
        {
            this.EndCheck(CheckState.Failed, $"Timed out after {check.Timeout.TotalSeconds:0.#} s.");
            return;
        }

        CheckStep step;
        try
        {
            step = this.steps!.MoveNext()
                ? this.steps.Current
                : CheckStep.Fail("The check ended without passing or failing.");
        }
        catch (Exception ex)
        {
            step = CheckStep.Fail($"Threw {ex.GetType().Name}: {ex.Message}");
        }

        if (step.State != CheckState.Running)
        {
            this.EndCheck(step.State, step.Message ?? string.Empty);
        }
    }

    /// <summary>Records the current check's result, runs its cleanup, and moves on.</summary>
    /// <param name="state">Passed or failed.</param>
    /// <param name="message">What was seen.</param>
    private void EndCheck(CheckState state, string message)
    {
        var check = this.checks[this.checkIndex];
        try
        {
            this.steps?.Dispose();
            check.Cleanup(this.context);
        }
        catch (Exception ex)
        {
            state = CheckState.Failed;
            message += $" Cleanup threw {ex.GetType().Name}: {ex.Message}";
        }

        this.steps = null;
        var elapsed = Stopwatch.GetElapsedTime(this.checkStarted);
        this.results.Add(new CheckResult(check.Name, state == CheckState.Passed, message, elapsed.TotalMilliseconds, new Dictionary<string, double>(this.context.Metrics)));
        this.log.Debug($"Test bridge: {check.Name} {(state == CheckState.Passed ? "passed" : "FAILED")}: {message}");

        if (this.running)
        {
            this.NextCheck();
        }
    }

    /// <summary>Writes the results file and logs a one-line summary.</summary>
    private void FinishRun()
    {
        this.running = false;
        var failed = 0;
        foreach (var result in this.results)
        {
            if (!result.Passed)
            {
                failed++;
            }
        }

        var path = Path.Combine(this.folder, ResultsFileName);
        AtomicFile.WriteAllText(path, this.ResultsJson(failed == 0));
        this.log.Information($"Test bridge: {this.results.Count - failed}/{this.results.Count} checks passed, results in {path}");
    }

    /// <summary>
    /// Builds the results document. The script matches <c>buildTime</c> against the DLL it built, and
    /// <c>request</c> against the request it wrote, to know the results are fresh.
    /// </summary>
    /// <param name="passed">Whether every check passed.</param>
    private string ResultsJson(bool passed)
    {
        using var stream = new MemoryStream();
        using (var json = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true }))
        {
            json.WriteStartObject();
            json.WriteString("buildTime", this.context.BuiltAt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
            json.WriteString("dllPath", this.context.DllPath);
            json.WriteString("request", this.request);
            json.WriteString("startedAt", this.runStarted.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
            json.WriteString("finishedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
            json.WriteBoolean("passed", passed);
            json.WriteStartArray("checks");
            foreach (var result in this.results)
            {
                json.WriteStartObject();
                json.WriteString("name", result.Name);
                json.WriteBoolean("passed", result.Passed);
                json.WriteString("message", result.Message);
                json.WriteNumber("durationMs", Math.Round(result.DurationMs, 1));
                json.WriteStartObject("metrics");
                foreach (var (name, value) in result.Metrics)
                {
                    // JSON has no NaN or infinity; write those as null rather than failing the whole results file.
                    if (double.IsFinite(value))
                    {
                        json.WriteNumber(name, Math.Round(value, 4));
                    }
                    else
                    {
                        json.WriteNull(name);
                    }
                }

                json.WriteEndObject();
                json.WriteEndObject();
            }

            json.WriteEndArray();
            json.WriteEndObject();
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    /// <summary>One check's outcome, as written to the results file.</summary>
    /// <param name="Name">The check's name.</param>
    /// <param name="Passed">Whether it passed.</param>
    /// <param name="Message">What was seen.</param>
    /// <param name="DurationMs">How long it ran, in milliseconds.</param>
    /// <param name="Metrics">Numbers it measured.</param>
    private sealed record CheckResult(string Name, bool Passed, string Message, double DurationMs, Dictionary<string, double> Metrics);
}
