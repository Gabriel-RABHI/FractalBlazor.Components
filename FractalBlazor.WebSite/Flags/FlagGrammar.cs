using FractalBlazor.WebSite.StateModel;

namespace FractalBlazor.WebSite.Flags;

/// <summary>
/// What the letters of a flag name stand for. Each family has its own vocabulary, named after
/// the type that declares the flags: in a layout component <c>M</c> starts a margin, on
/// <c>FbText</c> it is the medium size. A vocabulary is a tree of parts; walking it from the
/// root, a path that ends on a complete part spells a flag. A name spelled twice in one
/// family is an error: its reading would be ambiguous.
/// </summary>
internal sealed class FlagGrammar
{
    /// <summary>
    /// A flag name read in a family: its parts, and its rank in the grammar, which orders the
    /// catalog.
    /// </summary>
    internal sealed record Reading(string Name, IReadOnlyList<FlagToken> Tokens, int Order);

    private sealed record Part(string Code, string Meaning, string Category, bool Complete, IReadOnlyList<Part> Children);

    private readonly Dictionary<string, Dictionary<string, Reading>> _families = new(StringComparer.Ordinal);
    private int _order;

    private FlagGrammar()
    {
    }

    public bool TryRead(string family, string name, out Reading reading)
    {
        reading = default!;
        return _families.TryGetValue(family, out var names) && names.TryGetValue(name, out reading!);
    }

    /// <summary>
    /// The reading of a flag no family explains: its name, alone, after every other flag.
    /// </summary>
    public static Reading Unexplained(string name) => new(name, [new FlagToken(name, name, "Other")], int.MaxValue);

    public static FlagGrammar Build()
    {
        var grammar = new FlagGrammar();
        grammar.Add(Layout(),
            "FbLayoutComponentBase", "FbLayoutSurfaceComponentBase", "FbFlexBoxBase", "FbRow", "FbStack",
            "FbRowItem", "FbStackItem", "FbRowSeparator", "FbStackSeparator", "FbGridContainer", "FbGridItem");
        grammar.Add(Text(), "FbText");
        grammar.Add(Icon(), "FbIcon");
        grammar.Add(Breaks(), "FbRowBreak", "FbStackBreak");
        grammar.Add(
            [
                Word("Left", "Slides from the left", "Side"),
                Word("Right", "Slides from the right", "Side"),
                Word("Top", "Slides from the top", "Side"),
                Word("Bottom", "Slides from the bottom", "Side"),
                Word("Modal", "Over the whole screen", "Mode"),
                Word("Inline", "In its place, pushes the layout", "Mode"),
                Word("InlineModal", "In its place, over its zone", "Mode")
            ],
            "FbDrawer");
        grammar.Add(
            [
                Word("Mirror", "Box on the right", "Placement"),
                Word("Circle", "Round box", "Shape"),
                Word("Top", "Box at the top of the content", "Alignment"),
                Word("Center", "Box centered on the content", "Alignment"),
                Word("Bottom", "Box at the bottom of the content", "Alignment")
            ],
            "FbCheckbox");
        grammar.Add(
            [
                Word("AutoToggle", "A click on the anchor opens and closes it", "Behavior"),
                Word("Block", "Takes the full width", "Layout")
            ],
            "FbDropDown");
        grammar.Add(
            [
                Word("Automatic", "Shown while hovered", "Visibility"),
                Word("Visible", "Shown, whatever the mouse", "Visibility"),
                Word("Fixed", "Always shown", "Visibility")
            ],
            "FbShowWhenOver");
        grammar.Add([Word("Reset", "Waits again once out of view", "Behavior")], "FbShowWhenVisible");
        grammar.Add(
            [
                Word("Periodic", "Updates at each interval", "Behavior"),
                Word("OneTime", "Updates once", "Behavior")
            ],
            "FbUpdatable");
        grammar.Add([Word("Scoped", "Themes its content only", "Theme")], "FbTheme", "FbLayoutTheme");
        grammar.Add([Word("Invert", "Swaps the light and dark anchors", "Theme")], "FbVariant", "FbLayoutVariant");
        grammar.Add(
            [
                Word("D", "Disabled", "Behavior"),
                Word("Submit", "Submits the enclosing form", "Behavior")
            ],
            "FbButton");
        grammar.Add([Word("D", "Disabled", "Behavior")], "FbInputBase");
        grammar.Add([Word("Immediate", "Sends each keystroke", "Behavior")], "FbFieldBase");
        return grammar;
    }

    private void Add(IReadOnlyList<Part> roots, params string[] families)
    {
        var names = new Dictionary<string, Reading>(StringComparer.Ordinal);
        Spell(roots, "", [], names);
        foreach (var family in families)
            _families[family] = names;
    }

    private void Spell(IReadOnlyList<Part> parts, string prefix, List<FlagToken> path, Dictionary<string, Reading> names)
    {
        foreach (var part in parts)
        {
            var name = prefix + part.Code;
            path.Add(new FlagToken(part.Code, part.Meaning, part.Category));
            if (part.Complete && !names.TryAdd(name, new Reading(name, path.ToList(), _order++)))
                throw new InvalidOperationException($"The flag {name} is spelled twice in one family.");
            Spell(part.Children, name, path, names);
            path.RemoveAt(path.Count - 1);
        }
    }

    // ------------------------------------------------------------------ Layout

    private static Part[] Layout() =>
    [
        Prefix("P", "Padding", "Spacing", Spacings(), Sides(Spacings)),
        Prefix("M", "Margin", "Spacing", Spacings(), Sides(Spacings)),
        Prefix("G", "Gutter", "Spacing", Spacings(),
        [
            Prefix("R", "Row", "Axis", Spacings()),
            Prefix("C", "Column", "Axis", Spacings())
        ]),
        Prefix("W", "With", "Surface",
        [
            Prefix("B", "Background", "Surface",
            [
                Word("N", "None", "Fill"),
                Word("S", "Surface", "Fill"),
                Word("A", "Accent", "Fill"),
                Word("H", "Highlight", "Fill")
            ]),
            Prefix("F", "Frame", "Surface", Strengths(withNone: true)),
            Prefix("R", "Radius", "Surface", [Word("N", "None", "Radius"), .. Spacings("Radius")]),
            Prefix("S", "Separator", "Surface", Strengths(withNone: false)),
            Prefix("G", "Grid", "Surface", Strengths(withNone: false)),
            Word("NTW", "No text wrap", "Text"),
            Word("TW", "Text wrap", "Text")
        ]),
        Word("Hover", "Lights up when hovered", "Surface"),
        Prefix("J", "Justify", "Alignment",
        [
            Word("S", "Start", "Alignment"),
            Word("E", "End", "Alignment"),
            Word("C", "Center", "Alignment"),
            Word("St", "Stretch", "Alignment"),
            Prefix("H", "Horizontal", "Direction", ItemAlignments(), [Prefix("C", "Content", "Target", ContentAlignments())]),
            Prefix("V", "Vertical", "Direction", ItemAlignments(), [Prefix("C", "Content", "Target", ContentAlignments())])
        ]),
        Prefix("S", "Self", "Alignment",
        [
            Prefix("J", "Justify", "Alignment",
            [
                Prefix("H", "Horizontal", "Direction", SelfAlignments()),
                Prefix("V", "Vertical", "Direction", SelfAlignments())
            ])
        ]),
        Prefix("V", "Visible", "Visibility",
        [
            Prefix("O", "Over", "Side", Breakpoints("_")),
            Prefix("U", "Under", "Side", Breakpoints("_"))
        ]),
        Word("Hide", "Hidden, keeps its place", "Visibility"),
        Prefix("E", "Enable", "Feature",
        [
            Word("S", "Selection", "Feature"),
            Word("O", "Outline", "Feature")
        ]),
        Prefix("D", "Disable", "Feature", [Word("S", "Selection", "Feature")]),
        Prefix("F", "Flow", "Grid",
        [
            Word("R", "Rows", "Flow", [Word("D", "Dense", "Flow")]),
            Word("C", "Columns", "Flow", [Word("D", "Dense", "Flow")])
        ]),
        Word("NoFlex", "Keeps its content size", "Layout"),
        Word("Scrollable", "Scrolls its content", "Layout"),
        Word("Reverse", "Reverse order", "Layout"),
        Word("SingleLine", "On a single line", "Layout"),
        Word("MultiLine", "Wraps onto several lines", "Layout"),
        Word("ReverseMultiLine", "Wraps, in reverse order", "Layout")
    ];

    private static Part[] Spacings() => Spacings("Spacing");

    private static Part[] Spacings(string category) =>
    [
        Word("S", "Small", category),
        Word("M", "Medium", category),
        Word("L", "Large", category),
        Word("X", "Extra large", category)
    ];

    private static Part[] Sides(Func<Part[]> next) =>
    [
        Prefix("H", "Horizontal", "Direction", next()),
        Prefix("V", "Vertical", "Direction", next()),
        Prefix("T", "Top", "Direction", next()),
        Prefix("B", "Bottom", "Direction", next()),
        Prefix("L", "Left", "Direction", next()),
        Prefix("R", "Right", "Direction", next())
    ];

    private static Part[] Strengths(bool withNone) =>
    [
        .. withNone ? [Word("N", "None", "Strength")] : Array.Empty<Part>(),
        Word("L", "Light", "Strength"),
        Word("M", "Medium", "Strength"),
        Word("S", "Strong", "Strength")
    ];

    private static Part[] ItemAlignments() =>
    [
        Word("D", "Default", "Alignment"),
        Word("S", "Start", "Alignment"),
        Word("E", "End", "Alignment"),
        Word("C", "Center", "Alignment"),
        Word("St", "Stretch", "Alignment"),
        Word("Bl", "Baseline", "Alignment"),
        Word("SB", "Space between", "Alignment"),
        Word("SA", "Space around", "Alignment"),
        Word("Ev", "Evenly", "Alignment")
    ];

    private static Part[] ContentAlignments() =>
    [
        Word("D", "Default", "Alignment"),
        Word("S", "Start", "Alignment"),
        Word("E", "End", "Alignment"),
        Word("C", "Center", "Alignment"),
        Word("St", "Stretch", "Alignment"),
        Word("SB", "Space between", "Alignment"),
        Word("SA", "Space around", "Alignment"),
        Word("SE", "Space evenly", "Alignment")
    ];

    private static Part[] SelfAlignments() =>
    [
        Word("D", "Default", "Alignment"),
        Word("N", "Normal", "Alignment"),
        Word("S", "Start", "Alignment"),
        Word("E", "End", "Alignment"),
        Word("C", "Center", "Alignment"),
        Word("St", "Stretch", "Alignment"),
        Word("Bl", "Baseline", "Alignment")
    ];

    private static Part[] Breakpoints(string prefix) =>
    [
        Word(prefix + "XS", "Extra small (512 px)", "Breakpoint"),
        Word(prefix + "S", "Small (640 px)", "Breakpoint"),
        Word(prefix + "M", "Medium (768 px)", "Breakpoint"),
        Word(prefix + "L", "Large (1024 px)", "Breakpoint"),
        Word(prefix + "XL", "Extra large (1280 px)", "Breakpoint"),
        Word(prefix + "XXL", "Extra extra large (1536 px)", "Breakpoint")
    ];

    // ------------------------------------------------------------------ Text, icons, breaks

    private static Part[] Text() =>
    [
        Word("XS", "Extra small", "Size"),
        Word("S", "Small", "Size"),
        Word("M", "Medium", "Size"),
        Word("L", "Large", "Size"),
        Word("XL", "Extra large", "Size"),
        Word("XXL", "Extra extra large", "Size"),
        Word("T", "Thin", "Weight"),
        Word("B", "Bold", "Weight"),
        Word("XB", "Extra bold", "Weight"),
        .. Colors(),
        Word("NW", "No wrap", "Overflow"),
        Word("TR", "Trimmed with an ellipsis", "Overflow")
    ];

    private static Part[] Icon() =>
    [
        Word("S", "Small", "Size"),
        Word("M", "Medium", "Size"),
        Word("L", "Large", "Size"),
        Word("X", "Extra large", "Size"),
        .. Colors()
    ];

    private static Part[] Colors() =>
    [
        Word("Subtle", "Subtle", "Color"),
        Word("Muted", "Muted", "Color"),
        Word("Highlight", "Highlighted", "Color")
    ];

    private static Part[] Breaks() =>
    [
        .. Breakpoints(""),
        Word("Under", "Breaks under the breakpoint", "Side"),
        Word("Over", "Breaks over the breakpoint", "Side")
    ];

    private static Part Word(string code, string meaning, string category) => new(code, meaning, category, true, []);

    /// <summary>
    /// A complete part that more parts can follow: <c>FR</c> is a flag, <c>FRD</c> another.
    /// </summary>
    private static Part Word(string code, string meaning, string category, params Part[][] children)
        => new(code, meaning, category, true, children.SelectMany(group => group).ToList());

    private static Part Prefix(string code, string meaning, string category, params Part[][] children)
        => new(code, meaning, category, false, children.SelectMany(group => group).ToList());
}
