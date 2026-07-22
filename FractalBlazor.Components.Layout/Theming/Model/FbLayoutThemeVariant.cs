using FractalBlazor.Components.Layout.Theming.Contracts;
using FractalBlazor.Components.Layout.Theming.Model;

namespace FractalBlazor.Components.Layout;

public sealed class FbLayoutThemeVariant
{
    public FbLayoutThemeVariant(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = FbLayoutThemeVariants.Normalize(name);
    }

    public string Name { get; }

    public FbThemeLayoutColors? LayoutColors { get; init; }
}
