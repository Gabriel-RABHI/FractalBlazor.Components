using FractalBlazor.Components.Layout.Colors;

namespace FractalBlazor.Components.Tests.Theming;

/// <summary>
/// The base palette: one column per shade, 980 and 990 darker than 950.
/// </summary>
public sealed class PaletteTests
{
    [Fact]
    public void Every_shade_has_its_column()
        => Assert.Equal(Enum.GetValues<FbThemeBaseShadesIndex>().Length, FbThemeBaseColors.FbBaseColors.GetLength(1));

    [Fact]
    public void A_shade_is_read_from_its_own_column()
    {
        Assert.Equal("#460809", FbThemeBaseColors.GetColor(FbThemeBaseColorsIndex.Red, FbThemeBaseShadesIndex._950));
        Assert.Equal("#FB2C36", FbThemeBaseColors.GetColor(FbThemeBaseColorsIndex.Red, FbThemeBaseShadesIndex._500));
        Assert.Equal("#FEF2F2", FbThemeBaseColors.GetColor(FbThemeBaseColorsIndex.Red, FbThemeBaseShadesIndex._50));
    }

    [Fact]
    public void Shades_980_and_990_go_on_darkening_every_color()
    {
        foreach (var color in Enum.GetValues<FbThemeBaseColorsIndex>())
        {
            var l950 = FbColorMath.Lightness(FbThemeBaseColors.GetColor(color, FbThemeBaseShadesIndex._950));
            var l980 = FbColorMath.Lightness(FbThemeBaseColors.GetColor(color, FbThemeBaseShadesIndex._980));
            var l990 = FbColorMath.Lightness(FbThemeBaseColors.GetColor(color, FbThemeBaseShadesIndex._990));

            Assert.True(l980 < l950, $"{color}: 980 is not darker than 950.");
            Assert.True(l990 < l980, $"{color}: 990 is not darker than 980.");
        }
    }
}
