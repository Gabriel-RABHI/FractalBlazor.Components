using System;
using System.Threading.Tasks;
using FractalBlazor.Components.Layout.Abstracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout
{
    public class FbStackItem : FbLayoutSurfaceComponentBase
    {
        #region HIDDEN
        private FbFlexItemSelfAlign _selfAlign = FbFlexItemSelfAlign.None;
        private FbFlexItemSelfAlign _defaultSelfAlign;

        public FbStackItem()
        {
            IsFlex = true;
        }

        protected override void CaptureParameterDefaults()
        {
            base.CaptureParameterDefaults();
            _defaultSelfAlign = _selfAlign;
        }

        protected override void RestoreParameterDefaults()
        {
            base.RestoreParameterDefaults();
            _selfAlign = _defaultSelfAlign;
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

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1)]
        private struct ItemKey
        {
            public SurfaceKey Surface;
            public FbFlexItemSelfAlign SelfAlign;
        }

        private string ComputedStyle
            => ResolveStyle(new ItemKey { Surface = SurfaceStyleKey, SelfAlign = _selfAlign }, SurfaceStyleInputs,
                static component => ((FbStackItem)component).BuildItemStyle());

        private string BuildItemStyle()
            => BuildSurfaceStyle() +
               "flex-wrap: wrap;" +
               (_selfAlign != FbFlexItemSelfAlign.None ? $"align-self:{VerticalAlignString};" : "");
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
        public bool SJHBl { get => _selfAlign == FbFlexItemSelfAlign.Baseline; set { if (value) _selfAlign = FbFlexItemSelfAlign.Baseline; } }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(2, "style", ComputedStyle);
            builder.AddAttribute(3, "class", AggregatedClasses);
            if (OnClick.HasDelegate)
                builder.AddAttribute(4, "onclick", OnClick);
            builder.AddContent(5, ChildContent);
            builder.CloseElement();
        }
    }
}
