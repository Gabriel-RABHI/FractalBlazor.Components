namespace FractalBlazor.Components.Layout;

public static class FbLayoutThemeVariants
{
    public const string Default = "Default";
    public const string Selected = "Selected";
    public const string Focused = "Focused";
    public const string Error = "Error";
    public const string Warning = "Warning";
    public const string Disabled = "Disabled";
    public const string Success = "Success";
    public const string Info = "Info";

    public static readonly IReadOnlyList<string> Standard =
    [Default, Selected, Focused, Error, Warning, Disabled, Success, Info];

    public static string Normalize(string? variant)
        => string.IsNullOrWhiteSpace(variant) ? Default : variant.Trim();
}

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
    public IReadOnlyList<FbLayoutThemeVariant> Variants { get; init; } = [];
}

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

public sealed class FbResolvedLayoutTheme
{
    internal FbResolvedLayoutTheme(
        string name,
        FbThemeLayoutSpacings spacings,
        FbThemeLayoutCorners corners,
        IReadOnlyDictionary<string, FbResolvedLayoutThemeVariant> variants,
        IReadOnlyList<string> variantOrder)
    {
        Name = name;
        Spacings = spacings;
        Corners = corners;
        Variants = variants;
        VariantOrder = variantOrder;
    }

    public string Name { get; }
    public FbThemeLayoutSpacings Spacings { get; }
    public FbThemeLayoutCorners Corners { get; }
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

public sealed class FbResolvedLayoutThemeVariant
{
    internal FbResolvedLayoutThemeVariant(string name, FbThemeLayoutColors layoutColors, FbThemeLayoutBorders borders)
    {
        Name = name;
        LayoutColors = layoutColors;
        Borders = borders;
    }

    public string Name { get; }
    public FbThemeLayoutColors LayoutColors { get; }
    public FbThemeLayoutBorders Borders { get; }
}
