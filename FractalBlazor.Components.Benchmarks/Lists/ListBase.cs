using Microsoft.AspNetCore.Components;

namespace FractalBlazor.Components.Benchmarks.Lists
{
    /// <summary>
    /// Root of a benchmarked list: renders <see cref="Rows"/> framed rows, and can be
    /// re-rendered from the benchmark through <see cref="Current"/>.
    /// </summary>
    public abstract class ListBase : ComponentBase
    {
        public static ListBase? Current { get; private set; }

        [Parameter]
        public int Rows { get; set; }

        /// <summary>
        /// Changes the text of every row, so each one renders again.
        /// </summary>
        protected int Generation { get; private set; }

        protected override void OnInitialized() => Current = this;

        public Task BumpAsync() => InvokeAsync(() =>
        {
            Generation++;
            StateHasChanged();
        });
    }
}
