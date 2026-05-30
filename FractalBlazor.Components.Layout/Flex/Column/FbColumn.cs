using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout
{
    public class FbColumn : FbFlexBoxBase
    {
        public FbColumn()
        {
            ColumnDisplay = true;
            AlignItems = FbFlexAlignItems.Stretch;
            Flex = 1;
        }

        protected string ComputedColumnClasses
        {
            get
            {
                return $"{Classes} {WrapClassString} {ResponsiveClassString}";
            }
        }

        // ************************************************************************************************ //
        // ***************************************    PUBLIC   ******************************************** //
        // ************************************************************************************************ //

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

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "cpnt", $"column[{StoreId}]");
            builder.AddAttribute(2, "style", ComputedStyle);
            builder.AddAttribute(3, "class", $"fb-column {ComputedColumnClasses}");
            builder.AddContent(4, ChildContent);
            builder.CloseElement();
        }
    }
}
