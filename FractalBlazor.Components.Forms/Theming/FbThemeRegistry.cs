using System.Collections.ObjectModel;
using FractalBlazor.Components.Layout;
using Microsoft.Extensions.DependencyInjection;

namespace FractalBlazor.Components.Forms.Theming;

public interface IFbThemeRegistry
{
    FbThemeSetup Default { get; }
    void Register(FbThemeSetup theme);
    bool TryGet(string name, out FbThemeSetup theme);
    FbResolvedTheme Resolve(string theme, string branch);
}

public sealed class FbThemeRegistry : IFbThemeRegistry
{
    private readonly object _sync = new();
    private readonly Dictionary<string, FbThemeSetup> _themes = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, FbResolvedTheme> _resolved = new(StringComparer.OrdinalIgnoreCase);

    public FbThemeRegistry()
    {
        Default = FbThemeDefaults.Create();
        ValidateDefinition(Default);
        _themes.Add(Default.Name, Default);
        CacheTheme(Default);
    }

    public FbThemeSetup Default { get; }

    public void Register(FbThemeSetup theme)
    {
        ArgumentNullException.ThrowIfNull(theme);

        lock (_sync)
        {
            ValidateDefinition(theme);

            if (_themes.ContainsKey(theme.Name))
                throw new InvalidOperationException($"A theme named '{theme.Name}' is already registered.");

            if (theme.Parent is null)
                throw new InvalidOperationException($"Theme '{theme.Name}' must inherit from the default theme.");

            if (!_themes.TryGetValue(theme.Parent.Name, out var registeredParent) ||
                !ReferenceEquals(theme.Parent, registeredParent))
            {
                throw new InvalidOperationException(
                    $"The parent '{theme.Parent.Name}' of theme '{theme.Name}' must be registered first.");
            }

            _themes.Add(theme.Name, theme);

            try
            {
                CacheTheme(theme);
            }
            catch
            {
                _themes.Remove(theme.Name);
                RemoveCachedTheme(theme.Name);
                throw;
            }
        }
    }

    public bool TryGet(string name, out FbThemeSetup theme)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        lock (_sync)
            return _themes.TryGetValue(name.Trim(), out theme!);
    }

    public FbResolvedTheme Resolve(string theme, string branch)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(theme);
        ArgumentException.ThrowIfNullOrWhiteSpace(branch);

        lock (_sync)
        {
            var key = CacheKey(theme, branch);
            return _resolved.TryGetValue(key, out var resolved)
                ? resolved
                : throw new KeyNotFoundException($"Theme '{theme}' does not define branch '{branch}'.");
        }
    }

    private void CacheTheme(FbThemeSetup theme)
    {
        foreach (var branch in GetBranchNames(theme))
        {
            var resolved = ResolveCore(theme, branch);
            _resolved.Add(CacheKey(theme.Name, branch), resolved);
        }
    }

    private void RemoveCachedTheme(string theme)
    {
        var prefix = FbThemeCssNames.Normalize(theme) + "\u001f";
        foreach (var key in _resolved.Keys.Where(value => value.StartsWith(prefix, StringComparison.Ordinal)).ToArray())
            _resolved.Remove(key);
    }

    private static FbResolvedTheme ResolveCore(FbThemeSetup theme, string branchName)
    {
        if (FindBranch(theme, branchName) is null)
            throw new InvalidOperationException($"Theme '{theme.Name}' cannot resolve branch '{branchName}'.");

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

        var typography = new FbThemeFormTypography
        {
            TextFontFamily = Required(FindThemeValue(theme, value => value.Typography?.TextFontFamily), "Typography.TextFontFamily"),
            CodeFontFamily = Required(FindThemeValue(theme, value => value.Typography?.CodeFontFamily), "Typography.CodeFontFamily"),
            FontSizeBase = Required(FindThemeValue(theme, value => value.Typography?.FontSizeBase), "Typography.FontSizeBase"),
            LineHeight = Required(FindThemeValue(theme, value => value.Typography?.LineHeight), "Typography.LineHeight"),
            SmallCoef = Required(FindThemeValue(theme, value => value.Typography?.SmallCoef), "Typography.SmallCoef"),
            MediumCoef = Required(FindThemeValue(theme, value => value.Typography?.MediumCoef), "Typography.MediumCoef"),
            LargeCoef = Required(FindThemeValue(theme, value => value.Typography?.LargeCoef), "Typography.LargeCoef"),
            ExtraLargeCoef = Required(FindThemeValue(theme, value => value.Typography?.ExtraLargeCoef), "Typography.ExtraLargeCoef"),
            ThinWeight = Required(FindThemeValue(theme, value => value.Typography?.ThinWeight), "Typography.ThinWeight"),
            DefaultWeight = Required(FindThemeValue(theme, value => value.Typography?.DefaultWeight), "Typography.DefaultWeight"),
            BoldWeight = Required(FindThemeValue(theme, value => value.Typography?.BoldWeight), "Typography.BoldWeight"),
            ExtraBoldWeight = Required(FindThemeValue(theme, value => value.Typography?.ExtraBoldWeight), "Typography.ExtraBoldWeight")
        };

        var textVariant = new FbThemeFormTextVariant
        {
            DefaultHighMix = Required(FindBranchValue(theme, branchName, value => value.TextVariant?.DefaultHighMix), $"{branchName}.TextVariant.DefaultHighMix"),
            SubtleHighMix = Required(FindBranchValue(theme, branchName, value => value.TextVariant?.SubtleHighMix), $"{branchName}.TextVariant.SubtleHighMix"),
            MutedHighMix = Required(FindBranchValue(theme, branchName, value => value.TextVariant?.MutedHighMix), $"{branchName}.TextVariant.MutedHighMix"),
            HighlightHighMix = Required(FindBranchValue(theme, branchName, value => value.TextVariant?.HighlightHighMix), $"{branchName}.TextVariant.HighlightHighMix")
        };

        var variantNames = GetVariantNames(theme, branchName);
        var variants = new Dictionary<string, FbResolvedThemeVariant>(StringComparer.OrdinalIgnoreCase);

        foreach (var variantName in variantNames)
            variants.Add(variantName, ResolveVariant(theme, branchName, variantName));

        return new FbResolvedTheme(
            theme.Name,
            branchName,
            spacings,
            corners,
            typography,
            textVariant,
            new ReadOnlyDictionary<string, FbResolvedThemeVariant>(variants),
            variantNames.AsReadOnly());
    }

    private static FbResolvedThemeVariant ResolveVariant(FbThemeSetup theme, string branchName, string variantName)
    {
        FbResolvedThemeVariant? defaultVariant = null;
        if (!variantName.Equals(FbThemeVariants.Default, StringComparison.OrdinalIgnoreCase))
            defaultVariant = ResolveVariant(theme, branchName, FbThemeVariants.Default);

        string Pick(Func<FbThemeVariant, string?> selector, Func<FbResolvedThemeVariant, string?> fallback, string name)
            => Required(FindVariantValue(theme, branchName, variantName, selector) ??
                        (defaultVariant is null ? null : fallback(defaultVariant)), name);

        var layoutColors = new FbThemeLayoutColors
        {
            LowAnchor = Pick(value => value.LayoutColors?.LowAnchor, value => value.LayoutColors.LowAnchor, $"{branchName}.{variantName}.LayoutColors.LowAnchor"),
            Tint = Pick(value => value.LayoutColors?.Tint, value => value.LayoutColors.Tint, $"{branchName}.{variantName}.LayoutColors.Tint"),
            HighAnchor = Pick(value => value.LayoutColors?.HighAnchor, value => value.LayoutColors.HighAnchor, $"{branchName}.{variantName}.LayoutColors.HighAnchor"),
            SurfaceMix = Pick(value => value.LayoutColors?.SurfaceMix, value => value.LayoutColors.SurfaceMix, $"{branchName}.{variantName}.LayoutColors.SurfaceMix"),
            AccentOffset = Pick(value => value.LayoutColors?.AccentOffset, value => value.LayoutColors.AccentOffset, $"{branchName}.{variantName}.LayoutColors.AccentOffset"),
            HighlightOffset = Pick(value => value.LayoutColors?.HighlightOffset, value => value.LayoutColors.HighlightOffset, $"{branchName}.{variantName}.LayoutColors.HighlightOffset")
        };

        var formColors = new FbThemeFormColors
        {
            LowAnchor = Pick(value => value.FormColors?.LowAnchor, value => value.FormColors.LowAnchor, $"{branchName}.{variantName}.FormColors.LowAnchor"),
            HighAnchor = Pick(value => value.FormColors?.HighAnchor, value => value.FormColors.HighAnchor, $"{branchName}.{variantName}.FormColors.HighAnchor")
        };

        var borders = new FbThemeLayoutBorders
        {
            LightMix = Pick(value => value.Borders?.LightMix, value => value.Borders.LightMix, $"{branchName}.{variantName}.Borders.LightMix"),
            LightSize = Pick(value => value.Borders?.LightSize, value => value.Borders.LightSize, $"{branchName}.{variantName}.Borders.LightSize"),
            MediumMix = Pick(value => value.Borders?.MediumMix, value => value.Borders.MediumMix, $"{branchName}.{variantName}.Borders.MediumMix"),
            MediumSize = Pick(value => value.Borders?.MediumSize, value => value.Borders.MediumSize, $"{branchName}.{variantName}.Borders.MediumSize"),
            StrongMix = Pick(value => value.Borders?.StrongMix, value => value.Borders.StrongMix, $"{branchName}.{variantName}.Borders.StrongMix"),
            StrongSize = Pick(value => value.Borders?.StrongSize, value => value.Borders.StrongSize, $"{branchName}.{variantName}.Borders.StrongSize")
        };

        return new FbResolvedThemeVariant(variantName, layoutColors, formColors, borders);
    }

    private static List<string> GetBranchNames(FbThemeSetup theme)
    {
        var result = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (var current = theme; current is not null; current = current.Parent)
        {
            foreach (var branch in current.Branches)
            {
                if (seen.Add(branch.Name))
                    result.Add(branch.Name);
            }
        }

        return result;
    }

    private static List<string> GetVariantNames(FbThemeSetup theme, string branchName)
    {
        var result = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        void Add(string name)
        {
            if (seen.Add(name))
                result.Add(name);
        }

        Add(FbThemeVariants.Default);
        foreach (var standard in FbThemeVariants.Standard.Skip(1))
        {
            if (FindVariant(theme, branchName, standard) is not null)
                Add(standard);
        }

        for (var current = theme; current is not null; current = current.Parent)
        {
            var branch = current.Branches.FirstOrDefault(value => value.Name.Equals(branchName, StringComparison.OrdinalIgnoreCase));
            if (branch is null)
                continue;

            foreach (var variant in branch.Variants)
                Add(variant.Name);
        }

        return result;
    }

    private static string? FindThemeValue(FbThemeSetup theme, Func<FbThemeSetup, string?> selector)
    {
        for (var current = theme; current is not null; current = current.Parent)
        {
            var value = selector(current);
            if (value is not null)
                return value;
        }
        return null;
    }

    private static string? FindBranchValue(FbThemeSetup theme, string branchName, Func<FbThemeBranch, string?> selector)
    {
        for (var current = theme; current is not null; current = current.Parent)
        {
            var branch = current.Branches.FirstOrDefault(value => value.Name.Equals(branchName, StringComparison.OrdinalIgnoreCase));
            if (branch is null)
                continue;

            var value = selector(branch);
            if (value is not null)
                return value;
        }
        return null;
    }

    private static string? FindVariantValue(
        FbThemeSetup theme,
        string branchName,
        string variantName,
        Func<FbThemeVariant, string?> selector)
    {
        for (var current = theme; current is not null; current = current.Parent)
        {
            var branch = current.Branches.FirstOrDefault(value => value.Name.Equals(branchName, StringComparison.OrdinalIgnoreCase));
            var variant = branch?.Variants.FirstOrDefault(value => value.Name.Equals(variantName, StringComparison.OrdinalIgnoreCase));
            if (variant is null)
                continue;

            var value = selector(variant);
            if (value is not null)
                return value;
        }
        return null;
    }

    private static FbThemeBranch? FindBranch(FbThemeSetup theme, string branchName)
    {
        for (var current = theme; current is not null; current = current.Parent)
        {
            var branch = current.Branches.FirstOrDefault(value => value.Name.Equals(branchName, StringComparison.OrdinalIgnoreCase));
            if (branch is not null)
                return branch;
        }
        return null;
    }

    private static FbThemeVariant? FindVariant(FbThemeSetup theme, string branchName, string variantName)
    {
        for (var current = theme; current is not null; current = current.Parent)
        {
            var branch = current.Branches.FirstOrDefault(value => value.Name.Equals(branchName, StringComparison.OrdinalIgnoreCase));
            var variant = branch?.Variants.FirstOrDefault(value => value.Name.Equals(variantName, StringComparison.OrdinalIgnoreCase));
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

    private static string CacheKey(string theme, string branch)
        => FbThemeCssNames.Normalize(theme) + "\u001f" + FbThemeCssNames.Normalize(branch);

    private static void ValidateDefinition(FbThemeSetup theme)
    {
        FbThemeCssNames.Normalize(theme.Name);

        var visited = new HashSet<FbThemeSetup>(ReferenceEqualityComparer.Instance);
        for (var current = theme; current is not null; current = current.Parent)
        {
            if (!visited.Add(current))
                throw new InvalidOperationException($"Theme '{theme.Name}' contains an inheritance cycle.");
        }

        var branches = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var branch in theme.Branches)
        {
            FbThemeCssNames.Normalize(branch.Name);
            if (!branches.Add(branch.Name))
                throw new InvalidOperationException($"Theme '{theme.Name}' contains duplicate branch '{branch.Name}'.");

            var variants = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var variant in branch.Variants)
            {
                FbThemeCssNames.Normalize(variant.Name);
                if (!variants.Add(variant.Name))
                    throw new InvalidOperationException($"Branch '{branch.Name}' contains duplicate variant '{variant.Name}'.");

                ValidateValues(variant.LayoutColors);
                ValidateValues(variant.FormColors);
                ValidateValues(variant.Borders);
            }

            ValidateValues(branch.TextVariant);
        }

        ValidateValues(theme.Spacings);
        ValidateValues(theme.Corners);
        ValidateValues(theme.Typography);
    }

    private static void ValidateValues(FbThemeLayoutSpacings? value) => Validate(value is null ? [] : [value.S, value.M, value.L, value.X]);
    private static void ValidateValues(FbThemeLayoutCorners? value) => Validate(value is null ? [] : [value.S, value.M, value.L, value.X]);
    private static void ValidateValues(FbThemeLayoutColors? value) => Validate(value is null ? [] : [value.LowAnchor, value.Tint, value.HighAnchor, value.SurfaceMix, value.AccentOffset, value.HighlightOffset]);
    private static void ValidateValues(FbThemeLayoutBorders? value) => Validate(value is null ? [] : [value.LightMix, value.LightSize, value.MediumMix, value.MediumSize, value.StrongMix, value.StrongSize]);
    private static void ValidateValues(FbThemeFormColors? value) => Validate(value is null ? [] : [value.LowAnchor, value.HighAnchor]);
    private static void ValidateValues(FbThemeFormTextVariant? value) => Validate(value is null ? [] : [value.DefaultHighMix, value.SubtleHighMix, value.MutedHighMix, value.HighlightHighMix]);
    private static void ValidateValues(FbThemeFormTypography? value) => Validate(value is null ? [] : [value.TextFontFamily, value.CodeFontFamily, value.FontSizeBase, value.LineHeight, value.SmallCoef, value.MediumCoef, value.LargeCoef, value.ExtraLargeCoef, value.ThinWeight, value.DefaultWeight, value.BoldWeight, value.ExtraBoldWeight]);

    private static void Validate(IEnumerable<string?> values)
    {
        foreach (var value in values)
        {
            if (value is not null)
                FbThemeCssNames.ValidateCssValue(value, "theme definition");
        }
    }
}

public static class FbThemeServiceCollectionExtensions
{
    public static IServiceCollection AddFractalBlazorTheming(
        this IServiceCollection services,
        Action<IFbThemeRegistry>? configure = null,
        Action<IFbLayoutThemeRegistry>? configureLayout = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var layoutRegistry = new FbLayoutThemeRegistry();
        configureLayout?.Invoke(layoutRegistry);
        services.AddSingleton<IFbLayoutThemeRegistry>(layoutRegistry);

        var registry = new FbThemeRegistry();
        configure?.Invoke(registry);
        services.AddSingleton<IFbThemeRegistry>(registry);

        return services;
    }
}
