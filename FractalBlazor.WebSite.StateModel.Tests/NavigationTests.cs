using FractalBlazor.Components.Common.Contracts;

namespace FractalBlazor.WebSite.StateModel.Tests;

/// <summary>
/// The web site is a model: these tests drive it without any interface.
/// </summary>
public sealed class NavigationTests
{
    private readonly RootState _root = new(new RecordingThemeModeStore());

    private SectionState Section(MenuItemIntent intent) => _root.Header.Sections.Single(s => s.Intent == intent);

    private PageLinkState Link(MenuItemIntent intent)
        => _root.Header.Sections.SelectMany(s => s.Pages).Single(l => l.Intent == intent);

    private static Func<int> CountNotifications(INotifyStateChanged state)
    {
        var count = 0;
        state.OnStateChanged += () => count++;
        return () => count;
    }

    [Fact]
    public void The_top_menu_follows_the_tree()
    {
        Assert.Equal(
            [MenuItemIntent.QuickStart, MenuItemIntent.Components, MenuItemIntent.Compositions,
             MenuItemIntent.Advanced, MenuItemIntent.BuildingBlocks, MenuItemIntent.Pricing],
            _root.Header.Sections.Select(s => s.Intent));
        Assert.Equal(
            [MenuItemIntent.Introduction, MenuItemIntent.Installation, MenuItemIntent.Basics,
             MenuItemIntent.Principles, MenuItemIntent.Themes],
            Section(MenuItemIntent.QuickStart).Pages.Select(p => p.Intent));
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
        Link(MenuItemIntent.Drawer).Handle(new SelectableEntryState.Select());

        var page = Assert.IsType<DrawerPage>(_root.Navigation.Content.Page);
        Assert.NotNull(page.Drawer);
    }

    [Fact]
    public void Selecting_a_section_shows_its_entries_and_opens_its_first_page()
    {
        var components = Section(MenuItemIntent.Components);

        components.Handle(new SelectableEntryState.Select());

        Assert.True(components.IsSelected);
        Assert.False(Section(MenuItemIntent.QuickStart).IsSelected);
        Assert.Same(components.Entries, _root.Navigation.SideMenu.Entries);
        Assert.IsType<RowStackPage>(_root.Navigation.Content.Page);
        Assert.False(Link(MenuItemIntent.Introduction).IsSelected);
    }

    [Fact]
    public void A_group_titles_pages_of_its_section_and_may_still_be_empty()
    {
        var components = Section(MenuItemIntent.Components);
        var groups = components.Entries.Cast<MenuGroupState>().ToList();

        Assert.Equal(
            [MenuItemIntent.Layout, MenuItemIntent.Panels, MenuItemIntent.View, MenuItemIntent.Inputs, MenuItemIntent.Selectors],
            groups.Select(g => g.Intent));
        Assert.Equal(7, groups[0].Pages.Count);
        Assert.All(groups.Skip(1), group => Assert.Empty(group.Pages));

        var drawer = Link(MenuItemIntent.Drawer);
        Assert.Same(groups[0], drawer.Group);
        Assert.Same(components, drawer.Section);
    }

    [Fact]
    public void A_section_without_entries_is_a_page_itself()
    {
        var compositions = Section(MenuItemIntent.Compositions);

        compositions.Handle(new SelectableEntryState.Select());

        Assert.True(compositions.IsSelected);
        Assert.Empty(_root.Navigation.SideMenu.Entries);
        Assert.Same(compositions.Home, _root.Navigation.Content.Link);
        Assert.IsType<FormsPage>(_root.Navigation.Content.Page);
    }

    [Fact]
    public void A_section_reopens_the_page_it_showed_last()
    {
        Link(MenuItemIntent.Principles).Handle(new SelectableEntryState.Select());
        Section(MenuItemIntent.Components).Handle(new SelectableEntryState.Select());

        Section(MenuItemIntent.QuickStart).Handle(new SelectableEntryState.Select());

        Assert.IsType<StateFirstPage>(_root.Navigation.Content.Page);
    }

    [Fact]
    public void Opening_a_page_notifies_only_the_states_that_changed()
    {
        var introduction = CountNotifications(Link(MenuItemIntent.Introduction));
        var installation = CountNotifications(Link(MenuItemIntent.Installation));
        var principles = CountNotifications(Link(MenuItemIntent.Principles));
        var section = CountNotifications(Section(MenuItemIntent.QuickStart));
        var sideMenu = CountNotifications(_root.Navigation.SideMenu);
        var content = CountNotifications(_root.Navigation.Content);

        Link(MenuItemIntent.Installation).Handle(new SelectableEntryState.Select());

        Assert.Equal(1, introduction());
        Assert.Equal(1, installation());
        Assert.Equal(0, principles());
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
        var compositions = Section(MenuItemIntent.Compositions);
        ((FormsPage)compositions.Home!.Page).Contact.Handle(new ContactState.SetName("Ada"));

        Section(MenuItemIntent.Components).Handle(new SelectableEntryState.Select());
        compositions.Handle(new SelectableEntryState.Select());

        Assert.Equal("Ada", ((FormsPage)_root.Navigation.Content.Page!).Contact.Name);
    }
}
