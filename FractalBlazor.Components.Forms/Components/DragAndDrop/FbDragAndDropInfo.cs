using FractalBlazor.Components.Forms.Components.Display;
using FractalBlazor.Components.Layout;
using FractalBlazor.Components.Layout.Abstracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Forms.Components.DragAndDrop
{
    /// <summary>
    /// The information line of an area (<see cref="FbDragAndDropMaster.ShowInfo"/>): its
    /// receivers, the dragged item and the receiver it is over. The area refreshes it alone.
    /// </summary>
    internal sealed class FbDragAndDropInfo : FbComponentBase
    {
        private readonly RenderFragment _content;

        public FbDragAndDropInfo()
        {
            _content = BuildContent;
        }

        [Parameter, EditorRequired]
        public FbDragAndDropMaster Master { get; set; } = default!;

        internal void Refresh() => Render();

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<FbRow>(0);
            builder.AddComponentParameter(1, nameof(FbRow.NoFlex), true);
            builder.AddComponentParameter(2, nameof(FbRow.MultiLine), true);
            builder.AddComponentParameter(3, nameof(FbRow.GM), true);
            builder.AddComponentParameter(4, nameof(FbRow.PHM), true);
            builder.AddComponentParameter(5, nameof(FbRow.PVS), true);
            builder.AddComponentParameter(6, nameof(FbRow.WBA), true);
            builder.AddComponentParameter(7, nameof(FbRow.WRM), true);
            builder.AddComponentParameter(8, nameof(FbRow.ChildContent), _content);
            builder.CloseComponent();
        }

        private void BuildContent(RenderTreeBuilder builder)
        {
            Item(builder, 0, "Receivers", Master.ReceiverCount.ToString());
            Item(builder, 10, "Dragged", Master.DragId?.ToString() ?? "—");
            Item(builder, 20, "Over", Master.Over?.DropId?.ToString() ?? "—");
        }

        private static void Item(RenderTreeBuilder builder, int sequence, string label, string value)
        {
            builder.OpenComponent<FbText>(sequence);
            builder.AddComponentParameter(sequence + 1, nameof(FbText.S), true);
            builder.AddComponentParameter(sequence + 2, nameof(FbText.Muted), true);
            builder.AddComponentParameter(sequence + 3, nameof(FbText.Value), label);
            builder.CloseComponent();
            builder.OpenComponent<FbText>(sequence + 4);
            builder.AddComponentParameter(sequence + 5, nameof(FbText.S), true);
            builder.AddComponentParameter(sequence + 6, nameof(FbText.Code), true);
            builder.AddComponentParameter(sequence + 7, nameof(FbText.Value), value);
            builder.CloseComponent();
        }
    }
}
