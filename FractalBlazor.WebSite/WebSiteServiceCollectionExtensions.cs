using FractalBlazor.Components.Forms;
using FractalBlazor.WebSite.States;
using FractalBlazor.WebSite.ThemeEditor;
using Microsoft.Extensions.DependencyInjection;

namespace FractalBlazor.WebSite;

public static class WebSiteServiceCollectionExtensions
{
    /// <summary>
    /// Registers FractalBlazor and the states of the site. Scoped states live as long as the
    /// circuit on the server, and as long as the page in WebAssembly.
    /// </summary>
    public static IServiceCollection AddFractalBlazorWebSite(this IServiceCollection services)
    {
        services.AddFractalBlazor();
        services.AddScoped<RootState>();
        services.AddScoped<ThemeEditorModel>();
        return services;
    }
}
