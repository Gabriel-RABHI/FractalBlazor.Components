using FractalBlazor.Components.Forms.Theming;
using FractalBlazor.Components.Forms.Theming.Constants;

namespace FractalBlazor.Components.Tests.Theming;

public sealed class VariantTests : BunitContext
{
    private string StyleOf(Action<ComponentParameterCollectionBuilder<FbVariant>> parameters)
        => Render<FbVariant>(parameters).Find("div.fb-theme-scope").GetAttribute("style")!;

    [Fact]
    public void A_variant_only_redirects_its_anchors()
    {
        var style = StyleOf(p => p.Add(x => x.Variant, FbThemeVariants.Error));

        Assert.Equal(
            "--fb-low-anchor:var(--fb-error-low-anchor);" +
            "--fb-mid-anchor:var(--fb-error-mid-anchor);" +
            "--fb-high-anchor:var(--fb-error-high-anchor);",
            style);
    }

    [Fact]
    public void Invert_reverses_the_gradient()
    {
        var style = StyleOf(p => p.Add(x => x.Variant, FbThemeVariants.Error).Add(x => x.Invert, true));

        Assert.Equal(
            "--fb-low-anchor:var(--fb-error-high-anchor);" +
            "--fb-mid-anchor:var(--fb-error-mid-anchor);" +
            "--fb-high-anchor:var(--fb-error-low-anchor);",
            style);
    }

    [Fact]
    public void Layout_invert_reverses_the_gradient_too()
    {
        var style = Render<FractalBlazor.Components.Layout.Theming.FbLayoutVariant>(p => p.Add(x => x.Invert, true))
            .Find("div").GetAttribute("style");

        Assert.Equal(
            "--fb-low-anchor:var(--fb-default-high-anchor);" +
            "--fb-mid-anchor:var(--fb-default-mid-anchor);" +
            "--fb-high-anchor:var(--fb-default-low-anchor);",
            style);
    }

    [Fact]
    public void Empty_variant_falls_back_to_default()
    {
        var style = StyleOf(p => p.Add(x => x.Variant, ""));

        Assert.StartsWith("--fb-low-anchor:var(--fb-default-low-anchor);", style);
    }
}
