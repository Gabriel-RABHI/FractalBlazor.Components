using System.ComponentModel.DataAnnotations;
using FractalBlazor.Components.Common.Contracts;

namespace FractalBlazor.WebSite.States
{
    /// <summary>
    /// The footer: a line of text and the theme selector.
    /// </summary>
    public sealed class FooterState
    {
        public FooterState(string text, ThemeSelectorState theme)
        {
            Text = text;
            Theme = theme;
        }

        public string Text { get; }

        public ThemeSelectorState Theme { get; }
    }

    public enum ThemeMode
    {
        [Display(Name = "Sombre")]
        Dark,

        [Display(Name = "Clair")]
        Light
    }

    /// <summary>
    /// The theme of the page, dark or light.
    /// </summary>
    public sealed class ThemeSelectorState : ObservableState
    {
        public sealed record SetMode(ThemeMode Value) : IStateAction<ThemeMode>;

        public static IReadOnlyList<ThemeMode> Modes { get; } = [ThemeMode.Dark, ThemeMode.Light];

        public ThemeMode Mode { get; private set; }

        public void Handle(SetMode action)
        {
            if (Mode == action.Value)
                return;

            Mode = action.Value;
            NotifyStateChanged();
        }
    }
}
