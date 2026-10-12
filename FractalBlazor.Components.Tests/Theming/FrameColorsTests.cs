using FractalBlazor.Components.Forms.Theming.Constants;
using FractalBlazor.Components.Forms.Theming.Helpers;
using FractalBlazor.Components.Forms.Theming.Model;
using FractalBlazor.Components.Forms.Theming.Registry;
using FractalBlazor.Components.Forms.Theming.Solver;
using FractalBlazor.Components.Layout.Theming.Model;

namespace FractalBlazor.Components.Tests.Theming;

public sealed class FrameColorsTests
{
    private readonly FbFormThemeRegistry _registry = new();

    private FbThemeSetup Derived(params FbThemeColorVariant[] variants) => new("Framed", _registry.Default)
    {
        Branches = [new FbThemeBranch(FbThemeBranches.Dark) { Variants = variants }]
    };

    /// <summary>
    /// The default theme with every frame anchor removed, as a theme without FrameColors resolves.
    /// </summary>
    private FbResolvedTheme WithoutFrameColors()
    {
        var theme = _registry.Resolve(_registry.Default.Name, FbThemeBranches.Dark);
        var variants = theme.VariantOrder.ToDictionary(
            name => name,
            name =>
            {
                var variant = theme.GetVariant(name);
                return new FbResolvedThemeVariant(name, variant.LayoutColors, variant.FormColors, new FbThemeFormColors());
            },
            StringComparer.OrdinalIgnoreCase);

        return new FbResolvedTheme(theme.Name, theme.Branch, theme.Spacings, theme.Corners, theme.Typography,
            theme.MasterTint, theme.TextVariant, theme.BordersMix, theme.SurfaceMix, variants, theme.VariantOrder);
    }

    [Theory]
    [InlineData(FbThemeBranches.Dark)]
    [InlineData(FbThemeBranches.Light)]
    public void Default_theme_declares_its_frame_anchors(string branch)
    {
        var css = FbThemeCssWriter.ToCssVariables(_registry.Resolve(_registry.Default.Name, branch));

        Assert.Contains("--fb-default-frame-low-anchor:", css);
        Assert.Contains("--fb-default-frame-high-anchor:", css);
    }

    [Fact]
    public void Without_frame_colors_no_anchor_is_written_but_the_references_stay()
    {
        var css = FbThemeCssWriter.ToCssVariables(WithoutFrameColors());

        Assert.DoesNotContain("-frame-low-anchor:#", css);
        Assert.DoesNotContain("-frame-high-anchor:#", css);
        Assert.Contains("--fb-frame-low-anchor:var(--fb-default-frame-low-anchor);", css);
        Assert.Contains("--fb-frame-high-anchor:var(--fb-default-frame-high-anchor);", css);
    }

    [Fact]
    public void Declared_frame_anchors_are_written_for_the_variant()
    {
        var css = FbThemeCssWriter.ToCssVariables(_registry.Resolve(
            Derived(new FbThemeColorVariant(FbThemeVariants.Error) { FrameColors = new FbThemeFormColors { LowAnchor = "#102030", HighAnchor = "#a0b0c0" } }),
            FbThemeBranches.Dark));

        Assert.Contains("--fb-error-frame-low-anchor:#102030;", css);
        Assert.Contains("--fb-error-frame-high-anchor:#a0b0c0;", css);
    }

    [Fact]
    public void A_variant_without_frame_colors_takes_those_of_the_default_variant()
    {
        var theme = _registry.Resolve(
            Derived(
                new FbThemeColorVariant(FbThemeVariants.Default) { FrameColors = new FbThemeFormColors { LowAnchor = "#102030", HighAnchor = "#a0b0c0" } },
                new FbThemeColorVariant("Custom") { LayoutColors = new FbThemeLayoutColors { HighAnchor = "#ff00ff" } }),
            FbThemeBranches.Dark);

        Assert.Equal("#102030", theme.GetVariant("Custom").FrameColors.LowAnchor);
        Assert.Equal("#a0b0c0", theme.GetVariant("Custom").FrameColors.HighAnchor);
    }

    [Fact]
    public void One_frame_anchor_can_be_declared_alone()
    {
        var parent = _registry.Resolve(_registry.Default.Name, FbThemeBranches.Dark).GetVariant(FbThemeVariants.Info).FrameColors;

        var theme = _registry.Resolve(
            Derived(new FbThemeColorVariant(FbThemeVariants.Info) { FrameColors = new FbThemeFormColors { HighAnchor = "#123456" } }),
            FbThemeBranches.Dark);

        var info = theme.GetVariant(FbThemeVariants.Info).FrameColors;
        Assert.Equal(parent.LowAnchor, info.LowAnchor);
        Assert.Equal("#123456", info.HighAnchor);
    }

    [Fact]
    public void Inversion_exchanges_the_frame_anchors()
    {
        var references = FbThemeCssNames.VariantReferences("error", foreground: true, invert: true);

        Assert.Contains("--fb-frame-low-anchor:var(--fb-error-frame-high-anchor);", references);
        Assert.Contains("--fb-frame-high-anchor:var(--fb-error-frame-low-anchor);", references);
    }

    [Fact]
    public void Invalid_frame_anchor_is_refused()
    {
        var theme = Derived(new FbThemeColorVariant(FbThemeVariants.Error) { FrameColors = new FbThemeFormColors { LowAnchor = "red;}" } });

        Assert.Throws<InvalidOperationException>(() => _registry.Register(theme));
    }
}
