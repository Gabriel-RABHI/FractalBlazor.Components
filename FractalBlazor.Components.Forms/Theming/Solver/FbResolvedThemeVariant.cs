using FractalBlazor.Components.Layout.Theming.Model;

namespace FractalBlazor.Components.Forms.Theming.Solver;

public sealed class FbResolvedThemeVariant
{
    internal FbResolvedThemeVariant(string name, FbThemeLayoutColors layoutColors)
    {
        Name = name;
        LayoutColors = layoutColors;
    }

    public string Name { get; }

    public FbThemeLayoutColors LayoutColors { get; }
}
