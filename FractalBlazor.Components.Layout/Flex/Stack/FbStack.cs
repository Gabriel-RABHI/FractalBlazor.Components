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

        protected FbFrame Frame { get => _frame; set => _frame = value; }

        protected FbFrame Separator { get => _separator; set => _separator = value; }

        protected FbBackground Background { get => _background; set => _background = value; }

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

                if (_frame is FbFrame.Small)
                    return $"{baseClasse} {baseClasse}-small-frame";
                else if (_frame is FbFrame.Medium)
                    return $"{baseClasse} {baseClasse}-medium-frame";
                else if (_frame is FbFrame.Large)
                    return $"{baseClasse} {baseClasse}-large-frame";
                else
                    return "";
            }
        }

        private string SeparatorClass
        {
            get
            {
                string baseClasse = "framed-separator-stack";

                if (_separator is FbFrame.Small)
                    return $"{baseClasse} {baseClasse}-small-frame";
                else if (_separator is FbFrame.Medium)
                    return $"{baseClasse} {baseClasse}-medium-frame";
                else if (_separator is FbFrame.Large)
                    return $"{baseClasse} {baseClasse}-large-frame";
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
                    case FbBackground.Default:
                        return "fb-default-background";
                    case FbBackground.Accent:
                        return "fb-accent-background";
                    case FbBackground.Highlight:
                        return "fb-highlight-background";
                }
                return "";
            }
        }

        protected string ComputedStackClasses
        {
            get
            {
                return $"{Classes} {ComputedFrameClasses} {BackgroundClasses} {WrapClassString} {ResponsiveClassString}";
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
        /// With -> Frame -> Small
        /// </summary>
        [Parameter]
        public bool WFrameS { get => Frame == FbFrame.Small; set => Frame = FbFrame.Small; }

        /// <summary>
        /// With -> Frame -> Medium
        /// </summary>
        [Parameter]
        public bool WFrameM { get => Frame == FbFrame.Medium; set => Frame = FbFrame.Medium; }

        /// <summary>
        /// With -> Frame -> Large
        /// </summary>
        [Parameter]
        public bool WFrameL { get => Frame == FbFrame.Large; set => Frame = FbFrame.Large; }

        /// <summary>
        /// With -> Separator -> Small
        /// </summary>
        [Parameter]
        public bool WSeparatorS { get => Separator == FbFrame.Small; set => Separator = FbFrame.Small; }

        /// <summary>
        /// With -> Separator -> Medium
        /// </summary>
        [Parameter]
        public bool WSeparatorM { get => Separator == FbFrame.Medium; set => Separator = FbFrame.Medium; }

        /// <summary>
        /// With -> Separator -> Large
        /// </summary>
        [Parameter]
        public bool WSeparatorL { get => Separator == FbFrame.Large; set => Separator = FbFrame.Large; }

        /// <summary>
        /// With -> Grid -> Small
        /// </summary>
        [Parameter]
        public bool WGridS { get => WFrameS && WSeparatorS; set => WFrameS = WSeparatorS = true; }

        /// <summary>
        /// With -> Grid -> Medium
        /// </summary>
        [Parameter]
        public bool WGridM { get => WFrameM && WSeparatorM; set => WFrameM = WSeparatorM = true; }

        /// <summary>
        /// With -> Grid -> Large
        /// </summary>
        [Parameter]
        public bool WGridL { get => WFrameL && WSeparatorL; set => WFrameL = WSeparatorL = true; }

        /// <summary>
        /// With -> Radius -> Small
        /// </summary>
        [Parameter]
        public bool WRS { get => Radius == FbPresets.S_Radius; set => Radius = FbPresets.S_Radius; }
        
        /// <summary>
        /// With -> Radius -> Medium
        /// </summary>
        [Parameter]
        public bool WRM { get => Radius == FbPresets.M_Radius; set => Radius = FbPresets.M_Radius; }
        
        /// <summary>
        /// With -> Radius -> Large
        /// </summary>
        [Parameter]
        public bool WRL { get => Radius == FbPresets.L_Radius; set => Radius = FbPresets.L_Radius; }
        
        /// <summary>
        /// With -> Radius -> Extra Large
        /// </summary>
        [Parameter]
        public bool WRX { get => Radius == FbPresets.X_Radius; set => Radius = FbPresets.X_Radius; }

        /// <summary>
        /// With -> Background -> Default
        /// </summary>
        [Parameter]
        public bool WDefaultBackground { get => _background == FbBackground.Default; set => _background = FbBackground.Default; }

        /// <summary>
        /// With -> Background -> Accent
        /// </summary>
        [Parameter]
        public bool WAccentBackground { get => _background == FbBackground.Accent; set => _background = FbBackground.Accent; }

        /// <summary>
        /// With -> Background -> Highlight
        /// </summary>
        [Parameter]
        public bool WHighlightBackground { get => _background == FbBackground.Highlight; set => _background = FbBackground.Highlight; }

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
