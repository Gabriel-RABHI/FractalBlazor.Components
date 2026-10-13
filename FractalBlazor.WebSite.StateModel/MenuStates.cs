using FractalBlazor.Components.Common.Contracts;

namespace FractalBlazor.WebSite.StateModel
{
    /// <summary>
    /// An entry of a menu. A menu is a polymorphic list: each entry is rendered by the view
    /// declared for its own type.
    /// </summary>
    public abstract class MenuEntryState : ObservableState
    {
    }

    /// <summary>
    /// An entry that opens something. Its view sends <see cref="Select"/>; the navigation
    /// moves the selection, and only the entries whose selection changed notify their views.
    /// </summary>
    public abstract class SelectableEntryState : MenuEntryState
    {
        public sealed record Select : IStateAction;

        protected SelectableEntryState(MenuItemIntent intent)
        {
            Intent = intent;
        }

        public MenuItemIntent Intent { get; }

        public bool IsSelected { get; private set; }

        public void Handle(Select action) => Open();

        protected abstract void Open();

        internal void SetSelected(bool selected)
        {
            if (IsSelected == selected)
                return;

            IsSelected = selected;
            NotifyStateChanged();
        }
    }

    /// <summary>
    /// A section of the top menu. Either it carries the entries of the menu on the left,
    /// shown while the section is open, or it has none and is a page itself.
    /// </summary>
    public sealed class SectionState : SelectableEntryState
    {
        /// <param name="entries">Builds the entries with this section as their parent.</param>
        public SectionState(NavigationState navigation, MenuItemIntent intent, Func<SectionState, IEnumerable<MenuEntryState>> entries)
            : base(intent)
        {
            Navigation = navigation;
            Entries = entries(this).ToList().AsReadOnly();
            LastPage = Pages.FirstOrDefault();
        }

        /// <param name="page">The model of the page the section opens.</param>
        public SectionState(NavigationState navigation, MenuItemIntent intent, object page)
            : base(intent)
        {
            Navigation = navigation;
            Entries = [];
            Home = new PageLinkState(this, intent, page);
            LastPage = Home;
        }

        internal NavigationState Navigation { get; }

        /// <summary>
        /// The entries of the menu on the left: pages and groups of pages.
        /// </summary>
        public IReadOnlyList<MenuEntryState> Entries { get; }

        /// <summary>
        /// The page of a section without entries, which is a page itself.
        /// </summary>
        public PageLinkState? Home { get; }

        /// <summary>
        /// Every page of the menu on the left, those of the groups included.
        /// </summary>
        public IEnumerable<PageLinkState> Pages
        {
            get
            {
                foreach (var entry in Entries)
                {
                    if (entry is PageLinkState page)
                        yield return page;
                    else if (entry is MenuGroupState group)
                        foreach (var grouped in group.Pages)
                            yield return grouped;
                }
            }
        }

        /// <summary>
        /// The page opened last in this section, opened again when the section is selected.
        /// </summary>
        public PageLinkState? LastPage { get; internal set; }

        protected override void Open() => Navigation.Open(this);
    }

    /// <summary>
    /// A titled group of pages in the menu on the left. It opens nothing itself.
    /// </summary>
    public sealed class MenuGroupState : MenuEntryState
    {
        /// <param name="pages">Builds the pages with this group as their parent; none for a
        /// group still empty.</param>
        public MenuGroupState(SectionState section, MenuItemIntent intent, Func<MenuGroupState, IEnumerable<PageLinkState>>? pages = null)
        {
            Section = section;
            Intent = intent;
            Pages = (pages?.Invoke(this) ?? []).ToList().AsReadOnly();
        }

        public SectionState Section { get; }

        public MenuItemIntent Intent { get; }

        public IReadOnlyList<PageLinkState> Pages { get; }
    }

    /// <summary>
    /// An entry of the menu on the left. It gives the model of its page, which the content
    /// area renders with the view declared for the model type.
    /// </summary>
    public sealed class PageLinkState : SelectableEntryState
    {
        public PageLinkState(SectionState section, MenuItemIntent intent, object page)
            : base(intent)
        {
            Section = section;
            Page = page;
        }

        public PageLinkState(MenuGroupState group, MenuItemIntent intent, object page)
            : this(group.Section, intent, page)
        {
            Group = group;
        }

        public SectionState Section { get; }

        /// <summary>
        /// The group the page belongs to, if any.
        /// </summary>
        public MenuGroupState? Group { get; }

        public object Page { get; }

        protected override void Open() => Section.Navigation.Open(this);
    }
}
