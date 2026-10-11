using FractalBlazor.Components.Forms.Theming.Constants;
using FractalBlazor.WebSite.States;

namespace FractalBlazor.WebSite;

/// <summary>
/// The theme mode kept between visits. The selector writes the cookie; each host reads it
/// before the first render (the server during its render, WebAssembly through JS), so the
/// page never starts with the wrong theme.
/// </summary>
public static class ThemeCookie
{
    public const string Name = "fb-branch";

    public static ThemeMode Parse(string? value)
        => value == FbThemeBranches.Light ? ThemeMode.Light : ThemeMode.Dark;

    /// <summary>
    /// The theme branch of <paramref name="mode"/>, which is also the value of the cookie.
    /// </summary>
    public static string Branch(ThemeMode mode)
        => mode == ThemeMode.Light ? FbThemeBranches.Light : FbThemeBranches.Dark;
}
