using System.Collections.Concurrent;
using FractalBlazor.Components.Common.Contracts;
using FractalBlazor.Components.Forms.Components.Inputs;
using FractalBlazor.Components.Forms.Mapping;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace FractalBlazor.Components.Forms.Components.Panels
{
    /// <summary>
    /// A row of tabs that selects an index: <see cref="FbInputBase{TValue, TAction}.Value"/> is
    /// the selected tab. It only selects: showing the content of the selected tab is up to the
    /// page. Each label is rendered by <see cref="FbDynamicView{TSelector}"/> with
    /// <see cref="ItemSelector"/>, so labels may be strings, enumerations or objects.
    /// </summary>
    /// <remarks>
    /// ARIA <c>tablist</c> and <c>tab</c> roles. The arrows move the selection, <c>Home</c> and
    /// <c>End</c> go to the first and last tab, and only the selected tab is in the tab order.
    /// </remarks>
    /// <example><c>&lt;FbTabs TAction="FbSetValue&lt;int&gt;" @bind-Value="tab" Items="@(["Compte", "Mot de passe"])" /&gt;</c></example>
    public class FbTabs<TAction> : FbInputBase<int, TAction>
        where TAction : IStateAction<int>
    {
        private static readonly ConcurrentDictionary<Type, Type> _viewTypes = new();

        private ElementReference[] _tabs = [];

        /// <summary>
        /// Labels of the tabs, in order.
        /// </summary>
        [Parameter, EditorRequired]
        public IReadOnlyList<object> Items { get; set; } = [];

        /// <summary>
        /// Selector of the views that render the labels. Defaults to
        /// <see cref="FbViewSelector.Item"/>; any selector enumeration works.
        /// </summary>
        [Parameter]
        public Enum? ItemSelector { get; set; }

        /// <summary>
        /// The tab in the tab order: the selected one, or the first while none is.
        /// </summary>
        private int FocusableIndex => Value >= 0 && Value < Items.Count ? Value : 0;

        protected override void OnParametersSet()
        {
            if (_tabs.Length != Items.Count)
                _tabs = new ElementReference[Items.Count];
        }

        protected override void BuildControl(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", RootClasses(string.IsNullOrWhiteSpace(Classes) ? "fb-tabs" : "fb-tabs " + Classes));
            builder.AddAttribute(3, "role", "tablist");
            builder.AddAttribute(4, "data-fb-nav-keys", true);
            builder.AddAttribute(5, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, HandleKeyDownAsync));

            var focusable = FocusableIndex;
            for (var index = 0; index < Items.Count; index++)
            {
                var tab = index;
                var selected = index == Value;

                builder.OpenElement(6, "button");
                builder.SetKey(index);
                builder.AddAttribute(7, "type", "button");
                builder.AddAttribute(8, "role", "tab");
                builder.AddAttribute(9, "class", selected ? "fb-tab fb-tab-selected" : "fb-tab");
                builder.AddAttribute(10, "aria-selected", selected ? "true" : "false");
                builder.AddAttribute(11, "tabindex", index == focusable ? "0" : "-1");
                builder.AddAttribute(12, "disabled", IsDisabled);
                builder.AddAttribute(13, "onclick", EventCallback.Factory.Create(this, () => CommitAsync(tab)));
                builder.AddElementReferenceCapture(14, element => _tabs[tab] = element);
                BuildLabel(builder, 15, Items[index]);
                builder.CloseElement();
            }

            builder.CloseElement();
        }

        private void BuildLabel(RenderTreeBuilder builder, int sequence, object item)
        {
            var selector = ItemSelector ?? FbViewSelector.Item;
            var viewType = _viewTypes.GetOrAdd(selector.GetType(), static type => typeof(FbDynamicView<>).MakeGenericType(type));

            builder.OpenComponent(sequence, viewType);
            builder.AddAttribute(sequence + 1, "Item", item);
            builder.AddAttribute(sequence + 2, "Selector", selector);
            builder.CloseComponent();
        }

        private async Task HandleKeyDownAsync(KeyboardEventArgs args)
        {
            if (IsDisabled || Items.Count == 0)
                return;

            var current = FocusableIndex;
            int? next = args.Key switch
            {
                "ArrowRight" => (current + 1) % Items.Count,
                "ArrowLeft" => (current - 1 + Items.Count) % Items.Count,
                "Home" => 0,
                "End" => Items.Count - 1,
                _ => null
            };

            if (next is not { } index || index == Value)
                return;

            await CommitAsync(index);
            await FocusAsync(index);
        }

        private async Task FocusAsync(int index)
        {
            try
            {
                await _tabs[index].FocusAsync();
            }
            catch (Exception exception) when (exception is InvalidOperationException or Microsoft.JSInterop.JSException or Microsoft.JSInterop.JSDisconnectedException)
            {
                // The tabs are not rendered interactively (prerender, disconnected circuit).
            }
        }
    }
}
