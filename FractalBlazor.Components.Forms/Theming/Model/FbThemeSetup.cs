using FractalBlazor.Components.Layout.Theming.Model;

namespace FractalBlazor.Components.Forms.Theming.Model;

public sealed class FbThemeSetup
{
    public FbThemeSetup(string name, FbThemeSetup? parent = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name.Trim();
        Parent = parent;
    }

    public string Name { get; }

    public FbThemeSetup? Parent { get; }

    public FbThemeLayoutSpacings? Spacings { get; init; }

    public FbThemeLayoutCorners? Corners { get; init; }

    public FbThemeFormTypography? Typography { get; init; }

    public IReadOnlyList<FbThemeBranch> Branches { get; init; } = [];
}
