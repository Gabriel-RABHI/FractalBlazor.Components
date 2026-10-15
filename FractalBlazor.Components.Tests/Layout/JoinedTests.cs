using FractalBlazor.Components.Layout;

namespace FractalBlazor.Components.Tests.Layout;

/// <summary>
/// Joined: the children of a row or a stack form one group, whose ends take the outer corners
/// of the container.
/// </summary>
public sealed class JoinedTests : BunitContext
{
    private static string LayoutStylesheet()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "FractalBlazor.Components.slnx")))
            directory = directory.Parent;
        return File.ReadAllText(Path.Combine(directory?.FullName ?? throw new DirectoryNotFoundException("Repository root not found."),
            "FractalBlazor.Components.Layout", "wwwroot", "fb-components-layout.css"));
    }

    [Fact]
    public void A_joined_row_carries_its_class()
    {
        var row = Render<FbRow>(p => p.Add(x => x.Joined, true)).Find("div");

        Assert.Contains("fb-joined", row.ClassList);
        Assert.DoesNotContain("fb-reverse", row.ClassList);
    }

    [Fact]
    public void A_reversed_group_says_so()
    {
        Assert.Contains("fb-reverse", Render<FbRow>(p => p.Add(x => x.Joined, true).Add(x => x.Reverse, true)).Find("div").ClassList);
        Assert.Contains("fb-reverse", Render<FbStack>(p => p.Add(x => x.Joined, true).Add(x => x.Reverse, true)).Find("div").ClassList);
    }

    [Fact]
    public void Reverse_alone_adds_no_class()
        => Assert.DoesNotContain("fb-reverse", Render<FbRow>(p => p.Add(x => x.Reverse, true)).Find("div").ClassList);

    [Fact]
    public void A_group_no_longer_joined_loses_its_class()
    {
        var cut = Render<FbStack>(p => p.Add(x => x.Joined, true));

        cut.Render(p => p.Add(x => x.Joined, false));

        Assert.DoesNotContain("fb-joined", cut.Find("div").ClassList);
    }

    [Theory]
    [InlineData(".fb-row.fb-joined:not(.fb-reverse) > :first-child,\n.fb-row.fb-joined.fb-reverse > :last-child {\n    border-top-left-radius: inherit;\n    border-bottom-left-radius: inherit;")]
    [InlineData(".fb-row.fb-joined:not(.fb-reverse) > :last-child,\n.fb-row.fb-joined.fb-reverse > :first-child {\n    border-top-right-radius: inherit;\n    border-bottom-right-radius: inherit;")]
    [InlineData(".fb-stack.fb-joined:not(.fb-reverse) > :first-child,\n.fb-stack.fb-joined.fb-reverse > :last-child {\n    border-top-left-radius: inherit;\n    border-top-right-radius: inherit;")]
    [InlineData(".fb-stack.fb-joined:not(.fb-reverse) > :last-child,\n.fb-stack.fb-joined.fb-reverse > :first-child {\n    border-bottom-left-radius: inherit;\n    border-bottom-right-radius: inherit;")]
    public void The_ends_take_the_outer_corners_of_the_container(string rule)
        => Assert.Contains(rule, LayoutStylesheet().Replace("\r\n", "\n"));
}
