using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout
{
    public class FbStack : FbFlexBoxBase
    {
        #region HIDDEN
        private FbFrame _frame = FbFrame.None;
        private FbFrame _separator = FbFrame.None;
        private FbBackground _background = FbBackground.None;

        [Parameter]
        public FbFrame Frame { get => _frame; set => _frame = value; }

        [Parameter]
        public FbFrame Separator { get => _separator; set => _separator = value; }

        [Parameter]
        public FbBackground Background { get => _background; set => _background = value; }

        public FbStack()
        {
            Direction = FbFlexDirection.Column;
            JHSt = true;
            IsFlex = true;
            Flex = 1;
        }

        private string FrameClass
        {
            get
            {
                string baseClasse = "framed-stack";

                if (_frame is FbFrame.Light)
                    return $"{baseClasse} {baseClasse}-light-frame";
                else if (_frame is FbFrame.Medium)
                    return $"{baseClasse} {baseClasse}-medium-frame";
                else if (_frame is FbFrame.Strong)
                    return $"{baseClasse} {baseClasse}-strong-frame";
                else
                    return "";
            }
        }

        private string SeparatorClass
        {
            get
            {
                string baseClasse = "framed-separator-stack";

                if (_separator is FbFrame.Light)
                    return $"{baseClasse} {baseClasse}-light-frame";
                else if (_separator is FbFrame.Medium)
                    return $"{baseClasse} {baseClasse}-medium-frame";
                else if (_separator is FbFrame.Strong)
                    return $"{baseClasse} {baseClasse}-strong-frame";
                else
                    return "";
            }
        }

        private string ComputedFrameClasses
        {
            get
            {
                if (FrameClass != "" || SeparatorClass != "")
                    return $"{FrameClass} {SeparatorClass}";
                return "";
            }
        }

        private string BackgroundClasses
        {
            get
            {
                switch (_background)
                {
                    case FbBackground.Surface:
                        return "fb-bg-surface";
                    case FbBackground.Accent:
                        return "fb-bg-accent";
                    case FbBackground.Highlight:
                        return "fb-bg-highlight";
                }
                return "";
            }
        }

        protected string ComputedStackClasses
        {
            get
            {
                return $"{Classes} {ComputedFrameClasses} {BackgroundClasses} {HoverClassString} {WrapClassString} {ResponsiveClassString}";
            }
        }

        // -------------- FbFlexAlignContent ------------ //
        protected bool JustifyColumnsLeft { get => base.AlignContent == FbFlexAlignContent.Start; set => base.AlignContent = FbFlexAlignContent.Start; }

        protected bool JustifyColumnsRight { get => base.AlignContent == FbFlexAlignContent.End; set => base.AlignContent = FbFlexAlignContent.End; }

        protected bool JustifyColumnsCenter { get => base.AlignContent == FbFlexAlignContent.Center; set => base.AlignContent = FbFlexAlignContent.Center; }

        protected bool JustifyColumnsSpaceBetwen { get => base.AlignContent == FbFlexAlignContent.SpaceBetween; set => base.AlignContent = FbFlexAlignContent.SpaceBetween; }

        protected bool JustifyColumnsSpaceAround { get => base.AlignContent == FbFlexAlignContent.SpaceAround; set => base.AlignContent = FbFlexAlignContent.SpaceAround; }

        protected bool JustifyColumnsStretched { get => base.AlignContent == FbFlexAlignContent.Stretch; set => base.AlignContent = FbFlexAlignContent.Stretch; }
        #endregion

        // ************************************************************************************************ //
        // ***************************************    PUBLIC   ******************************************** //
        // ************************************************************************************************ //

        /// <summary>
        /// On click event callback
        /// </summary>
        [Parameter]
        public EventCallback OnClick { get; set; }

        // -------- MISSING --------- //
        /// <summary>
        /// Minimum width
        /// </summary>
        [Parameter]
        public new string MinWidth { get => base.MinWidth; set => base.MinWidth = value; }

        /// <summary>
        /// Minimum height
        /// </summary>
        [Parameter]
        public new string MinHeight { get => base.MinHeight; set => base.MinHeight = value; }

        /// <summary>
        /// Maximum height
        /// </summary>
        [Parameter]
        public new string MaxHeight { get => base.MaxHeight; set => base.MaxHeight = value; }
        
        // -------- MISSING --------- //

        /// <summary>
        /// Reverse direction
        /// </summary>
        [Parameter]
        public bool Reverse
        {
            get => Direction == FbFlexDirection.ColumnReverse;
            set
            {
                if (value)
                    Direction = FbFlexDirection.ColumnReverse;
                else Direction = FbFlexDirection.Column;
            }
        }

        // -------------- FbFlexAlignItems ------------ //
        /// <summary>
        /// Justify -> Start
        /// </summary>
        [Parameter]
        public bool JS { get => JHS && JVS; set => JHS = JVS = true; }

        /// <summary>
        /// Justify -> End
        /// </summary>
        [Parameter]
        public bool JE { get => JHE && JVE; set => JHE = JVE = true; }

        /// <summary>
        /// Justify -> Center
        /// </summary>
        [Parameter]
        public bool JC { get => JHC && JVC; set => JHC = JVC = true; }

        /// <summary>
        /// Justify -> Stretch
        /// </summary>
        [Parameter]
        public bool JSt { get => JHSt && JVSt; set => JHSt = JVSt = true; }

        // -------------- FbFlexJustify ------------ //
        /// <summary>
        /// Justify -> Vertical -> Start
        /// </summary>
        [Parameter]
        public bool JVS { get => base.Justify == FbFlexJustify.Start; set => base.Justify = FbFlexJustify.Start; }

        /// <summary>
        /// Justify -> Vertical -> End
        /// </summary>
        [Parameter]
        public bool JVE { get => base.Justify == FbFlexJustify.End; set => base.Justify = FbFlexJustify.End; }

        /// <summary>
        /// Justify -> Vertical -> Center
        /// </summary>
        [Parameter]
        public bool JVC { get => base.Justify == FbFlexJustify.Center; set => base.Justify = FbFlexJustify.Center; }
        
        /// <summary>
        /// Justify -> Vertical -> Stretch
        /// </summary>
        [Parameter]
        public bool JVSt { get => base.Justify == FbFlexJustify.Stretch; set => base.Justify = FbFlexJustify.Stretch; }

        /// <summary>
        /// Justify -> Vertical -> Space Between
        /// </summary>
        [Parameter]
        public bool JVSB { get => base.Justify == FbFlexJustify.SpaceBetween; set => base.Justify = FbFlexJustify.SpaceBetween; }

        /// <summary>
        /// Justify -> Vertical -> Space Around
        /// </summary>
        [Parameter]
        public bool JVSA { get => base.Justify == FbFlexJustify.SpaceAround; set => base.Justify = FbFlexJustify.SpaceAround; }

        /// <summary>
        /// Justify -> Vertical -> Evenly
        /// </summary>
        [Parameter]
        public bool JVEv { get => base.Justify == FbFlexJustify.Evenly; set => base.Justify = FbFlexJustify.Evenly; }

        // -------------- FbFlexAlignItems ------------ //
        /// <summary>
        /// Justify -> Horizontal -> Start
        /// </summary>
        [Parameter]
        public bool JHS { get => base.AlignItems == FbFlexAlignItems.Start; set => base.AlignItems = FbFlexAlignItems.Start; }

        /// <summary>
        /// Justify -> Horizontal -> End
        /// </summary>
        [Parameter]
        public bool JHE { get => base.AlignItems == FbFlexAlignItems.End; set => base.AlignItems = FbFlexAlignItems.End; }

        /// <summary>
        /// Justify -> Horizontal -> Center
        /// </summary>
        [Parameter]
        public bool JHC { get => base.AlignItems == FbFlexAlignItems.Center; set => base.AlignItems = FbFlexAlignItems.Center; }

        /// <summary>
        /// Justify -> Horizontal -> Stretch
        /// </summary>
        [Parameter]
        public bool JHSt { get => base.AlignItems == FbFlexAlignItems.Stretch; set => base.AlignItems = FbFlexAlignItems.Stretch; }

        // -------------- FbRow Frame ------------ //
        /// <summary>
        /// With -> Frame -> Light
        /// </summary>
        [Parameter]
        public bool WFL { get => Frame == FbFrame.Light; set { if (value) Frame = FbFrame.Light; } }

        /// <summary>
        /// With -> Frame -> Medium
        /// </summary>
        [Parameter]
        public bool WFM { get => Frame == FbFrame.Medium; set { if (value) Frame = FbFrame.Medium; } }

        /// <summary>
        /// With -> Frame -> Strong
        /// </summary>
        [Parameter]
        public bool WFS { get => Frame == FbFrame.Strong; set { if (value) Frame = FbFrame.Strong; } }

        /// <summary>
        /// With -> Separator -> Light
        /// </summary>
        [Parameter]
        public bool WSL { get => Separator == FbFrame.Light; set { if (value) Separator = FbFrame.Light; } }

        /// <summary>
        /// With -> Separator -> Medium
        /// </summary>
        [Parameter]
        public bool WSM { get => Separator == FbFrame.Medium; set { if (value) Separator = FbFrame.Medium; } }

        /// <summary>
        /// With -> Separator -> Strong
        /// </summary>
        [Parameter]
        public bool WSS { get => Separator == FbFrame.Strong; set { if (value) Separator = FbFrame.Strong; } }

        /// <summary>
        /// With -> Grid -> Light
        /// </summary>
        [Parameter]
        public bool WGL { get => WFL && WSL; set { if (value) WFL = WSL = true; } }

        /// <summary>
        /// With -> Grid -> Medium
        /// </summary>
        [Parameter]
        public bool WGM { get => WFM && WSM; set { if (value) WFM = WSM = true; } }

        /// <summary>
        /// With -> Grid -> Strong
        /// </summary>
        [Parameter]
        public bool WGS { get => WFS && WSS; set { if (value) WFS = WSS = true; } }

        /// <summary>
        /// With -> Radius -> Small
        /// </summary>
        [Parameter]
        public bool WRS { get => Radius == FbLayoutPresets.RS; set { if (value) Radius = FbLayoutPresets.RS; } }
        
        /// <summary>
        /// With -> Radius -> Medium
        /// </summary>
        [Parameter]
        public bool WRM { get => Radius == FbLayoutPresets.RM; set { if (value) Radius = FbLayoutPresets.RM; } }
        
        /// <summary>
        /// With -> Radius -> Large
        /// </summary>
        [Parameter]
        public bool WRL { get => Radius == FbLayoutPresets.RL; set { if (value) Radius = FbLayoutPresets.RL; } }
        
        /// <summary>
        /// With -> Radius -> Extra Large
        /// </summary>
        [Parameter]
        public bool WRX { get => Radius == FbLayoutPresets.RX; set { if (value) Radius = FbLayoutPresets.RX; } }

        /// <summary>
        /// With -> Background -> Surface
        /// </summary>
        [Parameter]
        public bool WBS { get => _background == FbBackground.Surface; set { if (value) _background = FbBackground.Surface; } }

        /// <summary>
        /// With -> Background -> Accent
        /// </summary>
        [Parameter]
        public bool WBA { get => _background == FbBackground.Accent; set { if (value) _background = FbBackground.Accent; } }

        /// <summary>
        /// With -> Background -> Highlight
        /// </summary>
        [Parameter]
        public bool WBH { get => _background == FbBackground.Highlight; set { if (value) _background = FbBackground.Highlight; } }

        [Parameter]
        [Obsolete("Use WBS instead.")]
        public bool L0 { get => WBS; set => WBS = value; }

        [Parameter]
        [Obsolete("Use WBA instead.")]
        public bool L1 { get => WBA; set => WBA = value; }

        [Parameter]
        [Obsolete("Use WBH instead.")]
        public bool L2 { get => WBH; set => WBH = value; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "cpnt", $"stack[{StoreId}]");
            builder.AddAttribute(2, "style", ComputedStyle);
            builder.AddAttribute(3, "class", $"fb-stack {ComputedStackClasses}");
            builder.AddAttribute(4, "onclick", EventCallback.Factory.Create(this, () => OnClick.InvokeAsync()));

            switch (MasterSlaveStatus)
            {
                case FbFlexMasterSlave.Master:
                    builder.AddAttribute(5, "data-fb-width-master-id", WidthMasterId);
                    break;
                case FbFlexMasterSlave.Slave:
                    builder.AddAttribute(6, "data-fb-width-slave-id", WidthSlaveId);
                    break;
            }

            builder.AddContent(7, ChildContent);
            builder.CloseElement();
        }
    }
}
