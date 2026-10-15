using System.Collections.Concurrent;
using System.Text;
using FractalBlazor.Components.Layout.Colors;
using FractalBlazor.Components.Layout.Theming.Contracts;

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

/// <summary>
/// Positions of the frame lines on the gradient, relative to the neutral of their block and
/// measured from the end of the gradient: a frame sits at
/// <c>100 − (100 − mix) × (100 − neutral) / 100</c> percent. At a neutral of 0 %, it sits at
/// its mix; the higher the neutral, the closer to the high anchor.
/// </summary>
public sealed class FbThemeLayoutBordersMix
{
    public string? LightMix { get; init; }

    public string? MediumMix { get; init; }

    public string? StrongMix { get; init; }
}

/// <summary>
/// The neutral of the page and how far each fill moves it. A filled block takes the neutral of
/// its nearest filled ancestor, adds the offset of its fill, and paints the gradient there; its
/// texts and frames are placed from this new neutral.
/// </summary>
public sealed class FbThemeLayoutSurfaceMix
{
    /// <summary>
    /// The neutral of the page, where the gradient starts: <c>0%</c> is the low anchor.
    /// </summary>
    public string? NeutralMix { get; init; }

    public string? SurfaceOffset { get; init; }

    public string? AccentOffset { get; init; }

    public string? HighlightOffset { get; init; }

    /// <summary>
    /// Added to the neutral of a hoverable element while it is hovered.
    /// </summary>
    public string? HoverOffset { get; init; }
}

/// <summary>
/// The gradient of a variant: three anchors, at 0 %, 50 % and 100 %. Backgrounds, texts and
/// frames are all positions on it.
/// </summary>
public sealed class FbThemeLayoutColors
{
    /// <summary>
    /// The color at 0 %: the page background at a neutral of 0 %.
    /// </summary>
    public string? LowAnchor { get; init; }

    /// <summary>
    /// The color at 50 %.
    /// </summary>
    public string? MidAnchor { get; init; }

    /// <summary>
    /// The color at 100 %: the strongest text.
    /// </summary>
    public string? HighAnchor { get; init; }
}

/// <summary>
/// The tint every color of a theme, or of a branch, leans toward. Every anchor keeps its
/// lightness and moves its hue and chroma toward <see cref="ColorTint"/> by
/// <see cref="TintMix"/>: a colored tint draws all the hues toward it, a gray one lowers the
/// intensity of every hue. Since each color of the stylesheets samples the anchors, all of
/// them follow.
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
    /// The three anchors of a variant, at 0 %, 50 % and 100 % of its gradient.
    /// </summary>
    public const string LowAnchor = "low-anchor";

    public const string MidAnchor = "mid-anchor";

    public const string HighAnchor = "high-anchor";

    /// <summary>
    /// Class of an element that carries its own variant: the colors are computed again on it.
    /// </summary>
    public const string VariantClass = "fb-variant";

    /// <summary>
    /// Class that reverses the gradient of the variant of an element.
    /// </summary>
    public const string InvertClass = "fb-invert";

    /// <summary>
    /// Variants whose rules are in the Layout stylesheet; a theme writes the rules of the
    /// variants it adds.
    /// </summary>
    public static IReadOnlyList<string> StandardVariants => FbLayoutThemeVariants.Standard;

    private const int ClassCacheCapacity = 256;
    private static readonly ConcurrentDictionary<(string Variant, bool Invert), string> _classes = new();

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
    /// The redirections that activate a variant: its three anchors, or, inverted, the low and
    /// high ones exchanged (the middle one stays). A theme points the active anchors at its
    /// <c>Default</c> variant with them; the variant rules use them too.
    /// </summary>
    /// <param name="normalizedVariant">Variant name, already normalized for CSS.</param>
    /// <param name="invert">Exchange the low and high anchors.</param>
    public static string VariantReferences(string normalizedVariant, bool invert = false)
        => invert
            ? ActiveReference(normalizedVariant, LowAnchor, HighAnchor) + ActiveReference(normalizedVariant, HighAnchor, LowAnchor)
            : ActiveReference(normalizedVariant, LowAnchor) + ActiveReference(normalizedVariant, MidAnchor) + ActiveReference(normalizedVariant, HighAnchor);

    /// <summary>
    /// The rules of a variant class: <c>.fb-v-error</c> redirects the three anchors to those of
    /// <c>Error</c>; with <c>fb-invert</c>, the low and high ones are exchanged. An element
    /// whose variant has no rule keeps the anchors of its parent.
    /// </summary>
    /// <param name="normalizedVariant">Variant name, already normalized for CSS.</param>
    public static string VariantRules(string normalizedVariant)
    {
        var selector = "." + VariantClassName(normalizedVariant);
        return $"{selector}{{{VariantReferences(normalizedVariant)}}}{selector}.{InvertClass}{{{VariantReferences(normalizedVariant, invert: true)}}}";
    }

    /// <summary>
    /// Appends the rules of the variants that are not standard: the Layout stylesheet holds
    /// the others.
    /// </summary>
    public static void AppendVariantRules(StringBuilder builder, IEnumerable<string> variants)
    {
        foreach (var variant in variants)
        {
            if (!StandardVariants.Contains(variant, StringComparer.OrdinalIgnoreCase))
                builder.Append(VariantRules(Normalize(variant)));
        }
    }

    /// <summary>
    /// The classes that put <paramref name="variant"/> on an element:
    /// <c>fb-variant fb-v-error</c>, plus <c>fb-invert</c> when <paramref name="invert"/> is set.
    /// An empty name means <c>Default</c>. Built once per name, then cached.
    /// </summary>
    public static string VariantClasses(string? variant, bool invert = false)
    {
        var name = string.IsNullOrWhiteSpace(variant) ? FbLayoutThemeVariants.Default : variant;
        var key = (name, invert);
        if (_classes.TryGetValue(key, out var cached))
            return cached;

        var classes = VariantClass + " " + VariantClassName(Normalize(name)) + (invert ? " " + InvertClass : "");
        if (_classes.Count < ClassCacheCapacity)
            _classes.TryAdd(key, classes);
        return classes;
    }

    private static string VariantClassName(string normalizedVariant) => "fb-v-" + normalizedVariant;

    /// <summary>
    /// Declares the positions of the gradient. The neutral and the offsets stay percentages,
    /// so that they add up in CSS; the frame mixes become fractions, since CSS cannot multiply
    /// two percentages.
    /// </summary>
    public static void AppendGradient(StringBuilder builder, FbThemeLayoutSurfaceMix surface, FbThemeLayoutBordersMix borders)
    {
        AppendDeclaration(builder, "--fb-neutral-mix", surface.NeutralMix!);
        AppendDeclaration(builder, "--fb-surface-offset", surface.SurfaceOffset!);
        AppendDeclaration(builder, "--fb-accent-offset", surface.AccentOffset!);
        AppendDeclaration(builder, "--fb-highlight-offset", surface.HighlightOffset!);
        AppendDeclaration(builder, "--fb-hover-offset", surface.HoverOffset!);
        AppendDeclaration(builder, "--fb-frame-light-mix", Fraction(borders.LightMix, "BordersMix.LightMix"));
        AppendDeclaration(builder, "--fb-frame-medium-mix", Fraction(borders.MediumMix, "BordersMix.MediumMix"));
        AppendDeclaration(builder, "--fb-frame-strong-mix", Fraction(borders.StrongMix, "BordersMix.StrongMix"));
    }

    /// <summary>
    /// Declares the three anchors of a variant, under the master tint.
    /// </summary>
    public static void AppendAnchors(StringBuilder builder, string variant, FbThemeLayoutColors colors, FbThemeMasterTint tint)
    {
        AppendDeclaration(builder, ValueName(variant, LowAnchor), Tinted(colors.LowAnchor!, tint));
        AppendDeclaration(builder, ValueName(variant, MidAnchor), Tinted(colors.MidAnchor!, tint));
        AppendDeclaration(builder, ValueName(variant, HighAnchor), Tinted(colors.HighAnchor!, tint));
    }

    /// <summary>
    /// A percentage of the theme as a fraction: <c>80%</c> gives <c>0.8</c>.
    /// </summary>
    public static string Fraction(string? percent, string name)
    {
        ValidatePercent(percent, name);
        FbColorMath.TryParsePercent(percent, out var value);
        return (value / 100d).ToString("0.######", System.Globalization.CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Refuses a mix that is not a percentage between <c>0%</c> and <c>100%</c>.
    /// </summary>
    public static void ValidatePercent(string? value, string name)
    {
        if (!(value is not null && value.Trim().EndsWith('%') && FbColorMath.TryParsePercent(value, out var percent)
              && percent is >= 0d and <= 100d))
            throw new InvalidOperationException($"{name} must be a percentage between 0% and 100%.");
    }

    /// <summary>
    /// Declares <c>color-scheme</c> from the page background, the gradient sampled at the
    /// neutral, so native widgets (date picker, select list, scrollbars) follow the theme.
    /// Nothing is written when an anchor is not a hexadecimal color.
    /// </summary>
    public static void AppendColorScheme(StringBuilder builder, FbThemeLayoutColors colors, string? neutralMix)
    {
        FbColorMath.TryParsePercent(neutralMix, out var neutral);
        var scheme = FbColorMath.ColorSchemeOf(FbColorMath.Sample(colors.LowAnchor, colors.MidAnchor, colors.HighAnchor, neutral));
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
