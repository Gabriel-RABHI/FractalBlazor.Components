using System.Collections.ObjectModel;

namespace FractalBlazor.Components.Layout;

public interface IFbLayoutThemeRegistry
{
    FbLayoutThemeSetup Default { get; }
    void Register(FbLayoutThemeSetup theme);
    bool TryGet(string name, out FbLayoutThemeSetup theme);
    FbResolvedLayoutTheme Resolve(string theme);
}

public sealed class FbLayoutThemeRegistry : IFbLayoutThemeRegistry
{
    private readonly object _sync = new();
    private readonly Dictionary<string, FbLayoutThemeSetup> _themes = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, FbResolvedLayoutTheme> _resolved = new(StringComparer.OrdinalIgnoreCase);

    public FbLayoutThemeRegistry()
    {
        Default = FbLayoutThemeDefaults.Create();
        ValidateDefinition(Default);
        _themes.Add(Default.Name, Default);
        _resolved.Add(Default.Name, ResolveCore(Default));
    }

    public FbLayoutThemeSetup Default { get; }

    public void Register(FbLayoutThemeSetup theme)
    {
        ArgumentNullException.ThrowIfNull(theme);

        lock (_sync)
        {
            ValidateDefinition(theme);

            if (_themes.ContainsKey(theme.Name))
                throw new InvalidOperationException($"A layout theme named '{theme.Name}' is already registered.");

            if (theme.Parent is null)
                throw new InvalidOperationException($"The layout theme '{theme.Name}' must inherit from the default theme.");

            if (!_themes.TryGetValue(theme.Parent.Name, out var registeredParent) ||
                !ReferenceEquals(theme.Parent, registeredParent))
            {
                throw new InvalidOperationException(
                    $"The parent '{theme.Parent.Name}' of layout theme '{theme.Name}' must be registered first.");
            }

            _themes.Add(theme.Name, theme);

            try
            {
                _resolved.Add(theme.Name, ResolveCore(theme));
            }
            catch
            {
                _themes.Remove(theme.Name);
                throw;
            }
        }
    }

    public bool TryGet(string name, out FbLayoutThemeSetup theme)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        lock (_sync)
            return _themes.TryGetValue(name.Trim(), out theme!);
    }

    public FbResolvedLayoutTheme Resolve(string theme)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(theme);
        lock (_sync)
        {
            return _resolved.TryGetValue(theme.Trim(), out var resolved)
                ? resolved
                : throw new KeyNotFoundException($"The layout theme '{theme}' is not registered.");
        }
    }

    private static FbResolvedLayoutTheme ResolveCore(FbLayoutThemeSetup theme)
    {
        var spacings = new FbThemeLayoutSpacings
        {
            S = Required(FindThemeValue(theme, value => value.Spacings?.S), "Spacings.S"),
            M = Required(FindThemeValue(theme, value => value.Spacings?.M), "Spacings.M"),
            L = Required(FindThemeValue(theme, value => value.Spacings?.L), "Spacings.L"),
            X = Required(FindThemeValue(theme, value => value.Spacings?.X), "Spacings.X")
        };

        var corners = new FbThemeLayoutCorners
        {
            S = Required(FindThemeValue(theme, value => value.Corners?.S), "Corners.S"),
            M = Required(FindThemeValue(theme, value => value.Corners?.M), "Corners.M"),
            L = Required(FindThemeValue(theme, value => value.Corners?.L), "Corners.L"),
            X = Required(FindThemeValue(theme, value => value.Corners?.X), "Corners.X")
        };

        var variantNames = GetVariantNames(theme);
        var variants = new Dictionary<string, FbResolvedLayoutThemeVariant>(StringComparer.OrdinalIgnoreCase);

        foreach (var variantName in variantNames)
            variants.Add(variantName, ResolveVariant(theme, variantName));

        return new FbResolvedLayoutTheme(
            theme.Name,
            spacings,
            corners,
            new ReadOnlyDictionary<string, FbResolvedLayoutThemeVariant>(variants),
            variantNames.AsReadOnly());
    }

    private static FbResolvedLayoutThemeVariant ResolveVariant(FbLayoutThemeSetup theme, string variantName)
    {
        FbResolvedLayoutThemeVariant? defaultVariant = null;

        if (!variantName.Equals(FbLayoutThemeVariants.Default, StringComparison.OrdinalIgnoreCase))
            defaultVariant = ResolveVariant(theme, FbLayoutThemeVariants.Default);

        string Pick(Func<FbLayoutThemeVariant, string?> selector, Func<FbResolvedLayoutThemeVariant, string?> fallback, string name)
            => Required(FindVariantValue(theme, variantName, selector) ??
                        (defaultVariant is null ? null : fallback(defaultVariant)), name);

        var colors = new FbThemeLayoutColors
        {
            LowAnchor = Pick(value => value.LayoutColors?.LowAnchor, value => value.LayoutColors.LowAnchor, $"{variantName}.LayoutColors.LowAnchor"),
            Tint = Pick(value => value.LayoutColors?.Tint, value => value.LayoutColors.Tint, $"{variantName}.LayoutColors.Tint"),
            HighAnchor = Pick(value => value.LayoutColors?.HighAnchor, value => value.LayoutColors.HighAnchor, $"{variantName}.LayoutColors.HighAnchor"),
            SurfaceMix = Pick(value => value.LayoutColors?.SurfaceMix, value => value.LayoutColors.SurfaceMix, $"{variantName}.LayoutColors.SurfaceMix"),
            AccentOffset = Pick(value => value.LayoutColors?.AccentOffset, value => value.LayoutColors.AccentOffset, $"{variantName}.LayoutColors.AccentOffset"),
            HighlightOffset = Pick(value => value.LayoutColors?.HighlightOffset, value => value.LayoutColors.HighlightOffset, $"{variantName}.LayoutColors.HighlightOffset")
        };

        var borders = new FbThemeLayoutBorders
        {
            LightMix = Pick(value => value.Borders?.LightMix, value => value.Borders.LightMix, $"{variantName}.Borders.LightMix"),
            LightSize = Pick(value => value.Borders?.LightSize, value => value.Borders.LightSize, $"{variantName}.Borders.LightSize"),
            MediumMix = Pick(value => value.Borders?.MediumMix, value => value.Borders.MediumMix, $"{variantName}.Borders.MediumMix"),
            MediumSize = Pick(value => value.Borders?.MediumSize, value => value.Borders.MediumSize, $"{variantName}.Borders.MediumSize"),
            StrongMix = Pick(value => value.Borders?.StrongMix, value => value.Borders.StrongMix, $"{variantName}.Borders.StrongMix"),
            StrongSize = Pick(value => value.Borders?.StrongSize, value => value.Borders.StrongSize, $"{variantName}.Borders.StrongSize")
        };

        return new FbResolvedLayoutThemeVariant(variantName, colors, borders);
    }

    private static List<string> GetVariantNames(FbLayoutThemeSetup theme)
    {
        var names = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        void Add(string name)
        {
            if (seen.Add(name))
                names.Add(name);
        }

        Add(FbLayoutThemeVariants.Default);

        foreach (var standard in FbLayoutThemeVariants.Standard.Skip(1))
        {
            if (FindVariant(theme, standard) is not null)
                Add(standard);
        }

        for (var current = theme; current is not null; current = current.Parent)
        {
            foreach (var variant in current.Variants)
                Add(variant.Name);
        }

        return names;
    }

    private static string? FindThemeValue(FbLayoutThemeSetup theme, Func<FbLayoutThemeSetup, string?> selector)
    {
        for (var current = theme; current is not null; current = current.Parent)
        {
            var value = selector(current);
            if (value is not null)
                return value;
        }

        return null;
    }

    private static string? FindVariantValue(
        FbLayoutThemeSetup theme,
        string variantName,
        Func<FbLayoutThemeVariant, string?> selector)
    {
        for (var current = theme; current is not null; current = current.Parent)
        {
            var variant = current.Variants.FirstOrDefault(
                value => value.Name.Equals(variantName, StringComparison.OrdinalIgnoreCase));
            if (variant is null)
                continue;

            var value = selector(variant);
            if (value is not null)
                return value;
        }

        return null;
    }

    private static FbLayoutThemeVariant? FindVariant(FbLayoutThemeSetup theme, string variantName)
    {
        for (var current = theme; current is not null; current = current.Parent)
        {
            var variant = current.Variants.FirstOrDefault(
                value => value.Name.Equals(variantName, StringComparison.OrdinalIgnoreCase));
            if (variant is not null)
                return variant;
        }

        return null;
    }

    private static string Required(string? value, string name)
    {
        FbThemeCssNames.ValidateCssValue(value, name);
        return value!;
    }

    private static void ValidateDefinition(FbLayoutThemeSetup theme)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(theme.Name);
        FbThemeCssNames.Normalize(theme.Name);

        var visited = new HashSet<FbLayoutThemeSetup>(ReferenceEqualityComparer.Instance);
        for (var current = theme; current is not null; current = current.Parent)
        {
            if (!visited.Add(current))
                throw new InvalidOperationException($"The layout theme '{theme.Name}' contains an inheritance cycle.");
        }

        var variants = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var variant in theme.Variants)
        {
            if (!variants.Add(variant.Name))
                throw new InvalidOperationException($"The layout theme '{theme.Name}' contains duplicate variant '{variant.Name}'.");

            FbThemeCssNames.Normalize(variant.Name);
            ValidateValues(variant.LayoutColors);
            ValidateValues(variant.Borders);
        }

        ValidateValues(theme.Spacings);
        ValidateValues(theme.Corners);
    }

    private static void ValidateValues(FbThemeLayoutSpacings? values)
        => ValidateValues(values is null ? [] : [values.S, values.M, values.L, values.X]);

    private static void ValidateValues(FbThemeLayoutCorners? values)
        => ValidateValues(values is null ? [] : [values.S, values.M, values.L, values.X]);

    private static void ValidateValues(FbThemeLayoutColors? values)
        => ValidateValues(values is null ? [] :
            [values.LowAnchor, values.Tint, values.HighAnchor, values.SurfaceMix, values.AccentOffset, values.HighlightOffset]);

    private static void ValidateValues(FbThemeLayoutBorders? values)
        => ValidateValues(values is null ? [] :
            [values.LightMix, values.LightSize, values.MediumMix, values.MediumSize, values.StrongMix, values.StrongSize]);

    private static void ValidateValues(IEnumerable<string?> values)
    {
        foreach (var value in values)
        {
            if (value is not null)
                FbThemeCssNames.ValidateCssValue(value, "theme definition");
        }
    }
}

public static class FbLayoutThemeDefaults
{
    public static FbLayoutThemeSetup Create()
    {
        var defaultBorders = new FbThemeLayoutBorders
        {
            LightMix = "8%",
            LightSize = "0.0625rem",
            MediumMix = "14%",
            MediumSize = "0.0625rem",
            StrongMix = "22%",
            StrongSize = "0.125rem"
        };

        return new FbLayoutThemeSetup("Default")
        {
            Spacings = FbThemeLayoutSpacings.Default,
            Corners = FbThemeLayoutCorners.Default,
            Variants =
            [
                new(FbLayoutThemeVariants.Default)
                {
                    LayoutColors = Colors("#111113", "#34343a", "#f7f7f8"),
                    Borders = defaultBorders
                },
                new(FbLayoutThemeVariants.Selected)
                {
                    LayoutColors = Colors("#0d1c31", "#214d82", "#eef6ff", "10%", "16%", "26%")
                },
                new(FbLayoutThemeVariants.Focused)
                {
                    LayoutColors = Colors("#0b1d33", "#1f5f9f", "#f0f8ff", "10%", "18%", "28%")
                },
                new(FbLayoutThemeVariants.Error)
                {
                    LayoutColors = Colors("#2a0f14", "#5c1c28", "#fff5f6", "10%", "12%", "22%"),
                    Borders = new FbThemeLayoutBorders { LightMix = "14%", MediumMix = "24%", StrongMix = "38%" }
                },
                new(FbLayoutThemeVariants.Warning)
                {
                    LayoutColors = Colors("#2a1d08", "#6a4a10", "#fff8e1", "10%", "14%", "24%")
                },
                new(FbLayoutThemeVariants.Disabled)
                {
                    LayoutColors = Colors("#151517", "#303036", "#a5a5ad", "6%", "6%", "10%"),
                    Borders = new FbThemeLayoutBorders { LightMix = "6%", MediumMix = "9%", StrongMix = "14%" }
                },
                new(FbLayoutThemeVariants.Success)
                {
                    LayoutColors = Colors("#0e2418", "#1d5a38", "#ecfff4", "10%", "12%", "22%")
                },
                new(FbLayoutThemeVariants.Info)
                {
                    LayoutColors = Colors("#0b2030", "#174f73", "#eef9ff", "10%", "12%", "22%")
                }
            ]
        };
    }

    private static FbThemeLayoutColors Colors(
        string low,
        string tint,
        string high,
        string surface = "8%",
        string accent = "10%",
        string highlight = "18%")
        => new()
        {
            LowAnchor = low,
            Tint = tint,
            HighAnchor = high,
            SurfaceMix = surface,
            AccentOffset = accent,
            HighlightOffset = highlight
        };
}
