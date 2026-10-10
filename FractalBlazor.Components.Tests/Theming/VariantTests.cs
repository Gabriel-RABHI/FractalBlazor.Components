using FractalBlazor.Components.Forms.Theming;
using FractalBlazor.Components.Forms.Theming.Constants;

namespace FractalBlazor.Components.Tests.Theming;

public sealed class VariantTests : BunitContext
{
    private string StyleOf(Action<ComponentParameterCollectionBuilder<FbVariant>> parameters)
        => Render<FbVariant>(parameters).Find("div.fb-theme-scope").GetAttribute("style")!;

    [Fact]
    public void A_variant_only_redirects_the_four_anchors()
    {
        var style = StyleOf(p => p.Add(x => x.Variant, FbThemeVariants.Error));

        Assert.Equal(
            "--fb-bg-low-anchor:var(--fb-error-bg-low-anchor);" +
            "--fb-bg-high-anchor:var(--fb-error-bg-high-anchor);" +
            "--fb-fg-low-anchor:var(--fb-error-fg-low-anchor);" +
            "--fb-fg-high-anchor:var(--fb-error-fg-high-anchor);",
            style);
    }

    [Fact]
    public void Swap_background_exchanges_the_background_anchors_only()
    {
        var style = StyleOf(p => p.Add(x => x.Variant, FbThemeVariants.Error).Add(x => x.SB, true));

        Assert.Contains("--fb-bg-low-anchor:var(--fb-error-bg-high-anchor);", style);
        Assert.Contains("--fb-bg-high-anchor:var(--fb-error-bg-low-anchor);", style);
        Assert.Contains("--fb-fg-low-anchor:var(--fb-error-fg-low-anchor);", style);
    }

    [Fact]
    public void Swap_foreground_exchanges_the_foreground_anchors_only()
    {
        var style = StyleOf(p => p.Add(x => x.SF, true));

        Assert.Contains("--fb-bg-low-anchor:var(--fb-default-bg-low-anchor);", style);
        Assert.Contains("--fb-fg-low-anchor:var(--fb-default-fg-high-anchor);", style);
        Assert.Contains("--fb-fg-high-anchor:var(--fb-default-fg-low-anchor);", style);
    }

    [Fact]
    public void Empty_variant_falls_back_to_default()
    {
        var style = StyleOf(p => p.Add(x => x.Variant, ""));

        Assert.StartsWith("--fb-bg-low-anchor:var(--fb-default-bg-low-anchor);", style);
    }
}
