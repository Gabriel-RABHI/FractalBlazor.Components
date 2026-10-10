using FractalBlazor.Components.Forms.Theming;
using FractalBlazor.Components.Forms.Theming.Constants;
using FractalBlazor.Components.Layout.Theming;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Tests.Theming;

public sealed class VariantValidationTests : BunitContext
{
    public VariantValidationTests()
    {
        Services.AddFractalBlazorTheming();
    }

    private static RenderFragment Variant<TVariant>(string name) where TVariant : IComponent
        => builder =>
        {
            builder.OpenComponent<TVariant>(0);
            builder.AddAttribute(1, "Variant", name);
            builder.CloseComponent();
        };

    [Fact]
    public void Known_variant_inside_a_theme_renders()
    {
        var cut = Render<FbTheme>(p => p.Add(x => x.ChildContent, Variant<FbVariant>(FbThemeVariants.Error)));

        Assert.Single(cut.FindAll("div.fb-theme-scope"));
    }

    [Fact]
    public void Unknown_variant_inside_a_theme_fails_explicitly()
    {
        var error = Assert.Throws<KeyNotFoundException>(
            () => Render<FbTheme>(p => p.Add(x => x.ChildContent, Variant<FbVariant>("Nope"))));

        Assert.Contains("Nope", error.Message);
    }

    [Fact]
    public void Variant_without_a_theme_context_is_trusted()
    {
        var cut = Render<FbVariant>(p => p.Add(x => x.Variant, "Custom"));

        Assert.Contains("--fb-custom-bg-low-anchor", cut.Find("div").GetAttribute("style"));
    }

    [Fact]
    public void Unknown_layout_variant_inside_a_layout_theme_fails_explicitly()
    {
        Assert.Throws<KeyNotFoundException>(
            () => Render<FbLayoutTheme>(p => p.Add(x => x.ChildContent, Variant<FbLayoutVariant>("Nope"))));
    }

    [Fact]
    public void Layout_variant_without_a_theme_context_is_trusted()
    {
        var cut = Render<FbLayoutVariant>(p => p.Add(x => x.Variant, "Error"));

        Assert.Contains("--fb-error-bg-low-anchor", cut.Find("div").GetAttribute("style"));
    }
}
