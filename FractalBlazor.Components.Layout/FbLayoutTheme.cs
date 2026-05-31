using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout
{
    /// <summary>
    /// Component that dynamically generates and updates layout style variables at runtime.
    /// </summary>
    public class FbLayoutTheme : FbSimpleComponentBase
    {
        /// <summary>
        /// CSS Selector to apply the layout variables to. Defaults to ":root".
        /// </summary>
        [Parameter]
        public string Selector { get; set; } = ":root";

        /// <summary>
        /// Border size for small frames.
        /// </summary>
        [Parameter]
        public string SmallFrameBorderSize { get; set; } = "0.07rem";

        /// <summary>
        /// Border color for small frames.
        /// </summary>
        [Parameter]
        public string SmallFrameBorderColor { get; set; } = "#e2e2e2";

        /// <summary>
        /// Border size for medium frames.
        /// </summary>
        [Parameter]
        public string MediumFrameBorderSize { get; set; } = "0.07rem";

        /// <summary>
        /// Border color for medium frames.
        /// </summary>
        [Parameter]
        public string MediumFrameBorderColor { get; set; } = "#cecece";

        /// <summary>
        /// Border size for large frames.
        /// </summary>
        [Parameter]
        public string LargeFrameBorderSize { get; set; } = "0.14rem";

        /// <summary>
        /// Border color for large frames.
        /// </summary>
        [Parameter]
        public string LargeFrameBorderColor { get; set; } = "#e2e2e2";

        /// <summary>
        /// Background color for default background.
        /// </summary>
        [Parameter]
        public string DefaultBackground { get; set; } = "#FFF";

        /// <summary>
        /// Background color for accent background.
        /// </summary>
        [Parameter]
        public string AccentBackground { get; set; } = "#f8f7f7";

        /// <summary>
        /// Background color for highlight background.
        /// </summary>
        [Parameter]
        public string HighlightBackground { get; set; } = "#e8f3ff";

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "style");
            builder.AddContent(1, 
                $"{Selector} {{\n" +
                $"  --fb-small-frame-border-size: {SmallFrameBorderSize};\n" +
                $"  --fb-small-frame-border-color: {SmallFrameBorderColor};\n" +
                $"  --fb-medium-frame-border-size: {MediumFrameBorderSize};\n" +
                $"  --fb-medium-frame-border-color: {MediumFrameBorderColor};\n" +
                $"  --fb-large-frame-border-size: {LargeFrameBorderSize};\n" +
                $"  --fb-large-frame-border-color: {LargeFrameBorderColor};\n" +
                $"  --fb-default-background: {DefaultBackground};\n" +
                $"  --fb-accent-background: {AccentBackground};\n" +
                $"  --fb-highlight-background: {HighlightBackground};\n" +
                $"}}"
            );
            builder.CloseElement();
        }
    }
}
