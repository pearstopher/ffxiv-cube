namespace Cube.Tests;

/// <summary>Locates files in the repository from a test's output folder, for tests that inspect the repo or build output.</summary>
internal static class TestPaths
{
    /// <summary>Finds the repository root (the folder holding <c>Cube.slnx</c>) above the test's output folder.</summary>
    public static string RepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Cube.slnx")))
        {
            dir = dir.Parent;
        }

        return dir?.FullName ?? throw new InvalidOperationException("Cube.slnx not found above the test output folder.");
    }
}
