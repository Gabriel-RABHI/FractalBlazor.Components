using FractalBlazor.Components.Forms.Contracts;
using FractalBlazor.Components.Forms.Core;
using FractalBlazor.Components.Forms.Theming;
using FractalBlazor.Components.Forms.Theming.Constants;
using FractalBlazor.Components.Layout;
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

        [Parameter]
        public RenderFragment? StartContent { get; set; }

        [Parameter]
        public RenderFragment? EndContent { get; set; }

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
            builder.OpenComponent<FbRow>(0);
            builder.AddAttribute(1, nameof(FbRow.JVC), true);

            if (_hasFocus)
            {
                builder.AddAttribute(2, nameof(FbRow.EOutline), true);
                builder.AddAttribute(3, nameof(FbRow.WFS), true);
            } else
            {
                builder.AddAttribute(2, nameof(FbRow.DOutline), true);
                builder.AddAttribute(3, nameof(FbRow.WFM), true);
            }

            builder.AddAttribute(4, nameof(FbRow.GS), true);
            builder.AddAttribute(5, nameof(FbRow.PVS), true);
            builder.AddAttribute(6, nameof(FbRow.PHM), true);
            builder.AddAttribute(7, nameof(FbRow.WBA), true);
            builder.AddAttribute(8, nameof(FbRow.WRM), true);
            builder.AddAttribute(9, nameof(FbRow.DS), true);
            builder.AddAttribute(10, nameof(FbRow.JVBl), true);
            builder.AddAttribute(11, nameof(FbRow.Classes), "fb-input-text");

            builder.AddAttribute( 12, nameof(FbRow.ChildContent), (RenderFragment)BuildRowContent);

            builder.CloseComponent();
        }

        private void BuildRowContent(RenderTreeBuilder builder)
        {
            builder.AddContent(0, StartContent);

            builder.OpenElement(1, "input");
            builder.AddAttribute(2, "type", "text");
            builder.AddAttribute( 3, "class", "fb-input-text__input");
            builder.AddAttribute(4, "value", _currentInputValue);

            string eventName = Immediate ? "oninput" : "onchange";

            builder.AddAttribute( 5, eventName, EventCallback.Factory.Create<ChangeEventArgs>(this, HandleChange));
            builder.AddAttribute( 6, "onkeyup", EventCallback.Factory.Create<KeyboardEventArgs>( this, HandleKeyUp));
            builder.AddAttribute( 7, "onfocus", EventCallback.Factory.Create<FocusEventArgs>( this, HandleFocusAsync));
            builder.AddAttribute( 8, "onblur", EventCallback.Factory.Create<FocusEventArgs>( this, HandleBlurAsync));

            builder.CloseElement();

            builder.AddContent(9, EndContent);
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
            StateHasChanged();

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
                TAction action = FbActionFactory<TAction, string>.Create(newValue);
                FbActionDispatcher<TAction>.Dispatch(Handler, action);
            }
        }
    }
}
