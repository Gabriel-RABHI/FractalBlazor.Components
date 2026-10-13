using System;
using System.Threading.Tasks;
using FractalBlazor.Components.Layout.Abstracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace FractalBlazor.Components.Layout
{
    public class FbDropDown : FbAfterRenderComponentBase, IAsyncDisposable
    {
        [Inject]
        public IJSRuntime JS { get; set; } = default!;

        /// <summary>
        /// Child content acting as the anchor component/element (e.g. input, button, select box).
        /// </summary>
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// The content of the dropdown menu/zone.
        /// </summary>
        [Parameter]
        public RenderFragment? DropDownContent { get; set; }

        /// <summary>
        /// Gets or sets if the dropdown is currently open.
        /// </summary>
        [Parameter]
        public bool IsOpen { get; set; }

        /// <summary>
        /// Event callback triggered when the open state changes.
        /// </summary>
        [Parameter]
        public EventCallback<bool> IsOpenChanged { get; set; }

        /// <summary>
        /// Event callback triggered when the dropdown is closed via click-away/overlay.
        /// </summary>
        [Parameter]
        public EventCallback OnClose { get; set; }

        /// <summary>
        /// If true, clicking on the anchor element automatically toggles the open state.
        /// </summary>
        [Parameter]
        public bool AutoToggle { get; set; } = true;

        /// <summary>
        /// Custom style for the dropdown container.
        /// </summary>
        [Parameter]
        public string Style { get; set; } = "";

        /// <summary>
        /// Custom classes for the dropdown container.
        /// </summary>
        [Parameter]
        public string Classes { get; set; } = "";

        /// <summary>
        /// Custom style for the dropdown content container.
        /// </summary>
        [Parameter]
        public string DropDownStyle { get; set; } = "";

        /// <summary>
        /// Custom classes for the dropdown content container.
        /// </summary>
        [Parameter]
        public string DropDownClasses { get; set; } = "";

        /// <summary>
        /// Z-index of the dropdown container. The overlay will have ZIndex - 1.
        /// </summary>
        [Parameter]
        public int ZIndex { get; set; } = 9999;

        /// <summary>
        /// The anchor and the zone take the full width of the parent instead of their content's.
        /// </summary>
        [Parameter]
        public bool Block { get; set; }

        private bool _previousIsOpen;
        private bool _shouldInitDropdown;

        // True once the script has set up the open dropdown: only then is there something to
        // destroy. A dropdown never opened, or prerendered, never calls the script.
        private bool _initialized;
        private readonly string _containerId = "fb-ddc-" + Guid.NewGuid().ToString("N");
        private readonly string _dropdownId = "fb-ddd-" + Guid.NewGuid().ToString("N");

        protected override void OnParametersSet()
        {
            if (IsOpen && !_previousIsOpen)
            {
                _shouldInitDropdown = true;
            }
            else if (!IsOpen && _previousIsOpen)
            {
                _ = DestroyDropdownAsync();
            }
            _previousIsOpen = IsOpen;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (_shouldInitDropdown)
            {
                _shouldInitDropdown = false;
                try
                {
                    await JS.InvokeVoidAsync("FractalBlazor.initDropdown", _containerId, _dropdownId);
                    _initialized = true;
                }
                catch (Exception)
                {
                    // Catch potential errors if JS runtime is not available
                }
            }
        }

        private async Task ToggleDropdown()
        {
            IsOpen = !IsOpen;
            await IsOpenChanged.InvokeAsync(IsOpen);
            if (!IsOpen)
            {
                await OnClose.InvokeAsync();
            }
            StateHasChanged();
        }

        private async Task CloseDropdown()
        {
            IsOpen = false;
            await IsOpenChanged.InvokeAsync(false);
            await OnClose.InvokeAsync();
            StateHasChanged();
        }

        /// <summary>
        /// <c>Escape</c>, pressed anywhere inside the dropdown while it is open, closes it.
        /// </summary>
        private Task HandleKeyDownAsync(KeyboardEventArgs args)
            => IsOpen && args.Key == "Escape" ? CloseDropdown() : Task.CompletedTask;

        private async Task DestroyDropdownAsync()
        {
            if (!_initialized)
                return;
            _initialized = false;
            try
            {
                await JS.InvokeVoidAsync("FractalBlazor.destroyDropdown", _dropdownId);
            }
            catch (Exception)
            {
                // Catch potential errors if JS runtime is not available or disconnected
            }
        }

        public async ValueTask DisposeAsync()
        {
            await DestroyDropdownAsync();
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            // Outer container
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "id", _containerId);
            builder.AddAttribute(2, "class", $"fb-dropdown-container{(Block ? " fb-dropdown-block" : "")} {Classes}".TrimEnd());
            if (!string.IsNullOrWhiteSpace(Style))
                builder.AddAttribute(3, "style", Style);
            if (IsOpen)
                builder.AddAttribute(18, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, HandleKeyDownAsync));

            // Anchor wrapper
            builder.OpenElement(4, "div");
            builder.AddAttribute(5, "class", "fb-dropdown-anchor");
            if (AutoToggle)
            {
                builder.AddAttribute(6, "onclick", EventCallback.Factory.Create(this, ToggleDropdown));
                builder.AddAttribute(7, "style", "cursor: pointer;");
            }
            builder.AddContent(8, ChildContent);
            builder.CloseElement();

            if (IsOpen)
            {
                // Overlay for click-away detection
                builder.OpenElement(9, "div");
                builder.AddAttribute(10, "class", "fb-dropdown-overlay");
                builder.AddAttribute(11, "style", $"z-index: {ZIndex - 1};");
                builder.AddAttribute(12, "onclick", EventCallback.Factory.Create(this, CloseDropdown));
                builder.CloseElement();

                // Dropdown content wrapper
                builder.OpenElement(13, "div");
                builder.AddAttribute(14, "id", _dropdownId);
                builder.AddAttribute(15, "class", $"fb-dropdown-content {DropDownClasses}");
                builder.AddAttribute(16, "style", $"z-index: {ZIndex}; {DropDownStyle}");
                builder.AddContent(17, DropDownContent);
                builder.CloseElement();
            }

            builder.CloseElement(); // Close outer container
        }
    }
}
