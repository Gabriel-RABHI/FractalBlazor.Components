using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout
{
    public class FbColumnBreak : FbSimpleComponentBase
    {
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "span");
            builder.AddAttribute(1, "cpnt", "column-break");
            builder.AddAttribute(2, "style", "width: 100%; height:0px");
            builder.CloseElement();
        }
    }
}
