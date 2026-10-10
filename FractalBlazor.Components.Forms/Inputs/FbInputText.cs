using FractalBlazor.Components.Forms.Contracts;
using FractalBlazor.Components.Layout;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace FractalBlazor.Components.Forms.Inputs
{
    /// <summary>
    /// Single-line text field. While it has the focus, it keeps what the user types even if
    /// the state re-renders it; <c>Escape</c> restores the value it had when it got the focus.
    /// </summary>
    public class FbInputText<TAction> : FbInputBase<string, TAction>
        where TAction : IStateAction<string>
    {
        private readonly RenderFragment _rowContent;
        private string _pristineValue = string.Empty;
        private string _currentInputValue = string.Empty;
        private bool _hasFocus;

        public FbInputText()
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

        protected override void OnParametersSet()
        {
            if (!_hasFocus)
                _currentInputValue = Value ?? string.Empty;
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
            builder.AddAttribute(11, nameof(FbRow.Classes), $"fb-control fb-input-text {Classes}".TrimEnd());
            builder.AddAttribute(12, nameof(FbRow.ChildContent), _rowContent);
            builder.CloseComponent();
        }

        private void BuildRowContent(RenderTreeBuilder builder)
        {
            builder.AddContent(0, StartContent);
            builder.OpenElement(1, "input");
            builder.AddAttribute(2, "type", "text");
            builder.AddAttribute(3, "class", "fb-control-input fb-input-text__input");
            builder.AddAttribute(4, "value", _currentInputValue);
            builder.AddAttribute(5, "placeholder", Placeholder);
            builder.AddAttribute(6, "disabled", Disabled);
            builder.AddAttribute(7, Immediate ? "oninput" : "onchange", EventCallback.Factory.Create<ChangeEventArgs>(this, HandleChangeAsync));
            builder.AddAttribute(8, "onkeyup", EventCallback.Factory.Create<KeyboardEventArgs>(this, HandleKeyUpAsync));
            builder.AddAttribute(9, "onfocus", EventCallback.Factory.Create<FocusEventArgs>(this, HandleFocusAsync));
            builder.AddAttribute(10, "onblur", EventCallback.Factory.Create<FocusEventArgs>(this, HandleBlurAsync));
            builder.CloseElement();
            builder.AddContent(11, EndContent);
        }

        private Task HandleChangeAsync(ChangeEventArgs e)
        {
            _currentInputValue = e.Value?.ToString() ?? string.Empty;
            return CommitAsync(_currentInputValue);
        }

        private Task HandleKeyUpAsync(KeyboardEventArgs e)
        {
            if (e.Key != "Escape")
                return Task.CompletedTask;

            _currentInputValue = _pristineValue;
            StateHasChanged();
            return CommitAsync(_pristineValue);
        }

        private async Task HandleFocusAsync(FocusEventArgs e)
        {
            _hasFocus = true;
            _pristineValue = Value ?? string.Empty;
            StateHasChanged();
            if (OnFocus.HasDelegate)
                await OnFocus.InvokeAsync();
        }

        private async Task HandleBlurAsync(FocusEventArgs e)
        {
            _hasFocus = false;
            _currentInputValue = Value ?? string.Empty;
            StateHasChanged();
            if (OnBlur.HasDelegate)
                await OnBlur.InvokeAsync();
        }
    }
}
