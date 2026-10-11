using FractalBlazor.Components.Forms.Icons;
using FractalBlazor.Components.Forms.Mapping;
using FractalBlazor.WebSite.States;

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
            MenuItemIntent.QuickStart => FbMaterialIcons.RocketLaunch,
            MenuItemIntent.Introduction => FbMaterialIcons.AutoStories,
            MenuItemIntent.Installation => FbMaterialIcons.InstallDesktop,
            MenuItemIntent.StateFirst => FbMaterialIcons.AccountTree,
            MenuItemIntent.Components => FbMaterialIcons.Widgets,
            MenuItemIntent.Layout => FbMaterialIcons.ViewQuilt,
            MenuItemIntent.Forms => FbMaterialIcons.DynamicForm,
            MenuItemIntent.Themes => FbMaterialIcons.Palette,
            MenuItemIntent.Variants => FbMaterialIcons.Contrast,
            MenuItemIntent.ThemeEditor => FbMaterialIcons.Tune,
            MenuItemIntent.GitHub => FbMaterialIcons.Code,
            MenuItemIntent.LinkedIn => FbMaterialIcons.Work,
            _ => FbMaterialIcons.Circle
        };
    }
}
