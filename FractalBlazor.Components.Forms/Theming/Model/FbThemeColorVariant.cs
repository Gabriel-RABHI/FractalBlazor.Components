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

    /// <summary>
    /// The gradient of the variant: backgrounds, texts and frames are all positions on it.
    /// </summary>
    public FbThemeLayoutColors? LayoutColors { get; init; }
}
