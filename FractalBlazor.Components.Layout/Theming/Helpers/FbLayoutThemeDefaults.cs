using FractalBlazor.Components.Layout.Colors;
using FractalBlazor.Components.Layout.Theming.Contracts;
using FractalBlazor.Components.Layout.Theming.Model;

namespace FractalBlazor.Components.Layout.Theming.Helpers;

public static class FbLayoutThemeDefaults
{
    /// <summary>
    /// The default Layout theme: the anchors of each variant come from the base palette, and
    /// the master tint draws them all slightly toward sky blue.
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
                LightMix = "8%",
                MediumMix = "14%",
                StrongMix = "28%"
            },
            SurfaceMix = new FbThemeLayoutSurfaceMix
            {
                SurfaceMix = "8%",
                AccentOffset = "10%",
                HighlightOffset = "18%",
                HoverOffset = "4%"
            },
            Variants =
            [
                Variant(FbLayoutThemeVariants.Default, FbThemeBaseColorsIndex.Zinc),
                Variant(FbLayoutThemeVariants.Selected, FbThemeBaseColorsIndex.Blue),
                Variant(FbLayoutThemeVariants.Error, FbThemeBaseColorsIndex.Rose),
                Variant(FbLayoutThemeVariants.Warning, FbThemeBaseColorsIndex.Amber),
                Variant(FbLayoutThemeVariants.Disabled, FbThemeBaseColorsIndex.Gray),
                Variant(FbLayoutThemeVariants.Success, FbThemeBaseColorsIndex.Emerald),
                Variant(FbLayoutThemeVariants.Info, FbThemeBaseColorsIndex.Sky)
            ]
        };

    private static FbLayoutThemeVariant Variant(string name, FbThemeBaseColorsIndex color)
        => new(name)
        {
            LayoutColors = new FbThemeLayoutColors
            {
                LowAnchor = FbThemeBaseColors.GetColor(color, FbThemeBaseShadesIndex._950),
                HighAnchor = FbThemeBaseColors.GetColor(color, FbThemeBaseShadesIndex._50)
            }
        };
}
