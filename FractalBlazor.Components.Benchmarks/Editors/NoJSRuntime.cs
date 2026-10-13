using Microsoft.JSInterop;

namespace FractalBlazor.Components.Benchmarks.Editors
{
    /// <summary>
    /// JavaScript runtime that answers every call with a default value, and counts them. The
    /// fields of every library inject one, but the HTML renderer never runs their after-render
    /// code, where they call it: the <c>html</c> command prints the count.
    /// </summary>
    public sealed class NoJSRuntime : IJSRuntime
    {
        private static int _calls;

        public static int Calls => _calls;

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
        {
            Interlocked.Increment(ref _calls);
            return ValueTask.FromResult<TValue>(default!);
        }

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
            => InvokeAsync<TValue>(identifier, args);
    }
}
