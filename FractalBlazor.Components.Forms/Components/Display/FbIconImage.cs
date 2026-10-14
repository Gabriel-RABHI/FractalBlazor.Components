using FractalBlazor.Components.Layout;
using FractalBlazor.Components.Layout.Abstracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Forms.Components.Display
{
    /// <summary>
    /// Renders an image sized like an <c>FbIcon</c>, by its height relative to the surrounding
    /// text; its width follows the ratio of the image. A logo in a header, a picture beside a
    /// label.
    /// </summary>
    /// <example><c>&lt;FbIconImage Src="images/logo.png" Alt="FractalBlazor" L /&gt;</c></example>
    public sealed class FbIconImage : FbComponentBase
    {
        private static readonly string[] SizeClasses =
        {
            "fb-icon-image",
            "fb-icon-image fb-icon-image-s",
            "fb-icon-image fb-icon-image-m",
            "fb-icon-image fb-icon-image-l",
            "fb-icon-image fb-icon-image-x"
        };

        private FbSpacing _size;

        /// <summary>
        /// Address of the image.
        /// </summary>
        [Parameter, EditorRequired]
        public string Src { get; set; } = "";

        /// <summary>
        /// Text alternative. Without it the image is decorative: its <c>alt</c> is empty and
        /// assistive technologies skip it.
        /// </summary>
        [Parameter]
        public string? Alt { get; set; }

        [Parameter]
        public FbSpacing Size { get => _size; set => _size = value; }

        [Parameter]
        public bool S { get => _size == FbSpacing.S; set { if (value) _size = FbSpacing.S; } }

        [Parameter]
        public bool M { get => _size == FbSpacing.M; set { if (value) _size = FbSpacing.M; } }

        [Parameter]
        public bool L { get => _size == FbSpacing.L; set { if (value) _size = FbSpacing.L; } }

        [Parameter]
        public bool X { get => _size == FbSpacing.X; set { if (value) _size = FbSpacing.X; } }

        protected override void OnBeforeParametersSet() => _size = default;

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "img");
            builder.AddAttribute(1, "class", SizeClasses[Math.Min((int)_size, SizeClasses.Length - 1)]);
            builder.AddAttribute(2, "src", Src);
            builder.AddAttribute(3, "alt", Alt ?? "");
            builder.CloseElement();
        }
    }
}
