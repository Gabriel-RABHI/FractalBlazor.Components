using FractalBlazor.Components.Common.Contracts;

namespace FractalBlazor.Demo.States
{

    public class TopBarState
    {
        private RootState _parent;

        public TopBarState(RootState parent)
        {
            _parent = parent;
        }
    }

    public class RootState
    {
        public TopBarState TopBar { get; }

        // The root menu items
        public IReadOnlyList<MainMenuItem> MenuItems { get; }

        public ContactState Contact { get; } = new();

        public RootState()
        {
            TopBar = new TopBarState(this);

            MenuItems = new List<MainMenuItem>
            {
                new MenuLinkItemState(null, MenuItemIntent.Introduction),
                new MainMenuGroupState(null, MenuItemIntent.QuickStart, isOpened: false, group =>
                [
                    new MenuLinkItemState(group, MenuItemIntent.Principles),
                    new MenuSeparatorItemState(group),
                    new MenuLinkItemState(group, MenuItemIntent.Forms)
                ])
            };
        }
    }

    public enum MenuItemIntent { Introduction, QuickStart, Principles, Forms }

    // 1. The Base Item
    public abstract class MainMenuItem
    {
        public MainMenuGroupState? Parent { get; }

        // Protected constructor so only derived classes can set it
        protected MainMenuItem(MainMenuGroupState? parent)
        {
            Parent = parent;
        }
    }

    // 2. An item with an Intent (Links and Groups)
    public abstract class MenuIntentItem : MainMenuItem
    {
        public MenuItemIntent Intent { get; }

        protected MenuIntentItem(MainMenuGroupState? parent, MenuItemIntent intent)
            : base(parent)
        {
            Intent = intent;
        }
    }

    // 3. The Leaf Node (Clickable Link)
    public class MenuLinkItemState : MenuIntentItem
    {
        public MenuLinkItemState(MainMenuGroupState? parent, MenuItemIntent intent)
            : base(parent, intent) { }
    }

    // 4. The Visual Separator (No intent required)
    public class MenuSeparatorItemState : MainMenuItem
    {
        public MenuSeparatorItemState(MainMenuGroupState? parent)
            : base(parent) { }
    }

    // 5. The Group Node
    public class MainMenuGroupState : MenuIntentItem, INotifyStateChanged
    {
        public bool IsOpened { get; private set; }

        public IReadOnlyList<MainMenuItem> Children { get; }

        // The single-line Action
        public record ToggleGroupAction(bool IsOpened) : IStateAction<bool>;

        // Re-renders the views of this group only
        public event Action? OnStateChanged;

        /// <param name="children">Builds the children with this group as their parent.</param>
        public MainMenuGroupState(
            MainMenuGroupState? parent,
            MenuItemIntent intent,
            bool isOpened,
            Func<MainMenuGroupState, IEnumerable<MainMenuItem>>? children = null)
            : base(parent, intent)
        {
            IsOpened = isOpened;
            Children = children?.Invoke(this).ToList().AsReadOnly() ?? new List<MainMenuItem>().AsReadOnly();
        }

        public void Handle(ToggleGroupAction action)
        {
            if (IsOpened != action.IsOpened)
            {
                IsOpened = action.IsOpened;
                OnStateChanged?.Invoke();
            }
        }
    }
}
