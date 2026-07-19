namespace FractalBlazor.Components.Layout;

/// <summary>Controls how the CSS Grid auto-placement algorithm fills the grid.</summary>
public enum FbGridAutoFlow : byte
{
    Row,
    Column,
    RowDense,
    ColumnDense
}

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

/// <summary>Default alignment of items within all grid areas.</summary>
public enum FbGridTrackAlignment : byte
{
    Normal,
    Start,
    End,
    Center,
    Stretch,
    Baseline
}

/// <summary>Alignment of grid tracks when the grid is smaller than its container.</summary>
public enum FbGridContentAlignment : byte
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

    internal static string TrackAlignment(FbGridTrackAlignment value) => value switch
    {
        FbGridTrackAlignment.Start => "start",
        FbGridTrackAlignment.End => "end",
        FbGridTrackAlignment.Center => "center",
        FbGridTrackAlignment.Stretch => "stretch",
        FbGridTrackAlignment.Baseline => "baseline",
        _ => "normal"
    };

    internal static string ContentAlignment(FbGridContentAlignment value) => value switch
    {
        FbGridContentAlignment.Start => "start",
        FbGridContentAlignment.End => "end",
        FbGridContentAlignment.Center => "center",
        FbGridContentAlignment.Stretch => "stretch",
        FbGridContentAlignment.SpaceBetween => "space-between",
        FbGridContentAlignment.SpaceAround => "space-around",
        FbGridContentAlignment.SpaceEvenly => "space-evenly",
        _ => "normal"
    };
}
