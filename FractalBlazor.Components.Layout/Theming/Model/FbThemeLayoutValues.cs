using System.Text;

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
}

public sealed class FbThemeLayoutColors
{
    public string? LowAnchor { get; init; }

    public string? HighAnchor { get; init; }
}

public sealed class FbThemeMasterTint
{
    public string? ColorTint { get; init; }

    public int TintPercent { get; init; }
}

public static class FbThemeCssNames
{
    public static readonly IReadOnlyList<string> LayoutVariantTokens =
    [
        "bg-low-anchor",
        "bg-high-anchor"
    ];

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
        => $"{ActiveName(token)}:var({ValueName(variant, token)});";

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
