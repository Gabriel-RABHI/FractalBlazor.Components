using FractalBlazor.Components.Layout.Theming.Solver;
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

    /// <summary>
    /// Applies the theme to the child content only, through a neutral <c>div.fb-theme-scope</c>
    /// that carries the variables inline. <see cref="Selector"/> is then ignored.
    /// </summary>
    [Parameter]
    public bool Scoped { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private FbResolvedLayoutTheme? _resolved;
    private FbLayoutThemeContext? _context;
    private string _variables = "";

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var resolved = Registry.Resolve(Theme);
        if (!ReferenceEquals(resolved, _resolved))
        {
            _resolved = resolved;
            _variables = FbLayoutThemeCssWriter.ToCssVariables(resolved);
            if (_context is null)
                _context = new FbLayoutThemeContext(resolved);
            else
                _context.Theme = resolved;
        }

        if (Scoped)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "fb-theme-scope fb-theme");
            builder.AddAttribute(2, "style", _variables);
            BuildContent(builder);
            builder.CloseElement();
            return;
        }

        FbThemeCssNames.ValidateSelector(Selector);
        builder.OpenElement(3, "style");
        builder.AddMarkupContent(4, $"{Selector}{{{_variables}}}");
        builder.CloseElement();

        BuildContent(builder);
    }

    private void BuildContent(RenderTreeBuilder builder)
    {
        if (ChildContent is null)
            return;

        builder.OpenComponent<CascadingValue<FbLayoutThemeContext>>(10);
        builder.AddAttribute(11, "Value", _context);
        builder.AddAttribute(12, "IsFixed", true);
        builder.AddAttribute(13, "ChildContent", ChildContent);
        builder.CloseComponent();
    }
}
