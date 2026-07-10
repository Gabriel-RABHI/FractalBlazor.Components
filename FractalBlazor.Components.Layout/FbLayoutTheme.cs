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
        // ---------------- ABSOLUTE, ROOT VARIABLES ---------------- //

        /// <summary>
        /// CSS Selector to apply the layout variables to. Defaults to ":root".
        /// </summary>
        [Parameter]
        public string Selector { get; set; } = ":root";

        /// <summary>
        /// Background color for default background.
        /// </summary>
        [Parameter]
        public string AbsoluteBackColor { get; set; } = "#000";

        /// <summary>
        /// Background color for default background.
        /// </summary>
        [Parameter]
        public string AbsoluteForeColor { get; set; } = "#FFF";

        /// <summary>
        /// Background color for default background.
        /// </summary>
        [Parameter]
        public string BaseBackColor { get; set; } = "#546E7A";

        /// <summary>
        /// Background color for default background.
        /// </summary>
        [Parameter]
        public string BaseForeColor { get; set; } = "#546E7A";

        /// <summary>
        /// Offset from the current computed .
        /// </summary>
        [Parameter]
        public string DefaultBackgroundBackMix { get; set; } = "90%";

        /// <summary>
        /// Offset from the current computed .
        /// </summary>
        [Parameter]
        public string DefaultForegroundForeMix { get; set; } = "90%";

        // ---------------- RELATIVE TO CURRENT BACKGROUND ---------------- //

        /// <summary>
        /// Offset from the current computed background : still the same with 0%.
        /// </summary>
        [Parameter]
        public string SurfaceBackgroundMixOffset { get; set; } = "0%";
        /// <summary>
        /// Background color for accent background.
        /// </summary>
        [Parameter]
        public string AccentBackgroundMixOffset { get; set; } = "8%";

        /// <summary>
        /// Background color for highlight background.
        /// </summary>
        [Parameter]
        public string HighlightBackgroundMixOffset { get; set; } = "16%";

        // ---------------- RELATIVE TO CURRENT BACKGROUND LEVEL ---------------- //
        /// <summary>
        /// Background color for highlight background.
        /// </summary>
        [Parameter]
        public string HoverBackgroundMixOffset { get; set; } = "12%";

        // ---------------- RELATIVE TO CURRENT BACKGROUND ---------------- //

        /// <summary>
        /// Border size for small frames.
        /// </summary>
        [Parameter]
        public string SmallFrameBorderSize { get; set; } = "0.07rem";

        /// <summary>
        /// Border color for small frames.
        /// </summary>
        [Parameter]
        public string SmallFrameBorderMixOffset { get; set; } = "10%";

        /// <summary>
        /// Border size for medium frames.
        /// </summary>
        [Parameter]
        public string MediumFrameBorderSize { get; set; } = "0.07rem";

        /// <summary>
        /// Border color for medium frames.
        /// </summary>
        [Parameter]
        public string MediumFrameBorderMixOffset { get; set; } = "20%";

        /// <summary>
        /// Border size for large frames.
        /// </summary>
        [Parameter]
        public string LargeFrameBorderSize { get; set; } = "0.14rem";

        /// <summary>
        /// Border color for large frames.
        /// </summary>
        [Parameter]
        public string LargeFrameBorderMixOffset { get; set; } = "20%";

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "style");
            builder.AddContent(1, 
                $"{Selector} {{\n" +
                // -------- Base elements
                $"  --fb-abs-back-color: {AbsoluteBackColor};\n" +
                $"  --fb-abs-front-color: {AbsoluteForeColor};\n" +
                $"  --fb-base-back-color: {BaseBackColor};\n" +

                // -------- (bm) Back Mix
                $"  --fb-parent-bm: {DefaultBackgroundBackMix};\n" +
                $"  --fb-current-bm: {DefaultBackgroundBackMix};\n" +

                // -------- (bo) Back Offsets
                $"  --fb-constant-background-bo: {SurfaceBackgroundMixOffset};\n" +
                $"  --fb-accent-background-bo: {AccentBackgroundMixOffset};\n" +
                $"  --fb-highlight-background-bo: {HighlightBackgroundMixOffset};\n" +
                $"  --fb-hover-background-bo: {HoverBackgroundMixOffset};\n" +

                // -------- (bo) Back Offsets
                $"  --fb-s-frame-border-bo: {SmallFrameBorderMixOffset};\n" +
                $"  --fb-m-frame-border-bo: {MediumFrameBorderMixOffset};\n" +
                $"  --fb-l-frame-border-bo: {LargeFrameBorderMixOffset};\n" +

                // -------- Sizes
                $"  --fb-s-frame-border-size: {SmallFrameBorderSize};\n" +
                $"  --fb-m-frame-border-size: {MediumFrameBorderSize};\n" +
                $"  --fb-l-frame-border-size: {LargeFrameBorderSize};\n" +

                // -------- Spacings
                $"  --fb-s-spacing: {FbLayoutPresets.ToRem(FbLayoutPresets.S)};\n" +
                $"  --fb-m-spacing: {FbLayoutPresets.ToRem(FbLayoutPresets.M)};\n" +
                $"  --fb-l-spacing: {FbLayoutPresets.ToRem(FbLayoutPresets.L)};\n" +
                $"  --fb-x-spacing: {FbLayoutPresets.ToRem(FbLayoutPresets.X)};\n" +

                // -------- Radius
                $"  --fb-s-radius: {FbLayoutPresets.ToRem(FbLayoutPresets.RS)};\n" +
                $"  --fb-m-radius: {FbLayoutPresets.ToRem(FbLayoutPresets.RM)};\n" +
                $"  --fb-l-radius: {FbLayoutPresets.ToRem(FbLayoutPresets.RL)};\n" +
                $"  --fb-x-radius: {FbLayoutPresets.ToRem(FbLayoutPresets.RX)};\n" +
                $"}}"
            );
            builder.CloseElement();
        }
    }
}
