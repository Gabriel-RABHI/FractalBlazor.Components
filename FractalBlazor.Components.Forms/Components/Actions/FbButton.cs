using FractalBlazor.Components.Common.Contracts;
using FractalBlazor.Components.Forms.Components.Icons;
using FractalBlazor.Components.Forms.Core;
using FractalBlazor.Components.Forms.Theming.Helpers;
using FractalBlazor.Components.Layout.Abstracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace FractalBlazor.Components.Forms.Components.Actions
{
    /// <summary>
    /// A native <c>button</c> that speaks the Layout grammar: fill, frame, radius, padding and
    /// hover come from the same flags as <c>FbRow</c>. Its defaults (<c>WBA WFM WRM PVS PHM
    /// Hover</c>) can be replaced flag by flag, or removed with <c>WBN</c>, <c>WFN</c>, <c>WRN</c>.
    /// </summary>
    /// <remarks>
    /// A click first sends <see cref="Action"/> to <see cref="Handler"/> (state-first), then
    /// raises <see cref="OnClick"/>. The functional state of the button (<c>Error</c> for a
    /// destructive action, <c>Selected</c> for the primary one…) comes from <see cref="Variant"/>.
    /// </remarks>
    public class FbButton : FbLayoutSurfaceComponentBase
    {
        private readonly RenderFragment _content;

        public FbButton()
        {
            IsInlineFlex = true;
            WBA = true;
            WFM = true;
            WRM = true;
            PVM = true;
            PHL = true;
            Hover = true;
            _content = BuildContent;
        }

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Icon shown before the content, usually a <see cref="FbMaterialIcons"/> constant.
        /// </summary>
        [Parameter]
        public string? Icon { get; set; }

        /// <summary>
        /// Icon shown after the content.
        /// </summary>
        [Parameter]
        public string? EndIcon { get; set; }

        /// <summary>
        /// State object that receives <see cref="Action"/>.
        /// </summary>
        [Parameter]
        public object? Handler { get; set; }

        /// <summary>
        /// Action sent to <see cref="Handler"/> on click, handled by its <c>Handle</c> method
        /// for the action type.
        /// </summary>
        [Parameter]
        public IStateAction? Action { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        /// <summary>
        /// Disabled: the button sends nothing and renders with the <c>Disabled</c> variant,
        /// unless <see cref="Variant"/> says otherwise.
        /// </summary>
        [Parameter]
        public bool D { get => IsDisabled; set => IsDisabled = value; }

        private bool IsDisabled { get; set; }

        /// <summary>
        /// Functional state of the button: <c>Selected</c>, <c>Error</c>, <c>Success</c>…
        /// </summary>
        [Parameter]
        public string? Variant { get; set; }

        /// <summary>
        /// Submit the enclosing form instead of acting as a plain button.
        /// </summary>
        [Parameter]
        public bool Submit { get; set; }

        /// <summary>
        /// Accessible name, required when the button only shows an icon.
        /// </summary>
        [Parameter]
        public string? Title { get; set; }

        private string? _surfaceClasses, _variantClasses;
        private string _buttonClasses = "";

        /// <summary>
        /// The surface and variant classes come from shared caches: the button classes are only
        /// joined again when one of them changes.
        /// </summary>
        private string ButtonClasses
        {
            get
            {
                var surfaceClasses = AggregatedClasses;
                var variantClasses = FbVariantScope.Classes(FbVariantScope.Effective(Variant, IsDisabled));
                if (!ReferenceEquals(surfaceClasses, _surfaceClasses) || !ReferenceEquals(variantClasses, _variantClasses) || _buttonClasses.Length == 0)
                {
                    (_surfaceClasses, _variantClasses) = (surfaceClasses, variantClasses);
                    _buttonClasses = FbVariantScope.WithClasses("fb-control fb-button " + surfaceClasses, variantClasses);
                }
                return _buttonClasses;
            }
        }

        /// <summary>
        /// The button carries its variant itself, by its classes.
        /// </summary>
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "button");
            builder.AddAttribute(1, "type", Submit ? "submit" : "button");
            builder.AddAttribute(2, "class", ButtonClasses);
            builder.AddAttribute(3, "style", AggregatedStyles);
            builder.AddAttribute(4, "disabled", IsDisabled);
            builder.AddAttribute(5, "title", Title);
            builder.AddAttribute(6, "aria-label", Title);
            if (!IsDisabled && (OnClick.HasDelegate || (Handler is not null && Action is not null)))
                builder.AddAttribute(7, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, HandleClickAsync));
            builder.AddContent(8, _content);
            builder.CloseElement();
        }

        private void BuildContent(RenderTreeBuilder builder)
        {
            if (!string.IsNullOrEmpty(Icon))
            {
                builder.OpenComponent<FbIcon>(0);
                builder.AddAttribute(1, nameof(FbIcon.Icon), Icon);
                builder.CloseComponent();
            }

            builder.AddContent(2, ChildContent);

            if (!string.IsNullOrEmpty(EndIcon))
            {
                builder.OpenComponent<FbIcon>(3);
                builder.AddAttribute(4, nameof(FbIcon.Icon), EndIcon);
                builder.CloseComponent();
            }
        }

        private Task HandleClickAsync(MouseEventArgs args)
        {
            if (IsDisabled)
                return Task.CompletedTask;

            if (Handler is not null && Action is not null)
                FbActionDispatcher.Dispatch(Handler, Action);

            return OnClick.HasDelegate ? OnClick.InvokeAsync(args) : Task.CompletedTask;
        }
    }
}
