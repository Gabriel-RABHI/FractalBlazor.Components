using FractalBlazor.Components.Layout;
using FractalBlazor.Components.Layout.Utilities;

namespace FractalBlazor.Components.Tests.Layout;

public sealed class StyleCacheTests
{
    private sealed class ProbeRow : FbRow
    {
        public string? Styles => AggregatedStyles;
    }

    private static string Atom(string declaration) => FbAtoms.NameOf(declaration);

    [Fact]
    public void Identical_states_share_the_same_class_instance()
    {
        var first = new ProbeRow { WBA = true, WFM = true, Hover = true, PL = true };
        var second = new ProbeRow { WBA = true, WFM = true, Hover = true, PL = true };

        Assert.Same(first.RowClasses, second.RowClasses);
        Assert.Equal($"fb-row fb-wba fb-wfm fb-hover {Atom("padding:var(--fb-space-l)")}", first.RowClasses);
    }

    [Fact]
    public void Custom_classes_are_kept_and_never_shared()
    {
        var first = new ProbeRow { WBA = true, Classes = "custom" };
        var second = new ProbeRow { WBA = true, Classes = "custom" };

        Assert.Equal("fb-row fb-wba custom", first.RowClasses);
        Assert.NotSame(first.RowClasses, second.RowClasses);
    }

    [Fact]
    public void A_default_row_only_carries_its_base_class()
    {
        Assert.Equal("fb-row", new ProbeRow().RowClasses);
        Assert.Equal("fb-stack", new FbStack().StackClasses);
    }

    [Fact]
    public void A_row_only_adds_the_atoms_that_differ_from_its_base_rule()
    {
        var row = new ProbeRow { JVBl = true, GS = true, PVS = true, PHM = true, WRM = true, NoFlex = true };

        string[] expected =
        [
            "fb-row",
            Atom("flex:initial"),
            Atom("padding-block:var(--fb-space-s)"),
            Atom("padding-inline:var(--fb-space-m)"),
            Atom("border-radius:var(--fb-radius-m)"),
            Atom("align-items:baseline"),
            Atom("gap:var(--fb-space-s)")
        ];
        Assert.Equal(string.Join(' ', expected), row.RowClasses);
    }

    [Fact]
    public void Unequal_sides_get_one_atom_each()
    {
        var row = new ProbeRow { PTS = true, PBL = true };

        Assert.Equal($"fb-row {Atom("padding-top:var(--fb-space-s)")} {Atom("padding-bottom:var(--fb-space-l)")}", row.RowClasses);
    }

    [Fact]
    public void States_that_differ_only_by_radius_never_share_classes()
    {
        var large = new ProbeRow { PM = true, WRL = true };
        var small = new ProbeRow { PM = true, WRS = true };

        Assert.Contains(Atom("border-radius:var(--fb-radius-l)"), large.RowClasses.Split(' '));
        Assert.Contains(Atom("border-radius:var(--fb-radius-s)"), small.RowClasses.Split(' '));
    }

    [Fact]
    public void Rows_without_free_form_values_render_no_style()
    {
        Assert.Null(new ProbeRow { PL = true, GM = true, WRM = true, JHC = true }.Styles);
    }

    [Fact]
    public void Free_form_values_stay_inline()
    {
        var row = new ProbeRow { Style = "color:red", MinHeight = "5rem", Flex = 20 };

        Assert.Equal("flex:20;color:red;min-height:5rem;", row.Styles);
        Assert.Contains(Atom("flex:initial"), row.RowClasses.Split(' '));
    }

    [Fact]
    public void Width_bounds_stay_inline_and_a_new_bound_gives_a_new_style()
    {
        var row = new ProbeRow { MinWidth = "10rem", MaxWidth = "72rem" };
        Assert.Equal("min-width:10rem;max-width:72rem;", row.Styles);

        row.MaxWidth = "60rem";

        Assert.Equal("min-width:10rem;max-width:60rem;", row.Styles);
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
    public void Hide_changes_the_classes()
    {
        var row = new ProbeRow { PS = true };
        var visible = row.RowClasses;

        row.Hide = true;

        Assert.DoesNotContain(Atom("visibility:hidden"), visible.Split(' '));
        Assert.Contains(Atom("visibility:hidden"), row.RowClasses.Split(' '));
    }

    [Fact]
    public void Unchanged_parameters_return_the_previous_string()
    {
        var row = new ProbeRow { Style = "color:red" };

        Assert.Same(row.Styles, row.Styles);
        Assert.Same(row.RowClasses, row.RowClasses);
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
