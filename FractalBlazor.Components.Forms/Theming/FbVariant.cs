using FractalBlazor.Components.Forms.Theming.Constants;
using FractalBlazor.Components.Forms.Theming.Helpers;
using FractalBlazor.Components.Layout.Abstracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Forms.Theming;

public sealed class FbVariant : FbComponentBase
{
    [Parameter]
    public string Variant { get; set; } = FbThemeVariants.Default;

    [Parameter]
    public string Classes { get; set; } = "";

    [Parameter]
    public string Style { get; set; } = "";

    /// <summary>
    /// Swap -> Background: exchange the low and high background anchors (fills, frames).
    /// A light card inside a dark page, without a new branch.
    /// </summary>
    [Parameter]
    public bool SB { get; set; }

    /// <summary>
    /// Swap -> Foreground: exchange the low and high foreground anchors (text, icons).
    /// </summary>
    [Parameter]
    public bool SF { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var normalized = FbThemeVariants.Normalize(Variant);

        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", $"fb-theme-scope {Classes}".TrimEnd());
        builder.AddAttribute(2, "style", FbThemeCssWriter.BuildVariantReferences(normalized, SB, SF) + Style);
        builder.AddContent(3, ChildContent);
        builder.CloseElement();
    }
}
