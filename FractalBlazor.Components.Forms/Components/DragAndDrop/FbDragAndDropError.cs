using FractalBlazor.Components.Forms.Components.Display;
using FractalBlazor.Components.Forms.Components.Icons;
using FractalBlazor.Components.Forms.Theming;
using FractalBlazor.Components.Forms.Theming.Constants;
using FractalBlazor.Components.Layout;
using FractalBlazor.Components.Layout.Abstracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Forms.Components.DragAndDrop
{
    /// <summary>
    /// A drag and drop component placed where it cannot work says so above its content, in
    /// the Error variant on the surface background.
    /// </summary>
    internal sealed class FbDragAndDropError : FbComponentBase
    {
        private readonly RenderFragment _row, _content;

        public FbDragAndDropError()
        {
            _row = BuildRow;
            _content = BuildContent;
        }

        [Parameter, EditorRequired]
        public string Message { get; set; } = "";

        public static void Render(RenderTreeBuilder builder, int sequence, string message)
        {
            builder.OpenComponent<FbDragAndDropError>(sequence);
            builder.AddComponentParameter(sequence + 1, nameof(Message), message);
            builder.CloseComponent();
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<FbVariant>(0);
            builder.AddComponentParameter(1, nameof(FbVariant.Variant), FbThemeVariants.Error);
            builder.AddComponentParameter(2, nameof(FbVariant.ChildContent), _row);
            builder.CloseComponent();
        }

        private void BuildRow(RenderTreeBuilder builder)
        {
            builder.OpenComponent<FbRow>(0);
            builder.AddComponentParameter(1, nameof(FbRow.NoFlex), true);
            builder.AddComponentParameter(2, nameof(FbRow.GS), true);
            builder.AddComponentParameter(3, nameof(FbRow.PHM), true);
            builder.AddComponentParameter(4, nameof(FbRow.PVS), true);
            builder.AddComponentParameter(5, nameof(FbRow.WBS), true);
            builder.AddComponentParameter(6, nameof(FbRow.WFM), true);
            builder.AddComponentParameter(7, nameof(FbRow.WRM), true);
            builder.AddComponentParameter(8, nameof(FbRow.ChildContent), _content);
            builder.CloseComponent();
        }

        private void BuildContent(RenderTreeBuilder builder)
        {
            builder.OpenComponent<FbIcon>(0);
            builder.AddComponentParameter(1, nameof(FbIcon.Icon), FbMaterialIcons.Error);
            builder.AddComponentParameter(2, nameof(FbIcon.S), true);
            builder.CloseComponent();
            builder.OpenComponent<FbText>(3);
            builder.AddComponentParameter(4, nameof(FbText.S), true);
            builder.AddComponentParameter(5, nameof(FbText.Value), Message);
            builder.CloseComponent();
        }
    }
}
