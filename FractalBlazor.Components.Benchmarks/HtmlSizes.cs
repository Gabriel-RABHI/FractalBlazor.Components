using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using FractalBlazor.Components.Benchmarks.Editors;
using FractalBlazor.Components.Benchmarks.Lists;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FractalBlazor.Components.Benchmarks
{
    /// <summary>
    /// The <c>html</c> command: renders each scenario once, as the benchmarks do, and prints
    /// the size of the HTML produced, raw and compressed, and its number of elements. Each
    /// scenario is then rendered again as by <c>Rerender</c>, and its components are disposed
    /// with the renderer, as at the end of <c>FirstRender</c>; the last two columns count the
    /// JavaScript calls of both renders, then of the disposal.
    /// </summary>
    internal static partial class HtmlSizes
    {
        private const int Rows = 1000;

        public static async Task PrintAsync()
        {
            await using var services = Scenarios.CreateServices();
            var applicants = ApplicantDto.Generate(Rows);

            Console.WriteLine("| Scenario | Library | HTML | gzip | Brotli | Elements | JS calls, renders | JS calls, disposal |");
            Console.WriteLine("| --- | --- | ---: | ---: | ---: | ---: | ---: | ---: |");
            foreach (var library in Enum.GetValues<Library>())
                Print("Rows", library, await RenderAsync(services, Scenarios.List(library), nameof(ListBase.Rows), Rows, () => ListBase.Current!.BumpAsync()));
            foreach (var library in Enum.GetValues<Library>().Where(library => library != Library.ComponentBase))
                Print("Editors", library, await RenderAsync(services, Scenarios.Editors(library), nameof(EditorListBase.Applicants), applicants, () => EditorListBase.Current!.BumpAsync()));

        }

        private static async Task<(string Html, int Calls, int DisposalCalls)> RenderAsync(ServiceProvider services, Type root, string parameter, object value, Func<Task> rerender)
        {
            var start = NoJSRuntime.Calls;
            var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
            var html = await renderer.Dispatcher.InvokeAsync(async () =>
            {
                var output = await renderer.RenderComponentAsync(root, ParameterView.FromDictionary(new Dictionary<string, object?> { [parameter] = value }));
                return output.ToHtmlString();
            });
            await rerender();
            var rendered = NoJSRuntime.Calls;
            await renderer.DisposeAsync();
            return (html, rendered - start, NoJSRuntime.Calls - rendered);
        }

        private static void Print(string scenario, Library library, (string Html, int Calls, int DisposalCalls) render)
        {
            var bytes = Encoding.UTF8.GetBytes(render.Html);
            var gzip = Compress(bytes, stream => new GZipStream(stream, CompressionLevel.SmallestSize, leaveOpen: true));
            var brotli = Compress(bytes, stream => new BrotliStream(stream, CompressionLevel.SmallestSize, leaveOpen: true));
            Console.WriteLine($"| {scenario} | {library} | {Kilobytes(bytes.Length)} | {Kilobytes(gzip)} | {Kilobytes(brotli)} | {Element().Count(render.Html)} | {render.Calls} | {render.DisposalCalls} |");
        }

        private static int Compress(byte[] bytes, Func<Stream, Stream> compressor)
        {
            using var output = new MemoryStream();
            using (var stream = compressor(output))
                stream.Write(bytes);
            return (int)output.Length;
        }

        private static string Kilobytes(int bytes) => (bytes / 1024.0).ToString("0.0 KB", System.Globalization.CultureInfo.InvariantCulture);

        [GeneratedRegex("<[a-zA-Z]")]
        private static partial Regex Element();
    }
}
