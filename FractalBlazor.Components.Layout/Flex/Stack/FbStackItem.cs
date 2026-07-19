using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout
{
    public class FbStackItem : FbLayoutComponentBase
    {
        #region HIDDEN
        private FbFlexItemSelfAlign _selfAlign = FbFlexItemSelfAlign.None;
        private int _flex = 0;
        private bool _hadFlex = false;

        public FbStackItem()
        {
            IsFlex = true;
        }

        private string VerticalAlignString
        {
            get
            {
                // auto | flex-start | flex-end | center | baseline | stretch
                switch (_selfAlign)
                {
                    case FbFlexItemSelfAlign.Start:
                        return "flex-start";
                    case FbFlexItemSelfAlign.End:
                        return "flex-end";
                    case FbFlexItemSelfAlign.Center:
                        return "center";
                    case FbFlexItemSelfAlign.Stretch:
                        return "stretch";
                    case FbFlexItemSelfAlign.Baseline:
                        return "baseline";
                }
                return "";
            }
        }

        private new string ComputedStyle
        {
            get
            {
                return ComputedBaseStyle +
                        "flex-wrap: wrap;" +
                        (_selfAlign != FbFlexItemSelfAlign.None ? $"align-self:{VerticalAlignString};" : "");
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
        /// On click event callback
        /// </summary>
        [Parameter]
        public EventCallback OnClick { get; set; }

        // -------- In Column
        /// <summary>
        /// Self -> Justify -> Horizontal -> Default
        /// </summary>
        [Parameter]
        public bool SJHD { get => _selfAlign == FbFlexItemSelfAlign.None; set { if (value) _selfAlign = FbFlexItemSelfAlign.None; } }

        /// <summary>
        /// Self -> Justify -> Horizontal ->Start
        /// </summary>
        [Parameter]
        public bool SJHS { get => _selfAlign == FbFlexItemSelfAlign.Start; set { if (value) _selfAlign = FbFlexItemSelfAlign.Start; } }

        /// <summary>
        /// Self -> Justify -> Horizontal ->End
        /// </summary>
        [Parameter]
        public bool SJHE { get => _selfAlign == FbFlexItemSelfAlign.End; set { if (value) _selfAlign = FbFlexItemSelfAlign.End; } }

        /// <summary>
        /// Self -> Justify -> Horizontal ->Center
        /// </summary>
        [Parameter]
        public bool SJHC { get => _selfAlign == FbFlexItemSelfAlign.Center; set { if (value) _selfAlign = FbFlexItemSelfAlign.Center; } }

        /// <summary>
        /// Self -> Justify -> Horizontal ->Stretch
        /// </summary>
        [Parameter]
        public bool SJHSt { get => _selfAlign == FbFlexItemSelfAlign.Stretch; set { if (value) _selfAlign = FbFlexItemSelfAlign.Stretch; } }

        /// <summary>
        /// Self -> Justify -> Horizontal ->Baseline
        /// </summary>
        [Parameter]
        public bool SJHBL { get => _selfAlign == FbFlexItemSelfAlign.Baseline; set { if (value) _selfAlign = FbFlexItemSelfAlign.Baseline; } }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "cpnt", $"stack-item[{StoreId}]");
            builder.AddAttribute(2, "style", ComputedStyle);
            builder.AddAttribute(3, "class", $"{Classes} {WrapClassString} {ResponsiveClassString}");
            builder.AddAttribute(4, "onclick", EventCallback.Factory.Create(this, () => OnClick.InvokeAsync()));
            builder.AddContent(5, ChildContent);
            builder.CloseElement();
        }
    }
}
