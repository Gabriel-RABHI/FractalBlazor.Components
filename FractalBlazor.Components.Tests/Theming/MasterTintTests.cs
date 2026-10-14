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
/// The master tint draws every anchor toward one color, keeping its lightness: a colored tint
/// leans all the hues toward it, a gray one lowers their intensity.
/// </summary>
public sealed class MasterTintTests
{
    private readonly FbFormThemeRegistry _forms = new();

    private static int Spread(string color)
    {
        var hex = color.TrimStart('#');
        int[] channels = [Convert.ToInt32(hex[..2], 16), Convert.ToInt32(hex[2..4], 16), Convert.ToInt32(hex[4..6], 16)];
        return channels.Max() - channels.Min();
    }

    [Fact]
    public void An_anchor_keeps_its_lightness_and_leans_toward_the_tint()
    {
        var tinted = FbColorMath.Tint("#15151e", "#0ea5e9", 40);

        Assert.Equal(FbColorMath.Lightness("#15151e"), FbColorMath.Lightness(tinted), 2);
        Assert.NotEqual("#15151E", tinted);
        Assert.Equal("#15151E", FbColorMath.Tint("#15151e", "#0ea5e9", 0));
    }

    [Fact]
    public void A_gray_tint_lowers_the_intensity_of_a_color()
    {
        var half = FbColorMath.Tint("#e11d48", "#808080", 50);
        var full = FbColorMath.Tint("#e11d48", "#808080", 100);

        Assert.True(Spread(half) < Spread("#e11d48"));
        Assert.True(Spread(full) <= 2);
        Assert.Equal(FbColorMath.Lightness("#e11d48"), FbColorMath.Lightness(full), 2);
    }

    [Fact]
    public void An_active_tint_is_declared_once_and_read_by_every_anchor()
    {
        var css = FbLayoutThemeCssWriter.ToCssVariables(new FbLayoutThemeRegistry().Resolve("Default"));
        var zinc950 = FbThemeBaseColors.GetColor(FbThemeBaseColorsIndex.Zinc, FbThemeBaseShadesIndex._950);
        var lightness = FbColorMath.Lightness(zinc950).ToString("0.####", System.Globalization.CultureInfo.InvariantCulture);

        Assert.Contains($"--fb-tint-color:{FbThemeBaseColors.GetColor(FbThemeBaseColorsIndex.Sky)};--fb-tint-mix:5%;", css);
        Assert.Contains($"--fb-default-low-anchor:color-mix(in oklab,{zinc950},oklch(from var(--fb-tint-color) {lightness} c h) var(--fb-tint-mix));", css);
    }

    [Theory]
    [InlineData(FbThemeBranches.Dark)]
    [InlineData(FbThemeBranches.Light)]
    public void A_tint_mixed_at_zero_leaves_the_anchors_as_declared(string branch)
    {
        var theme = _forms.Resolve(_forms.Default.Name, branch);
        var css = FbThemeCssWriter.ToCssVariables(theme);

        Assert.Equal("0%", theme.MasterTint.TintMix);
        Assert.DoesNotContain("--fb-tint-color", css);
        Assert.Contains($"--fb-default-low-anchor:{theme.GetVariant(FbThemeVariants.Default).LayoutColors.LowAnchor};", css);
    }

    [Fact]
    public void A_branch_inherits_the_tint_value_by_value()
    {
        var edited = new FbThemeSetup("Edited", _forms.Default)
        {
            Branches = [new FbThemeBranch(FbThemeBranches.Dark) { MasterTint = new FbThemeMasterTint { TintMix = "20%" } }]
        };

        var dark = _forms.Resolve(edited, FbThemeBranches.Dark);
        var light = _forms.Resolve(edited, FbThemeBranches.Light);

        Assert.Equal("20%", dark.MasterTint.TintMix);
        Assert.Equal(_forms.Resolve(_forms.Default.Name, FbThemeBranches.Dark).MasterTint.ColorTint, dark.MasterTint.ColorTint);
        Assert.Equal("0%", light.MasterTint.TintMix);
        Assert.Contains("--fb-tint-mix:20%;", FbThemeCssWriter.ToCssVariables(dark));
    }

    [Fact]
    public void A_layout_theme_inherits_the_tint_value_by_value()
    {
        var registry = new FbLayoutThemeRegistry();
        registry.Register(new FbLayoutThemeSetup("Gray", registry.Default)
        {
            MasterTint = new FbThemeMasterTint { ColorTint = "#808080" }
        });

        var tint = registry.Resolve("Gray").MasterTint;

        Assert.Equal("#808080", tint.ColorTint);
        Assert.Equal("5%", tint.TintMix);
    }

    [Theory]
    [InlineData("150%")]
    [InlineData("-5%")]
    [InlineData("strong")]
    public void A_mix_outside_zero_to_a_hundred_percent_is_refused(string mix)
    {
        var registry = new FbLayoutThemeRegistry();

        Assert.Throws<InvalidOperationException>(() => registry.Register(new FbLayoutThemeSetup("Wrong", registry.Default)
        {
            MasterTint = new FbThemeMasterTint { TintMix = mix }
        }));
    }

    [Fact]
    public void Contrasts_are_measured_on_the_tinted_anchors()
    {
        var edited = new FbThemeSetup("Gray", _forms.Default)
        {
            Branches = [new FbThemeBranch(FbThemeBranches.Dark) { MasterTint = new FbThemeMasterTint { ColorTint = "#808080", TintMix = "100%" } }]
        };
        var theme = _forms.Resolve(edited, FbThemeBranches.Dark);
        var selected = theme.GetVariant(FbThemeVariants.Selected);
        FbColorMath.TryParsePercent(theme.SurfaceMix.NeutralMix, out var neutral);
        FbColorMath.TryParsePercent(theme.SurfaceMix.SurfaceOffset, out var surfaceOffset);

        var surface = FbThemeContrast.Analyze(theme).Entries
            .First(entry => entry.Variant == FbThemeVariants.Selected && entry.Background == FbContrastBackground.Surface);

        var expected = FbColorMath.Sample(
            FbColorMath.Tint(selected.LayoutColors.LowAnchor!, "#808080", 100),
            FbColorMath.Tint(selected.LayoutColors.MidAnchor!, "#808080", 100),
            FbColorMath.Tint(selected.LayoutColors.HighAnchor!, "#808080", 100),
            neutral + surfaceOffset);
        Assert.Equal(expected, surface.BackgroundColor);
        Assert.True(Spread(surface.BackgroundColor) <= 2);
    }
}
