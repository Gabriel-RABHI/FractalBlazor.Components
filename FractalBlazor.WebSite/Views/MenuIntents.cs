using FractalBlazor.Components.Forms.Components.Icons;
using FractalBlazor.Components.Forms.Mapping;
using FractalBlazor.WebSite.StateModel;

namespace FractalBlazor.WebSite.Views
{
    /// <summary>
    /// How the views present a menu intent: the states only say what an entry is about.
    /// </summary>
    public static class MenuIntents
    {
        public static string Label(MenuItemIntent intent) => FbValueView.TextOf(intent);

        public static string Icon(MenuItemIntent intent) => intent switch
        {
            MenuItemIntent.Introduction => FbMaterialIcons.AutoStories,
            MenuItemIntent.Installation => FbMaterialIcons.InstallDesktop,
            MenuItemIntent.Basics => FbMaterialIcons.School,
            MenuItemIntent.Principles => FbMaterialIcons.AccountTree,
            MenuItemIntent.Themes => FbMaterialIcons.Palette,
            MenuItemIntent.RowStack => FbMaterialIcons.ViewColumn,
            MenuItemIntent.Grid => FbMaterialIcons.GridView,
            MenuItemIntent.ShowWhenOver => FbMaterialIcons.Mouse,
            MenuItemIntent.ShowWhenVisible => FbMaterialIcons.Visibility,
            MenuItemIntent.Drawer => FbMaterialIcons.MenuOpen,
            MenuItemIntent.DropDown => FbMaterialIcons.ArrowDropDownCircle,
            MenuItemIntent.Updatable => FbMaterialIcons.Sync,
            MenuItemIntent.DragDropBasics => FbMaterialIcons.OpenWith,
            MenuItemIntent.DragDropRules => FbMaterialIcons.Rule,
            MenuItemIntent.DragDropSamples => FbMaterialIcons.ViewKanban,
            MenuItemIntent.Slider => FbMaterialIcons.Tune,
            MenuItemIntent.Progress => FbMaterialIcons.DonutLarge,
            MenuItemIntent.Badge => FbMaterialIcons.Label,
            MenuItemIntent.Templates => FbMaterialIcons.Dashboard,
            _ => FbMaterialIcons.Circle
        };

        /// <summary>
        /// The image that stands for an intent instead of an icon, the logo of a brand, or
        /// <c>null</c>.
        /// </summary>
        public static string? Image(MenuItemIntent intent) => intent switch
        {
            MenuItemIntent.GitHub => SiteImages.GitHub,
            MenuItemIntent.LinkedIn => SiteImages.LinkedIn,
            _ => null
        };
    }
}
