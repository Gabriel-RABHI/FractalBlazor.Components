using FractalBlazor.Components.Forms.Theming.Model;
using FractalBlazor.Components.Layout.Colors;
using FractalBlazor.Components.Layout.Theming.Helpers;
using FractalBlazor.Components.Layout.Theming.Model;

namespace FractalBlazor.Components.Forms.Theming.Constants;

/// <summary>
/// The default theme: the gradient of every variant comes from the base palette, shades 950,
/// 500 and 50 in the dark branch and the other way round in the light one. Disabled keeps a
/// short gradient, 950 / 700 / 500, so that its texts stay dim.
/// </summary>
public static class FbThemeDefaults
{
    public static FbThemeSetup Create()
        => new("Default")
        {
            Spacings = FbThemeLayoutSpacings.Default,
            Corners = FbThemeLayoutCorners.Default,
            Typography = FbThemeFormTypography.Default,
            Branches =
            [
                CreateBranch(FbThemeBranches.Dark, light: false),
                CreateBranch(FbThemeBranches.Light, light: true)
            ]
        };

    private static FbThemeBranch CreateBranch(string name, bool light)
        => new(name)
        {
            MasterTint = new FbThemeMasterTint
            {
                ColorTint = FbThemeBaseColors.GetColor(FbThemeBaseColorsIndex.Sky),
                TintMix = "5%"
            },
            SurfaceMix = FbLayoutThemeDefaults.Create().SurfaceMix,
            BordersMix = FbLayoutThemeDefaults.Create().BordersMix,
            TextVariantMix = new FbThemeFormTextVariantMix
            {
                SubtleHighMix = "40%",
                MutedHighMix = "60%",
                DefaultHighMix = "80%",
                HighlightHighMix = "95%"
            },
            Variants =
            [
                Variant(FbThemeVariants.Default, FbThemeBaseColorsIndex.Zinc, light),
                DarkVariant(FbThemeVariants.Selected, FbThemeBaseColorsIndex.Blue, light),
                DarkVariant(FbThemeVariants.Error, FbThemeBaseColorsIndex.Red, light),
                DarkVariant(FbThemeVariants.Warning, FbThemeBaseColorsIndex.Orange, light),
                Variant(FbThemeVariants.Disabled, FbThemeBaseColorsIndex.Gray, light,
                    FbThemeBaseShadesIndex._950, FbThemeBaseShadesIndex._700, FbThemeBaseShadesIndex._500),
                DarkVariant(FbThemeVariants.Success, FbThemeBaseColorsIndex.Green, light),
                DarkVariant(FbThemeVariants.Info, FbThemeBaseColorsIndex.Sky, light)
            ]
        };

    private static FbThemeColorVariant Variant(string name, FbThemeBaseColorsIndex color, bool light)
        => Variant(name, color, light, FbThemeBaseShadesIndex._950, FbThemeBaseShadesIndex._500, FbThemeBaseShadesIndex._50);

    private static FbThemeColorVariant DarkVariant(string name, FbThemeBaseColorsIndex color, bool light)
        => Variant(name, color, light, FbThemeBaseShadesIndex._990, FbThemeBaseShadesIndex._500, FbThemeBaseShadesIndex._50);

    /// <summary>
    /// A gradient from <paramref name="dark"/> to <paramref name="bright"/> in the dark branch,
    /// reversed in the light one: the light branch reads the shades from the other end of the
    /// palette.
    /// </summary>
    private static FbThemeColorVariant Variant(string name, FbThemeBaseColorsIndex color, bool light,
        FbThemeBaseShadesIndex dark, FbThemeBaseShadesIndex mid, FbThemeBaseShadesIndex bright)
        => new(name)
        {
            LayoutColors = new FbThemeLayoutColors
            {
                LowAnchor = FbThemeBaseColors.GetColor(color, light ? Mirror(dark) : dark),
                MidAnchor = FbThemeBaseColors.GetColor(color, light ? Mirror(mid) : mid),
                HighAnchor = FbThemeBaseColors.GetColor(color, light ? Mirror(bright) : bright)
            }
        };

    /// <summary>
    /// The shade at the same distance from the other end of the palette: 950 gives 50, 700
    /// gives 300, 500 stays. 980 and 990 have no light counterpart: they give 50, the lightest.
    /// </summary>
    private static FbThemeBaseShadesIndex Mirror(FbThemeBaseShadesIndex shade)
        => (FbThemeBaseShadesIndex)Math.Min(
            (int)FbThemeBaseShadesIndex._950 + (int)FbThemeBaseShadesIndex._50 - (int)shade,
            (int)FbThemeBaseShadesIndex._50);
}
