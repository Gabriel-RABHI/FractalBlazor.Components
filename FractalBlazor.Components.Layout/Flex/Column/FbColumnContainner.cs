using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout
{
    public class FbColumnContainner : FbFlexBoxBase
    {
        private FbFlexSize _minWithSize = FbFlexSize.None;
        private FbFrame _frame = FbFrame.None;
        private FbFrame _separator = FbFrame.None;
        private FbFrame _grid = FbFrame.None;

        public FbColumnContainner()
        {
            base.Wrap = FbFlexWrap.Wrap;
            AlignItems = FbFlexAlignItems.Stretch;
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "cpnt", $"column-cntr[{StoreId}]");
            builder.AddAttribute(2, "style", $"{Style} {ComputedStyle}");
            builder.AddAttribute(3, "class", $"fb-column-containner {Classes} {ResponsiveClassString}");
            builder.AddContent(4, ChildContent);
            builder.CloseElement();
        }
    }
}
