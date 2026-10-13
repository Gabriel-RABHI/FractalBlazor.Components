using BenchmarkDotNet.Attributes;
using FractalBlazor.Components.Benchmarks.Editors;
using FractalBlazor.Components.Benchmarks.Lists;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.Services;
using Radzen;

namespace FractalBlazor.Components.Benchmarks
{
    public enum Library
    {
        ComponentBase,
        FractalBlazor,
        MudBlazor,
        Radzen,
        FluentUI
    }

    /// <summary>
    /// Renders a list of framed rows, each holding one text, with the server-side HTML
    /// renderer. <c>FirstRender</c> instantiates and renders every component;
    /// <c>Rerender</c> changes the text of every row of an already rendered list.
    /// </summary>
    [MemoryDiagnoser]
    [MediumRunJob]
    public class RenderBenchmarks
    {
        private ServiceProvider _services = default!;
        private HtmlRenderer _renderer = default!;

        [Params(Library.ComponentBase, Library.FractalBlazor, Library.MudBlazor, Library.Radzen, Library.FluentUI)]
        public Library Library { get; set; }

        [Params(1000)]
        public int Rows { get; set; }

        [GlobalSetup(Target = nameof(FirstRender))]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddSingleton<IJSRuntime, NoJSRuntime>();
            services.AddMudServices();
            services.AddRadzenComponents();
            services.AddFluentUIComponents();
            _services = services.BuildServiceProvider();
        }

        /// <summary>
        /// The list is rendered once; each invocation of <see cref="Rerender"/> then renders
        /// all its rows again.
        /// </summary>
        [GlobalSetup(Target = nameof(Rerender))]
        public void SetupRendered()
        {
            Setup();
            _renderer = new HtmlRenderer(_services, _services.GetRequiredService<ILoggerFactory>());
            RenderListAsync().GetAwaiter().GetResult();
        }

        [GlobalCleanup]
        public async Task CleanupAsync()
        {
            if (_renderer is not null)
                await _renderer.DisposeAsync();
            await _services.DisposeAsync();
        }

        [Benchmark]
        public async Task FirstRender()
        {
            _renderer = new HtmlRenderer(_services, _services.GetRequiredService<ILoggerFactory>());
            await RenderListAsync();
            await _renderer.DisposeAsync();
        }

        [Benchmark]
        public Task Rerender() => ListBase.Current!.BumpAsync();

        private Task RenderListAsync() => _renderer.Dispatcher.InvokeAsync(() =>
        {
            var parameters = ParameterView.FromDictionary(new Dictionary<string, object?> { [nameof(ListBase.Rows)] = Rows });
            return Library switch
            {
                Library.FractalBlazor => _renderer.RenderComponentAsync<FractalList>(parameters),
                Library.MudBlazor => _renderer.RenderComponentAsync<MudList>(parameters),
                Library.Radzen => _renderer.RenderComponentAsync<RadzenList>(parameters),
                Library.FluentUI => _renderer.RenderComponentAsync<FluentList>(parameters),
                _ => _renderer.RenderComponentAsync<PlainList>(parameters)
            };
        });
    }
}
