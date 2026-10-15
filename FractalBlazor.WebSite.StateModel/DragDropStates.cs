using FractalBlazor.Components.Common.Contracts;

namespace FractalBlazor.WebSite.StateModel
{
    /// <summary>
    /// What the rules of an area answered for a zone: the dragged item, the receiver, the
    /// zone and the preview.
    /// </summary>
    public sealed record DropPreview(object DragId, object DropId, FbDropZone Zone, FbDropFeedback Feedback);

    /// <summary>
    /// A drag and drop demo that keeps the last drop it received, and the last preview its
    /// rules gave. Only its own views follow: the area of the demo does not render again.
    /// </summary>
    public abstract class DropLogState : ObservableState
    {
        public FbDrop? LastDrop { get; private set; }

        public int DropCount { get; private set; }

        public DropPreview? LastPreview { get; private set; }

        public void Handle(FbDrop drop)
        {
            LastDrop = drop;
            DropCount++;
            Apply(drop);
            NotifyStateChanged();
        }

        /// <summary>
        /// What the drop changes in the demo, before its views follow.
        /// </summary>
        protected virtual void Apply(FbDrop drop)
        {
        }

        protected FbDropFeedback Record(object dragId, object dropId, FbDropZone zone, FbDropFeedback feedback)
        {
            LastPreview = new DropPreview(dragId, dropId, zone, feedback);
            NotifyStateChanged();
            return feedback;
        }
    }

    /// <summary>
    /// A demo of the basics: an area without rules, whose items hold a button and a field, to
    /// show that their content stays alive.
    /// </summary>
    public sealed class DragDropDemoState : DropLogState
    {
        public sealed record Click : IStateAction;

        public sealed record SetNote(string Value) : IStateAction<string>;

        public int Clicks { get; private set; }

        public string Note { get; private set; } = "";

        public void Handle(Click action)
        {
            Clicks++;
            NotifyStateChanged();
        }

        public void Handle(SetNote action)
        {
            if (Note == action.Value)
                return;

            Note = action.Value;
            NotifyStateChanged();
        }
    }

    /// <summary>
    /// Compatibility: of two items, only the first suits the receiver. The receiver shows it
    /// as soon as the drag starts.
    /// </summary>
    public sealed class CompatibilityDemoState : DropLogState, IFbDragAndDropRules
    {
        public const string Compatible = "Élément compatible";

        public const string Incompatible = "Élément incompatible";

        public const string Receiver = "Receveur";

        public bool Accepts(object dragId, object dropId) => Equals(dragId, Compatible);

        public FbDropFeedback Preview(object dragId, object dropId, FbDropZone zone) => FbDrop.NaturalFeedback(zone);
    }

    /// <summary>
    /// Preview: each item asks for its own place on the receiver, whatever the zone.
    /// </summary>
    public sealed class PreviewDemoState : DropLogState, IFbDragAndDropRules
    {
        public const string Receiver = "Receveur";

        /// <summary>
        /// The items, each with the place the rules give it.
        /// </summary>
        public static IReadOnlyList<(string Name, FbDropFeedback Feedback)> Items { get; } =
        [
            ("Au centre", FbDropFeedback.Center),
            ("Interdit", FbDropFeedback.Forbidden),
            ("À droite", FbDropFeedback.Right),
            ("En haut", FbDropFeedback.Top)
        ];

        public bool Accepts(object dragId, object dropId) => true;

        public FbDropFeedback Preview(object dragId, object dropId, FbDropZone zone)
            => Record(dragId, dropId, zone, Items.FirstOrDefault(item => Equals(item.Name, dragId)).Feedback);
    }

    /// <summary>
    /// Splits: one item, and a receiver for each split. Each zone offers its natural place.
    /// </summary>
    public sealed class SplitDemoState : DropLogState, IFbDragAndDropRules
    {
        public const string Item = "Élément";

        public bool Accepts(object dragId, object dropId) => true;

        public FbDropFeedback Preview(object dragId, object dropId, FbDropZone zone)
            => Record(dragId, dropId, zone, FbDrop.NaturalFeedback(zone));
    }

    /// <summary>
    /// The tone of an item of the reordered list: the views turn it into a variant.
    /// </summary>
    public enum ItemTone
    {
        Blue,
        Green,
        Orange,
        Purple
    }

    public sealed record ReorderItem(string Name, ItemTone Tone);

    /// <summary>
    /// A list reordered by drag and drop: an item dropped on the first half of another goes
    /// before it, on the second half after it. Shown twice, as a column and as a row.
    /// </summary>
    public sealed class ReorderListState : ObservableState, IFbDragAndDropRules
    {
        private readonly List<ReorderItem> _items =
        [
            new("Élément 1", ItemTone.Blue),
            new("Élément 2", ItemTone.Green),
            new("Élément 3", ItemTone.Orange),
            new("Élément 4", ItemTone.Purple)
        ];

        public IReadOnlyList<ReorderItem> Items => _items;

        public bool Accepts(object dragId, object dropId) => dragId is ReorderItem && dropId is ReorderItem;

        /// <summary>
        /// An item over itself goes nowhere.
        /// </summary>
        public FbDropFeedback Preview(object dragId, object dropId, FbDropZone zone)
            => Equals(dragId, dropId) ? FbDropFeedback.None : FbDrop.NaturalFeedback(zone);

        public void Handle(FbDrop drop)
        {
            if (drop.DragId is not ReorderItem dragged || drop.DropId is not ReorderItem target || dragged == target)
                return;

            _items.Remove(dragged);
            var index = _items.IndexOf(target);
            var after = drop.Feedback is FbDropFeedback.Bottom or FbDropFeedback.Right;
            _items.Insert(after ? index + 1 : index, dragged);
            NotifyStateChanged();
        }
    }
}
