using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout
{
    public class FbStack : FbFlexBoxBase
    {
        #region HIDDEN
        private FbFrame _separator = FbFrame.None;

        public FbStack()
        {
            Direction = FbFlexDirection.Column;
            JHSt = true;
            IsFlex = true;
            Flex = 1;
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

        protected string AggregatedClasses
        {
            get
            {
                return $"{base.AggregatedClasses} {SeparatorClass}";
            }
        }
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
        /// Justify (both vertical and horizontal) -> Start
        /// </summary>
        [Parameter]
        public bool JS { get => JHS && JVS; set { if (value) JHS = JVS = true; } }

        /// <summary>
        /// Justify (both vertical and horizontal) -> End
        /// </summary>
        [Parameter]
        public bool JE { get => JHE && JVE; set { if (value) JHE = JVE = true; } }

        /// <summary>
        /// Justify (both vertical and horizontal) -> Center
        /// </summary>
        [Parameter]
        public bool JC { get => JHC && JVC; set { if (value) JHC = JVC = true; } }

        /// <summary>
        /// Justify (both vertical and horizontal) -> Stretch
        /// </summary>
        [Parameter]
        public bool JSt { get => JHSt && JVSt; set { if (value) JHSt = JVSt = true; } }

        // -------------- FbFlexJustify ------------ //
        /// <summary>
        /// Justify -> Vertical -> Start
        /// </summary>
        [Parameter]
        public bool JVS { get => base.Justify == FbFlexJustify.Start; set { if (value) base.Justify = FbFlexJustify.Start; } }

        /// <summary>
        /// Justify -> Vertical -> End
        /// </summary>
        [Parameter]
        public bool JVE { get => base.Justify == FbFlexJustify.End; set { if (value) base.Justify = FbFlexJustify.End; } }

        /// <summary>
        /// Justify -> Vertical -> Center
        /// </summary>
        [Parameter]
        public bool JVC { get => base.Justify == FbFlexJustify.Center; set { if (value) base.Justify = FbFlexJustify.Center; } }
        
        /// <summary>
        /// Justify -> Vertical -> Stretch
        /// </summary>
        [Parameter]
        public bool JVSt { get => base.Justify == FbFlexJustify.Stretch; set { if (value) base.Justify = FbFlexJustify.Stretch; } }

        /// <summary>
        /// Justify -> Vertical -> Space Between
        /// </summary>
        [Parameter]
        public bool JVSB { get => base.Justify == FbFlexJustify.SpaceBetween; set { if (value) base.Justify = FbFlexJustify.SpaceBetween; } }

        /// <summary>
        /// Justify -> Vertical -> Space Around
        /// </summary>
        [Parameter]
        public bool JVSA { get => base.Justify == FbFlexJustify.SpaceAround; set { if (value) base.Justify = FbFlexJustify.SpaceAround; } }

        /// <summary>
        /// Justify -> Vertical -> Evenly
        /// </summary>
        [Parameter]
        public bool JVEv { get => base.Justify == FbFlexJustify.Evenly; set { if (value) base.Justify = FbFlexJustify.Evenly; } }

        // -------------- FbFlexAlignItems ------------ //
        /// <summary>
        /// Justify -> Horizontal -> Start
        /// </summary>
        [Parameter]
        public bool JHS { get => base.AlignItems == FbFlexAlignItems.Start; set { if (value) base.AlignItems = FbFlexAlignItems.Start; } }

        /// <summary>
        /// Justify -> Horizontal -> End
        /// </summary>
        [Parameter]
        public bool JHE { get => base.AlignItems == FbFlexAlignItems.End; set { if (value) base.AlignItems = FbFlexAlignItems.End; } }

        /// <summary>
        /// Justify -> Horizontal -> Center
        /// </summary>
        [Parameter]
        public bool JHC { get => base.AlignItems == FbFlexAlignItems.Center; set { if (value) base.AlignItems = FbFlexAlignItems.Center; } }

        /// <summary>
        /// Justify -> Horizontal -> Stretch
        /// </summary>
        [Parameter]
        public bool JHSt { get => base.AlignItems == FbFlexAlignItems.Stretch; set { if (value) base.AlignItems = FbFlexAlignItems.Stretch; } }

        // -------------- FbRow Frame ------------ //
        /// <summary>
        /// With -> Separator -> Light
        /// </summary>
        [Parameter]
        public bool WSL { get => _separator == FbFrame.Light; set { if (value) _separator = FbFrame.Light; } }

        /// <summary>
        /// With -> Separator -> Medium
        /// </summary>
        [Parameter]
        public bool WSM { get => _separator == FbFrame.Medium; set { if (value) _separator = FbFrame.Medium; } }

        /// <summary>
        /// With -> Separator -> Strong
        /// </summary>
        [Parameter]
        public bool WSS { get => _separator == FbFrame.Strong; set { if (value) _separator = FbFrame.Strong; } }

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

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "cpnt", $"stack[{StoreId}]");
            builder.AddAttribute(2, "style", AggregatedStyles);
            builder.AddAttribute(3, "class", $"fb-stack {AggregatedClasses}");
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
