namespace FractalBlazor.WebSite.StateModel
{
    // The models of the pages, one type per page: the content area renders each with the
    // view declared for its type. A documentation page needs no data, its type is enough.

    public sealed record IntroductionPage;

    public sealed record InstallationPage;

    /// <summary>
    /// The state-first model of the application, under Principles.
    /// </summary>
    public sealed record StateFirstPage;

    /// <summary>
    /// The variants, then the theme editor.
    /// </summary>
    public sealed record ThemesPage(VariantsPage Variants, ThemeEditorPage Editor);

    public sealed record VariantsPage;

    public sealed record ThemeEditorPage;

    public sealed record RowStackPage;

    public sealed record GridPage;

    /// <summary>
    /// The drawer, with the state it demonstrates.
    /// </summary>
    public sealed record DrawerPage(DrawerDemoState Drawer);

    /// <summary>
    /// The contact form, under Compositions. The page owns its state, which survives
    /// navigation.
    /// </summary>
    public sealed record FormsPage(ContactState Contact);

    /// <summary>
    /// A page whose content is still to come.
    /// </summary>
    public sealed record PendingPage;
}
