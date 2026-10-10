using FractalBlazor.Components.Layout.Theming.Solver;

namespace FractalBlazor.Components.Layout.Theming.Registry;

/// <summary>
/// Cascaded by <c>FbLayoutTheme</c> with <c>IsFixed</c>, so descendants never subscribe to it.
/// The instance lives as long as its theme component and always exposes the theme it last
/// resolved. <c>FbLayoutVariant</c> uses it to reject unknown variant names.
/// </summary>
public sealed class FbLayoutThemeContext
{
    internal FbLayoutThemeContext(FbResolvedLayoutTheme theme) => Theme = theme;

    public FbResolvedLayoutTheme Theme { get; internal set; }
}
