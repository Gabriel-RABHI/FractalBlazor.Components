namespace FractalBlazor.Components.Layout.Theming;

public sealed class FbLayoutThemeContext
{
    internal FbLayoutThemeContext(FbResolvedLayoutTheme theme) => Theme = theme;
    public FbResolvedLayoutTheme Theme { get; }
}
