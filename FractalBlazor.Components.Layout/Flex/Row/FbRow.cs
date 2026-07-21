using System;
using System.Threading.Tasks;
using FractalBlazor.Components.Layout.Flex;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout
{
    public class FbRow : FbFlexBoxBase
    {
        #region HIDDEN
        private FbFrame _separator = FbFrame.None;

        public FbRow()
        {
            Direction = FbFlexDirection.Row;
            SingleLine = true;
            JHSt = true;
            IsFlex = true;
            Flex = 1;
        }

        private string SeparatorClass
        {
            get
            {
                string baseClasse = "framed-separator-row";

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

        protected string ComputedRowClasses
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
            get => Direction == FbFlexDirection.RowReverse;
            set
            {
                if (value)
                    Direction = FbFlexDirection.RowReverse;
                else Direction = FbFlexDirection.Row;
            }
        }

        /// <summary>
        /// Single line setting
        /// </summary>
        [Parameter]
        public bool SingleLine { get => base.Wrap == FbFlexWrap.NoWrap; set { if (value) base.Wrap = FbFlexWrap.NoWrap; } }

        /// <summary>
        /// Multi line setting
        /// </summary>
        [Parameter]
        public bool MultiLine { get => base.Wrap == FbFlexWrap.Wrap; set { if (value) base.Wrap = FbFlexWrap.Wrap; } }

        /// <summary>
        /// Reverse multi line setting
        /// </summary>
        [Parameter]
        public bool ReverseMultiLine { get => base.Wrap == FbFlexWrap.WrapReverse; set { if (value) base.Wrap = FbFlexWrap.WrapReverse; } }

        // -------------- FbFlexJustify ------------ //
        /// <summary>
        /// Justify -> Start
        /// </summary>
        [Parameter]
        public bool JS { get => JHS && JVS; set { if (value) JHS = JVS = true; } }

        /// <summary>
        /// Justify -> End
        /// </summary>
        [Parameter]
        public bool JE { get => JHE && JVE; set { if (value) JHE = JVE = true; } }

        /// <summary>
        /// Justify -> Center
        /// </summary>
        [Parameter]
        public bool JC { get => JHC && JVC; set { if (value) JHC = JVC = true; } }

        /// <summary>
        /// Justify -> Stretch
        /// </summary>
        [Parameter]
        public bool JSt { get => JHSt && JVSt; set { if (value) JHSt = JVSt = true; } }

        // -------------- FbFlexJustify ------------ //
        /// <summary>
        /// Justify -> Horizontal -> Start
        /// </summary>
        [Parameter]
        public bool JHS { get => base.Justify == FbFlexJustify.Start; set { if (value) base.Justify = FbFlexJustify.Start; } }

        /// <summary>
        /// Justify -> Horizontal -> End
        /// </summary>
        [Parameter]
        public bool JHE { get => base.Justify == FbFlexJustify.End; set { if (value) base.Justify = FbFlexJustify.End; } }

        /// <summary>
        /// Justify Horizontal Center
        /// </summary>
        [Parameter]
        public bool JHC { get => base.Justify == FbFlexJustify.Center; set { if (value) base.Justify = FbFlexJustify.Center; } }

        /// <summary>
        /// Justify -> Horizontal -> Stretch
        /// </summary>
        [Parameter]
        public bool JHSt { get => base.Justify == FbFlexJustify.Stretch; set { if (value) base.Justify = FbFlexJustify.Stretch; } }

        /// <summary>
        /// Justify -> Horizontal -> Space Between
        /// </summary>
        [Parameter]
        public bool JHSB { get => base.Justify == FbFlexJustify.SpaceBetween; set { if (value) base.Justify = FbFlexJustify.SpaceBetween; } }

        /// <summary>
        /// Justify -> Horizontal -> Space Around
        /// </summary>
        [Parameter]
        public bool JHSA { get => base.Justify == FbFlexJustify.SpaceAround; set { if (value) base.Justify = FbFlexJustify.SpaceAround; } }

        /// <summary>
        /// Justify -> Horizontal -> Evenly
        /// </summary>
        [Parameter]
        public bool JHEv { get => base.Justify == FbFlexJustify.Evenly; set { if (value) base.Justify = FbFlexJustify.Evenly; } }

        // -------------- FbFlexAlignItems ------------ //
        /// <summary>
        /// Justify -> Vertical -> Start
        /// </summary>
        [Parameter]
        public bool JVS { get => base.AlignItems == FbFlexAlignItems.Start; set { if (value) base.AlignItems = FbFlexAlignItems.Start; } }

        /// <summary>
        /// Justify -> Vertical -> End
        /// </summary>
        [Parameter]
        public bool JVE { get => base.AlignItems == FbFlexAlignItems.End; set { if (value) base.AlignItems = FbFlexAlignItems.End; } }

        /// <summary>
        /// Justify -> Vertical -> Center
        /// </summary>
        [Parameter]
        public bool JVC { get => base.AlignItems == FbFlexAlignItems.Center; set { if (value) base.AlignItems = FbFlexAlignItems.Center; } }

        /// <summary>
        /// Justify -> Vertical -> Baseline
        /// </summary>
        [Parameter]
        public bool JVBl { get => base.AlignItems == FbFlexAlignItems.Baseline; set { if (value) base.AlignItems = FbFlexAlignItems.Baseline; } }

        /// <summary>
        /// Justify -> Vertical -> Stretch
        /// </summary>
        [Parameter]
        public bool JVSt { get => base.AlignItems == FbFlexAlignItems.Stretch; set { if (value) base.AlignItems = FbFlexAlignItems.Stretch; } }

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
            builder.AddAttribute(1, "cpnt", $"row[{StoreId}]");
            builder.AddAttribute(2, "style", AggregatedStyles);
            builder.AddAttribute(3, "class", $"fb-row {ComputedRowClasses}");
            builder.AddAttribute(4, "onclick", EventCallback.Factory.Create(this, () => OnClick.InvokeAsync()));
            builder.AddContent(7, ChildContent);
            builder.CloseElement();
        }
    }
}
