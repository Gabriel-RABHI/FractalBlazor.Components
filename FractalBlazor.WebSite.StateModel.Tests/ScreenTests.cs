using FractalBlazor.Components.Common.Contracts;

namespace FractalBlazor.WebSite.StateModel.Tests;

/// <summary>
/// The layout follows the width the view sends: these tests narrow and widen the screen
/// without any interface.
/// </summary>
public sealed class ScreenTests
{
    private readonly RootState _root = new(new RecordingThemeModeStore());

    private ScreenState Screen => _root.Screen;

    private MenuDrawerState Drawer => _root.Navigation.Drawer;

    private HeaderMenuState HeaderMenu => _root.Header.Menu;

    private void Resize(FbWidthClass width) => Screen.Handle(new ScreenState.SetWidth(width));

    private PageLinkState Link(MenuItemIntent intent)
        => _root.Header.Sections.SelectMany(s => s.Pages).Single(l => l.Intent == intent);

    private SectionState Section(MenuItemIntent intent) => _root.Header.Sections.Single(s => s.Intent == intent);

    private static Func<int> CountNotifications(INotifyStateChanged state)
    {
        var count = 0;
        state.OnStateChanged += () => count++;
        return () => count;
    }

    [Fact]
    public void Before_any_measure_the_screen_is_laid_out_for_a_desktop()
    {
        Assert.False(Screen.Compact);
        Assert.False(Drawer.IsOpen);
        Assert.False(HeaderMenu.IsOpen);
    }

    [Theory]
    [InlineData(FbWidthClass.XXL, false)]
    [InlineData(FbWidthClass.XL, false)]
    [InlineData(FbWidthClass.L, false)]
    [InlineData(FbWidthClass.M, true)]
    [InlineData(FbWidthClass.S, true)]
    [InlineData(FbWidthClass.Base, true)]
    public void Both_menus_fold_under_1024_px(FbWidthClass width, bool compact)
    {
        Resize(width);

        Assert.Equal(width, Screen.Width);
        Assert.Equal(compact, Screen.Compact);
    }

    [Fact]
    public void Only_a_new_width_notifies_the_screen_and_the_application()
    {
        var screen = CountNotifications(Screen);
        var root = CountNotifications(_root);

        Resize(FbWidthClass.S);
        Resize(FbWidthClass.S);

        Assert.Equal(1, screen());
        Assert.Equal(1, root());
    }

    [Fact]
    public void The_menu_button_toggles_the_drawer()
    {
        Resize(FbWidthClass.S);
        var notifications = CountNotifications(Drawer);

        Drawer.Handle(new PopupState.Toggle());
        Assert.True(Drawer.IsOpen);

        Drawer.Handle(new PopupState.Toggle());
        Assert.False(Drawer.IsOpen);
        Assert.Equal(2, notifications());
    }

    [Fact]
    public void A_dismissed_drawer_closes_and_notifies_only_its_view()
    {
        Resize(FbWidthClass.S);
        Drawer.Handle(new PopupState.Toggle());
        var drawer = CountNotifications(Drawer);
        var root = CountNotifications(_root);

        Drawer.Handle(new PopupState.SetOpen(false));
        Drawer.Handle(new PopupState.SetOpen(false));

        Assert.False(Drawer.IsOpen);
        Assert.Equal(1, drawer());
        Assert.Equal(0, root());
    }

    [Fact]
    public void Opening_a_page_closes_the_drawer()
    {
        Resize(FbWidthClass.S);
        Drawer.Handle(new PopupState.Toggle());

        Link(MenuItemIntent.Principles).Handle(new SelectableEntryState.Select());

        Assert.False(Drawer.IsOpen);
        Assert.IsType<StateFirstPage>(_root.Navigation.Content.Page);
    }

    [Fact]
    public void Choosing_the_page_already_open_closes_the_drawer_too()
    {
        Resize(FbWidthClass.S);
        Drawer.Handle(new PopupState.Toggle());

        Link(MenuItemIntent.Introduction).Handle(new SelectableEntryState.Select());

        Assert.False(Drawer.IsOpen);
    }

    [Fact]
    public void Choosing_a_section_closes_the_menu_of_the_header()
    {
        Resize(FbWidthClass.XS);
        HeaderMenu.Handle(new PopupState.Toggle());

        Section(MenuItemIntent.Components).Handle(new SelectableEntryState.Select());

        Assert.False(HeaderMenu.IsOpen);
        Assert.IsType<RowStackPage>(_root.Navigation.Content.Page);
    }

    [Fact]
    public void The_drawer_follows_the_menu_it_carries()
    {
        var drawer = CountNotifications(Drawer);

        Section(MenuItemIntent.Compositions).Handle(new SelectableEntryState.Select());
        Assert.False(Drawer.HasMenu);

        Section(MenuItemIntent.Components).Handle(new SelectableEntryState.Select());
        Assert.True(Drawer.HasMenu);
        Assert.Equal(2, drawer());
    }

    [Fact]
    public void Widening_the_screen_closes_the_menus_it_no_longer_shows()
    {
        Resize(FbWidthClass.S);
        Drawer.Handle(new PopupState.Toggle());
        HeaderMenu.Handle(new PopupState.Toggle());

        Resize(FbWidthClass.M);
        Assert.True(Drawer.IsOpen);
        Assert.True(HeaderMenu.IsOpen);

        Resize(FbWidthClass.L);
        Assert.False(Drawer.IsOpen);
        Assert.False(HeaderMenu.IsOpen);
    }
}
