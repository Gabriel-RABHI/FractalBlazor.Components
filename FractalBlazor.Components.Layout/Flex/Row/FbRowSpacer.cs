using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout
{
    public class FbRowSpacer : FbComponentBase
    {
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "span");
            builder.AddAttribute(1, "cpnt", "row-separator");
            builder.AddAttribute(2, "style", "flex-grow: 1 !important;");
            builder.CloseElement();
        }
    }
}
