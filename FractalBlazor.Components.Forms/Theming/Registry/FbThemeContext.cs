using FractalBlazor.Components.Forms.Theming.Solver;

namespace FractalBlazor.Components.Forms.Theming.Registry;

/// <summary>
/// Cascaded by <c>FbTheme</c> with <c>IsFixed</c>, so descendants never subscribe to it.
/// The instance lives as long as its theme component and always exposes the theme it last
/// resolved. <c>FbVariant</c> uses it to reject unknown variant names.
/// </summary>
public sealed class FbThemeContext
{
    internal FbThemeContext(FbResolvedTheme theme) => Theme = theme;

    public FbResolvedTheme Theme { get; internal set; }
}
