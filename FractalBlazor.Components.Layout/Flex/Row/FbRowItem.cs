using System;
using System.Threading.Tasks;
using FractalBlazor.Components.Layout.Abstracts;
using FractalBlazor.Components.Layout.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout
{
    public class FbRowItem : FbLayoutSurfaceComponentBase
    {
        #region HIDDEN
        private FbFlexItemSelfAlign _selfAlign = FbFlexItemSelfAlign.None;
        private FbFlexItemSelfAlign _defaultSelfAlign;

        public FbRowItem()
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

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1)]
        private struct ItemKey
        {
            public SurfaceKey Surface;
            public FbFlexItemSelfAlign SelfAlign;
        }

        private string ItemClasses
            => ResolveClasses(new ItemKey { Surface = SurfaceKeyValue, SelfAlign = _selfAlign },
                static component => ((FbRowItem)component).BuildItemClasses());

        private string BuildItemClasses()
        {
            var line = new FbAtomLine();
            AddSurfaceAtoms(line);
            line.Set(FbAxis.FlexWrap, (int)FbFlexWrap.Wrap);
            line.Set(FbAxis.AlignSelf, (int)_selfAlign);
            return line.Classes(BuildSurfaceClasses(), null);
        }
        #endregion

        // ************************************************************************************************ //
        // ***************************************    PUBLIC   ******************************************** //
        // ************************************************************************************************ //

        /// <summary>
        /// Child content
        /// </summary>
        [Parameter]
        public RenderFragment? ChildContent { get; set; }
        
        /// <summary>
        /// On click event callback
        /// </summary>
        [Parameter]
        public EventCallback OnClick { get; set; }

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
            builder.AddAttribute(2, "style", AggregatedStyles);
            builder.AddAttribute(3, "class", ItemClasses);
            if (OnClick.HasDelegate)
                builder.AddAttribute(4, "onclick", OnClick);
            builder.AddContent(5, ChildContent);
            builder.CloseElement();
        }
    }
}
