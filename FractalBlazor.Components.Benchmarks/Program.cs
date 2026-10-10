using BenchmarkDotNet.Running;

BenchmarkSwitcher.FromAssembly(typeof(FractalBlazor.Components.Benchmarks.RenderBenchmarks).Assembly).Run(args);
