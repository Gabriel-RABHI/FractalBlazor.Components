using FractalBlazor.Components.Forms.Theming.Constants;
using FractalBlazor.Components.Forms.Theming.Registry;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Forms.Theming.Helpers;

/// <summary>
/// Neutral <c>div.fb-theme-scope</c> rendered around a control. It only redirects the theme
/// anchors when a variant is given, so a control without a variant inherits the one of its
/// parent, and switching variant keeps the DOM, and the focus, in place.
/// </summary>
internal static class FbVariantScope
{
    /// <summary>
    /// Variant applied to a control: its own, else <c>Disabled</c> when disabled, else none.
    /// </summary>
    public static string? Effective(string? variant, bool disabled)
        => !string.IsNullOrWhiteSpace(variant) ? variant : disabled ? FbThemeVariants.Disabled : null;

    public static void Build(RenderTreeBuilder builder, string? variant, FbThemeContext? context, RenderFragment content)
    {
        var normalized = variant is null ? null : FbThemeVariants.Normalize(variant);
        if (normalized is not null)
            context?.Theme.GetVariant(normalized);

        builder.OpenRegion(0);
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", "fb-theme-scope");
        builder.AddAttribute(2, "style", normalized is null ? null : FbThemeCssWriter.BuildVariantReferences(normalized));
        builder.AddContent(3, content);
        builder.CloseElement();
        builder.CloseRegion();
    }
}
