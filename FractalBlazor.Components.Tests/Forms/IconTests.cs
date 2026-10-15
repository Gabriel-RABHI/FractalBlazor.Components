using FractalBlazor.Components.Forms.Components.Display;
using FractalBlazor.Components.Forms.Components.Icons;
using FractalBlazor.Components.Forms.Theming.Constants;
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
            .Add(x => x.Color, FbTextColor.Muted));

        Assert.Equal("fb-icon fb-icon-l fb-muted", flags.Find("svg").GetAttribute("class"));
        Assert.Equal(flags.Find("svg").GetAttribute("class"), enums.Find("svg").GetAttribute("class"));
    }

    [Fact]
    public void A_variant_puts_its_classes_on_the_icon()
    {
        var cut = Render<FbIcon>(p => p.Add(x => x.Icon, FbMaterialIcons.Error).Add(x => x.Variant, FbThemeVariants.Error).Add(x => x.Highlight, true));

        Assert.Equal("fb-icon fb-highlight fb-variant fb-v-error", cut.Find("svg").GetAttribute("class"));
    }

    [Fact]
    public void Without_a_variant_the_icon_keeps_the_one_of_its_parent()
    {
        var cut = Render<FbIcon>(p => p.Add(x => x.Icon, FbMaterialIcons.Error).Add(x => x.Variant, FbThemeVariants.Error));

        cut.Render(p => p.Add(x => x.Icon, FbMaterialIcons.Error).Add(x => x.Variant, ""));

        Assert.Equal("fb-icon", cut.Find("svg").GetAttribute("class"));
    }
}
