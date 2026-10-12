using FractalBlazor.WebSite.StateModel;
using Microsoft.JSInterop;

namespace FractalBlazor.WebSite;

/// <summary>
/// The web site's implementation of <see cref="IThemeModeStore"/>: the mode goes to
/// <see cref="ThemeCookie"/>, written by the site script, on the server as in WebAssembly.
/// </summary>
internal sealed class CookieThemeModeStore(IJSRuntime js) : IThemeModeStore
{
    public void Save(ThemeMode mode) => _ = SaveAsync(mode);

    private async Task SaveAsync(ThemeMode mode)
    {
        try
        {
            await js.InvokeVoidAsync("fbWebSite.setCookie", ThemeCookie.Name, ThemeCookie.Branch(mode));
        }
        catch (JSDisconnectedException)
        {
            // The circuit is gone, and the page with it: there is nothing left to keep.
        }
    }
}
