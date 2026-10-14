namespace FractalBlazor.WebSite.Views
{
    /// <summary>
    /// The images of the site, served from the static assets of this library. 64 px squares,
    /// shown with <c>FbIconImage</c> at the size of an icon.
    /// </summary>
    public static class SiteImages
    {
        private const string Folder = "_content/FractalBlazor.WebSite/images/";

        public const string Logo = Folder + "logo-fractal-blazor-small-64.png";

        public const string GitHub = Folder + "logo-github-64.png";

        public const string LinkedIn = Folder + "logo-linkedinl-64.png";

        /// <summary>
        /// A light bulb: the flag assistant.
        /// </summary>
        public const string Tool = Folder + "logo-tool-64.png";
    }
}
