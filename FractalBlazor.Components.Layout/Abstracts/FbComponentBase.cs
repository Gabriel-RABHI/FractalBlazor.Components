using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout.Abstracts
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
            if (_initialized)
                RestoreParameterDefaults();
            else
                CaptureParameterDefaults();

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

        /// <summary>
        /// Called once, before the first parameter assignment, after every constructor has run.
        /// Components that keep parameter values in private fields snapshot them here.
        /// </summary>
        protected virtual void CaptureParameterDefaults() {
        }

        /// <summary>
        /// Called before every later parameter assignment. Restoring the snapshot makes the
        /// rendered state depend only on the parameters of the current render: a boolean
        /// flag set to <c>false</c>, or no longer passed, falls back to its default.
        /// </summary>
        protected virtual void RestoreParameterDefaults() {
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
