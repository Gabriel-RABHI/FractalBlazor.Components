using FractalBlazor.Components.Forms.Contracts;
using FractalBlazor.Components.Forms.Theming.Constants;
using FractalBlazor.Components.Forms.Theming.Helpers;
using FractalBlazor.Components.Forms.Theming.Model;
using FractalBlazor.Components.Forms.Theming.Solver;
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

    /// <summary>
    /// A theme that is not registered, applied instead of <see cref="Theme"/>: the preview of a
    /// theme editor, for instance. It must inherit from a registered theme.
    /// </summary>
    [Parameter]
    public FbThemeSetup? Setup { get; set; }

    [Parameter]
    public string Selector { get; set; } = ":root";

    /// <summary>
    /// Applies the theme to the child content only, through a neutral <c>div.fb-theme-scope</c>
    /// that carries the variables inline. <see cref="Selector"/> is then ignored. A page can
    /// host a <c>Light</c> island inside a <c>Dark</c> theme this way.
    /// </summary>
    [Parameter]
    public bool Scoped { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private FbResolvedTheme? _resolved;
    private string _variables = "";
    private string _variantRules = "";
    private FbThemeSetup? _resolvedSetup;
    private string? _resolvedSetupBranch;

    /// <summary>
    /// Registered themes are resolved once by the registry. An unregistered <see cref="Setup"/>
    /// is resolved again only when another instance or another branch is given.
    /// </summary>
    private FbResolvedTheme ResolveTheme()
    {
        if (Setup is null)
            return Registry.Resolve(Theme, Branch);

        if (_resolved is null || !ReferenceEquals(Setup, _resolvedSetup) || Branch != _resolvedSetupBranch)
        {
            _resolvedSetup = Setup;
            _resolvedSetupBranch = Branch;
            return Registry.Resolve(Setup, Branch);
        }

        return _resolved;
    }

    /// <summary>
    /// Writes the variables of the theme, and the rules of the variants it adds to the standard
    /// ones. A variant is a CSS class: nothing is cascaded to the content.
    /// </summary>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var resolved = ResolveTheme();
        if (!ReferenceEquals(resolved, _resolved))
        {
            _resolved = resolved;
            _variables = FbThemeCssWriter.ToCssVariables(resolved);
            _variantRules = FbThemeCssWriter.ToVariantRules(resolved);
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
