using FractalBlazor.Components.Forms.Theming.Constants;
using FractalBlazor.Components.Forms.Theming.Model;
using FractalBlazor.Components.Layout.Theming.Model;

namespace FractalBlazor.Components.Forms.Theming.Solver;

public sealed class FbResolvedTheme
{
    internal FbResolvedTheme(
        string name,
        string branch,
        FbThemeLayoutSpacings spacings,
        FbThemeLayoutCorners corners,
        FbThemeFormTypography typography,
        FbThemeFormTextVariant textVariant,
        IReadOnlyDictionary<string, FbResolvedThemeVariant> variants,
        IReadOnlyList<string> variantOrder)
    {
        Name = name;
        Branch = branch;
        Spacings = spacings;
        Corners = corners;
        Typography = typography;
        TextVariant = textVariant;
        Variants = variants;
        VariantOrder = variantOrder;
    }

    public string Name { get; }

    public string Branch { get; }

    public FbThemeLayoutSpacings Spacings { get; }

    public FbThemeLayoutCorners Corners { get; }

    public FbThemeFormTypography Typography { get; }

    public FbThemeFormTextVariant TextVariant { get; }

    public IReadOnlyDictionary<string, FbResolvedThemeVariant> Variants { get; }

    public IReadOnlyList<string> VariantOrder { get; }

    public FbResolvedThemeVariant GetVariant(string? name)
    {
        var normalized = FbThemeVariants.Normalize(name);
        return Variants.TryGetValue(normalized, out var variant)
            ? variant
            : throw new KeyNotFoundException(
                $"Variant '{normalized}' is not defined by theme '{Name}' branch '{Branch}'.");
    }
}
