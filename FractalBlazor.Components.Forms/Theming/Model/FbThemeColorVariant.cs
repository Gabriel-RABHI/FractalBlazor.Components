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
}
