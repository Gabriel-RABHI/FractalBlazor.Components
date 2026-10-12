namespace FractalBlazor.WebSite.StateModel
{
    /// <summary>
    /// Keeps the theme mode chosen by the user between visits. The model only declares it:
    /// the assembly that hosts the model implements it (a cookie on the web site) and
    /// injects it.
    /// </summary>
    public interface IThemeModeStore
    {
        /// <summary>
        /// Saves <paramref name="mode"/>. A state handles its actions synchronously, so the
        /// implementation may finish the work in the background.
        /// </summary>
        void Save(ThemeMode mode);
    }
}
