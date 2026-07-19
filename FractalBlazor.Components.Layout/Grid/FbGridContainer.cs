using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout;

/// <summary>
/// A container-query-based CSS Grid. Responsive values are relative to this
/// component's width, not to the browser viewport.
/// </summary>
public class FbGridContainer : FbComponentBase
{
    /// <summary>Content placed on the grid.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>Number of equal-width columns. Valid values are 1 through 64.</summary>
    [Parameter]
    public int Columns { get; set; } = 12;

    [Parameter] public int? ColumnsXS { get; set; }
    [Parameter] public int? ColumnsS { get; set; }
    [Parameter] public int? ColumnsM { get; set; }
    [Parameter] public int? ColumnsL { get; set; }
    [Parameter] public int? ColumnsXL { get; set; }
    [Parameter] public int? ColumnsXXL { get; set; }

    /// <summary>Gap between rows and columns.</summary>
    [Parameter]
    public FbSpacing Gap { get; set; } = FbSpacing._0;

    [Parameter] public FbSpacing? GapXS { get; set; }
    [Parameter] public FbSpacing? GapS { get; set; }
    [Parameter] public FbSpacing? GapM { get; set; }
    [Parameter] public FbSpacing? GapL { get; set; }
    [Parameter] public FbSpacing? GapXL { get; set; }
    [Parameter] public FbSpacing? GapXXL { get; set; }

    /// <summary>Overrides <see cref="Gap"/> for rows.</summary>
    [Parameter]
    public FbSpacing RowGap { get; set; } = FbSpacing.None;

    /// <summary>Overrides <see cref="Gap"/> for columns.</summary>
    [Parameter]
    public FbSpacing ColumnGap { get; set; } = FbSpacing.None;

    /// <summary>Controls automatic item placement.</summary>
    [Parameter]
    public FbGridAutoFlow AutoFlow { get; set; } = FbGridAutoFlow.Row;

    [Parameter]
    public FbGridTrackAlignment AlignItems { get; set; } = FbGridTrackAlignment.Stretch;

    [Parameter]
    public FbGridTrackAlignment JustifyItems { get; set; } = FbGridTrackAlignment.Stretch;

    [Parameter]
    public FbGridContentAlignment AlignContent { get; set; } = FbGridContentAlignment.Normal;

    [Parameter]
    public FbGridContentAlignment JustifyContent { get; set; } = FbGridContentAlignment.Normal;

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

    [Parameter]
    public string Classes { get; set; } = string.Empty;

    [Parameter]
    public string Style { get; set; } = string.Empty;

    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

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
        builder.AddAttribute(2, "class", JoinClasses("fb-grid-container", Classes));
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

        AppendCustomStyle(css, Style);
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

    private static string SpacingToCss(FbSpacing value) =>
        value is FbSpacing._0 ? "0" : FbLayoutPresets.ToSpacingCss(value);

    private static void Add(StringBuilder css, string property, string value) =>
        css.Append(property).Append(':').Append(value).Append(';');

    private static void AppendCustomStyle(StringBuilder css, string style)
    {
        if (string.IsNullOrWhiteSpace(style))
            return;

        css.Append(style.Trim());
        if (css[^1] != ';')
            css.Append(';');
    }

    private static string JoinClasses(string required, string optional) =>
        string.IsNullOrWhiteSpace(optional) ? required : $"{required} {optional.Trim()}";

    private static void ValidateRange(int? value, string parameterName)
    {
        if (value is < 1 or > 64)
            throw new ArgumentOutOfRangeException(parameterName, value, "Grid columns must be between 1 and 64.");
    }
}

/// <summary>
/// Backwards-compatible spelling matching the existing Containner components.
/// Prefer <see cref="FbGridContainer"/> in new code.
/// </summary>
[Obsolete("Use FbGridContainer. This alias only preserves the existing Containner spelling.")]
public sealed class FbGridContainner : FbGridContainer
{
}
