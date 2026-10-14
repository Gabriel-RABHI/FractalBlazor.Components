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
/// Each background is a block filled at the first level, on the page: the neutral of the
/// branch moved by the offset of its fill.
/// </summary>
public static class FbThemeContrast
{
    /// <param name="theme">Resolved theme, as <c>IFbFormThemeRegistry.Resolve</c> returns it.</param>
    /// <param name="inverted">
    /// Analyze the variants as <c>&lt;FbVariant Invert&gt;</c> renders them: the gradient
    /// reversed.
    /// </param>
    public static FbContrastReport Analyze(FbResolvedTheme theme, bool inverted = false)
    {
        ArgumentNullException.ThrowIfNull(theme);

        var entries = new List<FbContrastEntry>();
        var skipped = new List<string>();

        if (!TryPercent(theme.SurfaceMix.NeutralMix, out var neutral)
            || !TryPercent(theme.SurfaceMix.SurfaceOffset, out var surfaceOffset)
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

        var tint = theme.MasterTint;
        var tinted = tint.IsActive;
        if (tinted && !FbColorMath.IsHexColor(tint.ColorTint))
        {
            skipped.Add("All variants: the master tint is not a #RGB or #RRGGBB color, the ratios ignore it.");
            tinted = false;
        }
        FbColorMath.TryParsePercent(tint.TintMix, out var tintMix);
        string Tint(string anchor) => FbColorMath.Tint(anchor, tint.ColorTint!, tintMix);

        // The neutral of each background: the fill offset added to the neutral of the page.
        var backgrounds = new[]
        {
            (FbContrastBackground.Surface, Math.Min(100d, neutral + surfaceOffset)),
            (FbContrastBackground.Accent, Math.Min(100d, neutral + accentOffset)),
            (FbContrastBackground.Highlight, Math.Min(100d, neutral + highlightOffset))
        };
        var texts = new[]
        {
            (FbContrastText.Subtle, subtleMix),
            (FbContrastText.Muted, mutedMix),
            (FbContrastText.Default, defaultMix),
            (FbContrastText.Highlight, highlightMix)
        };

        foreach (var name in theme.VariantOrder)
        {
            var colors = theme.GetVariant(name).LayoutColors;
            var (low, mid, high) = (colors.LowAnchor, colors.MidAnchor, colors.HighAnchor);
            if (inverted)
                (low, high) = (high, low);

            if (!FbColorMath.IsHexColor(low) || !FbColorMath.IsHexColor(mid) || !FbColorMath.IsHexColor(high))
            {
                skipped.Add($"{name}: an anchor is not a #RGB or #RRGGBB color.");
                continue;
            }

            // The anchors the stylesheets paint: under the master tint, as the CSS writer sets them.
            if (tinted)
                (low, mid, high) = (Tint(low!), Tint(mid!), Tint(high!));

            foreach (var (background, blockNeutral) in backgrounds)
            {
                var backgroundColor = FbColorMath.Sample(low, mid, high, blockNeutral)!;
                foreach (var (text, mix) in texts)
                {
                    var textColor = FbColorMath.Sample(low, mid, high, FbColorMath.Relative(mix, blockNeutral))!;
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
