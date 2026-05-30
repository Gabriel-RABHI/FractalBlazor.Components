using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout
{
    public class FbRowBreak : FbSimpleComponentBase
    {
        #region HIDDEN
        private string ClassName
        {
            get
            {
                switch (Break)
                {
                    case FbBreaks.XXS_480px:
                        return Under ? "row-break-under-xxs" : "row-break-over-xxs";
                    case FbBreaks.XS_600px:
                        return Under ? "row-break-under-xs" : "row-break-over-xs";
                    case FbBreaks.S_960px:
                        return Under ? "row-break-under-s" : "row-break-over-s";
                    case FbBreaks.M_1280px:
                        return Under ? "row-break-under-m" : "row-break-over-m";
                    case FbBreaks.L_1600px:
                        return Under ? "row-break-under-l" : "row-break-over-l";
                    case FbBreaks.XL_1960px:
                        return Under ? "row-break-under-xl" : "row-break-over-xl";
                }
                return "break-row-under-s";
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
        public FbBreaks Break { get; set; } = FbBreaks.S_960px;

        /// <summary>
        /// Break -> Extra Extra Small (480px)
        /// </summary>
        [Parameter]
        public bool XXS { get => Break == FbBreaks.XXS_480px; set => Break = FbBreaks.XXS_480px; }

        /// <summary>
        /// Break -> Extra Small (600px)
        /// </summary>
        [Parameter]
        public bool XS { get => Break == FbBreaks.XS_600px; set => Break = FbBreaks.XS_600px; }

        /// <summary>
        /// Break -> Small (960px)
        /// </summary>
        [Parameter]
        public bool S { get => Break == FbBreaks.S_960px; set => Break = FbBreaks.S_960px; }

        /// <summary>
        /// Break -> Medium (1280px)
        /// </summary>
        [Parameter]
        public bool M { get => Break == FbBreaks.M_1280px; set => Break = FbBreaks.M_1280px; }

        /// <summary>
        /// Break -> Large (1600px)
        /// </summary>
        [Parameter]
        public bool L { get => Break == FbBreaks.L_1600px; set => Break = FbBreaks.L_1600px; }

        /// <summary>
        /// Break -> Extra Large (1960px)
        /// </summary>
        [Parameter]
        public bool XL { get => Break == FbBreaks.XL_1960px; set => Break = FbBreaks.XL_1960px; }

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
            builder.AddAttribute(1, "cpnt", "row-break");
            builder.AddAttribute(2, "style", "width:100%; height:0px;");
            builder.AddAttribute(3, "class", ClassName);
            builder.CloseElement();
        }
    }
}
