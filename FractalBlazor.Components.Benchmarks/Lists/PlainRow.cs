using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Benchmarks.Lists
{
    /// <summary>
    /// What FbRow would be as an ordinary ComponentBase: same flags, same markup, classes and
    /// styles built at each render. It isolates the cost of the component model.
    /// </summary>
    public sealed class PlainRow : ComponentBase
    {
        [Parameter] public bool PL { get; set; }
        [Parameter] public bool GM { get; set; }
        [Parameter] public bool WBA { get; set; }
        [Parameter] public bool WFM { get; set; }
        [Parameter] public bool WRM { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "style",
                "display:flex;flex-direction:row;justify-content:stretch;align-items:center;flex:1;" +
                (PL ? "padding:var(--fb-space-l);" : "") +
                (GM ? "gap:var(--fb-space-m);" : "") +
                (WRM ? "border-radius:var(--fb-radius-m);" : ""));
            builder.AddAttribute(2, "class",
                "fb-row" + (WBA ? " fb-bg-accent" : "") + (WFM ? " fb-framed fb-framed-medium" : ""));
            builder.AddContent(3, ChildContent);
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
            builder.AddAttribute(1, "class", "fb-txt" + (S ? " fb-t-s" : "") + (Muted ? " fb-fg-muted" : ""));
            builder.AddContent(2, ChildContent);
            builder.CloseElement();
        }
    }
}
