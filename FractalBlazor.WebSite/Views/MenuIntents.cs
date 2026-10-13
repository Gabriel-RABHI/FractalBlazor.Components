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
            MenuItemIntent.Templates => FbMaterialIcons.Dashboard,
            MenuItemIntent.GitHub => FbMaterialIcons.Code,
            MenuItemIntent.LinkedIn => FbMaterialIcons.Work,
            _ => FbMaterialIcons.Circle
        };
    }
}
