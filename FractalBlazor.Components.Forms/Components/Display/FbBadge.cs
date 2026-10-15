using FractalBlazor.Components.Forms.Theming.Helpers;
using FractalBlazor.Components.Layout;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Forms.Components.Display
{
    /// <summary>
    /// A small rounded label: a status, a count, a tag. It is a row, inline and round, padded
    /// <c>PHM PVS</c> and filled with the full color of its variant (<c>WBH</c>); it shows
    /// <see cref="Label"/>, or <c>ChildContent</c> in its place (an icon and a text…).
    /// With <see cref="Dot"/>, it is only a small dot of that color.
    /// </summary>
    /// <remarks>
    /// The Layout flags of the row remain to change its look: <c>WBA</c> for a softer fill,
    /// <c>WBN WFM</c> for an outline, <c>PHS</c>, <c>GM</c>… A <see cref="Variant"/>
    /// (<c>Error</c>, <c>Success</c>, <c>Red</c>…) colors it.
    /// </remarks>
    /// <example><c>&lt;FbBadge Label="Nouveau" Variant="@FbThemeVariants.Success" /&gt;</c></example>
    public class FbBadge : FbRow
    {
        private bool _dot, _defaultDot;
        private string? _rowClasses, _variantClasses;
        private bool _classesDot;
        private string _badgeClasses = "";

        public FbBadge()
        {
            IsInlineFlex = true;
            NoFlex = true;
            JVC = true;
            GS = true;
            PHM = true;
            PVS = true;
            WRR = true;
            WBH = true;
            WNTW = true;
        }

        /// <summary>
        /// The text of the badge. <c>ChildContent</c>, when given, takes its place.
        /// </summary>
        [Parameter]
        public string? Label { get; set; }

        /// <summary>
        /// A small dot of the color of the badge, without content: a status beside a name, a
        /// mark on an icon. Give it a <see cref="Title"/>, which names it for assistive
        /// technologies.
        /// </summary>
        [Parameter]
        public bool Dot { get => _dot; set => _dot = value; }

        /// <summary>
        /// Functional state of the badge: <c>Error</c>, <c>Success</c>, <c>Info</c>, a color…
        /// </summary>
        [Parameter]
        public string? Variant { get; set; }

        /// <summary>
        /// Tooltip of the badge; the accessible name of a <see cref="Dot"/>.
        /// </summary>
        [Parameter]
        public string? Title { get; set; }

        protected override void CaptureParameterDefaults()
        {
            base.CaptureParameterDefaults();
            _defaultDot = _dot;
        }

        protected override void RestoreParameterDefaults()
        {
            base.RestoreParameterDefaults();
            _dot = _defaultDot;
        }

        /// <summary>
        /// The row classes come from the shared cache: the badge classes are only joined again
        /// when they, the variant or the dot change.
        /// </summary>
        private string BadgeClasses
        {
            get
            {
                var rowClasses = RowClasses;
                var variantClasses = FbVariantScope.Classes(FbVariantScope.Effective(Variant, disabled: false));
                if (!ReferenceEquals(rowClasses, _rowClasses) || !ReferenceEquals(variantClasses, _variantClasses) || _classesDot != _dot || _badgeClasses.Length == 0)
                {
                    (_rowClasses, _variantClasses, _classesDot) = (rowClasses, variantClasses, _dot);
                    _badgeClasses = FbVariantScope.WithClasses((_dot ? "fb-badge fb-badge-dot " : "fb-badge ") + rowClasses, variantClasses);
                }
                return _badgeClasses;
            }
        }

        /// <summary>
        /// An inline element, so that a badge can sit in a sentence. A dot has no content and
        /// is named by its title.
        /// </summary>
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "span");
            builder.AddAttribute(1, "class", BadgeClasses);
            builder.AddAttribute(2, "style", AggregatedStyles);
            builder.AddAttribute(3, "title", Title);
            if (_dot)
            {
                builder.AddAttribute(4, "role", "img");
                builder.AddAttribute(5, "aria-label", Title);
            }
            if (OnClick.HasDelegate)
                builder.AddAttribute(6, "onclick", OnClick);

            if (!_dot)
            {
                if (ChildContent is not null)
                    builder.AddContent(7, ChildContent);
                else if (!string.IsNullOrEmpty(Label))
                {
                    builder.OpenComponent<FbText>(8);
                    builder.AddComponentParameter(9, nameof(FbText.S), true);
                    builder.AddComponentParameter(10, nameof(FbText.B), true);
                    builder.AddComponentParameter(11, nameof(FbText.ChildContent), (RenderFragment)(text => text.AddContent(0, Label)));
                    builder.CloseComponent();
                }
            }
            builder.CloseElement();
        }
    }
}
