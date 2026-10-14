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
    public void Default_theme_writes_the_hover_offset_of_each_branch(string branch)
    {
        var theme = _registry.Resolve(_registry.Default.Name, branch);

        Assert.False(string.IsNullOrWhiteSpace(theme.SurfaceMix.HoverOffset));
        Assert.Contains($"--fb-hover-offset:{theme.SurfaceMix.HoverOffset};", FbThemeCssWriter.ToCssVariables(theme));
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
                    SurfaceMix = new FbThemeLayoutSurfaceMix { HoverOffset = "27%" }
                }
            ]
        };
        var defaultDark = _registry.Resolve(_registry.Default.Name, FbThemeBranches.Dark);
        var defaultLight = _registry.Resolve(_registry.Default.Name, FbThemeBranches.Light);

        var dark = _registry.Resolve(edited, FbThemeBranches.Dark);
        var light = _registry.Resolve(edited, FbThemeBranches.Light);

        Assert.Equal("27%", dark.SurfaceMix.HoverOffset);
        Assert.Equal(defaultDark.SurfaceMix.SurfaceOffset, dark.SurfaceMix.SurfaceOffset);
        Assert.Equal(defaultLight.SurfaceMix.HoverOffset, light.SurfaceMix.HoverOffset);
    }

    [Fact]
    public void Layout_theme_writes_its_hover_offset()
    {
        var theme = new FbLayoutThemeRegistry().Resolve("Default");

        Assert.Equal("10%", theme.SurfaceMix.HoverOffset);
        Assert.Contains("--fb-hover-offset:10%;", FbLayoutThemeCssWriter.ToCssVariables(theme));
    }
}
