using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout
{
    public class FbUpdatable : FbSimpleComponentBase
    {
        private bool _periodic = true;
        private TimeSpan _interval = new TimeSpan(0, 0, 1);

        /// <summary>
        /// Child content
        /// </summary>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Periodic update setting
        /// </summary>
        [Parameter]
        public bool Periodic
        {
            get => _periodic;
            set
            {
                if (_periodic != value)
                {
                    _periodic = value;
                    if (!_periodic)
                        Updated = true;
                }
            }
        }

        /// <summary>
        /// One time update setting
        /// </summary>
        [Parameter]
        public bool OneTime { get => !Periodic; set => Periodic = !value; }

        /// <summary>
        /// Update interval
        /// </summary>
        [Parameter]
        public TimeSpan Interval
        {
            get => _interval;
            set => _interval = value;
        }

        public void Update()
        {
            Updated = false;
            LastUpdate = DateTime.Now;
        }

        public void UpdateIn(TimeSpan interval)
        {
            _interval = interval;
            Updated = false;
            LastUpdate = DateTime.Now;
        }

        internal DateTime LastUpdate { get; set; } = DateTime.Now;

        internal bool Updated
        {
            get;
            set;
        } = false;

        public void UpdateState()
        {
            InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }
        
        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
                BackgroundProcess.AddUpdatable(this);
            base.OnAfterRender(firstRender);
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.AddContent(0, ChildContent);
        }
    }
}
