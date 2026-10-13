using FractalBlazor.Components.Common.Contracts;
using FractalBlazor.Components.Forms.Core;
using FractalBlazor.Components.Forms.Theming.Constants;
using FractalBlazor.Components.Forms.Theming.Helpers;
using FractalBlazor.Components.Forms.Theming.Registry;
using FractalBlazor.Components.Layout.Abstracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Forms.Components.Inputs
{
    /// <summary>
    /// Base of every field. A new value is first sent as a <typeparamref name="TAction"/> to
    /// <see cref="Handler"/> (state-first), then reported through <see cref="ValueChanged"/>,
    /// which makes <c>@bind-Value</c> possible without cost when it is not used.
    /// </summary>
    /// <remarks>
    /// When <see cref="Variant"/> or <see cref="Disabled"/> asks for it, the root element of the
    /// control carries the variant itself (<see cref="RootClasses"/>, <see cref="VariantStyle"/>);
    /// otherwise the field inherits the variant of its parent. Switching to <c>Error</c> while
    /// typing only changes attributes of that element, so the DOM, and the focus, stay in place.
    /// </remarks>
    public abstract class FbInputBase<TValue, TAction> : FbComponentBase
        where TAction : IStateAction<TValue>
    {
        [CascadingParameter]
        public FbThemeContext? ThemeContext { get; set; }

        /// <summary>
        /// Current value, usually read from the state.
        /// </summary>
        [Parameter]
        public TValue Value { get; set; } = default!;

        /// <summary>
        /// Raised after the action is sent. Enables <c>@bind-Value</c>.
        /// </summary>
        [Parameter]
        public EventCallback<TValue> ValueChanged { get; set; }

        /// <summary>
        /// State object that receives the action through its <c>Handle(TAction)</c> method.
        /// </summary>
        [Parameter]
        public object? Handler { get; set; }

        /// <summary>
        /// Disables the field and renders it with the <c>Disabled</c> variant, unless
        /// <see cref="Variant"/> says otherwise.
        /// </summary>
        [Parameter]
        public bool Disabled { get; set; }

        /// <summary>
        /// Functional state of the field: <c>Error</c>, <c>Warning</c>, <c>Success</c>…
        /// </summary>
        [Parameter]
        public string? Variant { get; set; }

        /// <summary>
        /// Custom CSS classes added to the control.
        /// </summary>
        [Parameter]
        public string Classes { get; set; } = "";

        /// <summary>
        /// Variant actually applied, or <c>null</c> to inherit the parent one.
        /// </summary>
        protected string? EffectiveVariant => FbVariantScope.Effective(Variant, Disabled);

        /// <summary>
        /// Sends <paramref name="value"/> to the state, then to <see cref="ValueChanged"/>.
        /// A disabled field sends nothing.
        /// </summary>
        protected async Task CommitAsync(TValue value)
        {
            if (Disabled)
                return;

            if (Handler is not null)
                FbActionDispatcher<TAction>.Dispatch(Handler, FbActionFactory<TAction, TValue>.Create(value));

            if (ValueChanged.HasDelegate)
                await ValueChanged.InvokeAsync(value);
        }

        /// <summary>
        /// Redirections of the effective variant, to put in the <c>style</c> of the root element,
        /// or <c>null</c> without a variant. Computed before each <see cref="BuildControl"/>.
        /// </summary>
        protected string? VariantStyle { get; private set; }

        /// <summary>
        /// Classes of the root element: <paramref name="classes"/>, plus the variant class when
        /// the control carries a variant.
        /// </summary>
        protected string RootClasses(string classes) => FbVariantScope.WithClass(classes, VariantStyle);

        /// <summary>
        /// Renders the control, whose root element takes <see cref="RootClasses"/> and
        /// <see cref="VariantStyle"/>.
        /// </summary>
        protected abstract void BuildControl(RenderTreeBuilder builder);

        protected sealed override void BuildRenderTree(RenderTreeBuilder builder)
        {
            VariantStyle = FbVariantScope.Style(EffectiveVariant, ThemeContext);
            BuildControl(builder);
        }
    }
}
