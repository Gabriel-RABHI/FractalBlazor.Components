using FractalBlazor.Components.Common.Contracts;
using FractalBlazor.Components.Forms.Core;
using FractalBlazor.Components.Layout.Abstracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace FractalBlazor.Components.Forms.Components.Sensors
{
    /// <summary>
    /// Measures the width of its parent and sends its category to the state, as a
    /// <typeparamref name="TAction"/> to <see cref="Handler"/>, each time it changes: the
    /// state decides how the screen is laid out, and the views follow it with <c>if</c>.
    /// </summary>
    /// <remarks>
    /// It renders an empty, hidden element, and the browser calls .NET only when the
    /// category changes, never while the width moves inside one. Nothing is measured before
    /// the component is interactive: until then, the state keeps its own value.
    /// </remarks>
    public class FbWidthSensor<TAction> : FbAfterRenderComponentBase, IAsyncDisposable
        where TAction : IStateAction<FbWidthClass>
    {
        private ElementReference _marker;
        private DotNetObjectReference<FbWidthSensor<TAction>>? _self;
        private IJSObjectReference? _observer;
        private bool _disposed;

        [Inject]
        public IJSRuntime JS { get; set; } = default!;

        /// <summary>
        /// Category the state holds: nothing is sent while the measure equals it.
        /// </summary>
        [Parameter]
        public FbWidthClass Value { get; set; }

        /// <summary>
        /// Raised after the action is sent. Enables <c>@bind-Value</c>.
        /// </summary>
        [Parameter]
        public EventCallback<FbWidthClass> ValueChanged { get; set; }

        /// <summary>
        /// State object that receives the action through its <c>Handle(TAction)</c> method.
        /// </summary>
        [Parameter]
        public object? Handler { get; set; }

        /// <summary>
        /// Called by the browser when the width of the parent enters another category.
        /// </summary>
        [JSInvokable]
        public async Task OnWidthClassChanged(int category)
        {
            var value = (FbWidthClass)category;
            if (_disposed || value == Value)
                return;

            if (Handler is not null)
                FbActionDispatcher<TAction>.Dispatch(Handler, FbActionFactory<TAction, FbWidthClass>.Create(value));

            if (ValueChanged.HasDelegate)
                await ValueChanged.InvokeAsync(value);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender || _disposed)
                return;

            _self = DotNetObjectReference.Create(this);
            try
            {
                _observer = await JS.InvokeAsync<IJSObjectReference>("FractalBlazor.observeWidth", _marker, _self, FbWidthClasses.Thresholds);
            }
            catch (JSDisconnectedException)
            {
                // The circuit is gone: there is nothing left to measure.
            }
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "span");
            builder.AddAttribute(1, "class", "fb-width-sensor");
            builder.AddAttribute(2, "aria-hidden", "true");
            builder.AddElementReferenceCapture(3, reference => _marker = reference);
            builder.CloseElement();
        }

        public async ValueTask DisposeAsync()
        {
            _disposed = true;
            if (_observer is not null)
            {
                try
                {
                    await _observer.InvokeVoidAsync("dispose");
                    await _observer.DisposeAsync();
                }
                catch (JSDisconnectedException)
                {
                    // The circuit is gone: the browser side no longer exists.
                }
                _observer = null;
            }
            _self?.Dispose();
        }
    }
}
