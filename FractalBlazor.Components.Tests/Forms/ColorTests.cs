using FractalBlazor.Components.Common.Contracts;
using FractalBlazor.Components.Forms.Components.Inputs;
using Microsoft.AspNetCore.Components;

namespace FractalBlazor.Components.Tests.Forms;

public sealed class ColorTests : BunitContext
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
}
