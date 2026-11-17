using System.Reflection;
using System.Xml.Linq;

namespace Cube.Tests;

/// <summary>
/// Checks the documentation rules (CLAUDE.md, "Code is the documentation"): every test class has a
/// summary saying which rule it checks, and every namespace has an overview page for the generated docs. Test methods are named as behaviour and need no summary,
/// so the compiler doesn't enforce this for the test project; this test does.
/// </summary>
public class DocumentationCoverageTests
{
    [Fact]
    public void Every_test_class_has_a_summary()
    {
        var assembly = typeof(DocumentationCoverageTests).Assembly;
        var xmlPath = Path.ChangeExtension(assembly.Location, ".xml");
        Assert.True(File.Exists(xmlPath), $"No XML documentation file at {xmlPath}; is GenerateDocumentationFile on?");

        var documented = XDocument.Load(xmlPath)
            .Descendants("member")
            .Where(m => m.Element("summary") is { } s && !string.IsNullOrWhiteSpace(s.Value))
            .Select(m => (string)m.Attribute("name")!)
            .ToHashSet();

        var testClasses = assembly.GetTypes()
            .Where(t => t.IsClass && t.IsPublic)
            .Where(t => t.GetMethods().Any(m => m.GetCustomAttributes<FactAttribute>().Any()));

        var missing = testClasses.Where(t => !documented.Contains($"T:{t.FullName}")).Select(t => t.Name).ToList();

        Assert.True(missing.Count == 0, $"Test classes without a <summary>: {string.Join(", ", missing)}");
    }

    [Fact]
    public void Every_namespace_has_an_overview_page()
    {
        var root = TestPaths.RepoRoot();
        var namespaces = new[] { "Cube", "Cube.Core", "Cube.Snapshot", "Cube.Tests" }
            .SelectMany(project => Directory.EnumerateFiles(Path.Combine(root, project), "*.cs", SearchOption.AllDirectories))
            .Where(file => !file.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .SelectMany(File.ReadLines)
            .Where(line => line.StartsWith("namespace ", StringComparison.Ordinal))
            .Select(line => line["namespace ".Length..].TrimEnd(';', ' ', '{'))
            .ToHashSet();

        var missing = namespaces
            .Where(ns => !File.Exists(Path.Combine(root, "docs", "namespaces", $"{ns}.md")))
            .Order()
            .ToList();

        Assert.True(missing.Count == 0, $"Namespaces without docs/namespaces/<name>.md: {string.Join(", ", missing)}");
    }
}
