using FractalBlazor.Components.Forms.Theming.Model;
using FractalBlazor.Components.Layout.Colors;
using FractalBlazor.Components.Layout.Theming.Model;

namespace FractalBlazor.Components.Forms.Theming.Constants;

/// <summary>
/// The default theme, tuned in the theme editor: low surfaces with strong accent and
/// highlight offsets, a marked hover, and hand-picked background, text and frame anchors for
/// every variant of both branches.
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
            SurfaceMix = "3%",
            AccentOffset = "30%",
            HighlightOffset = "85%",
            HoverOffset = "15%"
        };

    private static FbThemeBranch CreateDarkBranch()
        => new(FbThemeBranches.Dark)
        {
            MasterTint = CreateMasterTint(),
            TextVariantMix = new FbThemeFormTextVariantMix
            {
                DefaultHighMix = "75%",
                SubtleHighMix = "20%",
                MutedHighMix = "35%",
                HighlightHighMix = "100%"
            },
            BordersMix = new FbThemeLayoutBordersMix
            {
                LightMix = "8%",
                MediumMix = "20%",
                StrongMix = "45%"
            },
            SurfaceMix = CreateSurfaceMix(),
            Variants =
            [
                Variant(FbThemeVariants.Default, "#15151e", "#292a2e", "#303240", "#f0f3f4", "#1b1b27", "#686d97"),
                Variant(FbThemeVariants.Selected, "#0d0c1d", "#163cfe", "#2448ff", "#e5f2ff", "#141051", "#395afe"),
                Variant(FbThemeVariants.Error, "#270707", "#fb0404", "#a02222", "#febebe", "#330a0a", "#fb0404"),
                Variant(FbThemeVariants.Warning, "#1d1002", "#ff8800", "#ff7300", "#d2b5a3", "#2b1803", "#ff8800"),
                Variant(FbThemeVariants.Disabled, "#191e29", "#242a33", "#2d394e", "#4f5963", "#191e29", "#3d4552"),
                Variant(FbThemeVariants.Success, "#062d1e", "#03c700", "#66ff00", "#c2ffe8", "#084f34", "#03c700"),
                Variant(FbThemeVariants.Info, "#02122c", "#009dff", "#00adff", "#90ddfe", "#092b62", "#009dff")
            ]
        };

    private static FbThemeBranch CreateLightBranch()
        => new(FbThemeBranches.Light)
        {
            MasterTint = CreateMasterTint(),
            TextVariantMix = new FbThemeFormTextVariantMix
            {
                DefaultHighMix = "75%",
                SubtleHighMix = "30%",
                MutedHighMix = "50%",
                HighlightHighMix = "100%"
            },
            BordersMix = new FbThemeLayoutBordersMix
            {
                LightMix = "8%",
                MediumMix = "20%",
                StrongMix = "45%"
            },
            SurfaceMix = CreateSurfaceMix(),
            Variants =
            [
                Variant(FbThemeVariants.Default, "#ffffff", "#e2e3e9", "#deedf2", "#1d1d2f", "#e3e3e3", "#484a61"),
                Variant(FbThemeVariants.Selected, "#e5f2ff", "#526eff", "#9ecfff", "#0a1b43", "#e5f2ff", "#002aff"),
                Variant(FbThemeVariants.Error, "#fde7e7", "#fb0404", "#fde7f0", "#480505", "#fde7e7", "#fb0404"),
                Variant(FbThemeVariants.Warning, "#fff4e5", "#ffa200", "#fff9e5", "#331600", "#fee5c3", "#d18400"),
                Variant(FbThemeVariants.Disabled, "#eef2f6", "#b5bbc4", "#eef2f6", "#848994", "#ebebeb", "#6a717c"),
                Variant(FbThemeVariants.Success, "#e8fdec", "#00ff4c", "#e8fdeb", "#001916", "#e8fdec", "#00992e"),
                Variant(FbThemeVariants.Info, "#e5f5ff", "#00adff", "#e5f5ff", "#010f18", "#e5f5ff", "#00adff")
            ]
        };

    private static FbThemeColorVariant Variant(
        string name,
        string layoutLow,
        string layoutHigh,
        string formLow,
        string formHigh,
        string frameLow,
        string frameHigh)
        => new(name)
        {
            LayoutColors = new FbThemeLayoutColors { LowAnchor = layoutLow, HighAnchor = layoutHigh },
            FormColors = new FbThemeFormColors { LowAnchor = formLow, HighAnchor = formHigh },
            FrameColors = new FbThemeFormColors { LowAnchor = frameLow, HighAnchor = frameHigh }
        };
}
