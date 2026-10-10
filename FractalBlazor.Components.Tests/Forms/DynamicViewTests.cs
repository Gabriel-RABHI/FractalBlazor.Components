using FractalBlazor.Components.Common.Attributes;
using FractalBlazor.Components.Forms.Contracts;
using FractalBlazor.Components.Common.Contracts;
using FractalBlazor.Components.Forms.Core;
using FractalBlazor.Components.Forms.Mapping;
using FractalBlazor.Components.Layout.Abstracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace FractalBlazor.Components.Tests.Forms;

public enum TestSelector
{
    Viewer,
    Summary
}

public class AnimalState
{
}

public sealed class CatState : AnimalState
{
}

[ViewFor<TestSelector>(typeof(AnimalState), TestSelector.Viewer)]
public sealed class AnimalView : FbComponentBase
{
    [Parameter]
    public object? Item { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "span");
        builder.AddAttribute(1, "class", "animal");
        builder.CloseElement();
    }
}

[ViewFor<TestSelector>(typeof(CounterState), TestSelector.Summary)]
public sealed class CounterSummary : FbComponentBase
{
    [Parameter]
    public object? Item { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "span");
        builder.AddAttribute(1, "class", "summary");
        builder.CloseElement();
    }
}

public sealed class CounterState : INotifyStateChanged
{
    private Action? _changed;

    public int Subscribers { get; private set; }

    public int Value { get; private set; }

    public event Action? OnStateChanged
    {
        add { _changed += value; Subscribers++; }
        remove { _changed -= value; Subscribers--; }
    }

    public void Increment()
    {
        Value++;
        _changed?.Invoke();
    }
}

[ViewFor<TestSelector>(typeof(CounterState), TestSelector.Viewer)]
public sealed class CounterView : FbComponentBase
{
    [Parameter]
    public object? Item { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "span");
        builder.AddAttribute(1, "class", "counter");
        builder.AddContent(2, ((CounterState)Item!).Value);
        builder.CloseElement();
    }
}

public sealed class DynamicViewTests : BunitContext
{
    public DynamicViewTests()
    {
        Services.AddSingleton<IFbViewRegistry<TestSelector>, FbViewRegistry<TestSelector>>();
    }

    [Fact]
    public void Picks_the_view_declared_for_the_state_type()
    {
        var state = new CounterState();

        var cut = Render<FbDynamicView<TestSelector>>(p => p.Add(x => x.Item, state));

        Assert.Equal("0", cut.Find(".counter").TextContent);
    }

    [Fact]
    public void Selector_picks_another_view_of_the_same_state()
    {
        var cut = Render<FbDynamicView<TestSelector>>(p => p
            .Add(x => x.Item, new CounterState())
            .Add(x => x.Selector, TestSelector.Summary));

        Assert.NotNull(cut.Find(".summary"));
    }

    [Fact]
    public void View_declared_for_a_base_type_displays_derived_types()
    {
        var cut = Render<FbDynamicView<TestSelector>>(p => p.Add(x => x.Item, new CatState()));

        Assert.NotNull(cut.Find(".animal"));
    }

    [Fact]
    public void Missing_view_is_reported()
    {
        var cut = Render<FbDynamicView<TestSelector>>(p => p
            .Add(x => x.Item, new CatState())
            .Add(x => x.Selector, TestSelector.Summary));

        Assert.Contains("No Summary view registered for type CatState", cut.Find(".fb-missing-view").TextContent);
    }

    [Fact]
    public void Rerenders_when_the_state_notifies()
    {
        var state = new CounterState();
        var cut = Render<FbDynamicView<TestSelector>>(p => p.Add(x => x.Item, state));

        cut.InvokeAsync(state.Increment);

        cut.WaitForAssertion(() => Assert.Equal("1", cut.Find(".counter").TextContent));
    }

    [Fact]
    public async Task Disposal_unsubscribes_from_the_state()
    {
        var state = new CounterState();
        Render<FbDynamicView<TestSelector>>(p => p.Add(x => x.Item, state));
        Assert.Equal(1, state.Subscribers);

        await DisposeComponentsAsync();

        Assert.Equal(0, state.Subscribers);
    }
}
