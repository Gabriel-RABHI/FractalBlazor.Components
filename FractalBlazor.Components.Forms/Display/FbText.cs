using FractalBlazor.Components.Layout.Abstracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace FractalBlazor.Components.Forms.Display
{
    public enum FbTextSize : byte
    {
        /// <summary>
        /// Default is M
        /// </summary>
        None,
        /// <summary>
        /// Extra small
        /// </summary>
        XS,
        /// <summary>
        /// Small
        /// </summary>
        S,
        /// <summary>
        /// Medium, as default, or paragrph
        /// </summary>
        M,
        /// <summary>
        /// Large (H3)
        /// </summary>
        L,
        /// <summary>
        /// Extra large (H2)
        /// </summary>
        XL,
        /// <summary>
        /// Extra extra large (H1)
        /// </summary>
        XXL
    }

    public enum FbTextWeight : byte
    {
        None,
        T,
        B,
        XB
    }

    public enum FbTextIntent : byte
    {
        Default = 0,
        Subtle = 1,
        Muted = 2,
        Highlight = 3
    }

    public enum FbTextModifiers : byte
    {
        NoWrap,
        Trim
    }

    public class FbText : FbComponentBase
    {
        private static string[] FbTextSizeClasses = { "", "fb-t-xs ", "fb-t-s ", "fb-t-m ", "fb-t-l ", "fb-t-xl ", "fb-t-xxl " };
        private static string[] FbTextWeightClasses = { "", "fb-t-t ", "fb-t-b ", "fb-t-xb " };
        private static string[] FbTextIntentClasses = { "", "fb-fg-subtle ", "fb-fg-muted ", "fb-fg-highlight " };
        private static string[] FbTextModifiersClasses = { "", "fb-t-nw ", "fb-t-tr " };

        private RenderHandle _renderHandle;

        private FbTextSize _size;
        private FbTextWeight _weight;
        private FbTextIntent _intent;
        private FbTextModifiers _modifiers;

        // -------- Scale
        [Parameter]
        public bool XS { get => _size == FbTextSize.XS; set { if (value) _size = FbTextSize.XS; } }

        [Parameter]
        public bool S { get => _size == FbTextSize.S; set { if (value) _size = FbTextSize.S; } }

        [Parameter]
        public bool M { get => _size == FbTextSize.M; set { if (value) _size = FbTextSize.M; } }

        [Parameter]
        public bool L { get => _size == FbTextSize.L; set { if (value) _size = FbTextSize.L; } }

        [Parameter]
        public bool XL { get => _size == FbTextSize.XL; set { if (value) _size = FbTextSize.XL; } }

        [Parameter]
        public bool XXL { get => _size == FbTextSize.XXL; set { if (value) _size = FbTextSize.XXL; } }

        // -------- Weight
        [Parameter]
        public bool T { get => _weight == FbTextWeight.T; set { if (value) _weight = FbTextWeight.T; } }

        [Parameter]
        public bool B { get => _weight == FbTextWeight.B; set { if (value) _weight = FbTextWeight.B; } }

        [Parameter]
        public bool XB { get => _weight == FbTextWeight.XB; set { if (value) _weight = FbTextWeight.XB; } }

        // -------- Intent
        [Parameter]
        public bool Subtle { get => _intent == FbTextIntent.Subtle; set { if (value) _intent = FbTextIntent.Subtle; } }

        [Parameter]
        public bool Muted { get => _intent == FbTextIntent.Muted; set { if (value) _intent = FbTextIntent.Muted; } }

        [Parameter]
        public bool Highlight { get => _intent == FbTextIntent.Highlight; set { if (value) _intent = FbTextIntent.Highlight; } }

        // -------- Modifiers
        [Parameter]
        public bool NW { get => _modifiers == FbTextModifiers.NoWrap; set { if (value) _modifiers = FbTextModifiers.NoWrap; } }

        [Parameter]
        public bool TR { get => _modifiers == FbTextModifiers.Trim; set { if (value) _modifiers = FbTextModifiers.Trim; } }

        // -------- Content
        [Parameter]
        public string Value { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        public void Attach(RenderHandle renderHandle) => _renderHandle = renderHandle;

        protected override void OnBeforeParametersSet()
        {
            _size = default;
            _weight = default;
            _intent = default;
            _modifiers = default;
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "span");

            builder.AddAttribute(1, "class", BuildCssClass());

            if (Value != null)
                builder.AddContent(2, Value);
            else if (ChildContent != null)
                builder.AddContent(3, ChildContent);

            builder.CloseElement();
        }

        private string BuildCssClass()
        {
            string size = FbTextSizeClasses[(byte)_size];
            string weight = FbTextWeightClasses[(byte)_weight];
            string intent = FbTextIntentClasses[(byte)_intent];
            string mod = FbTextModifiersClasses[(byte)_modifiers];

            return $"fb-txt {size}{weight}{intent}{mod}".TrimEnd();
        }
    }
}
