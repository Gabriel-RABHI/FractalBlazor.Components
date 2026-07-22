using FractalBlazor.Components.Layout.Theming.Contracts;
using FractalBlazor.Components.Layout.Theming.Model;

namespace FractalBlazor.Components.Layout.Theming.Solver;

public sealed class FbResolvedLayoutTheme
{
    internal FbResolvedLayoutTheme(
        string name,
        FbThemeLayoutSpacings spacings,
        FbThemeLayoutCorners corners,
        FbThemeMasterTint masterTint,
        FbThemeLayoutBordersMix bordersMix,
        FbThemeLayoutSurfaceMix surfaceMix,
        IReadOnlyDictionary<string, FbResolvedLayoutThemeVariant> variants,
        IReadOnlyList<string> variantOrder)
    {
        Name = name;
        Spacings = spacings;
        Corners = corners;
        MasterTint = masterTint;
        BordersMix = bordersMix;
        SurfaceMix = surfaceMix;
        Variants = variants;
        VariantOrder = variantOrder;
    }

    public string Name { get; }

    public FbThemeLayoutSpacings Spacings { get; }

    public FbThemeLayoutCorners Corners { get; }

    public FbThemeMasterTint MasterTint { get; }

    public FbThemeLayoutBordersMix BordersMix { get; }

    public FbThemeLayoutSurfaceMix SurfaceMix { get; }

    public IReadOnlyDictionary<string, FbResolvedLayoutThemeVariant> Variants { get; }

    public IReadOnlyList<string> VariantOrder { get; }

    public FbResolvedLayoutThemeVariant GetVariant(string? name)
    {
        var normalized = FbLayoutThemeVariants.Normalize(name);
        return Variants.TryGetValue(normalized, out var variant)
            ? variant
            : throw new KeyNotFoundException($"The layout variant '{normalized}' is not defined by theme '{Name}'.");
    }
}
