using FractalBlazor.Components.Forms.Theming.Model;
using FractalBlazor.Components.Layout.Theming.Model;

namespace FractalBlazor.Components.Forms.Theming.Solver;

public sealed class FbResolvedThemeVariant
{
    internal FbResolvedThemeVariant(
        string name,
        FbThemeLayoutColors layoutColors,
        FbThemeFormColors formColors,
        FbThemeLayoutBorders borders)
    {
        Name = name;
        LayoutColors = layoutColors;
        FormColors = formColors;
        Borders = borders;
    }

    public string Name { get; }

    public FbThemeLayoutColors LayoutColors { get; }

    public FbThemeFormColors FormColors { get; }

    public FbThemeLayoutBorders Borders { get; }
}
