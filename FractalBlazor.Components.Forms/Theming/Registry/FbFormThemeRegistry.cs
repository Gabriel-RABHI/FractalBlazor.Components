using System.Collections.ObjectModel;
using FractalBlazor.Components.Forms.Contracts;
using FractalBlazor.Components.Forms.Theming.Constants;
using FractalBlazor.Components.Forms.Theming.Model;
using FractalBlazor.Components.Forms.Theming.Solver;
using FractalBlazor.Components.Layout.Colors;
using FractalBlazor.Components.Layout.Theming.Model;

namespace FractalBlazor.Components.Forms.Theming.Registry;

public sealed class FbFormThemeRegistry : IFbFormThemeRegistry
{
    private readonly object _sync = new();
    private readonly Dictionary<string, FbThemeSetup> _themes = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, FbResolvedTheme> _resolved = new(StringComparer.OrdinalIgnoreCase);

    public FbFormThemeRegistry()
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

    public FbResolvedTheme Resolve(FbThemeSetup theme, string branch)
    {
        ArgumentNullException.ThrowIfNull(theme);
        ArgumentException.ThrowIfNullOrWhiteSpace(branch);

        lock (_sync)
        {
            ValidateDefinition(theme);

            if (theme.Parent is null ||
                !_themes.TryGetValue(theme.Parent.Name, out var registeredParent) ||
                !ReferenceEquals(theme.Parent, registeredParent))
            {
                throw new InvalidOperationException($"Theme '{theme.Name}' must inherit from a registered theme.");
            }

            return ResolveCore(theme, branch.Trim());
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
            // A size the theme gives no family follows the text family.
            ExtraSmallFontFamily = FindThemeValue(theme, value => value.Typography?.ExtraSmallFontFamily) ?? FbThemeFormTypography.TextFont,
            SmallFontFamily = FindThemeValue(theme, value => value.Typography?.SmallFontFamily) ?? FbThemeFormTypography.TextFont,
            MediumFontFamily = FindThemeValue(theme, value => value.Typography?.MediumFontFamily) ?? FbThemeFormTypography.TextFont,
            LargeFontFamily = FindThemeValue(theme, value => value.Typography?.LargeFontFamily) ?? FbThemeFormTypography.TextFont,
            ExtraLargeFontFamily = FindThemeValue(theme, value => value.Typography?.ExtraLargeFontFamily) ?? FbThemeFormTypography.TextFont,
            ExtraExtraLargeFontFamily = FindThemeValue(theme, value => value.Typography?.ExtraExtraLargeFontFamily) ?? FbThemeFormTypography.TextFont,
            FontSizeBase = Required(FindThemeValue(theme, value => value.Typography?.FontSizeBase), "Typography.FontSizeBase"),
            LineHeight = Required(FindThemeValue(theme, value => value.Typography?.LineHeight), "Typography.LineHeight"),
            ExtraSmallCoef = Required(FindThemeValue(theme, value => value.Typography?.ExtraSmallCoef), "Typography.ExtraSmallCoef"),
            SmallCoef = Required(FindThemeValue(theme, value => value.Typography?.SmallCoef), "Typography.SmallCoef"),
            MediumCoef = Required(FindThemeValue(theme, value => value.Typography?.MediumCoef), "Typography.MediumCoef"),
            LargeCoef = Required(FindThemeValue(theme, value => value.Typography?.LargeCoef), "Typography.LargeCoef"),
            ExtraLargeCoef = Required(FindThemeValue(theme, value => value.Typography?.ExtraLargeCoef), "Typography.ExtraLargeCoef"),
            ExtraExtraLargeCoef = Required(FindThemeValue(theme, value => value.Typography?.ExtraExtraLargeCoef), "Typography.ExtraExtraLargeCoef"),
            ThinWeight = Required(FindThemeValue(theme, value => value.Typography?.ThinWeight), "Typography.ThinWeight"),
            DefaultWeight = Required(FindThemeValue(theme, value => value.Typography?.DefaultWeight), "Typography.DefaultWeight"),
            BoldWeight = Required(FindThemeValue(theme, value => value.Typography?.BoldWeight), "Typography.BoldWeight"),
            ExtraBoldWeight = Required(FindThemeValue(theme, value => value.Typography?.ExtraBoldWeight), "Typography.ExtraBoldWeight")
        };

        var textVariant = new FbThemeFormTextVariantMix
        {
            DefaultHighMix = Required(FindBranchValue(theme, branchName, value => value.TextVariantMix?.DefaultHighMix), $"{branchName}.TextVariant.DefaultHighMix"),
            SubtleHighMix = Required(FindBranchValue(theme, branchName, value => value.TextVariantMix?.SubtleHighMix), $"{branchName}.TextVariant.SubtleHighMix"),
            MutedHighMix = Required(FindBranchValue(theme, branchName, value => value.TextVariantMix?.MutedHighMix), $"{branchName}.TextVariant.MutedHighMix"),
            HighlightHighMix = Required(FindBranchValue(theme, branchName, value => value.TextVariantMix?.HighlightHighMix), $"{branchName}.TextVariant.HighlightHighMix")
        };

        var masterTint = new FbThemeMasterTint
        {
            ColorTint = Required(FindBranchValue(theme, branchName, value => value.MasterTint?.ColorTint), $"{branchName}.MasterTint.ColorTint"),
            TintMix = Required(FindBranchValue(theme, branchName, value => value.MasterTint?.TintMix), $"{branchName}.MasterTint.TintMix")
        };

        var bordersMix = new FbThemeLayoutBordersMix
        {
            LightMix = Required(FindBranchValue(theme, branchName, value => value.BordersMix?.LightMix), $"{branchName}.BordersMix.LightMix"),
            MediumMix = Required(FindBranchValue(theme, branchName, value => value.BordersMix?.MediumMix), $"{branchName}.BordersMix.MediumMix"),
            StrongMix = Required(FindBranchValue(theme, branchName, value => value.BordersMix?.StrongMix), $"{branchName}.BordersMix.StrongMix")
        };

        var surfaceMix = new FbThemeLayoutSurfaceMix
        {
            NeutralMix = Required(FindBranchValue(theme, branchName, value => value.SurfaceMix?.NeutralMix), $"{branchName}.SurfaceMix.NeutralMix"),
            SurfaceOffset = Required(FindBranchValue(theme, branchName, value => value.SurfaceMix?.SurfaceOffset), $"{branchName}.SurfaceMix.SurfaceOffset"),
            AccentOffset = Required(FindBranchValue(theme, branchName, value => value.SurfaceMix?.AccentOffset), $"{branchName}.SurfaceMix.AccentOffset"),
            HighlightOffset = Required(FindBranchValue(theme, branchName, value => value.SurfaceMix?.HighlightOffset), $"{branchName}.SurfaceMix.HighlightOffset"),
            HoverOffset = Required(FindBranchValue(theme, branchName, value => value.SurfaceMix?.HoverOffset), $"{branchName}.SurfaceMix.HoverOffset")
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
            masterTint,
            textVariant,
            bordersMix,
            surfaceMix,
            new ReadOnlyDictionary<string, FbResolvedThemeVariant>(variants),
            variantNames.AsReadOnly());
    }

    private static FbResolvedThemeVariant ResolveVariant(FbThemeSetup theme, string branchName, string variantName)
    {
        FbResolvedThemeVariant? defaultVariant = null;
        if (!variantName.Equals(FbThemeVariants.Default, StringComparison.OrdinalIgnoreCase))
            defaultVariant = ResolveVariant(theme, branchName, FbThemeVariants.Default);

        string Pick(Func<FbThemeColorVariant, string?> selector, Func<FbResolvedThemeVariant, string?> fallback, string name)
            => Required(FindVariantValue(theme, branchName, variantName, selector) ??
                        (defaultVariant is null ? null : fallback(defaultVariant)), name);

        var layoutColors = new FbThemeLayoutColors
        {
            LowAnchor = Pick(value => value.LayoutColors?.LowAnchor, value => value.LayoutColors.LowAnchor, $"{branchName}.{variantName}.LayoutColors.LowAnchor"),
            MidAnchor = Pick(value => value.LayoutColors?.MidAnchor, value => value.LayoutColors.MidAnchor, $"{branchName}.{variantName}.LayoutColors.MidAnchor"),
            HighAnchor = Pick(value => value.LayoutColors?.HighAnchor, value => value.LayoutColors.HighAnchor, $"{branchName}.{variantName}.LayoutColors.HighAnchor")
        };

        return new FbResolvedThemeVariant(variantName, layoutColors);
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
        // The known variants in their order, functional ones then colors; those a theme adds after.
        foreach (var known in FbThemeVariants.All.Skip(1))
        {
            if (FindVariant(theme, branchName, known) is not null)
                Add(known);
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
        Func<FbThemeColorVariant, string?> selector)
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

    private static FbThemeColorVariant? FindVariant(FbThemeSetup theme, string branchName, string variantName)
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
            }

            ValidateValues(branch.MasterTint);
            ValidateValues(branch.TextVariantMix);
            ValidateValues(branch.BordersMix);
            ValidateValues(branch.SurfaceMix);
        }

        ValidateValues(theme.Spacings);
        ValidateValues(theme.Corners);
        ValidateValues(theme.Typography);
    }

    private static void ValidateValues(FbThemeLayoutSpacings? value)
        => Validate(value is null ? [] : [value.S, value.M, value.L, value.X]);

    private static void ValidateValues(FbThemeLayoutCorners? value)
        => Validate(value is null ? [] : [value.S, value.M, value.L, value.X]);

    private static void ValidateValues(FbThemeLayoutColors? value)
        => Validate(value is null ? [] : [value.LowAnchor, value.MidAnchor, value.HighAnchor]);

    private static void ValidateValues(FbThemeMasterTint? value)
    {
        if (value is null)
            return;

        Validate([value.ColorTint]);
        ValidatePercents(("MasterTint.TintMix", value.TintMix));
    }

    private static void ValidateValues(FbThemeLayoutBordersMix? value)
    {
        if (value is not null)
            ValidatePercents(("BordersMix.LightMix", value.LightMix), ("BordersMix.MediumMix", value.MediumMix),
                ("BordersMix.StrongMix", value.StrongMix));
    }

    private static void ValidateValues(FbThemeLayoutSurfaceMix? value)
    {
        if (value is not null)
            ValidatePercents(("SurfaceMix.NeutralMix", value.NeutralMix), ("SurfaceMix.SurfaceOffset", value.SurfaceOffset),
                ("SurfaceMix.AccentOffset", value.AccentOffset), ("SurfaceMix.HighlightOffset", value.HighlightOffset),
                ("SurfaceMix.HoverOffset", value.HoverOffset));
    }

    private static void ValidateValues(FbThemeFormTextVariantMix? value)
    {
        if (value is not null)
            ValidatePercents(("TextVariantMix.DefaultHighMix", value.DefaultHighMix), ("TextVariantMix.SubtleHighMix", value.SubtleHighMix),
                ("TextVariantMix.MutedHighMix", value.MutedHighMix), ("TextVariantMix.HighlightHighMix", value.HighlightHighMix));
    }

    /// <summary>
    /// The mixes are positions on the gradient: percentages between <c>0%</c> and <c>100%</c>.
    /// A value left out is inherited.
    /// </summary>
    private static void ValidatePercents(params (string Name, string? Value)[] values)
    {
        foreach (var (name, value) in values)
        {
            if (value is not null)
                FbThemeCssNames.ValidatePercent(value, name);
        }
    }

    private static void ValidateValues(FbThemeFormTypography? value)
        => Validate(value is null ? [] : [value.TextFontFamily, value.CodeFontFamily,
            value.ExtraSmallFontFamily, value.SmallFontFamily, value.MediumFontFamily,
            value.LargeFontFamily, value.ExtraLargeFontFamily, value.ExtraExtraLargeFontFamily, value.FontSizeBase, value.LineHeight, value.ExtraSmallCoef, value.SmallCoef, value.MediumCoef, value.LargeCoef, value.ExtraLargeCoef, value.ExtraExtraLargeCoef, value.ThinWeight, value.DefaultWeight, value.BoldWeight, value.ExtraBoldWeight]);

    private static void Validate(IEnumerable<string?> values)
    {
        foreach (var value in values)
        {
            if (value is not null)
                FbThemeCssNames.ValidateCssValue(value, "theme definition");
        }
    }
}
