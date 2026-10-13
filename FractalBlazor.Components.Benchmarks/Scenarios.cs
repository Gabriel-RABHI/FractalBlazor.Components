using FractalBlazor.Components.Benchmarks.Editors;
using FractalBlazor.Components.Benchmarks.Lists;
using FractalBlazor.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.Services;
using Radzen;

namespace FractalBlazor.Components.Benchmarks
{
    /// <summary>
    /// What the benchmarks and the <c>html</c> command render: the services of the four
    /// libraries, and the root component of each scenario for each library.
    /// </summary>
    internal static class Scenarios
    {
        public static ServiceProvider CreateServices()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddSingleton<IJSRuntime, NoJSRuntime>();
            services.AddFractalBlazor();
            services.AddMudServices();
            services.AddRadzenComponents();
            services.AddFluentUIComponents();
            return services.BuildServiceProvider();
        }

        /// <summary>
        /// Root of the list of framed rows.
        /// </summary>
        public static Type List(Library library) => library switch
        {
            Library.FractalBlazor => typeof(FractalList),
            Library.MudBlazor => typeof(MudList),
            Library.Radzen => typeof(RadzenList),
            Library.FluentUI => typeof(FluentList),
            _ => typeof(PlainList)
        };

        /// <summary>
        /// Root of the stack of editors.
        /// </summary>
        public static Type Editors(Library library) => library switch
        {
            Library.MudBlazor => typeof(MudEditorList),
            Library.Radzen => typeof(RadzenEditorList),
            Library.FluentUI => typeof(FluentEditorList),
            _ => typeof(FractalEditorList)
        };
    }
}
