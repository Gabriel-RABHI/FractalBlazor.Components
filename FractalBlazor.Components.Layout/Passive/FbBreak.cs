using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout
{
    public class FbBreak : FbSimpleComponentBase
    {
        /// <summary>
        /// Content to show on smaller screens
        /// </summary>
        [Parameter]
        public RenderFragment SmallContent { get; set; }

        /// <summary>
        /// Content to show on larger screens
        /// </summary>
        [Parameter]
        public RenderFragment LargeContent { get; set; }

        /// <summary>
        /// Is inline element
        /// </summary>
        [Parameter]
        public bool IsInline { get; set; } = false;

        /// <summary>
        /// Responsive breakpoint
        /// </summary>
        [Parameter]
        public FbBreaks Break { get; set; } = FbBreaks.XS_600px;

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
        /// Custom inline CSS style
        /// </summary>
        [Parameter]
        public string Style { get; set; }

        private string ComputedStyle
        {
            get
            {
                return $" {(string.IsNullOrWhiteSpace(Style) ? "" : Style)}";
            }
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            switch (Break)
            {
                case FbBreaks.XXS_480px:
                    RenderBreakContent(builder, "hide-under-xxs", "hide-over-xxs");
                    break;
                case FbBreaks.XS_600px:
                    RenderBreakContent(builder, "hide-under-xs", "hide-over-xs");
                    break;
                case FbBreaks.S_960px:
                    RenderBreakContent(builder, "hide-under-s", "hide-over-s");
                    break;
                case FbBreaks.M_1280px:
                    RenderBreakContent(builder, "hide-under-m", "hide-over-m");
                    break;
                case FbBreaks.L_1600px:
                    RenderBreakContent(builder, "hide-under-l", "hide-over-l");
                    break;
                case FbBreaks.XL_1960px:
                    RenderBreakContent(builder, "hide-under-xl", "hide-over-xl");
                    break;
            }
        }

        private void RenderBreakContent(RenderTreeBuilder builder, string largeClass, string smallClass)
        {
            builder.OpenElement(0, "span");
            builder.AddAttribute(1, "class", largeClass);
            builder.AddContent(2, LargeContent);
            builder.CloseElement();

            builder.OpenElement(3, "span");
            builder.AddAttribute(4, "class", smallClass);
            builder.AddContent(5, SmallContent);
            builder.CloseElement();
        }
    }
}
