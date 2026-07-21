using FractalBlazor.Components.Forms.Contracts;
using FractalBlazor.Components.Forms.Theming.Registry;
using FractalBlazor.Components.Layout;
using FractalBlazor.Components.Layout.Theming.Contracts;
using FractalBlazor.Components.Layout.Theming.Registry;
using Microsoft.Extensions.DependencyInjection;

namespace FractalBlazor.Components.Forms.Theming;

public static class FbThemeServiceCollectionExtensions
{
    public static IServiceCollection AddFractalBlazorTheming(
        this IServiceCollection services,
        Action<IFbFormThemeRegistry>? configure = null,
        Action<IFbLayoutThemeRegistry>? configureLayout = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var layoutRegistry = new FbLayoutThemeRegistry();
        configureLayout?.Invoke(layoutRegistry);
        services.AddSingleton<IFbLayoutThemeRegistry>(layoutRegistry);

        var registry = new FbFormThemeRegistry();
        configure?.Invoke(registry);
        services.AddSingleton<IFbFormThemeRegistry>(registry);

        return services;
    }
}
