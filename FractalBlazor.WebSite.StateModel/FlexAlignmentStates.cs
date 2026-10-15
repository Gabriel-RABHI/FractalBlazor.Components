using System.ComponentModel.DataAnnotations;
using FractalBlazor.Components.Common.Contracts;

namespace FractalBlazor.WebSite.StateModel
{
    /// <summary>
    /// The two flex containers of the library.
    /// </summary>
    public enum FlexContainer
    {
        [Display(Name = "FbRow")]
        Row,

        [Display(Name = "FbStack")]
        Stack
    }

    /// <summary>
    /// An axis of the screen: the H or the V of an alignment flag.
    /// </summary>
    public enum FlexAxis
    {
        Horizontal,
        Vertical
    }

    /// <summary>
    /// An alignment on one axis: the end of a flag, <c>JHC</c> being Center on the horizontal
    /// axis.
    /// </summary>
    public enum FlexAlignment
    {
        [Display(Name = "Début")]
        Start,

        [Display(Name = "Centre")]
        Center,

        [Display(Name = "Fin")]
        End,

        [Display(Name = "Étirer")]
        Stretch,

        [Display(Name = "Ligne de base")]
        Baseline,

        [Display(Name = "Espace entre")]
        SpaceBetween,

        [Display(Name = "Espace autour")]
        SpaceAround,

        [Display(Name = "Espace égal")]
        SpaceEvenly
    }

    /// <summary>
    /// The gutter between the children: the G flags.
    /// </summary>
    public enum FlexGutter
    {
        [Display(Name = "Aucun")]
        None,

        S,

        M,

        L,

        X
    }

    /// <summary>
    /// The alignment flags of <c>FbRow</c> and <c>FbStack</c>, and the CSS they write. A flag
    /// names an axis of the screen, H or V, whatever the container; the container only decides
    /// which property each axis is: <c>justify-content</c> along the flow of its children,
    /// <c>align-items</c> across it.
    /// </summary>
    public static class FlexAlignments
    {
        private static readonly FlexAlignment[] FlowChoices =
            [FlexAlignment.Start, FlexAlignment.Center, FlexAlignment.End, FlexAlignment.SpaceBetween, FlexAlignment.SpaceAround, FlexAlignment.SpaceEvenly];

        private static readonly FlexAlignment[] RowCrossChoices =
            [FlexAlignment.Start, FlexAlignment.Center, FlexAlignment.End, FlexAlignment.Stretch, FlexAlignment.Baseline];

        private static readonly FlexAlignment[] StackCrossChoices =
            [FlexAlignment.Start, FlexAlignment.Center, FlexAlignment.End, FlexAlignment.Stretch];

        /// <summary>
        /// The axis the children follow: horizontal in a row, vertical in a stack.
        /// </summary>
        public static FlexAxis FlowAxis(FlexContainer container)
            => container == FlexContainer.Row ? FlexAxis.Horizontal : FlexAxis.Vertical;

        public static bool IsFlow(FlexContainer container, FlexAxis axis) => FlowAxis(container) == axis;

        public static string Name(FlexContainer container) => container == FlexContainer.Row ? "FbRow" : "FbStack";

        /// <summary>
        /// The alignments an axis offers. The flow axis shares the free space out between the
        /// children; the cross axis stretches them, and a row also aligns their baselines.
        /// Stretch is left out of the flow axis, where CSS packs the children at the start.
        /// </summary>
        public static IReadOnlyList<FlexAlignment> Choices(FlexContainer container, FlexAxis axis)
            => IsFlow(container, axis) ? FlowChoices
                : container == FlexContainer.Row ? RowCrossChoices
                : StackCrossChoices;

        public static string Suffix(FlexAlignment alignment) => alignment switch
        {
            FlexAlignment.Start => "S",
            FlexAlignment.Center => "C",
            FlexAlignment.End => "E",
            FlexAlignment.Stretch => "St",
            FlexAlignment.Baseline => "Bl",
            FlexAlignment.SpaceBetween => "SB",
            FlexAlignment.SpaceAround => "SA",
            _ => "Ev"
        };

        public static string Flag(FlexAxis axis, FlexAlignment alignment)
            => (axis == FlexAxis.Horizontal ? "JH" : "JV") + Suffix(alignment);

        /// <summary>
        /// The CSS property an axis is in a container.
        /// </summary>
        public static string Property(FlexContainer container, FlexAxis axis)
            => IsFlow(container, axis) ? "justify-content" : "align-items";

        public static string Value(FlexAlignment alignment) => alignment switch
        {
            FlexAlignment.Start => "flex-start",
            FlexAlignment.Center => "center",
            FlexAlignment.End => "flex-end",
            FlexAlignment.Stretch => "stretch",
            FlexAlignment.Baseline => "baseline",
            FlexAlignment.SpaceBetween => "space-between",
            FlexAlignment.SpaceAround => "space-around",
            _ => "space-evenly"
        };

        public static string Declaration(FlexContainer container, FlexAxis axis, FlexAlignment alignment)
            => $"{Property(container, axis)}: {Value(alignment)}";

        /// <summary>
        /// The flags of both axes: the shortcut (<c>JC</c>) when they are equal and one exists.
        /// </summary>
        public static string Flags(FlexAlignment horizontal, FlexAlignment vertical)
            => horizontal == vertical && horizontal is FlexAlignment.Start or FlexAlignment.Center or FlexAlignment.End or FlexAlignment.Stretch
                ? "J" + Suffix(horizontal)
                : Flag(FlexAxis.Horizontal, horizontal) + " " + Flag(FlexAxis.Vertical, vertical);

        public static string? GutterFlag(FlexGutter gutter) => gutter == FlexGutter.None ? null : "G" + gutter;

        public static string? GutterDeclaration(FlexGutter gutter)
            => gutter == FlexGutter.None ? null : $"gap: var(--fb-space-{gutter.ToString().ToLowerInvariant()})";
    }

    /// <summary>
    /// The pad of the Row / Stack page: one position of the group, given with the same flags to
    /// a row and to a stack.
    /// </summary>
    public sealed class AlignmentPadState : ObservableState
    {
        public sealed record SetPosition(FlexAlignment Horizontal, FlexAlignment Vertical) : IStateAction;

        /// <summary>
        /// The positions of each axis, offered by both containers.
        /// </summary>
        public static IReadOnlyList<FlexAlignment> Positions { get; } = [FlexAlignment.Start, FlexAlignment.Center, FlexAlignment.End];

        public FlexAlignment Horizontal { get; private set; } = FlexAlignment.Center;

        public FlexAlignment Vertical { get; private set; } = FlexAlignment.End;

        public bool IsAt(FlexAlignment horizontal, FlexAlignment vertical) => Horizontal == horizontal && Vertical == vertical;

        public FlexAlignment On(FlexAxis axis) => axis == FlexAxis.Horizontal ? Horizontal : Vertical;

        /// <summary>
        /// The flags, written in full: the same for both containers.
        /// </summary>
        public string Flags => FlexAlignments.Flag(FlexAxis.Horizontal, Horizontal) + " " + FlexAlignments.Flag(FlexAxis.Vertical, Vertical);

        public string Code(FlexContainer container) => $"<{FlexAlignments.Name(container)} {Flags}>";

        public string Declaration(FlexContainer container, FlexAxis axis) => FlexAlignments.Declaration(container, axis, On(axis));

        /// <summary>
        /// A position outside the pad is ignored.
        /// </summary>
        public void Handle(SetPosition action)
        {
            if (!Positions.Contains(action.Horizontal) || !Positions.Contains(action.Vertical) || IsAt(action.Horizontal, action.Vertical))
                return;

            (Horizontal, Vertical) = (action.Horizontal, action.Vertical);
            NotifyStateChanged();
        }
    }

    /// <summary>
    /// The playground of the Row / Stack page: a container, an alignment on each axis and a
    /// gutter, with the code they make.
    /// </summary>
    public sealed class FlexPlaygroundState : ObservableState
    {
        public sealed record SetContainer(FlexContainer Value) : IStateAction<FlexContainer>;

        public sealed record SetHorizontal(FlexAlignment Value) : IStateAction<FlexAlignment>;

        public sealed record SetVertical(FlexAlignment Value) : IStateAction<FlexAlignment>;

        public sealed record SetGutter(FlexGutter Value) : IStateAction<FlexGutter>;

        public static IReadOnlyList<FlexContainer> Containers { get; } = Enum.GetValues<FlexContainer>();

        public static IReadOnlyList<FlexGutter> Gutters { get; } = Enum.GetValues<FlexGutter>();

        public FlexContainer Container { get; private set; } = FlexContainer.Row;

        public FlexAlignment Horizontal { get; private set; } = FlexAlignment.Start;

        public FlexAlignment Vertical { get; private set; } = FlexAlignment.Center;

        public FlexGutter Gutter { get; private set; } = FlexGutter.M;

        public FlexAlignment On(FlexAxis axis) => axis == FlexAxis.Horizontal ? Horizontal : Vertical;

        public IReadOnlyList<FlexAlignment> Choices(FlexAxis axis) => FlexAlignments.Choices(Container, axis);

        public bool IsFlow(FlexAxis axis) => FlexAlignments.IsFlow(Container, axis);

        public string Declaration(FlexAxis axis) => FlexAlignments.Declaration(Container, axis, On(axis));

        /// <summary>
        /// The opening tag, as it would be written.
        /// </summary>
        public string Tag
            => $"<{FlexAlignments.Name(Container)} {FlexAlignments.Flags(Horizontal, Vertical)}"
               + (FlexAlignments.GutterFlag(Gutter) is { } gutter ? " " + gutter : "") + ">";

        public string Code => $"{Tag}\n    …\n</{FlexAlignments.Name(Container)}>";

        /// <summary>
        /// The declarations the flags write, horizontal axis first.
        /// </summary>
        public string Css
            => string.Join("\n", new[] { Declaration(FlexAxis.Horizontal), Declaration(FlexAxis.Vertical), FlexAlignments.GutterDeclaration(Gutter) }
                .Where(declaration => declaration is not null)
                .Select(declaration => declaration + ";"));

        /// <summary>
        /// The other container keeps the alignments it offers too; the others come back to the
        /// start.
        /// </summary>
        public void Handle(SetContainer action)
        {
            if (Container == action.Value)
                return;

            Container = action.Value;
            if (!Choices(FlexAxis.Horizontal).Contains(Horizontal))
                Horizontal = FlexAlignment.Start;
            if (!Choices(FlexAxis.Vertical).Contains(Vertical))
                Vertical = FlexAlignment.Start;
            NotifyStateChanged();
        }

        /// <summary>
        /// An alignment the axis does not offer is ignored.
        /// </summary>
        public void Handle(SetHorizontal action)
        {
            if (Horizontal == action.Value || !Choices(FlexAxis.Horizontal).Contains(action.Value))
                return;

            Horizontal = action.Value;
            NotifyStateChanged();
        }

        public void Handle(SetVertical action)
        {
            if (Vertical == action.Value || !Choices(FlexAxis.Vertical).Contains(action.Value))
                return;

            Vertical = action.Value;
            NotifyStateChanged();
        }

        public void Handle(SetGutter action)
        {
            if (Gutter == action.Value)
                return;

            Gutter = action.Value;
            NotifyStateChanged();
        }
    }
}
