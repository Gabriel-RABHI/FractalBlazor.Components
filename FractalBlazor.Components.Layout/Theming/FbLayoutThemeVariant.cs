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
    public FbThemeLayoutBorders? Borders { get; init; }
}
