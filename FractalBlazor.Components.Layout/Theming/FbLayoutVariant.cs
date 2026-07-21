using FractalBlazor.Components.Layout.Abstracts;
using FractalBlazor.Components.Layout.Theming.Contracts;
using FractalBlazor.Components.Layout.Theming.Helpers;
using FractalBlazor.Components.Layout.Theming.Registry;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout.Theming;

public sealed class FbLayoutVariant : FbComponentBase
{
    [CascadingParameter]
    public FbLayoutThemeContext? ThemeContext { get; set; }

    [Parameter]
    public string Variant { get; set; } = FbLayoutThemeVariants.Default;

    [Parameter]
    public string Classes { get; set; } = "";

    [Parameter]
    public string Style { get; set; } = "";

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (ThemeContext is null)
            throw new InvalidOperationException($"{nameof(FbLayoutVariant)} must be rendered inside {nameof(FbLayoutTheme)}.");

        var normalized = FbLayoutThemeVariants.Normalize(Variant);
        ThemeContext.Theme.GetVariant(normalized);

        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", $"fb-theme-scope {Classes}".TrimEnd());
        builder.AddAttribute(2, "style", FbLayoutThemeCssWriter.BuildVariantReferences(normalized) + Style);
        builder.AddContent(3, ChildContent);
        builder.CloseElement();
    }
}
