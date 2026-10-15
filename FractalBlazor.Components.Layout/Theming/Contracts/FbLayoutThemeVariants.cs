namespace FractalBlazor.Components.Layout.Theming.Contracts;

/// <summary>
/// The variants every theme defines: the functional ones (<see cref="Standard"/>), which say
/// what an element is (selected, an error…), and one per color of the base palette
/// (<see cref="Colors"/>), which only give it a color. Their rules are in the Layout
/// stylesheet.
/// </summary>
public static class FbLayoutThemeVariants
{
    public const string Default = "Default";
    public const string Selected = "Selected";
    public const string Error = "Error";
    public const string Warning = "Warning";
    public const string Disabled = "Disabled";
    public const string Success = "Success";
    public const string Info = "Info";

    public const string Red = "Red";
    public const string Orange = "Orange";
    public const string Amber = "Amber";
    public const string Yellow = "Yellow";
    public const string Lime = "Lime";
    public const string Green = "Green";
    public const string Emerald = "Emerald";
    public const string Teal = "Teal";
    public const string Cyan = "Cyan";
    public const string Sky = "Sky";
    public const string Blue = "Blue";
    public const string Indigo = "Indigo";
    public const string Violet = "Violet";
    public const string Purple = "Purple";
    public const string Fuchsia = "Fuchsia";
    public const string Pink = "Pink";
    public const string Rose = "Rose";

    public const string Slate = "Slate";
    public const string Gray = "Gray";
    public const string Zinc = "Zinc";
    public const string Neutral = "Neutral";
    public const string Stone = "Stone";
    public const string Taupe = "Taupe";
    public const string Mauve = "Mauve";
    public const string Mist = "Mist";
    public const string Olive = "Olive";

    /// <summary>
    /// The functional variants.
    /// </summary>
    public static readonly IReadOnlyList<string> Standard =
    [Default, Selected, Error, Warning, Disabled, Success, Info];

    /// <summary>
    /// One variant per color of the base palette, in its order: the chromatic colors, then
    /// the neutral ones.
    /// </summary>
    public static readonly IReadOnlyList<string> Colors =
    [
        Red, Orange, Amber, Yellow, Lime, Green, Emerald, Teal, Cyan, Sky, Blue, Indigo, Violet, Purple, Fuchsia, Pink, Rose,
        Slate, Gray, Zinc, Neutral, Stone, Taupe, Mauve, Mist, Olive
    ];

    /// <summary>
    /// The functional variants, then the colors.
    /// </summary>
    public static readonly IReadOnlyList<string> All = [.. Standard, .. Colors];

    public static string Normalize(string? variant)
        => string.IsNullOrWhiteSpace(variant) ? Default : variant.Trim();
}
