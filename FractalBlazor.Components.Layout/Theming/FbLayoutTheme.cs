using FractalBlazor.Components.Layout.Abstracts;
using FractalBlazor.Components.Layout.Theming.Contracts;
using FractalBlazor.Components.Layout.Theming.Helpers;
using FractalBlazor.Components.Layout.Theming.Model;
using FractalBlazor.Components.Layout.Theming.Registry;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout.Theming;

public sealed class FbLayoutTheme : FbComponentBase
{
    [Inject]
    public IFbLayoutThemeRegistry Registry { get; set; } = default!;

    [Parameter]
    public string Theme { get; set; } = "Default";

    [Parameter]
    public string Selector { get; set; } = ":root";

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        FbThemeCssNames.ValidateSelector(Selector);
        var resolved = Registry.Resolve(Theme);
        var variables = FbLayoutThemeCssWriter.ToCssVariables(resolved);

        builder.OpenElement(0, "style");
        builder.AddMarkupContent(1, $"{Selector}{{{variables}}}");
        builder.CloseElement();

        if (ChildContent is null)
            return;

        builder.OpenComponent<CascadingValue<FbLayoutThemeContext>>(2);
        builder.AddAttribute(3, "Value", new FbLayoutThemeContext(resolved));
        builder.AddAttribute(4, "ChildContent", ChildContent);
        builder.CloseComponent();
    }
}
