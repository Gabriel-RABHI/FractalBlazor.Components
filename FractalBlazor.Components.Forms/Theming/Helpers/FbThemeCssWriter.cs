using System.Text;
using FractalBlazor.Components.Forms.Theming.Constants;
using FractalBlazor.Components.Forms.Theming.Solver;
using FractalBlazor.Components.Layout.Theming.Model;

namespace FractalBlazor.Components.Forms.Theming.Helpers;

public static class FbThemeCssWriter
{
    public static readonly IReadOnlyList<string> VariantTokens =
    [.. FbThemeCssNames.LayoutVariantTokens, "fg-low-anchor", "fg-high-anchor"];

    public static string ToCssVariables(FbResolvedTheme theme)
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

        Append(builder, "--fb-default-fg-default-high-mix", theme.TextVariant.DefaultHighMix);
        Append(builder, "--fb-default-fg-subtle-high-mix", theme.TextVariant.SubtleHighMix);
        Append(builder, "--fb-default-fg-muted-high-mix", theme.TextVariant.MutedHighMix);
        Append(builder, "--fb-default-fg-highlight-high-mix", theme.TextVariant.HighlightHighMix);

        Append(builder, "--fb-txt-font-family", theme.Typography.TextFontFamily);
        Append(builder, "--fb-code-font-family", theme.Typography.CodeFontFamily);
        Append(builder, "--fb-txt-base-size", theme.Typography.FontSizeBase);
        Append(builder, "--fb-txt-base-weight", theme.Typography.DefaultWeight);
        Append(builder, "--fb-txt-base-line-height", theme.Typography.LineHeight);
        Append(builder, "--fb-txt-xs-coef", theme.Typography.ExtraSmallCoef);
        Append(builder, "--fb-txt-s-coef", theme.Typography.SmallCoef);
        Append(builder, "--fb-txt-m-coef", theme.Typography.MediumCoef);
        Append(builder, "--fb-txt-l-coef", theme.Typography.LargeCoef);
        Append(builder, "--fb-txt-xl-coef", theme.Typography.ExtraLargeCoef);
        Append(builder, "--fb-txt-xxl-coef", theme.Typography.ExtraExtraLargeCoef);
        Append(builder, "--fb-txt-t-weight", theme.Typography.ThinWeight);
        Append(builder, "--fb-txt-b-weight", theme.Typography.BoldWeight);
        Append(builder, "--fb-txt-xb-weight", theme.Typography.ExtraBoldWeight);

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
        AppendReference(builder, "fg-default-high-mix", "default-fg-default-high-mix");
        AppendReference(builder, "fg-subtle-high-mix", "default-fg-subtle-high-mix");
        AppendReference(builder, "fg-muted-high-mix", "default-fg-muted-high-mix");
        AppendReference(builder, "fg-highlight-high-mix", "default-fg-highlight-high-mix");
        builder.Append(BuildVariantReferences(FbThemeVariants.Default));

        return builder.ToString();
    }

    public static string BuildVariantReferences(string? variant)
    {
        var normalized = FbThemeCssNames.Normalize(FbThemeVariants.Normalize(variant));
        var builder = new StringBuilder();

        foreach (var token in VariantTokens)
            builder.Append(FbThemeCssNames.ActiveReference(normalized, token));

        return builder.ToString();
    }

    private static void AppendVariant(StringBuilder builder, FbResolvedThemeVariant variant)
    {
        var name = variant.Name;
        AppendVariant(builder, name, "bg-low-anchor", variant.LayoutColors.LowAnchor);
        AppendVariant(builder, name, "bg-tint", variant.LayoutColors.Tint);
        AppendVariant(builder, name, "bg-high-anchor", variant.LayoutColors.HighAnchor);
        AppendVariant(builder, name, "bg-surface-mix", variant.LayoutColors.SurfaceMix);
        AppendVariant(builder, name, "bg-accent-offset", variant.LayoutColors.AccentOffset);
        AppendVariant(builder, name, "bg-highlight-offset", variant.LayoutColors.HighlightOffset);
        AppendVariant(builder, name, "frame-light-mix", variant.Borders.LightMix);
        AppendVariant(builder, name, "frame-medium-mix", variant.Borders.MediumMix);
        AppendVariant(builder, name, "frame-strong-mix", variant.Borders.StrongMix);
        AppendVariant(builder, name, "fg-low-anchor", variant.FormColors.LowAnchor);
        AppendVariant(builder, name, "fg-high-anchor", variant.FormColors.HighAnchor);
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
