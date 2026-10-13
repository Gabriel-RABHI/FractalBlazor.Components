using System.Text.RegularExpressions;
using FractalBlazor.Components.Layout.Utilities;

namespace FractalBlazor.Components.Tests.Layout;

/// <summary>
/// The atoms are generated: their table must stay coherent, and the stylesheet must hold the
/// section generated from it.
/// </summary>
public sealed partial class AtomTests
{
    [Fact]
    public void Names_are_short_and_unique()
    {
        Assert.All(FbAtoms.Names, name => Assert.Matches("^fb-[a-z]{1,2}$", name));
        Assert.Equal(FbAtoms.Names.Count, FbAtoms.Names.Distinct().Count());
        Assert.Equal(FbAtoms.Declarations.Count, FbAtoms.Declarations.Distinct().Count());
    }

    [Theory]
    [InlineData("padding:var(--fb-space-l)", "padding-block:var(--fb-space-s)")]
    [InlineData("padding-block:var(--fb-space-s)", "padding-top:var(--fb-space-l)")]
    [InlineData("padding-inline:var(--fb-space-s)", "padding-right:var(--fb-space-l)")]
    [InlineData("margin:var(--fb-space-l)", "margin-inline:var(--fb-space-s)")]
    [InlineData("margin-block:var(--fb-space-s)", "margin-bottom:var(--fb-space-l)")]
    [InlineData("flex-direction:row", "flex-flow:column")]
    [InlineData("align-items:center", "place-items:start")]
    public void A_property_comes_after_the_shorthand_it_overrides(string shorthand, string property)
        => Assert.True(
            Index(shorthand) < Index(property),
            $"'{property}' must follow '{shorthand}' in the stylesheet to override it.");

    [Fact]
    public void Stylesheet_holds_the_generated_section()
    {
        var css = File.ReadAllText(Path.Combine(RepositoryRoot(), FbAtomStylesheet.RelativePath));

        Assert.True(
            FbAtomStylesheet.Section() == FbAtomStylesheet.Read(css),
            "The atoms section is out of date: run `dotnet run --project FractalBlazor.Components.Tools -- atoms`.");
    }

    [Fact]
    public void No_named_class_takes_the_name_of_an_atom()
    {
        var root = RepositoryRoot();
        var named = new HashSet<string>();
        foreach (var stylesheet in new[] { FbAtomStylesheet.RelativePath, "FractalBlazor.Components.Forms/wwwroot/fb-components-forms.css" })
        {
            var css = File.ReadAllText(Path.Combine(root, stylesheet));
            if (FbAtomStylesheet.Read(css) is { } section)
                css = css.Replace("\r\n", "\n").Replace(section, "");
            named.UnionWith(ClassSelector().Matches(css).Select(match => match.Groups[1].Value));
        }

        Assert.All(FbAtoms.Names, name => Assert.DoesNotContain(name, named));
    }

    private static int Index(string declaration) => FbAtoms.Declarations.ToList().IndexOf(declaration) switch
    {
        < 0 => throw new ArgumentException($"No atom declares '{declaration}'."),
        var index => index
    };

    private static string RepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "FractalBlazor.Components.slnx")))
            directory = directory.Parent;
        return directory?.FullName ?? throw new DirectoryNotFoundException("Repository root not found.");
    }

    [GeneratedRegex(@"(?<![\w-])\.([a-zA-Z][\w-]*)")]
    private static partial Regex ClassSelector();
}
