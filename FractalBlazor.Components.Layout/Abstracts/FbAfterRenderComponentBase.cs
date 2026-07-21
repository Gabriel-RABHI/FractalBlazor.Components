using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace FractalBlazor.Components.Layout.Abstracts
{
    public abstract class FbAfterRenderComponentBase : FbComponentBase, IHandleAfterRender
    {
        private bool _hasCalledOnAfterRender;

        Task IHandleAfterRender.OnAfterRenderAsync()
        {
            var firstRender = !_hasCalledOnAfterRender;
            _hasCalledOnAfterRender = true;

            OnAfterRender(firstRender);
            return OnAfterRenderAsync(firstRender);
        }

        protected virtual void OnAfterRender(bool firstRender) { }

        protected virtual Task OnAfterRenderAsync(bool firstRender) => Task.CompletedTask;
    }
}
