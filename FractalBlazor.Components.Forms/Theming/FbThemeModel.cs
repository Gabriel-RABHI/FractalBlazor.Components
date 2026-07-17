using FractalBlazor.Components.Layout;

namespace FractalBlazor.Components.Forms.Theming;

public static class FbThemeBranches
{
    public const string Dark = "Dark";
    public const string Light = "Light";
}

public static class FbThemeVariants
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

public sealed class FbThemeVariant
{
    public FbThemeVariant(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = FbThemeVariants.Normalize(name);
    }

    public string Name { get; }
    public FbThemeLayoutColors? LayoutColors { get; init; }
    public FbThemeFormColors? FormColors { get; init; }
    public FbThemeLayoutBorders? Borders { get; init; }
}

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

public sealed class FbResolvedThemeVariant
{
    internal FbResolvedThemeVariant(
        string name,
        FbThemeLayoutColors layoutColors,
        FbThemeFormColors formColors,
        FbThemeLayoutBorders borders)
    {
        Name = name;
        LayoutColors = layoutColors;
        FormColors = formColors;
        Borders = borders;
    }

    public string Name { get; }
    public FbThemeLayoutColors LayoutColors { get; }
    public FbThemeFormColors FormColors { get; }
    public FbThemeLayoutBorders Borders { get; }
}
