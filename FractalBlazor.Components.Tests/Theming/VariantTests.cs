using FractalBlazor.Components.Forms.Theming;
using FractalBlazor.Components.Forms.Theming.Constants;
using FractalBlazor.Components.Layout.Theming;

namespace FractalBlazor.Components.Tests.Theming;

/// <summary>
/// A variant is a class: the scope only carries it, without any inline redirection.
/// </summary>
public sealed class VariantTests : BunitContext
{
    private AngleSharp.Dom.IElement ScopeOf(Action<ComponentParameterCollectionBuilder<FbVariant>> parameters)
        => Render<FbVariant>(parameters).Find("div.fb-theme-scope");

    [Fact]
    public void A_variant_is_a_class()
    {
        var scope = ScopeOf(p => p.Add(x => x.Variant, FbThemeVariants.Error));

        Assert.Equal("fb-theme-scope fb-variant fb-v-error", scope.GetAttribute("class"));
        Assert.Null(scope.GetAttribute("style"));
    }

    [Fact]
    public void Invert_adds_its_class()
        => Assert.Equal("fb-theme-scope fb-variant fb-v-error fb-invert",
            ScopeOf(p => p.Add(x => x.Variant, FbThemeVariants.Error).Add(x => x.Invert, true)).GetAttribute("class"));

    [Fact]
    public void Layout_invert_adds_the_same_classes()
        => Assert.Equal("fb-theme-scope fb-variant fb-v-default fb-invert",
            Render<FbLayoutVariant>(p => p.Add(x => x.Invert, true)).Find("div").GetAttribute("class"));

    [Fact]
    public void Empty_variant_falls_back_to_default()
        => Assert.Contains("fb-v-default", ScopeOf(p => p.Add(x => x.Variant, "")).ClassList);

    [Fact]
    public void A_name_is_normalized_for_its_class()
        => Assert.Contains("fb-v-brand-accent", ScopeOf(p => p.Add(x => x.Variant, "Brand Accent")).ClassList);

    [Fact]
    public void Custom_classes_and_style_come_after_the_variant()
    {
        var scope = ScopeOf(p => p.Add(x => x.Variant, FbThemeVariants.Info).Add(x => x.Classes, "card").Add(x => x.Style, "gap:0"));

        Assert.Equal("fb-theme-scope fb-variant fb-v-info card", scope.GetAttribute("class"));
        Assert.Equal("gap:0", scope.GetAttribute("style"));
    }
}
