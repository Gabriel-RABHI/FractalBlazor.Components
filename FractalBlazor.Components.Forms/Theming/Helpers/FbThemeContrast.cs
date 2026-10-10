using FractalBlazor.Components.Forms.Theming.Solver;
using FractalBlazor.Components.Layout.Colors;

namespace FractalBlazor.Components.Forms.Theming.Helpers;

public enum FbContrastBackground : byte
{
    Surface,
    Accent,
    Highlight
}

public enum FbContrastText : byte
{
    Subtle,
    Muted,
    Default,
    Highlight
}

/// <summary>
/// One text color drawn on one background, inside one variant, as the browser will paint it.
/// </summary>
public sealed record FbContrastEntry(
    string Variant,
    FbContrastBackground Background,
    FbContrastText Text,
    string BackgroundColor,
    string TextColor,
    double Ratio)
{
    /// <summary>WCAG 2.2 AA for normal text (4.5:1).</summary>
    public bool MeetsAA => Ratio >= 4.5d;

    /// <summary>WCAG 2.2 AA for large text and user interface components (3:1).</summary>
    public bool MeetsAALarge => Ratio >= 3d;

    public override string ToString()
        => $"{Variant} {Text} on {Background}: {TextColor} on {BackgroundColor} = {Ratio:0.00}:1";
}

/// <summary>
/// Contrast of every text color on every background of every variant of a resolved theme.
/// </summary>
public sealed class FbContrastReport
{
    internal FbContrastReport(string theme, string branch, IReadOnlyList<FbContrastEntry> entries, IReadOnlyList<string> skipped)
    {
        Theme = theme;
        Branch = branch;
        Entries = entries;
        Skipped = skipped;
    }

    public string Theme { get; }

    public string Branch { get; }

    public IReadOnlyList<FbContrastEntry> Entries { get; }

    /// <summary>
    /// Variants that could not be computed, with the reason: anchors other than <c>#RGB</c> or
    /// <c>#RRGGBB</c>, or mixes other than plain percentages.
    /// </summary>
    public IReadOnlyList<string> Skipped { get; }

    public IEnumerable<FbContrastEntry> Below(double ratio) => Entries.Where(entry => entry.Ratio < ratio);
}

/// <summary>
/// Computes the colors produced by the stylesheet formulas and their WCAG contrast. Mixing
/// percentages keeps the palette coherent but guarantees no contrast: this report is the check.
/// </summary>
public static class FbThemeContrast
{
    /// <param name="theme">Resolved theme, as <c>IFbFormThemeRegistry.Resolve</c> returns it.</param>
    /// <param name="inverted">
    /// Analyze the variants as <c>&lt;FbVariant Invert&gt;</c> renders them: background and
    /// foreground anchors exchanged.
    /// </param>
    public static FbContrastReport Analyze(FbResolvedTheme theme, bool inverted = false)
    {
        ArgumentNullException.ThrowIfNull(theme);

        var entries = new List<FbContrastEntry>();
        var skipped = new List<string>();

        if (!TryPercent(theme.SurfaceMix.SurfaceMix, out var surfaceMix)
            || !TryPercent(theme.SurfaceMix.AccentOffset, out var accentOffset)
            || !TryPercent(theme.SurfaceMix.HighlightOffset, out var highlightOffset)
            || !TryPercent(theme.TextVariant.SubtleHighMix, out var subtleMix)
            || !TryPercent(theme.TextVariant.MutedHighMix, out var mutedMix)
            || !TryPercent(theme.TextVariant.DefaultHighMix, out var defaultMix)
            || !TryPercent(theme.TextVariant.HighlightHighMix, out var highlightMix))
        {
            skipped.Add("All variants: the surface or text mixes are not plain percentages.");
            return new FbContrastReport(theme.Name, theme.Branch, entries, skipped);
        }

        foreach (var name in theme.VariantOrder)
        {
            var variant = theme.GetVariant(name);
            var bgLow = variant.LayoutColors.LowAnchor;
            var bgHigh = variant.LayoutColors.HighAnchor;
            var fgLow = variant.FormColors.LowAnchor;
            var fgHigh = variant.FormColors.HighAnchor;
            if (inverted)
                (bgLow, bgHigh, fgLow, fgHigh) = (fgHigh, fgLow, bgHigh, bgLow);

            if (!FbColorMath.IsHexColor(bgLow) || !FbColorMath.IsHexColor(bgHigh)
                || !FbColorMath.IsHexColor(fgLow) || !FbColorMath.IsHexColor(fgHigh))
            {
                skipped.Add($"{name}: an anchor is not a #RGB or #RRGGBB color.");
                continue;
            }

            // Same formulas as the stylesheets: accent and highlight are offsets from the surface.
            var surface = FbColorMath.Mix(bgLow!, bgHigh!, surfaceMix);
            var backgrounds = new[]
            {
                (FbContrastBackground.Surface, surface),
                (FbContrastBackground.Accent, FbColorMath.Mix(surface, bgHigh!, accentOffset)),
                (FbContrastBackground.Highlight, FbColorMath.Mix(surface, bgHigh!, highlightOffset))
            };
            var texts = new[]
            {
                (FbContrastText.Subtle, FbColorMath.Mix(fgLow!, fgHigh!, subtleMix)),
                (FbContrastText.Muted, FbColorMath.Mix(fgLow!, fgHigh!, mutedMix)),
                (FbContrastText.Default, FbColorMath.Mix(fgLow!, fgHigh!, defaultMix)),
                (FbContrastText.Highlight, FbColorMath.Mix(fgLow!, fgHigh!, highlightMix))
            };

            foreach (var (background, backgroundColor) in backgrounds)
            {
                foreach (var (text, textColor) in texts)
                {
                    entries.Add(new FbContrastEntry(
                        name, background, text, backgroundColor, textColor,
                        FbColorMath.ContrastRatio(backgroundColor, textColor)));
                }
            }
        }

        return new FbContrastReport(theme.Name, theme.Branch, entries, skipped);
    }

    private static bool TryPercent(string? value, out double percent)
        => FbColorMath.TryParsePercent(value, out percent);
}
