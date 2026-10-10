using System;
using System.Threading.Tasks;
using FractalBlazor.Components.Layout.Abstracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout
{
    public class FbRowBreak : FbComponentBase
    {
        #region HIDDEN
        private string ClassName
        {
            get
            {
                switch (Break)
                {
                    case FbBreaks.XS:
                        return Under ? "fb-row-break-under-xs" : "fb-row-break-over-xs";
                    case FbBreaks.S:
                        return Under ? "fb-row-break-under-s" : "fb-row-break-over-s";
                    case FbBreaks.M:
                        return Under ? "fb-row-break-under-m" : "fb-row-break-over-m";
                    case FbBreaks.L:
                        return Under ? "fb-row-break-under-l" : "fb-row-break-over-l";
                    case FbBreaks.XL:
                        return Under ? "fb-row-break-under-xl" : "fb-row-break-over-xl";
                    case FbBreaks.XXL:
                        return Under ? "fb-row-break-under-xxl" : "fb-row-break-over-xxl";
                }
                return "fb-row-break-under-s";
            }
        }
        #endregion

        // ************************************************************************************************ //
        // ***************************************    PUBLIC   ******************************************** //
        // ************************************************************************************************ //
        /// <summary>
        /// Responsive breakpoint
        /// </summary>
        [Parameter]
        public FbBreaks Break { get; set; } = FbBreaks.XL;

        private (FbBreaks Break, bool Under) _defaults;

        protected override void CaptureParameterDefaults()
        {
            base.CaptureParameterDefaults();
            _defaults = (Break, Under);
        }

        protected override void RestoreParameterDefaults()
        {
            base.RestoreParameterDefaults();
            (Break, Under) = _defaults;
        }

        /// <summary>
        /// Break -> Extra Small (512px)
        /// </summary>
        [Parameter]
        public bool XS { get => Break == FbBreaks.XS; set { if (value) Break = FbBreaks.XS; } }

        /// <summary>
        /// Break -> Small (640px)
        /// </summary>
        [Parameter]
        public bool S { get => Break == FbBreaks.S; set { if (value) Break = FbBreaks.S; } }

        /// <summary>
        /// Break -> Medium (768px)
        /// </summary>
        [Parameter]
        public bool M { get => Break == FbBreaks.M; set { if (value) Break = FbBreaks.M; } }

        /// <summary>
        /// Break -> Large (1024px)
        /// </summary>
        [Parameter]
        public bool L { get => Break == FbBreaks.L; set { if (value) Break = FbBreaks.L; } }

        /// <summary>
        /// Break -> Extra Large (1280px)
        /// </summary>
        [Parameter]
        public bool XL { get => Break == FbBreaks.XL; set { if (value) Break = FbBreaks.XL; } }

        /// <summary>
        /// Break -> Extra Extra Large (1536px)
        /// </summary>
        [Parameter]
        public bool XXL { get => Break == FbBreaks.XXL; set { if (value) Break = FbBreaks.XXL; } }

        /// <summary>
        /// Under breakpoint setting
        /// </summary>
        [Parameter]
        public bool Under { get; set; } = true;

        /// <summary>
        /// Over breakpoint setting
        /// </summary>
        [Parameter]
        public bool Over { get => !Under; set => Under = !value; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(2, "style", "width:100%; height:0px;");
            builder.AddAttribute(3, "class", ClassName);
            builder.CloseElement();
        }
    }
}
