using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace FractalBlazor.Components.Layout
{
    public class FbShowWhenOverPerformance : FbSimpleComponentBase, IAsyncDisposable
    {
        [Inject]
        public IJSRuntime JS { get; set; }

        /// <summary>
        /// Verification delay in milliseconds
        /// </summary>
        [Parameter]
        public int VerificationDelay { get; set; } = 1000;

        /// <summary>
        /// Scroll parent element ID
        /// </summary>
        [Parameter]
        public string ScrollParentID { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("runFbShowOnOverPerformance", new object[] { ScrollParentID, VerificationDelay });
            }
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                await JS.InvokeVoidAsync("stopFbShowOnOverPerformance");
            }
            catch (JSDisconnectedException)
            { }
        }
    }
}
