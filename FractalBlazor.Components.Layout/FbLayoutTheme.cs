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
        /// Border size for small frames.
        /// </summary>
        [Parameter]
        public string SmallFrameBorderSize { get; set; } = "0.07rem";

        /// <summary>
        /// Border color for small frames.
        /// </summary>
        [Parameter]
        public string SmallFrameBorderColor { get; set; } = "#121212";

        /// <summary>
        /// Border size for medium frames.
        /// </summary>
        [Parameter]
        public string MediumFrameBorderSize { get; set; } = "0.07rem";

        /// <summary>
        /// Border color for medium frames.
        /// </summary>
        [Parameter]
        public string MediumFrameBorderColor { get; set; } = "#181818";

        /// <summary>
        /// Border size for large frames.
        /// </summary>
        [Parameter]
        public string LargeFrameBorderSize { get; set; } = "0.07rem";

        /// <summary>
        /// Border color for large frames.
        /// </summary>
        [Parameter]
        public string LargeFrameBorderColor { get; set; } = "#202020";

        /// <summary>
        /// Background color for default background.
        /// </summary>
        [Parameter]
        public string DefaultBackground { get; set; } = "#040404";

        /// <summary>
        /// Background color for accent background.
        /// </summary>
        [Parameter]
        public string AccentBackground { get; set; } = "#080808";

        /// <summary>
        /// Background color for highlight background.
        /// </summary>
        [Parameter]
        public string HighlightBackground { get; set; } = "#0D0D0D";

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
