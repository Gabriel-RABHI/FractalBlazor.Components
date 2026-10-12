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
    /// Highlight strengths) must meet WCAG AA on every background. Subtle and Muted are
    /// secondary texts, and the colored variants a design choice made in the theme editor:
    /// their contrast is reported, not enforced.
    /// </summary>
    [Theory]
    [InlineData(FbThemeBranches.Dark)]
    [InlineData(FbThemeBranches.Light)]
    public void Default_variant_readable_text_meets_wcag_aa(string branch)
        => AssertReadableTextMeetsAA(FbThemeContrast.Analyze(new FbFormThemeRegistry().Resolve("Default", branch)));

    /// <summary>
    /// An inverted card is checked on its Surface and Accent backgrounds: with a strong
    /// Highlight offset, an inverted Highlight background comes close to the text anchors.
    /// </summary>
    [Theory]
    [InlineData(FbThemeBranches.Dark)]
    [InlineData(FbThemeBranches.Light)]
    public void Inverted_default_variant_readable_text_meets_wcag_aa(string branch)
        => AssertReadableTextMeetsAA(
            FbThemeContrast.Analyze(new FbFormThemeRegistry().Resolve("Default", branch), inverted: true),
            FbContrastBackground.Surface, FbContrastBackground.Accent);

    private static void AssertReadableTextMeetsAA(FbContrastReport report, params FbContrastBackground[] backgrounds)
    {
        var failing = report.Entries
            .Where(entry => entry.Variant == FbThemeVariants.Default)
            .Where(entry => backgrounds.Length == 0 || backgrounds.Contains(entry.Background))
            .Where(entry => (entry.Text is FbContrastText.Default or FbContrastText.Highlight) && !entry.MeetsAA)
            .Select(entry => entry.ToString())
            .ToList();

        Assert.Empty(failing);
    }
}
