using FractalBlazor.Components.Layout.Abstracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace FractalBlazor.Components.Forms.Components.Display
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

    /// <summary>
    /// Rendering color of a text, from the least to the most visible. It is a rendering
    /// semantic: functional states (Error, Selected…) come from the enclosing variant.
    /// </summary>
    public enum FbTextColor : byte
    {
        Default = 0,
        Subtle = 1,
        Muted = 2,
        Highlight = 3
    }

    [Flags]
    public enum FbTextModifiers : byte
    {
        None = 0,
        /// <summary>
        /// Keep the text on one line.
        /// </summary>
        NoWrap = 1,
        /// <summary>
        /// Keep the text on one line and end it with an ellipsis when it overflows.
        /// </summary>
        Trim = 2
    }

    /// <summary>
    /// Text with a size, a weight and a color from the theme. Boolean flags (<c>&lt;FbText S B Muted&gt;</c>)
    /// cover static markup; <see cref="Size"/>, <see cref="Weight"/> and <see cref="Color"/>
    /// cover values computed at run time.
    /// </summary>
    public class FbText : FbComponentBase
    {
        private const int SizeCount = 7, WeightCount = 4, ColorCount = 4, ModifierCount = 4;

        private static readonly string[] SizeClasses = { "", " fb-t-xs", " fb-t-s", " fb-t-m", " fb-t-l", " fb-t-xl", " fb-t-xxl" };
        private static readonly string[] WeightClasses = { "", " fb-t-t", " fb-t-b", " fb-t-xb" };
        private static readonly string[] ColorClasses = { "", " fb-subtle", " fb-muted", " fb-highlight" };
        private static readonly string[] ModifierClasses = { "", " fb-t-nw", " fb-t-tr", " fb-t-nw fb-t-tr" };

        // Every combination is built once and shared by all instances.
        private static readonly string?[] _classes = new string?[SizeCount * WeightCount * ColorCount * ModifierCount];

        private FbTextSize _size;
        private FbTextWeight _weight;
        private FbTextColor _color;
        private FbTextModifiers _modifiers;

        // -------- Enum API
        [Parameter]
        public FbTextSize Size { get => _size; set => _size = value; }

        [Parameter]
        public FbTextWeight Weight { get => _weight; set => _weight = value; }

        [Parameter]
        public FbTextColor Color { get => _color; set => _color = value; }

        [Parameter]
        public FbTextModifiers Modifiers { get => _modifiers; set => _modifiers = value; }

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

        // -------- Color
        [Parameter]
        public bool Subtle { get => _color == FbTextColor.Subtle; set { if (value) _color = FbTextColor.Subtle; } }

        [Parameter]
        public bool Muted { get => _color == FbTextColor.Muted; set { if (value) _color = FbTextColor.Muted; } }

        [Parameter]
        public bool Highlight { get => _color == FbTextColor.Highlight; set { if (value) _color = FbTextColor.Highlight; } }

        // -------- Modifiers (cumulative)
        /// <summary>
        /// No Wrap: keep the text on one line.
        /// </summary>
        [Parameter]
        public bool NW { get => _modifiers.HasFlag(FbTextModifiers.NoWrap); set { if (value) _modifiers |= FbTextModifiers.NoWrap; } }

        /// <summary>
        /// Trim: end an overflowing line with an ellipsis.
        /// </summary>
        [Parameter]
        public bool TR { get => _modifiers.HasFlag(FbTextModifiers.Trim); set { if (value) _modifiers |= FbTextModifiers.Trim; } }

        // -------- Content
        [Parameter]
        public string? Value { get; set; }

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        protected override void OnBeforeParametersSet()
        {
            _size = default;
            _weight = default;
            _color = default;
            _modifiers = default;
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "span");

            builder.AddAttribute(1, "class", CssClass);

            if (Value != null)
                builder.AddContent(2, Value);
            else if (ChildContent != null)
                builder.AddContent(3, ChildContent);

            builder.CloseElement();
        }

        private string CssClass
        {
            get
            {
                var size = Math.Min((int)_size, SizeCount - 1);
                var weight = Math.Min((int)_weight, WeightCount - 1);
                var color = Math.Min((int)_color, ColorCount - 1);
                var modifiers = (int)_modifiers & (ModifierCount - 1);
                var index = size + SizeCount * (weight + WeightCount * (color + ColorCount * modifiers));

                return _classes[index] ??= "fb-txt" + SizeClasses[size] + WeightClasses[weight] + ColorClasses[color] + ModifierClasses[modifiers];
            }
        }
    }
}
