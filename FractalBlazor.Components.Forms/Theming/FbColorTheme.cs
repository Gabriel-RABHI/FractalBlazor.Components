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
        public string ForegroundAnchor { get; set; } = "#111113";

        [Parameter]
        public string ForegroundHighAnchor { get; set; } = "#F7F7F8";


        // -------- https://mudblazor.com/features/colors#material-colors-csharp-and-material-colors
        [Parameter]
        public string RedColor { get; set; } = "#F44336";

        // ...

        [Parameter]
        public string ForegroundDefaultHighMix { get; set; } = "82%";

        [Parameter]
        public string ForegroundSubtleHighMix { get; set; } = "46%";

        [Parameter]
        public string ForegroundMutedHighMix { get; set; } = "64%";

        [Parameter]
        public string ForegroundHighlightHighMix { get; set; } = "100%";

        [Parameter]
        [Obsolete("Use ForegroundDefaultHighMix instead.")]
        public string ForegroundDefaultTintMix { get => ForegroundDefaultHighMix; set => ForegroundDefaultHighMix = value; }

        [Parameter]
        [Obsolete("Use ForegroundHighlightHighMix instead.")]
        public string ForegroundHighlightTintMix { get => ForegroundHighlightHighMix; set => ForegroundHighlightHighMix = value; }

        [Parameter]
        [Obsolete("Use ForegroundAnchor instead.")]
        public string Color { get => ForegroundAnchor; set => ForegroundAnchor = value; }

        [Parameter]
        [Obsolete("Use ForegroundSubtleHighMix instead.")]
        public string ShadowColorBackMix { get => ForegroundSubtleHighMix; set => ForegroundSubtleHighMix = value; }

        [Parameter]
        [Obsolete("Use ForegroundMutedHighMix instead.")]
        public string MutedColorBackMix { get => ForegroundMutedHighMix; set => ForegroundMutedHighMix = value; }

        [Parameter]
        [Obsolete("Use ForegroundHighlightHighMix instead.")]
        public string AccentColorFrontMix { get => ForegroundHighlightHighMix; set => ForegroundHighlightHighMix = value; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "style");
            builder.AddContent(1,
                $"{Selector} {{\n" +
                $"  --fb-fg-anchor: {ForegroundAnchor};\n" +
                $"  --fb-fg-high-anchor: {ForegroundHighAnchor};\n" +
                $"  --fb-fg-default-high-mix: {ForegroundDefaultHighMix};\n" +
                $"  --fb-fg-subtle-high-mix: {ForegroundSubtleHighMix};\n" +
                $"  --fb-fg-muted-high-mix: {ForegroundMutedHighMix};\n" +
                $"  --fb-fg-highlight-high-mix: {ForegroundHighlightHighMix};\n" +
                $"  --fb-fg-default: color-mix(in oklab, var(--fb-fg-anchor), var(--fb-fg-high-anchor) var(--fb-fg-default-high-mix));\n" +
                $"  --fb-fg-subtle: color-mix(in oklab, var(--fb-fg-anchor), var(--fb-fg-high-anchor) var(--fb-fg-subtle-high-mix));\n" +
                $"  --fb-fg-muted: color-mix(in oklab, var(--fb-fg-anchor), var(--fb-fg-high-anchor) var(--fb-fg-muted-high-mix));\n" +
                $"  --fb-fg-highlight: color-mix(in oklab, var(--fb-fg-anchor), var(--fb-fg-high-anchor) var(--fb-fg-highlight-high-mix));\n" +
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
