using FractalBlazor.Components.Layout.Theming.Model;

namespace FractalBlazor.Components.Layout.Theming.Solver;

public sealed class FbResolvedLayoutThemeVariant
{
    internal FbResolvedLayoutThemeVariant(string name, FbThemeLayoutColors layoutColors)
    {
        Name = name;
        LayoutColors = layoutColors;
    }

    public string Name { get; }

    public FbThemeLayoutColors LayoutColors { get; }
}
