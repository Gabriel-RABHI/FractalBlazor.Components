using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout
{
    public class FbFlexItem : FbLayoutVisibleComponentBase
    {
        #region HIDDEN
        private FbFlexVerticalAlign _verticalAlign = FbFlexVerticalAlign.None;

        public FbFlexItem()
        {
            IsFlex = true;
        }

        private string VerticalAlignString
        {
            get
            {
                // auto | flex-start | flex-end | center | baseline | stretch
                switch (_verticalAlign)
                {
                    case FbFlexVerticalAlign.Top:
                        return "flex-start";
                    case FbFlexVerticalAlign.Bottom:
                        return "flex-end";
                    case FbFlexVerticalAlign.Center:
                        return "center";
                    case FbFlexVerticalAlign.Stretch:
                        return "stretch";
                    case FbFlexVerticalAlign.Baseline:
                        return "baseline";
                }
                return "";
            }
        }

        private new string ComputedStyle
        {
            get
            {
                return AggregatedStyles +
                        (_verticalAlign != FbFlexVerticalAlign.None ? $"align-self:{VerticalAlignString};" : "");
            }
        }
        #endregion

        // ************************************************************************************************ //
        // ***************************************    PUBLIC   ******************************************** //
        // ************************************************************************************************ //
        /// <summary>
        /// Child content
        /// </summary>
        [Parameter]
        public RenderFragment ChildContent { get; set; }
        
        /// <summary>
        /// Align -> Top
        /// </summary>
        [Parameter]
        public bool Top { get => _verticalAlign == FbFlexVerticalAlign.Top; set { if (value) _verticalAlign = FbFlexVerticalAlign.Top; } }

        /// <summary>
        /// Align -> Bottom
        /// </summary>
        [Parameter]
        public bool Bottom { get => _verticalAlign == FbFlexVerticalAlign.Bottom; set { if (value) _verticalAlign = FbFlexVerticalAlign.Bottom; } }
        
        /// <summary>
        /// Align -> Right
        /// </summary>
        [Parameter]
        public bool Right { get => _verticalAlign == FbFlexVerticalAlign.Top; set { if (value) _verticalAlign = FbFlexVerticalAlign.Top; } }

        /// <summary>
        /// Align -> Left
        /// </summary>
        [Parameter]
        public bool Left { get => _verticalAlign == FbFlexVerticalAlign.Bottom; set { if (value) _verticalAlign = FbFlexVerticalAlign.Bottom; } }

        /// <summary>
        /// Align -> Center
        /// </summary>
        [Parameter]
        public bool Center { get => _verticalAlign == FbFlexVerticalAlign.Center; set { if (value) _verticalAlign = FbFlexVerticalAlign.Center; } }

        /// <summary>
        /// Align -> Stretch
        /// </summary>
        [Parameter]
        public bool Stretch { get => _verticalAlign == FbFlexVerticalAlign.Stretch; set { if (value) _verticalAlign = FbFlexVerticalAlign.Stretch; } }

        /// <summary>
        /// Align -> Baseline
        /// </summary>
        [Parameter]
        public bool Baseline { get => _verticalAlign == FbFlexVerticalAlign.Baseline; set { if (value) _verticalAlign = FbFlexVerticalAlign.Baseline; } }
        
        /// <summary>
        /// Vertical alignment setting
        /// </summary>
        [Parameter]
        public FbFlexVerticalAlign VerticalAlign { get => _verticalAlign; set => _verticalAlign = value; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "style", ComputedStyle);
            builder.AddAttribute(2, "class", Classes);
            builder.AddContent(3, ChildContent);
            builder.CloseElement();
        }
    }
}
