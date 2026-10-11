namespace FractalBlazor.WebSite.States
{
    // The models of the pages, one type per page: the content area renders each with the
    // view declared for its type. A documentation page needs no data, its type is enough.

    public sealed record IntroductionPage;

    public sealed record InstallationPage;

    public sealed record StateFirstPage;

    public sealed record LayoutPage;

    /// <summary>
    /// The contact form. The page owns its state, which survives navigation.
    /// </summary>
    public sealed record FormsPage(ContactState Contact);

    public sealed record VariantsPage;

    public sealed record ThemeEditorPage;
}
