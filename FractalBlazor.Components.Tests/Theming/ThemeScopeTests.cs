using FractalBlazor.Components.Forms.Theming;
using FractalBlazor.Components.Forms.Theming.Constants;
using FractalBlazor.Components.Layout.Theming;

namespace FractalBlazor.Components.Tests.Theming;

public sealed class ThemeScopeTests : BunitContext
{
    public ThemeScopeTests()
    {
        Services.AddFractalBlazorTheming();
    }

    [Fact]
    public void Global_theme_writes_a_style_block_on_the_selector()
    {
        var cut = Render<FbTheme>(p => p.Add(x => x.Branch, FbThemeBranches.Light));

        var style = cut.Find("style").TextContent;
        Assert.StartsWith(":root{", style);
        Assert.Contains("--fb-default-bg-low-anchor:", style);
        Assert.Contains("--fb-bg-low-anchor:var(--fb-default-bg-low-anchor);", style);
    }

    [Fact]
    public void Scoped_theme_carries_its_variables_inline_on_a_neutral_div()
    {
        var cut = Render<FbTheme>(p => p
            .Add(x => x.Branch, FbThemeBranches.Light)
            .Add(x => x.Scoped, true)
            .AddChildContent("<span class=\"inside\"></span>"));

        Assert.Empty(cut.FindAll("style"));
        var scope = cut.Find("div.fb-theme-scope.fb-theme");
        Assert.Contains("--fb-default-bg-low-anchor:", scope.GetAttribute("style"));
        Assert.NotNull(scope.QuerySelector(".inside"));
    }

    [Fact]
    public void Dark_and_light_islands_resolve_different_anchors()
    {
        var dark = Render<FbTheme>(p => p.Add(x => x.Branch, FbThemeBranches.Dark).Add(x => x.Scoped, true));
        var light = Render<FbTheme>(p => p.Add(x => x.Branch, FbThemeBranches.Light).Add(x => x.Scoped, true));

        Assert.NotEqual(
            dark.Find("div").GetAttribute("style"),
            light.Find("div").GetAttribute("style"));
    }

    [Theory]
    [InlineData(FbThemeBranches.Dark, "color-scheme:dark;")]
    [InlineData(FbThemeBranches.Light, "color-scheme:light;")]
    public void Theme_declares_the_color_scheme_of_its_page_background(string branch, string expected)
    {
        var cut = Render<FbTheme>(p => p.Add(x => x.Branch, branch));

        Assert.Contains(expected, cut.Find("style").TextContent);
    }

    [Fact]
    public void Scoped_layout_theme_carries_its_variables_inline()
    {
        var cut = Render<FbLayoutTheme>(p => p.Add(x => x.Scoped, true));

        Assert.Empty(cut.FindAll("style"));
        Assert.Contains("--fb-default-bg-low-anchor:", cut.Find("div.fb-theme-scope").GetAttribute("style"));
    }
}
