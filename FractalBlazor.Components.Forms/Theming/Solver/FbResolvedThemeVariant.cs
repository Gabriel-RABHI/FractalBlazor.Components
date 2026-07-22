using FractalBlazor.Components.Forms.Theming.Model;
using FractalBlazor.Components.Layout.Theming.Model;

namespace FractalBlazor.Components.Forms.Theming.Solver;

public sealed class FbResolvedThemeVariant
{
    internal FbResolvedThemeVariant(
        string name,
        FbThemeLayoutColors layoutColors,
        FbThemeFormColors formColors)
    {
        Name = name;
        LayoutColors = layoutColors;
        FormColors = formColors;
    }

    public string Name { get; }

    public FbThemeLayoutColors LayoutColors { get; }

    public FbThemeFormColors FormColors { get; }
}
