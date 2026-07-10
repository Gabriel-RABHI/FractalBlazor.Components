using FractalBlazor.Components.Layout;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace FractalBlazor.Components.Forms.Theming
{
    public class FbColorTheme : FbComponentBase, IFbCssVariables
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

        public string ToCssVariables()
        {
            var builder = new StringBuilder();

            Append(builder, "--fb-default-fg-anchor", ForegroundAnchor);
            Append(builder, "--fb-default-fg-high-anchor", ForegroundHighAnchor);
            Append(builder, "--fb-default-fg-default-high-mix", ForegroundDefaultHighMix);
            Append(builder, "--fb-default-fg-subtle-high-mix", ForegroundSubtleHighMix);
            Append(builder, "--fb-default-fg-muted-high-mix", ForegroundMutedHighMix);
            Append(builder, "--fb-default-fg-highlight-high-mix", ForegroundHighlightHighMix);
            AppendRaw(builder, "--fb-fg-anchor:var(--fb-default-fg-anchor);");
            AppendRaw(builder, "--fb-fg-high-anchor:var(--fb-default-fg-high-anchor);");
            AppendRaw(builder, "--fb-fg-default-high-mix:var(--fb-default-fg-default-high-mix);");
            AppendRaw(builder, "--fb-fg-subtle-high-mix:var(--fb-default-fg-subtle-high-mix);");
            AppendRaw(builder, "--fb-fg-muted-high-mix:var(--fb-default-fg-muted-high-mix);");
            AppendRaw(builder, "--fb-fg-highlight-high-mix:var(--fb-default-fg-highlight-high-mix);");
            AppendRaw(builder, "--fb-current-fg-default-high-mix:var(--fb-fg-default-high-mix);");
            AppendRaw(builder, "--fb-current-fg-subtle-high-mix:var(--fb-fg-subtle-high-mix);");
            AppendRaw(builder, "--fb-current-fg-muted-high-mix:var(--fb-fg-muted-high-mix);");
            AppendRaw(builder, "--fb-current-fg-highlight-high-mix:var(--fb-fg-highlight-high-mix);");
            AppendRaw(builder, "--fb-fg-default:color-mix(in oklab,var(--fb-fg-anchor),var(--fb-fg-high-anchor) var(--fb-current-fg-default-high-mix));");
            AppendRaw(builder, "--fb-fg-subtle:color-mix(in oklab,var(--fb-fg-anchor),var(--fb-fg-high-anchor) var(--fb-current-fg-subtle-high-mix));");
            AppendRaw(builder, "--fb-fg-muted:color-mix(in oklab,var(--fb-fg-anchor),var(--fb-fg-high-anchor) var(--fb-current-fg-muted-high-mix));");
            AppendRaw(builder, "--fb-fg-highlight:color-mix(in oklab,var(--fb-fg-anchor),var(--fb-fg-high-anchor) var(--fb-current-fg-highlight-high-mix));");

            return builder.ToString();
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "style");
            builder.AddContent(1, $"{Selector}{{{ToCssVariables()}}}");
            builder.CloseElement();
        }

        private static void Append(StringBuilder builder, string name, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                builder.Append(name).Append(':').Append(value).Append(';');
        }

        private static void AppendRaw(StringBuilder builder, string value)
            => builder.Append(value);
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
