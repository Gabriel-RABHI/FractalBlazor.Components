using FractalBlazor.Components.Layout;
using FractalBlazor.Components.Layout.Utilities;

namespace FractalBlazor.Components.Tests.Layout;

public sealed class StyleCacheTests
{
    private sealed class ProbeRow : FbRow
    {
        public string Styles => AggregatedStyles;

        public string SurfaceClasses => AggregatedClasses;
    }

    [Fact]
    public void Identical_states_share_the_same_class_instance()
    {
        var first = new ProbeRow { WBA = true, WFM = true, Hover = true };
        var second = new ProbeRow { WBA = true, WFM = true, Hover = true };

        Assert.Same(first.SurfaceClasses, second.SurfaceClasses);
        Assert.Equal("fb-bg-accent fb-framed fb-framed-medium fb-hover", first.SurfaceClasses);
    }

    [Fact]
    public void Custom_classes_are_kept_and_never_shared()
    {
        var first = new ProbeRow { WBA = true, Classes = "custom" };
        var second = new ProbeRow { WBA = true, Classes = "custom" };

        Assert.Equal("custom fb-bg-accent", first.SurfaceClasses);
        Assert.NotSame(first.SurfaceClasses, second.SurfaceClasses);
    }

    [Fact]
    public void Identical_states_share_the_same_style_instance()
    {
        var first = new ProbeRow { PL = true, GM = true, WRM = true };
        var second = new ProbeRow { PL = true, GM = true, WRM = true };

        Assert.Same(first.Styles, second.Styles);
    }

    [Fact]
    public void States_that_differ_only_by_radius_never_share_a_style()
    {
        var large = new ProbeRow { PM = true, WRL = true };
        var small = new ProbeRow { PM = true, WRS = true };

        Assert.NotEqual(large.Styles, small.Styles);
        Assert.Contains("border-radius:var(--fb-radius-l)", large.Styles);
        Assert.Contains("border-radius:var(--fb-radius-s)", small.Styles);
    }

    [Fact]
    public void Free_form_styles_are_never_shared()
    {
        var first = new ProbeRow { Style = "color:red" };
        var second = new ProbeRow { Style = "color:red" };

        Assert.Equal(first.Styles, second.Styles);
        Assert.NotSame(first.Styles, second.Styles);
    }

    [Fact]
    public void Hide_changes_the_style()
    {
        var row = new ProbeRow { PS = true };
        var visible = row.Styles;

        row.Hide = true;

        Assert.DoesNotContain("visibility:hidden", visible);
        Assert.Contains("visibility:hidden", row.Styles);
    }

    [Fact]
    public void Unchanged_parameters_return_the_previous_string()
    {
        var row = new ProbeRow { Style = "color:red" };

        Assert.Same(row.Styles, row.Styles);
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1)]
    private record struct Sample(int A, byte B, long C);

    [Fact]
    public void State_equality_compares_every_byte()
    {
        var left = new Sample(1, 2, 3);
        var right = new Sample(1, 2, 3);
        var other = new Sample(1, 2, 4);

        Assert.True(FbState.Equal(in left, in right));
        Assert.False(FbState.Equal(in left, in other));
        Assert.Equal(FbState.Hash(in left), FbState.Hash(in right));
    }
}
