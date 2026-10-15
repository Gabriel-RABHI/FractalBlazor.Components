using FractalBlazor.Components.Common.Contracts;

namespace FractalBlazor.WebSite.StateModel
{
    /// <summary>
    /// The playground of the Grid page: a number of columns, and an item placed by hand among
    /// items that place themselves: its span, its start column and its row span. A gap and the
    /// flow complete it, with the code and the CSS they make.
    /// </summary>
    public sealed class GridPlaygroundState : ObservableState
    {
        public sealed record SetColumns(int Value) : IStateAction<int>;

        public sealed record SetSpan(int Value) : IStateAction<int>;

        public sealed record SetStart(int Value) : IStateAction<int>;

        public sealed record SetRowSpan(int Value) : IStateAction<int>;

        public sealed record SetGap(FlexGutter Value) : IStateAction<FlexGutter>;

        public sealed record SetDense(bool Value) : IStateAction<bool>;

        public const int MaxColumns = 12;

        public const int MaxRowSpan = 3;

        public static IReadOnlyList<FlexGutter> Gaps { get; } = Enum.GetValues<FlexGutter>();

        public int Columns { get; private set; } = 6;

        public int Span { get; private set; } = 3;

        /// <summary>
        /// The column line the item starts on, from 1; 0 lets the grid place it.
        /// </summary>
        public int Start { get; private set; } = 1;

        public int RowSpan { get; private set; } = 2;

        public FlexGutter Gap { get; private set; } = FlexGutter.M;

        public bool Dense { get; private set; }

        /// <summary>
        /// The last line the item can start on and still fit in the grid.
        /// </summary>
        public int MaxStart => Columns - Span + 1;

        public string ContainerTag
            => $"<FbGridContainer Columns=\"{Columns}\""
               + (FlexAlignments.GutterFlag(Gap) is { } gap ? " " + gap : "")
               + (Dense ? " FRD" : "") + ">";

        public string ItemTag
            => "<FbGridItem"
               + (Span > 1 ? $" ColumnSpan=\"{Span}\"" : "")
               + (Start > 0 ? $" ColumnStart=\"{Start}\"" : "")
               + (RowSpan > 1 ? $" RowSpan=\"{RowSpan}\"" : "") + ">";

        public string Code => $"{ContainerTag}\n    <FbGridItem>1</FbGridItem>\n    <FbGridItem>2</FbGridItem>\n    {ItemTag}A</FbGridItem>\n    …\n</FbGridContainer>";

        /// <summary>
        /// The declarations of the grid, then those of the item A.
        /// </summary>
        public string Css
            => string.Join("\n", new[]
            {
                $"grid-template-columns: repeat({Columns}, minmax(0, 1fr));",
                FlexAlignments.GutterDeclaration(Gap) is { } gap ? gap + ";" : null,
                Dense ? "grid-auto-flow: row dense;" : null,
                $"/* A */ grid-column: {(Start > 0 ? Start.ToString() : "auto")} / span {Span};",
                $"/* A */ grid-row: auto / span {RowSpan};"
            }.Where(line => line is not null));

        /// <summary>
        /// Fewer columns shorten the item and bring it back into the grid.
        /// </summary>
        public void Handle(SetColumns action)
        {
            var columns = Math.Clamp(action.Value, 1, MaxColumns);
            if (Columns == columns)
                return;

            Columns = columns;
            Span = Math.Min(Span, Columns);
            if (Start > MaxStart)
                Start = MaxStart;
            NotifyStateChanged();
        }

        /// <summary>
        /// A wider item moves back to keep fitting in the grid.
        /// </summary>
        public void Handle(SetSpan action)
        {
            var span = Math.Clamp(action.Value, 1, Columns);
            if (Span == span)
                return;

            Span = span;
            if (Start > MaxStart)
                Start = MaxStart;
            NotifyStateChanged();
        }

        public void Handle(SetStart action)
        {
            var start = Math.Clamp(action.Value, 0, MaxStart);
            if (Start == start)
                return;

            Start = start;
            NotifyStateChanged();
        }

        public void Handle(SetRowSpan action)
        {
            var rowSpan = Math.Clamp(action.Value, 1, MaxRowSpan);
            if (RowSpan == rowSpan)
                return;

            RowSpan = rowSpan;
            NotifyStateChanged();
        }

        public void Handle(SetGap action)
        {
            if (Gap == action.Value)
                return;

            Gap = action.Value;
            NotifyStateChanged();
        }

        public void Handle(SetDense action)
        {
            if (Dense == action.Value)
                return;

            Dense = action.Value;
            NotifyStateChanged();
        }
    }

    /// <summary>
    /// A step of the responsive grid demo: from <see cref="MinWidth"/>, the grid has
    /// <see cref="Columns"/> columns.
    /// </summary>
    public sealed record GridStep(string Name, int MinWidth, int Columns);

    /// <summary>
    /// The width given to a responsive grid. The grid reads its own width, not the window's:
    /// the steps it reaches are those of this width.
    /// </summary>
    public sealed class GridResponsiveState : ObservableState
    {
        public sealed record SetWidth(int Value) : IStateAction<int>;

        public const int MinWidth = 280;

        public const int MaxWidth = 1100;

        /// <summary>
        /// The steps of the demo grid, as its parameters set them: <c>Columns="1"</c>,
        /// <c>ColumnsXS="2"</c>, <c>ColumnsS="3"</c>, <c>ColumnsM="4"</c>, <c>ColumnsL="6"</c>.
        /// </summary>
        public static IReadOnlyList<GridStep> Steps { get; } =
        [
            new("Base", 0, 1),
            new("XS", 512, 2),
            new("S", 640, 3),
            new("M", 768, 4),
            new("L", 1024, 6)
        ];

        public int Width { get; private set; } = 700;

        /// <summary>
        /// The widest step the grid reaches.
        /// </summary>
        public GridStep Step => Steps.Last(step => Width >= step.MinWidth);

        public void Handle(SetWidth action)
        {
            var width = Math.Clamp(action.Value, MinWidth, MaxWidth);
            if (Width == width)
                return;

            Width = width;
            NotifyStateChanged();
        }
    }

    /// <summary>
    /// The alignment of the items of a grid in their cells: one flag per axis of the screen,
    /// as in a row or a stack.
    /// </summary>
    public sealed class GridAlignmentState : ObservableState
    {
        public sealed record SetHorizontal(FlexAlignment Value) : IStateAction<FlexAlignment>;

        public sealed record SetVertical(FlexAlignment Value) : IStateAction<FlexAlignment>;

        public static IReadOnlyList<FlexAlignment> Choices { get; } =
            [FlexAlignment.Start, FlexAlignment.Center, FlexAlignment.End, FlexAlignment.Stretch];

        public FlexAlignment Horizontal { get; private set; } = FlexAlignment.Center;

        public FlexAlignment Vertical { get; private set; } = FlexAlignment.End;

        public FlexAlignment On(FlexAxis axis) => axis == FlexAxis.Horizontal ? Horizontal : Vertical;

        /// <summary>
        /// Both flags, written in full: a grid has no shortcut.
        /// </summary>
        public string Flags => FlexAlignments.Flag(FlexAxis.Horizontal, Horizontal) + " " + FlexAlignments.Flag(FlexAxis.Vertical, Vertical);

        public string Code => $"<FbGridContainer Columns=\"3\" AutoRows=\"5rem\" GS {Flags}>";

        /// <summary>
        /// The CSS of an axis: <c>justify-items</c> across the columns, <c>align-items</c> along
        /// them. A grid aligns to <c>start</c> and <c>end</c>, not to <c>flex-start</c>.
        /// </summary>
        public string Declaration(FlexAxis axis)
            => (axis == FlexAxis.Horizontal ? "justify-items" : "align-items") + ": " + On(axis) switch
            {
                FlexAlignment.Start => "start",
                FlexAlignment.Center => "center",
                FlexAlignment.End => "end",
                _ => "stretch"
            };

        public void Handle(SetHorizontal action)
        {
            if (Horizontal == action.Value || !Choices.Contains(action.Value))
                return;

            Horizontal = action.Value;
            NotifyStateChanged();
        }

        public void Handle(SetVertical action)
        {
            if (Vertical == action.Value || !Choices.Contains(action.Value))
                return;

            Vertical = action.Value;
            NotifyStateChanged();
        }
    }
}
