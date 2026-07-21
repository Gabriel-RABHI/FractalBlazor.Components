using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout
{
    public class FbRowItem : FbLayoutVisibleComponentBase
    {
        #region HIDDEN
        private FbFlexItemSelfAlign _selfAlign = FbFlexItemSelfAlign.None;

        public FbRowItem()
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

        private new string ComputedStyle
        {
            get
            {
                return AggregatedStyles +
                        "flex-wrap: wrap;" +
                        (_selfAlign != FbFlexItemSelfAlign.None ? $"align-self:{VerticalAlignString};" : "");
            }
        }

        // -------- In Row
        /// <summary>
        /// Self -> Justify -> Vertical -> Default
        /// </summary>
        [Parameter]
        public bool SJVD { get => _selfAlign == FbFlexItemSelfAlign.None; set { if (value) _selfAlign = FbFlexItemSelfAlign.None; } }

        /// <summary>
        /// Self -> Justify -> Vertical -> Start
        /// </summary>
        [Parameter]
        public bool SJVS { get => _selfAlign == FbFlexItemSelfAlign.Start; set { if (value) _selfAlign = FbFlexItemSelfAlign.Start; } }

        /// <summary>
        /// Self -> Justify -> Vertical -> End
        /// </summary>
        [Parameter]
        public bool SJVE { get => _selfAlign == FbFlexItemSelfAlign.End; set { if (value) _selfAlign = FbFlexItemSelfAlign.End; } }

        /// <summary>
        /// Self -> Justify -> Vertical -> Center
        /// </summary>
        [Parameter]
        public bool SJVC { get => _selfAlign == FbFlexItemSelfAlign.Center; set { if (value) _selfAlign = FbFlexItemSelfAlign.Center; } }

        /// <summary>
        /// Self -> Justify -> Vertical -> Stretch
        /// </summary>
        [Parameter]
        public bool SJVSt { get => _selfAlign == FbFlexItemSelfAlign.Stretch; set { if (value) _selfAlign = FbFlexItemSelfAlign.Stretch; } }

        /// <summary>
        /// Self -> Justify -> Vertical -> Baseline
        /// </summary>
        [Parameter]
        public bool SJVBl { get => _selfAlign == FbFlexItemSelfAlign.Baseline; set { if (value) _selfAlign = FbFlexItemSelfAlign.Baseline; } }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "cpnt", $"row-item[{StoreId}]");
            builder.AddAttribute(2, "style", ComputedStyle);
            builder.AddAttribute(3, "class", $"{Classes} {AggregatedClasses}");
            builder.AddAttribute(4, "onclick", EventCallback.Factory.Create(this, () => OnClick.InvokeAsync()));
            builder.AddContent(5, ChildContent);
            builder.CloseElement();
        }
    }
}
