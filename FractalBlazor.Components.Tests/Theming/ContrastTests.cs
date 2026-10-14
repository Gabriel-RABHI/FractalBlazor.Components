using FractalBlazor.Components.Forms.Theming.Constants;
using FractalBlazor.Components.Forms.Theming.Helpers;
using FractalBlazor.Components.Forms.Theming.Registry;
using FractalBlazor.Components.Layout.Colors;

namespace FractalBlazor.Components.Tests.Theming;

public sealed class ContrastTests(ITestOutputHelper output)
{
    [Fact]
    public void Black_on_white_is_21_to_1()
    {
        Assert.Equal(21d, FbColorMath.ContrastRatio("#000000", "#FFFFFF"), 2);
        Assert.Equal(1d, FbColorMath.ContrastRatio("#FFF", "#FFFFFF"), 2);
    }

    [Fact]
    public void Mix_matches_css_color_mix_in_oklab()
    {
        // color-mix(in oklab, black, white 50%) is rgb(99 99 99) in browsers.
        Assert.Equal("#636363", FbColorMath.Mix("#000000", "#FFFFFF", 50));
        Assert.Equal("#123456", FbColorMath.Mix("#123456", "#ABCDEF", 0));
        Assert.Equal("#ABCDEF", FbColorMath.Mix("#123456", "#ABCDEF", 100));
    }

    [Theory]
    [InlineData(FbThemeBranches.Dark)]
    [InlineData(FbThemeBranches.Light)]
    public void Default_theme_report_covers_every_variant(string branch)
    {
        var theme = new FbFormThemeRegistry().Resolve("Default", branch);

        var report = FbThemeContrast.Analyze(theme);

        foreach (var entry in report.Entries)
            output.WriteLine(entry.ToString());

        Assert.Empty(report.Skipped);
        Assert.Equal(theme.VariantOrder.Count * 3 * 4, report.Entries.Count);
    }

    /// <summary>
    /// Guard rail for palette changes: the body text of the Default variant (Default and
    /// Highlight strengths) must meet WCAG AA on the Surface and Accent backgrounds, and the
    /// Highlight text on the Highlight background too. The Highlight background sits near the
    /// middle of the gradient, where the Default text of the default theme reaches about
    /// 4.1:1. Subtle and Muted are secondary texts, and the colored variants a design choice
    /// made in the theme editor: their contrast is reported, not enforced.
    /// </summary>
    [Theory]
    [InlineData(FbThemeBranches.Dark)]
    [InlineData(FbThemeBranches.Light)]
    public void Default_variant_readable_text_meets_wcag_aa(string branch)
        => AssertReadableTextMeetsAA(FbThemeContrast.Analyze(new FbFormThemeRegistry().Resolve("Default", branch)));

    /// <summary>
    /// An inverted card reverses the gradient: the same guard rail applies.
    /// </summary>
    [Theory]
    [InlineData(FbThemeBranches.Dark)]
    [InlineData(FbThemeBranches.Light)]
    public void Inverted_default_variant_readable_text_meets_wcag_aa(string branch)
        => AssertReadableTextMeetsAA(
            FbThemeContrast.Analyze(new FbFormThemeRegistry().Resolve("Default", branch), inverted: true));

    private static void AssertReadableTextMeetsAA(FbContrastReport report)
    {
        var failing = report.Entries
            .Where(entry => entry.Variant == FbThemeVariants.Default)
            .Where(entry => entry.Text == FbContrastText.Highlight
                || (entry.Text == FbContrastText.Default && entry.Background != FbContrastBackground.Highlight))
            .Where(entry => !entry.MeetsAA)
            .Select(entry => entry.ToString())
            .ToList();

        Assert.Empty(failing);
    }
}
