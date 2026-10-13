using BenchmarkDotNet.Attributes;
using FractalBlazor.Components.Benchmarks.Editors;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FractalBlazor.Components.Benchmarks
{
    /// <summary>
    /// Renders a stack of editors with the server-side HTML renderer, one per applicant, each
    /// editing the five fields of the DTO: three texts, a date and a status.
    /// <c>FirstRender</c> instantiates and renders every field; <c>Rerender</c> changes the
    /// status of every applicant of an already rendered stack.
    /// </summary>
    [MemoryDiagnoser]
    [MediumRunJob]
    public class EditorBenchmarks
    {
        private ServiceProvider _services = default!;
        private HtmlRenderer _renderer = default!;
        private IReadOnlyList<ApplicantDto> _applicants = [];

        [Params(Library.FractalBlazor, Library.MudBlazor, Library.Radzen, Library.FluentUI)]
        public Library Library { get; set; }

        [Params(1000)]
        public int Rows { get; set; }

        [GlobalSetup(Target = nameof(FirstRender))]
        public void Setup()
        {
            _services = Scenarios.CreateServices();
            _applicants = ApplicantDto.Generate(Rows);
        }

        /// <summary>
        /// The stack is rendered once; each invocation of <see cref="Rerender"/> then renders
        /// all its editors again.
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
        public Task Rerender() => EditorListBase.Current!.BumpAsync();

        private Task RenderListAsync() => _renderer.Dispatcher.InvokeAsync(() =>
        {
            var parameters = ParameterView.FromDictionary(new Dictionary<string, object?> { [nameof(EditorListBase.Applicants)] = _applicants });
            return _renderer.RenderComponentAsync(Scenarios.Editors(Library), parameters);
        });
    }
}
