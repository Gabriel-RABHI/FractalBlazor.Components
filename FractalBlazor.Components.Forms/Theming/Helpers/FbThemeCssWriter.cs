using System.Text;
using FractalBlazor.Components.Forms.Theming.Constants;
using FractalBlazor.Components.Forms.Theming.Solver;
using FractalBlazor.Components.Layout.Theming.Model;

namespace FractalBlazor.Components.Forms.Theming.Helpers;

public static class FbThemeCssWriter
{
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

        FbThemeCssNames.AppendGradient(builder, theme.SurfaceMix, theme.BordersMix);
        // Fractions, like the frame mixes: a text sits at 100% - (1 - mix) * (100% - neutral).
        Append(builder, "--fb-fg-subtle-mix", FbThemeCssNames.Fraction(theme.TextVariant.SubtleHighMix, "TextVariantMix.SubtleHighMix"));
        Append(builder, "--fb-fg-muted-mix", FbThemeCssNames.Fraction(theme.TextVariant.MutedHighMix, "TextVariantMix.MutedHighMix"));
        Append(builder, "--fb-fg-default-mix", FbThemeCssNames.Fraction(theme.TextVariant.DefaultHighMix, "TextVariantMix.DefaultHighMix"));
        Append(builder, "--fb-fg-highlight-mix", FbThemeCssNames.Fraction(theme.TextVariant.HighlightHighMix, "TextVariantMix.HighlightHighMix"));

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

        FbThemeCssNames.AppendColorScheme(builder, theme.GetVariant(FbThemeVariants.Default).LayoutColors, theme.SurfaceMix.NeutralMix);

        FbThemeCssNames.AppendTint(builder, theme.MasterTint);
        foreach (var variantName in theme.VariantOrder)
        {
            var variant = theme.GetVariant(variantName);
            FbThemeCssNames.AppendAnchors(builder, variant.Name, variant.LayoutColors, theme.MasterTint);
        }

        AppendReference(builder, "space-s", "default-space-s");
        AppendReference(builder, "space-m", "default-space-m");
        AppendReference(builder, "space-l", "default-space-l");
        AppendReference(builder, "space-x", "default-space-x");
        AppendReference(builder, "radius-s", "default-radius-s");
        AppendReference(builder, "radius-m", "default-radius-m");
        AppendReference(builder, "radius-l", "default-radius-l");
        AppendReference(builder, "radius-x", "default-radius-x");
        builder.Append(FbThemeCssNames.VariantReferences(FbThemeCssNames.Normalize(FbThemeVariants.Default)));

        return builder.ToString();
    }

    /// <summary>
    /// The rules of the variant classes this theme adds to the standard ones, for a
    /// <c>&lt;style&gt;</c> element; empty when the theme only has standard variants.
    /// </summary>
    public static string ToVariantRules(FbResolvedTheme theme)
    {
        ArgumentNullException.ThrowIfNull(theme);
        var builder = new StringBuilder();
        FbThemeCssNames.AppendVariantRules(builder, theme.VariantOrder);
        return builder.ToString();
    }

    private static void Append(StringBuilder builder, string name, string? value)
    {
        FbThemeCssNames.ValidateCssValue(value, name);
        FbThemeCssNames.AppendDeclaration(builder, name, value!);
    }

    private static void AppendReference(StringBuilder builder, string activeToken, string sourceToken)
        => builder.Append("--fb-").Append(activeToken).Append(":var(--fb-").Append(sourceToken).Append(");");
}
