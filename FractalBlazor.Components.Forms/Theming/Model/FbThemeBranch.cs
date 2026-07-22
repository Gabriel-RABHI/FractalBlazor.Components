namespace FractalBlazor.Components.Forms.Theming.Model;

public sealed class FbThemeBranch
{
    public FbThemeBranch(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name.Trim();
    }

    public string Name { get; }

    public FbThemeFormTextVariant? TextVariant { get; init; }

    public IReadOnlyList<FbThemeVariant> Variants { get; init; } = [];
}
