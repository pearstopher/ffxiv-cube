using System.Text;

namespace Cube.Core;

/// <summary>
/// Writes a file so that readers only ever see the old or the complete new content, never a half-written one.
/// </summary>
/// <remarks>
/// The in-game test bridge writes its results while the test script may be polling for them
/// (PROJECT.md, "Automated testing"); a partial file would read as broken JSON. The content goes to a
/// temporary file next to the target first, then replaces the target in one move.
/// </remarks>
public static class AtomicFile
{
    /// <summary>The suffix of the temporary file written before the final move.</summary>
    public const string TemporarySuffix = ".tmp";

    /// <summary>Writes <paramref name="contents"/> to <paramref name="path"/> as UTF-8 (no BOM), replacing any existing file.</summary>
    /// <param name="path">The file to write.</param>
    /// <param name="contents">The full new contents.</param>
    public static void WriteAllText(string path, string contents)
    {
        var temporary = path + TemporarySuffix;
        File.WriteAllText(temporary, contents, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        File.Move(temporary, path, overwrite: true);
    }
}
