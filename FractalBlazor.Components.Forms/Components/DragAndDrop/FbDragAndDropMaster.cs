using FractalBlazor.Components.Common.Contracts;
using FractalBlazor.Components.Forms.Core;
using FractalBlazor.Components.Forms.Theming.Constants;
using FractalBlazor.Components.Layout.Abstracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Forms.Components.DragAndDrop
{
    /// <summary>
    /// A drag and drop area: the <see cref="FbDraggable"/> items and the
    /// <see cref="FbDraggableReceiver"/> receivers it contains work together. A page may hold
    /// several areas, each with its own rules.
    /// </summary>
    /// <remarks>
    /// <para>
    /// State-first: the area changes nothing by itself. When an item is released on a zone
    /// that accepts it, the area sends <see cref="FbDrop"/> to <see cref="Handler"/>. When
    /// the handler implements <see cref="IFbDragAndDropRules"/>, it also decides which
    /// receivers accept the dragged item and what a drop on each zone would do; without it,
    /// every receiver accepts and each zone offers its natural place.
    /// </para>
    /// <para>
    /// The area renders no element and never renders again during a drag: only the zone layer
    /// of the receivers concerned does, when the drag starts, enters a zone, leaves it, or
    /// ends.
    /// </para>
    /// </remarks>
    public sealed class FbDragAndDropMaster : FbComponentBase
    {
        private readonly List<FbDraggableReceiver> _receivers = [];
        private readonly RenderFragment _content;
        private FbDragAndDropInfo? _info;
        private FbDraggable? _source;

        public FbDragAndDropMaster()
        {
            _content = BuildContent;
        }

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// The state that receives <see cref="FbDrop"/>, handled by its <c>Handle</c> method,
        /// and that may implement <see cref="IFbDragAndDropRules"/>.
        /// </summary>
        [Parameter]
        public object? Handler { get; set; }

        /// <summary>
        /// Variant of the signs of an accepted drop: the receivers that accept the dragged
        /// item, and the place it would take.
        /// </summary>
        [Parameter]
        public string DropVariant { get; set; } = FbThemeVariants.Selected;

        /// <summary>
        /// Variant of a zone that forbids the drop.
        /// </summary>
        [Parameter]
        public string ForbiddenVariant { get; set; } = FbThemeVariants.Error;

        /// <summary>
        /// Shows the state of the drag: the receivers of the area, the dragged item, and the
        /// zone and the preview of each receiver.
        /// </summary>
        [Parameter]
        public bool ShowInfo { get; set; }

        /// <summary>
        /// The identifier of the dragged item, or <c>null</c> outside a drag.
        /// </summary>
        public object? DragId { get; private set; }

        public bool IsDragging => _source is not null;

        internal int ReceiverCount => _receivers.Count;

        /// <summary>
        /// The receiver the dragged item is over, if any.
        /// </summary>
        internal FbDraggableReceiver? Over { get; private set; }

        private IFbDragAndDropRules? Rules => Handler as IFbDragAndDropRules;

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<CascadingValue<FbDragAndDropMaster>>(0);
            builder.AddComponentParameter(1, nameof(CascadingValue<FbDragAndDropMaster>.Value), this);
            builder.AddComponentParameter(2, nameof(CascadingValue<FbDragAndDropMaster>.IsFixed), true);
            builder.AddComponentParameter(3, nameof(CascadingValue<FbDragAndDropMaster>.ChildContent), _content);
            builder.CloseComponent();
        }

        private void BuildContent(RenderTreeBuilder builder)
        {
            // The reference is captured when the line is created, and kept while it stays.
            if (!ShowInfo)
                _info = null;
            else
            {
                builder.OpenComponent<FbDragAndDropInfo>(0);
                builder.AddComponentParameter(1, nameof(FbDragAndDropInfo.Master), this);
                builder.AddComponentReferenceCapture(2, info => _info = (FbDragAndDropInfo)info);
                builder.CloseComponent();
            }
            builder.AddContent(3, ChildContent);
        }

        internal void Register(FbDraggableReceiver receiver) => _receivers.Add(receiver);

        internal void Unregister(FbDraggableReceiver receiver)
        {
            _receivers.Remove(receiver);
            if (Over == receiver)
                Over = null;
        }

        /// <summary>
        /// A drag starts on <paramref name="source"/>: every receiver learns whether it
        /// accepts the item. The browser takes the image of the dragged item once the
        /// <c>dragstart</c> handlers are done; the zones appear after it, so that the page
        /// does not change under it.
        /// </summary>
        internal async Task StartAsync(FbDraggable source)
        {
            if (source.DragId is null)
                return;

            _source = source;
            DragId = source.DragId;
            Over = null;
            await Task.Yield();
            if (_source != source)
                return;

            foreach (var receiver in _receivers.ToArray())
                receiver.Begin(Accepts(receiver));
            RefreshInfo();
        }

        /// <summary>
        /// The drag ends, dropped or not: the receivers go back to rest. A drag ended by a
        /// drop is already over when its source reports the end.
        /// </summary>
        internal void End(FbDraggable? source = null)
        {
            if (_source is null || (source is not null && source != _source))
                return;

            _source = null;
            DragId = null;
            Over = null;
            foreach (var receiver in _receivers.ToArray())
                receiver.Finish();
            RefreshInfo();
        }

        /// <summary>
        /// The dragged item enters a zone of <paramref name="receiver"/>: the receiver it
        /// leaves goes back to waiting, and this one shows what a drop would do.
        /// </summary>
        internal void Enter(FbDraggableReceiver receiver, FbDropZone zone)
        {
            if (DragId is null || receiver.DropId is null)
                return;

            if (Over is { } previous && previous != receiver)
                previous.Show(null, FbDropFeedback.None);
            Over = receiver;
            receiver.Show(zone, Preview(receiver.DropId, zone));
            RefreshInfo();
        }

        /// <summary>
        /// The dragged item leaves a zone. Entering the next zone comes first: a zone that is
        /// no longer the current one is left already.
        /// </summary>
        internal void Leave(FbDraggableReceiver receiver, FbDropZone zone)
        {
            if (receiver.Zone != zone)
                return;

            receiver.Show(null, FbDropFeedback.None);
            if (Over == receiver)
                Over = null;
            RefreshInfo();
        }

        /// <summary>
        /// The item is released on a zone: the drag ends, then, if the zone accepts it,
        /// <see cref="FbDrop"/> goes to <see cref="Handler"/>.
        /// </summary>
        internal void Drop(FbDraggableReceiver receiver, FbDropZone zone)
        {
            if (DragId is not { } dragId || receiver.DropId is not { } dropId)
                return;

            var feedback = receiver.Zone == zone ? receiver.Feedback : Preview(dropId, zone);
            End();
            if (FbDrop.IsAccepted(feedback) && Handler is not null)
                FbActionDispatcher.Dispatch(Handler, new FbDrop(dragId, dropId, zone, feedback));
        }

        private bool Accepts(FbDraggableReceiver receiver)
            => DragId is { } dragId && receiver.DropId is { } dropId && (Rules?.Accepts(dragId, dropId) ?? true);

        private FbDropFeedback Preview(object dropId, FbDropZone zone)
            => DragId is { } dragId && Rules is { } rules ? rules.Preview(dragId, dropId, zone) : FbDrop.NaturalFeedback(zone);

        private void RefreshInfo() => _info?.Refresh();
    }
}
