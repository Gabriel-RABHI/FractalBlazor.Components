using System.ComponentModel.DataAnnotations;
using FractalBlazor.Components.Common.Contracts;

namespace FractalBlazor.WebSite.StateModel
{
    /// <summary>
    /// The web site, held entirely by its state. Every element of the screen is a
    /// state object rendered by the view declared for its type: the header with the sections
    /// of the top menu, the menu on the left with the pages of the open section, the open
    /// page, and the footer. One instance per circuit on the server, per page in WebAssembly.
    /// </summary>
    public sealed class RootState : ObservableState
    {
        public const string RepositoryUrl = "https://github.com/Gabriel-RABHI/FractalBlazor.Components";

        public const string LinkedInUrl = "https://www.linkedin.com/";

        /// <param name="themeModeStore">Keeps the theme mode chosen in the footer.</param>
        /// <param name="flagCatalog">The flags of the libraries, for the flag assistant.</param>
        public RootState(IThemeModeStore themeModeStore, IFlagCatalog flagCatalog)
        {
            Navigation = new NavigationState();
            Header = new HeaderState(
                "Fractal Blazor",
                [
                    new SectionState(Navigation, MenuItemIntent.QuickStart, section =>
                    [
                        new PageLinkState(section, MenuItemIntent.Introduction, new IntroductionPage()),
                        new PageLinkState(section, MenuItemIntent.Installation, new InstallationPage()),
                        new PageLinkState(section, MenuItemIntent.Basics, new PendingPage()),
                        new PageLinkState(section, MenuItemIntent.Principles, new StateFirstPage()),
                        new PageLinkState(section, MenuItemIntent.Themes, new ThemesPage(new VariantsPage(), new ThemeEditorPage()))
                    ]),
                    new SectionState(Navigation, MenuItemIntent.Components, section =>
                    [
                        new MenuGroupState(section, MenuItemIntent.Layout, group =>
                        [
                            new PageLinkState(group, MenuItemIntent.RowStack, new RowStackPage(new AlignmentPadState(), new FlexPlaygroundState())),
                            new PageLinkState(group, MenuItemIntent.Grid, new GridPage()),
                            new PageLinkState(group, MenuItemIntent.ShowWhenOver, new PendingPage()),
                            new PageLinkState(group, MenuItemIntent.ShowWhenVisible, new PendingPage()),
                            new PageLinkState(group, MenuItemIntent.Drawer, new DrawerPage(new DrawerDemoState())),
                            new PageLinkState(group, MenuItemIntent.DropDown, new PendingPage()),
                            new PageLinkState(group, MenuItemIntent.Updatable, new PendingPage())
                        ]),
                        new MenuGroupState(section, MenuItemIntent.Panels),
                        new MenuGroupState(section, MenuItemIntent.View),
                        new MenuGroupState(section, MenuItemIntent.Inputs),
                        new MenuGroupState(section, MenuItemIntent.Selectors)
                    ]),
                    new SectionState(Navigation, MenuItemIntent.DragAndDrop, section =>
                    [
                        new PageLinkState(section, MenuItemIntent.DragDropBasics,
                            new DragDropBasicsPage(new DragDropDemoState(), new DragDropDemoState(), new DragDropDemoState(), new DragDropDemoState(), new DragDropDemoState())),
                        new PageLinkState(section, MenuItemIntent.DragDropRules,
                            new DragDropRulesPage(new CompatibilityDemoState(), new PreviewDemoState(), new SplitDemoState())),
                        new PageLinkState(section, MenuItemIntent.DragDropSamples, new DragDropSamplesPage(new ReorderListState(), new KanbanBoardState()))
                    ]),
                    new SectionState(Navigation, MenuItemIntent.Compositions, new FormsPage(new ContactState())),
                    new SectionState(Navigation, MenuItemIntent.Advanced, new PendingPage()),
                    new SectionState(Navigation, MenuItemIntent.BuildingBlocks, section =>
                    [
                        new PageLinkState(section, MenuItemIntent.Templates, new PendingPage())
                    ]),
                    new SectionState(Navigation, MenuItemIntent.Pricing, new PendingPage())
                ],
                [
                    new ExternalLinkState(MenuItemIntent.LinkedIn, LinkedInUrl),
                    new ExternalLinkState(MenuItemIntent.GitHub, RepositoryUrl)
                ],
                new FlagAssistantState(flagCatalog));
            Footer = new FooterState("Composants Blazor state-first", new ThemeSelectorState(themeModeStore));
            Screen = new ScreenState(Navigation.Drawer, Header.Menu);

            // The menu of the header closes once the section chosen in it is open. The
            // application view lays the areas out from the screen: it follows its changes.
            Navigation.Opened += Header.Menu.Close;
            Screen.OnStateChanged += NotifyStateChanged;

            Navigation.Open(Header.Sections[0]);
        }

        /// <summary>
        /// The width of the window and the layout it implies.
        /// </summary>
        public ScreenState Screen { get; }

        public HeaderState Header { get; }

        /// <summary>
        /// The menu on the left and the open page.
        /// </summary>
        public NavigationState Navigation { get; }

        public FooterState Footer { get; }
    }

    /// <summary>
    /// What a menu entry is about. The views turn it into a label (its <c>Display</c> name)
    /// and an icon: the state stays free of any presentation.
    /// </summary>
    public enum MenuItemIntent
    {
        [Display(Name = "Quick Start")]
        QuickStart,

        [Display(Name = "Introduction")]
        Introduction,

        [Display(Name = "Installation")]
        Installation,

        [Display(Name = "Basics")]
        Basics,

        [Display(Name = "Principles")]
        Principles,

        [Display(Name = "Themes")]
        Themes,

        [Display(Name = "Components")]
        Components,

        [Display(Name = "Layout")]
        Layout,

        [Display(Name = "Row / Stack")]
        RowStack,

        [Display(Name = "Grid")]
        Grid,

        [Display(Name = "Show When Over")]
        ShowWhenOver,

        [Display(Name = "Show When Visible")]
        ShowWhenVisible,

        [Display(Name = "Drawer")]
        Drawer,

        [Display(Name = "Drop Down")]
        DropDown,

        [Display(Name = "Updatable")]
        Updatable,

        [Display(Name = "Panels")]
        Panels,

        [Display(Name = "View")]
        View,

        [Display(Name = "Inputs")]
        Inputs,

        [Display(Name = "Selectors")]
        Selectors,

        [Display(Name = "Drag & Drop")]
        DragAndDrop,

        [Display(Name = "Basics")]
        DragDropBasics,

        [Display(Name = "Rules & Zones")]
        DragDropRules,

        [Display(Name = "Samples")]
        DragDropSamples,

        [Display(Name = "Compositions")]
        Compositions,

        [Display(Name = "Advanced")]
        Advanced,

        [Display(Name = "Building Blocks")]
        BuildingBlocks,

        [Display(Name = "Templates")]
        Templates,

        [Display(Name = "Pricing")]
        Pricing,

        [Display(Name = "GitHub")]
        GitHub,

        [Display(Name = "LinkedIn")]
        LinkedIn
    }
}
