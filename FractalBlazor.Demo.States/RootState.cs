using FractalBlazor.Components.Forms.Contracts;

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

        public RootState()
        {
            TopBar = new TopBarState(this);

            // Example initialization
            MenuItems = new List<MainMenuItem>
            {
                new MenuLinkItemState(null, MenuItemIntent.Introduction),
                new MainMenuGroupState(null, MenuItemIntent.QuickStart, isOpened: false)
                // ... add children to the group
            };
        }
    }

    public enum MenuItemIntent { Introduction, QuickStart, Principles }

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
    public class MainMenuGroupState : MenuIntentItem
    {
        public bool IsOpened { get; private set; }

        // The missing piece: A group must hold its children!
        public IReadOnlyList<MainMenuItem> Children { get; }

        // The single-line Action
        public record ToggleGroupAction(bool IsOpened) : IStateAction<bool>;

        // Optional event to notify the Blazor UI to re-render just this group
        public event Action? OnStateChanged;

        public MainMenuGroupState(
            MainMenuGroupState? parent,
            MenuItemIntent intent,
            bool isOpened,
            IEnumerable<MainMenuItem>? children = null)
            : base(parent, intent)
        {
            IsOpened = isOpened;
            Children = children?.ToList().AsReadOnly() ?? new List<MainMenuItem>().AsReadOnly();
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
