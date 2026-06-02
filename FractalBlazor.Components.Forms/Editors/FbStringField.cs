using FractalBlazor.Components.Forms.Contracts;
using FractalBlazor.Components.Layout;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Text;

namespace FractalBlazor.Components.Forms.Editors
{
    public class FbStringInput<TAction> : FbSimpleComponentBase
        where TAction : IStateAction<TAction, string>
    {
        private string _pristineValue = string.Empty;

        [Parameter]
        public string Value { get; set; } = string.Empty;

        [Parameter]
        public IActionHandler<TAction>? Handler { get; set; }

        [Parameter]
        public EventCallback OnFocus { get; set; }

        [Parameter]
        public EventCallback OnBlur { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "input");
            builder.AddAttribute(1, "type", "text");
            builder.AddAttribute(2, "value", Value);

            builder.AddAttribute(3, "onchange", EventCallback.Factory.Create<ChangeEventArgs>(this, HandleChange));
            builder.AddAttribute(4, "onkeyup", EventCallback.Factory.Create<KeyboardEventArgs>(this, HandleKeyUp));
            builder.AddAttribute(5, "onfocus", EventCallback.Factory.Create<FocusEventArgs>(this, HandleFocusAsync));
            builder.AddAttribute(6, "onblur", EventCallback.Factory.Create<FocusEventArgs>(this, HandleBlurAsync));

            builder.CloseElement();
        }

        private void HandleChange(ChangeEventArgs e)
        {
            var newValue = e.Value?.ToString() ?? string.Empty;
            DispatchAction(newValue);
        }

        private void HandleKeyUp(KeyboardEventArgs e)
        {
            if (e.Key == "Escape")
                DispatchAction(_pristineValue);
        }

        private async Task HandleFocusAsync(FocusEventArgs e)
        {
            _pristineValue = Value;
            if (OnFocus.HasDelegate)
                await OnFocus.InvokeAsync();
        }

        private async Task HandleBlurAsync(FocusEventArgs e)
        {
            if (OnBlur.HasDelegate)
            {
                await OnBlur.InvokeAsync();
            }
        }

        private void DispatchAction(string newValue)
        {
            TAction action = TAction.Create(newValue);
            Handler?.Handle(action);
        }
    }
}
