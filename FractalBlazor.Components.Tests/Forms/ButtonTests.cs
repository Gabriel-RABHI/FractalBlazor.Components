using FractalBlazor.Components.Forms.Icons;
using FractalBlazor.Components.Forms.Inputs;
using FractalBlazor.Components.Forms.Theming.Constants;

namespace FractalBlazor.Components.Tests.Forms;

public sealed class ButtonTests : BunitContext
{
    [Fact]
    public void Default_button_uses_the_layout_grammar()
    {
        var cut = Render<FbButton>(p => p.AddChildContent("Save"));

        var button = cut.Find("button");
        Assert.Equal("button", button.GetAttribute("type"));
        Assert.Contains("fb-bg-accent", button.ClassList);
        Assert.Contains("fb-framed-medium", button.ClassList);
        Assert.Contains("fb-hover", button.ClassList);
        Assert.Contains("border-radius:var(--fb-radius-m)", button.GetAttribute("style"));
        Assert.Equal("Save", button.TextContent);
    }

    [Fact]
    public void Defaults_can_be_replaced_or_removed()
    {
        var cut = Render<FbButton>(p => p.Add(x => x.WBH, true).Add(x => x.WFN, true));

        var button = cut.Find("button");
        Assert.Contains("fb-bg-highlight", button.ClassList);
        Assert.DoesNotContain("fb-bg-accent", button.ClassList);
        Assert.DoesNotContain("fb-framed", button.ClassList);
    }

    [Fact]
    public void Click_sends_the_action_then_raises_on_click()
    {
        var state = new PersonState();
        var clicks = 0;
        var cut = Render<FbButton>(p => p
            .Add(x => x.Handler, state)
            .Add(x => x.Action, new ResetAction())
            .Add(x => x.OnClick, () => clicks++));

        cut.Find("button").Click();

        Assert.Equal(1, state.Resets);
        Assert.Equal(1, clicks);
    }

    [Fact]
    public void Button_without_behaviour_registers_no_click()
    {
        var cut = Render<FbButton>();

        Assert.DoesNotContain("onclick", cut.Markup);
    }

    [Fact]
    public void Disabled_button_is_inert_and_uses_the_disabled_variant()
    {
        var state = new PersonState();
        var cut = Render<FbButton>(p => p
            .Add(x => x.Handler, state)
            .Add(x => x.Action, new ResetAction())
            .Add(x => x.Disabled, true));

        Assert.True(cut.Find("button").HasAttribute("disabled"));
        Assert.DoesNotContain("onclick", cut.Markup);
        Assert.Contains("var(--fb-disabled-bg-low-anchor)", cut.Find("div.fb-theme-scope").GetAttribute("style"));
    }

    [Fact]
    public void Variant_gives_the_functional_state()
    {
        var cut = Render<FbButton>(p => p.Add(x => x.Variant, FbThemeVariants.Error));

        Assert.Contains("var(--fb-error-bg-low-anchor)", cut.Find("div.fb-theme-scope").GetAttribute("style"));
    }

    [Fact]
    public void Icon_comes_before_the_content()
    {
        var cut = Render<FbButton>(p => p.Add(x => x.Icon, FbMaterialIcons.Search).AddChildContent("<span>Find</span>"));

        var children = cut.Find("button").Children;
        Assert.Equal("svg", children[0].LocalName);
        Assert.Equal("span", children[1].LocalName);
    }
}
