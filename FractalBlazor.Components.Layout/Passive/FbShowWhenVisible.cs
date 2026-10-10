using System;
using System.Threading.Tasks;
using FractalBlazor.Components.Layout.Abstracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace FractalBlazor.Components.Layout
{
    /// <summary>
    /// Renders <see cref="WaitContent"/> until the component scrolls into the viewport,
    /// then renders <see cref="ChildContent"/>. Visibility comes from one
    /// <c>IntersectionObserver</c> per instance, released when the component is disposed.
    /// </summary>
    public sealed class FbShowWhenVisible : FbAfterRenderComponentBase, IAsyncDisposable
    {
        private ElementReference _anchor;
        private DotNetObjectReference<FbShowWhenVisible>? _self;
        private IJSObjectReference? _observer;
        private bool _visible;
        private bool _disposed;

        [Inject]
        public IJSRuntime JS { get; set; } = default!;

        /// <summary>
        /// Content shown once the component is visible.
        /// </summary>
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Content shown while waiting. Defaults to a <see cref="FbCircularProgress"/>.
        /// </summary>
        [Parameter]
        public RenderFragment? WaitContent { get; set; }

        /// <summary>
        /// Action invoked each time the component becomes visible.
        /// </summary>
        [Parameter]
        public Action? DoWhenVisible { get; set; }

        /// <summary>
        /// Go back to <see cref="WaitContent"/> when the component leaves the viewport.
        /// The content is then wrapped in a <c>div</c> that stays observed.
        /// </summary>
        [Parameter]
        public bool Reset { get; set; }

        [JSInvokable]
        public void OnVisibilityChanged(bool visible)
        {
            if (_disposed || visible == _visible || (!visible && !Reset))
                return;

            _visible = visible;
            if (visible)
                DoWhenVisible?.Invoke();
            StateHasChanged();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (_disposed)
                return;

            if (_observer is null && !_visible)
            {
                _self ??= DotNetObjectReference.Create(this);
                _observer = await JS.InvokeAsync<IJSObjectReference>("FractalBlazor.observeVisibility", _anchor, _self, Reset);
            }
            else if (_observer is not null && _visible && !Reset)
            {
                await ReleaseObserverAsync();
            }
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (Reset)
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "class", "fb-show-when-visible");
                builder.AddElementReferenceCapture(2, reference => _anchor = reference);
                if (_visible)
                    builder.AddContent(3, ChildContent);
                else
                    BuildWaitContent(builder);
                builder.CloseElement();
            }
            else if (!_visible)
            {
                builder.OpenElement(5, "div");
                builder.AddAttribute(6, "class", "fb-show-when-visible");
                builder.AddElementReferenceCapture(7, reference => _anchor = reference);
                BuildWaitContent(builder);
                builder.CloseElement();
            }
            else
            {
                builder.AddContent(8, ChildContent);
            }
        }

        private void BuildWaitContent(RenderTreeBuilder builder)
        {
            builder.OpenRegion(10);
            if (WaitContent is not null)
            {
                builder.AddContent(0, WaitContent);
            }
            else
            {
                builder.OpenElement(1, "div");
                builder.AddAttribute(2, "class", "fb-show-when-visible-wait");
                builder.OpenComponent<FbCircularProgress>(3);
                builder.CloseComponent();
                builder.CloseElement();
            }
            builder.CloseRegion();
        }

        private async Task ReleaseObserverAsync()
        {
            if (_observer is null)
                return;

            var observer = _observer;
            _observer = null;
            try
            {
                await observer.InvokeVoidAsync("dispose");
                await observer.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
                // The circuit is gone: the browser side no longer exists.
            }
        }

        public async ValueTask DisposeAsync()
        {
            _disposed = true;
            await ReleaseObserverAsync();
            _self?.Dispose();
        }
    }
}
