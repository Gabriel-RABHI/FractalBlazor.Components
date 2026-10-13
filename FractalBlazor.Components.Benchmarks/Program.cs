using BenchmarkDotNet.Running;
using FractalBlazor.Components.Benchmarks;

// "html" prints the size of the HTML each scenario produces; anything else runs the benchmarks.
if (args.FirstOrDefault() == "html")
    await HtmlSizes.PrintAsync();
else
    BenchmarkSwitcher.FromAssembly(typeof(RenderBenchmarks).Assembly).Run(args);
