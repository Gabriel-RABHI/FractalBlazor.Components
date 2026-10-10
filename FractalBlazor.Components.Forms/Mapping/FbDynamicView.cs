using FractalBlazor.Components.Common.Contracts;
using FractalBlazor.Components.Forms.Contracts;
using FractalBlazor.Components.Layout.Abstracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Forms.Mapping
{
    /// <summary>
    /// Renders the view registered for the run-time type of <see cref="Item"/> and for
    /// <see cref="Selector"/>, and passes it the item as its <c>Item</c> parameter. When the
    /// item implements <see cref="INotifyStateChanged"/>, the view is re-rendered on each
    /// notification: the state drives the interface.
    /// </summary>
    public class FbDynamicView<TSelector> : FbComponentBase, IDisposable
        where TSelector : struct, Enum
    {
        private Type? _viewType;
        private INotifyStateChanged? _subscribedState;

        [Inject]
        public IFbViewRegistry<TSelector> Registry { get; set; } = default!;

        [Parameter, EditorRequired]
        public object? Item { get; set; }

        /// <summary>
        /// Which view of the item to render: <c>Viewer</c>, <c>Summary</c>, <c>Editor</c>…
        /// </summary>
        [Parameter]
        public TSelector Selector { get; set; }

        /// <summary>
        /// Extra parameters passed to the view.
        /// </summary>
        [Parameter(CaptureUnmatchedValues = true)]
        public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

        protected override void OnParametersSet()
        {
            if (!ReferenceEquals(Item, _subscribedState))
            {
                Unsubscribe();
                if (Item is INotifyStateChanged state)
                {
                    _subscribedState = state;
                    state.OnStateChanged += HandleStateChanged;
                }
            }

            _viewType = Item is null ? null : Registry.GetViewType(Item.GetType(), Selector);
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (_viewType is not null)
            {
                builder.OpenComponent(0, _viewType);
                builder.AddAttribute(1, "Item", Item);
                if (AdditionalAttributes is not null)
                    builder.AddMultipleAttributes(2, AdditionalAttributes);
                builder.CloseComponent();
            }
            else if (Item is not null)
            {
                builder.OpenElement(3, "div");
                builder.AddAttribute(4, "class", "fb-missing-view");
                builder.AddContent(5, $"No {Selector} view registered for type {Item.GetType().Name}");
                builder.CloseElement();
            }
        }

        private void HandleStateChanged() => InvokeAsync(StateHasChanged);

        public void Dispose() => Unsubscribe();

        private void Unsubscribe()
        {
            if (_subscribedState is null)
                return;

            _subscribedState.OnStateChanged -= HandleStateChanged;
            _subscribedState = null;
        }
    }
}
