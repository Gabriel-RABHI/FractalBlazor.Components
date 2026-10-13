using FractalBlazor.Components.Common.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Forms.Components.Switchers
{
    /// <summary>
    /// Where the box of an <see cref="FbCheckbox{TAction}"/> sits along its content.
    /// </summary>
    public enum FbCheckboxAlign : byte
    {
        Center = 0,
        Top = 1,
        Bottom = 2
    }

    /// <summary>
    /// A checkbox drawn with the theme: a square (or round) box next to its content, which may
    /// be any markup. A native checkbox, hidden but focusable, carries the state: the label
    /// toggles it, the keyboard reaches it and assistive technologies announce it.
    /// </summary>
    /// <remarks>
    /// The box is on the left; <see cref="Mirror"/> puts it on the right. Next to a tall
    /// content it is centered, or aligned with the <see cref="Top"/> or the <see cref="Bottom"/>.
    /// </remarks>
    /// <example><c>&lt;FbCheckbox TAction="FbSetValue&lt;bool&gt;" @bind-Value="terms" Circle Top&gt;…&lt;/FbCheckbox&gt;</c></example>
    public class FbCheckbox<TAction> : FbToggleBase<TAction>
        where TAction : IStateAction<bool>
    {
        // Classes of every combination of alignment, mirror and circle, built once.
        private static readonly string[] CombinedClasses = BuildClasses();

        private FbCheckboxAlign _align;

        /// <summary>
        /// Puts the box after the content, on the right.
        /// </summary>
        [Parameter]
        public bool Mirror { get; set; }

        /// <summary>
        /// Draws a round box instead of a square one.
        /// </summary>
        [Parameter]
        public bool Circle { get; set; }

        /// <summary>
        /// Aligns the box with the top of the content.
        /// </summary>
        [Parameter]
        public bool Top { get => _align == FbCheckboxAlign.Top; set { if (value) _align = FbCheckboxAlign.Top; } }

        /// <summary>
        /// Centers the box on the content (the default).
        /// </summary>
        [Parameter]
        public bool Center { get => _align == FbCheckboxAlign.Center; set { if (value) _align = FbCheckboxAlign.Center; } }

        /// <summary>
        /// Aligns the box with the bottom of the content.
        /// </summary>
        [Parameter]
        public bool Bottom { get => _align == FbCheckboxAlign.Bottom; set { if (value) _align = FbCheckboxAlign.Bottom; } }

        /// <summary>
        /// Alignment of the box, for a value computed at run time.
        /// </summary>
        [Parameter]
        public FbCheckboxAlign Align { get => _align; set => _align = value; }

        protected override string InputClass => "fb-check-input";

        protected override void OnBeforeParametersSet()
        {
            base.OnBeforeParametersSet();
            _align = FbCheckboxAlign.Center;
            Mirror = false;
            Circle = false;
        }

        protected override void BuildControl(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "label");
            builder.AddAttribute(1, "class", CssClass);

            builder.OpenElement(2, "input");
            builder.AddAttribute(3, "type", "checkbox");
            builder.AddAttribute(4, "class", InputClass);
            builder.AddAttribute(5, "checked", Value);
            builder.AddAttribute(6, "disabled", Disabled);
            builder.AddAttribute(7, "onchange", EventCallback.Factory.Create<ChangeEventArgs>(this, HandleChangeAsync));
            builder.CloseElement();

            // The box and its mark are drawn by the stylesheet.
            builder.OpenElement(8, "span");
            builder.AddAttribute(9, "class", "fb-check-box");
            builder.AddAttribute(10, "aria-hidden", "true");
            builder.CloseElement();

            builder.OpenElement(15, "span");
            builder.AddAttribute(16, "class", "fb-check-content");
            builder.AddContent(17, ChildContent);
            builder.CloseElement();

            builder.CloseElement();
        }

        private string CssClass
        {
            get
            {
                var classes = CombinedClasses[(int)_align * 4 + (Mirror ? 2 : 0) + (Circle ? 1 : 0)];
                return string.IsNullOrWhiteSpace(Classes) ? classes : classes + " " + Classes;
            }
        }

        private static string[] BuildClasses()
        {
            string[] aligns = ["fb-check-center", "fb-check-top", "fb-check-bottom"];
            var classes = new string[aligns.Length * 4];
            for (var align = 0; align < aligns.Length; align++)
            {
                for (var flags = 0; flags < 4; flags++)
                {
                    classes[align * 4 + flags] = "fb-control fb-check " + aligns[align]
                        + ((flags & 2) != 0 ? " fb-check-mirror" : "")
                        + ((flags & 1) != 0 ? " fb-check-circle" : "");
                }
            }
            return classes;
        }
    }
}
