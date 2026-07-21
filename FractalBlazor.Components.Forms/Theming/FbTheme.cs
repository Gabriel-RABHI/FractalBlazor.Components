using FractalBlazor.Components.Forms.Contracts;
using FractalBlazor.Components.Forms.Theming.Constants;
using FractalBlazor.Components.Forms.Theming.Helpers;
using FractalBlazor.Components.Layout.Abstracts;
using FractalBlazor.Components.Layout.Theming.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Forms.Theming;

public sealed class FbTheme : FbComponentBase
{
    [Inject]
    public IFbFormThemeRegistry Registry { get; set; } = default!;

    [Parameter]
    public string Theme { get; set; } = "Default";

    [Parameter]
    public string Branch { get; set; } = FbThemeBranches.Dark;

    [Parameter]
    public string Selector { get; set; } = ":root";

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        FbThemeCssNames.ValidateSelector(Selector);
        var resolved = Registry.Resolve(Theme, Branch);
        var variables = FbThemeCssWriter.ToCssVariables(resolved);

        builder.OpenElement(0, "style");
        builder.AddMarkupContent(1, $"{Selector}{{{variables}}}");
        builder.CloseElement();

        builder.AddContent(2, ChildContent);
    }
}
