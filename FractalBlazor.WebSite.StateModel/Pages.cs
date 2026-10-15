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

    /// <summary>
    /// FbRow and FbStack, with the states of their two demos: the alignment pad and the
    /// playground.
    /// </summary>
    public sealed record RowStackPage(AlignmentPadState Pad, FlexPlaygroundState Playground);

    public sealed record GridPage;

    /// <summary>
    /// The drawer, with the state it demonstrates.
    /// </summary>
    public sealed record DrawerPage(DrawerDemoState Drawer);

    /// <summary>
    /// The basics of drag and drop, with the state of each demo.
    /// </summary>
    public sealed record DragDropBasicsPage(DragDropDemoState Master, DragDropDemoState Handles, DragDropDemoState Info, DragDropDemoState Receiver, DragDropDemoState Nested);

    /// <summary>
    /// The rules of an area: compatibility, preview, splits.
    /// </summary>
    public sealed record DragDropRulesPage(CompatibilityDemoState Compatibility, PreviewDemoState Preview, SplitDemoState Splits);

    /// <summary>
    /// A reordered list and a kanban board.
    /// </summary>
    public sealed record DragDropSamplesPage(ReorderListState List, KanbanBoardState Board);

    /// <summary>
    /// The slider: a labeled scale, verticals, dots and values.
    /// </summary>
    public sealed record SliderPage(ScaleSliderDemoState Scales, VerticalSliderDemoState Verticals, DotsSliderDemoState Dots, TickMarkSliderDemoState Marks);

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
