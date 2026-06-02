using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout
{
    public abstract class FbSimpleComponentBase : IComponent, IHandleAfterRender
    {
        protected RenderHandle _renderHandle;
        private bool _initialized;
        private bool _hasCalledOnAfterRender;

        public void Attach(RenderHandle renderHandle)
        {
            _renderHandle = renderHandle;
        }

        public virtual Task SetParametersAsync(ParameterView parameters)
        {
            OnBeforeParametersSet();
            parameters.SetParameterProperties(this);

            if (!_initialized)
            {
                _initialized = true;
                OnInitialized();
            }

            OnParametersSet();
            Render();
            return Task.CompletedTask;
        }

        protected virtual void OnInitialized() {
        }

        protected virtual void OnBeforeParametersSet() {
        }

        protected virtual void OnParametersSet() {
        }

        protected void Render()
        {
            if (_renderHandle.IsInitialized)
            {
                _renderHandle.Render(BuildRenderTree);
            }
        }

        protected virtual void BuildRenderTree(RenderTreeBuilder builder) {
        }

        Task IHandleAfterRender.OnAfterRenderAsync()
        {
            var firstRender = !_hasCalledOnAfterRender;
            _hasCalledOnAfterRender = true;

            OnAfterRender(firstRender);
            return OnAfterRenderAsync(firstRender);
        }

        protected virtual void OnAfterRender(bool firstRender) { }

        protected virtual Task OnAfterRenderAsync(bool firstRender) => Task.CompletedTask;

        protected Task InvokeAsync(Action work) => _renderHandle.Dispatcher.InvokeAsync(work);

        protected Task InvokeAsync(Func<Task> work) => _renderHandle.Dispatcher.InvokeAsync(work);

        protected void StateHasChanged() => Render();
    }
}
