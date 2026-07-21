using FractalBlazor.Components.Layout.Theming.Solver;

namespace FractalBlazor.Components.Layout.Theming.Registry;

public sealed class FbLayoutThemeContext
{
    internal FbLayoutThemeContext(FbResolvedLayoutTheme theme) => Theme = theme;

    public FbResolvedLayoutTheme Theme { get; }
}
