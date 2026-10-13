using System.Text.RegularExpressions;
using FractalBlazor.Components.Common.Contracts;

namespace FractalBlazor.Components.Tests.Common;

public sealed partial class WidthClassTests
{
    [Theory]
    [InlineData(0, FbWidthClass.Base)]
    [InlineData(511.9, FbWidthClass.Base)]
    [InlineData(512, FbWidthClass.XS)]
    [InlineData(767.5, FbWidthClass.S)]
    [InlineData(768, FbWidthClass.M)]
    [InlineData(1535, FbWidthClass.XL)]
    [InlineData(1536, FbWidthClass.XXL)]
    [InlineData(4000, FbWidthClass.XXL)]
    public void A_width_falls_in_the_category_of_the_largest_breakpoint_it_reaches(double width, FbWidthClass expected)
        => Assert.Equal(expected, FbWidthClasses.Of(width));

    [Fact]
    public void Categories_are_ordered_like_their_widths()
    {
        Assert.True(FbWidthClass.Base < FbWidthClass.XS);
        Assert.True(FbWidthClass.M < FbWidthClass.L);
        Assert.Equal(Enum.GetValues<FbWidthClass>().Length - 1, FbWidthClasses.Thresholds.Count);
    }

    [Fact]
    public void Thresholds_are_the_breakpoints_of_the_layout_stylesheet()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "FractalBlazor.Components.slnx")))
            directory = directory.Parent;
        var css = File.ReadAllText(Path.Combine(directory!.FullName, "FractalBlazor.Components.Layout", "wwwroot", "fb-components-layout.css"));

        var breakpoints = ContainerQuery().Matches(css).Select(match => int.Parse(match.Groups[1].Value)).ToHashSet();

        Assert.Equal(breakpoints.Order(), FbWidthClasses.Thresholds);
    }

    [GeneratedRegex(@"@container \(min-width: (\d+)px\)")]
    private static partial Regex ContainerQuery();
}
