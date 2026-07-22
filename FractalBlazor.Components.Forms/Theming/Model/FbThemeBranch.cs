namespace FractalBlazor.Components.Forms.Theming.Model;

using FractalBlazor.Components.Layout.Theming.Model;

public sealed class FbThemeBranch
{
    public FbThemeBranch(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name.Trim();
    }

    public string Name { get; }

    public FbThemeMasterTint? MasterTint { get; init; }

    public FbThemeFormTextVariantMix? TextVariantMix { get; init; }

    public FbThemeLayoutBordersMix? BordersMix { get; init; }

    public FbThemeLayoutSurfaceMix? SurfaceMix { get; init; }

    public IReadOnlyList<FbThemeColorVariant> Variants { get; init; } = [];
}
