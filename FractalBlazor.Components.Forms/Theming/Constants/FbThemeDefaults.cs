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
                CreateDarkBranch(),
                CreateLightBranch()
            ]
        };

    private static FbThemeMasterTint CreateMasterTint()
        => new()
        {
            ColorTint = FbThemeBaseColors.GetColor(FbThemeBaseColorsIndex.Sky),
            TintPercent = 15
        };

    private static FbThemeLayoutSurfaceMix CreateSurfaceMix()
        => new()
        {
            SurfaceMix = "8%",
            AccentOffset = "10%",
            HighlightOffset = "18%"
        };

    /// <summary>
    /// Tuned in the theme editor: stronger frames, a wider gap between text levels, and
    /// hand-picked anchors for every variant except Default, which stays on the palette.
    /// </summary>
    private static FbThemeBranch CreateDarkBranch()
    {
        var masterTint = CreateMasterTint();

        return new FbThemeBranch(FbThemeBranches.Dark)
        {
            MasterTint = masterTint,
            TextVariantMix = new FbThemeFormTextVariantMix
            {
                DefaultHighMix = "75%",
                SubtleHighMix = "12%",
                MutedHighMix = "30%",
                HighlightHighMix = "100%"
            },
            BordersMix = new FbThemeLayoutBordersMix
            {
                LightMix = "15%",
                MediumMix = "24%",
                StrongMix = "50%"
            },
            SurfaceMix = CreateSurfaceMix(),
            Variants =
            [
                Variant(FbThemeVariants.Default, FbThemeBaseColorsIndex.Zinc, light: false, masterTint),
                Variant(FbThemeVariants.Selected, "#1d1683", "#0070ff", "#0070ff", "#e5f2ff"),
                Variant(FbThemeVariants.Error, "#5e1313", "#fb0404", "#a02222", "#febebe"),
                Variant(FbThemeVariants.Warning, "#4d4a00", "#ff8800", "#ff7300", "#d2b5a3"),
                Variant(FbThemeVariants.Disabled, "#030916", "#44536a", "#21334f", "#7a8590"),
                Variant(FbThemeVariants.Success, "#0b603f", "#00ff7b", "#00ff55", "#c2ffe8"),
                Variant(FbThemeVariants.Info, "#044d71", "#009dff", "#00adff", "#95a6fe")
            ]
        };
    }

    private static FbThemeBranch CreateLightBranch()
    {
        var masterTint = CreateMasterTint();

        return new FbThemeBranch(FbThemeBranches.Light)
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
            SurfaceMix = CreateSurfaceMix(),
            Variants =
            [
                Variant(FbThemeVariants.Default, FbThemeBaseColorsIndex.Zinc, light: true, masterTint),
                Variant(FbThemeVariants.Selected, FbThemeBaseColorsIndex.Blue, light: true, masterTint),
                Variant(FbThemeVariants.Error, FbThemeBaseColorsIndex.Red, light: true, masterTint),
                Variant(FbThemeVariants.Warning, FbThemeBaseColorsIndex.Amber, light: true, masterTint),
                Variant(FbThemeVariants.Disabled, FbThemeBaseColorsIndex.Gray, light: true, masterTint),
                Variant(FbThemeVariants.Success, FbThemeBaseColorsIndex.Emerald, light: true, masterTint),
                Variant(FbThemeVariants.Info, FbThemeBaseColorsIndex.Sky, light: true, masterTint)
            ]
        };
    }

    private static FbThemeColorVariant Variant(
        string name,
        string layoutLow,
        string layoutHigh,
        string formLow,
        string formHigh)
        => new(name)
        {
            LayoutColors = new FbThemeLayoutColors { LowAnchor = layoutLow, HighAnchor = layoutHigh },
            FormColors = new FbThemeFormColors { LowAnchor = formLow, HighAnchor = formHigh }
        };

    private static FbThemeColorVariant Variant(
        string name,
        FbThemeBaseColorsIndex color,
        bool light,
        FbThemeMasterTint masterTint)
    {
        var lighter = FbThemeBaseShadesIndex._50;
        var medium = FbThemeBaseShadesIndex._500;
        var darker = FbThemeBaseShadesIndex._950;
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
