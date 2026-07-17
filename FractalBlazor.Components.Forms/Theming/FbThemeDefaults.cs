using FractalBlazor.Components.Layout;

namespace FractalBlazor.Components.Forms.Theming;

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
            TextVariant = new FbThemeFormTextVariant
            {
                DefaultHighMix = "82%",
                SubtleHighMix = "46%",
                MutedHighMix = "64%",
                HighlightHighMix = "100%"
            },
            Variants = CreateVariants(light)
        };

    private static IReadOnlyList<FbThemeVariant> CreateVariants(bool light)
    {
        var borders = new FbThemeLayoutBorders
        {
            LightMix = "8%",
            LightSize = "0.0625rem",
            MediumMix = "14%",
            MediumSize = "0.0625rem",
            StrongMix = "22%",
            StrongSize = "0.125rem"
        };

        return light
            ?
            [
                Variant(FbThemeVariants.Default, "#f7f7f8", "#d8dae0", "#111113", "#f7f7f8", "#111113", borders, "8%", "8%", "16%"),
                Variant(FbThemeVariants.Selected, "#eef6ff", "#bddcff", "#164f86", "#eef6ff", "#164f86", accent: "12%", highlight: "22%"),
                Variant(FbThemeVariants.Focused, "#f0f8ff", "#b9ddff", "#0f5f9f", "#f0f8ff", "#0f5f9f", accent: "14%", highlight: "24%"),
                Variant(FbThemeVariants.Error, "#fff4f5", "#ffd9dd", "#701824", "#fff4f5", "#701824", new FbThemeLayoutBorders { LightMix = "14%", MediumMix = "24%", StrongMix = "38%" }, "10%", "12%", "22%"),
                Variant(FbThemeVariants.Warning, "#fff8e1", "#ffe3a3", "#6a4300", "#fff8e1", "#6a4300", accent: "14%", highlight: "24%"),
                Variant(FbThemeVariants.Disabled, "#f1f1f3", "#dddddf", "#68686f", "#f1f1f3", "#68686f", new FbThemeLayoutBorders { LightMix = "6%", MediumMix = "9%", StrongMix = "14%" }, "6%", "6%", "10%"),
                Variant(FbThemeVariants.Success, "#effbf4", "#c7efd7", "#175c35", "#effbf4", "#175c35", accent: "12%", highlight: "22%"),
                Variant(FbThemeVariants.Info, "#eef9ff", "#c9eaff", "#175776", "#eef9ff", "#175776", accent: "12%", highlight: "22%")
            ]
            :
            [
                Variant(FbThemeVariants.Default, "#111113", "#34343a", "#f7f7f8", "#111113", "#f7f7f8", borders),
                Variant(FbThemeVariants.Selected, "#0d1c31", "#214d82", "#eef6ff", "#0d1c31", "#eef6ff", accent: "16%", highlight: "26%"),
                Variant(FbThemeVariants.Focused, "#0b1d33", "#1f5f9f", "#f0f8ff", "#0b1d33", "#f0f8ff", accent: "18%", highlight: "28%"),
                Variant(FbThemeVariants.Error, "#2a0f14", "#5c1c28", "#fff5f6", "#2a0f14", "#fff5f6", new FbThemeLayoutBorders { LightMix = "14%", MediumMix = "24%", StrongMix = "38%" }, "10%", "12%", "22%"),
                Variant(FbThemeVariants.Warning, "#2a1d08", "#6a4a10", "#fff8e1", "#2a1d08", "#fff8e1", accent: "14%", highlight: "24%"),
                Variant(FbThemeVariants.Disabled, "#151517", "#303036", "#a5a5ad", "#151517", "#a5a5ad", new FbThemeLayoutBorders { LightMix = "6%", MediumMix = "9%", StrongMix = "14%" }, "6%", "6%", "10%"),
                Variant(FbThemeVariants.Success, "#0e2418", "#1d5a38", "#ecfff4", "#0e2418", "#ecfff4", accent: "12%", highlight: "22%"),
                Variant(FbThemeVariants.Info, "#0b2030", "#174f73", "#eef9ff", "#0b2030", "#eef9ff", accent: "12%", highlight: "22%")
            ];
    }

    private static FbThemeVariant Variant(
        string name,
        string backgroundLow,
        string tint,
        string backgroundHigh,
        string foregroundLow,
        string foregroundHigh,
        FbThemeLayoutBorders? borders = null,
        string surface = "8%",
        string accent = "10%",
        string highlight = "18%")
        => new(name)
        {
            LayoutColors = new FbThemeLayoutColors
            {
                LowAnchor = backgroundLow,
                Tint = tint,
                HighAnchor = backgroundHigh,
                SurfaceMix = surface,
                AccentOffset = accent,
                HighlightOffset = highlight
            },
            FormColors = new FbThemeFormColors
            {
                LowAnchor = foregroundLow,
                HighAnchor = foregroundHigh
            },
            Borders = borders
        };
}
