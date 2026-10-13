using FractalBlazor.Components.Common.Contracts;

namespace FractalBlazor.WebSite.StateModel
{
    /// <summary>
    /// The screen: the width category of the window, sent by the view, and what it implies
    /// for the layout. The views follow it with <c>if</c>: no media query decides.
    /// </summary>
    public sealed class ScreenState : ObservableState
    {
        private readonly PopupState _drawer;
        private readonly PopupState _headerMenu;

        public sealed record SetWidth(FbWidthClass Value) : IStateAction<FbWidthClass>;

        /// <param name="drawer">Closed when the menu on the left leaves it.</param>
        /// <param name="headerMenu">Closed when the header shows its sections again.</param>
        public ScreenState(PopupState drawer, PopupState headerMenu)
        {
            _drawer = drawer;
            _headerMenu = headerMenu;
        }

        /// <summary>
        /// Wide until the window is measured: the first render, before any measure, is the
        /// one of a desktop.
        /// </summary>
        public FbWidthClass Width { get; private set; } = FbWidthClass.XL;

        /// <summary>
        /// Under 1024 px, both menus fold: the sections and the links of the header move into
        /// a menu, and the menu on the left into a drawer.
        /// </summary>
        public bool Compact => Width < FbWidthClass.L;

        public void Handle(SetWidth action)
        {
            if (Width == action.Value)
                return;

            Width = action.Value;
            if (!Compact)
            {
                _drawer.Close();
                _headerMenu.Close();
            }
            NotifyStateChanged();
        }
    }

    /// <summary>
    /// A menu that opens over the page: the drawer of the menu on the left, the menu of the
    /// compact header. Its view sends <see cref="Toggle"/> from a button, and
    /// <see cref="SetOpen"/> when the user dismisses it; the navigation closes it once it has
    /// opened what was chosen.
    /// </summary>
    public abstract class PopupState : ObservableState
    {
        public sealed record Toggle : IStateAction;

        public sealed record SetOpen(bool Value) : IStateAction<bool>;

        public bool IsOpen { get; private set; }

        public void Handle(Toggle action) => Apply(!IsOpen);

        public void Handle(SetOpen action) => Apply(action.Value);

        internal void Close() => Apply(false);

        private void Apply(bool open)
        {
            if (IsOpen == open)
                return;

            IsOpen = open;
            NotifyStateChanged();
        }
    }

    /// <summary>
    /// The drawer that carries the menu on the left on a narrow screen. It notifies when
    /// the menu changes too: its button only shows for a section that has a menu.
    /// </summary>
    public sealed class MenuDrawerState : PopupState
    {
        public MenuDrawerState(SideMenuState menu)
        {
            Menu = menu;
            menu.OnStateChanged += NotifyStateChanged;
        }

        public SideMenuState Menu { get; }

        public bool HasMenu => Menu.Entries.Count > 0;
    }

    /// <summary>
    /// The menu of the compact header: its sections and its links.
    /// </summary>
    public sealed class HeaderMenuState : PopupState
    {
        public HeaderMenuState(HeaderState header)
        {
            Header = header;
        }

        public HeaderState Header { get; }
    }
}
