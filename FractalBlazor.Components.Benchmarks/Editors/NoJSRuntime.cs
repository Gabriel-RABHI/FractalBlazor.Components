using Microsoft.JSInterop;

namespace FractalBlazor.Components.Benchmarks.Editors
{
    /// <summary>
    /// JavaScript runtime that answers every call with a default value. Fields of both
    /// libraries inject one, but the HTML renderer never runs their after-render code,
    /// where they call it.
    /// </summary>
    public sealed class NoJSRuntime : IJSRuntime
    {
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
            => ValueTask.FromResult<TValue>(default!);

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
            => ValueTask.FromResult<TValue>(default!);
    }
}
