using FractalBlazor.Components.Forms.Contracts;
using FractalBlazor.Components.Forms.Core;
using FractalBlazor.Components.Forms.Theming;
using FractalBlazor.Components.Layout.Theming.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FractalBlazor.Components.Forms;

public static class FbServiceCollectionExtensions
{
    /// <summary>
    /// Registers everything Forms needs: the theme registries, and a view registry for every
    /// selector enumeration (<see cref="IFbViewRegistry{TSelector}"/> as an open generic), used
    /// by <c>FbDynamicView</c> and the lists of <c>FbSelect</c>.
    /// </summary>
    public static IServiceCollection AddFractalBlazor(
        this IServiceCollection services,
        Action<IFbFormThemeRegistry>? configure = null,
        Action<IFbLayoutThemeRegistry>? configureLayout = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddFractalBlazorTheming(configure, configureLayout);
        services.TryAddSingleton(typeof(IFbViewRegistry<>), typeof(FbViewRegistry<>));
        return services;
    }
}
