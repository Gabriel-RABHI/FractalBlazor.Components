using FractalBlazor.Components.Forms;
using FractalBlazor.WebSite.Flags;
using FractalBlazor.WebSite.StateModel;
using FractalBlazor.WebSite.ThemeEditor;
using Microsoft.Extensions.DependencyInjection;

namespace FractalBlazor.WebSite;

public static class WebSiteServiceCollectionExtensions
{
    /// <summary>
    /// Registers FractalBlazor, the states of the site and the services their model declares.
    /// Scoped states live as long as the circuit on the server, and as long as the page in
    /// WebAssembly.
    /// </summary>
    public static IServiceCollection AddFractalBlazorWebSite(this IServiceCollection services)
    {
        services.AddFractalBlazor();
        services.AddScoped<IThemeModeStore, CookieThemeModeStore>();
        services.AddSingleton<IFlagCatalog, ComponentFlagCatalog>();
        services.AddScoped<RootState>();
        services.AddScoped<ThemeEditorModel>();
        services.AddScoped<ThemeEditorAutoSave>();
        return services;
    }
}
