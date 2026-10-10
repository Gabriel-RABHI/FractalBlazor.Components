using System.Text.RegularExpressions;

namespace FractalBlazor.Components.Tests.Layout;

/// <summary>
/// Every JS function called from C# lives under the global <c>FractalBlazor</c> object,
/// and exists in the Layout script.
/// </summary>
public sealed partial class InteropConventionTests
{
    [Fact]
    public void Every_function_called_from_csharp_is_defined()
    {
        var root = RepositoryRoot();
        var script = File.ReadAllText(Path.Combine(root, "FractalBlazor.Components.Layout/wwwroot/fb-components-layout.js"));
        var defined = Definition().Matches(script).Select(m => m.Groups[1].Value).ToHashSet();

        var called = Directory.EnumerateFiles(Path.Combine(root, "FractalBlazor.Components.Layout"), "*.cs", SearchOption.AllDirectories)
            .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}"))
            .SelectMany(path => Call().Matches(File.ReadAllText(path)).Select(m => m.Groups[1].Value))
            .ToHashSet();

        Assert.NotEmpty(called);
        Assert.All(called, name => Assert.Contains(name, defined));
    }

    [Fact]
    public void No_function_is_declared_in_the_global_scope()
    {
        var script = File.ReadAllText(Path.Combine(RepositoryRoot(), "FractalBlazor.Components.Layout/wwwroot/fb-components-layout.js"));

        Assert.Empty(GlobalDeclaration().Matches(script));
    }

    private static string RepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "FractalBlazor.Components.slnx")))
            directory = directory.Parent;
        return directory?.FullName ?? throw new DirectoryNotFoundException("Repository root not found.");
    }

    [GeneratedRegex(@"\bfb\.(\w+)\s*=\s*function")]
    private static partial Regex Definition();

    // "FractalBlazor.name" in an interop call, or $"FractalBlazor.name(…)" in an inline handler.
    [GeneratedRegex(@"(?:""|\$"")FractalBlazor\.(\w+)")]
    private static partial Regex Call();

    [GeneratedRegex(@"^(?:function\s+\w+|let\s+\w+|var\s+\w+|const\s+\w+)", RegexOptions.Multiline)]
    private static partial Regex GlobalDeclaration();
}
