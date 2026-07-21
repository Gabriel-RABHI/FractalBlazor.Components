using FractalBlazor.Components.Layout.Theming.Contracts;
using FractalBlazor.Components.Layout.Theming.Model;

namespace FractalBlazor.Components.Layout.Theming.Helpers;

public static class FbLayoutThemeDefaults
{
    public static FbLayoutThemeSetup Create()
    {
        var defaultBorders = new FbThemeLayoutBorders
        {
            LightMix = "8%",
            LightSize = "0.0625rem",
            MediumMix = "14%",
            MediumSize = "0.0625rem",
            StrongMix = "22%",
            StrongSize = "0.125rem"
        };

        return new FbLayoutThemeSetup("Default")
        {
            Spacings = FbThemeLayoutSpacings.Default,
            Corners = FbThemeLayoutCorners.Default,
            Variants =
            [
                new(FbLayoutThemeVariants.Default)
                {
                    LayoutColors = Colors("#111113", "#34343a", "#f7f7f8"),
                    Borders = defaultBorders
                },
                new(FbLayoutThemeVariants.Selected)
                {
                    LayoutColors = Colors("#0d1c31", "#214d82", "#eef6ff", "10%", "16%", "26%")
                },
                new(FbLayoutThemeVariants.Focused)
                {
                    LayoutColors = Colors("#0b1d33", "#1f5f9f", "#f0f8ff", "10%", "18%", "28%")
                },
                new(FbLayoutThemeVariants.Error)
                {
                    LayoutColors = Colors("#2a0f14", "#5c1c28", "#fff5f6", "10%", "12%", "22%"),
                    Borders = new FbThemeLayoutBorders { LightMix = "14%", MediumMix = "24%", StrongMix = "38%" }
                },
                new(FbLayoutThemeVariants.Warning)
                {
                    LayoutColors = Colors("#2a1d08", "#6a4a10", "#fff8e1", "10%", "14%", "24%")
                },
                new(FbLayoutThemeVariants.Disabled)
                {
                    LayoutColors = Colors("#151517", "#303036", "#a5a5ad", "6%", "6%", "10%"),
                    Borders = new FbThemeLayoutBorders { LightMix = "6%", MediumMix = "9%", StrongMix = "14%" }
                },
                new(FbLayoutThemeVariants.Success)
                {
                    LayoutColors = Colors("#0e2418", "#1d5a38", "#ecfff4", "10%", "12%", "22%")
                },
                new(FbLayoutThemeVariants.Info)
                {
                    LayoutColors = Colors("#0b2030", "#174f73", "#eef9ff", "10%", "12%", "22%")
                }
            ]
        };
    }

    private static FbThemeLayoutColors Colors(
        string low,
        string tint,
        string high,
        string surface = "8%",
        string accent = "10%",
        string highlight = "18%")
        => new()
        {
            LowAnchor = low,
            Tint = tint,
            HighAnchor = high,
            SurfaceMix = surface,
            AccentOffset = accent,
            HighlightOffset = highlight
        };
}
