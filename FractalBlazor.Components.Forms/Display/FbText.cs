using FractalBlazor.Components.Layout;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace FractalBlazor.Components.Forms.Display
{
    public enum FbTextSize : byte
    {
        None,
        S,
        M,
        L,
        X
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
        None,
        Mute,
        Accent,
        Primary,
        Error
    }

    public enum FbTextModifiers : byte
    {
        NoWrap,
        Trim
    }

    public class FbText : FbComponentBase
    {
        private static string[] FbTextSizeClasses = { "", "fb-t-s", "fb-t-m", "fb-t-l", "fb-t-x" };
        private static string[] FbTextWeightClasses = { "", "fb-t-t", "fb-t-b", "fb-t-xb" };
        private static string[] FbTextIntentClasses = { "", "fb-t-mute", "fb-t-acc", "fb-t-pri", "fb-t-err" };
        private static string[] FbTextModifiersClasses = { "", "fb-t-nw", "fb-t-tr" };

        private RenderHandle _renderHandle;

        private FbTextSize _size;
        private FbTextWeight _weight;
        private FbTextIntent _intent;
        private FbTextModifiers _modifiers;

        // -------- Scale
        [Parameter]
        public bool S { get => _size == FbTextSize.S; set => _size = FbTextSize.S; }

        [Parameter]
        public bool M { get => _size == FbTextSize.M; set => _size = FbTextSize.M; }

        [Parameter]
        public bool L { get => _size == FbTextSize.L; set => _size = FbTextSize.L; }

        [Parameter]
        public bool X { get => _size == FbTextSize.X; set => _size = FbTextSize.X; }

        // -------- Weight
        [Parameter]
        public bool T { get => _weight == FbTextWeight.T; set => _weight = FbTextWeight.T; }

        [Parameter]
        public bool B { get => _weight == FbTextWeight.B; set => _weight = FbTextWeight.B; }

        [Parameter]
        public bool XB { get => _weight == FbTextWeight.XB; set => _weight = FbTextWeight.XB; }

        // -------- Intent
        [Parameter]
        public bool Mute { get => _intent == FbTextIntent.Mute; set => _intent = FbTextIntent.Mute; }

        [Parameter]
        public bool Accent { get => _intent == FbTextIntent.Accent; set => _intent = FbTextIntent.Accent; }

        [Parameter]
        public bool Primary { get => _intent == FbTextIntent.Primary; set => _intent = FbTextIntent.Primary; }

        [Parameter]
        public bool Error { get => _intent == FbTextIntent.Error; set => _intent = FbTextIntent.Error; }

        // -------- Modifiers
        [Parameter]
        public bool NW { get => _modifiers == FbTextModifiers.NoWrap; set => _modifiers = FbTextModifiers.NoWrap; }

        [Parameter]
        public bool TR { get => _modifiers == FbTextModifiers.Trim; set => _modifiers = FbTextModifiers.Trim; }

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
