namespace FractalBlazor.WebSite.StateModel.Tests;

/// <summary>
/// The states of the progress demos: the share a slider sets, and a loading that stops and
/// starts again.
/// </summary>
public sealed class ProgressDemoTests
{
    [Fact]
    public void A_new_share_notifies_once_and_the_same_share_does_not()
    {
        var linear = new LinearProgressDemoState();
        var circular = new CircularProgressDemoState();
        var notifications = 0;
        linear.OnStateChanged += () => notifications++;

        linear.Handle(new LinearProgressDemoState.SetDone(30));
        linear.Handle(new LinearProgressDemoState.SetDone(30));
        circular.Handle(new CircularProgressDemoState.SetDone(80));

        Assert.Equal((30, 80, 1), (linear.Done, circular.Done, notifications));
    }

    [Fact]
    public void Loading_stops_and_starts_again()
    {
        var loading = new IndeterminateProgressDemoState();

        Assert.True(loading.Loading);
        loading.Handle(new IndeterminateProgressDemoState.ToggleLoading());
        Assert.False(loading.Loading);
        loading.Handle(new IndeterminateProgressDemoState.ToggleLoading());
        Assert.True(loading.Loading);
    }
}
