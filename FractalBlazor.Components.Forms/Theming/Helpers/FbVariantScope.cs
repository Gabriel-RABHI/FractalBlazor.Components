using FractalBlazor.Components.Forms.Theming.Constants;
using FractalBlazor.Components.Layout.Theming.Model;

namespace FractalBlazor.Components.Forms.Theming.Helpers;

/// <summary>
/// Variant of a control, applied on its own root element by classes: <c>fb-variant</c>
/// computes the theme colors again there, and <c>fb-v-error</c> redirects the anchors. A
/// control without a variant gets neither and inherits the variant of its parent; switching
/// variant only changes the class attribute of the same element, so the DOM, and the focus,
/// stay in place. A variant the theme does not know has no rule: the control keeps the
/// variant of its parent.
/// </summary>
internal static class FbVariantScope
{
    /// <summary>
    /// Variant applied to a control: its own, else <c>Disabled</c> when disabled, else none.
    /// </summary>
    public static string? Effective(string? variant, bool disabled)
        => !string.IsNullOrWhiteSpace(variant) ? variant : disabled ? FbThemeVariants.Disabled : null;

    /// <summary>
    /// Classes that apply <paramref name="variant"/> on an element, or <c>null</c> without a
    /// variant.
    /// </summary>
    public static string? Classes(string? variant)
        => variant is null ? null : FbThemeCssNames.VariantClasses(variant);

    /// <summary>
    /// <paramref name="classes"/> followed by <paramref name="variantClasses"/>, when there are
    /// any.
    /// </summary>
    public static string WithClasses(string classes, string? variantClasses)
        => variantClasses is null ? classes : classes + " " + variantClasses;
}
