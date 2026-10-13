using FractalBlazor.Components.Forms.Theming.Constants;
using FractalBlazor.Components.Forms.Theming.Registry;

namespace FractalBlazor.Components.Forms.Theming.Helpers;

/// <summary>
/// Variant of a control, applied on its own root element: the <see cref="Class"/> class
/// recomputes the theme colors there, and <see cref="Style"/> redirects the anchors. A control
/// without a variant gets neither and inherits the variant of its parent; switching variant
/// only changes two attributes of the same element, so the DOM, and the focus, stay in place.
/// </summary>
internal static class FbVariantScope
{
    /// <summary>
    /// Class of a root element that carries its own variant, named after the <c>Variant</c>
    /// parameter.
    /// </summary>
    public const string Class = "fb-variant";

    /// <summary>
    /// Variant applied to a control: its own, else <c>Disabled</c> when disabled, else none.
    /// </summary>
    public static string? Effective(string? variant, bool disabled)
        => !string.IsNullOrWhiteSpace(variant) ? variant : disabled ? FbThemeVariants.Disabled : null;

    /// <summary>
    /// Redirections that apply <paramref name="variant"/> on an element, or <c>null</c> without
    /// a variant. A variant unknown to the theme of <paramref name="context"/> throws.
    /// </summary>
    public static string? Style(string? variant, FbThemeContext? context)
    {
        if (variant is null)
            return null;

        var normalized = FbThemeVariants.Normalize(variant);
        context?.Theme.GetVariant(normalized);
        return FbThemeCssWriter.BuildVariantReferences(normalized);
    }

    /// <summary>
    /// <paramref name="classes"/> with <see cref="Class"/> when <paramref name="style"/> applies a
    /// variant.
    /// </summary>
    public static string WithClass(string classes, string? style)
        => style is null ? classes : classes + " " + Class;
}
