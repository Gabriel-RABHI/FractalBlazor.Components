using FractalBlazor.Components.Layout;
using FractalBlazor.Components.Layout.Utilities;

namespace FractalBlazor.Components.Tests.Layout;

/// <summary>
/// Frames on one side, dotted frames and round corners: classes named after their flags.
/// </summary>
public sealed class SideFrameTests : BunitContext
{
    private AngleSharp.Dom.IElement RowOf(Action<ComponentParameterCollectionBuilder<FbRow>> parameters)
        => Render<FbRow>(parameters).Find("div");

    [Fact]
    public void Each_side_takes_its_own_strength()
    {
        var row = RowOf(p => p.Add(x => x.WFTL, true).Add(x => x.WFRM, true).Add(x => x.WFBS, true).Add(x => x.WFLL, true));

        Assert.Contains("fb-wftl", row.ClassList);
        Assert.Contains("fb-wfrm", row.ClassList);
        Assert.Contains("fb-wfbs", row.ClassList);
        Assert.Contains("fb-wfll", row.ClassList);
        Assert.DoesNotContain("fb-wfl", row.ClassList);
    }

    [Fact]
    public void Left_light_is_not_the_light_frame()
    {
        Assert.Contains("fb-wfl", RowOf(p => p.Add(x => x.WFL, true)).ClassList);
        Assert.DoesNotContain("fb-wfll", RowOf(p => p.Add(x => x.WFL, true)).ClassList);
    }

    [Fact]
    public void A_later_strength_replaces_an_earlier_one_on_the_same_side()
    {
        var row = RowOf(p => p.Add(x => x.WFRL, true).Add(x => x.WFRS, true));

        Assert.Contains("fb-wfrs", row.ClassList);
        Assert.DoesNotContain("fb-wfrl", row.ClassList);
    }

    [Fact]
    public void Dot_styles_the_frame()
        => Assert.Contains("fb-wfd", RowOf(p => p.Add(x => x.WFM, true).Add(x => x.WFD, true)).ClassList);

    [Fact]
    public void Round_replaces_the_radius_of_the_scale()
    {
        var row = RowOf(p => p.Add(x => x.WRM, true).Add(x => x.WRR, true));

        Assert.Contains("fb-wrr", row.ClassList);
        Assert.DoesNotContain(FbAtoms.NameOf("border-radius:var(--fb-radius-m)"), row.ClassList);
    }

    [Fact]
    public void A_radius_of_the_scale_written_after_round_replaces_it()
    {
        var row = RowOf(p => p.Add(x => x.WRR, true).Add(x => x.WRM, true));

        Assert.DoesNotContain("fb-wrr", row.ClassList);
        Assert.Contains(FbAtoms.NameOf("border-radius:var(--fb-radius-m)"), row.ClassList);
    }

    [Fact]
    public void Flags_no_longer_passed_fall_back_to_the_defaults()
    {
        var cut = Render<FbRow>(p => p.Add(x => x.WFTS, true).Add(x => x.WFD, true).Add(x => x.WRR, true));

        cut.Render(p => p.Add(x => x.WFTS, false));

        var row = cut.Find("div");
        Assert.DoesNotContain("fb-wfts", row.ClassList);
        Assert.DoesNotContain("fb-wfd", row.ClassList);
        Assert.DoesNotContain("fb-wrr", row.ClassList);
    }
}
