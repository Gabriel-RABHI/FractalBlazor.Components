namespace FractalBlazor.WebSite.StateModel.Tests;

/// <summary>
/// A small catalog of flags, enough to drive the assistant: padding that reads two ways
/// (<c>PL</c> is Large, <c>PLS</c> is Left then Small), a margin, two text flags of their
/// own family, and a word.
/// </summary>
internal sealed class TestFlagCatalog : IFlagCatalog
{
    public static readonly FlagToken Padding = new("P", "Padding", "Spacing");
    public static readonly FlagToken Margin = new("M", "Margin", "Spacing");
    public static readonly FlagToken Small = new("S", "Small", "Spacing");
    public static readonly FlagToken Large = new("L", "Large", "Spacing");
    public static readonly FlagToken Horizontal = new("H", "Horizontal", "Direction");
    public static readonly FlagToken Left = new("L", "Left", "Direction");
    public static readonly FlagToken Medium = new("M", "Medium", "Size");
    public static readonly FlagToken Bold = new("B", "Bold", "Weight");
    public static readonly FlagToken NoFlex = new("NoFlex", "Keeps its content size", "Layout");

    private static readonly string[] Boxes = ["FbButton", "FbRow", "FbStack"];
    private static readonly string[] Flexes = ["FbRow", "FbStack"];

    public IReadOnlyList<FlagInfo> Flags { get; } =
    [
        new("PS", [Padding, Small], Boxes),
        new("PL", [Padding, Large], Boxes),
        new("PHS", [Padding, Horizontal, Small], Boxes),
        new("PLS", [Padding, Left, Small], Flexes),
        new("PLL", [Padding, Left, Large], Flexes),
        new("MS", [Margin, Small], ["FbRow", "FbRowItem", "FbStack"]),
        new("M", [Medium], ["FbText"]),
        new("B", [Bold], ["FbText"]),
        new("NoFlex", [NoFlex], Flexes)
    ];

    public IReadOnlyList<string> Components { get; } = ["FbButton", "FbRow", "FbRowItem", "FbStack", "FbText"];
}
