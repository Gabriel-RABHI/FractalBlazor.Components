using FractalBlazor.Components.Common.Contracts;
using FractalBlazor.Components.Layout.Abstracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Forms.Components.DragAndDrop
{
    /// <summary>
    /// A surface where a dragged item can be dropped, identified by <see cref="DropId"/>. Its
    /// surface is split into zones (<see cref="Split"/>, or a <c>Z*</c> flag): top, center,
    /// bottom, left, right. The surface flags (fill, frame, radius, padding…) apply to it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// At rest, the receiver adds nothing to its content. During a drag, if the rules of the
    /// area accept the dragged item, a layer of zones covers the receiver: the content below
    /// does not catch the drag, and the layer shows where the item would go. A receiver that
    /// refuses the item stays out of the drag.
    /// </para>
    /// <para>
    /// Receivers nest, and a draggable can be a receiver: an inner receiver lies over the
    /// receiver around it.
    /// </para>
    /// </remarks>
    public class FbDraggableReceiver : FbLayoutSurfaceComponentBase, IDisposable
    {
        private FbDropSplit _split = FbDropSplit.TopCenterBottom;
        private FbDropSplit _defaultSplit;
        private FbDragAndDropMaster? _registeredIn;
        private FbDropZones? _zones;
        private string? _surfaceClasses;
        private string _classes = "";

        [CascadingParameter]
        public FbDragAndDropMaster? Master { get; set; }

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Its share of the free space of the row or the stack around it, as for an FbRow: by
        /// default it keeps the size of its content.
        /// </summary>
        [Parameter]
        public new int Flex { get => base.Flex; set => base.Flex = value; }

        /// <summary>
        /// The identifier of the receiver, given to the rules of the area and to the drop:
        /// often the state object it shows.
        /// </summary>
        [Parameter]
        public object? DropId { get; set; }

        /// <summary>
        /// How the surface is split into zones: top, center and bottom thirds by default.
        /// </summary>
        [Parameter]
        public FbDropSplit Split { get => _split; set => _split = value; }

        /// <summary>
        /// Zones -> Center: the whole surface.
        /// </summary>
        [Parameter]
        public bool ZC { get => _split == FbDropSplit.Center; set { if (value) _split = FbDropSplit.Center; } }

        /// <summary>
        /// Zones -> Top, Bottom: two halves.
        /// </summary>
        [Parameter]
        public bool ZTB { get => _split == FbDropSplit.TopBottom; set { if (value) _split = FbDropSplit.TopBottom; } }

        /// <summary>
        /// Zones -> Top, Center, Bottom: three thirds, the default.
        /// </summary>
        [Parameter]
        public bool ZTCB { get => _split == FbDropSplit.TopCenterBottom; set { if (value) _split = FbDropSplit.TopCenterBottom; } }

        /// <summary>
        /// Zones -> Left, Top, Center, Bottom, Right.
        /// </summary>
        [Parameter]
        public bool ZLTCBR { get => _split == FbDropSplit.LeftTopCenterBottomRight; set { if (value) _split = FbDropSplit.LeftTopCenterBottomRight; } }

        /// <summary>
        /// Zones -> Left, Top, Bottom, Right.
        /// </summary>
        [Parameter]
        public bool ZLTBR { get => _split == FbDropSplit.LeftTopBottomRight; set { if (value) _split = FbDropSplit.LeftTopBottomRight; } }

        /// <summary>
        /// Zones -> Left, Center, Right.
        /// </summary>
        [Parameter]
        public bool ZLCR { get => _split == FbDropSplit.LeftCenterRight; set { if (value) _split = FbDropSplit.LeftCenterRight; } }

        /// <summary>
        /// Zones -> Left, Right: two halves.
        /// </summary>
        [Parameter]
        public bool ZLR { get => _split == FbDropSplit.LeftRight; set { if (value) _split = FbDropSplit.LeftRight; } }

        /// <summary>
        /// True while a drag is on and the rules accept the dragged item here.
        /// </summary>
        internal bool Accepting { get; private set; }

        /// <summary>
        /// The zone the dragged item is over, if any.
        /// </summary>
        internal FbDropZone? Zone { get; private set; }

        /// <summary>
        /// What a drop on <see cref="Zone"/> would do.
        /// </summary>
        internal FbDropFeedback Feedback { get; private set; }

        private string ReceiverClasses
        {
            get
            {
                var surfaceClasses = AggregatedClasses;
                if (!ReferenceEquals(surfaceClasses, _surfaceClasses) || _classes.Length == 0)
                {
                    _surfaceClasses = surfaceClasses;
                    _classes = string.IsNullOrEmpty(surfaceClasses) ? "fb-drop-receiver" : "fb-drop-receiver " + surfaceClasses;
                }
                return _classes;
            }
        }

        protected override void CaptureParameterDefaults()
        {
            base.CaptureParameterDefaults();
            _defaultSplit = _split;
        }

        protected override void RestoreParameterDefaults()
        {
            base.RestoreParameterDefaults();
            _split = _defaultSplit;
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (Master == _registeredIn)
                return;

            _registeredIn?.Unregister(this);
            Master?.Register(this);
            _registeredIn = Master;
        }

        /// <summary>
        /// The zone layer comes before the content: an inner receiver, later in the document,
        /// lies over it.
        /// </summary>
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            var error = Master is null ? "FbDraggableReceiver must be placed inside an FbDragAndDropMaster."
                : DropId is null ? "FbDraggableReceiver needs a DropId."
                : null;
            if (error is not null)
            {
                _zones = null;
                FbDragAndDropError.Render(builder, 0, error);
                builder.AddContent(1, ChildContent);
                return;
            }

            builder.OpenElement(2, "div");
            builder.AddAttribute(3, "class", ReceiverClasses);
            builder.AddAttribute(4, "style", AggregatedStyles);
            builder.OpenComponent<FbDropZones>(5);
            builder.AddComponentParameter(6, nameof(FbDropZones.Receiver), this);
            builder.AddComponentReferenceCapture(7, zones => _zones = (FbDropZones)zones);
            builder.CloseComponent();
            builder.AddContent(8, ChildContent);
            builder.CloseElement();
        }

        internal void Begin(bool accepting)
        {
            (Accepting, Zone, Feedback) = (accepting, null, FbDropFeedback.None);
            _zones?.Refresh();
        }

        internal void Show(FbDropZone? zone, FbDropFeedback feedback)
        {
            (Zone, Feedback) = (zone, feedback);
            _zones?.Refresh();
        }

        internal void Finish()
        {
            (Accepting, Zone, Feedback) = (false, null, FbDropFeedback.None);
            _zones?.Refresh();
        }

        public void Dispose()
        {
            _registeredIn?.Unregister(this);
            _registeredIn = null;
        }
    }
}
