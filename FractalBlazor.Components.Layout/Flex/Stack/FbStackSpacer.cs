using System;
using System.Threading.Tasks;
using FractalBlazor.Components.Layout.Abstracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout
{
    /// <summary>
    /// Takes the free space of its stack: the children after it go to the bottom. It is not a
    /// child to separate (class <c>fb-spacer</c>).
    /// </summary>
    public class FbStackSpacer : FbComponentBase
    {
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "span");
            builder.AddAttribute(1, "class", "fb-spacer");
            builder.AddAttribute(2, "style", "flex-grow: 1 !important;");
            builder.CloseElement();
        }
    }
}
