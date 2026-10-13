namespace FractalBlazor.WebSite.StateModel.Tests;

public sealed class DrawerDemoTests
{
    private readonly DrawerDemoState _drawer = new();

    [Fact]
    public void Another_mode_closes_the_drawer()
    {
        _drawer.Handle(new PopupState.Toggle());

        _drawer.Handle(new DrawerDemoState.SetMode(DrawerMode.Modal));

        Assert.Equal(DrawerMode.Modal, _drawer.Mode);
        Assert.False(_drawer.IsOpen);
    }

    [Fact]
    public void Another_side_keeps_the_drawer_open()
    {
        _drawer.Handle(new PopupState.Toggle());

        _drawer.Handle(new DrawerDemoState.SetSide(DrawerSide.Top));

        Assert.Equal(DrawerSide.Top, _drawer.Side);
        Assert.False(_drawer.IsHorizontal);
        Assert.True(_drawer.IsOpen);
    }
}
