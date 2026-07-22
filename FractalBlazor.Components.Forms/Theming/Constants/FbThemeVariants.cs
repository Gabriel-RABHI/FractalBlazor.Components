namespace FractalBlazor.Components.Forms.Theming.Constants;

public static class FbThemeVariants
{
    public const string Default = "Default";
    public const string Selected = "Selected";
    public const string Error = "Error";
    public const string Warning = "Warning";
    public const string Disabled = "Disabled";
    public const string Success = "Success";
    public const string Info = "Info";

    public static readonly IReadOnlyList<string> Standard =
        [Default, Selected, Error, Warning, Disabled, Success, Info];

    public static string Normalize(string? variant)
        => string.IsNullOrWhiteSpace(variant) ? Default : variant.Trim();
}
