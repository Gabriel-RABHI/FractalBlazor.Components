using FractalBlazor.Components.Common.Contracts;

namespace FractalBlazor.WebSite.StateModel
{
    /// <summary>
    /// Opens sections and pages. Each area of the screen is its own state, so opening a page
    /// re-renders the two entries whose selection changed and the content, not the whole
    /// application.
    /// </summary>
    public sealed class NavigationState
    {
        public SideMenuState SideMenu { get; } = new();

        public ContentState Content { get; } = new();

        /// <summary>
        /// Shows the entries of <paramref name="section"/> and opens the page it showed last.
        /// </summary>
        public void Open(SectionState section)
        {
            if (section.LastPage is not null)
            {
                Open(section.LastPage);
                return;
            }

            ShowSection(section);
            Content.Link?.SetSelected(false);
            Content.Show(null);
        }

        public void Open(PageLinkState link)
        {
            ShowSection(link.Section);
            if (ReferenceEquals(Content.Link, link))
                return;

            Content.Link?.SetSelected(false);
            link.SetSelected(true);
            link.Section.LastPage = link;
            Content.Show(link);
        }

        private void ShowSection(SectionState section)
        {
            if (ReferenceEquals(SideMenu.Section, section))
                return;

            SideMenu.Section?.SetSelected(false);
            section.SetSelected(true);
            SideMenu.Show(section);
        }
    }

    /// <summary>
    /// The menu on the left: the entries of the open section.
    /// </summary>
    public sealed class SideMenuState : ObservableState
    {
        public SectionState? Section { get; private set; }

        public IReadOnlyList<MenuEntryState> Entries => Section?.Entries ?? [];

        internal void Show(SectionState section)
        {
            Section = section;
            NotifyStateChanged();
        }
    }

    /// <summary>
    /// The content area: the open page.
    /// </summary>
    public sealed class ContentState : ObservableState
    {
        public PageLinkState? Link { get; private set; }

        /// <summary>
        /// The model of the open page.
        /// </summary>
        public object? Page => Link?.Page;

        internal void Show(PageLinkState? link)
        {
            Link = link;
            NotifyStateChanged();
        }
    }
}
