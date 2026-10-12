using FractalBlazor.Components.Forms.Theming.Constants;
using FractalBlazor.Components.Forms.Theming.Helpers;
using FractalBlazor.Components.Forms.Theming.Model;
using FractalBlazor.Components.Forms.Theming.Registry;
using FractalBlazor.Components.Layout.Theming.Helpers;
using FractalBlazor.Components.Layout.Theming.Model;
using FractalBlazor.Components.Layout.Theming.Registry;

namespace FractalBlazor.Components.Tests.Theming;

public sealed class HoverOffsetTests
{
    private readonly FbFormThemeRegistry _registry = new();

    [Theory]
    [InlineData(FbThemeBranches.Dark)]
    [InlineData(FbThemeBranches.Light)]
    public void Default_theme_offsets_hovered_surfaces_by_four_percent(string branch)
    {
        var theme = _registry.Resolve(_registry.Default.Name, branch);

        Assert.Equal("4%", theme.SurfaceMix.HoverOffset);
        Assert.Contains("--fb-bg-hover-offset:4%;", FbThemeCssWriter.ToCssVariables(theme));
    }

    [Fact]
    public void A_branch_sets_its_own_hover_offset()
    {
        var edited = new FbThemeSetup("Edited", _registry.Default)
        {
            Branches =
            [
                new FbThemeBranch(FbThemeBranches.Dark)
                {
                    SurfaceMix = new FbThemeLayoutSurfaceMix { HoverOffset = "10%" }
                }
            ]
        };

        var dark = _registry.Resolve(edited, FbThemeBranches.Dark);
        var light = _registry.Resolve(edited, FbThemeBranches.Light);

        Assert.Equal("10%", dark.SurfaceMix.HoverOffset);
        Assert.Equal("8%", dark.SurfaceMix.SurfaceMix);
        Assert.Equal("4%", light.SurfaceMix.HoverOffset);
    }

    [Fact]
    public void Layout_theme_writes_its_hover_offset()
    {
        var theme = new FbLayoutThemeRegistry().Resolve("Default");

        Assert.Equal("4%", theme.SurfaceMix.HoverOffset);
        Assert.Contains("--fb-bg-hover-offset:4%;", FbLayoutThemeCssWriter.ToCssVariables(theme));
    }
}
