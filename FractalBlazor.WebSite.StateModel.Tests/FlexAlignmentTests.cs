namespace FractalBlazor.WebSite.StateModel.Tests;

/// <summary>
/// The alignment flags name the axes of the screen: a row and a stack take the same flags and
/// only write them to different properties.
/// </summary>
public sealed class FlexAlignmentTests
{
    [Fact]
    public void The_same_flags_write_swapped_properties()
    {
        var pad = new AlignmentPadState();

        pad.Handle(new AlignmentPadState.SetPosition(FlexAlignment.Center, FlexAlignment.End));

        Assert.Equal("<FbRow JHC JVE>", pad.Code(FlexContainer.Row));
        Assert.Equal("<FbStack JHC JVE>", pad.Code(FlexContainer.Stack));
        Assert.Equal("justify-content: center", pad.Declaration(FlexContainer.Row, FlexAxis.Horizontal));
        Assert.Equal("align-items: flex-end", pad.Declaration(FlexContainer.Row, FlexAxis.Vertical));
        Assert.Equal("align-items: center", pad.Declaration(FlexContainer.Stack, FlexAxis.Horizontal));
        Assert.Equal("justify-content: flex-end", pad.Declaration(FlexContainer.Stack, FlexAxis.Vertical));
    }

    [Fact]
    public void The_pad_moves_once_and_only_inside()
    {
        var pad = new AlignmentPadState();
        var notifications = 0;
        pad.OnStateChanged += () => notifications++;

        pad.Handle(new AlignmentPadState.SetPosition(FlexAlignment.Start, FlexAlignment.Start));
        pad.Handle(new AlignmentPadState.SetPosition(FlexAlignment.Start, FlexAlignment.Start));
        pad.Handle(new AlignmentPadState.SetPosition(FlexAlignment.SpaceBetween, FlexAlignment.Start));

        Assert.True(pad.IsAt(FlexAlignment.Start, FlexAlignment.Start));
        Assert.Equal(1, notifications);
    }

    [Fact]
    public void The_flow_axis_shares_the_space_out_and_the_cross_axis_stretches()
    {
        Assert.Equal(FlexAxis.Horizontal, FlexAlignments.FlowAxis(FlexContainer.Row));
        Assert.Equal(FlexAxis.Vertical, FlexAlignments.FlowAxis(FlexContainer.Stack));

        var rowFlow = FlexAlignments.Choices(FlexContainer.Row, FlexAxis.Horizontal);
        var stackFlow = FlexAlignments.Choices(FlexContainer.Stack, FlexAxis.Vertical);
        Assert.Equal(rowFlow, stackFlow);
        Assert.Contains(FlexAlignment.SpaceEvenly, rowFlow);
        Assert.DoesNotContain(FlexAlignment.Stretch, rowFlow);

        Assert.Contains(FlexAlignment.Stretch, FlexAlignments.Choices(FlexContainer.Row, FlexAxis.Vertical));
        Assert.Contains(FlexAlignment.Baseline, FlexAlignments.Choices(FlexContainer.Row, FlexAxis.Vertical));
        Assert.Contains(FlexAlignment.Stretch, FlexAlignments.Choices(FlexContainer.Stack, FlexAxis.Horizontal));
        Assert.DoesNotContain(FlexAlignment.Baseline, FlexAlignments.Choices(FlexContainer.Stack, FlexAxis.Horizontal));
        Assert.DoesNotContain(FlexAlignment.SpaceBetween, FlexAlignments.Choices(FlexContainer.Stack, FlexAxis.Horizontal));
    }

    [Theory]
    [InlineData(FlexAlignment.Start, "JHS", "JVS")]
    [InlineData(FlexAlignment.Stretch, "JHSt", "JVSt")]
    [InlineData(FlexAlignment.Baseline, "JHBl", "JVBl")]
    [InlineData(FlexAlignment.SpaceBetween, "JHSB", "JVSB")]
    [InlineData(FlexAlignment.SpaceAround, "JHSA", "JVSA")]
    [InlineData(FlexAlignment.SpaceEvenly, "JHEv", "JVEv")]
    public void A_flag_is_the_axis_then_the_alignment(FlexAlignment alignment, string horizontal, string vertical)
    {
        Assert.Equal(horizontal, FlexAlignments.Flag(FlexAxis.Horizontal, alignment));
        Assert.Equal(vertical, FlexAlignments.Flag(FlexAxis.Vertical, alignment));
    }

    [Fact]
    public void The_playground_writes_its_code_and_its_css()
    {
        var playground = new FlexPlaygroundState();

        playground.Handle(new FlexPlaygroundState.SetContainer(FlexContainer.Stack));
        playground.Handle(new FlexPlaygroundState.SetHorizontal(FlexAlignment.End));
        playground.Handle(new FlexPlaygroundState.SetVertical(FlexAlignment.SpaceBetween));
        playground.Handle(new FlexPlaygroundState.SetGutter(FlexGutter.L));

        Assert.Equal("<FbStack JHE JVSB GL>\n    …\n</FbStack>", playground.Code);
        Assert.Equal("align-items: flex-end;\njustify-content: space-between;\ngap: var(--fb-space-l);", playground.Css);
    }

    [Fact]
    public void Equal_alignments_take_the_shortcut()
    {
        var playground = new FlexPlaygroundState();

        playground.Handle(new FlexPlaygroundState.SetHorizontal(FlexAlignment.Center));
        playground.Handle(new FlexPlaygroundState.SetGutter(FlexGutter.None));

        Assert.Equal("<FbRow JC>", playground.Tag);
        Assert.Equal("justify-content: center;\nalign-items: center;", playground.Css);
    }

    [Fact]
    public void An_alignment_the_axis_does_not_offer_is_ignored()
    {
        var playground = new FlexPlaygroundState();
        var notifications = 0;
        playground.OnStateChanged += () => notifications++;

        playground.Handle(new FlexPlaygroundState.SetVertical(FlexAlignment.SpaceBetween));
        playground.Handle(new FlexPlaygroundState.SetHorizontal(FlexAlignment.Stretch));

        Assert.Equal(FlexAlignment.Start, playground.Horizontal);
        Assert.Equal(FlexAlignment.Center, playground.Vertical);
        Assert.Equal(0, notifications);
    }

    [Fact]
    public void The_other_container_keeps_what_it_offers()
    {
        var playground = new FlexPlaygroundState();
        playground.Handle(new FlexPlaygroundState.SetHorizontal(FlexAlignment.SpaceBetween));
        playground.Handle(new FlexPlaygroundState.SetVertical(FlexAlignment.End));

        playground.Handle(new FlexPlaygroundState.SetContainer(FlexContainer.Stack));

        Assert.Equal(FlexAlignment.Start, playground.Horizontal);
        Assert.Equal(FlexAlignment.End, playground.Vertical);

        playground.Handle(new FlexPlaygroundState.SetHorizontal(FlexAlignment.Stretch));
        playground.Handle(new FlexPlaygroundState.SetContainer(FlexContainer.Row));

        Assert.Equal(FlexAlignment.Start, playground.Horizontal);
        Assert.Equal(FlexAlignment.End, playground.Vertical);
    }
}
