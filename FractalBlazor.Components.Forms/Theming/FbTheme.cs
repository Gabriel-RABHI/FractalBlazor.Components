using FractalBlazor.Components.Forms.Contracts;
using FractalBlazor.Components.Forms.Theming.Constants;
using FractalBlazor.Components.Forms.Theming.Helpers;
using FractalBlazor.Components.Forms.Theming.Model;
using FractalBlazor.Components.Forms.Theming.Registry;
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
    private FbThemeContext? _context;
    private string _variables = "";
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

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var resolved = ResolveTheme();
        if (!ReferenceEquals(resolved, _resolved))
        {
            _resolved = resolved;
            _variables = FbThemeCssWriter.ToCssVariables(resolved);
            if (_context is null)
                _context = new FbThemeContext(resolved);
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

        builder.OpenComponent<CascadingValue<FbThemeContext>>(10);
        builder.AddAttribute(11, "Value", _context);
        builder.AddAttribute(12, "IsFixed", true);
        builder.AddAttribute(13, "ChildContent", ChildContent);
        builder.CloseComponent();
    }
}
