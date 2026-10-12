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
    /// A line between two groups of entries.
    /// </summary>
    public sealed class MenuSeparatorState : MenuEntryState
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
    /// A section of the top menu. It carries the entries of the menu on the left, shown
    /// while the section is open.
    /// </summary>
    public sealed class SectionState : SelectableEntryState
    {
        /// <param name="entries">Builds the entries with this section as their parent.</param>
        public SectionState(NavigationState navigation, MenuItemIntent intent, Func<SectionState, IEnumerable<MenuEntryState>> entries)
            : base(intent)
        {
            Navigation = navigation;
            Entries = entries(this).ToList().AsReadOnly();
            LastPage = Entries.OfType<PageLinkState>().FirstOrDefault();
        }

        internal NavigationState Navigation { get; }

        /// <summary>
        /// The entries of the menu on the left: pages and separators.
        /// </summary>
        public IReadOnlyList<MenuEntryState> Entries { get; }

        /// <summary>
        /// The page opened last in this section, opened again when the section is selected.
        /// </summary>
        public PageLinkState? LastPage { get; internal set; }

        protected override void Open() => Navigation.Open(this);
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

        public SectionState Section { get; }

        public object Page { get; }

        protected override void Open() => Section.Navigation.Open(this);
    }
}
