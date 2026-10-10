using FractalBlazor.Components.Layout;

namespace FractalBlazor.Components.Tests.Layout;

public sealed class RenderingTests : BunitContext
{
    [Fact]
    public void Row_without_click_handler_registers_no_event()
    {
        var cut = Render<FbRow>();

        Assert.DoesNotContain("onclick", cut.Markup);
    }

    [Fact]
    public void Row_with_click_handler_invokes_it()
    {
        var clicks = 0;
        var cut = Render<FbRow>(p => p.Add(x => x.OnClick, () => clicks++));

        cut.Find("div").Click();

        Assert.Equal(1, clicks);
    }

    [Fact]
    public void Components_render_no_debug_attribute()
    {
        var cut = Render<FbStack>(p => p.AddChildContent<FbStackItem>());

        Assert.DoesNotContain("cpnt", cut.Markup);
    }

    [Fact]
    public void Item_classes_are_not_duplicated()
    {
        var cut = Render<FbRowItem>(p => p.Add(x => x.Classes, "custom"));

        Assert.Single(cut.Find("div").ClassList, c => c == "custom");
    }
}
