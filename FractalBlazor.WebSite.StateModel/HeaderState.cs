namespace FractalBlazor.WebSite.StateModel
{
    /// <summary>
    /// The header: the name of the application, the sections of the top menu, and links to
    /// other sites. On a narrow screen, the sections and the links move into
    /// <see cref="Menu"/>.
    /// </summary>
    public sealed class HeaderState
    {
        public HeaderState(string name, IReadOnlyList<SectionState> sections, IReadOnlyList<ExternalLinkState> links)
        {
            Name = name;
            Sections = sections;
            Links = links;
            Menu = new HeaderMenuState(this);
        }

        public string Name { get; }

        public IReadOnlyList<SectionState> Sections { get; }

        public IReadOnlyList<ExternalLinkState> Links { get; }

        /// <summary>
        /// The menu of the compact header.
        /// </summary>
        public HeaderMenuState Menu { get; }
    }

    /// <summary>
    /// A link that leaves the application.
    /// </summary>
    public sealed record ExternalLinkState(MenuItemIntent Intent, string Url);
}
