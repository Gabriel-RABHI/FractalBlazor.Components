namespace FractalBlazor.WebSite.StateModel.Tests;

/// <summary>
/// The state of the badge demo: messages that arrive and are read, a presence that changes.
/// </summary>
public sealed class BadgeDemoTests
{
    [Fact]
    public void Messages_arrive_and_are_all_read_at_once()
    {
        var demo = new BadgeDemoState();
        var notifications = 0;
        demo.OnStateChanged += () => notifications++;

        demo.Handle(new BadgeDemoState.Receive());
        demo.Handle(new BadgeDemoState.ReadAll());
        demo.Handle(new BadgeDemoState.ReadAll());

        Assert.Equal((0, 2), (demo.Unread, notifications));
    }

    [Fact]
    public void The_presence_goes_off_and_on()
    {
        var demo = new BadgeDemoState();

        demo.Handle(new BadgeDemoState.ToggleOnline());

        Assert.False(demo.Online);
    }
}
