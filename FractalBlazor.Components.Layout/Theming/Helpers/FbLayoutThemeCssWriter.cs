using FractalBlazor.Components.Layout.Theming.Contracts;
using FractalBlazor.Components.Layout.Theming.Model;
using FractalBlazor.Components.Layout.Theming.Solver;
using System.Text;

namespace FractalBlazor.Components.Layout.Theming.Helpers;

public static class FbLayoutThemeCssWriter
{
    public static string ToCssVariables(FbResolvedLayoutTheme theme)
    {
        ArgumentNullException.ThrowIfNull(theme);
        var builder = new StringBuilder();

        Append(builder, "--fb-default-space-s", theme.Spacings.S);
        Append(builder, "--fb-default-space-m", theme.Spacings.M);
        Append(builder, "--fb-default-space-l", theme.Spacings.L);
        Append(builder, "--fb-default-space-x", theme.Spacings.X);

        Append(builder, "--fb-default-radius-s", theme.Corners.S);
        Append(builder, "--fb-default-radius-m", theme.Corners.M);
        Append(builder, "--fb-default-radius-l", theme.Corners.L);
        Append(builder, "--fb-default-radius-x", theme.Corners.X);

        Append(builder, "--fb-bg-surface-mix", theme.SurfaceMix.SurfaceMix);
        Append(builder, "--fb-bg-accent-offset", theme.SurfaceMix.AccentOffset);
        Append(builder, "--fb-bg-highlight-offset", theme.SurfaceMix.HighlightOffset);
        Append(builder, "--fb-bg-hover-offset", theme.SurfaceMix.HoverOffset);

        Append(builder, "--fb-frame-light-mix", theme.BordersMix.LightMix);
        Append(builder, "--fb-frame-medium-mix", theme.BordersMix.MediumMix);
        Append(builder, "--fb-frame-strong-mix", theme.BordersMix.StrongMix);

        FbThemeCssNames.AppendColorScheme(builder, theme.GetVariant(FbLayoutThemeVariants.Default).LayoutColors.LowAnchor);

        FbThemeCssNames.AppendTint(builder, theme.MasterTint);
        foreach (var variantName in theme.VariantOrder)
            AppendVariant(builder, theme.GetVariant(variantName), theme.MasterTint);

        AppendReference(builder, "space-s", "default-space-s");
        AppendReference(builder, "space-m", "default-space-m");
        AppendReference(builder, "space-l", "default-space-l");
        AppendReference(builder, "space-x", "default-space-x");

        AppendReference(builder, "radius-s", "default-radius-s");
        AppendReference(builder, "radius-m", "default-radius-m");
        AppendReference(builder, "radius-l", "default-radius-l");
        AppendReference(builder, "radius-x", "default-radius-x");
        builder.Append(BuildVariantReferences(FbLayoutThemeVariants.Default));

        return builder.ToString();
    }

    /// <summary>
    /// Inline redirections that activate <paramref name="variant"/> on a DOM branch.
    /// <paramref name="invert"/> exchanges its low and high background anchors.
    /// </summary>
    public static string BuildVariantReferences(string? variant, bool invert = false)
        => FbThemeCssNames.VariantReferences(
            FbThemeCssNames.Normalize(FbLayoutThemeVariants.Normalize(variant)),
            foreground: false, invert);

    private static void AppendVariant(StringBuilder builder, FbResolvedLayoutThemeVariant variant, FbThemeMasterTint tint)
    {
        var name = variant.Name;
        AppendVariant(builder, name, "bg-low-anchor", variant.LayoutColors.LowAnchor, tint);
        AppendVariant(builder, name, "bg-high-anchor", variant.LayoutColors.HighAnchor, tint);
    }

    private static void AppendVariant(StringBuilder builder, string variant, string token, string? value, FbThemeMasterTint tint)
        => Append(builder, FbThemeCssNames.ValueName(variant, token), value is null ? null : FbThemeCssNames.Tinted(value, tint));

    private static void Append(StringBuilder builder, string name, string? value)
    {
        FbThemeCssNames.ValidateCssValue(value, name);
        FbThemeCssNames.AppendDeclaration(builder, name, value!);
    }

    private static void AppendReference(StringBuilder builder, string activeToken, string sourceToken)
        => builder.Append("--fb-").Append(activeToken).Append(":var(--fb-").Append(sourceToken).Append(");");
}
