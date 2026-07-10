using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout
{
    public abstract class FbComponentBase : IComponent
    {
        protected RenderHandle _renderHandle;
        private bool _initialized;

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

        protected Task InvokeAsync(Action work) => _renderHandle.Dispatcher.InvokeAsync(work);

        protected Task InvokeAsync(Func<Task> work) => _renderHandle.Dispatcher.InvokeAsync(work);

        protected void StateHasChanged() => Render();
    }
}
