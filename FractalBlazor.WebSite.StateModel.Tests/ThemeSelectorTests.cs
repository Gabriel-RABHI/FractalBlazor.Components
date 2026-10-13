namespace FractalBlazor.WebSite.StateModel.Tests;

public sealed class ThemeSelectorTests
{
    private readonly RecordingThemeModeStore _store = new();
    private readonly ThemeSelectorState _theme;
    private int _notifications;

    public ThemeSelectorTests()
    {
        _theme = new RootState(_store, new TestFlagCatalog()).Footer.Theme;
        _theme.OnStateChanged += () => _notifications++;
    }

    [Fact]
    public void Choosing_a_mode_applies_and_saves_it()
    {
        _theme.Handle(new ThemeSelectorState.SetMode(ThemeMode.Light));

        Assert.Equal(ThemeMode.Light, _theme.Mode);
        Assert.Equal([ThemeMode.Light], _store.Saved);
        Assert.Equal(1, _notifications);
    }

    [Fact]
    public void Choosing_the_current_mode_changes_nothing()
    {
        _theme.Handle(new ThemeSelectorState.SetMode(ThemeMode.Light));

        _theme.Handle(new ThemeSelectorState.SetMode(ThemeMode.Light));

        Assert.Single(_store.Saved);
        Assert.Equal(1, _notifications);
    }

    [Fact]
    public void Restoring_a_mode_applies_it_without_saving_it_again()
    {
        _theme.Handle(new ThemeSelectorState.Restore(ThemeMode.Light));

        Assert.Equal(ThemeMode.Light, _theme.Mode);
        Assert.Empty(_store.Saved);
        Assert.Equal(1, _notifications);
    }
}
