namespace FractalBlazor.Components.Layout;

/// <summary>Controls how the CSS Grid auto-placement algorithm fills the grid.</summary>
public enum FbGridAutoFlow : byte
{
    Row,
    Column,
    RowDense,
    ColumnDense
}

// -------- ITEMS
/// <summary>Alignment of grid items inside their grid areas.</summary>
public enum FbGridItemAlignment : byte
{
    Auto,
    Normal,
    Start,
    End,
    Center,
    Stretch,
    Baseline
}

/// <summary>Justify of grid items inside their grid areas.</summary>
public enum FbGridItemJustify : byte
{
    Auto,
    Start,
    End,
    Center,
    Stretch
}

// -------- CONTAINNER
/// <summary>Default alignment of items within all grid areas.</summary>
public enum FbGridContainerItemAlignment : byte
{
    Normal,
    Start,
    End,
    Center,
    Stretch,
    Baseline
}

/// <summary>Alignment of grid tracks when the grid is smaller than its container.</summary>
public enum FbGridContainerContentAlignment : byte
{
    Normal,
    Start,
    End,
    Center,
    Stretch,
    SpaceBetween,
    SpaceAround,
    SpaceEvenly
}

internal static class FbGridCss
{
    internal static string AutoFlow(FbGridAutoFlow value) => value switch
    {
        FbGridAutoFlow.Column => "column",
        FbGridAutoFlow.RowDense => "row dense",
        FbGridAutoFlow.ColumnDense => "column dense",
        _ => "row"
    };

    internal static string ItemAlignment(FbGridItemAlignment value) => value switch
    {
        FbGridItemAlignment.Normal => "normal",
        FbGridItemAlignment.Start => "start",
        FbGridItemAlignment.End => "end",
        FbGridItemAlignment.Center => "center",
        FbGridItemAlignment.Stretch => "stretch",
        FbGridItemAlignment.Baseline => "baseline",
        _ => "auto"
    };

    internal static string ItemAlignment(FbGridItemJustify value) => value switch {
        FbGridItemJustify.Start => "start",
        FbGridItemJustify.End => "end",
        FbGridItemJustify.Center => "center",
        FbGridItemJustify.Stretch => "stretch",
        _ => "auto"
    };

    internal static string TrackAlignment(FbGridContainerItemAlignment value) => value switch
    {
        FbGridContainerItemAlignment.Start => "start",
        FbGridContainerItemAlignment.End => "end",
        FbGridContainerItemAlignment.Center => "center",
        FbGridContainerItemAlignment.Stretch => "stretch",
        FbGridContainerItemAlignment.Baseline => "baseline",
        _ => "normal"
    };

    internal static string ContentAlignment(FbGridContainerContentAlignment value) => value switch
    {
        FbGridContainerContentAlignment.Start => "start",
        FbGridContainerContentAlignment.End => "end",
        FbGridContainerContentAlignment.Center => "center",
        FbGridContainerContentAlignment.Stretch => "stretch",
        FbGridContainerContentAlignment.SpaceBetween => "space-between",
        FbGridContainerContentAlignment.SpaceAround => "space-around",
        FbGridContainerContentAlignment.SpaceEvenly => "space-evenly",
        _ => "normal"
    };
}
