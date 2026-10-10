using FractalBlazor.Components.Forms.Display;
using FractalBlazor.Components.Layout;
using FractalBlazor.Components.Layout.Abstracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Forms.Icons
{
    /// <summary>
    /// Renders an icon from <see cref="FbMaterialIcons"/> as an inline SVG painted with the
    /// current text color, so it follows the theme, the variant and the surrounding text.
    /// </summary>
    /// <example><c>&lt;FbIcon Icon="@FbMaterialIcons.Search" L Muted /&gt;</c></example>
    public sealed class FbIcon : FbComponentBase
    {
        private const int SizeCount = 5, ColorCount = 4;

        private static readonly string[] SizeClasses = { "", " fb-icon-s", " fb-icon-m", " fb-icon-l", " fb-icon-x" };
        private static readonly string[] ColorClasses = { "", " fb-fg-subtle", " fb-fg-muted", " fb-fg-highlight" };
        private static readonly string?[] _classes = new string?[SizeCount * ColorCount];

        private FbSpacing _size;
        private FbTextColor _color;

        /// <summary>
        /// SVG content of a 24×24 icon, usually a <see cref="FbMaterialIcons"/> constant.
        /// </summary>
        [Parameter, EditorRequired]
        public string Icon { get; set; } = "";

        /// <summary>
        /// Accessible name. Without it the icon is decorative and hidden from assistive technologies.
        /// </summary>
        [Parameter]
        public string? Title { get; set; }

        [Parameter]
        public FbSpacing Size { get => _size; set => _size = value; }

        [Parameter]
        public FbTextColor Color { get => _color; set => _color = value; }

        [Parameter]
        public bool S { get => _size == FbSpacing.S; set { if (value) _size = FbSpacing.S; } }

        [Parameter]
        public bool M { get => _size == FbSpacing.M; set { if (value) _size = FbSpacing.M; } }

        [Parameter]
        public bool L { get => _size == FbSpacing.L; set { if (value) _size = FbSpacing.L; } }

        [Parameter]
        public bool X { get => _size == FbSpacing.X; set { if (value) _size = FbSpacing.X; } }

        [Parameter]
        public bool Subtle { get => _color == FbTextColor.Subtle; set { if (value) _color = FbTextColor.Subtle; } }

        [Parameter]
        public bool Muted { get => _color == FbTextColor.Muted; set { if (value) _color = FbTextColor.Muted; } }

        [Parameter]
        public bool Highlight { get => _color == FbTextColor.Highlight; set { if (value) _color = FbTextColor.Highlight; } }

        protected override void OnBeforeParametersSet()
        {
            _size = default;
            _color = default;
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "svg");
            builder.AddAttribute(1, "class", CssClass);
            builder.AddAttribute(2, "viewBox", "0 0 24 24");
            builder.AddAttribute(3, "fill", "currentColor");
            builder.AddAttribute(4, "focusable", "false");
            if (string.IsNullOrWhiteSpace(Title))
            {
                builder.AddAttribute(5, "aria-hidden", "true");
            }
            else
            {
                builder.AddAttribute(6, "role", "img");
                builder.OpenElement(7, "title");
                builder.AddContent(8, Title);
                builder.CloseElement();
            }
            builder.AddMarkupContent(9, Icon);
            builder.CloseElement();
        }

        private string CssClass
        {
            get
            {
                var size = Math.Min((int)_size, SizeCount - 1);
                var color = Math.Min((int)_color, ColorCount - 1);
                var index = size + SizeCount * color;
                return _classes[index] ??= "fb-icon" + SizeClasses[size] + ColorClasses[color];
            }
        }
    }
}
