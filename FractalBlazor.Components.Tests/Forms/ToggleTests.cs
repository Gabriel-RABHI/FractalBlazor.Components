using FractalBlazor.Components.Forms.Contracts;
using FractalBlazor.Components.Forms.Inputs;

namespace FractalBlazor.Components.Tests.Forms;

public sealed record SubscribeAction(bool Subscribed) : IStateAction<bool>;

public sealed class NewsletterState
{
    public bool Subscribed { get; private set; }

    public void Handle(SubscribeAction action) => Subscribed = action.Subscribed;
}

public sealed class ToggleTests : BunitContext
{
    [Fact]
    public void Checkbox_sends_its_new_state()
    {
        var state = new NewsletterState();
        var cut = Render<FbCheckbox<SubscribeAction>>(p => p
            .Add(x => x.Handler, state)
            .Add(x => x.Value, state.Subscribed)
            .AddChildContent("Newsletter"));

        cut.Find("input").Change(true);

        Assert.True(state.Subscribed);
    }

    [Fact]
    public void Checkbox_is_wrapped_in_its_label()
    {
        var cut = Render<FbCheckbox<SubscribeAction>>(p => p.Add(x => x.Value, true).AddChildContent("Newsletter"));

        var label = cut.Find("label.fb-toggle");
        Assert.Equal("Newsletter", label.TextContent);
        Assert.True(label.QuerySelector("input.fb-checkbox")!.HasAttribute("checked"));
    }

    [Fact]
    public void Switch_is_announced_as_a_switch()
    {
        var cut = Render<FbSwitch<SubscribeAction>>();

        var input = cut.Find("input");
        Assert.Equal("switch", input.GetAttribute("role"));
        Assert.Contains("fb-switch", input.ClassList);
    }

    [Fact]
    public void Disabled_switch_sends_nothing()
    {
        var state = new NewsletterState();
        var cut = Render<FbSwitch<SubscribeAction>>(p => p.Add(x => x.Handler, state).Add(x => x.Disabled, true));

        cut.Find("input").Change(true);

        Assert.False(state.Subscribed);
    }
}
