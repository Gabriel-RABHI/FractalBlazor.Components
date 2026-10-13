using FractalBlazor.Components.Common.Contracts;
using FractalBlazor.Components.Forms.Components.Inputs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Forms.Components.Switchers
{
    /// <summary>
    /// Boolean field on a native checkbox wrapped in its label, so a click on the text
    /// toggles it and assistive technologies announce both together.
    /// </summary>
    public abstract class FbToggleBase<TAction> : FbInputBase<bool, TAction>
        where TAction : IStateAction<bool>
    {
        /// <summary>
        /// Label text or content, placed after the control.
        /// </summary>
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// CSS class of the native input, which carries the visual of the toggle.
        /// </summary>
        protected abstract string InputClass { get; }

        /// <summary>
        /// ARIA role of the native input, or <c>null</c> for a plain checkbox.
        /// </summary>
        protected virtual string? InputRole => null;

        protected override void BuildControl(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "label");
            builder.AddAttribute(1, "class", RootClasses($"fb-control fb-toggle {Classes}".TrimEnd()));
            builder.AddAttribute(2, "style", VariantStyle);
            builder.OpenElement(3, "input");
            builder.AddAttribute(4, "type", "checkbox");
            builder.AddAttribute(5, "class", InputClass);
            builder.AddAttribute(6, "role", InputRole);
            builder.AddAttribute(7, "checked", Value);
            builder.AddAttribute(8, "disabled", Disabled);
            builder.AddAttribute(9, "onchange", EventCallback.Factory.Create<ChangeEventArgs>(this, HandleChangeAsync));
            builder.CloseElement();
            builder.AddContent(10, ChildContent);
            builder.CloseElement();
        }

        protected Task HandleChangeAsync(ChangeEventArgs e)
            => CommitAsync(e.Value is bool value ? value : bool.TryParse(e.Value?.ToString(), out var parsed) && parsed);
    }

    /// <summary>
    /// On/off switch, announced as a switch to assistive technologies.
    /// </summary>
    public class FbSwitch<TAction> : FbToggleBase<TAction>
        where TAction : IStateAction<bool>
    {
        protected override string InputClass => "fb-switch";

        protected override string? InputRole => "switch";
    }
}
