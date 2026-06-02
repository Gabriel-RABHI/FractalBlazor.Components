using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace FractalBlazor.Components.Layout
{
    public class FbAdaptiveDivHeight : FbComponentBase, IAsyncDisposable
    {
        [Inject]
        public IJSRuntime JS { get; set; }

        /// <summary>
        /// Child content
        /// </summary>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Target element ID
        /// </summary>
        [Parameter]
        public string TargetId { get; set; }

        /// <summary>
        /// Gap height
        /// </summary>
        [Parameter]
        public int Gap { get; set; }

        /// <summary>
        /// Is in modal setting
        /// </summary>
        [Parameter]
        public bool InModal { get; set; }

        /// <summary>
        /// Hide scrollbar setting
        /// </summary>
        [Parameter]
        public bool HideScrollbar { get; set; }

        /// <summary>
        /// Bottom padding in Rem
        /// </summary>
        [Parameter]
        public double PaddingBottomRem { get; set; } = 1.2;

        /// <summary>
        /// Interval delay in milliseconds
        /// </summary>
        [Parameter]
        public int IntervalDelay { get; set; } = 1000;

        /// <summary>
        /// Custom inline CSS style
        /// </summary>
        [Parameter]
        public string Style { get; set; }

        /// <summary>
        /// Custom CSS classes
        /// </summary>
        [Parameter]
        public string Classes { get; set; }

        private string ComputedStyle
        {
            get
            {
                var baseStyle = $"overflow-y:scroll; scroll-behavior: smooth; {Style}";
                if (HideScrollbar)
                    baseStyle += " overflow: hidden;";

                return baseStyle;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("runFbAdaptDivHeightInterval", new object[] { TargetId, Gap, PaddingBottomRem, InModal, IntervalDelay });
            }
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                await JS.InvokeVoidAsync("stopFbAdaptDivHeightInterval", new object[] { TargetId });
            }
            catch (JSDisconnectedException)
            { }
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "id", TargetId);
            builder.AddAttribute(2, "style", ComputedStyle);
            builder.AddAttribute(3, "class", Classes);
            builder.AddContent(4, ChildContent);
            builder.CloseElement();
        }
    }
}
