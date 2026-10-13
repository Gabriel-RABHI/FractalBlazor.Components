using FractalBlazor.Components.Layout;

namespace FractalBlazor.Components.Tests.Layout;

public sealed class DropDownTests : BunitContext
{
    public DropDownTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private IRenderedComponent<FbDropDown> RenderOpen(Action<bool> changed)
        => Render<FbDropDown>(p => p
            .Add(x => x.IsOpen, true)
            .Add(x => x.IsOpenChanged, changed)
            .Add(x => x.ChildContent, "<span class=\"anchor\">Open</span>")
            .Add(x => x.DropDownContent, "<span class=\"zone\">Zone</span>"));

    [Fact]
    public void Escape_closes_the_zone()
    {
        bool? open = null;
        var cut = RenderOpen(value => open = value);

        cut.Find(".fb-dropdown-container").KeyDown(key: "Escape");

        Assert.False(open);
        Assert.Empty(cut.FindAll(".zone"));
    }

    [Fact]
    public void Other_keys_keep_the_zone_open()
    {
        bool? open = null;
        var cut = RenderOpen(value => open = value);

        cut.Find(".fb-dropdown-container").KeyDown(key: "a");

        Assert.Null(open);
        Assert.NotEmpty(cut.FindAll(".zone"));
    }

    [Fact]
    public void Click_outside_closes_the_zone()
    {
        bool? open = null;
        var cut = RenderOpen(value => open = value);

        cut.Find(".fb-dropdown-overlay").Click();

        Assert.False(open);
    }

    [Fact]
    public async Task A_dropdown_never_opened_never_calls_the_script()
    {
        Render<FbDropDown>(p => p.Add(x => x.ChildContent, "<span class=\"anchor\">Open</span>"));

        await DisposeComponentsAsync();

        Assert.DoesNotContain(JSInterop.Invocations, invocation => invocation.Identifier.StartsWith("FractalBlazor.", StringComparison.Ordinal));
    }

    [Fact]
    public async Task An_opened_dropdown_is_destroyed_once()
    {
        var cut = RenderOpen(_ => { });

        cut.Render(p => p.Add(x => x.IsOpen, false));
        await DisposeComponentsAsync();

        Assert.Single(JSInterop.Invocations, invocation => invocation.Identifier == "FractalBlazor.initDropdown");
        Assert.Single(JSInterop.Invocations, invocation => invocation.Identifier == "FractalBlazor.destroyDropdown");
    }

    [Fact]
    public void Block_takes_the_full_width()
    {
        var cut = Render<FbDropDown>(p => p.Add(x => x.Block, true));

        Assert.Contains("fb-dropdown-block", cut.Find(".fb-dropdown-container").ClassList);
    }
}
