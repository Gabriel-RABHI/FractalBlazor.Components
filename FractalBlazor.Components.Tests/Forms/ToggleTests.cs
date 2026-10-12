using FractalBlazor.Components.Common.Contracts;
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

        var label = cut.Find("label.fb-check");
        Assert.Equal("Newsletter", label.TextContent);
        Assert.True(label.QuerySelector("input.fb-check-input")!.HasAttribute("checked"));
        Assert.Equal("Newsletter", label.QuerySelector(".fb-check-content")!.TextContent);
    }

    [Fact]
    public void Checkbox_draws_its_box_before_the_content_and_centers_it()
    {
        var cut = Render<FbCheckbox<SubscribeAction>>(p => p.AddChildContent("Newsletter"));

        var label = cut.Find("label");
        Assert.Equal(["input", "span", "span"], label.Children.Select(child => child.LocalName));
        Assert.Contains("fb-check-box", label.Children[1].ClassList);
        Assert.Equal("true", label.Children[1].GetAttribute("aria-hidden"));
        Assert.Contains("fb-check-center", label.ClassList);
        Assert.DoesNotContain("fb-check-mirror", label.ClassList);
        Assert.DoesNotContain("fb-check-circle", label.ClassList);
    }

    [Fact]
    public void Checkbox_options_set_their_classes()
    {
        var cut = Render<FbCheckbox<SubscribeAction>>(p => p.Add(x => x.Mirror, true).Add(x => x.Circle, true).Add(x => x.Bottom, true));

        var label = cut.Find("label");
        Assert.Contains("fb-check-mirror", label.ClassList);
        Assert.Contains("fb-check-circle", label.ClassList);
        Assert.Contains("fb-check-bottom", label.ClassList);
        Assert.DoesNotContain("fb-check-center", label.ClassList);
    }

    [Fact]
    public void Checkbox_alignment_can_be_computed()
    {
        var cut = Render<FbCheckbox<SubscribeAction>>(p => p.Add(x => x.Align, FbCheckboxAlign.Top));

        Assert.Contains("fb-check-top", cut.Find("label").ClassList);
    }

    [Fact]
    public void Checkbox_options_follow_the_parameters()
    {
        var cut = Render<FbCheckbox<SubscribeAction>>(p => p.Add(x => x.Mirror, true).Add(x => x.Top, true));

        cut.Render(p => p.Add(x => x.Circle, true));

        var label = cut.Find("label");
        Assert.DoesNotContain("fb-check-mirror", label.ClassList);
        Assert.Contains("fb-check-center", label.ClassList);
        Assert.Contains("fb-check-circle", label.ClassList);
    }

    [Fact]
    public void Disabled_checkbox_sends_nothing()
    {
        var state = new NewsletterState();
        var cut = Render<FbCheckbox<SubscribeAction>>(p => p.Add(x => x.Handler, state).Add(x => x.Disabled, true));

        cut.Find("input").Change(true);

        Assert.False(state.Subscribed);
        Assert.True(cut.Find("input").HasAttribute("disabled"));
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
