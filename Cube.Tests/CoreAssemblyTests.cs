using Cube.Core;

namespace Cube.Tests;

/// <summary>
/// Checks that Core stays free of Dalamud, game structs and Lumina, so all its logic can be unit tested
/// without the game (CLAUDE.md, "Reference: the previous plugin" layout rule).
/// </summary>
public class CoreAssemblyTests
{
    // Core must stay testable without the game: no Dalamud, game structs or Lumina.
    [Theory]
    [InlineData("Dalamud")]
    [InlineData("FFXIVClientStructs")]
    [InlineData("Lumina")]
    [InlineData("InteropGenerator")]
    public void Core_does_not_reference_game_assemblies(string forbiddenPrefix)
    {
        var references = typeof(CoreAssembly).Assembly.GetReferencedAssemblies();

        Assert.DoesNotContain(references, r => r.Name!.StartsWith(forbiddenPrefix, StringComparison.Ordinal));
    }
}
