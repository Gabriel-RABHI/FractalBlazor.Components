namespace FractalBlazor.Components.Common.Contracts
{
    /// <summary>
    /// A zone of a drop receiver: where, on it, the dragged item is.
    /// </summary>
    public enum FbDropZone : byte
    {
        Top,
        Center,
        Bottom,
        Left,
        Right
    }

    /// <summary>
    /// What a drop on a zone would do: refused (<see cref="None"/>, <see cref="Forbidden"/>),
    /// or the place the dragged item would take.
    /// </summary>
    public enum FbDropFeedback : byte
    {
        /// <summary>
        /// Refused, without a sign.
        /// </summary>
        None,

        /// <summary>
        /// Refused, and shown as forbidden.
        /// </summary>
        Forbidden,

        /// <summary>
        /// Inserted above the receiver.
        /// </summary>
        Top,

        /// <summary>
        /// Put inside the receiver.
        /// </summary>
        Center,

        /// <summary>
        /// Inserted below the receiver.
        /// </summary>
        Bottom,

        /// <summary>
        /// Inserted on the left of the receiver.
        /// </summary>
        Left,

        /// <summary>
        /// Inserted on the right of the receiver.
        /// </summary>
        Right
    }

    /// <summary>
    /// The rules of a drag and drop area, implemented by the state that handles its drops
    /// (<see cref="FbDrop"/>). Without them, every receiver accepts every item, and each zone
    /// offers its natural place.
    /// </summary>
    public interface IFbDragAndDropRules
    {
        /// <summary>
        /// Whether a receiver can take the dragged item at all. Asked for every receiver when
        /// a drag starts: the receivers that accept show it, the others stay out of the drag.
        /// </summary>
        bool Accepts(object dragId, object dropId);

        /// <summary>
        /// What a drop on a zone would do. Asked when the dragged item enters the zone.
        /// </summary>
        FbDropFeedback Preview(object dragId, object dropId, FbDropZone zone);
    }

    /// <summary>
    /// Sent to the handler of a drag and drop area when an item is released on a zone that
    /// accepts it: <paramref name="Feedback"/> is the place the preview gave.
    /// </summary>
    public sealed record FbDrop(object DragId, object DropId, FbDropZone Zone, FbDropFeedback Feedback) : IStateAction
    {
        /// <summary>
        /// The natural place of a zone: the preview of an area without rules.
        /// </summary>
        public static FbDropFeedback NaturalFeedback(FbDropZone zone) => zone switch
        {
            FbDropZone.Top => FbDropFeedback.Top,
            FbDropZone.Bottom => FbDropFeedback.Bottom,
            FbDropZone.Left => FbDropFeedback.Left,
            FbDropZone.Right => FbDropFeedback.Right,
            _ => FbDropFeedback.Center
        };

        /// <summary>
        /// True for a feedback that lets the item be dropped.
        /// </summary>
        public static bool IsAccepted(FbDropFeedback feedback) => feedback is not (FbDropFeedback.None or FbDropFeedback.Forbidden);
    }
}
