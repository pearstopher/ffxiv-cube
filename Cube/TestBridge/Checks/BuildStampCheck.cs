using System.Collections.Generic;
using System.Globalization;
using Cube.Core;

namespace Cube.TestBridge.Checks;

/// <summary>
/// Checks the plugin that ran the checks is the build on disk: the build time read at load equals the
/// DLL's current write time. If not, the game is running a stale build and every other result is
/// about old code (CLAUDE.md: the startup build line makes "my change didn't take effect" obvious).
/// </summary>
internal sealed class BuildStampCheck : BridgeCheck
{
    /// <inheritdoc/>
    public override string Name => "loaded-build-is-current";

    /// <inheritdoc/>
    public override IEnumerable<CheckStep> Run(BridgeContext context)
    {
        var onDisk = BuildInfo.ReadBuildTime(context.DllPath);
        var loaded = context.BuiltAt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        var current = onDisk.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

        yield return onDisk == context.BuiltAt
            ? CheckStep.Pass($"Loaded build {loaded} is the DLL on disk.")
            : CheckStep.Fail($"Loaded build {loaded}, but the DLL on disk was written {current}: stale build.");
    }
}
