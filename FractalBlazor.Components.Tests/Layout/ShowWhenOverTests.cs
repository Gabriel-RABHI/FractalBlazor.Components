using FractalBlazor.Components.Layout;

namespace FractalBlazor.Components.Tests.Layout;

/// <summary>
/// FbShowWhenOver lays its contents over its child content: only the contents catch the
/// pointer, the child content below stays reachable.
/// </summary>
public sealed class ShowWhenOverTests : BunitContext
{
    private static string LayoutStylesheet()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "FractalBlazor.Components.slnx")))
            directory = directory.Parent;
        return File.ReadAllText(Path.Combine(directory?.FullName ?? throw new DirectoryNotFoundException("Repository root not found."),
            "FractalBlazor.Components.Layout", "wwwroot", "fb-components-layout.css")).Replace("\r\n", "\n");
    }

    [Fact]
    public void Each_content_sits_in_a_placing_box_of_the_frame()
    {
        var cut = Render<FbShowWhenOver>(p => p
            .Add(x => x.TopRightContent, "Actions")
            .AddChildContent("Content"));

        var content = cut.Find(".fb-swo-frame > .fb-utils-fit-parent > div");
        Assert.Equal("Actions", content.TextContent);
        Assert.Contains("Content", cut.Markup);
    }

    [Fact]
    public void A_frame_shown_by_the_mouse_says_so()
    {
        var frame = Render<FbShowWhenOver>(p => p.Add(x => x.TopRightContent, "Actions")).Find(".fb-swo-frame");

        Assert.Contains("fb-swo-auto", frame.ClassList);
        Assert.Contains("visibility: collapse", frame.GetAttribute("style"));
    }

    [Fact]
    public void A_fixed_frame_is_always_shown()
    {
        var cut = Render<FbShowWhenOver>(p => p.Add(x => x.Fixed, true).Add(x => x.TopRightContent, "Actions"));
        var frame = cut.Find(".fb-swo-frame");

        Assert.True(cut.Instance.Fixed);
        Assert.DoesNotContain("fb-swo-auto", frame.ClassList);
        Assert.Contains("visibility: inherit", frame.GetAttribute("style"));
        Assert.Contains("'-'", cut.Find("div").GetAttribute("onmouseover"));
    }

    [Theory]
    [InlineData(".fb-swo-frame,\n.fb-swo-frame > .fb-utils-fit-parent {\n    pointer-events: none;")]
    [InlineData(".fb-swo-frame > .fb-utils-fit-parent > * {\n    pointer-events: auto;")]
    public void Only_the_contents_catch_the_pointer(string rule)
        => Assert.Contains(rule, LayoutStylesheet());
}
