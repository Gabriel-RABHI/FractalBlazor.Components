using System.Globalization;
using System.Text;
using FractalBlazor.Components.Layout.Abstracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout;

/// <summary>
/// An item placed in an <see cref="FbGridContainer"/>. Every responsive
/// parameter is implemented with CSS custom properties and container queries.
/// </summary>
public sealed class FbGridItem : FbLayoutVisibleComponentBase
{
    private string _cachedStyle = null;

    public FbGridItem()
    {
        IsGrid = true;
    }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public int ColumnSpan { get; set; } = 1;
    [Parameter] public int? ColumnSpanXS { get; set; }
    [Parameter] public int? ColumnSpanS { get; set; }
    [Parameter] public int? ColumnSpanM { get; set; }
    [Parameter] public int? ColumnSpanL { get; set; }
    [Parameter] public int? ColumnSpanXL { get; set; }
    [Parameter] public int? ColumnSpanXXL { get; set; }

    /// <summary>One-based column line. Null lets CSS auto-place the item.</summary>
    [Parameter] public int? ColumnStart { get; set; }
    [Parameter] public int? ColumnStartXS { get; set; }
    [Parameter] public int? ColumnStartS { get; set; }
    [Parameter] public int? ColumnStartM { get; set; }
    [Parameter] public int? ColumnStartL { get; set; }
    [Parameter] public int? ColumnStartXL { get; set; }
    [Parameter] public int? ColumnStartXXL { get; set; }

    [Parameter] public int RowSpan { get; set; } = 1;
    [Parameter] public int? RowSpanXS { get; set; }
    [Parameter] public int? RowSpanS { get; set; }
    [Parameter] public int? RowSpanM { get; set; }
    [Parameter] public int? RowSpanL { get; set; }
    [Parameter] public int? RowSpanXL { get; set; }
    [Parameter] public int? RowSpanXXL { get; set; }

    /// <summary>One-based row line. Null lets CSS auto-place the item.</summary>
    [Parameter] public int? RowStart { get; set; }
    [Parameter] public int? RowStartXS { get; set; }
    [Parameter] public int? RowStartS { get; set; }
    [Parameter] public int? RowStartM { get; set; }
    [Parameter] public int? RowStartL { get; set; }
    [Parameter] public int? RowStartXL { get; set; }
    [Parameter] public int? RowStartXXL { get; set; }

    [Parameter] public int Order { get; set; }
    [Parameter] public int? OrderXS { get; set; }
    [Parameter] public int? OrderS { get; set; }
    [Parameter] public int? OrderM { get; set; }
    [Parameter] public int? OrderL { get; set; }
    [Parameter] public int? OrderXL { get; set; }
    [Parameter] public int? OrderXXL { get; set; }

    private FbGridItemAlignment AlignSelf { get; set; } = FbGridItemAlignment.Auto;

    private FbGridItemJustify JustifySelf { get; set; } = FbGridItemJustify.Auto;

    // -------- In Column
    /// <summary>
    /// Self -> Justify -> Vertical -> Default
    /// </summary>
    [Parameter]
    public bool SJVD { get => AlignSelf == FbGridItemAlignment.Auto; set { if (value) AlignSelf = FbGridItemAlignment.Auto; } }

    /// <summary>
    /// Self -> Justify -> Vertical -> Normal
    /// </summary>
    [Parameter]
    public bool SJVN { get => AlignSelf == FbGridItemAlignment.Normal; set { if (value) AlignSelf = FbGridItemAlignment.Normal; } }

    /// <summary>
    /// Self -> Justify -> Vertical ->Start
    /// </summary>
    [Parameter]
    public bool SJVS { get => AlignSelf == FbGridItemAlignment.Start; set { if (value) AlignSelf = FbGridItemAlignment.Start; } }

    /// <summary>
    /// Self -> Justify -> Vertical ->End
    /// </summary>
    [Parameter]
    public bool SJVE { get => AlignSelf == FbGridItemAlignment.End; set { if (value) AlignSelf = FbGridItemAlignment.End; } }

    /// <summary>
    /// Self -> Justify -> Vertical ->Center
    /// </summary>
    [Parameter]
    public bool SJVC { get => AlignSelf == FbGridItemAlignment.Center; set { if (value) AlignSelf = FbGridItemAlignment.Center; } }

    /// <summary>
    /// Self -> Justify -> Vertical ->Stretch
    /// </summary>
    [Parameter]
    public bool SJVSt { get => AlignSelf == FbGridItemAlignment.Stretch; set { if (value) AlignSelf = FbGridItemAlignment.Stretch; } }

    /// <summary>
    /// Self -> Justify -> Vertical ->Baseline
    /// </summary>
    [Parameter]
    public bool SJVBl { get => AlignSelf == FbGridItemAlignment.Baseline; set { if (value) AlignSelf = FbGridItemAlignment.Baseline; } }

    // -------- In Column
    /// <summary>
    /// Self -> Justify -> Horizontal -> Default
    /// </summary>
    [Parameter]
    public bool SJHD { get => JustifySelf == FbGridItemJustify.Auto; set { if (value) JustifySelf = FbGridItemJustify.Auto; } }

    /// <summary>
    /// Self -> Justify -> Horizontal ->Start
    /// </summary>
    [Parameter]
    public bool SJHS { get => JustifySelf == FbGridItemJustify.Start; set { if (value) JustifySelf = FbGridItemJustify.Start; } }

    /// <summary>
    /// Self -> Justify -> Horizontal ->End
    /// </summary>
    [Parameter]
    public bool SJHE { get => JustifySelf == FbGridItemJustify.End; set { if (value) JustifySelf = FbGridItemJustify.End; } }

    /// <summary>
    /// Self -> Justify -> Horizontal ->Center
    /// </summary>
    [Parameter]
    public bool SJHC { get => JustifySelf == FbGridItemJustify.Center; set { if (value) JustifySelf = FbGridItemJustify.Center; } }

    /// <summary>
    /// Self -> Justify -> Horizontal ->Stretch
    /// </summary>
    [Parameter]
    public bool SJHSt { get => JustifySelf == FbGridItemJustify.Stretch; set { if (value) JustifySelf = FbGridItemJustify.Stretch; } }

    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    protected override void OnParametersSet()
    {
        ValidateSpan(ColumnSpan, nameof(ColumnSpan));
        ValidateSpan(ColumnSpanXS, nameof(ColumnSpanXS));
        ValidateSpan(ColumnSpanS, nameof(ColumnSpanS));
        ValidateSpan(ColumnSpanM, nameof(ColumnSpanM));
        ValidateSpan(ColumnSpanL, nameof(ColumnSpanL));
        ValidateSpan(ColumnSpanXL, nameof(ColumnSpanXL));
        ValidateSpan(ColumnSpanXXL, nameof(ColumnSpanXXL));

        ValidateSpan(RowSpan, nameof(RowSpan));
        ValidateSpan(RowSpanXS, nameof(RowSpanXS));
        ValidateSpan(RowSpanS, nameof(RowSpanS));
        ValidateSpan(RowSpanM, nameof(RowSpanM));
        ValidateSpan(RowSpanL, nameof(RowSpanL));
        ValidateSpan(RowSpanXL, nameof(RowSpanXL));
        ValidateSpan(RowSpanXXL, nameof(RowSpanXXL));

        ValidateStart(ColumnStart, nameof(ColumnStart));
        ValidateStart(ColumnStartXS, nameof(ColumnStartXS));
        ValidateStart(ColumnStartS, nameof(ColumnStartS));
        ValidateStart(ColumnStartM, nameof(ColumnStartM));
        ValidateStart(ColumnStartL, nameof(ColumnStartL));
        ValidateStart(ColumnStartXL, nameof(ColumnStartXL));
        ValidateStart(ColumnStartXXL, nameof(ColumnStartXXL));

        ValidateStart(RowStart, nameof(RowStart));
        ValidateStart(RowStartXS, nameof(RowStartXS));
        ValidateStart(RowStartS, nameof(RowStartS));
        ValidateStart(RowStartM, nameof(RowStartM));
        ValidateStart(RowStartL, nameof(RowStartL));
        ValidateStart(RowStartXL, nameof(RowStartXL));
        ValidateStart(RowStartXXL, nameof(RowStartXXL));
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, "class", $"fb-grid-item {Classes.Trim()} {AggregatedClasses}");
        builder.AddAttribute(3, "style", BuildStyle());
        builder.AddContent(4, ChildContent);
        builder.CloseElement();
    }

    private string BuildStyle()
    {
        var css = new StringBuilder(384);
        if (!string.IsNullOrWhiteSpace(AggregatedStyles))
            css.Append(AggregatedStyles.Trim());

        Add(css, "--fb-grid-column-span", ColumnSpan);
        AddResponsive(css, "column-span", ColumnSpanXS, ColumnSpanS, ColumnSpanM, ColumnSpanL, ColumnSpanXL, ColumnSpanXXL);
        Add(css, "--fb-grid-column-start", ColumnStart);
        AddResponsive(css, "column-start", ColumnStartXS, ColumnStartS, ColumnStartM, ColumnStartL, ColumnStartXL, ColumnStartXXL);
        Add(css, "--fb-grid-row-span", RowSpan);
        AddResponsive(css, "row-span", RowSpanXS, RowSpanS, RowSpanM, RowSpanL, RowSpanXL, RowSpanXXL);
        Add(css, "--fb-grid-row-start", RowStart);
        AddResponsive(css, "row-start", RowStartXS, RowStartS, RowStartM, RowStartL, RowStartXL, RowStartXXL);
        Add(css, "--fb-grid-order", Order);
        AddResponsive(css, "order", OrderXS, OrderS, OrderM, OrderL, OrderXL, OrderXXL);
        Add(css, "--fb-grid-align-self", FbGridCss.ItemAlignment(AlignSelf));
        Add(css, "--fb-grid-justify-self", FbGridCss.ItemAlignment(JustifySelf));

        if (!string.IsNullOrWhiteSpace(Style))
        {
            css.Append(Style.Trim());
            if (css[^1] != ';')
                css.Append(';');
        }

        return css.ToString();
    }

    private static void AddResponsive(StringBuilder css, string name, int? xs, int? s, int? m, int? l, int? xl, int? xxl)
    {
        Add(css, $"--fb-grid-{name}-xs", xs);
        Add(css, $"--fb-grid-{name}-s", s);
        Add(css, $"--fb-grid-{name}-m", m);
        Add(css, $"--fb-grid-{name}-l", l);
        Add(css, $"--fb-grid-{name}-xl", xl);
        Add(css, $"--fb-grid-{name}-xxl", xxl);
    }

    private static void Add(StringBuilder css, string property, int? value)
    {
        if (value.HasValue)
            Add(css, property, value.Value.ToString(CultureInfo.InvariantCulture));
    }

    private static void Add(StringBuilder css, string property, string value) =>
        css.Append(property).Append(':').Append(value).Append(';');

    private static void ValidateSpan(int? value, string parameterName)
    {
        if (value is < 1 or > 64)
            throw new ArgumentOutOfRangeException(parameterName, value, "Grid spans must be between 1 and 64.");
    }

    private static void ValidateStart(int? value, string parameterName)
    {
        if (value is < 1 or > 65)
            throw new ArgumentOutOfRangeException(parameterName, value, "Grid start lines must be between 1 and 65.");
    }
}
