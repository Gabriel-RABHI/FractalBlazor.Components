using FractalBlazor.Components.Forms.Theming.Constants;
using FractalBlazor.Components.Layout.Theming.Model;

namespace FractalBlazor.Components.Forms.Theming.Model;

public sealed class FbThemeColorVariant
{
    public FbThemeColorVariant(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = FbThemeVariants.Normalize(name);
    }

    public string Name { get; }

    public FbThemeLayoutColors? LayoutColors { get; init; }

    public FbThemeFormColors? FormColors { get; init; }

    /// <summary>
    /// Anchors of the frame lines. Without them, a frame goes from the current background
    /// towards the high background anchor; with them, from the low to the high frame anchor.
    /// </summary>
    public FbThemeFormColors? FrameColors { get; init; }
}
