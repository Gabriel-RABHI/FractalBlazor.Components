using System.Collections.Concurrent;
using System.Text;
using FractalBlazor.Components.Layout.Colors;

namespace FractalBlazor.Components.Layout.Theming.Model;

public sealed class FbThemeLayoutSpacings
{
    public string? S { get; init; }
    public string? M { get; init; }
    public string? L { get; init; }
    public string? X { get; init; }

    public static FbThemeLayoutSpacings Dense => new() { S = "0.125rem", M = "0.25rem", L = "0.5rem", X = "1rem" };
    public static FbThemeLayoutSpacings Default => new() { S = "0.25rem", M = "0.5rem", L = "0.875rem", X = "1.75rem" };
    public static FbThemeLayoutSpacings Large => new() { S = "0.375rem", M = "0.75rem", L = "1.25rem", X = "2.5rem" };
    public static FbThemeLayoutSpacings Spaced => new() { S = "0.5rem", M = "1rem", L = "1.75rem", X = "3.5rem" };
}

public sealed class FbThemeLayoutCorners
{
    public string? S { get; init; }
    public string? M { get; init; }
    public string? L { get; init; }
    public string? X { get; init; }

    public static FbThemeLayoutCorners Square => new() { S = "0rem", M = "0rem", L = "0rem", X = "0rem" };
    public static FbThemeLayoutCorners Default => new() { S = "0.125rem", M = "0.25rem", L = "0.4375rem", X = "1rem" };
    public static FbThemeLayoutCorners Rounded => new() { S = "0.25rem", M = "0.5rem", L = "0.875rem", X = "2rem" };
}

public sealed class FbThemeLayoutBordersMix
{
    public string? LightMix { get; init; }

    public string? MediumMix { get; init; }

    public string? StrongMix { get; init; }
}

public sealed class FbThemeLayoutSurfaceMix
{
    public string? SurfaceMix { get; init; }

    public string? AccentOffset { get; init; }

    public string? HighlightOffset { get; init; }

    /// <summary>
    /// Added to the background, frame and text mixes while a hoverable surface is hovered.
    /// </summary>
    public string? HoverOffset { get; init; }
}

public sealed class FbThemeLayoutColors
{
    public string? LowAnchor { get; init; }

    public string? HighAnchor { get; init; }
}

/// <summary>
/// The tint every color of a theme, or of a branch, leans toward. Every anchor keeps its
/// lightness and moves its hue and chroma toward <see cref="ColorTint"/> by
/// <see cref="TintMix"/>: a colored tint draws all the hues toward it, a gray one lowers the
/// intensity of every hue. Since each color of the stylesheets mixes two anchors, all of them
/// follow.
/// </summary>
public sealed class FbThemeMasterTint
{
    /// <summary>
    /// The color the anchors lean toward.
    /// </summary>
    public string? ColorTint { get; init; }

    /// <summary>
    /// How far the anchors lean, as a CSS percentage: <c>0%</c> leaves them as declared,
    /// <c>100%</c> gives them the hue and chroma of <see cref="ColorTint"/>.
    /// </summary>
    public string? TintMix { get; init; }

    /// <summary>
    /// Whether the tint changes anything: a color, and a mix above <c>0%</c>.
    /// </summary>
    public bool IsActive => !string.IsNullOrWhiteSpace(ColorTint) && FbColorMath.TryParsePercent(TintMix, out var mix) && mix > 0d;
}

public static class FbThemeCssNames
{
    /// <summary>
    /// Anchor pair redirected by a Layout variant: <c>bg-low-anchor</c> and <c>bg-high-anchor</c>.
    /// </summary>
    public const string BackgroundAnchors = "bg";

    /// <summary>
    /// Anchor pair redirected by a Forms variant, on top of <see cref="BackgroundAnchors"/>.
    /// </summary>
    public const string ForegroundAnchors = "fg";

    private const int ReferenceCacheCapacity = 256;
    private static readonly ConcurrentDictionary<(string Variant, bool Foreground, bool Invert), string> _references = new();

    public static string Normalize(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var builder = new StringBuilder(name.Length);
        var separatorPending = false;

        foreach (var character in name.Trim())
        {
            if (char.IsLetterOrDigit(character))
            {
                if (separatorPending && builder.Length > 0)
                    builder.Append('-');

                builder.Append(char.ToLowerInvariant(character));
                separatorPending = false;
            }
            else
            {
                separatorPending = true;
            }
        }

        if (builder.Length == 0)
            throw new ArgumentException("The name must contain at least one letter or digit.", nameof(name));

        return builder.ToString();
    }

    public static string ValueName(string variant, string token)
        => $"--fb-{Normalize(variant)}-{token}";

    public static string ActiveName(string token) => $"--fb-{token}";

    public static string ActiveReference(string variant, string token)
        => ActiveReference(variant, token, token);

    /// <summary>
    /// Points the active token <paramref name="token"/> at the value <paramref name="sourceToken"/> of a variant.
    /// </summary>
    public static string ActiveReference(string variant, string token, string sourceToken)
        => $"{ActiveName(token)}:var({ValueName(variant, sourceToken)});";

    /// <summary>
    /// Builds the inline redirections that activate a variant: the background anchors, then,
    /// when <paramref name="foreground"/> is set, the foreground anchors.
    /// </summary>
    /// <param name="normalizedVariant">Variant name, already normalized for CSS.</param>
    /// <param name="foreground">Also redirect the foreground anchors (Forms).</param>
    /// <param name="invert">
    /// Turns a dark variant into a light one and the other way round, as <c>Conception.md</c>
    /// describes: with foreground anchors, the background anchors take the foreground ones
    /// (the absolute foreground becomes the absolute background) and the other way round;
    /// with background anchors only, the low and high anchors are exchanged.
    /// </param>
    public static string VariantReferences(string normalizedVariant, bool foreground, bool invert)
    {
        var key = (normalizedVariant, foreground, invert);
        if (_references.TryGetValue(key, out var cached))
            return cached;

        const string bgLow = "bg-low-anchor", bgHigh = "bg-high-anchor", fgLow = "fg-low-anchor", fgHigh = "fg-high-anchor",
            frameLow = "frame-low-anchor", frameHigh = "frame-high-anchor";
        var builder = new StringBuilder();
        if (!invert)
        {
            builder.Append(ActiveReference(normalizedVariant, bgLow));
            builder.Append(ActiveReference(normalizedVariant, bgHigh));
            if (foreground)
            {
                builder.Append(ActiveReference(normalizedVariant, fgLow));
                builder.Append(ActiveReference(normalizedVariant, fgHigh));
            }
        }
        else if (foreground)
        {
            builder.Append(ActiveReference(normalizedVariant, bgLow, fgHigh));
            builder.Append(ActiveReference(normalizedVariant, bgHigh, fgLow));
            builder.Append(ActiveReference(normalizedVariant, fgLow, bgHigh));
            builder.Append(ActiveReference(normalizedVariant, fgHigh, bgLow));
        }
        else
        {
            builder.Append(ActiveReference(normalizedVariant, bgLow, bgHigh));
            builder.Append(ActiveReference(normalizedVariant, bgHigh, bgLow));
        }

        // Frame anchors, when the variant declares them. A reference to an undefined anchor
        // leaves the active one unset, and the frames fall back to the background anchors.
        builder.Append(ActiveReference(normalizedVariant, frameLow, invert ? frameHigh : frameLow));
        builder.Append(ActiveReference(normalizedVariant, frameHigh, invert ? frameLow : frameHigh));
        var references = builder.ToString();

        if (_references.Count < ReferenceCacheCapacity)
            _references.TryAdd(key, references);
        return references;
    }

    /// <summary>
    /// Declares <c>color-scheme</c> from the page background, so native widgets (date picker,
    /// select list, scrollbars) follow the theme. Nothing is written when the background is
    /// not a hexadecimal color.
    /// </summary>
    public static void AppendColorScheme(StringBuilder builder, string? pageBackground)
    {
        var scheme = FbColorMath.ColorSchemeOf(pageBackground);
        if (scheme is not null)
            builder.Append("color-scheme:").Append(scheme).Append(';');
    }

    /// <summary>
    /// The tint the anchors lean toward, and how far: set by <see cref="AppendTint"/>.
    /// </summary>
    public const string TintColor = "--fb-tint-color";

    public const string TintMix = "--fb-tint-mix";

    /// <summary>
    /// Declares the master tint, when it changes anything: <c>--fb-tint-color</c> and
    /// <c>--fb-tint-mix</c>, which every tinted anchor reads.
    /// </summary>
    public static void AppendTint(StringBuilder builder, FbThemeMasterTint tint)
    {
        if (!tint.IsActive)
            return;

        AppendDeclaration(builder, TintColor, tint.ColorTint!);
        AppendDeclaration(builder, TintMix, tint.TintMix!);
    }

    /// <summary>
    /// An anchor under the master tint. A hexadecimal anchor keeps its lightness: it is mixed
    /// with the tint brought to that lightness, so only its hue and chroma move. Any other CSS
    /// color is mixed with the tint as it is. Without an active tint, the anchor is returned
    /// unchanged.
    /// </summary>
    public static string Tinted(string anchor, FbThemeMasterTint tint)
    {
        if (!tint.IsActive)
            return anchor;

        if (!FbColorMath.IsHexColor(anchor))
            return $"color-mix(in oklab,{anchor},var({TintColor}) var({TintMix}))";

        var lightness = FbColorMath.Lightness(anchor).ToString("0.####", System.Globalization.CultureInfo.InvariantCulture);
        return $"color-mix(in oklab,{anchor},oklch(from var({TintColor}) {lightness} c h) var({TintMix}))";
    }

    public static void AppendDeclaration(StringBuilder builder, string name, string value)
    {
        ValidateCssValue(value, name);
        builder.Append(name).Append(':').Append(value).Append(';');
    }

    public static void ValidateCssValue(string? value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException($"The CSS value '{name}' is not resolved.");

        if (value.IndexOfAny([';', '{', '}', '\r', '\n']) >= 0)
            throw new InvalidOperationException($"The CSS value '{name}' contains an invalid character.");
    }

    public static void ValidateSelector(string selector)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(selector);
        if (selector.IndexOfAny(['{', '}', '\r', '\n']) >= 0)
            throw new ArgumentException("The CSS selector contains an invalid character.", nameof(selector));
    }
}
