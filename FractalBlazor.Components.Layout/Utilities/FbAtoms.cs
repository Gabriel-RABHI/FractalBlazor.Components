using System.Text;

namespace FractalBlazor.Components.Layout.Utilities
{
    /// <summary>
    /// What a layout component can set: one CSS property, or a group of properties that move
    /// together, per axis. A component sets at most one atom per axis. The order of the axes is
    /// the order of the stylesheet, so a shorthand comes before the properties it covers and
    /// is overridden by them, as it was when they were written inline.
    /// </summary>
    internal enum FbAxis : byte
    {
        Position,
        Display,
        Flex,
        UserSelect,
        Visibility,
        Padding,
        PaddingBlock,
        PaddingInline,
        PaddingTop,
        PaddingBottom,
        PaddingLeft,
        PaddingRight,
        Margin,
        MarginBlock,
        MarginInline,
        MarginTop,
        MarginBottom,
        MarginLeft,
        MarginRight,
        Radius,
        FlexDirection,
        FlexWrap,
        FlexFlow,
        JustifyContent,
        AlignItems,
        AlignContent,
        PlaceItems,
        AlignSelf,
        Gap,
        MinWidth,
        MinHeight,
        MaxHeight
    }

    /// <summary>
    /// Technical classes of Layout: each one holds a single declaration of the table below and
    /// is named after its position, <c>fb-a</c> to <c>fb-z</c>, then <c>fb-aa</c>… The
    /// stylesheet section that defines them is generated from this table
    /// (<see cref="FbAtomStylesheet"/>), so a component never writes these declarations inline.
    /// </summary>
    internal static class FbAtoms
    {
        public const int Relative = 0;
        public const int UserSelectNone = 0;
        public const int UserSelectText = 1;
        public const int Hidden = 0;
        public const int Column = 0;

        /// <summary>
        /// Flex factors from 0 to <see cref="MaxFlex"/> have an atom; others stay inline.
        /// </summary>
        public const int MaxFlex = 12;

        private static readonly List<string> _declarations = [];
        /// <summary>
        /// Number of axes.
        /// </summary>
        public static readonly int AxisCount = Enum.GetValues<FbAxis>().Length;

        private static readonly short[][] _atoms = new short[AxisCount][];
        private static readonly short[] _resets = new short[_atoms.Length];
        private static readonly Dictionary<string, int> _byDeclaration;

        static FbAtoms()
        {
            Array.Fill(_resets, (short)-1);

            Add(FbAxis.Position, ["position:relative"]);
            Add<FbBaseDisplayMode>(FbAxis.Display, mode => mode switch
            {
                FbBaseDisplayMode.Block => "display:block",
                FbBaseDisplayMode.InlineBloc => "display:inline-block",
                FbBaseDisplayMode.Flex => "display:flex",
                FbBaseDisplayMode.InlineFlex => "display:inline-flex",
                FbBaseDisplayMode.Table => "display:table",
                FbBaseDisplayMode.TableRow => "display:table-row",
                FbBaseDisplayMode.TableCell => "display:table-cell",
                FbBaseDisplayMode.Grid => "display:grid",
                _ => null
            });
            Add(FbAxis.Flex, [.. Enumerable.Range(0, MaxFlex + 1).Select(factor => $"flex:{factor}")]);
            _resets[(int)FbAxis.Flex] = NewAtom("flex:initial");
            Add(FbAxis.UserSelect, ["-webkit-user-select:none;user-select:none", "-webkit-user-select:text;user-select:text"]);
            Add(FbAxis.Visibility, ["visibility:hidden"]);

            AddBox(FbAxis.Padding, "padding", FbLayoutHelper.ToSpacingCss);
            AddBox(FbAxis.Margin, "margin", FbLayoutHelper.ToSpacingCss);
            Add<FbSpacing>(FbAxis.Radius, size => size == FbSpacing.None ? null : $"border-radius:{FbLayoutHelper.ToRadiusCss(size)}");

            Add<FbFlexDirection>(FbAxis.FlexDirection, direction => "flex-direction:" + direction switch
            {
                FbFlexDirection.Row => "row",
                FbFlexDirection.RowReverse => "row-reverse",
                FbFlexDirection.Column => "column",
                _ => "column-reverse"
            });
            Add<FbFlexWrap>(FbAxis.FlexWrap, wrap => "flex-wrap:" + wrap switch
            {
                FbFlexWrap.NoWrap => "nowrap",
                FbFlexWrap.Wrap => "wrap",
                _ => "wrap-reverse"
            });
            Add(FbAxis.FlexFlow, ["flex-flow:column"]);
            Add<FbFlexJustify>(FbAxis.JustifyContent, justify => "justify-content:" + justify switch
            {
                FbFlexJustify.Start => "flex-start",
                FbFlexJustify.End => "flex-end",
                FbFlexJustify.Center => "center",
                FbFlexJustify.Stretch => "stretch",
                FbFlexJustify.SpaceBetween => "space-between",
                FbFlexJustify.SpaceAround => "space-around",
                _ => "space-evenly"
            });
            Add<FbFlexAlignItems>(FbAxis.AlignItems, align => "align-items:" + align switch
            {
                FbFlexAlignItems.Start => "flex-start",
                FbFlexAlignItems.End => "flex-end",
                FbFlexAlignItems.Center => "center",
                FbFlexAlignItems.Baseline => "baseline",
                _ => "stretch"
            });
            Add<FbFlexAlignContent>(FbAxis.AlignContent, align => "align-content:" + align switch
            {
                FbFlexAlignContent.Start => "flex-start",
                FbFlexAlignContent.End => "flex-end",
                FbFlexAlignContent.Center => "center",
                FbFlexAlignContent.SpaceBetween => "space-between",
                FbFlexAlignContent.SpaceAround => "space-around",
                _ => "stretch"
            });
            Add<FbFlexPlaceItems>(FbAxis.PlaceItems, place => place switch
            {
                FbFlexPlaceItems.None => null,
                FbFlexPlaceItems.Center => "place-items:center",
                FbFlexPlaceItems.Normal => "place-items:normal",
                FbFlexPlaceItems.Start => "place-items:start",
                FbFlexPlaceItems.End => "place-items:end",
                FbFlexPlaceItems.SelfStart => "place-items:self-start",
                FbFlexPlaceItems.SelfEnd => "place-items:self-end",
                FbFlexPlaceItems.FlexStart => "place-items:flex-start",
                FbFlexPlaceItems.FlexEnd => "place-items:flex-end",
                FbFlexPlaceItems.Baseline => "place-items:baseline",
                _ => "place-items:stretch"
            });
            Add<FbFlexItemSelfAlign>(FbAxis.AlignSelf, align => align switch
            {
                FbFlexItemSelfAlign.None => null,
                FbFlexItemSelfAlign.Auto => "align-self:auto",
                FbFlexItemSelfAlign.Start => "align-self:flex-start",
                FbFlexItemSelfAlign.End => "align-self:flex-end",
                FbFlexItemSelfAlign.Center => "align-self:center",
                FbFlexItemSelfAlign.Baseline => "align-self:baseline",
                _ => "align-self:stretch"
            });
            Add<FbSpacing>(FbAxis.Gap, size => size == FbSpacing.None ? null : $"gap:{FbLayoutHelper.ToSpacingCss(size)}");
            AddSize(FbAxis.MinWidth, "min-width");
            AddSize(FbAxis.MinHeight, "min-height");
            AddSize(FbAxis.MaxHeight, "max-height");

            Names = [.. Enumerable.Range(0, _declarations.Count).Select(NameAt)];
            _byDeclaration = _declarations.Select((declaration, atom) => (declaration, atom)).ToDictionary(entry => entry.declaration, entry => entry.atom);
        }

        /// <summary>
        /// Class name of each atom.
        /// </summary>
        public static IReadOnlyList<string> Names { get; }

        /// <summary>
        /// Declaration held by each atom.
        /// </summary>
        public static IReadOnlyList<string> Declarations => _declarations;

        /// <summary>
        /// Atom of <paramref name="axis"/> for <paramref name="value"/>, or <c>-1</c> when that
        /// value declares nothing (<c>None</c>).
        /// </summary>
        public static short Of(FbAxis axis, int value)
        {
            var atoms = _atoms[(int)axis];
            return (uint)value < (uint)atoms.Length ? atoms[value] : (short)-1;
        }

        /// <summary>
        /// Atom that brings <paramref name="axis"/> back to its initial value, when a component
        /// leaves unset an axis that its base class declares; <c>-1</c> when there is none.
        /// </summary>
        public static short Reset(FbAxis axis) => _resets[(int)axis];

        public static bool HasFlex(int factor) => factor is >= 0 and <= MaxFlex;

        /// <summary>
        /// Class name of the atom holding <paramref name="declaration"/>, written as in the
        /// table (<c>padding:var(--fb-space-l)</c>).
        /// </summary>
        public static string NameOf(string declaration)
            => _byDeclaration.TryGetValue(declaration, out var atom)
                ? Names[atom]
                : throw new ArgumentException($"No atom declares '{declaration}'.", nameof(declaration));

        private static void Add(FbAxis axis, string[] declarations)
            => _atoms[(int)axis] = [.. declarations.Select(NewAtom)];

        private static void Add<TEnum>(FbAxis axis, Func<TEnum, string?> declaration) where TEnum : struct, Enum
        {
            var values = Enum.GetValues<TEnum>();
            var atoms = new short[values.Max(value => Convert.ToInt32(value)) + 1];
            Array.Fill(atoms, (short)-1);
            foreach (var value in values)
            {
                if (declaration(value) is { } text)
                    atoms[Convert.ToInt32(value)] = NewAtom(text);
            }
            _atoms[(int)axis] = atoms;
        }

        /// <summary>
        /// The seven axes of a box property, from <paramref name="all"/>: the shorthand, the
        /// block and inline pairs, then each side.
        /// </summary>
        private static void AddBox(FbAxis all, string property, Func<FbSpacing, string> css)
        {
            string[] suffixes = ["", "-block", "-inline", "-top", "-bottom", "-left", "-right"];
            for (var side = 0; side < suffixes.Length; side++)
                Add<FbSpacing>((FbAxis)((int)all + side), size => size == FbSpacing.None ? null : $"{property}{suffixes[side]}:{css(size)}");
        }

        private static void AddSize(FbAxis axis, string property)
            => Add<FbFlexSize>(axis, size => size switch
            {
                FbFlexSize.MaxContent => $"{property}:max-content",
                FbFlexSize.MinContent => $"{property}:min-content",
                _ => null
            });

        private static short NewAtom(string declaration)
        {
            _declarations.Add(declaration);
            return (short)(_declarations.Count - 1);
        }

        /// <summary>
        /// <c>fb-a</c> for 0, <c>fb-z</c> for 25, <c>fb-aa</c> for 26…
        /// </summary>
        private static string NameAt(int index)
        {
            var letters = new StringBuilder();
            for (var rest = index + 1; rest > 0; rest = (rest - 1) / 26)
                letters.Insert(0, (char)('a' + (rest - 1) % 26));
            return "fb-" + letters;
        }
    }

    /// <summary>
    /// The atoms of one component, one per axis, gathered level by level when its classes are
    /// not in the cache yet.
    /// </summary>
    internal sealed class FbAtomLine
    {
        private readonly short[] _atoms = new short[FbAtoms.AxisCount];

        public FbAtomLine() => Array.Fill(_atoms, (short)-1);

        public void Set(FbAxis axis, int value) => _atoms[(int)axis] = FbAtoms.Of(axis, value);

        /// <summary>
        /// Sets a box property from its shorthand and its four sides, with the fewest atoms:
        /// equal opposite sides become one block or inline atom.
        /// </summary>
        public void SetBox(FbAxis all, FbSpacing size, FbSpacing top, FbSpacing bottom, FbSpacing left, FbSpacing right)
        {
            Set(all, (int)size);
            if (top == bottom)
            {
                Set(all + 1, (int)top);
            }
            else
            {
                Set(all + 3, (int)top);
                Set(all + 4, (int)bottom);
            }
            if (left == right)
            {
                Set(all + 2, (int)left);
            }
            else
            {
                Set(all + 5, (int)left);
                Set(all + 6, (int)right);
            }
        }

        /// <summary>
        /// The <c>class</c> attribute: <paramref name="classes"/>, then the atoms that differ
        /// from <paramref name="baseline"/>, the line of the base class that already declares
        /// them in the stylesheet.
        /// </summary>
        public string Classes(string? classes, FbAtomLine? baseline)
        {
            var builder = new StringBuilder(classes, 128);
            for (var axis = 0; axis < _atoms.Length; axis++)
            {
                var atom = _atoms[axis];
                var inherited = baseline?._atoms[axis] ?? -1;
                if (atom == inherited)
                    continue;
                if (atom < 0)
                    atom = FbAtoms.Reset((FbAxis)axis);
                if (atom < 0)
                    continue;
                if (builder.Length > 0)
                    builder.Append(' ');
                builder.Append(FbAtoms.Names[atom]);
            }
            return builder.ToString();
        }

        /// <summary>
        /// The declarations of the line, for the rule of a base class.
        /// </summary>
        public string Declarations()
            => string.Join(";", _atoms.Where(atom => atom >= 0).Select(atom => FbAtoms.Declarations[atom]));
    }
}
