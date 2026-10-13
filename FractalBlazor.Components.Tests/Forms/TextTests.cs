using FractalBlazor.Components.Forms.Components.Display;

namespace FractalBlazor.Components.Tests.Forms;

public sealed class TextTests : BunitContext
{
    private string ClassOf(Action<ComponentParameterCollectionBuilder<FbText>> parameters)
        => Render<FbText>(parameters).Find("span").GetAttribute("class")!;

    [Fact]
    public void Flags_produce_their_classes()
    {
        Assert.Equal("fb-txt fb-t-s fb-t-b fb-fg-muted", ClassOf(p => p.Add(x => x.S, true).Add(x => x.B, true).Add(x => x.Muted, true)));
    }

    [Fact]
    public void Enum_api_matches_the_flags()
    {
        var flags = ClassOf(p => p.Add(x => x.XL, true).Add(x => x.T, true).Add(x => x.Highlight, true));
        var enums = ClassOf(p => p
            .Add(x => x.Size, FbTextSize.XL)
            .Add(x => x.Weight, FbTextWeight.T)
            .Add(x => x.Color, FbTextColor.Highlight));

        Assert.Equal(flags, enums);
    }

    [Fact]
    public void No_wrap_and_trim_are_cumulative()
    {
        Assert.Equal("fb-txt fb-t-nw", ClassOf(p => p.Add(x => x.NW, true)));
        Assert.Equal("fb-txt fb-t-tr", ClassOf(p => p.Add(x => x.TR, true)));
        Assert.Equal("fb-txt fb-t-nw fb-t-tr", ClassOf(p => p.Add(x => x.NW, true).Add(x => x.TR, true)));
    }

    [Fact]
    public void Plain_text_has_only_the_base_class()
    {
        var cut = Render<FbText>(p => p.Add(x => x.Value, "Hello"));

        Assert.Equal("fb-txt", cut.Find("span").GetAttribute("class"));
        Assert.Equal("Hello", cut.Find("span").TextContent);
    }

    [Fact]
    public void Flag_turned_off_falls_back_to_default()
    {
        var cut = Render<FbText>(p => p.Add(x => x.Muted, true));

        cut.Render(p => p.Add(x => x.Muted, false));

        Assert.Equal("fb-txt", cut.Find("span").GetAttribute("class"));
    }
}
