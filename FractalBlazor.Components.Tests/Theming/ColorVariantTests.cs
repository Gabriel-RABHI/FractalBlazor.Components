using FractalBlazor.Components.Forms.Theming;
using FractalBlazor.Components.Forms.Theming.Constants;
using FractalBlazor.Components.Forms.Theming.Registry;
using FractalBlazor.Components.Layout.Colors;
using FractalBlazor.Components.Layout.Theming;
using FractalBlazor.Components.Layout.Theming.Contracts;
using FractalBlazor.Components.Layout.Theming.Registry;

namespace FractalBlazor.Components.Tests.Theming;

/// <summary>
/// Every color of the base palette is a variant of the default themes, after the functional
/// ones: <c>Variant="Red"</c> gives an element a color, as <c>Variant="Error"</c> gives it a
/// meaning.
/// </summary>
public sealed class ColorVariantTests : BunitContext
{
    [Fact]
    public void The_color_variants_are_the_palette_in_its_order()
    {
        var palette = Enum.GetNames<FbThemeBaseColorsIndex>();

        Assert.Equal(palette, FbThemeVariants.Colors);
        Assert.Equal(palette, FbLayoutThemeVariants.Colors);
        Assert.Equal([.. FbThemeVariants.Standard, .. FbThemeVariants.Colors], FbThemeVariants.All);
    }

    [Theory]
    [InlineData(FbThemeBranches.Dark)]
    [InlineData(FbThemeBranches.Light)]
    public void The_default_theme_defines_every_color_after_the_functional_variants(string branch)
        => Assert.Equal(FbThemeVariants.All, new FbFormThemeRegistry().Resolve("Default", branch).VariantOrder);

    [Fact]
    public void The_default_layout_theme_defines_every_color_after_the_functional_variants()
        => Assert.Equal(FbLayoutThemeVariants.All, new FbLayoutThemeRegistry().Resolve("Default").VariantOrder);

    /// <summary>
    /// A chromatic color has the gradient of the functional variants, a neutral one that of
    /// Default: a functional variant is the color it is made of.
    /// </summary>
    [Theory]
    [InlineData(FbThemeBranches.Dark, FbThemeVariants.Red, FbThemeVariants.Error)]
    [InlineData(FbThemeBranches.Dark, FbThemeVariants.Blue, FbThemeVariants.Selected)]
    [InlineData(FbThemeBranches.Dark, FbThemeVariants.Orange, FbThemeVariants.Warning)]
    [InlineData(FbThemeBranches.Dark, FbThemeVariants.Green, FbThemeVariants.Success)]
    [InlineData(FbThemeBranches.Dark, FbThemeVariants.Sky, FbThemeVariants.Info)]
    [InlineData(FbThemeBranches.Dark, FbThemeVariants.Zinc, FbThemeVariants.Default)]
    [InlineData(FbThemeBranches.Light, FbThemeVariants.Red, FbThemeVariants.Error)]
    [InlineData(FbThemeBranches.Light, FbThemeVariants.Zinc, FbThemeVariants.Default)]
    public void A_functional_variant_is_the_color_it_is_made_of(string branch, string color, string functional)
    {
        var theme = new FbFormThemeRegistry().Resolve("Default", branch);

        Assert.Equivalent(theme.GetVariant(functional).LayoutColors, theme.GetVariant(color).LayoutColors);
    }

    [Fact]
    public void A_color_variant_is_a_class_with_a_rule_of_the_stylesheet()
    {
        Services.AddFractalBlazorTheming();
        var cut = Render<FbTheme>(p => p.Add(x => x.ChildContent, builder =>
        {
            builder.OpenComponent<FbVariant>(0);
            builder.AddAttribute(1, nameof(FbVariant.Variant), FbThemeVariants.Fuchsia);
            builder.CloseComponent();
        }));

        Assert.Contains("fb-v-fuchsia", cut.Find("div.fb-theme-scope").ClassList);
        Assert.Contains("--fb-fuchsia-mid-anchor:", cut.Find("style").TextContent);
        Assert.DoesNotContain(".fb-v-fuchsia", cut.Find("style").TextContent);
    }
}
