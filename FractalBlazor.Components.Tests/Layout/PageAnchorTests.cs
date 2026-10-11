using FractalBlazor.Components.Layout;

namespace FractalBlazor.Components.Tests.Layout;

public sealed class PageAnchorTests : BunitContext
{
    [Fact]
    public void Anchor_wraps_the_application_layout()
    {
        var cut = Render<FbPageAnchor>(p => p.AddChildContent<FbStack>());

        var anchor = cut.Find("div.fb-page-anchor");
        Assert.Single(anchor.Children, child => child.ClassList.Contains("fb-stack"));
    }

    [Fact]
    public void Anchor_accepts_surface_parameters()
    {
        var cut = Render<FbPageAnchor>(p => p.Add(x => x.WBS, true).Add(x => x.PM, true));

        var anchor = cut.Find("div");
        Assert.Contains("fb-page-anchor", anchor.ClassList);
        Assert.Contains("fb-bg-surface", anchor.ClassList);
        Assert.Contains("padding:", anchor.GetAttribute("style"));
    }

    [Fact]
    public void Scrollable_stack_and_row_get_the_scrollable_class()
    {
        var stack = Render<FbStack>(p => p.Add(x => x.Scrollable, true)).Find("div");
        var row = Render<FbRow>(p => p.Add(x => x.Scrollable, true)).Find("div");

        Assert.Contains("fb-scrollable", stack.ClassList);
        Assert.Contains("fb-scrollable", row.ClassList);
        Assert.DoesNotContain("overflow", stack.GetAttribute("style"));
    }

    [Fact]
    public void Stacks_do_not_scroll_by_default()
    {
        var cut = Render<FbStack>();

        Assert.DoesNotContain("fb-scrollable", cut.Find("div").ClassList);
    }

    [Fact]
    public void Scrollable_class_follows_the_parameter()
    {
        var cut = Render<FbStack>(p => p.Add(x => x.Scrollable, true).Add(x => x.GM, true));

        cut.Render(p => p.Add(x => x.Scrollable, false).Add(x => x.GM, true));

        Assert.DoesNotContain("fb-scrollable", cut.Find("div").ClassList);
    }
}
