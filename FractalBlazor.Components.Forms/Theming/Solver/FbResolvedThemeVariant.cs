using FractalBlazor.Components.Forms.Theming.Model;
using FractalBlazor.Components.Layout.Theming.Model;

namespace FractalBlazor.Components.Forms.Theming.Solver;

public sealed class FbResolvedThemeVariant
{
    internal FbResolvedThemeVariant(
        string name,
        FbThemeLayoutColors layoutColors,
        FbThemeFormColors formColors,
        FbThemeFormColors frameColors)
    {
        Name = name;
        LayoutColors = layoutColors;
        FormColors = formColors;
        FrameColors = frameColors;
    }

    public string Name { get; }

    public FbThemeLayoutColors LayoutColors { get; }

    public FbThemeFormColors FormColors { get; }

    /// <summary>
    /// Anchors of the frame lines; either one may be <c>null</c>, the CSS then falls back to
    /// the current background (low) or to the high background anchor (high).
    /// </summary>
    public FbThemeFormColors FrameColors { get; }
}
