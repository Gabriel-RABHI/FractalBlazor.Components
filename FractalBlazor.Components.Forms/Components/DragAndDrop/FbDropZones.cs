using FractalBlazor.Components.Common.Contracts;
using FractalBlazor.Components.Forms.Components.Display;
using FractalBlazor.Components.Forms.Theming.Helpers;
using FractalBlazor.Components.Layout.Abstracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace FractalBlazor.Components.Forms.Components.DragAndDrop
{
    /// <summary>
    /// The zone layer of a receiver: the only part of an area that renders during a drag. At
    /// rest, or when the receiver refuses the dragged item, it renders nothing.
    /// </summary>
    /// <remarks>
    /// While the receiver accepts, a feedback box shows its state (waiting, the place of the
    /// drop, forbidden), and empty zones laid out by the split catch the drag events. A zone
    /// where the preview refuses the drop carries <c>fb-drop-refused</c>: the script of Layout
    /// then no longer lets the item in, and the browser shows a refusal.
    /// </remarks>
    internal sealed class FbDropZones : FbComponentBase
    {
        private const int ZoneCount = 5;

        private readonly EventCallback<DragEventArgs>[] _enter = new EventCallback<DragEventArgs>[ZoneCount];
        private readonly EventCallback<DragEventArgs>[] _leave = new EventCallback<DragEventArgs>[ZoneCount];
        private readonly EventCallback<DragEventArgs>[] _drop = new EventCallback<DragEventArgs>[ZoneCount];

        // What the last render showed: a refresh that changes nothing renders nothing. Whether
        // a drag is on only shows in the information line.
        private (bool Accepting, FbDropZone? Zone, FbDropFeedback Feedback, bool Info, bool Dragging) _shown;

        public FbDropZones()
        {
            for (var index = 0; index < ZoneCount; index++)
            {
                var zone = (FbDropZone)index;
                _enter[index] = EventCallback.Factory.Create<DragEventArgs>(this, () => Master?.Enter(Receiver, zone));
                _leave[index] = EventCallback.Factory.Create<DragEventArgs>(this, () => Master?.Leave(Receiver, zone));
                _drop[index] = EventCallback.Factory.Create<DragEventArgs>(this, () => Master?.Drop(Receiver, zone));
            }
        }

        [Parameter, EditorRequired]
        public FbDraggableReceiver Receiver { get; set; } = default!;

        private FbDragAndDropMaster? Master => Receiver.Master;

        private (bool, FbDropZone?, FbDropFeedback, bool, bool) Showing
            => (Receiver.Accepting, Receiver.Zone, Receiver.Feedback, Master?.ShowInfo == true, Master is { ShowInfo: true, IsDragging: true });

        internal void Refresh()
        {
            if (Showing != _shown)
                Render();
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            _shown = Showing;
            if (Master is not { } master)
                return;

            if (master.ShowInfo)
            {
                builder.OpenElement(0, "span");
                builder.AddAttribute(1, "class", "fb-drop-info");
                builder.OpenComponent<FbText>(2);
                builder.AddComponentParameter(3, nameof(FbText.XS), true);
                builder.AddComponentParameter(4, nameof(FbText.Code), true);
                builder.AddComponentParameter(5, nameof(FbText.Value), InfoText(master));
                builder.CloseComponent();
                builder.CloseElement();
            }

            if (!Receiver.Accepting)
                return;

            builder.OpenElement(6, "div");
            builder.AddAttribute(7, "class", FeedbackClasses(master));
            builder.CloseElement();

            builder.OpenElement(8, "div");
            builder.AddAttribute(9, "class", FbDropSplits.Classes(Receiver.Split));
            foreach (var zone in FbDropSplits.Zones(Receiver.Split))
            {
                var index = (int)zone;
                builder.OpenElement(10, "div");
                builder.SetKey(zone);
                builder.AddAttribute(11, "class", IsRefused(zone) ? FbDropSplits.ZoneClass(zone) + " fb-drop-refused" : FbDropSplits.ZoneClass(zone));
                builder.AddAttribute(12, "ondragenter", _enter[index]);
                builder.AddAttribute(13, "ondragleave", _leave[index]);
                builder.AddAttribute(14, "ondrop", _drop[index]);
                builder.CloseElement();
            }
            builder.CloseElement();
        }

        private bool IsRefused(FbDropZone zone) => Receiver.Zone == zone && !FbDrop.IsAccepted(Receiver.Feedback);

        /// <summary>
        /// The feedback box, in the variant of an accepted drop or of a forbidden one: waiting
        /// while the item is elsewhere, then the place the drop would take.
        /// </summary>
        private string FeedbackClasses(FbDragAndDropMaster master)
        {
            var feedback = Receiver.Zone is null ? FbDropFeedback.None : Receiver.Feedback;
            var variant = feedback == FbDropFeedback.Forbidden ? master.ForbiddenVariant : master.DropVariant;
            return FbVariantScope.WithClasses(feedback switch
            {
                FbDropFeedback.Forbidden => "fb-drop-feedback fb-drop-forbidden",
                FbDropFeedback.Top => "fb-drop-feedback fb-drop-top",
                FbDropFeedback.Center => "fb-drop-feedback fb-drop-center",
                FbDropFeedback.Bottom => "fb-drop-feedback fb-drop-bottom",
                FbDropFeedback.Left => "fb-drop-feedback fb-drop-left",
                FbDropFeedback.Right => "fb-drop-feedback fb-drop-right",
                _ => "fb-drop-feedback"
            }, FbVariantScope.Classes(variant));
        }

        private string InfoText(FbDragAndDropMaster master)
            => !master.IsDragging ? "rest"
                : !Receiver.Accepting ? "refuses"
                : Receiver.Zone is { } zone ? $"{zone} → {Receiver.Feedback}"
                : "accepts";
    }
}
