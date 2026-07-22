using FractalBlazor.Components.Layout.Theming.Model;

namespace FractalBlazor.Components.Layout;

public sealed class FbLayoutThemeSetup
{
    public FbLayoutThemeSetup(string name, FbLayoutThemeSetup? parent = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name.Trim();
        Parent = parent;
    }

    public string Name { get; }

    public FbLayoutThemeSetup? Parent { get; }

    public FbThemeLayoutSpacings? Spacings { get; init; }

    public FbThemeLayoutCorners? Corners { get; init; }

    public FbThemeMasterTint? MasterTint { get; init; }

    public FbThemeLayoutBordersMix? BordersMix { get; init; }

    public FbThemeLayoutSurfaceMix? SurfaceMix { get; init; }

    public IReadOnlyList<FbLayoutThemeVariant> Variants { get; init; } = [];
}
