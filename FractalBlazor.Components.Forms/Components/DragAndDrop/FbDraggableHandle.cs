using FractalBlazor.Components.Layout.Abstracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Forms.Components.DragAndDrop
{
    /// <summary>
    /// The part of an <see cref="FbDraggable"/> by which it is grabbed: a press on it makes the
    /// nearest draggable around it draggable. A draggable may have several handles.
    /// </summary>
    /// <remarks>
    /// The handle produces no box (<c>display: contents</c>): its content is laid out as if it
    /// were not there, and takes the grab cursor.
    /// </remarks>
    public sealed class FbDraggableHandle : FbComponentBase
    {
        [CascadingParameter]
        public FbDraggable? Draggable { get; set; }

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (Draggable is null)
            {
                FbDragAndDropError.Render(builder, 0, "FbDraggableHandle must be placed inside an FbDraggable.");
                builder.AddContent(1, ChildContent);
                return;
            }

            builder.OpenElement(2, "div");
            builder.AddAttribute(3, "class", "fb-drag-handle");
            builder.AddContent(4, ChildContent);
            builder.CloseElement();
        }
    }
}
