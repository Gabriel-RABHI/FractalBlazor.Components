using FractalBlazor.Components.Forms.Contracts;
using FractalBlazor.Components.Layout;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Forms.Mapping
{
    public class FbDynamicView : FbComponentBase
    {
        private Type? _currentObjectType;
        private Type? _viewType;

        // --- Services ---

        [Inject]
        public IViewRegistry Registry { get; set; } = default!;

        // --- Parameters ---

        [Parameter, EditorRequired]
        public object? Item { get; set; }

        [Parameter(CaptureUnmatchedValues = true)]
        public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

        // --- Lifecycle ---

        protected override void OnParametersSet()
        {
            if (Item is not null)
            {
                var newType = Item.GetType();
                if (_currentObjectType != newType)
                {
                    _currentObjectType = newType;
                    _viewType = Registry.GetViewType(_currentObjectType);
                }
            } else
            {
                _currentObjectType = null;
                _viewType = null;
            }
        }

        // --- Rendering ---

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (_viewType is not null)
            {
                builder.OpenComponent(0, _viewType);
                builder.AddAttribute(1, "Item", Item);
                if (AdditionalAttributes is not null)
                {
                    builder.AddMultipleAttributes(2, AdditionalAttributes);
                }
                builder.CloseComponent();
            } else if (Item is not null)
            {
                builder.OpenElement(3, "div");
                builder.AddAttribute(4, "class", "fb-missing-view");
                builder.AddContent(5, $"No view registered for type {_currentObjectType?.Name}");
                builder.CloseElement();
            }
        }
    }
}
