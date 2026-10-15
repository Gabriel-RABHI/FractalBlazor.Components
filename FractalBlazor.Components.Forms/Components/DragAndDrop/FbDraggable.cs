using FractalBlazor.Components.Layout.Abstracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace FractalBlazor.Components.Forms.Components.DragAndDrop
{
    /// <summary>
    /// An item that can be dragged, identified by <see cref="DragId"/>. It is grabbed by its
    /// <see cref="FbDraggableHandle"/> handles only: the rest of its content keeps its own
    /// behavior (text selection, fields, buttons), and an item without a handle stays in
    /// place. The surface flags (fill, frame, radius, padding…) apply to it.
    /// </summary>
    /// <remarks>
    /// Draggables nest: a handle moves the nearest draggable around it. Only that one starts
    /// the drag; an image, a link or a text selection dragged from its content stays a drag
    /// of the browser.
    /// </remarks>
    public class FbDraggable : FbLayoutSurfaceComponentBase
    {
        private readonly RenderFragment _body;
        private readonly EventCallback<DragEventArgs> _onDragStart, _onDragEnd;
        private string? _surfaceClasses;
        private string _classes = "";

        public FbDraggable()
        {
            _body = BuildBody;
            _onDragStart = EventCallback.Factory.Create<DragEventArgs>(this, StartAsync);
            _onDragEnd = EventCallback.Factory.Create<DragEventArgs>(this, End);
        }

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
        /// The identifier of the item, given to the rules of the area and to the drop: often
        /// the state object the item shows.
        /// </summary>
        [Parameter]
        public object? DragId { get; set; }

        private string DraggableClasses
        {
            get
            {
                var surfaceClasses = AggregatedClasses;
                if (!ReferenceEquals(surfaceClasses, _surfaceClasses) || _classes.Length == 0)
                {
                    _surfaceClasses = surfaceClasses;
                    _classes = string.IsNullOrEmpty(surfaceClasses) ? "fb-draggable" : "fb-draggable " + surfaceClasses;
                }
                return _classes;
            }
        }

        /// <summary>
        /// The handles find their draggable by a fixed cascading value: the nearest one.
        /// </summary>
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<CascadingValue<FbDraggable>>(0);
            builder.AddComponentParameter(1, nameof(CascadingValue<FbDraggable>.Value), this);
            builder.AddComponentParameter(2, nameof(CascadingValue<FbDraggable>.IsFixed), true);
            builder.AddComponentParameter(3, nameof(CascadingValue<FbDraggable>.ChildContent), _body);
            builder.CloseComponent();
        }

        /// <summary>
        /// The element is not draggable at rest: a press on a handle makes it so, until the
        /// press or the drag ends (script of Layout). The start and the end of its drag do not
        /// reach the draggables around it.
        /// </summary>
        private void BuildBody(RenderTreeBuilder builder)
        {
            var error = Master is null ? "FbDraggable must be placed inside an FbDragAndDropMaster."
                : DragId is null ? "FbDraggable needs a DragId."
                : null;
            if (error is not null)
            {
                FbDragAndDropError.Render(builder, 0, error);
                builder.AddContent(1, ChildContent);
                return;
            }

            builder.OpenElement(2, "div");
            builder.AddAttribute(3, "class", DraggableClasses);
            builder.AddAttribute(4, "style", AggregatedStyles);
            builder.AddAttribute(5, "draggable", "false");
            builder.AddAttribute(6, "ondragstart", _onDragStart);
            builder.AddEventStopPropagationAttribute(7, "ondragstart", true);
            builder.AddAttribute(8, "ondragend", _onDragEnd);
            builder.AddEventStopPropagationAttribute(9, "ondragend", true);
            builder.AddContent(10, ChildContent);
            builder.CloseElement();
        }

        private Task StartAsync(DragEventArgs args) => Master?.StartAsync(this) ?? Task.CompletedTask;

        private void End(DragEventArgs args) => Master?.End(this);
    }
}
