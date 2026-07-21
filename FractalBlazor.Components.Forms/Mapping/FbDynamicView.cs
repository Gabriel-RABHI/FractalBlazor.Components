using FractalBlazor.Components.Forms.Contracts;
using FractalBlazor.Components.Layout.Abstracts;
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
        public IFbViewRegistry Registry { get; set; } = default!;

        // --- Parameters ---

        [Parameter, EditorRequired]
        public object? Item { get; set; }

        private INotifyStateChanged? _subscribedState;

        [Parameter(CaptureUnmatchedValues = true)]
        public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

        // --- Lifecycle ---

        protected override void OnParametersSet()
        {
            if (Item is not null)
            {
                if (!ReferenceEquals(Item, _subscribedState))
                {
                    Unsubscribe();
                    if (Item is INotifyStateChanged newState)
                    {
                        _subscribedState = newState;
                        _subscribedState.OnStateChanged += HandleStateChanged;
                    }
                }

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

        private void HandleStateChanged() => InvokeAsync(StateHasChanged);

        public void Dispose() => Unsubscribe();

        private void Unsubscribe()
        {
            if (_subscribedState != null)
            {
                _subscribedState.OnStateChanged -= HandleStateChanged;
                _subscribedState = null;
            }
        }
    }
}
