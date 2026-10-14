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
                StrongMix = "20%"
            },
            SurfaceMix = new FbThemeLayoutSurfaceMix
            {
                NeutralMix = "0%",
                SurfaceOffset = "2%",
                AccentOffset = "6%",
                HighlightOffset = "35%",
                HoverOffset = "10%"
            },
            Variants =
            [
                Variant(FbLayoutThemeVariants.Default, FbThemeBaseColorsIndex.Zinc),
                DarkVariant(FbLayoutThemeVariants.Selected, FbThemeBaseColorsIndex.Blue),
                DarkVariant(FbLayoutThemeVariants.Error, FbThemeBaseColorsIndex.Red),
                DarkVariant(FbLayoutThemeVariants.Warning, FbThemeBaseColorsIndex.Orange),
                Variant(FbLayoutThemeVariants.Disabled, FbThemeBaseColorsIndex.Gray,
                    FbThemeBaseShadesIndex._950, FbThemeBaseShadesIndex._700, FbThemeBaseShadesIndex._500),
                DarkVariant(FbLayoutThemeVariants.Success, FbThemeBaseColorsIndex.Green),
                DarkVariant(FbLayoutThemeVariants.Info, FbThemeBaseColorsIndex.Sky)
            ]
        };

    private static FbLayoutThemeVariant Variant(string name, FbThemeBaseColorsIndex color)
        => Variant(name, color, FbThemeBaseShadesIndex._950, FbThemeBaseShadesIndex._500, FbThemeBaseShadesIndex._50);

    private static FbLayoutThemeVariant DarkVariant(string name, FbThemeBaseColorsIndex color)
        => Variant(name, color, FbThemeBaseShadesIndex._990, FbThemeBaseShadesIndex._500, FbThemeBaseShadesIndex._50);

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
