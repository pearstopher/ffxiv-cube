using Cube.Core;

namespace Cube.Tests;

/// <summary>
/// Checks the in-game bridge's results are written atomically (PROJECT.md, "Automated testing":
/// the script polls the results file, so it must never see half a file).
/// </summary>
public sealed class AtomicFileTests : IDisposable
{
    private readonly string folder = Directory.CreateTempSubdirectory("cube-atomic-").FullName;

    public void Dispose()
    {
        foreach (var file in Directory.EnumerateFiles(this.folder))
        {
            File.Delete(file);
        }

        Directory.Delete(this.folder);
    }

    [Fact]
    public void Writes_new_file()
    {
        var path = Path.Combine(this.folder, "results.json");

        AtomicFile.WriteAllText(path, "{}");

        Assert.Equal("{}", File.ReadAllText(path));
    }

    [Fact]
    public void Replaces_existing_file_and_leaves_no_temporary_file()
    {
        var path = Path.Combine(this.folder, "results.json");
        File.WriteAllText(path, "old");

        AtomicFile.WriteAllText(path, "new");

        Assert.Equal("new", File.ReadAllText(path));
        Assert.Equal([path], Directory.GetFiles(this.folder));
    }

    [Fact]
    public void Writes_utf8_without_a_byte_order_mark()
    {
        var path = Path.Combine(this.folder, "results.json");

        AtomicFile.WriteAllText(path, "é");

        Assert.Equal(new byte[] { 0xC3, 0xA9 }, File.ReadAllBytes(path));
    }
}
