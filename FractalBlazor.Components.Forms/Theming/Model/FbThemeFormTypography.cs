namespace FractalBlazor.Components.Forms.Theming.Model;

public sealed class FbThemeFormTypography
{
    // -------- Typo
    public string? TextFontFamily { get; init; }

    public string? CodeFontFamily { get; init; }

    public string? FontSizeBase { get; init; }

    public string? LineHeight { get; init; }

    // -------- Coefs
    public string? ExtraSmallCoef { get; init; }

    public string? SmallCoef { get; init; }

    public string? MediumCoef { get; init; }

    public string? LargeCoef { get; init; }

    public string? ExtraLargeCoef { get; init; }

    public string? ExtraExtraLargeCoef { get; init; }

    // -------- Weights
    public string? ThinWeight { get; init; }

    public string? DefaultWeight { get; init; }

    public string? BoldWeight { get; init; }

    public string? ExtraBoldWeight { get; init; }

    public static FbThemeFormTypography Compact => new()
    {
        TextFontFamily = "'text-font',system-ui,-apple-system,sans-serif",
        CodeFontFamily = "'code-font',monospace",
        FontSizeBase = "13px",
        LineHeight = "1.35",
        ExtraSmallCoef = "0.7",
        SmallCoef = "0.85",
        MediumCoef = "1",
        LargeCoef = "1.2",
        ExtraLargeCoef = "1.5",
        ExtraExtraLargeCoef = "2",
        ThinWeight = "300",
        DefaultWeight = "400",
        BoldWeight = "600",
        ExtraBoldWeight = "800"
    };

    public static FbThemeFormTypography Default => new()
    {
        TextFontFamily = "'text-font',system-ui,-apple-system,sans-serif",
        CodeFontFamily = "'code-font',monospace",
        FontSizeBase = "14px",
        LineHeight = "1.4",
        ExtraSmallCoef = "0.7",
        SmallCoef = "0.85",
        MediumCoef = "1",
        LargeCoef = "1.25",
        ExtraLargeCoef = "1.6",
        ExtraExtraLargeCoef = "2",
        ThinWeight = "300",
        DefaultWeight = "400",
        BoldWeight = "600",
        ExtraBoldWeight = "800"
    };

    public static FbThemeFormTypography Large => new()
    {
        TextFontFamily = "'text-font',system-ui,-apple-system,sans-serif",
        CodeFontFamily = "'code-font',monospace",
        FontSizeBase = "16px",
        LineHeight = "1.5",
        ExtraSmallCoef = "0.75",
        SmallCoef = "0.875",
        MediumCoef = "1",
        LargeCoef = "1.25",
        ExtraLargeCoef = "1.625",
        ExtraExtraLargeCoef = "2",
        ThinWeight = "300",
        DefaultWeight = "400",
        BoldWeight = "600",
        ExtraBoldWeight = "800"
    };
}
