using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace Cube.Tests;

/// <summary>
/// Checks the in-game test bridge is compiled out of Release builds (CLAUDE.md, "Building and testing":
/// the bridge and fake-player code must not ship), and is present in Debug builds where the in-game checks need it.
/// </summary>
/// <remarks>
/// Reads the plugin DLL built with the same configuration as this test, using only its metadata, so neither
/// the plugin nor Dalamud is loaded. CI runs the tests in Release, so the Release case is checked on every push.
/// </remarks>
public class ReleaseBuildTests
{
#if DEBUG
    private const string Configuration = "Debug";
#else
    private const string Configuration = "Release";
#endif

    [Fact]
    public void Test_bridge_is_only_in_debug_builds()
    {
        var namespaces = PluginNamespaces();

#if DEBUG
        Assert.Contains("Cube.TestBridge", namespaces);
#else
        Assert.DoesNotContain(namespaces, ns => ns.StartsWith("Cube.TestBridge", StringComparison.Ordinal));
#endif
    }

    /// <summary>Returns the namespaces of all types in the plugin DLL of this test's configuration.</summary>
    private static HashSet<string> PluginNamespaces()
    {
        var dll = Path.Combine(TestPaths.RepoRoot(), "Cube", "bin", "x64", Configuration, "Cube.dll");
        Assert.True(File.Exists(dll), $"Plugin not built at {dll}; build the solution in {Configuration} first.");

        using var stream = File.OpenRead(dll);
        using var pe = new PEReader(stream);
        var metadata = pe.GetMetadataReader();
        var namespaces = new HashSet<string>(StringComparer.Ordinal);
        foreach (var handle in metadata.TypeDefinitions)
        {
            namespaces.Add(metadata.GetString(metadata.GetTypeDefinition(handle).Namespace));
        }

        return namespaces;
    }
}
