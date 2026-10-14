using FractalBlazor.Components.Forms.Theming.Constants;
using FractalBlazor.Components.Forms.Theming.Helpers;
using FractalBlazor.Components.Forms.Theming.Model;
using FractalBlazor.Components.Forms.Theming.Registry;
using FractalBlazor.Components.Layout;
using FractalBlazor.Components.Layout.Colors;
using FractalBlazor.Components.Layout.Theming.Helpers;
using FractalBlazor.Components.Layout.Theming.Model;
using FractalBlazor.Components.Layout.Theming.Registry;

namespace FractalBlazor.Components.Tests.Theming;

/// <summary>
/// One gradient per variant, three anchors at 0 %, 50 % and 100 %: backgrounds, texts and
/// frames are positions on it, placed from the neutral of their block.
/// </summary>
public sealed class GradientTests
{
    private readonly FbFormThemeRegistry _registry = new();

    private FbThemeSetup Derived(params FbThemeColorVariant[] variants) => new("Derived", _registry.Default)
    {
        Branches = [new FbThemeBranch(FbThemeBranches.Dark) { Variants = variants }]
    };

    [Theory]
    [InlineData(FbThemeBranches.Dark)]
    [InlineData(FbThemeBranches.Light)]
    public void Every_variant_writes_its_three_anchors(string branch)
    {
        var theme = _registry.Resolve(_registry.Default.Name, branch);
        var css = FbThemeCssWriter.ToCssVariables(theme);

        foreach (var name in theme.VariantOrder)
        {
            var colors = theme.GetVariant(name).LayoutColors;
            var variant = FbThemeCssNames.Normalize(name);
            Assert.Contains($"--fb-{variant}-low-anchor:{colors.LowAnchor};", css);
            Assert.Contains($"--fb-{variant}-mid-anchor:{colors.MidAnchor};", css);
            Assert.Contains($"--fb-{variant}-high-anchor:{colors.HighAnchor};", css);
        }
    }

    [Fact]
    public void The_light_branch_reverses_the_gradient_of_the_dark_one()
    {
        var dark = _registry.Resolve(_registry.Default.Name, FbThemeBranches.Dark).GetVariant(FbThemeVariants.Error).LayoutColors;
        var light = _registry.Resolve(_registry.Default.Name, FbThemeBranches.Light).GetVariant(FbThemeVariants.Error).LayoutColors;

        Assert.Equal(dark.LowAnchor, light.HighAnchor);
        Assert.Equal(dark.MidAnchor, light.MidAnchor);
        Assert.Equal(dark.HighAnchor, light.LowAnchor);
    }

    [Fact]
    public void The_neutral_and_offsets_stay_percentages_and_the_mixes_become_fractions()
    {
        var css = FbThemeCssWriter.ToCssVariables(_registry.Resolve(_registry.Default.Name, FbThemeBranches.Dark));

        Assert.Contains("--fb-neutral-mix:0%;--fb-surface-offset:5%;--fb-accent-offset:15%;--fb-highlight-offset:45%;--fb-hover-offset:10%;", css);
        Assert.Contains("--fb-frame-light-mix:0.05;--fb-frame-medium-mix:0.1;--fb-frame-strong-mix:0.15;", css);
        Assert.Contains("--fb-fg-subtle-mix:0.4;--fb-fg-muted-mix:0.6;--fb-fg-default-mix:0.8;--fb-fg-highlight-mix:0.95;", css);
    }

    [Fact]
    public void A_variant_redirects_its_three_anchors_and_invert_reverses_them()
    {
        Assert.Equal(
            "--fb-low-anchor:var(--fb-error-low-anchor);--fb-mid-anchor:var(--fb-error-mid-anchor);--fb-high-anchor:var(--fb-error-high-anchor);",
            FbThemeCssNames.VariantReferences("error", invert: false));
        Assert.Equal(
            "--fb-low-anchor:var(--fb-error-high-anchor);--fb-mid-anchor:var(--fb-error-mid-anchor);--fb-high-anchor:var(--fb-error-low-anchor);",
            FbThemeCssNames.VariantReferences("error", invert: true));
    }

    [Fact]
    public void An_anchor_left_out_is_inherited_one_by_one()
    {
        var parent = _registry.Resolve(_registry.Default.Name, FbThemeBranches.Dark).GetVariant(FbThemeVariants.Info).LayoutColors;

        var info = _registry.Resolve(
            Derived(new FbThemeColorVariant(FbThemeVariants.Info) { LayoutColors = new FbThemeLayoutColors { MidAnchor = "#123456" } }),
            FbThemeBranches.Dark).GetVariant(FbThemeVariants.Info).LayoutColors;

        Assert.Equal(parent.LowAnchor, info.LowAnchor);
        Assert.Equal("#123456", info.MidAnchor);
        Assert.Equal(parent.HighAnchor, info.HighAnchor);
    }

    [Fact]
    public void A_new_variant_takes_the_anchors_it_leaves_out_from_the_default_variant()
    {
        var theme = _registry.Resolve(
            Derived(new FbThemeColorVariant("Custom") { LayoutColors = new FbThemeLayoutColors { HighAnchor = "#a0b0c0" } }),
            FbThemeBranches.Dark);
        var defaults = theme.GetVariant(FbThemeVariants.Default).LayoutColors;

        var custom = theme.GetVariant("Custom").LayoutColors;

        Assert.Equal(defaults.LowAnchor, custom.LowAnchor);
        Assert.Equal(defaults.MidAnchor, custom.MidAnchor);
        Assert.Equal("#a0b0c0", custom.HighAnchor);
        Assert.Contains("--fb-custom-high-anchor:#a0b0c0;", FbThemeCssWriter.ToCssVariables(theme));
    }

    [Fact]
    public void Invalid_anchor_is_refused()
    {
        var theme = Derived(new FbThemeColorVariant(FbThemeVariants.Error) { LayoutColors = new FbThemeLayoutColors { MidAnchor = "red;}" } });

        Assert.Throws<InvalidOperationException>(() => _registry.Register(theme));
    }

    [Theory]
    [InlineData("150%")]
    [InlineData("-5%")]
    [InlineData("5")]
    [InlineData("calc(5% + 1%)")]
    public void A_mix_that_is_not_a_percentage_between_zero_and_a_hundred_is_refused(string mix)
    {
        var forms = new FbThemeSetup("Wrong", _registry.Default)
        {
            Branches = [new FbThemeBranch(FbThemeBranches.Dark) { BordersMix = new FbThemeLayoutBordersMix { LightMix = mix } }]
        };
        var layout = new FbLayoutThemeRegistry();

        Assert.Throws<InvalidOperationException>(() => _registry.Register(forms));
        Assert.Throws<InvalidOperationException>(() => layout.Register(new FbLayoutThemeSetup("Wrong", layout.Default)
        {
            SurfaceMix = new FbThemeLayoutSurfaceMix { AccentOffset = mix }
        }));
    }

    [Fact]
    public void The_layout_theme_writes_the_same_gradient()
    {
        var theme = new FbLayoutThemeRegistry().Resolve("Default");
        var css = FbLayoutThemeCssWriter.ToCssVariables(theme);

        Assert.Contains("--fb-neutral-mix:0%;--fb-surface-offset:5%;", css);
        Assert.Contains("--fb-default-mid-anchor:", css);
        Assert.EndsWith("--fb-low-anchor:var(--fb-default-low-anchor);--fb-mid-anchor:var(--fb-default-mid-anchor);--fb-high-anchor:var(--fb-default-high-anchor);", css);
    }

    [Fact]
    public void Sampling_goes_through_the_middle_anchor()
    {
        const string low = "#000000", mid = "#ff0000", high = "#ffffff";

        Assert.Equal("#000000", FbColorMath.Sample(low, mid, high, 0));
        Assert.Equal(FbColorMath.Mix(low, mid, 50), FbColorMath.Sample(low, mid, high, 25));
        Assert.Equal("#FF0000", FbColorMath.Sample(low, mid, high, 50));
        Assert.Equal(FbColorMath.Mix(mid, high, 50), FbColorMath.Sample(low, mid, high, 75));
        Assert.Equal("#FFFFFF", FbColorMath.Sample(low, mid, high, 100));
        Assert.Null(FbColorMath.Sample(low, "red", high, 50));
    }

    /// <summary>
    /// <c>100 − (100 − mix) × (100 − neutral) / 100</c>: the formula of the stylesheets.
    /// </summary>
    [Theory]
    [InlineData(80, 0, 80)]
    [InlineData(80, 75, 95)]
    [InlineData(5, 30, 33.5)]
    [InlineData(40, 100, 100)]
    public void Texts_and_frames_move_toward_the_end_of_the_gradient_as_the_neutral_rises(double mix, double neutral, double expected)
        => Assert.Equal(expected, FbColorMath.Relative(mix, neutral), 6);

    [Fact]
    public void Contrasts_follow_the_formulas_of_the_stylesheets()
    {
        var theme = _registry.Resolve(_registry.Default.Name, FbThemeBranches.Dark);
        var colors = theme.GetVariant(FbThemeVariants.Default).LayoutColors;

        var entry = FbThemeContrast.Analyze(theme).Entries.Single(value =>
            value.Variant == FbThemeVariants.Default && value.Background == FbContrastBackground.Accent && value.Text == FbContrastText.Default);

        // An accent block on the page: neutral 0 % + 15 %; its default text at 80 % from there.
        Assert.Equal(FbColorMath.Sample(colors.LowAnchor, colors.MidAnchor, colors.HighAnchor, 15), entry.BackgroundColor);
        Assert.Equal(FbColorMath.Sample(colors.LowAnchor, colors.MidAnchor, colors.HighAnchor, 83), entry.TextColor);
    }

    [Fact]
    public void An_inverted_report_reverses_the_gradient()
    {
        var theme = _registry.Resolve(_registry.Default.Name, FbThemeBranches.Dark);
        var colors = theme.GetVariant(FbThemeVariants.Default).LayoutColors;

        var entry = FbThemeContrast.Analyze(theme, inverted: true).Entries.First(value => value.Variant == FbThemeVariants.Default);

        Assert.Equal(FbColorMath.Sample(colors.HighAnchor, colors.MidAnchor, colors.LowAnchor, 5), entry.BackgroundColor);
    }
}
