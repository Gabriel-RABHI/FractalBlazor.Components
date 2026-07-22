using FractalBlazor.Components.Forms.Theming.Model;
using FractalBlazor.Components.Layout.Colors;
using FractalBlazor.Components.Layout.Theming.Model;

namespace FractalBlazor.Components.Forms.Theming.Constants;

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
    {
        var masterTint = new FbThemeMasterTint
        {
            ColorTint = FbThemeBaseColors.GetColor(FbThemeBaseColorsIndex.Sky),
            TintPercent = light ? 3 : 2
        };

        return new FbThemeBranch(name)
        {
            MasterTint = masterTint,
            TextVariantMix = new FbThemeFormTextVariantMix
            {
                DefaultHighMix = "82%",
                SubtleHighMix = "46%",
                MutedHighMix = "64%",
                HighlightHighMix = "100%"
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
                HighlightOffset = "18%"
            },
            Variants = CreateVariants(light, masterTint)
        };
    }

    private static IReadOnlyList<FbThemeColorVariant> CreateVariants(bool light, FbThemeMasterTint masterTint)
        =>
        [
            Variant(FbThemeVariants.Default, FbThemeBaseColorsIndex.Zinc, light, masterTint),
            Variant(FbThemeVariants.Selected, FbThemeBaseColorsIndex.Blue, light, masterTint),
            Variant(FbThemeVariants.Error, FbThemeBaseColorsIndex.Red, light, masterTint),
            Variant(FbThemeVariants.Warning, FbThemeBaseColorsIndex.Amber, light, masterTint),
            Variant(FbThemeVariants.Disabled, FbThemeBaseColorsIndex.Gray, light, masterTint),
            Variant(FbThemeVariants.Success, FbThemeBaseColorsIndex.Emerald, light, masterTint),
            Variant(FbThemeVariants.Info, FbThemeBaseColorsIndex.Sky, light, masterTint)
        ];

    private static FbThemeColorVariant Variant(
        string name,
        FbThemeBaseColorsIndex color,
        bool light,
        FbThemeMasterTint masterTint)
    {
        var lighter = color <= FbThemeBaseColorsIndex.Slate ? FbThemeBaseShadesIndex._200 : FbThemeBaseShadesIndex._50;
        var medium = color <= FbThemeBaseColorsIndex.Slate ? FbThemeBaseShadesIndex._600 : FbThemeBaseShadesIndex._400;
        var darker = color <= FbThemeBaseColorsIndex.Slate ? FbThemeBaseShadesIndex._900 : FbThemeBaseShadesIndex._950;
        // ------------------------ LIGHT ----- DARK -------------------------- //
        // BACKGROUND
        var lowShadeBg =    light ? lighter :   darker;
        var highShadeBg =   light ? medium :    medium;
        // FOREGROUD
        var lowShadeFg =    light ? lighter :    medium;
        var highShadeFg =   light ? darker :    lighter;

        var lowBg = FbThemeBaseColors.GetColor(color, lowShadeBg, masterTint);
        var highBg = FbThemeBaseColors.GetColor(color, highShadeBg, masterTint);
        var lowFg = FbThemeBaseColors.GetColor(color, lowShadeFg, masterTint);
        var highFg = FbThemeBaseColors.GetColor(color, highShadeFg, masterTint);

        return new FbThemeColorVariant(name)
        {
            LayoutColors = new FbThemeLayoutColors
            {
                LowAnchor = lowBg,
                HighAnchor = highBg
            },
            FormColors = new FbThemeFormColors
            {
                LowAnchor = lowFg,
                HighAnchor = highFg
            }
        };
    }
}
