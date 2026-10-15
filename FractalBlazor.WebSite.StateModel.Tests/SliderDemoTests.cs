using FractalBlazor.Components.Common.Contracts;

namespace FractalBlazor.WebSite.StateModel.Tests;

/// <summary>
/// The states of the slider demos take the values the sliders send, and notify only when one
/// changes.
/// </summary>
public sealed class SliderDemoTests
{
    [Fact]
    public void A_new_value_notifies_once_and_the_same_value_does_not()
    {
        var scales = new ScaleSliderDemoState();
        var notifications = 0;
        scales.OnStateChanged += () => notifications++;

        scales.Handle(new ScaleSliderDemoState.SetHours(new FbRange<int>(12, 20)));
        scales.Handle(new ScaleSliderDemoState.SetHours(new FbRange<int>(12, 20)));
        scales.Handle(new ScaleSliderDemoState.SetPrice(scales.Price));

        Assert.Equal(new FbRange<int>(12, 20), scales.Hours);
        Assert.Equal(1, notifications);
    }

    [Fact]
    public void The_day_counts_from_the_first_one()
    {
        var scales = new ScaleSliderDemoState();

        scales.Handle(new ScaleSliderDemoState.SetDay(6));

        Assert.Equal(new DateOnly(2022, 6, 19), scales.Date);
    }

    [Fact]
    public void Every_demo_value_can_be_set()
    {
        var verticals = new VerticalSliderDemoState();
        var dots = new DotsSliderDemoState();

        verticals.Handle(new VerticalSliderDemoState.SetLevel(30));
        verticals.Handle(new VerticalSliderDemoState.SetBand(new FbRange<int>(10, 20)));
        dots.Handle(new DotsSliderDemoState.SetRating(3));
        dots.Handle(new DotsSliderDemoState.SetSpan(new FbRange<int>(20, 80)));
        dots.Handle(new DotsSliderDemoState.SetStep(7));
        dots.Handle(new DotsSliderDemoState.SetSteps(new FbRange<int>(2, 4)));

        Assert.Equal((30, new FbRange<int>(10, 20)), (verticals.Level, verticals.Band));
        Assert.Equal((3, new FbRange<int>(20, 80), 7, new FbRange<int>(2, 4)), (dots.Rating, dots.Span, dots.Step, dots.Steps));
    }
}
