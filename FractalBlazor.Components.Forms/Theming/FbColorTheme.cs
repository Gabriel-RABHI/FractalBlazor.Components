using FractalBlazor.Components.Layout;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace FractalBlazor.Components.Forms.Theming
{
    public class FbColorTheme : FbComponentBase
    {
        /// <summary>
        /// CSS Selector to apply the layout variables to. Defaults to ":root".
        /// </summary>
        [Parameter]
        public string Selector { get; set; } = ":root";

        [Parameter]
        public string Color { get; set; } = "#BDBDBD";

        [Parameter]
        public string PrimaryColor { get; set; } = "#2196F3";


        // -------- https://mudblazor.com/features/colors#material-colors-csharp-and-material-colors
        [Parameter]
        public string RedColor { get; set; } = "#F44336";

        // ...

        [Parameter]
        public string ShadowColorBackMix { get; set; } = "60%";

        [Parameter]
        public string MutedColorBackMix { get; set; } = "35%";

        [Parameter]
        public string AccentColorFrontMix { get; set; } = "70%";

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "style");
            builder.AddContent(1,
                $"{Selector} {{\n" +
                $"  --fb-base-color: {Color};\n" +
                $"  --fb-primary-color: {PrimaryColor};\n" +
                $"  --fb-shadow-color: color-mix(in srgb, var(--fb-base-color), var(--fb-back-color) {ShadowColorBackMix});\n" +
                $"  --fb-mute-color: color-mix(in srgb, var(--fb-base-color), var(--fb-back-color) {MutedColorBackMix});\n" +
                $"  --fb-accent-color: color-mix(in srgb, var(--fb-base-color), var(--fb-front-color) {AccentColorFrontMix});\n" +
                $"}}"
            );
            builder.CloseElement();
        }
    }

    public class FbFontTheme : FbComponentBase
    {
        /// <summary>
        /// CSS Selector to apply the layout variables to. Defaults to ":root".
        /// </summary>
        [Parameter]
        public string Selector { get; set; } = ":root";

        // -------- Default
        [Parameter]
        public string FontSizeBase { get; set; } = "14px";

        [Parameter]
        public string FontWeight { get; set; } = "400";

        [Parameter]
        public string LineHeight { get; set; } = "1.4";

        // -------- Sizes
        [Parameter]
        public string SmallCoef { get; set; } = "0.85";

        [Parameter]
        public string MediumCoef { get; set; } = "1";

        [Parameter]
        public string LargeCoef { get; set; } = "1.25";

        [Parameter]
        public string ExtraLargeCoef { get; set; } = "1.6";

        // -------- Weights
        [Parameter]
        public string ThinWeight { get; set; } = "300";

        [Parameter]
        public string BoldWeight { get; set; } = "600";

        [Parameter]
        public string ExtraBoldWeight { get; set; } = "800";

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "style");
            builder.AddContent(1,
                $"{Selector} {{\n" + //--font-size-base
                $"  --font-size-base: {FontSizeBase};\n" +
                $"  --fb-txt-base-size: {FontSizeBase};\n" +
                $"  --fb-txt-base-weight: {FontWeight};\n" +
                $"  --fb-txt-base-line-height: {LineHeight};\n" +
                $"  --fb-txt-t-weight: {ThinWeight};\n" +
                $"  --fb-txt-b-weight: {BoldWeight};\n" +
                $"  --fb-txt-xb-weight: {ExtraBoldWeight};\n" +
                $"  --fb-txt-s-coef: {SmallCoef};\n" +
                $"  --fb-txt-m-coef: {MediumCoef};\n" +
                $"  --fb-txt-l-coef: {LargeCoef};\n" +
                $"  --fb-txt-x-coef: {ExtraLargeCoef};\n" +
                $"}}"
            );
            builder.CloseElement();
        }
    }
}
