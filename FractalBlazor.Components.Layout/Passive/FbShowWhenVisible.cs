using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace FractalBlazor.Components.Layout
{
    public class FbShowWhenVisible : FbComponentBase
    {
        private static object _locker = new object();
        private static Dictionary<Guid, FbShowWhenVisible> _recorded = new Dictionary<Guid, FbShowWhenVisible>();

        private bool _isVisibile = false;
        private Guid _myId = Guid.NewGuid();

        /// <summary>
        /// Child content
        /// </summary>
        [Parameter]
        public RenderFragment ChildContent { get; set; }
        
        /// <summary>
        /// Wait content
        /// </summary>
        [Parameter]
        public RenderFragment WaitContent { get; set; }

        /// <summary>
        /// Action when element becomes visible
        /// </summary>
        [Parameter]
        public Action DoWhenVisible { get; set; }

        /// <summary>
        /// Reset visibility state setting
        /// </summary>
        [Parameter]
        public bool Reset { get; set; } = false;

        protected override void OnInitialized()
        {
            lock (_locker)
                _recorded[_myId] = this;
        }

        public void BecomeVisible()
        {
            _isVisibile = true;
            InvokeAsync(() =>
            {
                if (DoWhenVisible != null)
                    DoWhenVisible();
                StateHasChanged();
            });
        }

        [JSInvokable]
        public static void VisibilityChangedMessageCaller(string cpntId)
        {
            var id = Guid.Parse(cpntId);
            lock (_locker)
            {
                if (_recorded.ContainsKey(id))
                {
                    _recorded[id].BecomeVisible();
                    _recorded.Remove(id);
                }
            }
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (!_isVisibile)
            {
                if (WaitContent == null)
                {
                    builder.OpenElement(0, "div");
                    builder.AddAttribute(1, "style", "text-align:center;padding:1rem;width:100%;");
                    builder.AddAttribute(2, "data-fb-visibility-id", _myId.ToString());
                    builder.AddAttribute(3, "data-no-reset", Reset.ToString().ToLower());
                    builder.OpenElement(4, "FbCircularProgress");
                    builder.CloseElement();
                    builder.CloseElement();
                }
                else
                {
                    builder.AddContent(5, WaitContent);
                }
            }
            else
            {
                builder.AddContent(6, ChildContent);
            }
        }
    }
}
