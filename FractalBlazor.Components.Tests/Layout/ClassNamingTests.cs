using FractalBlazor.Components.Layout;

namespace FractalBlazor.Components.Tests.Layout;

/// <summary>
/// A class set by a flag is named after it: the DOM reads like the markup that produced it.
/// </summary>
public sealed class ClassNamingTests : BunitContext
{
    [Fact]
    public void Surface_flags_give_their_name_to_their_class()
    {
        var cut = Render<FbRow>(p => p
            .Add(x => x.WBA, true)
            .Add(x => x.WFM, true)
            .Add(x => x.WSL, true)
            .Add(x => x.EO, true)
            .Add(x => x.WNTW, true));

        var classes = cut.Find("div").ClassList;
        Assert.Contains("fb-wba", classes);
        Assert.Contains("fb-wfm", classes);
        Assert.Contains("fb-wsl", classes);
        Assert.Contains("fb-eo", classes);
        Assert.Contains("fb-wntw", classes);
    }

    [Fact]
    public void Responsive_flags_give_their_name_to_their_class()
    {
        var over = Render<FbStack>(p => p.Add(x => x.VO_XL, true));
        var under = Render<FbStack>(p => p.Add(x => x.VU_XL, true));

        Assert.Contains("fb-vo-xl", over.Find("div").ClassList);
        Assert.Contains("fb-vu-xl", under.Find("div").ClassList);
    }
}
