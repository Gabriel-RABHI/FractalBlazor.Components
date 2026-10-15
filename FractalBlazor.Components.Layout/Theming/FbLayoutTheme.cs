using FractalBlazor.Components.Layout.Theming.Solver;
using FractalBlazor.Components.Layout.Abstracts;
using FractalBlazor.Components.Layout.Theming.Contracts;
using FractalBlazor.Components.Layout.Theming.Helpers;
using FractalBlazor.Components.Layout.Theming.Model;
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
    private string _variables = "";
    private string _variantRules = "";

    /// <summary>
    /// Writes the variables of the theme, and the rules of the variants it adds to the standard
    /// ones. A variant is a CSS class: nothing is cascaded to the content.
    /// </summary>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var resolved = Registry.Resolve(Theme);
        if (!ReferenceEquals(resolved, _resolved))
        {
            _resolved = resolved;
            _variables = FbLayoutThemeCssWriter.ToCssVariables(resolved);
            _variantRules = FbLayoutThemeCssWriter.ToVariantRules(resolved);
        }

        if (Scoped)
        {
            if (_variantRules.Length > 0)
            {
                builder.OpenElement(0, "style");
                builder.AddMarkupContent(1, _variantRules);
                builder.CloseElement();
            }

            builder.OpenElement(2, "div");
            builder.AddAttribute(3, "class", "fb-theme-scope fb-theme");
            builder.AddAttribute(4, "style", _variables);
            builder.AddContent(5, ChildContent);
            builder.CloseElement();
            return;
        }

        FbThemeCssNames.ValidateSelector(Selector);
        builder.OpenElement(6, "style");
        builder.AddMarkupContent(7, $"{Selector}{{{_variables}}}{_variantRules}");
        builder.CloseElement();

        builder.AddContent(8, ChildContent);
    }
}
