using FractalBlazor.Components.Forms.Components.Display;

namespace FractalBlazor.Components.Tests.Forms;

public sealed class IconImageTests : BunitContext
{
    [Fact]
    public void An_image_without_alt_is_decorative()
    {
        var image = Render<FbIconImage>(p => p.Add(x => x.Src, "images/logo.png")).Find("img");

        Assert.Equal("images/logo.png", image.GetAttribute("src"));
        Assert.Equal("", image.GetAttribute("alt"));
        Assert.Equal("fb-icon-image", image.GetAttribute("class"));
    }

    [Fact]
    public void Size_and_alt_reach_the_image()
    {
        var image = Render<FbIconImage>(p => p.Add(x => x.Src, "images/logo.png").Add(x => x.Alt, "FractalBlazor").Add(x => x.L, true))
            .Find("img");

        Assert.Equal("FractalBlazor", image.GetAttribute("alt"));
        Assert.Equal("fb-icon-image fb-icon-image-l", image.GetAttribute("class"));
    }

    [Fact]
    public void A_size_flag_does_not_stay_once_removed()
    {
        var cut = Render<FbIconImage>(p => p.Add(x => x.Src, "images/logo.png").Add(x => x.X, true));

        cut.Render(p => p.Add(x => x.Src, "images/logo.png"));

        Assert.Equal("fb-icon-image", cut.Find("img").GetAttribute("class"));
    }
}
