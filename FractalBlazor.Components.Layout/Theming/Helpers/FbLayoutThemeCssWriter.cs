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

        foreach (var variantName in theme.VariantOrder)
            AppendVariant(builder, theme.GetVariant(variantName));

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

    public static string BuildVariantReferences(string? variant)
    {
        var normalized = FbThemeCssNames.Normalize(FbLayoutThemeVariants.Normalize(variant));
        var builder = new StringBuilder();

        foreach (var token in FbThemeCssNames.LayoutVariantTokens)
            builder.Append(FbThemeCssNames.ActiveReference(normalized, token));

        return builder.ToString();
    }

    private static void AppendVariant(StringBuilder builder, FbResolvedLayoutThemeVariant variant)
    {
        var name = variant.Name;
        AppendVariant(builder, name, "bg-low-anchor", variant.LayoutColors.LowAnchor);
        AppendVariant(builder, name, "bg-tint", variant.LayoutColors.Tint);
        AppendVariant(builder, name, "bg-high-anchor", variant.LayoutColors.HighAnchor);
        AppendVariant(builder, name, "bg-surface-mix", variant.LayoutColors.SurfaceMix);
        AppendVariant(builder, name, "bg-accent-offset", variant.LayoutColors.AccentOffset);
        AppendVariant(builder, name, "bg-highlight-offset", variant.LayoutColors.HighlightOffset);
        AppendVariant(builder, name, "frame-light-mix", variant.Borders.LightMix);
        AppendVariant(builder, name, "frame-light-size", variant.Borders.LightSize);
        AppendVariant(builder, name, "frame-medium-mix", variant.Borders.MediumMix);
        AppendVariant(builder, name, "frame-medium-size", variant.Borders.MediumSize);
        AppendVariant(builder, name, "frame-strong-mix", variant.Borders.StrongMix);
        AppendVariant(builder, name, "frame-strong-size", variant.Borders.StrongSize);
    }

    private static void AppendVariant(StringBuilder builder, string variant, string token, string? value)
        => Append(builder, FbThemeCssNames.ValueName(variant, token), value);

    private static void Append(StringBuilder builder, string name, string? value)
    {
        FbThemeCssNames.ValidateCssValue(value, name);
        FbThemeCssNames.AppendDeclaration(builder, name, value!);
    }

    private static void AppendReference(StringBuilder builder, string activeToken, string sourceToken)
        => builder.Append("--fb-").Append(activeToken).Append(":var(--fb-").Append(sourceToken).Append(");");
}
