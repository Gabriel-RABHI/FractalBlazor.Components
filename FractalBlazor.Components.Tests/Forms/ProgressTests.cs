using System.Globalization;
using FractalBlazor.Components.Forms.Components.Display;
using FractalBlazor.Components.Forms.Theming.Constants;
using FractalBlazor.Components.Layout.Theming.Model;

namespace FractalBlazor.Components.Tests.Forms;

/// <summary>
/// The progress of the library: a bar, horizontal or vertical, or a ring, filled up to the
/// share done, or indeterminate without a value.
/// </summary>
public sealed class ProgressTests : BunitContext
{
    private IRenderedComponent<FbProgress> Progress(Action<ComponentParameterCollectionBuilder<FbProgress>> parameters)
        => Render<FbProgress>(parameters);

    [Fact]
    public void A_bar_is_filled_up_to_the_share_done()
    {
        var root = Progress(p => p.Add(x => x.Value, 62).Add(x => x.Title, "Import")).Find(".fb-progress");

        Assert.Equal("fb-progress", root.GetAttribute("class"));
        Assert.Equal("--fb-progress-value:62;", root.GetAttribute("style"));
        Assert.Equal(("progressbar", "Import", "0", "100", "62"),
            (root.GetAttribute("role"), root.GetAttribute("aria-label"), root.GetAttribute("aria-valuemin"),
             root.GetAttribute("aria-valuemax"), root.GetAttribute("aria-valuenow")));
        Assert.Equal(0.62.ToString("P0", CultureInfo.CurrentCulture), root.GetAttribute("aria-valuetext"));
        Assert.NotNull(root.QuerySelector(".fb-progress-track > .fb-progress-fill"));
        Assert.Null(root.QuerySelector(".fb-progress-label"));
    }

    [Theory]
    [InlineData(3, 12, "25")]
    [InlineData(20, 10, "100")]
    [InlineData(-4, 10, "0")]
    [InlineData(5, 0, "0")]
    public void The_share_is_counted_on_max_and_kept_between_0_and_100(double value, double max, string expected)
    {
        var root = Progress(p => p.Add(x => x.Value, value).Add(x => x.Max, max)).Find(".fb-progress");

        Assert.Equal($"--fb-progress-value:{expected};", root.GetAttribute("style"));
    }

    [Fact]
    public void Without_a_value_the_progress_is_indeterminate()
    {
        var root = Progress(p => p.Add(x => x.VL, true)).Find(".fb-progress");

        Assert.Contains("fb-progress-indeterminate", root.ClassList);
        Assert.False(root.HasAttribute("aria-valuenow"));
        Assert.False(root.HasAttribute("aria-valuetext"));
        Assert.False(root.HasAttribute("style"));
        Assert.Null(root.QuerySelector(".fb-progress-label"));
    }

    [Fact]
    public void A_ring_draws_two_circles_that_count_100_along_their_length()
    {
        var root = Progress(p => p.Add(x => x.Value, 40).Add(x => x.Circular, true).Add(x => x.Size, "6rem")).Find(".fb-progress");

        Assert.Contains("fb-progress-circular", root.ClassList);
        Assert.Equal("--fb-progress-value:40;--fb-progress-size:6rem;", root.GetAttribute("style"));
        var circles = root.QuerySelectorAll("svg.fb-progress-ring circle");
        Assert.Equal(["fb-progress-ring-track", "fb-progress-ring-fill"], circles.Select(circle => circle.GetAttribute("class")));
        Assert.All(circles, circle => Assert.Equal("100", circle.GetAttribute("pathLength")));
        Assert.Null(root.QuerySelector(".fb-progress-track"));
    }

    [Fact]
    public void A_vertical_bar_says_so_and_takes_its_thickness()
    {
        var root = Progress(p => p.Add(x => x.Value, 10).Add(x => x.Vertical, true).Add(x => x.Thickness, "4px")).Find(".fb-progress");

        Assert.Contains("fb-progress-vertical", root.ClassList);
        Assert.Equal("--fb-progress-value:10;--fb-progress-thickness:4px;", root.GetAttribute("style"));
    }

    [Fact]
    public void The_label_shows_the_share_in_its_format_or_the_content_given()
    {
        var formatted = Progress(p => p.Add(x => x.Value, 1).Add(x => x.Max, 3).Add(x => x.VL, true).Add(x => x.Format, "P1"));
        var content = Progress(p => p.Add(x => x.Value, 1).Add(x => x.Max, 3).Add(x => x.Circular, true)
            .AddChildContent("<b>1 / 3</b>"));

        Assert.Equal((1d / 3).ToString("P1", CultureInfo.CurrentCulture), formatted.Find(".fb-progress-label").TextContent);
        Assert.Equal("1 / 3", content.Find(".fb-progress-label b").TextContent);
    }

    [Fact]
    public void A_variant_colors_the_progress_on_its_root()
    {
        var root = Progress(p => p.Add(x => x.Value, 50).Add(x => x.Variant, FbThemeVariants.Success).Add(x => x.Classes, "mine")).Find(".fb-progress");

        Assert.Equal("fb-progress mine " + FbThemeCssNames.VariantClasses(FbThemeVariants.Success), root.GetAttribute("class"));
    }

    [Fact]
    public void A_flag_no_longer_passed_falls_back()
    {
        var cut = Progress(p => p.Add(x => x.Value, 50).Add(x => x.Circular, true).Add(x => x.VL, true));

        cut.Render(p => p.Add(x => x.Value, 60));

        Assert.Equal("fb-progress", cut.Find(".fb-progress").GetAttribute("class"));
        Assert.Empty(cut.FindAll(".fb-progress-label"));
    }
}
