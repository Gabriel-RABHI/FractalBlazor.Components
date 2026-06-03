using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout
{
    /// <summary>
    /// Component that dynamically generates and updates layout style variables at runtime.
    /// </summary>
    public class FbLayoutTheme : FbComponentBase
    {
        /// <summary>
        /// CSS Selector to apply the layout variables to. Defaults to ":root".
        /// </summary>
        [Parameter]
        public string Selector { get; set; } = ":root";


        /// <summary>
        /// Background color for default background.
        /// </summary>
        [Parameter]
        public string BackColor { get; set; } = "#000";

        /// <summary>
        /// Background color for default background.
        /// </summary>
        [Parameter]
        public string FrontColor { get; set; } = "#FFF";

        /// <summary>
        /// Background color for default background.
        /// </summary>
        [Parameter]
        public string LayoutBaseColor { get; set; } = "#546E7A";

        /// <summary>
        /// Border size for small frames.
        /// </summary>
        [Parameter]
        public string SmallFrameBorderSize { get; set; } = "0.07rem";

        /// <summary>
        /// Border color for small frames.
        /// </summary>
        [Parameter]
        public string SmallFrameBorderColorBackMix { get; set; } = "60%";

        /// <summary>
        /// Border size for medium frames.
        /// </summary>
        [Parameter]
        public string MediumFrameBorderSize { get; set; } = "0.07rem";

        /// <summary>
        /// Border color for medium frames.
        /// </summary>
        [Parameter]
        public string MediumFrameBorderColorBackMix { get; set; } = "40%";

        /// <summary>
        /// Border size for large frames.
        /// </summary>
        [Parameter]
        public string LargeFrameBorderSize { get; set; } = "0.07rem";

        /// <summary>
        /// Border color for large frames.
        /// </summary>
        [Parameter]
        public string LargeFrameBorderColorBackMix { get; set; } = "20%";

        /// <summary>
        /// Background color for default background.
        /// </summary>
        [Parameter]
        public string DefaultBackgroundBackMix { get; set; } = "80%";

        /// <summary>
        /// Background color for accent background.
        /// </summary>
        [Parameter]
        public string AccentBackgroundBackMix { get; set; } = "70%";

        /// <summary>
        /// Background color for highlight background.
        /// </summary>
        [Parameter]
        public string HighlightBackgroundBackMix { get; set; } = "60%";

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "style");
            builder.AddContent(1, 
                $"{Selector} {{\n" +
                $"  --fb-back-color: {BackColor};\n" +
                $"  --fb-front-color: {FrontColor};\n" +
                $"  --fb-layout-base-color: {LayoutBaseColor};\n" +
                $"  --fb-s-spacing: {FbLayoutPresets.ToRem(FbLayoutPresets.S)};\n" +
                $"  --fb-m-spacing: {FbLayoutPresets.ToRem(FbLayoutPresets.M)};\n" +
                $"  --fb-l-spacing: {FbLayoutPresets.ToRem(FbLayoutPresets.L)};\n" +
                $"  --fb-x-spacing: {FbLayoutPresets.ToRem(FbLayoutPresets.X)};\n" +
                $"  --fb-s-radius: {FbLayoutPresets.ToRem(FbLayoutPresets.RS)};\n" +
                $"  --fb-m-radius: {FbLayoutPresets.ToRem(FbLayoutPresets.RM)};\n" +
                $"  --fb-l-radius: {FbLayoutPresets.ToRem(FbLayoutPresets.RL)};\n" +
                $"  --fb-x-radius: {FbLayoutPresets.ToRem(FbLayoutPresets.RX)};\n" +
                $"  --fb-s-frame-border-size: {SmallFrameBorderSize};\n" +
                $"  --fb-s-frame-border-color: color-mix(in srgb, var(--fb-layout-base-color), var(--fb-back-color) {SmallFrameBorderColorBackMix});\n" +
                $"  --fb-m-frame-border-size: {MediumFrameBorderSize};\n" +
                $"  --fb-m-frame-border-color: color-mix(in srgb, var(--fb-layout-base-color), var(--fb-back-color) {MediumFrameBorderColorBackMix});\n" +
                $"  --fb-l-frame-border-size: {LargeFrameBorderSize};\n" +
                $"  --fb-l-frame-border-color: color-mix(in srgb, var(--fb-layout-base-color), var(--fb-back-color) {LargeFrameBorderColorBackMix});\n" +
                $"  --fb-default-background: color-mix(in srgb, var(--fb-layout-base-color), var(--fb-back-color) {DefaultBackgroundBackMix});\n" +
                $"  --fb-accent-background: color-mix(in srgb, var(--fb-layout-base-color), var(--fb-back-color) {AccentBackgroundBackMix});\n" +
                $"  --fb-highlight-background: color-mix(in srgb, var(--fb-layout-base-color), var(--fb-back-color) {HighlightBackgroundBackMix});\n" +
                $"}}"
            );
            builder.CloseElement();
        }
    }
}
