using FractalBlazor.Components.Layout.Colors;
using FractalBlazor.Components.Layout.Theming.Contracts;
using FractalBlazor.Components.Layout.Theming.Model;

namespace FractalBlazor.Components.Layout.Theming.Helpers;

public static class FbLayoutThemeDefaults
{
    /// <summary>
    /// The default Layout theme: the gradient of each variant comes from the base palette, dark
    /// at 0 % and light at 100 %, and the master tint draws all the anchors slightly toward sky
    /// blue.
    /// </summary>
    public static FbLayoutThemeSetup Create()
        => new("Default")
        {
            Spacings = FbThemeLayoutSpacings.Default,
            Corners = FbThemeLayoutCorners.Default,
            MasterTint = new FbThemeMasterTint
            {
                ColorTint = FbThemeBaseColors.GetColor(FbThemeBaseColorsIndex.Sky),
                TintMix = "5%"
            },
            BordersMix = new FbThemeLayoutBordersMix
            {
                LightMix = "5%",
                MediumMix = "10%",
                StrongMix = "15%"
            },
            SurfaceMix = new FbThemeLayoutSurfaceMix
            {
                NeutralMix = "0%",
                SurfaceOffset = "5%",
                AccentOffset = "15%",
                HighlightOffset = "45%",
                HoverOffset = "10%"
            },
            Variants =
            [
                Variant(FbLayoutThemeVariants.Default, FbThemeBaseColorsIndex.Zinc),
                Variant(FbLayoutThemeVariants.Selected, FbThemeBaseColorsIndex.Blue),
                Variant(FbLayoutThemeVariants.Error, FbThemeBaseColorsIndex.Rose),
                Variant(FbLayoutThemeVariants.Warning, FbThemeBaseColorsIndex.Amber),
                Variant(FbLayoutThemeVariants.Disabled, FbThemeBaseColorsIndex.Gray,
                    FbThemeBaseShadesIndex._950, FbThemeBaseShadesIndex._700, FbThemeBaseShadesIndex._500),
                Variant(FbLayoutThemeVariants.Success, FbThemeBaseColorsIndex.Emerald),
                Variant(FbLayoutThemeVariants.Info, FbThemeBaseColorsIndex.Sky)
            ]
        };

    private static FbLayoutThemeVariant Variant(string name, FbThemeBaseColorsIndex color)
        => Variant(name, color, FbThemeBaseShadesIndex._950, FbThemeBaseShadesIndex._500, FbThemeBaseShadesIndex._50);

    private static FbLayoutThemeVariant Variant(string name, FbThemeBaseColorsIndex color,
        FbThemeBaseShadesIndex low, FbThemeBaseShadesIndex mid, FbThemeBaseShadesIndex high)
        => new(name)
        {
            LayoutColors = new FbThemeLayoutColors
            {
                LowAnchor = FbThemeBaseColors.GetColor(color, low),
                MidAnchor = FbThemeBaseColors.GetColor(color, mid),
                HighAnchor = FbThemeBaseColors.GetColor(color, high)
            }
        };
}
