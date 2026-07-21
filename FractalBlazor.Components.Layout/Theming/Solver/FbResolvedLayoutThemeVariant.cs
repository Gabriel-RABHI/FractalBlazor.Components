using FractalBlazor.Components.Layout.Theming.Model;

namespace FractalBlazor.Components.Layout.Theming.Solver;

public sealed class FbResolvedLayoutThemeVariant
{
    internal FbResolvedLayoutThemeVariant(string name, FbThemeLayoutColors layoutColors, FbThemeLayoutBorders borders)
    {
        Name = name;
        LayoutColors = layoutColors;
        Borders = borders;
    }

    public string Name { get; }

    public FbThemeLayoutColors LayoutColors { get; }

    public FbThemeLayoutBorders Borders { get; }
}
