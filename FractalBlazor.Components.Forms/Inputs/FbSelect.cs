using System.Globalization;
using FractalBlazor.Components.Common.Contracts;
using FractalBlazor.Components.Layout;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Forms.Inputs
{
    /// <summary>
    /// Typed choice among <see cref="Items"/>, on the native <c>select</c>: keyboard, screen
    /// readers and mobile pickers work out of the box, in the color scheme of the theme.
    /// Options carry their index, so any item type works, even without a string form.
    /// </summary>
    public class FbSelect<TValue, TAction> : FbInputBase<TValue, TAction>
        where TAction : IStateAction<TValue>
    {
        private readonly RenderFragment _rowContent;

        public FbSelect()
        {
            _rowContent = BuildRowContent;
        }

        [Parameter, EditorRequired]
        public IReadOnlyList<TValue> Items { get; set; } = [];

        /// <summary>
        /// Text of an item. Defaults to <c>ToString()</c>.
        /// </summary>
        [Parameter]
        public Func<TValue, string>? ItemText { get; set; }

        /// <summary>
        /// Text of an empty first option, shown while the value is not one of the items.
        /// </summary>
        [Parameter]
        public string? Placeholder { get; set; }

        [Parameter]
        public RenderFragment? StartContent { get; set; }

        [Parameter]
        public RenderFragment? EndContent { get; set; }

        protected override void BuildControl(RenderTreeBuilder builder)
        {
            builder.OpenComponent<FbRow>(0);
            builder.AddAttribute(1, nameof(FbRow.WFM), true);
            builder.AddAttribute(2, nameof(FbRow.GS), true);
            builder.AddAttribute(3, nameof(FbRow.PVS), true);
            builder.AddAttribute(4, nameof(FbRow.PHM), true);
            builder.AddAttribute(5, nameof(FbRow.WBA), true);
            builder.AddAttribute(6, nameof(FbRow.WRM), true);
            builder.AddAttribute(7, nameof(FbRow.JVC), true);
            builder.AddAttribute(8, nameof(FbRow.Classes), $"fb-control fb-field fb-select-field {Classes}".TrimEnd());
            builder.AddAttribute(9, nameof(FbRow.ChildContent), _rowContent);
            builder.CloseComponent();
        }

        private void BuildRowContent(RenderTreeBuilder builder)
        {
            var selected = IndexOf(Value);

            builder.AddContent(0, StartContent);
            builder.OpenElement(1, "select");
            builder.AddAttribute(2, "class", "fb-control-input fb-select");
            builder.AddAttribute(3, "disabled", Disabled);
            builder.AddAttribute(4, "onchange", EventCallback.Factory.Create<ChangeEventArgs>(this, HandleChangeAsync));

            if (Placeholder is not null || selected < 0)
            {
                builder.OpenElement(5, "option");
                builder.AddAttribute(6, "value", "");
                builder.AddAttribute(7, "selected", selected < 0);
                builder.AddAttribute(8, "disabled", true);
                builder.AddContent(9, Placeholder);
                builder.CloseElement();
            }

            for (var index = 0; index < Items.Count; index++)
            {
                builder.OpenElement(10, "option");
                builder.SetKey(index);
                builder.AddAttribute(11, "value", index.ToString(CultureInfo.InvariantCulture));
                builder.AddAttribute(12, "selected", index == selected);
                builder.AddContent(13, TextOf(Items[index]));
                builder.CloseElement();
            }

            builder.CloseElement();
            builder.AddContent(14, EndContent);
        }

        private string TextOf(TValue item) => ItemText is not null ? ItemText(item) : item?.ToString() ?? string.Empty;

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

        private Task HandleChangeAsync(ChangeEventArgs e)
            => int.TryParse(e.Value?.ToString(), NumberStyles.None, CultureInfo.InvariantCulture, out var index) && index < Items.Count
                ? CommitAsync(Items[index])
                : Task.CompletedTask;
    }
}
