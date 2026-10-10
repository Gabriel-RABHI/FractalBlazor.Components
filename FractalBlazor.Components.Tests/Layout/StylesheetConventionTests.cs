using System.Text.RegularExpressions;

namespace FractalBlazor.Components.Tests.Layout;

/// <summary>
/// Layout must cohabit with other component libraries: every class and every
/// <c>@keyframes</c> declared by the stylesheets carries the <c>fb-</c> prefix.
/// </summary>
public sealed partial class StylesheetConventionTests
{
    public static TheoryData<string> Stylesheets => new()
    {
        "FractalBlazor.Components.Layout/wwwroot/fb-components-layout.css",
        "FractalBlazor.Components.Forms/wwwroot/fb-components-forms.css"
    };

    [Theory]
    [MemberData(nameof(Stylesheets))]
    public void Every_class_is_prefixed(string stylesheet)
    {
        var css = ReadWithoutCommentsAndUrls(stylesheet);

        var unprefixed = ClassSelector().Matches(css)
            .Select(m => m.Groups[1].Value)
            .Where(name => !name.StartsWith("fb-", StringComparison.Ordinal))
            .Distinct()
            .ToList();

        Assert.Empty(unprefixed);
    }

    [Theory]
    [MemberData(nameof(Stylesheets))]
    public void Every_keyframes_is_prefixed_and_defined(string stylesheet)
    {
        var css = ReadWithoutCommentsAndUrls(stylesheet);

        var declared = KeyframesDeclaration().Matches(css).Select(m => m.Groups[1].Value).ToHashSet();
        Assert.All(declared, name => Assert.StartsWith("fb-", name));

        var used = AnimationName().Matches(css).Select(m => m.Groups[1].Value);
        Assert.All(used, name => Assert.Contains(name, declared));
    }

    [Theory]
    [MemberData(nameof(Stylesheets))]
    public void No_literal_color_bypasses_the_theme(string stylesheet)
    {
        var css = ReadWithoutCommentsAndUrls(stylesheet);

        Assert.Empty(LiteralColor().Matches(css).Select(m => m.Value));
    }

    private static string ReadWithoutCommentsAndUrls(string relativePath)
    {
        var css = File.ReadAllText(Path.Combine(RepositoryRoot(), relativePath));
        css = Comment().Replace(css, "");
        return Url().Replace(css, "url()");
    }

    private static string RepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "FractalBlazor.Components.slnx")))
            directory = directory.Parent;
        return directory?.FullName ?? throw new DirectoryNotFoundException("Repository root not found.");
    }

    [GeneratedRegex(@"/\*.*?\*/", RegexOptions.Singleline)]
    private static partial Regex Comment();

    [GeneratedRegex(@"url\([^)]*\)")]
    private static partial Regex Url();

    [GeneratedRegex(@"(?<![\w-])\.([a-zA-Z][\w-]*)")]
    private static partial Regex ClassSelector();

    [GeneratedRegex(@"@keyframes\s+([\w-]+)")]
    private static partial Regex KeyframesDeclaration();

    [GeneratedRegex(@"#[0-9a-fA-F]{3,8}\b|\b(?:rgba?|hsla?|oklch|oklab|lab|lch)\(")]
    private static partial Regex LiteralColor();

    [GeneratedRegex(@"animation\s*:\s*([\w-]+)")]
    private static partial Regex AnimationName();
}
