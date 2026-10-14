using System.Collections.Concurrent;
using FractalBlazor.Components.Common.Contracts;
using FractalBlazor.Components.Forms.Components.Display;
using FractalBlazor.Components.Forms.Components.Inputs;
using FractalBlazor.Components.Forms.Mapping;
using FractalBlazor.Components.Layout;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace FractalBlazor.Components.Forms.Components.Selectors
{
    /// <summary>
    /// Chooses one element among <see cref="Items"/>, without knowing how elements look: the
    /// current value and every option are rendered by <see cref="FbDynamicView{TSelector}"/>
    /// with <see cref="ItemSelector"/>. A list can therefore mix strings, enumerations and
    /// objects, each displayed by its own view; simple values need no view at all.
    /// </summary>
    /// <remarks>
    /// The options open in an <see cref="FbDropDown"/>, closed by a click outside or by
    /// <c>Escape</c>. From the keyboard, arrows, <c>Home</c> and <c>End</c> move through the
    /// options, <c>Enter</c> or <c>Space</c> choose, <c>Tab</c> leaves.
    /// </remarks>
    public class FbSelect<TValue, TAction> : FbInputBase<TValue, TAction>
        where TAction : IStateAction<TValue>
    {
        private static readonly ConcurrentDictionary<Type, Type> _viewTypes = new();

        private readonly string _id = "fb-sel-" + Guid.NewGuid().ToString("N");
        private readonly RenderFragment _anchor;
        private readonly RenderFragment _zone;
        private ElementReference _anchorElement;
        private TValue _shown = default!;
        private bool _open;
        private int _active = -1;

        public FbSelect()
        {
            _anchor = BuildAnchor;
            _zone = BuildZone;
        }

        [Parameter, EditorRequired]
        public IReadOnlyList<TValue> Items { get; set; } = [];

        /// <summary>
        /// Selector of the views that render the elements. Defaults to
        /// <see cref="FbViewSelector.Item"/>; any selector enumeration works.
        /// </summary>
        [Parameter]
        public Enum? ItemSelector { get; set; }

        /// <summary>
        /// Text shown while the value is not one of the items.
        /// </summary>
        [Parameter]
        public string? Placeholder { get; set; }

        [Parameter]
        public RenderFragment? StartContent { get; set; }

        [Parameter]
        public RenderFragment? EndContent { get; set; }

        private int SelectedIndex => IndexOf(_shown);

        private string OptionId(int index) => $"{_id}-{index}";

        protected override void OnParametersSet() => _shown = Value;

        protected override void BuildControl(RenderTreeBuilder builder)
        {
            builder.OpenComponent<FbDropDown>(0);
            builder.AddAttribute(1, nameof(FbDropDown.IsOpen), _open);
            builder.AddAttribute(2, nameof(FbDropDown.IsOpenChanged), EventCallback.Factory.Create<bool>(this, SetOpen));
            builder.AddAttribute(3, nameof(FbDropDown.AutoToggle), !IsDisabled);
            builder.AddAttribute(4, nameof(FbDropDown.Block), true);
            builder.AddAttribute(5, nameof(FbDropDown.Classes), RootClasses($"fb-control fb-select {Classes}".TrimEnd()));
            builder.AddAttribute(6, nameof(FbDropDown.Style), VariantStyle);
            builder.AddAttribute(7, nameof(FbDropDown.DropDownClasses), "fb-select-zone");
            builder.AddAttribute(8, nameof(FbDropDown.ChildContent), _anchor);
            builder.AddAttribute(9, nameof(FbDropDown.DropDownContent), _zone);
            builder.CloseComponent();
        }

        private void BuildAnchor(RenderTreeBuilder builder)
        {
            var selected = SelectedIndex;

            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "fb-select-anchor");
            builder.AddAttribute(2, "role", "combobox");
            builder.AddAttribute(3, "tabindex", IsDisabled ? "-1" : "0");
            builder.AddAttribute(4, "aria-haspopup", "listbox");
            builder.AddAttribute(5, "aria-expanded", _open ? "true" : "false");
            builder.AddAttribute(6, "aria-controls", $"{_id}-list");
            builder.AddAttribute(7, "aria-activedescendant", _open && _active >= 0 ? OptionId(_active) : null);
            builder.AddAttribute(8, "aria-disabled", IsDisabled ? "true" : null);
            builder.AddAttribute(9, "data-fb-nav-keys", true);
            builder.AddAttribute(10, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, HandleKeyDownAsync));
            builder.AddElementReferenceCapture(11, element => _anchorElement = element);

            builder.OpenComponent<FbRow>(12);
            builder.AddAttribute(13, nameof(FbRow.EO), _open);
            builder.AddAttribute(14, nameof(FbRow.WFS), _open);
            builder.AddAttribute(15, nameof(FbRow.WFM), !_open);
            builder.AddAttribute(16, nameof(FbRow.WBS), true);
            builder.AddAttribute(17, nameof(FbRow.WRM), true);
            builder.AddAttribute(18, nameof(FbRow.PVS), true);
            builder.AddAttribute(19, nameof(FbRow.PHM), true);
            builder.AddAttribute(20, nameof(FbRow.GS), true);
            builder.AddAttribute(21, nameof(FbRow.JVC), true);
            builder.AddAttribute(22, nameof(FbRow.Classes), "fb-field");
            builder.AddAttribute(23, nameof(FbRow.ChildContent), (RenderFragment)(row =>
            {
                row.AddContent(0, StartContent);
                row.OpenElement(1, "div");
                row.AddAttribute(2, "class", "fb-select-value");
                if (selected >= 0)
                {
                    BuildItem(row, 3, Items[selected]);
                }
                else if (Placeholder is not null)
                {
                    row.OpenComponent<FbText>(6);
                    row.AddAttribute(7, nameof(FbText.Value), Placeholder);
                    row.AddAttribute(8, nameof(FbText.Subtle), true);
                    row.CloseComponent();
                }
                row.CloseElement();
                row.AddContent(9, EndContent);
                // The chevron is drawn by the stylesheet.
                row.OpenElement(10, "span");
                row.AddAttribute(11, "class", "fb-select-chevron");
                row.AddAttribute(12, "aria-hidden", "true");
                row.CloseElement();
            }));
            builder.CloseComponent();

            builder.CloseElement();
        }

        private void BuildZone(RenderTreeBuilder builder)
        {
            var selected = SelectedIndex;

            builder.OpenComponent<FbStack>(0);
            builder.AddAttribute(1, nameof(FbStack.WBS), true);
            builder.AddAttribute(2, nameof(FbStack.WFM), true);
            builder.AddAttribute(3, nameof(FbStack.WRM), true);
            builder.AddAttribute(4, nameof(FbStack.PS), true);
            builder.AddAttribute(5, nameof(FbStack.ChildContent), (RenderFragment)(zone =>
            {
                zone.OpenElement(0, "div");
                zone.AddAttribute(1, "id", $"{_id}-list");
                zone.AddAttribute(2, "role", "listbox");
                zone.AddAttribute(3, "class", "fb-select-list");

                for (var index = 0; index < Items.Count; index++)
                {
                    var option = index;
                    zone.OpenElement(4, "div");
                    zone.SetKey(option);
                    zone.AddAttribute(5, "id", OptionId(option));
                    zone.AddAttribute(6, "role", "option");
                    zone.AddAttribute(7, "aria-selected", option == selected ? "true" : "false");
                    zone.AddAttribute(8, "class", option == _active ? "fb-select-option fb-select-option-active" : "fb-select-option");
                    zone.AddAttribute(9, "onclick", EventCallback.Factory.Create(this, () => SelectAsync(option)));

                    zone.OpenComponent<FbRow>(10);
                    zone.AddAttribute(11, nameof(FbRow.GS), true);
                    zone.AddAttribute(12, nameof(FbRow.PHM), true);
                    zone.AddAttribute(13, nameof(FbRow.PVS), true);
                    zone.AddAttribute(14, nameof(FbRow.JVC), true);
                    zone.AddAttribute(15, nameof(FbRow.ChildContent), (RenderFragment)(row =>
                    {
                        row.OpenElement(0, "div");
                        row.AddAttribute(1, "class", "fb-select-value");
                        BuildItem(row, 2, Items[option]);
                        row.CloseElement();
                        if (option == selected)
                        {
                            // The check mark is drawn by the stylesheet.
                            row.OpenElement(5, "span");
                            row.AddAttribute(6, "class", "fb-select-check");
                            row.AddAttribute(7, "aria-hidden", "true");
                            row.CloseElement();
                        }
                    }));
                    zone.CloseComponent();

                    zone.CloseElement();
                }

                zone.CloseElement();
            }));
            builder.CloseComponent();
        }

        /// <summary>
        /// Renders one element through the view registered for its run-time type.
        /// </summary>
        private void BuildItem(RenderTreeBuilder builder, int sequence, TValue item)
        {
            var selector = ItemSelector ?? FbViewSelector.Item;
            var viewType = _viewTypes.GetOrAdd(selector.GetType(), static type => typeof(FbDynamicView<>).MakeGenericType(type));

            builder.OpenComponent(sequence, viewType);
            builder.AddAttribute(sequence + 1, "Item", item);
            builder.AddAttribute(sequence + 2, "Selector", selector);
            builder.CloseComponent();
        }

        private void SetOpen(bool open)
        {
            _open = open;
            _active = open ? Math.Max(SelectedIndex, 0) : -1;
            StateHasChanged();
        }

        private async Task SelectAsync(int index)
        {
            if (IsDisabled || index < 0 || index >= Items.Count)
                return;

            _shown = Items[index];
            _open = false;
            _active = -1;
            StateHasChanged();

            await CommitAsync(Items[index]);
            await FocusAnchorAsync();
        }

        private Task HandleKeyDownAsync(KeyboardEventArgs args)
        {
            if (IsDisabled || Items.Count == 0)
                return Task.CompletedTask;

            switch (args.Key)
            {
                case "ArrowDown":
                    Move(_open ? _active + 1 : Math.Max(SelectedIndex, 0));
                    break;
                case "ArrowUp":
                    Move(_open ? _active - 1 : SelectedIndex >= 0 ? SelectedIndex : Items.Count - 1);
                    break;
                case "Home" when _open:
                    Move(0);
                    break;
                case "End" when _open:
                    Move(Items.Count - 1);
                    break;
                case "Enter" or " ":
                    if (_open && _active >= 0)
                        return SelectAsync(_active);
                    SetOpen(!_open);
                    break;
                case "Tab" when _open:
                    SetOpen(false);
                    break;
            }

            return Task.CompletedTask;
        }

        private void Move(int index)
        {
            _open = true;
            _active = Math.Clamp(index, 0, Items.Count - 1);
            StateHasChanged();
        }

        private async Task FocusAnchorAsync()
        {
            try
            {
                await _anchorElement.FocusAsync();
            }
            catch (Exception exception) when (exception is InvalidOperationException or Microsoft.JSInterop.JSException or Microsoft.JSInterop.JSDisconnectedException)
            {
                // The anchor is not rendered interactively (prerender, disconnected circuit).
            }
        }

        private int IndexOf(TValue value)
        {
            var comparer = EqualityComparer<TValue>.Default;
            for (var index = 0; index < Items.Count; index++)
            {
                if (comparer.Equals(Items[index], value))
                    return index;
            }
            return -1;
        }
    }
}
