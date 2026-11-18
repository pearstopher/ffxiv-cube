using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Cube.Tests;

/// <summary>
/// Checks the plugin version in <c>Cube/Cube.csproj</c> is releasable (CLAUDE.md, "CI and releases"): it must be
/// <c>MAJOR.MINOR.PATCH.0</c> so the release workflow can match it to a <c>vMAJOR.MINOR.PATCH</c> tag, and it must
/// never be older than the version <c>repo.json</c> already publishes, or Dalamud would not offer the next release
/// as an update.
/// </summary>
/// <remarks>
/// <c>repo.json</c> only exists after the first release (the release workflow writes it), so the check against the
/// real file passes until then; the comparison itself is tested with fixed documents so it is never untested.
/// </remarks>
public class ReleaseVersionTests
{
    [Fact]
    public void Csproj_version_matches_the_tag_format()
    {
        Assert.Matches(new Regex(@"^\d+\.\d+\.\d+\.0$"), CsprojVersion());
    }

    [Fact]
    public void Csproj_version_is_not_older_than_the_published_version()
    {
        var repoJson = Path.Combine(TestPaths.RepoRoot(), "repo.json");
        if (!File.Exists(repoJson))
        {
            return; // Nothing published yet.
        }

        var csproj = Version.Parse(CsprojVersion());
        var published = PublishedVersion(File.ReadAllText(repoJson));
        Assert.True(
            IsNotOlder(csproj, published),
            $"Cube.csproj says {csproj}, but repo.json already publishes {published}. Bump <Version> in the csproj.");
    }

    [Theory]
    [InlineData("1.0.0.0", "1.0.0.0", true)]
    [InlineData("1.0.1.0", "1.0.0.0", true)]
    [InlineData("1.1.0.0", "1.0.9.0", true)]
    [InlineData("1.0.0.0", "1.0.1.0", false)]
    [InlineData("0.9.0.0", "1.0.0.0", false)]
    public void Older_csproj_version_than_published_fails(string csproj, string published, bool ok)
    {
        var repoJson = $$"""[ { "InternalName": "Cube", "AssemblyVersion": "{{published}}" } ]""";
        Assert.Equal(ok, IsNotOlder(Version.Parse(csproj), PublishedVersion(repoJson)));
    }

    /// <summary>True if the csproj version may be released over the published one (same or newer).</summary>
    /// <param name="csproj">The version in the plugin csproj.</param>
    /// <param name="published">The version repo.json publishes.</param>
    private static bool IsNotOlder(Version csproj, Version published) => csproj >= published;

    /// <summary>Reads <c>&lt;Version&gt;</c> from the plugin csproj.</summary>
    private static string CsprojVersion()
    {
        var csproj = XDocument.Load(Path.Combine(TestPaths.RepoRoot(), "Cube", "Cube.csproj"));
        return csproj.Descendants("Version").Single().Value.Trim();
    }

    /// <summary>Reads the published <c>AssemblyVersion</c> from a repo.json document (a one-entry list).</summary>
    /// <param name="repoJson">The repo.json text.</param>
    private static Version PublishedVersion(string repoJson)
    {
        using var repo = JsonDocument.Parse(repoJson);
        return Version.Parse(repo.RootElement[0].GetProperty("AssemblyVersion").GetString()!);
    }
}
