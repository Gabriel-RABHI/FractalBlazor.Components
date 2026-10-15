using FractalBlazor.Components.Forms.Theming.Helpers;
using FractalBlazor.Components.Forms.Theming.Model;
using FractalBlazor.Components.Forms.Theming.Registry;
using FractalBlazor.Components.Forms.Theming.Constants;

namespace FractalBlazor.Components.Tests.Theming;

/// <summary>
/// Each text size has its own font family in the theme and in the CSS: the text family unless
/// the theme gives another one.
/// </summary>
public sealed class TypographyTests
{
    private readonly FbFormThemeRegistry _registry = new();

    private static readonly string[] Sizes = ["xs", "s", "m", "l", "xl", "xxl"];

    private static IEnumerable<string?> Families(FbThemeFormTypography typography) =>
    [
        typography.ExtraSmallFontFamily, typography.SmallFontFamily, typography.MediumFontFamily,
        typography.LargeFontFamily, typography.ExtraLargeFontFamily, typography.ExtraExtraLargeFontFamily
    ];

    private static string FormsStylesheet()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "FractalBlazor.Components.slnx")))
            directory = directory.Parent;
        return File.ReadAllText(Path.Combine(directory?.FullName ?? throw new DirectoryNotFoundException("Repository root not found."),
            "FractalBlazor.Components.Forms", "wwwroot", "fb-components-forms.css")).Replace("\r\n", "\n");
    }

    [Fact]
    public void Every_size_of_the_default_theme_follows_the_text_family()
    {
        var typography = _registry.Resolve(_registry.Default.Name, FbThemeBranches.Dark).Typography;

        Assert.All(Families(typography), family => Assert.Equal(FbThemeFormTypography.TextFont, family));
        Assert.All(Families(FbThemeFormTypography.Compact).Concat(Families(FbThemeFormTypography.Large)),
            family => Assert.Equal(FbThemeFormTypography.TextFont, family));
    }

    [Fact]
    public void The_theme_declares_the_family_of_every_size()
    {
        var css = FbThemeCssWriter.ToCssVariables(_registry.Resolve(_registry.Default.Name, FbThemeBranches.Dark));

        Assert.All(Sizes, size => Assert.Contains($"--fb-txt-{size}-font-family:var(--fb-txt-font-family);", css));
    }

    [Fact]
    public void A_theme_gives_a_size_its_own_family_and_the_others_keep_the_text_one()
    {
        var theme = new FbThemeSetup("Titles", _registry.Default)
        {
            Typography = new FbThemeFormTypography
            {
                TextFontFamily = "'Inter',sans-serif",
                ExtraExtraLargeFontFamily = "'Playfair Display',serif"
            }
        };

        var css = FbThemeCssWriter.ToCssVariables(_registry.Resolve(theme, FbThemeBranches.Dark));

        Assert.Contains("--fb-txt-font-family:'Inter',sans-serif;", css);
        Assert.Contains("--fb-txt-xxl-font-family:'Playfair Display',serif;", css);
        Assert.Contains("--fb-txt-xl-font-family:var(--fb-txt-font-family);", css);
    }

    [Fact]
    public void A_family_the_css_would_misread_is_refused()
    {
        var theme = new FbThemeSetup("Broken", _registry.Default)
        {
            Typography = new FbThemeFormTypography { LargeFontFamily = "serif;color:red" }
        };

        Assert.Throws<InvalidOperationException>(() => _registry.Resolve(theme, FbThemeBranches.Dark));
    }

    [Theory]
    [InlineData("xs")]
    [InlineData("s")]
    [InlineData("m")]
    [InlineData("l")]
    [InlineData("xl")]
    [InlineData("xxl")]
    public void Each_size_class_takes_the_family_of_its_size(string size)
        => Assert.Contains(
            $".fb-t-{size} {{\n    font-size: calc(var(--fb-txt-base-size) * var(--fb-txt-{size}-coef));\n    font-family: var(--fb-txt-{size}-font-family);",
            FormsStylesheet());

    [Theory]
    [InlineData("html {\n    font-family: var(--fb-txt-m-font-family);")]
    [InlineData(".fb-theme {\n    font-family: var(--fb-txt-m-font-family);")]
    public void The_text_of_the_page_takes_the_family_of_the_medium_size(string rule)
        => Assert.Contains(rule, FormsStylesheet());
}
