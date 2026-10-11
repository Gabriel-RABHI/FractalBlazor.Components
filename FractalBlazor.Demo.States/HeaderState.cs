namespace FractalBlazor.Demo.States
{
    /// <summary>
    /// The header: the name of the application, the sections of the top menu, and links to
    /// other sites.
    /// </summary>
    public sealed class HeaderState
    {
        public HeaderState(string name, IReadOnlyList<SectionState> sections, IReadOnlyList<ExternalLinkState> links)
        {
            Name = name;
            Sections = sections;
            Links = links;
        }

        public string Name { get; }

        public IReadOnlyList<SectionState> Sections { get; }

        public IReadOnlyList<ExternalLinkState> Links { get; }
    }

    /// <summary>
    /// A link that leaves the application.
    /// </summary>
    public sealed record ExternalLinkState(MenuItemIntent Intent, string Url);
}
