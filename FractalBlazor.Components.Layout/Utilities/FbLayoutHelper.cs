namespace FractalBlazor.Components.Layout.Utilities;

public static class FbLayoutHelper
{
    public static string ToSpacingCss(FbSpacing value) => value switch
    {
        FbSpacing.S => "var(--fb-space-s)",
        FbSpacing.M => "var(--fb-space-m)",
        FbSpacing.L => "var(--fb-space-l)",
        FbSpacing.X => "var(--fb-space-x)",
        _ => throw InvalidSpacing(value)
    };

    public static string ToRadiusCss(FbSpacing value) => value switch
    {
        FbSpacing.S => "var(--fb-radius-s)",
        FbSpacing.M => "var(--fb-radius-m)",
        FbSpacing.L => "var(--fb-radius-l)",
        FbSpacing.X => "var(--fb-radius-x)",
        _ => throw InvalidSpacing(value)
    };

    private static ArgumentOutOfRangeException InvalidSpacing(FbSpacing value) =>
        new(nameof(value), value, $"{nameof(FbSpacing.None)} does not resolve to a CSS value.");
}
