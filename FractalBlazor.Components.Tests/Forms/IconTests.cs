using FractalBlazor.Components.Forms.Icons;
using FractalBlazor.Components.Layout;

namespace FractalBlazor.Components.Tests.Forms;

public sealed class IconTests : BunitContext
{
    [Fact]
    public void Decorative_icon_is_hidden_from_assistive_technologies()
    {
        var cut = Render<FbIcon>(p => p.Add(x => x.Icon, FbMaterialIcons.Search));

        var svg = cut.Find("svg");
        Assert.Equal("true", svg.GetAttribute("aria-hidden"));
        Assert.Equal("currentColor", svg.GetAttribute("fill"));
        Assert.NotEmpty(svg.QuerySelectorAll("path"));
    }

    [Fact]
    public void Titled_icon_is_announced()
    {
        var cut = Render<FbIcon>(p => p.Add(x => x.Icon, FbMaterialIcons.Search).Add(x => x.Title, "Search"));

        var svg = cut.Find("svg");
        Assert.Equal("img", svg.GetAttribute("role"));
        Assert.Equal("Search", svg.QuerySelector("title")!.TextContent);
    }

    [Fact]
    public void Size_and_color_flags_match_the_enum_api()
    {
        var flags = Render<FbIcon>(p => p.Add(x => x.Icon, FbMaterialIcons.Check).Add(x => x.L, true).Add(x => x.Muted, true));
        var enums = Render<FbIcon>(p => p.Add(x => x.Icon, FbMaterialIcons.Check).Add(x => x.Size, FbSpacing.L)
            .Add(x => x.Color, FractalBlazor.Components.Forms.Display.FbTextColor.Muted));

        Assert.Equal("fb-icon fb-icon-l fb-fg-muted", flags.Find("svg").GetAttribute("class"));
        Assert.Equal(flags.Find("svg").GetAttribute("class"), enums.Find("svg").GetAttribute("class"));
    }
}
