using FractalBlazor.Components.Forms.Theming.Constants;
using FractalBlazor.Components.Forms.Theming.Helpers;
using FractalBlazor.Components.Forms.Theming.Registry;
using FractalBlazor.Components.Layout.Abstracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Forms.Theming;

public sealed class FbVariant : FbComponentBase
{
    [CascadingParameter]
    public FbThemeContext? ThemeContext { get; set; }

    [Parameter]
    public string Variant { get; set; } = FbThemeVariants.Default;

    [Parameter]
    public string Classes { get; set; } = "";

    [Parameter]
    public string Style { get; set; } = "";

    /// <summary>
    /// Reverses the gradient of the variant, its low and high anchors exchanged: a dark variant
    /// renders light and the other way round. A light card inside a dark page, without a new branch.
    /// </summary>
    [Parameter]
    public bool Invert { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var normalized = FbThemeVariants.Normalize(Variant);

        // The context is absent across a render mode boundary (a static FbTheme around an
        // interactive page): the variant is then trusted, as CSS falls back silently anyway.
        ThemeContext?.Theme.GetVariant(normalized);

        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", $"fb-theme-scope {Classes}".TrimEnd());
        builder.AddAttribute(2, "style", FbThemeCssWriter.BuildVariantReferences(normalized, Invert) + Style);
        builder.AddContent(3, ChildContent);
        builder.CloseElement();
    }
}
