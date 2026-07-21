using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout;

/// <summary>
/// A container-query-based CSS Grid. Responsive values are relative to this
/// component's width, not to the browser viewport.
/// </summary>
public class FbGridContainer : FbLayoutVisibleComponentBase
{
    #region HIDDEN
    private FbFrame _separator = FbFrame.None;

    private string SeparatorClass {
        get {
            string baseClasse = "framed-separator-row";

            if (_separator is FbFrame.Light)
                return $"{baseClasse} {baseClasse}-light-frame";
            else if (_separator is FbFrame.Medium)
                return $"{baseClasse} {baseClasse}-medium-frame";
            else if (_separator is FbFrame.Strong)
                return $"{baseClasse} {baseClasse}-strong-frame";
            else
                return "";
        }
    }

    protected string AggregatedClasses {
        get {
            return $"{base.AggregatedClasses} {SeparatorClass}";
        }
    }

    public FbGridContainer()
    {
        IsGrid = true;
    }

    /// <summary>Gap between rows and columns.</summary>
    private FbSpacing Gap { get; set; } = FbSpacing.None;

    /// <summary>Overrides <see cref="Gap"/> for rows.</summary>
    private FbSpacing RowGap { get; set; } = FbSpacing.None;

    /// <summary>Overrides <see cref="Gap"/> for columns.</summary>
    private FbSpacing ColumnGap { get; set; } = FbSpacing.None;

    // -------- ITEMS
    // align-items : vertical alignment
    private FbGridContainerItemAlignment AlignItems { get; set; } = FbGridContainerItemAlignment.Stretch;

    // justify-items : horizontal alignment
    private FbGridContainerItemAlignment JustifyItems { get; set; } = FbGridContainerItemAlignment.Stretch;

    // -------- ALL THE CONTENT (if gaps)
    // align-content : vertical alignment
    private FbGridContainerContentAlignment AlignContent { get; set; } = FbGridContainerContentAlignment.Normal;

    // justify-content : horizontal alignment
    private FbGridContainerContentAlignment JustifyContent { get; set; } = FbGridContainerContentAlignment.Normal;

    #endregion

    // ************************************************************************************************ //
    // ***************************************    PUBLIC   ******************************************** //
    // ************************************************************************************************ //

    /// <summary>Content placed on the grid.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>Number of equal-width columns. Valid values are 1 through 64.</summary>
    [Parameter]
    public int Columns { get; set; } = 12;

    [Parameter]
    public int? ColumnsXS { get; set; }

    [Parameter]
    public int? ColumnsS { get; set; }

    [Parameter]
    public int? ColumnsM { get; set; }

    [Parameter]
    public int? ColumnsL { get; set; }

    [Parameter]
    public int? ColumnsXL { get; set; }

    [Parameter]
    public int? ColumnsXXL { get; set; }

    /// <summary>
    /// Gap -> Small
    /// </summary>
    [Parameter]
    public bool GS { get => Gap == FbSpacing.S; set { if (value) Gap = FbSpacing.S; } }

    /// <summary>
    /// Gap -> Medium
    /// </summary>
    [Parameter]
    public bool GM { get => Gap == FbSpacing.M; set { if (value) Gap = FbSpacing.M; } }

    /// <summary>
    /// Gap -> Large
    /// </summary>
    [Parameter]
    public bool GL { get => Gap == FbSpacing.L; set { if (value) Gap = FbSpacing.L; } }

    /// <summary>
    /// Gap -> Extra Large
    /// </summary>
    [Parameter]
    public bool GX { get => Gap == FbSpacing.X; set { if (value) Gap = FbSpacing.X; } }

    [Parameter]
    public FbSpacing? GapXS { get; set; }

    [Parameter]
    public FbSpacing? GapS { get; set; }

    [Parameter]
    public FbSpacing? GapM { get; set; }

    [Parameter]
    public FbSpacing? GapL { get; set; }

    [Parameter]
    public FbSpacing? GapXL { get; set; }

    [Parameter]
    public FbSpacing? GapXXL { get; set; }

    /// <summary>
    /// Gap -> Row -> Small
    /// </summary>
    [Parameter]
    public bool GRS { get => RowGap == FbSpacing.S; set { if (value) RowGap = FbSpacing.S; } }

    /// <summary>
    /// Gap -> Row -> Medium
    /// </summary>
    [Parameter]
    public bool GRM { get => RowGap == FbSpacing.M; set { if (value) RowGap = FbSpacing.M; } }

    /// <summary>
    /// Gap -> Row -> Large
    /// </summary>
    [Parameter]
    public bool GRL { get => RowGap == FbSpacing.L; set { if (value) RowGap = FbSpacing.L; } }

    /// <summary>
    /// Gap -> Row -> Extra Large
    /// </summary>
    [Parameter]
    public bool GRX { get => RowGap == FbSpacing.X; set { if (value) RowGap = FbSpacing.X; } }

    /// <summary>
    /// Gap -> Column -> Small
    /// </summary>
    [Parameter]
    public bool GCS { get => RowGap == FbSpacing.S; set { if (value) RowGap = FbSpacing.S; } }

    /// <summary>
    /// Gap -> Column -> Medium
    /// </summary>
    [Parameter]
    public bool GCM { get => RowGap == FbSpacing.M; set { if (value) RowGap = FbSpacing.M; } }

    /// <summary>
    /// Gap -> Column -> Large
    /// </summary>
    [Parameter]
    public bool GCL { get => RowGap == FbSpacing.L; set { if (value) RowGap = FbSpacing.L; } }

    /// <summary>
    /// Gap -> Column -> Extra Large
    /// </summary>
    [Parameter]
    public bool GCX { get => RowGap == FbSpacing.X; set { if (value) RowGap = FbSpacing.X; } }

    /// <summary>Controls automatic item placement.</summary>
    [Parameter]
    public FbGridAutoFlow AutoFlow { get; set; } = FbGridAutoFlow.Row;

    /// <summary>
    /// Flow -> Row
    /// </summary>
    [Parameter]
    public bool FRow { get => AutoFlow == FbGridAutoFlow.Row; set { if (value) AutoFlow = FbGridAutoFlow.Row; } }

    /// <summary>
    /// Flow -> Column 
    /// </summary>
    [Parameter]
    public bool FColumn { get => AutoFlow == FbGridAutoFlow.Column; set { if (value) AutoFlow = FbGridAutoFlow.Column; } }

    /// <summary>
    /// Flow -> Row Dense
    /// </summary>
    [Parameter]
    public bool FRowDense { get => AutoFlow == FbGridAutoFlow.RowDense; set { if (value) AutoFlow = FbGridAutoFlow.RowDense; } }

    /// <summary>
    /// Flow -> Column Dense
    /// </summary>
    [Parameter]
    public bool FColumnDense { get => AutoFlow == FbGridAutoFlow.ColumnDense; set { if (value) AutoFlow = FbGridAutoFlow.ColumnDense; } }

    /// <summary>
    /// Optional raw CSS grid-template-columns value. When set, it takes precedence
    /// over <see cref="Columns"/> and its responsive variants.
    /// </summary>
    [Parameter]
    public string TemplateColumns { get; set; } = string.Empty;

    /// <summary>Optional raw CSS grid-template-rows value.</summary>
    [Parameter]
    public string TemplateRows { get; set; } = string.Empty;

    /// <summary>Optional raw CSS grid-auto-rows value.</summary>
    [Parameter]
    public string AutoRows { get; set; } = string.Empty;

    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }


    // -------- In Column
    /// <summary>
    /// Justify Items -> Vertical -> Default
    /// </summary>
    [Parameter]
    public bool JVD { get => AlignItems == FbGridContainerItemAlignment.Normal; set { if (value) AlignItems = FbGridContainerItemAlignment.Normal; } }

    /// <summary>
    /// Justify Items -> Vertical ->Start
    /// </summary>
    [Parameter]
    public bool JVS { get => AlignItems == FbGridContainerItemAlignment.Start; set { if (value) AlignItems = FbGridContainerItemAlignment.Start; } }

    /// <summary>
    /// Justify Items -> Vertical ->End
    /// </summary>
    [Parameter]
    public bool JVE { get => AlignItems == FbGridContainerItemAlignment.End; set { if (value) AlignItems = FbGridContainerItemAlignment.End; } }

    /// <summary>
    /// Justify Items -> Vertical ->Center
    /// </summary>
    [Parameter]
    public bool JVC { get => AlignItems == FbGridContainerItemAlignment.Center; set { if (value) AlignItems = FbGridContainerItemAlignment.Center; } }

    /// <summary>
    /// Justify Items -> Vertical ->Stretch
    /// </summary>
    [Parameter]
    public bool JVSt { get => AlignItems == FbGridContainerItemAlignment.Stretch; set { if (value) AlignItems = FbGridContainerItemAlignment.Stretch; } }

    /// <summary>
    /// Justify Items -> Vertical ->Baseline
    /// </summary>
    [Parameter]
    public bool JVBl { get => AlignItems == FbGridContainerItemAlignment.Baseline; set { if (value) AlignItems = FbGridContainerItemAlignment.Baseline; } }

    // -------- In Column
    /// <summary>
    /// Justify Items -> Horizontal -> Default
    /// </summary>
    [Parameter]
    public bool JHD { get => JustifyItems == FbGridContainerItemAlignment.Normal; set { if (value) JustifyItems = FbGridContainerItemAlignment.Normal; } }

    /// <summary>
    /// Justify Items -> Horizontal ->Start
    /// </summary>
    [Parameter]
    public bool JHS { get => JustifyItems == FbGridContainerItemAlignment.Start; set { if (value) JustifyItems = FbGridContainerItemAlignment.Start; } }

    /// <summary>
    /// Justify Items -> Horizontal ->End
    /// </summary>
    [Parameter]
    public bool JHE { get => JustifyItems == FbGridContainerItemAlignment.End; set { if (value) JustifyItems = FbGridContainerItemAlignment.End; } }

    /// <summary>
    /// Justify Items -> Horizontal ->Center
    /// </summary>
    [Parameter]
    public bool JHC { get => JustifyItems == FbGridContainerItemAlignment.Center; set { if (value) JustifyItems = FbGridContainerItemAlignment.Center; } }

    /// <summary>
    /// Justify Items -> Horizontal ->Stretch
    /// </summary>
    [Parameter]
    public bool JHSt { get => JustifyItems == FbGridContainerItemAlignment.Stretch; set { if (value) JustifyItems = FbGridContainerItemAlignment.Stretch; } }

    /// <summary>
    /// Justify Items -> Horizontal ->Stretch
    /// </summary>
    [Parameter]
    public bool JHBl { get => JustifyItems == FbGridContainerItemAlignment.Baseline; set { if (value) JustifyItems = FbGridContainerItemAlignment.Baseline; } }

    // -------- In Column
    /// <summary>
    /// Justify Items -> Vertical -> Content -> Default
    /// </summary>
    [Parameter]
    public bool JVCD { get => AlignContent == FbGridContainerContentAlignment.Normal; set { if (value) AlignContent = FbGridContainerContentAlignment.Normal; } }

    /// <summary>
    /// Justify Items -> Vertical -> Content -> Start
    /// </summary>
    [Parameter]
    public bool JVCS { get => AlignContent == FbGridContainerContentAlignment.Start; set { if (value) AlignContent = FbGridContainerContentAlignment.Start; } }

    /// <summary>
    /// Justify Items -> Vertical -> Content -> End
    /// </summary>
    [Parameter]
    public bool JVCE { get => AlignContent == FbGridContainerContentAlignment.End; set { if (value) AlignContent = FbGridContainerContentAlignment.End; } }

    /// <summary>
    /// Justify Items -> Vertical -> Content -> Center
    /// </summary>
    [Parameter]
    public bool JVCC { get => AlignContent == FbGridContainerContentAlignment.Center; set { if (value) AlignContent = FbGridContainerContentAlignment.Center; } }

    /// <summary>
    /// Justify Items -> Vertical -> Content -> Stretch
    /// </summary>
    [Parameter]
    public bool JVCSt { get => AlignContent == FbGridContainerContentAlignment.Stretch; set { if (value) AlignContent = FbGridContainerContentAlignment.Stretch; } }

    /// <summary>
    /// Justify Items -> Vertical -> Content -> Space Between
    /// </summary>
    [Parameter]
    public bool JVCSB { get => AlignContent == FbGridContainerContentAlignment.SpaceBetween; set { if (value) AlignContent = FbGridContainerContentAlignment.SpaceBetween; } }

    /// <summary>
    /// Justify Items -> Vertical -> Content -> Space Around
    /// </summary>
    [Parameter]
    public bool JVCSA { get => AlignContent == FbGridContainerContentAlignment.SpaceAround; set { if (value) AlignContent = FbGridContainerContentAlignment.SpaceAround; } }

    /// <summary>
    /// Justify Items -> Vertical -> Content -> Space Evenly
    /// </summary>
    [Parameter]
    public bool JVCSE { get => AlignContent == FbGridContainerContentAlignment.SpaceEvenly; set { if (value) AlignContent = FbGridContainerContentAlignment.SpaceEvenly; } }


    // -------- JUSTIFY
    /// <summary>
    /// Justify Items -> Vertical -> Content -> Default
    /// </summary>
    [Parameter]
    public bool JHCD { get => JustifyContent == FbGridContainerContentAlignment.Normal; set { if (value) JustifyContent = FbGridContainerContentAlignment.Normal; } }

    /// <summary>
    /// Justify Items -> Vertical -> Content -> Start
    /// </summary>
    [Parameter]
    public bool JHCS { get => JustifyContent == FbGridContainerContentAlignment.Start; set { if (value) JustifyContent = FbGridContainerContentAlignment.Start; } }

    /// <summary>
    /// Justify Items -> Vertical -> Content -> End
    /// </summary>
    [Parameter]
    public bool JHCE { get => JustifyContent == FbGridContainerContentAlignment.End; set { if (value) JustifyContent = FbGridContainerContentAlignment.End; } }

    /// <summary>
    /// Justify Items -> Vertical -> Content -> Center
    /// </summary>
    [Parameter]
    public bool JHCC { get => JustifyContent == FbGridContainerContentAlignment.Center; set { if (value) JustifyContent = FbGridContainerContentAlignment.Center; } }

    /// <summary>
    /// Justify Items -> Vertical -> Content -> Stretch
    /// </summary>
    [Parameter]
    public bool JHCSt { get => JustifyContent == FbGridContainerContentAlignment.Stretch; set { if (value) JustifyContent = FbGridContainerContentAlignment.Stretch; } }

    /// <summary>
    /// Justify Items -> Vertical -> Content -> Space Between
    /// </summary>
    [Parameter]
    public bool JHCSB { get => JustifyContent == FbGridContainerContentAlignment.SpaceBetween; set { if (value) JustifyContent = FbGridContainerContentAlignment.SpaceBetween; } }

    /// <summary>
    /// Justify Items -> Vertical -> Content -> Space Around
    /// </summary>
    [Parameter]
    public bool JHCSA { get => JustifyContent == FbGridContainerContentAlignment.SpaceAround; set { if (value) JustifyContent = FbGridContainerContentAlignment.SpaceAround; } }

    /// <summary>
    /// Justify Items -> Vertical -> Content -> Space Evenly
    /// </summary>
    [Parameter]
    public bool JHCSE { get => JustifyContent == FbGridContainerContentAlignment.SpaceEvenly; set { if (value) JustifyContent = FbGridContainerContentAlignment.SpaceEvenly; } }


    protected override void OnParametersSet()
    {
        ValidateRange(Columns, nameof(Columns));
        ValidateRange(ColumnsXS, nameof(ColumnsXS));
        ValidateRange(ColumnsS, nameof(ColumnsS));
        ValidateRange(ColumnsM, nameof(ColumnsM));
        ValidateRange(ColumnsL, nameof(ColumnsL));
        ValidateRange(ColumnsXL, nameof(ColumnsXL));
        ValidateRange(ColumnsXXL, nameof(ColumnsXXL));
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, "class", JoinClasses("fb-grid-container", $"{AggregatedClasses}"));
        builder.AddAttribute(3, "style", BuildStyle());

        builder.OpenElement(4, "div");
        builder.AddAttribute(5, "class", "fb-grid");
        builder.AddContent(6, ChildContent);
        builder.CloseElement();

        builder.CloseElement();
    }

    private string BuildStyle()
    {
        var css = new StringBuilder(256);
        if (!string.IsNullOrWhiteSpace(base.AggregatedStyles))
            css.Append(base.AggregatedStyles.Trim());

        Add(css, "--fb-grid-columns", Columns);
        Add(css, "--fb-grid-columns-xs", ColumnsXS);
        Add(css, "--fb-grid-columns-s", ColumnsS);
        Add(css, "--fb-grid-columns-m", ColumnsM);
        Add(css, "--fb-grid-columns-l", ColumnsL);
        Add(css, "--fb-grid-columns-xl", ColumnsXL);
        Add(css, "--fb-grid-columns-xxl", ColumnsXXL);

        Add(css, "--fb-grid-gap", Gap);
        Add(css, "--fb-grid-gap-xs", GapXS);
        Add(css, "--fb-grid-gap-s", GapS);
        Add(css, "--fb-grid-gap-m", GapM);
        Add(css, "--fb-grid-gap-l", GapL);
        Add(css, "--fb-grid-gap-xl", GapXL);
        Add(css, "--fb-grid-gap-xxl", GapXXL);

        if (RowGap is not FbSpacing.None)
            Add(css, "--fb-grid-row-gap", SpacingToCss(RowGap));
        if (ColumnGap is not FbSpacing.None)
            Add(css, "--fb-grid-column-gap", SpacingToCss(ColumnGap));

        Add(css, "--fb-grid-auto-flow", FbGridCss.AutoFlow(AutoFlow));
        Add(css, "--fb-grid-align-items", FbGridCss.TrackAlignment(AlignItems));
        Add(css, "--fb-grid-justify-items", FbGridCss.TrackAlignment(JustifyItems));
        Add(css, "--fb-grid-align-content", FbGridCss.ContentAlignment(AlignContent));
        Add(css, "--fb-grid-justify-content", FbGridCss.ContentAlignment(JustifyContent));

        if (!string.IsNullOrWhiteSpace(TemplateColumns))
            Add(css, "--fb-grid-template-columns", TemplateColumns);
        if (!string.IsNullOrWhiteSpace(TemplateRows))
            Add(css, "--fb-grid-template-rows", TemplateRows);
        if (!string.IsNullOrWhiteSpace(AutoRows))
            Add(css, "--fb-grid-auto-rows", AutoRows);

        return css.ToString();
    }

    private static void Add(StringBuilder css, string property, int? value)
    {
        if (value.HasValue)
            Add(css, property, value.Value.ToString(System.Globalization.CultureInfo.InvariantCulture));
    }

    private static void Add(StringBuilder css, string property, FbSpacing value)
    {
        if (value is not FbSpacing.None)
            Add(css, property, SpacingToCss(value));
    }

    private static void Add(StringBuilder css, string property, FbSpacing? value)
    {
        if (value.HasValue && value.Value is not FbSpacing.None)
            Add(css, property, SpacingToCss(value.Value));
    }

    private static string SpacingToCss(FbSpacing value) => FbLayoutHelper.ToSpacingCss(value);

    private static void Add(StringBuilder css, string property, string value) =>
        css.Append(property).Append(':').Append(value).Append(';');

    private static string JoinClasses(string required, string optional) =>
        string.IsNullOrWhiteSpace(optional) ? required : $"{required} {optional.Trim()}";

    private static void ValidateRange(int? value, string parameterName)
    {
        if (value is < 1 or > 64)
            throw new ArgumentOutOfRangeException(parameterName, value, "Grid columns must be between 1 and 64.");
    }
}
