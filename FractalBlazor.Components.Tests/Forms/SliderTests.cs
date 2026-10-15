using System.Globalization;
using System.Numerics;
using FractalBlazor.Components.Common.Contracts;
using FractalBlazor.Components.Forms;
using FractalBlazor.Components.Forms.Components.Inputs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace FractalBlazor.Components.Tests.Forms;

/// <summary>
/// The slider of the library: one or two thumbs on a track drawn by the library, moved by the
/// keyboard or by a drag that the script reports with <c>OnSlide</c>.
/// </summary>
public sealed class SliderTests : BunitContext
{
    private readonly List<int> _sent = [];
    private readonly List<FbRange<int>> _ranges = [];

    public SliderTests()
    {
        Services.AddFractalBlazor();
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private IRenderedComponent<FbSlider<int, FbSetValue<int>>> Slider(int value, Action<ComponentParameterCollectionBuilder<FbSlider<int, FbSetValue<int>>>>? more = null)
        => Render<FbSlider<int, FbSetValue<int>>>(p =>
        {
            p.Add(x => x.Value, value)
             .Add(x => x.Max, 200)
             .Add(x => x.ValueChanged, EventCallback.Factory.Create<int>(this, _sent.Add));
            more?.Invoke(p);
        });

    private IRenderedComponent<FbRangeSlider<int, FbSetValue<FbRange<int>>>> Range(int start, int end, Action<ComponentParameterCollectionBuilder<FbRangeSlider<int, FbSetValue<FbRange<int>>>>>? more = null)
        => Render<FbRangeSlider<int, FbSetValue<FbRange<int>>>>(p =>
        {
            p.Add(x => x.Value, new FbRange<int>(start, end))
             .Add(x => x.ValueChanged, EventCallback.Factory.Create<FbRange<int>>(this, _ranges.Add));
            more?.Invoke(p);
        });

    private static string[] Labels(IRenderedComponent<IComponent> cut, string kind)
        => cut.FindAll("." + kind).Select(label => label.TextContent.Trim()).ToArray();

    // ------------------------------------------------------------------ Rendering

    [Fact]
    public void A_slider_places_one_thumb_on_its_track()
    {
        var cut = Slider(40);

        var root = cut.Find(".fb-slider");
        Assert.Equal("--fb-slider-from:0%;--fb-slider-to:20%;", root.GetAttribute("style"));
        Assert.Equal(("0", "200", "1"), (root.GetAttribute("data-min"), root.GetAttribute("data-max"), root.GetAttribute("data-step")));
        var thumb = cut.Find(".fb-slider-thumb");
        Assert.Equal(("slider", "40", "0", "200", "horizontal"),
            (thumb.GetAttribute("role"), thumb.GetAttribute("aria-valuenow"), thumb.GetAttribute("aria-valuemin"),
             thumb.GetAttribute("aria-valuemax"), thumb.GetAttribute("aria-orientation")));
        Assert.Contains("fb-slider-thumb-to", thumb.ClassList);
        Assert.Single(JSInterop.Invocations, invocation => invocation.Identifier == "FractalBlazor.initSlider");
    }

    [Fact]
    public void A_range_has_two_thumbs_that_bound_each_other()
    {
        var cut = Range(20, 60);

        Assert.Contains("fb-slider-range", cut.Find(".fb-slider").ClassList);
        Assert.Equal("--fb-slider-from:20%;--fb-slider-to:60%;", cut.Find(".fb-slider").GetAttribute("style"));
        var thumbs = cut.FindAll(".fb-slider-thumb");
        Assert.Equal(("20", "60"), (thumbs[0].GetAttribute("aria-valuenow"), thumbs[0].GetAttribute("aria-valuemax")));
        Assert.Equal(("60", "20"), (thumbs[1].GetAttribute("aria-valuenow"), thumbs[1].GetAttribute("aria-valuemin")));
    }

    [Fact]
    public void A_range_given_upside_down_reads_in_order()
    {
        var thumbs = Range(60, 20).FindAll(".fb-slider-thumb");

        Assert.Equal(["20", "60"], thumbs.Select(thumb => thumb.GetAttribute("aria-valuenow")));
    }

    [Fact]
    public void A_vertical_slider_says_so()
    {
        var cut = Slider(10, p => p.Add(x => x.Vertical, true));

        Assert.Contains("fb-slider-vertical", cut.Find(".fb-slider").ClassList);
        Assert.Equal("vertical", cut.Find(".fb-slider-thumb").GetAttribute("aria-orientation"));
    }

    // ------------------------------------------------------------------ Drag

    [Fact]
    public async Task A_drag_shows_the_value_and_sends_it_on_release()
    {
        var cut = Slider(40);

        await cut.InvokeAsync(() => cut.Instance.OnSlide(0, 87.4, done: false));
        Assert.Empty(_sent);
        Assert.Equal("87", cut.Find(".fb-slider-thumb").GetAttribute("aria-valuenow"));

        await cut.InvokeAsync(() => cut.Instance.OnSlide(0, 90.2, done: true));
        Assert.Equal([90], _sent);
        Assert.Equal("40", cut.Find(".fb-slider-thumb").GetAttribute("aria-valuenow"));
    }

    [Fact]
    public async Task Immediate_sends_the_value_while_the_thumb_moves()
    {
        var cut = Slider(40, p => p.Add(x => x.Immediate, true));

        await cut.InvokeAsync(() => cut.Instance.OnSlide(0, 87, done: false));
        await cut.InvokeAsync(() => cut.Instance.OnSlide(0, 87, done: true));

        Assert.Equal([87, 87], _sent);
    }

    [Theory]
    [InlineData(12, 5, 10)]
    [InlineData(13, 5, 15)]
    [InlineData(250, 5, 200)]
    [InlineData(-3, 5, 0)]
    public async Task A_dragged_value_lands_on_a_step_between_the_bounds(double at, int step, int expected)
    {
        var cut = Slider(40, p => p.Add(x => x.Step, step));

        await cut.InvokeAsync(() => cut.Instance.OnSlide(0, at, done: true));

        Assert.Equal([expected], _sent);
    }

    [Theory]
    [InlineData(0, 1, 0.1, 0.31, 0.3)]
    [InlineData(0, 1, 0.1, 0.68, 0.7)]
    [InlineData(0, 2, 0.25, 1.6, 1.5)]
    [InlineData(0.05, 1, 0.1, 0.16, 0.15)]
    [InlineData(0, 0.001, 0.00001, 0.000123, 0.00012)]
    [InlineData(0, 1, 0, 0.123, 0.123)]
    public void A_fractional_step_lands_on_round_values(double min, double max, double step, double at, double expected)
    {
        var cut = Render<FbSlider<double, FbSetValue<double>>>(p => p.Add(x => x.Min, min).Add(x => x.Max, max).Add(x => x.Step, step));

        Assert.Equal(expected, cut.Instance.Snap(at));
    }

    [Fact]
    public void Every_floating_type_takes_a_quarter_step()
    {
        Assert.Equal(0.75f, Quarter<float>(0.65));
        Assert.Equal(0.75d, Quarter<double>(0.65));
        Assert.Equal(0.75m, Quarter<decimal>(0.65));
    }

    private TValue Quarter<TValue>(double at) where TValue : struct, INumber<TValue>
        => Render<FbSlider<TValue, FbSetValue<TValue>>>(p => p.Add(x => x.Max, TValue.One).Add(x => x.Step, TValue.CreateChecked(0.25)))
            .Instance.Snap(at);

    [Fact]
    public async Task A_double_slider_moves_by_quarters()
    {
        var sent = new List<double>();
        var cut = Render<FbSlider<double, FbSetValue<double>>>(p => p
            .Add(x => x.Value, 0.5).Add(x => x.Max, 2d).Add(x => x.Step, 0.25)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<double>(this, sent.Add)));

        cut.Find(".fb-slider-thumb").KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });
        await cut.InvokeAsync(() => cut.Instance.OnSlide(0, 1.6, done: true));

        Assert.Equal([0.75, 1.5], sent);
        Assert.Equal("0.25", cut.Find(".fb-slider").GetAttribute("data-step"));
    }

    [Fact]
    public async Task A_dragged_thumb_of_a_range_stops_at_the_other_one()
    {
        var cut = Range(20, 60);

        await cut.InvokeAsync(() => cut.Instance.OnSlide(0, 80, done: true));
        await cut.InvokeAsync(() => cut.Instance.OnSlide(1, 5, done: true));

        Assert.Equal([new FbRange<int>(60, 60), new FbRange<int>(20, 20)], _ranges);
    }

    // ------------------------------------------------------------------ Keyboard

    [Theory]
    [InlineData("ArrowRight", 41)]
    [InlineData("ArrowUp", 41)]
    [InlineData("ArrowLeft", 39)]
    [InlineData("PageUp", 60)]
    [InlineData("PageDown", 20)]
    [InlineData("Home", 0)]
    [InlineData("End", 200)]
    public void The_keyboard_moves_the_thumb(string key, int expected)
    {
        var cut = Slider(40);

        cut.Find(".fb-slider-thumb").KeyDown(new KeyboardEventArgs { Key = key });

        Assert.Equal([expected], _sent);
        Assert.True(cut.Find(".fb-slider-thumb").HasAttribute("data-fb-nav-keys"));
    }

    [Fact]
    public void A_key_moves_a_thumb_of_a_range_up_to_the_other_one()
    {
        var cut = Range(20, 60);

        cut.FindAll(".fb-slider-thumb")[0].KeyDown(new KeyboardEventArgs { Key = "End" });
        cut.FindAll(".fb-slider-thumb")[1].KeyDown(new KeyboardEventArgs { Key = "Home" });

        Assert.Equal([new FbRange<int>(60, 60), new FbRange<int>(20, 20)], _ranges);
    }

    [Fact]
    public async Task A_disabled_slider_sends_nothing()
    {
        var cut = Slider(40, p => p.Add(x => x.D, true));

        var thumb = cut.Find(".fb-slider-thumb");
        Assert.Equal(("-1", "true"), (thumb.GetAttribute("tabindex"), thumb.GetAttribute("aria-disabled")));
        Assert.Contains("fb-v-disabled", cut.Find(".fb-slider").ClassList);

        thumb.KeyDown(new KeyboardEventArgs { Key = "End" });
        await cut.InvokeAsync(() => cut.Instance.OnSlide(0, 100, done: true));

        Assert.Empty(_sent);
    }

    // ------------------------------------------------------------------ Ticks and labels

    [Fact]
    public void Ticks_are_placed_and_labeled_and_those_of_the_filled_part_say_so()
    {
        var cut = Slider(100, p => p.Add(x => x.Ticks, 50).Add(x => x.TL, true).Add(x => x.Dot, true));

        var ticks = cut.FindAll(".fb-slider-tick");
        Assert.Equal(["--fb-slider-at:0%;", "--fb-slider-at:25%;", "--fb-slider-at:50%;", "--fb-slider-at:75%;", "--fb-slider-at:100%;"],
            ticks.Select(tick => tick.GetAttribute("style")));
        Assert.Equal([true, true, true, false, false], ticks.Select(tick => tick.ClassList.Contains("fb-slider-tick-in")));
        Assert.Equal(["0", "50", "100", "150", "200"], Labels(cut, "fb-slider-label"));
        Assert.Contains("fb-slider-ticks-dot", cut.Find(".fb-slider").ClassList);
        Assert.Contains("fb-slider-scale", cut.Find(".fb-slider").ClassList);
        Assert.Contains("fb-slider-tick-start", ticks[0].ClassList);
        Assert.Contains("fb-slider-tick-end", ticks[4].ClassList);
    }

    [Fact]
    public void Ticks_of_a_fractional_gap_land_on_round_values_up_to_the_end()
    {
        var cut = Render<FbSlider<double, FbSetValue<double>>>(p => p.Add(x => x.Max, 0.3).Add(x => x.Step, 0.1).Add(x => x.Ticks, 0.1));

        Assert.Equal([0, 0.1, 0.2, 0.3], cut.Instance.TickValues());
    }

    [Fact]
    public void Ticks_without_labels_draw_marks_only()
    {
        var cut = Slider(100, p => p.Add(x => x.Ticks, 50).Add(x => x.Dot, true));

        Assert.Equal(5, cut.FindAll(".fb-slider-tick").Count);
        Assert.Empty(cut.FindAll(".fb-slider-label"));
    }

    [Theory]
    [InlineData(FbTickMark.Dot, "fb-slider-ticks-dot")]
    [InlineData(FbTickMark.Line, "fb-slider-ticks-line")]
    [InlineData(FbTickMark.ShiftedLine, "fb-slider-ticks-shifted")]
    public void A_flag_or_the_tick_mark_chooses_how_ticks_are_marked(FbTickMark mark, string expected)
    {
        var byFlag = Slider(100, p => _ = mark switch
        {
            FbTickMark.Dot => p.Add(x => x.Dot, true),
            FbTickMark.Line => p.Add(x => x.Line, true),
            _ => p.Add(x => x.ShiftedLine, true)
        });
        var byMark = Slider(100, p => p.Add(x => x.TickMark, mark));

        Assert.Contains(expected, byFlag.Find(".fb-slider").ClassList);
        Assert.Contains(expected, byMark.Find(".fb-slider").ClassList);
        Assert.Equal(mark, byFlag.Instance.TickMark);
    }

    [Fact]
    public void Ticks_are_not_marked_by_default_and_a_mark_no_longer_passed_falls_back()
    {
        var cut = Slider(100, p => p.Add(x => x.Ticks, 50).Add(x => x.Line, true));

        cut.Render(p => p.Add(x => x.Value, 100).Add(x => x.Ticks, 50));

        Assert.Equal(FbTickMark.None, cut.Instance.TickMark);
        Assert.DoesNotContain(cut.Find(".fb-slider").ClassList, name => name.StartsWith("fb-slider-ticks-", StringComparison.Ordinal));
    }

    [Fact]
    public void The_ends_are_labeled_alone()
    {
        var cut = Slider(100, p => p.Add(x => x.EL, true));

        Assert.Equal(["0", "200"], Labels(cut, "fb-slider-label"));
    }

    [Fact]
    public void A_format_or_an_item_gives_the_labels()
    {
        var formatted = Slider(100, p => p.Add(x => x.EL, true).Add(x => x.Format, "0.0"));
        var items = Slider(100, p => p.Add(x => x.VL, true).Add(x => x.LabelOf, value => $"{value / 10} h"));

        Assert.Equal([0d.ToString("0.0", CultureInfo.CurrentCulture), 200d.ToString("0.0", CultureInfo.CurrentCulture)], Labels(formatted, "fb-slider-label"));
        Assert.Equal(["10 h"], Labels(items, "fb-slider-value"));
        Assert.Equal("10 h", items.Find(".fb-slider-thumb").GetAttribute("aria-valuetext"));
    }

    [Fact]
    public void Values_are_shown_beside_the_thumbs_or_in_a_bubble()
    {
        var cut = Range(20, 60, p => p.Add(x => x.VL, true).Add(x => x.VT, true));

        Assert.Equal(["20", "60"], Labels(cut, "fb-slider-value"));
        Assert.Equal(["20", "60"], Labels(cut, "fb-slider-bubble"));
        Assert.Contains("fb-slider-values", cut.Find(".fb-slider").ClassList);
    }

    [Fact]
    public void A_flag_no_longer_passed_falls_back()
    {
        var cut = Slider(10, p => p.Add(x => x.Vertical, true).Add(x => x.Mirror, true));

        cut.Render(p => p.Add(x => x.Value, 10));

        Assert.DoesNotContain("fb-slider-vertical", cut.Find(".fb-slider").ClassList);
        Assert.DoesNotContain("fb-slider-mirror", cut.Find(".fb-slider").ClassList);
    }
}
