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
        public RootState(IThemeModeStore themeModeStore)
        {
            Navigation = new NavigationState();
            Header = new HeaderState(
                "FractalBlazor",
                [
                    new SectionState(Navigation, MenuItemIntent.QuickStart, section =>
                    [
                        new PageLinkState(section, MenuItemIntent.Introduction, new IntroductionPage()),
                        new PageLinkState(section, MenuItemIntent.Installation, new InstallationPage()),
                        new MenuSeparatorState(),
                        new PageLinkState(section, MenuItemIntent.StateFirst, new StateFirstPage())
                    ]),
                    new SectionState(Navigation, MenuItemIntent.Components, section =>
                    [
                        new PageLinkState(section, MenuItemIntent.Layout, new LayoutPage(new DrawerDemoState())),
                        new PageLinkState(section, MenuItemIntent.Forms, new FormsPage(new ContactState()))
                    ]),
                    new SectionState(Navigation, MenuItemIntent.Themes, section =>
                    [
                        new PageLinkState(section, MenuItemIntent.Variants, new VariantsPage()),
                        new PageLinkState(section, MenuItemIntent.ThemeEditor, new ThemeEditorPage())
                    ])
                ],
                [
                    new ExternalLinkState(MenuItemIntent.GitHub, RepositoryUrl),
                    new ExternalLinkState(MenuItemIntent.LinkedIn, LinkedInUrl)
                ]);
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
        [Display(Name = "Démarrage")]
        QuickStart,

        [Display(Name = "Introduction")]
        Introduction,

        [Display(Name = "Installation")]
        Installation,

        [Display(Name = "State-first")]
        StateFirst,

        [Display(Name = "Composants")]
        Components,

        [Display(Name = "Mise en page")]
        Layout,

        [Display(Name = "Formulaires")]
        Forms,

        [Display(Name = "Thèmes")]
        Themes,

        [Display(Name = "Variantes")]
        Variants,

        [Display(Name = "Éditeur de thème")]
        ThemeEditor,

        [Display(Name = "GitHub")]
        GitHub,

        [Display(Name = "LinkedIn")]
        LinkedIn
    }
}
