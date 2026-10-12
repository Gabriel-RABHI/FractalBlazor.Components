namespace FractalBlazor.WebSite.StateModel.Tests;

/// <summary>
/// Stands for the cookie of the web site: remembers every saved mode.
/// </summary>
internal sealed class RecordingThemeModeStore : IThemeModeStore
{
    public List<ThemeMode> Saved { get; } = [];

    public void Save(ThemeMode mode) => Saved.Add(mode);
}
