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
        => _root.Header.Sections.SelectMany(s => s.Entries).OfType<PageLinkState>().Single(l => l.Intent == intent);

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
        Assert.False(Screen.MenuInDrawer);
        Assert.False(Screen.CompactHeader);
        Assert.False(Drawer.IsOpen);
        Assert.False(HeaderMenu.IsOpen);
    }

    [Theory]
    [InlineData(FbWidthClass.XXL, false, false)]
    [InlineData(FbWidthClass.L, false, false)]
    [InlineData(FbWidthClass.M, false, false)]
    [InlineData(FbWidthClass.S, true, false)]
    [InlineData(FbWidthClass.XS, true, true)]
    [InlineData(FbWidthClass.Base, true, true)]
    public void The_menu_moves_into_a_drawer_under_M_and_the_header_shrinks_under_S(FbWidthClass width, bool menuInDrawer, bool compactHeader)
    {
        Resize(width);

        Assert.Equal(width, Screen.Width);
        Assert.Equal(menuInDrawer, Screen.MenuInDrawer);
        Assert.Equal(compactHeader, Screen.CompactHeader);
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

        Link(MenuItemIntent.StateFirst).Handle(new SelectableEntryState.Select());

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

        Section(MenuItemIntent.Themes).Handle(new SelectableEntryState.Select());

        Assert.False(HeaderMenu.IsOpen);
        Assert.IsType<VariantsPage>(_root.Navigation.Content.Page);
    }

    [Fact]
    public void Widening_the_screen_closes_the_menus_it_no_longer_shows()
    {
        Resize(FbWidthClass.XS);
        Drawer.Handle(new PopupState.Toggle());
        HeaderMenu.Handle(new PopupState.Toggle());

        Resize(FbWidthClass.S);
        Assert.True(Drawer.IsOpen);
        Assert.False(HeaderMenu.IsOpen);

        Resize(FbWidthClass.L);
        Assert.False(Drawer.IsOpen);
    }
}
