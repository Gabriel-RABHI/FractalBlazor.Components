using System.ComponentModel.DataAnnotations;
using FractalBlazor.Components.Common.Contracts;
using FractalBlazor.Components.Forms;
using FractalBlazor.Components.Forms.Inputs;

namespace FractalBlazor.Components.Tests.Forms;

public sealed class TabsTests : BunitContext
{
    private enum Section
    {
        [Display(Name = "Compte")] Account,
        [Display(Name = "Mot de passe")] Password
    }

    private static readonly IReadOnlyList<object> Items = [Section.Account, Section.Password, "Réglages"];

    public TabsTests()
    {
        Services.AddFractalBlazor();
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private IRenderedComponent<FbTabs<FbSetValue<int>>> RenderTabs(int value, Action<int> changed, bool disabled = false)
        => Render<FbTabs<FbSetValue<int>>>(p => p
            .Add(x => x.Items, Items)
            .Add(x => x.Value, value)
            .Add(x => x.ValueChanged, changed)
            .Add(x => x.Disabled, disabled));

    [Fact]
    public void Each_item_is_a_tab_rendered_by_its_view()
    {
        var cut = RenderTabs(1, _ => { });

        Assert.NotNull(cut.Find("[role=tablist]"));
        var tabs = cut.FindAll("[role=tab]");
        Assert.Equal(["Compte", "Mot de passe", "Réglages"], tabs.Select(tab => tab.TextContent.Trim()));
        Assert.Equal(["false", "true", "false"], tabs.Select(tab => tab.GetAttribute("aria-selected")));
        Assert.Equal(["-1", "0", "-1"], tabs.Select(tab => tab.GetAttribute("tabindex")));
        Assert.Contains("fb-tab-selected", tabs[1].ClassList);
    }

    [Fact]
    public void A_click_selects_the_tab()
    {
        int? selected = null;
        var cut = RenderTabs(0, value => selected = value);

        cut.FindAll("[role=tab]")[2].Click();

        Assert.Equal(2, selected);
    }

    [Fact]
    public void A_click_sends_the_action_to_the_state()
    {
        var state = new TabState();
        var cut = Render<FbTabs<TabState.Open>>(p => p
            .Add(x => x.Items, Items)
            .Add(x => x.Handler, state));

        cut.FindAll("[role=tab]")[1].Click();

        Assert.Equal(1, state.Tab);
    }

    [Theory]
    [InlineData(0, "ArrowRight", 1)]
    [InlineData(2, "ArrowRight", 0)]
    [InlineData(0, "ArrowLeft", 2)]
    [InlineData(1, "Home", 0)]
    [InlineData(0, "End", 2)]
    public void Keys_move_the_selection(int value, string key, int expected)
    {
        int? selected = null;
        var cut = RenderTabs(value, index => selected = index);

        cut.Find("[role=tablist]").KeyDown(key: key);

        Assert.Equal(expected, selected);
    }

    [Fact]
    public void Disabled_tabs_select_nothing()
    {
        int? selected = null;
        var cut = RenderTabs(0, value => selected = value, disabled: true);

        cut.Find("[role=tablist]").KeyDown(key: "ArrowRight");

        Assert.Null(selected);
        Assert.All(cut.FindAll("[role=tab]"), tab => Assert.True(tab.HasAttribute("disabled")));
    }

    public sealed class TabState
    {
        public sealed record Open(int Value) : IStateAction<int>;

        public int Tab { get; private set; }

        public void Handle(Open action) => Tab = action.Value;
    }
}
