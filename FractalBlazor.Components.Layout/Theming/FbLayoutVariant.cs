using FractalBlazor.Components.Layout.Abstracts;
using FractalBlazor.Components.Layout.Theming.Contracts;
using FractalBlazor.Components.Layout.Theming.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout.Theming;

public sealed class FbLayoutVariant : FbComponentBase
{
    [Parameter]
    public string Variant { get; set; } = FbLayoutThemeVariants.Default;

    [Parameter]
    public string Classes { get; set; } = "";

    [Parameter]
    public string Style { get; set; } = "";

    /// <summary>
    /// Reverses the gradient of the variant, its low and high anchors exchanged: a dark variant
    /// renders light and the other way round.
    /// </summary>
    [Parameter]
    public bool Invert { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private string? _variantClasses, _classes, _class;

    /// <summary>
    /// A neutral div (<c>display: contents</c>) with the variant classes: <c>fb-v-error</c>
    /// redirects the anchors, <c>fb-invert</c> reverses them. A variant the theme does not know
    /// has no rule: the content keeps the variant of its parent.
    /// </summary>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var variantClasses = FbThemeCssNames.VariantClasses(Variant, Invert);
        if (_class is null || !ReferenceEquals(variantClasses, _variantClasses) || !ReferenceEquals(Classes, _classes))
        {
            (_variantClasses, _classes) = (variantClasses, Classes);
            _class = string.IsNullOrWhiteSpace(Classes)
                ? "fb-theme-scope " + variantClasses
                : "fb-theme-scope " + variantClasses + " " + Classes.Trim();
        }

        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", _class);
        builder.AddAttribute(2, "style", string.IsNullOrEmpty(Style) ? null : Style);
        builder.AddContent(3, ChildContent);
        builder.CloseElement();
    }
}
