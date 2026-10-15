using FractalBlazor.Components.Forms.Theming.Constants;
using FractalBlazor.Components.Forms.Theming.Registry;
using FractalBlazor.Components.Layout;

namespace FractalBlazor.Components.Tests;

public sealed class SmokeTests : BunitContext
{
    [Fact]
    public void Row_renders_a_div_with_the_row_class()
    {
        var cut = Render<FbRow>(parameters => parameters.Add(p => p.WBA, true));

        var row = cut.Find("div");
        Assert.Contains("fb-row", row.ClassList);
        Assert.Contains("fb-wba", row.ClassList);
    }

    [Fact]
    public void Default_theme_resolves_both_branches()
    {
        var registry = new FbFormThemeRegistry();

        var dark = registry.Resolve(registry.Default.Name, FbThemeBranches.Dark);
        var light = registry.Resolve(registry.Default.Name, FbThemeBranches.Light);

        Assert.Equal(FbThemeVariants.All.Count, dark.VariantOrder.Count);
        Assert.Equal(FbThemeVariants.All.Count, light.VariantOrder.Count);
    }
}
