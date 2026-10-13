using FractalBlazor.Components.Layout;

namespace FractalBlazor.Components.Tests.Layout;

public sealed class DrawerTests : BunitContext
{
    public DrawerTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private IRenderedComponent<FbDrawer> RenderDrawer(bool open, Action<ComponentParameterCollectionBuilder<FbDrawer>>? parameters = null, Action<bool>? changed = null)
        => Render<FbDrawer>(p =>
        {
            p.Add(x => x.IsOpen, open)
             .Add(x => x.ChildContent, "<span class=\"menu\">Menu</span>");
            if (changed is not null)
                p.Add(x => x.IsOpenChanged, changed);
            parameters?.Invoke(p);
        });

    private int Calls(string identifier) => JSInterop.Invocations.Count(invocation => invocation.Identifier == identifier);

    [Fact]
    public void A_closed_modal_drawer_is_an_empty_dialog_on_the_left()
    {
        var cut = RenderDrawer(open: false);

        var dialog = cut.Find("dialog");
        Assert.Contains("fb-drawer-modal", dialog.ClassList);
        Assert.Contains("fb-drawer-left", dialog.ClassList);
        Assert.Contains("fb-drawer-panel", dialog.ClassList);
        Assert.Contains("fb-wbs", dialog.ClassList);
        Assert.DoesNotContain("fb-drawer-open", dialog.ClassList);
        Assert.Empty(cut.FindAll(".menu"));
        Assert.Equal(0, Calls("FractalBlazor.showDrawer"));
    }

    [Fact]
    public void An_open_modal_drawer_renders_its_content_then_shows_the_dialog()
    {
        var cut = RenderDrawer(open: true);

        Assert.NotEmpty(cut.FindAll("dialog .menu"));
        Assert.Contains("fb-drawer-open", cut.Find("dialog").ClassList);
        Assert.Equal(1, Calls("FractalBlazor.showDrawer"));
    }

    [Fact]
    public void Closing_a_modal_drawer_closes_the_dialog_then_removes_its_content()
    {
        var cut = RenderDrawer(open: true);

        cut.Render(p => p.Add(x => x.IsOpen, false));

        Assert.Equal(1, Calls("FractalBlazor.closeDrawer"));
        Assert.Empty(cut.FindAll(".menu"));
    }

    [Fact]
    public async Task A_dismissal_asks_the_state_to_close_the_drawer()
    {
        bool? open = null;
        var cut = RenderDrawer(open: true, changed: value => open = value);

        await cut.InvokeAsync(cut.Instance.OnDismiss);

        Assert.False(open);
        Assert.NotEmpty(cut.FindAll(".menu"));
        Assert.Equal(0, Calls("FractalBlazor.closeDrawer"));
    }

    [Fact]
    public async Task A_closed_drawer_ignores_a_late_dismissal()
    {
        bool? open = null;
        var cut = RenderDrawer(open: false, changed: value => open = value);

        await cut.InvokeAsync(cut.Instance.OnDismiss);

        Assert.Null(open);
    }

    [Fact]
    public void A_closed_inline_drawer_keeps_its_content_inert()
    {
        var cut = RenderDrawer(open: false, p => p.Add(x => x.Inline, true));

        var frame = cut.Find(".fb-drawer");
        Assert.Contains("fb-drawer-inline", frame.ClassList);
        Assert.True(frame.HasAttribute("inert"));
        Assert.NotEmpty(cut.FindAll(".fb-drawer-panel .menu"));
        Assert.Empty(cut.FindAll(".fb-drawer-scrim"));
    }

    [Fact]
    public void An_open_inline_drawer_is_no_longer_inert()
    {
        var cut = RenderDrawer(open: true, p => p.Add(x => x.Inline, true));

        var frame = cut.Find(".fb-drawer");
        Assert.False(frame.HasAttribute("inert"));
        Assert.Contains("fb-drawer-open", frame.ClassList);
        Assert.Equal(0, Calls("FractalBlazor.showDrawer"));
    }

    [Fact]
    public void A_click_on_the_veil_of_an_inline_modal_drawer_asks_to_close_it()
    {
        bool? open = null;
        var cut = RenderDrawer(open: true, p => p.Add(x => x.InlineModal, true), value => open = value);

        cut.Find(".fb-drawer-scrim").Click();

        Assert.False(open);
    }

    [Fact]
    public void Escape_in_an_inline_modal_drawer_asks_to_close_it()
    {
        bool? open = null;
        var cut = RenderDrawer(open: true, p => p.Add(x => x.InlineModal, true), value => open = value);

        cut.Find(".fb-drawer").KeyDown(key: "Escape");

        Assert.False(open);
        Assert.Equal("dialog", cut.Find(".fb-drawer-panel").GetAttribute("role"));
    }

    [Fact]
    public void The_size_reaches_the_frame()
    {
        var cut = RenderDrawer(open: true, p => p.Add(x => x.Inline, true).Add(x => x.Size, " 20rem "));

        Assert.Equal("--fb-drawer-size:20rem", cut.Find(".fb-drawer").GetAttribute("style"));
    }

    [Fact]
    public void Side_and_mode_flags_fall_back_to_their_defaults_when_no_longer_passed()
    {
        var cut = RenderDrawer(open: false, p => p.Add(x => x.Right, true).Add(x => x.Inline, true));
        Assert.Contains("fb-drawer-right", cut.Find(".fb-drawer").ClassList);

        cut.Render(p => p.Add(x => x.Right, false).Add(x => x.Inline, false));

        var dialog = cut.Find("dialog");
        Assert.Contains("fb-drawer-left", dialog.ClassList);
        Assert.Contains("fb-drawer-modal", dialog.ClassList);
    }
}
