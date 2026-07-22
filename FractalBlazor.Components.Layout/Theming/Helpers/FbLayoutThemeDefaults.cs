using FractalBlazor.Components.Layout.Colors;
using FractalBlazor.Components.Layout.Theming.Contracts;
using FractalBlazor.Components.Layout.Theming.Model;

namespace FractalBlazor.Components.Layout.Theming.Helpers;

public static class FbLayoutThemeDefaults
{
    public static FbLayoutThemeSetup Create()
    {
        var masterTint = new FbThemeMasterTint
        {
            ColorTint = FbThemeBaseColors.GetColor(FbThemeBaseColorsIndex.Sky),
            TintPercent = 5
        };

        return new FbLayoutThemeSetup("Default")
        {
            Spacings = FbThemeLayoutSpacings.Default,
            Corners = FbThemeLayoutCorners.Default,
            MasterTint = masterTint,
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
                HighlightOffset = "18%"
            },
            Variants =
            [
                Variant(FbLayoutThemeVariants.Default, FbThemeBaseColorsIndex.Zinc, masterTint),
                Variant(FbLayoutThemeVariants.Selected, FbThemeBaseColorsIndex.Blue, masterTint),
                Variant(FbLayoutThemeVariants.Error, FbThemeBaseColorsIndex.Rose, masterTint),
                Variant(FbLayoutThemeVariants.Warning, FbThemeBaseColorsIndex.Amber, masterTint),
                Variant(FbLayoutThemeVariants.Disabled, FbThemeBaseColorsIndex.Gray, masterTint),
                Variant(FbLayoutThemeVariants.Success, FbThemeBaseColorsIndex.Emerald, masterTint),
                Variant(FbLayoutThemeVariants.Info, FbThemeBaseColorsIndex.Sky, masterTint)
            ]
        };
    }

    private static FbLayoutThemeVariant Variant(
        string name,
        FbThemeBaseColorsIndex color,
        FbThemeMasterTint masterTint)
        => new(name)
        {
            LayoutColors = new FbThemeLayoutColors
            {
                LowAnchor = FbThemeBaseColors.GetColor(color, FbThemeBaseShadesIndex._950, masterTint),
                HighAnchor = FbThemeBaseColors.GetColor(color, FbThemeBaseShadesIndex._50, masterTint)
            }
        };
}
