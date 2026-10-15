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

        FbThemeCssNames.AppendGradient(builder, theme.SurfaceMix, theme.BordersMix);
        FbThemeCssNames.AppendColorScheme(builder, theme.GetVariant(FbLayoutThemeVariants.Default).LayoutColors, theme.SurfaceMix.NeutralMix);

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
        builder.Append(FbThemeCssNames.VariantReferences(FbThemeCssNames.Normalize(FbLayoutThemeVariants.Default)));

        return builder.ToString();
    }

    /// <summary>
    /// The rules of the variant classes this theme adds to the standard ones, for a
    /// <c>&lt;style&gt;</c> element; empty when the theme only has standard variants.
    /// </summary>
    public static string ToVariantRules(FbResolvedLayoutTheme theme)
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
