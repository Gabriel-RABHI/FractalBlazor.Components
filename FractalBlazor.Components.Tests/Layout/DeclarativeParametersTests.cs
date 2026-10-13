using FractalBlazor.Components.Layout;
using FractalBlazor.Components.Layout.Utilities;

namespace FractalBlazor.Components.Tests.Layout;

/// <summary>
/// A boolean flag set to <c>false</c>, or no longer passed, must fall back to the
/// component default instead of keeping the value of a previous render.
/// </summary>
public sealed class DeclarativeParametersTests : BunitContext
{
    [Fact]
    public void Background_flag_turned_off_removes_the_background_class()
    {
        var cut = Render<FbRow>(p => p.Add(x => x.WBA, true));
        Assert.Contains("fb-wba", cut.Find("div").ClassList);

        cut.Render(p => p.Add(x => x.WBA, false));

        Assert.DoesNotContain("fb-wba", cut.Find("div").ClassList);
    }

    [Fact]
    public void Frame_flag_turned_off_removes_the_frame_class()
    {
        var cut = Render<FbStack>(p => p.Add(x => x.WFS, true));
        Assert.Contains("fb-wfs", cut.Find("div").ClassList);

        cut.Render(p => p.Add(x => x.WFS, false));

        Assert.DoesNotContain("fb-wfs", cut.Find("div").ClassList);
    }

    [Fact]
    public void Constructor_defaults_come_back_after_an_override()
    {
        var cut = Render<FbRow>(p => p.Add(x => x.JHC, true));
        Assert.Contains(FbAtoms.NameOf("justify-content:center"), cut.Find("div").ClassList);

        cut.Render(p => p.Add(x => x.JHC, false));

        Assert.Equal(["fb-row"], cut.Find("div").ClassList);
    }

    [Fact]
    public void Spacing_flag_turned_off_removes_the_padding()
    {
        var cut = Render<FbRow>(p => p.Add(x => x.PL, true));
        Assert.Contains(FbAtoms.NameOf("padding:var(--fb-space-l)"), cut.Find("div").ClassList);

        cut.Render(p => p.Add(x => x.PL, false));

        Assert.DoesNotContain(FbAtoms.NameOf("padding:var(--fb-space-l)"), cut.Find("div").ClassList);
    }

    [Fact]
    public void Outline_follows_the_flag()
    {
        var cut = Render<FbRow>(p => p.Add(x => x.EO, true));
        Assert.Contains("fb-eo", cut.Find("div").ClassList);

        cut.Render(p => p.Add(x => x.EO, false));

        Assert.DoesNotContain("fb-eo", cut.Find("div").ClassList);
    }

    [Fact]
    public void Separator_default_frame_comes_back()
    {
        var cut = Render<FbStackSeparator>(p => p.Add(x => x.WFL, true));
        Assert.Contains("fb-separator-light-frame", cut.Find("div").ClassList);

        cut.Render(p => p.Add(x => x.WFL, false));

        Assert.Contains("fb-separator-medium-frame", cut.Find("div").ClassList);
    }

    [Fact]
    public void Grid_gap_flag_turned_off_removes_the_gap()
    {
        var cut = Render<FbGridContainer>(p => p.Add(x => x.GM, true));
        Assert.Contains("--fb-grid-gap:", cut.Find("div").GetAttribute("style"));

        cut.Render(p => p.Add(x => x.GM, false));

        Assert.DoesNotContain("--fb-grid-gap:", cut.Find("div").GetAttribute("style"));
    }
}
