using FractalBlazor.Components.Forms.Contracts;
using FractalBlazor.Components.Layout;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace FractalBlazor.Components.Forms.Inputs
{
    /// <summary>
    /// Framed single-line field on a native <c>input</c>. While it has the focus, it keeps
    /// what the user types even if the state re-renders it; <c>Escape</c> restores the value
    /// it had when it got the focus. Text that cannot be parsed is kept but never sent.
    /// </summary>
    public abstract class FbFieldBase<TValue, TAction> : FbInputBase<TValue, TAction>
        where TAction : IStateAction<TValue>
    {
        private readonly RenderFragment _rowContent;
        private TValue _pristineValue = default!;
        private string _text = string.Empty;
        private bool _hasFocus;

        protected FbFieldBase()
        {
            _rowContent = BuildRowContent;
        }

        /// <summary>
        /// Send the value on every keystroke instead of when the field loses the focus.
        /// </summary>
        [Parameter]
        public bool Immediate { get; set; }

        [Parameter]
        public string? Placeholder { get; set; }

        [Parameter]
        public EventCallback OnFocus { get; set; }

        [Parameter]
        public EventCallback OnBlur { get; set; }

        /// <summary>
        /// Content placed before the input, inside the frame: a label, an icon.
        /// </summary>
        [Parameter]
        public RenderFragment? StartContent { get; set; }

        /// <summary>
        /// Content placed after the input, inside the frame: a unit, a button.
        /// </summary>
        [Parameter]
        public RenderFragment? EndContent { get; set; }

        /// <summary>
        /// <c>type</c> attribute of the native input.
        /// </summary>
        protected abstract string InputType { get; }

        /// <summary>
        /// Text shown in the input for <paramref name="value"/>.
        /// </summary>
        protected abstract string Format(TValue value);

        /// <summary>
        /// Converts the text typed by the user. Returning <c>false</c> keeps the text in the
        /// input without sending anything.
        /// </summary>
        protected abstract bool TryParse(string text, out TValue value);

        /// <summary>
        /// Adds type-specific attributes (<c>min</c>, <c>max</c>, <c>step</c>…) to the input,
        /// with sequence numbers from 100: the base uses lower ones.
        /// </summary>
        protected virtual void AddInputAttributes(RenderTreeBuilder builder)
        {
        }

        protected override void OnParametersSet()
        {
            if (!_hasFocus)
                _text = Format(Value);
        }

        protected override void BuildControl(RenderTreeBuilder builder)
        {
            builder.OpenComponent<FbRow>(0);
            builder.AddAttribute(1, nameof(FbRow.EOutline), _hasFocus);
            builder.AddAttribute(2, nameof(FbRow.WFS), _hasFocus);
            builder.AddAttribute(3, nameof(FbRow.WFM), !_hasFocus);
            builder.AddAttribute(4, nameof(FbRow.GS), true);
            builder.AddAttribute(5, nameof(FbRow.PVS), true);
            builder.AddAttribute(6, nameof(FbRow.PHM), true);
            builder.AddAttribute(7, nameof(FbRow.WBA), true);
            builder.AddAttribute(8, nameof(FbRow.WRM), true);
            builder.AddAttribute(9, nameof(FbRow.DS), true);
            builder.AddAttribute(10, nameof(FbRow.JVBl), true);
            builder.AddAttribute(11, nameof(FbRow.Classes), $"fb-control fb-field {Classes}".TrimEnd());
            builder.AddAttribute(12, nameof(FbRow.ChildContent), _rowContent);
            builder.CloseComponent();
        }

        private void BuildRowContent(RenderTreeBuilder builder)
        {
            builder.AddContent(0, StartContent);
            builder.OpenElement(1, "input");
            builder.AddAttribute(2, "type", InputType);
            builder.AddAttribute(3, "class", "fb-control-input");
            builder.AddAttribute(4, "value", _text);
            builder.AddAttribute(5, "placeholder", Placeholder);
            builder.AddAttribute(6, "disabled", Disabled);
            AddInputAttributes(builder);
            builder.AddAttribute(8, Immediate ? "oninput" : "onchange", EventCallback.Factory.Create<ChangeEventArgs>(this, HandleChangeAsync));
            builder.AddAttribute(9, "onkeyup", EventCallback.Factory.Create<KeyboardEventArgs>(this, HandleKeyUpAsync));
            builder.AddAttribute(10, "onfocus", EventCallback.Factory.Create<FocusEventArgs>(this, HandleFocusAsync));
            builder.AddAttribute(11, "onblur", EventCallback.Factory.Create<FocusEventArgs>(this, HandleBlurAsync));
            builder.CloseElement();
            builder.AddContent(12, EndContent);
        }

        private Task HandleChangeAsync(ChangeEventArgs e)
        {
            _text = e.Value?.ToString() ?? string.Empty;
            return TryParse(_text, out var value) ? CommitAsync(value) : Task.CompletedTask;
        }

        private Task HandleKeyUpAsync(KeyboardEventArgs e)
        {
            if (e.Key != "Escape")
                return Task.CompletedTask;

            _text = Format(_pristineValue);
            StateHasChanged();
            return CommitAsync(_pristineValue);
        }

        private async Task HandleFocusAsync(FocusEventArgs e)
        {
            _hasFocus = true;
            _pristineValue = Value;
            StateHasChanged();
            if (OnFocus.HasDelegate)
                await OnFocus.InvokeAsync();
        }

        private async Task HandleBlurAsync(FocusEventArgs e)
        {
            _hasFocus = false;
            _text = Format(Value);
            StateHasChanged();
            if (OnBlur.HasDelegate)
                await OnBlur.InvokeAsync();
        }
    }
}
