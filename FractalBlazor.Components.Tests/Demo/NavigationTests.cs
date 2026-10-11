using FractalBlazor.Components.Common.Contracts;
using FractalBlazor.WebSite.States;

namespace FractalBlazor.Components.Tests.Demo;

/// <summary>
/// The demo application is a model: these tests drive it without any interface.
/// </summary>
public sealed class NavigationTests
{
    private readonly RootState _root = new();

    private SectionState Section(MenuItemIntent intent) => _root.Header.Sections.Single(s => s.Intent == intent);

    private PageLinkState Link(MenuItemIntent intent)
        => _root.Header.Sections.SelectMany(s => s.Entries).OfType<PageLinkState>().Single(l => l.Intent == intent);

    private static Func<int> CountNotifications(INotifyStateChanged state)
    {
        var count = 0;
        state.OnStateChanged += () => count++;
        return () => count;
    }

    [Fact]
    public void Application_opens_on_the_first_page_of_the_first_section()
    {
        var quickStart = Section(MenuItemIntent.QuickStart);

        Assert.True(quickStart.IsSelected);
        Assert.Same(quickStart, _root.Navigation.SideMenu.Section);
        Assert.Same(quickStart.Entries, _root.Navigation.SideMenu.Entries);
        Assert.IsType<IntroductionPage>(_root.Navigation.Content.Page);
        Assert.True(Link(MenuItemIntent.Introduction).IsSelected);
    }

    [Fact]
    public void Each_menu_entry_gives_the_model_of_its_page()
    {
        Link(MenuItemIntent.Forms).Handle(new SelectableEntryState.Select());

        var page = Assert.IsType<FormsPage>(_root.Navigation.Content.Page);
        Assert.NotNull(page.Contact);
    }

    [Fact]
    public void Selecting_a_section_shows_its_entries_and_opens_its_first_page()
    {
        var components = Section(MenuItemIntent.Components);

        components.Handle(new SelectableEntryState.Select());

        Assert.True(components.IsSelected);
        Assert.False(Section(MenuItemIntent.QuickStart).IsSelected);
        Assert.Same(components.Entries, _root.Navigation.SideMenu.Entries);
        Assert.IsType<LayoutPage>(_root.Navigation.Content.Page);
        Assert.False(Link(MenuItemIntent.Introduction).IsSelected);
    }

    [Fact]
    public void A_section_reopens_the_page_it_showed_last()
    {
        Link(MenuItemIntent.StateFirst).Handle(new SelectableEntryState.Select());
        Section(MenuItemIntent.Themes).Handle(new SelectableEntryState.Select());

        Section(MenuItemIntent.QuickStart).Handle(new SelectableEntryState.Select());

        Assert.IsType<StateFirstPage>(_root.Navigation.Content.Page);
    }

    [Fact]
    public void Opening_a_page_notifies_only_the_states_that_changed()
    {
        var introduction = CountNotifications(Link(MenuItemIntent.Introduction));
        var installation = CountNotifications(Link(MenuItemIntent.Installation));
        var stateFirst = CountNotifications(Link(MenuItemIntent.StateFirst));
        var section = CountNotifications(Section(MenuItemIntent.QuickStart));
        var sideMenu = CountNotifications(_root.Navigation.SideMenu);
        var content = CountNotifications(_root.Navigation.Content);

        Link(MenuItemIntent.Installation).Handle(new SelectableEntryState.Select());

        Assert.Equal(1, introduction());
        Assert.Equal(1, installation());
        Assert.Equal(0, stateFirst());
        Assert.Equal(0, section());
        Assert.Equal(0, sideMenu());
        Assert.Equal(1, content());
    }

    [Fact]
    public void Opening_the_open_page_notifies_nothing()
    {
        var content = CountNotifications(_root.Navigation.Content);
        var introduction = CountNotifications(Link(MenuItemIntent.Introduction));

        Link(MenuItemIntent.Introduction).Handle(new SelectableEntryState.Select());

        Assert.Equal(0, content());
        Assert.Equal(0, introduction());
    }

    [Fact]
    public void A_page_keeps_its_state_across_navigation()
    {
        var forms = Link(MenuItemIntent.Forms);
        var contact = ((FormsPage)forms.Page).Contact;
        contact.Handle(new ContactState.SetName("Ada"));

        Section(MenuItemIntent.Themes).Handle(new SelectableEntryState.Select());
        forms.Handle(new SelectableEntryState.Select());

        Assert.Equal("Ada", ((FormsPage)_root.Navigation.Content.Page!).Contact.Name);
    }

    [Fact]
    public void Theme_selector_notifies_when_the_mode_changes()
    {
        var theme = _root.Footer.Theme;
        var notifications = CountNotifications(theme);

        theme.Handle(new ThemeSelectorState.SetMode(ThemeMode.Light));
        theme.Handle(new ThemeSelectorState.SetMode(ThemeMode.Light));

        Assert.Equal(ThemeMode.Light, theme.Mode);
        Assert.Equal(1, notifications());
    }
}
