using FractalBlazor.Components.Forms.Contracts;
using FractalBlazor.Components.Forms.Core;
using FractalBlazor.Components.Layout.Abstracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Text;

namespace FractalBlazor.Components.Forms.Inputs
{
    public class FbInputText<TAction> : FbComponentBase
        where TAction : IStateAction<string>
    {
        private string _pristineValue = string.Empty;
        private string _currentInputValue = string.Empty;
        private bool _hasFocus = false;

        [Parameter]
        public string Value { get; set; } = string.Empty;

        [Parameter]
        public bool Immediate { get; set; }

        [Parameter]
        public object? Handler { get; set; }

        [Parameter]
        public EventCallback OnFocus { get; set; }

        [Parameter]
        public EventCallback OnBlur { get; set; }

        // --- Lifecycle ---

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (!_hasFocus)
                _currentInputValue = Value;
        }

        // --- Rendering ---

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "input");
            builder.AddAttribute(1, "type", "text");

            builder.AddAttribute(2, "value", _currentInputValue);

            string eventName = Immediate ? "oninput" : "onchange";
            builder.AddAttribute(3, eventName, EventCallback.Factory.Create<ChangeEventArgs>(this, HandleChange));

            builder.AddAttribute(4, "onkeyup", EventCallback.Factory.Create<KeyboardEventArgs>(this, HandleKeyUp));
            builder.AddAttribute(5, "onfocus", EventCallback.Factory.Create<FocusEventArgs>(this, HandleFocusAsync));
            builder.AddAttribute(6, "onblur", EventCallback.Factory.Create<FocusEventArgs>(this, HandleBlurAsync));

            builder.CloseElement();
        }

        // --- Event Handlers ---

        private void HandleChange(ChangeEventArgs e)
        {
            _currentInputValue = e.Value?.ToString() ?? string.Empty;
            DispatchAction(_currentInputValue);
        }

        private void HandleKeyUp(KeyboardEventArgs e)
        {
            if (e.Key == "Escape")
            {
                _currentInputValue = _pristineValue;
                DispatchAction(_pristineValue);
            }
        }

        private async Task HandleFocusAsync(FocusEventArgs e)
        {
            _hasFocus = true;
            _pristineValue = Value;

            if (OnFocus.HasDelegate)
                await OnFocus.InvokeAsync();
        }

        private async Task HandleBlurAsync(FocusEventArgs e)
        {
            _hasFocus = false;
            _currentInputValue = Value;
            StateHasChanged();
            if (OnBlur.HasDelegate)
                await OnBlur.InvokeAsync();
        }

        // --- Dispatcher ---

        private void DispatchAction(string newValue)
        {
            if (Handler != null)
            {
                TAction action = ActionFactory<TAction, string>.Create(newValue);
                ActionDispatcher<TAction>.Dispatch(Handler, action);
            }
        }
    }
}
