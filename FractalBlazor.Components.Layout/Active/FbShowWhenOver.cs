using System;
using System.Threading.Tasks;
using FractalBlazor.Components.Layout.Abstracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout
{
    public class FbShowWhenOver : FbLayoutComponentBase
    {
        private Guid _id = Guid.NewGuid();

        private bool _visible = false;
        private bool _auto = true;
        private bool _oldShowState = true;

        private string _topLeftOffset = null;
        private string _topCenterOffset = null;
        private string _topRightOffset = null;

        private string _leftOffset = null;
        private string _rightOffset = null;

        private string _bottomLeftOffset = null;
        private string _bottomCenterOffset = null;
        private string _bottomRightOffset = null;

        /// <summary>
        /// Child content
        /// </summary>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        // -------- Top
        /// <summary>
        /// Top Left Content
        /// </summary>
        [Parameter]
        public RenderFragment TopLeftContent { get; set; }

        /// <summary>
        /// Top Center Content
        /// </summary>
        [Parameter]
        public RenderFragment TopCenterContent { get; set; }

        /// <summary>
        /// Top Right Content
        /// </summary>
        [Parameter]
        public RenderFragment TopRightContent { get; set; }

        // -------- Center
        /// <summary>
        /// Left Content
        /// </summary>
        [Parameter]
        public RenderFragment LeftContent { get; set; }

        /// <summary>
        /// Center Content
        /// </summary>
        [Parameter]
        public RenderFragment CenterContent { get; set; }

        /// <summary>
        /// Right Content
        /// </summary>
        [Parameter]
        public RenderFragment RightContent { get; set; }

        // -------- Bottom
        /// <summary>
        /// Bottom Left Content
        /// </summary>
        [Parameter]
        public RenderFragment BottomLeftContent { get; set; }

        /// <summary>
        /// Bottom Center Content
        /// </summary>
        [Parameter]
        public RenderFragment BottomCenterContent { get; set; }

        /// <summary>
        /// Bottom Right Content
        /// </summary>
        [Parameter]
        public RenderFragment BottomRightContent { get; set; }

        /// <summary>
        /// Offsets size
        /// </summary>
        [Parameter]
        public string Offsets
        {
            get => _topLeftOffset;
            set
            {
                _topLeftOffset = _topCenterOffset = _topRightOffset = value;
                _leftOffset = _rightOffset = value;
                _bottomLeftOffset = _bottomCenterOffset = _bottomRightOffset = value;
            }
        }

        /// <summary>
        /// Top Left Offset
        /// </summary>
        [Parameter]
        public string TopLeftOffset { get => _topLeftOffset; set => _topLeftOffset = value; }

        /// <summary>
        /// Top Center Offset
        /// </summary>
        [Parameter]
        public string TopCenterOffset { get => _topCenterOffset; set => _topCenterOffset = value; }

        /// <summary>
        /// Top Right Offset
        /// </summary>
        [Parameter]
        public string TopRightOffset { get => _topRightOffset; set => _topRightOffset = value; }

        /// <summary>
        /// Left Offset
        /// </summary>
        [Parameter]
        public string LeftOffset { get => _leftOffset; set => _leftOffset = value; }

        /// <summary>
        /// Right Offset
        /// </summary>
        [Parameter]
        public string RightOffset { get => _rightOffset; set => _rightOffset = value; }

        /// <summary>
        /// Bottom Left Offset
        /// </summary>
        [Parameter]
        public string BottomLeftOffset { get => _bottomLeftOffset; set => _bottomLeftOffset = value; }

        /// <summary>
        /// Bottom Center Offset
        /// </summary>
        [Parameter]
        public string BottomCenterOffset { get => _bottomCenterOffset; set => _bottomCenterOffset = value; }

        /// <summary>
        /// Bottom Right Offset
        /// </summary>
        [Parameter]
        public string BottomRightOffset { get => _bottomRightOffset; set => _bottomRightOffset = value; }

        /// <summary>
        /// Automatic visibility
        /// </summary>
        [Parameter]
        public bool Automatic
        {
            get => _auto;
            set
            {
                _auto = value;
            }
        }

        /// <summary>
        /// Explicit visibility
        /// </summary>
        [Parameter]
        public bool Visible
        {
            get => _visible;
            set
            {
                _visible = value;
                _auto = false;
            }
        }

        /// <summary>
        /// Fixed visibility
        /// </summary>
        [Parameter]
        public bool Fixed
        {
            get => !Automatic && Visible;
            set
            {
                Automatic = Visible = value;
            }
        }

        /// <summary>
        /// Z-index value
        /// </summary>
        [Parameter]
        public int ZIndex { get; set; } = 12;

        private string AutomaticTag => Automatic ? "AUTO" : "-";

        public bool Show => _auto ? false : _visible;

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "cpnt", $"show-when-over[{StoreId}]");
            builder.AddAttribute(2, "style", $"display: block; position: relative; {AggregatedStyles}");
            builder.AddAttribute(3, "class", Classes);
            builder.AddAttribute(4, "onmouseover", $"FbIsOver('{_id}', '{AutomaticTag}');");
            builder.AddAttribute(5, "onmouseout", $"FbIsLeaving('{_id}', '{AutomaticTag}');");

            builder.OpenElement(6, "div");
            builder.AddAttribute(7, "style", $"visibility: {(Show ? "inherit" : "collapse")}; opacity: {(Show ? "1" : "0")};");
            builder.AddAttribute(8, "class", "fb-utils-fit-parent fb-swo-frame");
            builder.AddAttribute(9, "id", _id.ToString());

            // -------- Top
            if (TopLeftContent != null)
            {
                builder.OpenElement(10, "div");
                builder.AddAttribute(11, "style", "display: flex; align-items: start; justify-content: start;");
                builder.AddAttribute(12, "class", "fb-utils-fit-parent");
                builder.OpenElement(13, "div");
                builder.AddAttribute(14, "style", $"{(string.IsNullOrWhiteSpace(_topLeftOffset) ? "" : $"margin-left:{_topLeftOffset}; margin-top:{_topLeftOffset};")} z-index:{ZIndex};");
                builder.AddContent(15, TopLeftContent);
                builder.CloseElement();
                builder.CloseElement();
            }

            if (TopCenterContent != null)
            {
                builder.OpenElement(16, "div");
                builder.AddAttribute(17, "style", "display: flex; align-items: start; justify-content: center;");
                builder.AddAttribute(18, "class", "fb-utils-fit-parent");
                builder.OpenElement(19, "div");
                builder.AddAttribute(20, "style", $"{(string.IsNullOrWhiteSpace(_topCenterOffset) ? "" : $"margin-top:{_topCenterOffset};")} z-index:{ZIndex};");
                builder.AddContent(21, TopCenterContent);
                builder.CloseElement();
                builder.CloseElement();
            }

            if (TopRightContent != null)
            {
                builder.OpenElement(22, "div");
                builder.AddAttribute(23, "style", "display: flex; align-items: start; justify-content: end;");
                builder.AddAttribute(24, "class", "fb-utils-fit-parent");
                builder.OpenElement(25, "div");
                builder.AddAttribute(26, "style", $"{(string.IsNullOrWhiteSpace(_topRightOffset) ? "" : $"margin-right:{_topRightOffset}; margin-top:{_topRightOffset};")} z-index:{ZIndex};");
                builder.AddContent(27, TopRightContent);
                builder.CloseElement();
                builder.CloseElement();
            }

            // -------- Center
            if (LeftContent != null)
            {
                builder.OpenElement(28, "div");
                builder.AddAttribute(29, "style", "display: flex; align-items: center; justify-content: start;");
                builder.AddAttribute(30, "class", "fb-utils-fit-parent");
                builder.OpenElement(31, "div");
                builder.AddAttribute(32, "style", $"{(string.IsNullOrWhiteSpace(_leftOffset) ? "" : $"margin-left:{_leftOffset};")} z-index:{ZIndex};");
                builder.AddContent(33, LeftContent);
                builder.CloseElement();
                builder.CloseElement();
            }

            if (CenterContent != null)
            {
                builder.OpenElement(34, "div");
                builder.AddAttribute(35, "style", "display: flex; align-items: center; justify-content: center;");
                builder.AddAttribute(36, "class", "fb-utils-fit-parent");
                builder.OpenElement(37, "div");
                builder.AddAttribute(38, "style", $"z-index:{ZIndex};");
                builder.AddContent(39, CenterContent);
                builder.CloseElement();
                builder.CloseElement();
            }

            if (RightContent != null)
            {
                builder.OpenElement(40, "div");
                builder.AddAttribute(41, "style", "display: flex; align-items: center; justify-content: end;");
                builder.AddAttribute(42, "class", "fb-utils-fit-parent");
                builder.OpenElement(43, "div");
                builder.AddAttribute(44, "style", $"{(string.IsNullOrWhiteSpace(_rightOffset) ? "" : $"margin-right:{_rightOffset};")} z-index:{ZIndex};");
                builder.AddContent(45, RightContent);
                builder.CloseElement();
                builder.CloseElement();
            }

            // -------- Bottom
            if (BottomLeftContent != null)
            {
                builder.OpenElement(46, "div");
                builder.AddAttribute(47, "style", "display: flex; align-items: end; justify-content: start;");
                builder.AddAttribute(48, "class", "fb-utils-fit-parent");
                builder.OpenElement(49, "div");
                builder.AddAttribute(50, "style", $"{(string.IsNullOrWhiteSpace(_bottomLeftOffset) ? "" : $"margin-left:{_bottomLeftOffset}; margin-bottom:{_bottomLeftOffset};")} z-index:{ZIndex};");
                builder.AddContent(51, BottomLeftContent);
                builder.CloseElement();
                builder.CloseElement();
            }

            if (BottomCenterContent != null)
            {
                builder.OpenElement(52, "div");
                builder.AddAttribute(53, "style", "display: flex; align-items: end; justify-content: center;");
                builder.AddAttribute(54, "class", "fb-utils-fit-parent");
                builder.OpenElement(55, "div");
                builder.AddAttribute(56, "style", $"{(string.IsNullOrWhiteSpace(_topCenterOffset) ? "" : $"margin-bottom:{_topCenterOffset};")} z-index:{ZIndex};");
                builder.AddContent(57, BottomCenterContent);
                builder.CloseElement();
                builder.CloseElement();
            }

            if (BottomRightContent != null)
            {
                builder.OpenElement(58, "div");
                builder.AddAttribute(59, "style", "display: flex; align-items: end; justify-content: end;");
                builder.AddAttribute(60, "class", "fb-utils-fit-parent");
                builder.OpenElement(61, "div");
                builder.AddAttribute(62, "style", $"{(string.IsNullOrWhiteSpace(_bottomRightOffset) ? "" : $"margin-right:{_bottomRightOffset}; margin-bottom:{_bottomRightOffset};")} z-index:{ZIndex};");
                builder.AddContent(63, BottomRightContent);
                builder.CloseElement();
                builder.CloseElement();
            }

            builder.CloseElement(); // Swo frame div

            builder.AddContent(64, ChildContent);
            builder.CloseElement(); // Main container div
        }
    }
}
