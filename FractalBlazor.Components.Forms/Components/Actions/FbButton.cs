using FractalBlazor.Components.Common.Contracts;
using FractalBlazor.Components.Forms.Core;
using FractalBlazor.Components.Forms.Theming.Helpers;
using FractalBlazor.Components.Layout.Abstracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace FractalBlazor.Components.Forms.Components.Actions
{
    /// <summary>
    /// The base button: it carries the function and does not deal with its content, which is
    /// whatever <see cref="ChildContent"/> holds. Its look comes from a size, <see cref="S"/>,
    /// <see cref="M"/> (default) or <see cref="L"/>, a kind, <see cref="KW"/> (default),
    /// <see cref="KS"/>, <see cref="KP"/> or <see cref="KT"/>, and a <see cref="Variant"/>. The
    /// Layout flags (fill, frame, radius, padding, gaps…) remain to adjust it: a flag written
    /// after a size or a kind corrects it.
    /// </summary>
    /// <remarks>
    /// A click first sends <see cref="Action"/> to <see cref="Handler"/> (state-first), then
    /// raises <see cref="OnClick"/>. The functional state of the button (<c>Error</c> for a
    /// destructive action, <c>Selected</c> for the primary one…) comes from <see cref="Variant"/>.
    /// </remarks>
    public class FbButton : FbLayoutSurfaceComponentBase
    {
        public FbButton()
        {
            IsInlineFlex = true;
            KW = true;
            M = true;
            WRM = true;
            Hover = true;
        }

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        // -------- Size: the padding
        /// <summary>
        /// Small: <c>PHM PVS</c>.
        /// </summary>
        [Parameter]
        public bool S { get => PHM && PVS; set { if (value) { PHM = true; PVS = true; } } }

        /// <summary>
        /// Medium, the default: <c>PHL PVM</c>.
        /// </summary>
        [Parameter]
        public bool M { get => PHL && PVM; set { if (value) { PHL = true; PVM = true; } } }

        /// <summary>
        /// Large: <c>PHX PVL</c>.
        /// </summary>
        [Parameter]
        public bool L { get => PHX && PVL; set { if (value) { PHX = true; PVL = true; } } }

        // -------- Kind: the fill and the frame
        /// <summary>
        /// Kind -> Wireframe, the default: a frame without a fill, <c>WBN WFM</c>.
        /// </summary>
        [Parameter]
        public bool KW { get => WBN && WFM; set { if (value) { WBN = true; WFM = true; } } }

        /// <summary>
        /// Kind -> Surface: <c>WBS WFM</c>.
        /// </summary>
        [Parameter]
        public bool KS { get => WBS && WFM; set { if (value) { WBS = true; WFM = true; } } }

        /// <summary>
        /// Kind -> Plain: the full color of the variant, <c>WBH WFM</c>.
        /// </summary>
        [Parameter]
        public bool KP { get => WBH && WFM; set { if (value) { WBH = true; WFM = true; } } }

        /// <summary>
        /// Kind -> Text: a fill without a frame, <c>WBS WFN</c>.
        /// </summary>
        [Parameter]
        public bool KT { get => WBS && WFN; set { if (value) { WBS = true; WFN = true; } } }

        // -------- Function
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
            builder.AddContent(8, ChildContent);
            builder.CloseElement();
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
