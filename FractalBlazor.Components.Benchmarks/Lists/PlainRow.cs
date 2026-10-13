using FractalBlazor.Components.Layout.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Benchmarks.Lists
{
    /// <summary>
    /// What FbRow would be as an ordinary ComponentBase: same flags, same markup, classes
    /// built at each render. It isolates the cost of the component model.
    /// </summary>
    public sealed class PlainRow : ComponentBase
    {
        private static readonly string PaddingL = " " + FbAtoms.NameOf("padding:var(--fb-space-l)");
        private static readonly string RadiusM = " " + FbAtoms.NameOf("border-radius:var(--fb-radius-m)");
        private static readonly string GapM = " " + FbAtoms.NameOf("gap:var(--fb-space-m)");

        [Parameter] public bool PL { get; set; }
        [Parameter] public bool GM { get; set; }
        [Parameter] public bool WBA { get; set; }
        [Parameter] public bool WFM { get; set; }
        [Parameter] public bool WRM { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class",
                "fb-row" + (WBA ? " fb-wba" : "") + (WFM ? " fb-wfm" : "") +
                (PL ? PaddingL : "") + (WRM ? RadiusM : "") + (GM ? GapM : ""));
            builder.AddContent(2, ChildContent);
            builder.CloseElement();
        }
    }

    public sealed class PlainText : ComponentBase
    {
        [Parameter] public bool S { get; set; }
        [Parameter] public bool Muted { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "span");
            builder.AddAttribute(1, "class", "fb-txt" + (S ? " fb-t-s" : "") + (Muted ? " fb-muted" : ""));
            builder.AddContent(2, ChildContent);
            builder.CloseElement();
        }
    }
}
