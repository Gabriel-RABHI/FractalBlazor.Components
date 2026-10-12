using System.ComponentModel.DataAnnotations;
using FractalBlazor.Components.Common.Contracts;

namespace FractalBlazor.WebSite.StateModel
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
        private readonly IThemeModeStore _store;

        /// <summary>
        /// The mode chosen by the user: applied and saved.
        /// </summary>
        public sealed record SetMode(ThemeMode Value) : IStateAction<ThemeMode>;

        /// <summary>
        /// The mode read back by the host before the first render: applied, not saved again.
        /// </summary>
        public sealed record Restore(ThemeMode Value) : IStateAction<ThemeMode>;

        public ThemeSelectorState(IThemeModeStore store)
        {
            _store = store;
        }

        public static IReadOnlyList<ThemeMode> Modes { get; } = [ThemeMode.Dark, ThemeMode.Light];

        public ThemeMode Mode { get; private set; }

        public void Handle(SetMode action)
        {
            if (Apply(action.Value))
                _store.Save(action.Value);
        }

        public void Handle(Restore action) => Apply(action.Value);

        private bool Apply(ThemeMode mode)
        {
            if (Mode == mode)
                return false;

            Mode = mode;
            NotifyStateChanged();
            return true;
        }
    }
}
