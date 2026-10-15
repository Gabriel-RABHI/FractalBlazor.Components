namespace FractalBlazor.WebSite.StateModel.Tests;

/// <summary>
/// The states of the Grid page: the playground keeps its item in the grid, the responsive
/// width finds its step, and the alignment writes the CSS of each axis.
/// </summary>
public sealed class GridDemoTests
{
    [Fact]
    public void The_playground_writes_its_code_and_its_css()
    {
        var grid = new GridPlaygroundState();

        Assert.Equal("<FbGridContainer Columns=\"6\" GM>", grid.ContainerTag);
        Assert.Equal("<FbGridItem ColumnSpan=\"3\" ColumnStart=\"1\" RowSpan=\"2\">", grid.ItemTag);
        Assert.Equal(
            "grid-template-columns: repeat(6, minmax(0, 1fr));\ngap: var(--fb-space-m);\n/* A */ grid-column: 1 / span 3;\n/* A */ grid-row: auto / span 2;",
            grid.Css);

        grid.Handle(new GridPlaygroundState.SetDense(true));
        grid.Handle(new GridPlaygroundState.SetGap(FlexGutter.None));
        grid.Handle(new GridPlaygroundState.SetStart(0));
        grid.Handle(new GridPlaygroundState.SetRowSpan(1));

        Assert.Equal("<FbGridContainer Columns=\"6\" FRD>", grid.ContainerTag);
        Assert.Equal("<FbGridItem ColumnSpan=\"3\">", grid.ItemTag);
        Assert.Contains("grid-auto-flow: row dense;", grid.Css);
        Assert.Contains("/* A */ grid-column: auto / span 3;", grid.Css);
    }

    [Fact]
    public void The_item_stays_in_the_grid()
    {
        var grid = new GridPlaygroundState();

        grid.Handle(new GridPlaygroundState.SetStart(4));
        Assert.Equal((4, 4), (grid.Start, grid.MaxStart));

        grid.Handle(new GridPlaygroundState.SetSpan(5));
        Assert.Equal((5, 2), (grid.Span, grid.Start));

        grid.Handle(new GridPlaygroundState.SetColumns(3));
        Assert.Equal((3, 3, 1), (grid.Columns, grid.Span, grid.Start));

        grid.Handle(new GridPlaygroundState.SetStart(9));
        grid.Handle(new GridPlaygroundState.SetRowSpan(8));
        Assert.Equal((1, GridPlaygroundState.MaxRowSpan), (grid.Start, grid.RowSpan));
    }

    [Fact]
    public void A_change_notifies_once_and_the_same_value_does_not()
    {
        var grid = new GridPlaygroundState();
        var notifications = 0;
        grid.OnStateChanged += () => notifications++;

        grid.Handle(new GridPlaygroundState.SetColumns(8));
        grid.Handle(new GridPlaygroundState.SetColumns(8));
        grid.Handle(new GridPlaygroundState.SetGap(FlexGutter.M));

        Assert.Equal(1, notifications);
    }

    [Theory]
    [InlineData(300, "Base", 1)]
    [InlineData(512, "XS", 2)]
    [InlineData(700, "S", 3)]
    [InlineData(1000, "M", 4)]
    [InlineData(5000, "L", 6)]
    public void A_width_reaches_the_widest_step_below_it(int width, string step, int columns)
    {
        var responsive = new GridResponsiveState();

        responsive.Handle(new GridResponsiveState.SetWidth(width));

        Assert.Equal((step, columns), (responsive.Step.Name, responsive.Step.Columns));
        Assert.InRange(responsive.Width, GridResponsiveState.MinWidth, GridResponsiveState.MaxWidth);
    }

    [Fact]
    public void The_alignment_names_the_screen_axes_and_writes_grid_css()
    {
        var alignment = new GridAlignmentState();

        Assert.Equal("JHC JVE", alignment.Flags);
        Assert.Equal("justify-items: center", alignment.Declaration(FlexAxis.Horizontal));
        Assert.Equal("align-items: end", alignment.Declaration(FlexAxis.Vertical));

        alignment.Handle(new GridAlignmentState.SetHorizontal(FlexAlignment.Stretch));
        alignment.Handle(new GridAlignmentState.SetVertical(FlexAlignment.SpaceBetween));

        Assert.Equal("JHSt JVE", alignment.Flags);
        Assert.Equal("justify-items: stretch", alignment.Declaration(FlexAxis.Horizontal));
    }
}
