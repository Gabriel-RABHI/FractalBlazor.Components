using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout
{
    public class FbBreak : FbComponentBase
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
        public FbBreaks Break { get; set; } = FbBreaks.XS;

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
                case FbBreaks.XS:
                    RenderBreakContent(builder, "hide-under-xs", "hide-over-xs");
                    break;
                case FbBreaks.S:
                    RenderBreakContent(builder, "hide-under-s", "hide-over-s");
                    break;
                case FbBreaks.M:
                    RenderBreakContent(builder, "hide-under-m", "hide-over-m");
                    break;
                case FbBreaks.L:
                    RenderBreakContent(builder, "hide-under-l", "hide-over-l");
                    break;
                case FbBreaks.XL:
                    RenderBreakContent(builder, "hide-under-xl", "hide-over-xl");
                    break;
                case FbBreaks.XXL:
                    RenderBreakContent(builder, "hide-under-xxl", "hide-over-xxl");
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
