using FractalBlazor.Components.Forms.Theming.Constants;
using FractalBlazor.Components.Forms.Theming.Helpers;
using FractalBlazor.Components.Forms.Theming.Model;
using FractalBlazor.Components.Forms.Theming.Registry;
using FractalBlazor.Components.Layout.Theming.Model;

namespace FractalBlazor.Components.Tests.Theming;

public sealed class FrameColorsTests
{
    private readonly FbFormThemeRegistry _registry = new();

    private FbThemeSetup WithFrames(string variant, FbThemeFormColors frames) => new("Framed", _registry.Default)
    {
        Branches =
        [
            new FbThemeBranch(FbThemeBranches.Dark)
            {
                Variants = [new FbThemeColorVariant(variant) { FrameColors = frames }]
            }
        ]
    };

    [Fact]
    public void Default_theme_declares_no_frame_anchor_but_redirects_them()
    {
        var css = FbThemeCssWriter.ToCssVariables(_registry.Resolve(_registry.Default.Name, FbThemeBranches.Dark));

        Assert.DoesNotContain("--fb-default-frame-low-anchor:", css);
        Assert.Contains("--fb-frame-low-anchor:var(--fb-default-frame-low-anchor);", css);
        Assert.Contains("--fb-frame-high-anchor:var(--fb-default-frame-high-anchor);", css);
    }

    [Fact]
    public void Declared_frame_anchors_are_written_for_the_variant()
    {
        var frames = new FbThemeFormColors { LowAnchor = "#102030", HighAnchor = "#a0b0c0" };

        var css = FbThemeCssWriter.ToCssVariables(_registry.Resolve(WithFrames(FbThemeVariants.Error, frames), FbThemeBranches.Dark));

        Assert.Contains("--fb-error-frame-low-anchor:#102030;", css);
        Assert.Contains("--fb-error-frame-high-anchor:#a0b0c0;", css);
        Assert.DoesNotContain("--fb-default-frame-low-anchor:", css);
    }

    [Fact]
    public void Variants_inherit_the_frame_anchors_of_the_default_variant()
    {
        var frames = new FbThemeFormColors { LowAnchor = "#102030", HighAnchor = "#a0b0c0" };

        var theme = _registry.Resolve(WithFrames(FbThemeVariants.Default, frames), FbThemeBranches.Dark);

        Assert.Equal("#102030", theme.GetVariant(FbThemeVariants.Error).FrameColors.LowAnchor);
        Assert.Equal("#a0b0c0", theme.GetVariant(FbThemeVariants.Error).FrameColors.HighAnchor);
    }

    [Fact]
    public void One_frame_anchor_can_be_declared_alone()
    {
        var theme = _registry.Resolve(WithFrames(FbThemeVariants.Info, new FbThemeFormColors { HighAnchor = "#00adff" }), FbThemeBranches.Dark);

        var info = theme.GetVariant(FbThemeVariants.Info).FrameColors;
        Assert.Null(info.LowAnchor);
        Assert.Equal("#00adff", info.HighAnchor);
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
        var theme = WithFrames(FbThemeVariants.Error, new FbThemeFormColors { LowAnchor = "red;}" });

        Assert.Throws<InvalidOperationException>(() => _registry.Register(theme));
    }
}
