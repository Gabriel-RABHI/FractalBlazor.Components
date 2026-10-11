using FractalBlazor.Components.Common.Contracts;
using FractalBlazor.Components.Forms.Inputs;
using Microsoft.AspNetCore.Components;

namespace FractalBlazor.Components.Tests.Forms;

public sealed class ColorAndRangeTests : BunitContext
{
    [Fact]
    public void Color_is_shown_and_sent_in_lowercase()
    {
        string? sent = null;
        var cut = Render<FbInputColor<FbSetValue<string>>>(p => p
            .Add(x => x.Value, "#ABC")
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string>(this, value => sent = value)));

        Assert.Equal("color", cut.Find("input").GetAttribute("type"));
        Assert.Equal("#aabbcc", cut.Find("input").GetAttribute("value"));

        cut.Find("input").Change("#12AB9F");

        Assert.Equal("#12ab9f", sent);
    }

    [Fact]
    public void Invalid_color_is_not_sent()
    {
        string? sent = null;
        var cut = Render<FbInputColor<FbSetValue<string>>>(p => p
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string>(this, value => sent = value)));

        cut.Find("input").Change("red");

        Assert.Null(sent);
    }

    [Fact]
    public void Range_uses_the_native_slider_with_its_bounds()
    {
        var sent = 0d;
        var cut = Render<FbInputRange<double, FbSetValue<double>>>(p => p
            .Add(x => x.Value, 8d)
            .Add(x => x.Min, 0d)
            .Add(x => x.Max, 40d)
            .Add(x => x.Step, 0.5d)
            .Add(x => x.Immediate, true)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<double>(this, value => sent = value)));

        var input = cut.Find("input");
        Assert.Equal("range", input.GetAttribute("type"));
        Assert.Equal("40", input.GetAttribute("max"));
        Assert.Equal("0.5", input.GetAttribute("step"));

        input.Input("12.5");

        Assert.Equal(12.5d, sent);
    }
}
