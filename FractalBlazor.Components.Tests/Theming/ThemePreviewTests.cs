using FractalBlazor.Components.Forms.Contracts;
using FractalBlazor.Components.Forms.Theming;
using FractalBlazor.Components.Forms.Theming.Constants;
using FractalBlazor.Components.Forms.Theming.Model;
using FractalBlazor.Components.Layout.Theming.Model;
using Microsoft.Extensions.DependencyInjection;

namespace FractalBlazor.Components.Tests.Theming;

public sealed class ThemePreviewTests : BunitContext
{
    private readonly IFbFormThemeRegistry _registry;

    public ThemePreviewTests()
    {
        Services.AddFractalBlazorTheming();
        _registry = Services.GetRequiredService<IFbFormThemeRegistry>();
    }

    private FbThemeSetup Edited(string surfaceOffset) => new("Edited", _registry.Default)
    {
        Branches =
        [
            new FbThemeBranch(FbThemeBranches.Dark)
            {
                SurfaceMix = new FbThemeLayoutSurfaceMix { SurfaceOffset = surfaceOffset }
            }
        ]
    };

    [Fact]
    public void Unregistered_theme_resolves_with_inheritance_and_stays_unregistered()
    {
        var resolved = _registry.Resolve(Edited("23%"), FbThemeBranches.Dark);

        Assert.Equal("23%", resolved.SurfaceMix.SurfaceOffset);
        Assert.Equal(_registry.Resolve(_registry.Default.Name, FbThemeBranches.Dark).SurfaceMix.AccentOffset, resolved.SurfaceMix.AccentOffset);
        Assert.Equal(FbThemeVariants.All.Count, resolved.VariantOrder.Count);
        Assert.False(_registry.TryGet("Edited", out _));
    }

    [Fact]
    public void Theme_whose_parent_is_not_registered_is_rejected()
    {
        var orphan = new FbThemeSetup("Orphan", new FbThemeSetup("Unknown"));

        Assert.Throws<InvalidOperationException>(() => _registry.Resolve(orphan, FbThemeBranches.Dark));
    }

    [Fact]
    public void Theme_component_applies_the_unregistered_setup()
    {
        var cut = Render<FbTheme>(p => p.Add(x => x.Setup, Edited("23%")).Add(x => x.Scoped, true));

        Assert.Contains("--fb-surface-offset:23%;", cut.Find("div.fb-theme-scope").GetAttribute("style"));
    }

    [Fact]
    public void A_new_setup_instance_updates_the_variables()
    {
        var cut = Render<FbTheme>(p => p.Add(x => x.Setup, Edited("23%")).Add(x => x.Scoped, true));

        cut.Render(p => p.Add(x => x.Setup, Edited("31%")).Add(x => x.Scoped, true));

        Assert.Contains("--fb-surface-offset:31%;", cut.Find("div.fb-theme-scope").GetAttribute("style"));
    }
}
