using FractalBlazor.Components.Layout;

namespace FractalBlazor.Components.Tests.Layout;

/// <summary>
/// Separators (WSL, WSM, WSS) draw a line between the children of a row, a grid or a stack,
/// whatever their element: a button or a text is separated like an item.
/// </summary>
public sealed class SeparatorRuleTests
{
    private static string LayoutStylesheet()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "FractalBlazor.Components.slnx")))
            directory = directory.Parent;
        return File.ReadAllText(Path.Combine(directory?.FullName ?? throw new DirectoryNotFoundException("Repository root not found."),
            "FractalBlazor.Components.Layout", "wwwroot", "fb-components-layout.css"));
    }

    [Theory]
    [InlineData(":is(.fb-row, .fb-grid-container):is(.fb-wsl, .fb-wsm, .fb-wss) > :not(.fb-spacer):nth-last-child(n+2)")]
    [InlineData(".fb-stack:is(.fb-wsl, .fb-wsm, .fb-wss) > :not(.fb-spacer):nth-last-child(n+2)")]
    public void Separators_reach_every_child_but_the_last_and_the_spacers(string selector)
        => Assert.Contains(selector + " {", LayoutStylesheet());

    [Fact]
    public void No_separator_is_limited_to_div_children()
        => Assert.DoesNotContain("> div:nth-last-child(n+2)", LayoutStylesheet());

    [Fact]
    public void Spacers_say_what_they_are()
    {
        using var context = new BunitContext();

        Assert.Equal("fb-spacer", context.Render<FbRowSpacer>().Find("span").GetAttribute("class"));
        Assert.Equal("fb-spacer", context.Render<FbStackSpacer>().Find("span").GetAttribute("class"));
    }
}
