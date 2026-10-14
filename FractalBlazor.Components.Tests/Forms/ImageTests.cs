using FractalBlazor.Components.Forms.Components.Display;

namespace FractalBlazor.Components.Tests.Forms;

public sealed class ImageTests : BunitContext
{
    [Fact]
    public void An_image_without_alt_is_decorative()
    {
        var image = Render<FbImage>(p => p.Add(x => x.Src, "images/logo.png")).Find("img");

        Assert.Equal("images/logo.png", image.GetAttribute("src"));
        Assert.Equal("", image.GetAttribute("alt"));
        Assert.Equal("fb-image", image.GetAttribute("class"));
    }

    [Fact]
    public void Size_and_alt_reach_the_image()
    {
        var image = Render<FbImage>(p => p.Add(x => x.Src, "images/logo.png").Add(x => x.Alt, "FractalBlazor").Add(x => x.L, true))
            .Find("img");

        Assert.Equal("FractalBlazor", image.GetAttribute("alt"));
        Assert.Equal("fb-image fb-image-l", image.GetAttribute("class"));
    }

    [Fact]
    public void A_size_flag_does_not_stay_once_removed()
    {
        var cut = Render<FbImage>(p => p.Add(x => x.Src, "images/logo.png").Add(x => x.X, true));

        cut.Render(p => p.Add(x => x.Src, "images/logo.png"));

        Assert.Equal("fb-image", cut.Find("img").GetAttribute("class"));
    }
}
