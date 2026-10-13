using FractalBlazor.Components.Layout.Abstracts;
using FractalBlazor.Components.Layout.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout
{
    /// <summary>
    /// Root of a single-page application layout. It fills the browser window, width and height,
    /// and the page itself never scrolls: its children are laid out in a column, a growing
    /// child takes the whole height, and a <c>Scrollable</c> stack or row inside it takes the
    /// space left by its siblings and scrolls on its own.
    /// </summary>
    public class FbPageAnchor : FbLayoutSurfaceComponentBase
    {
        #region HIDDEN
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1)]
        private struct AnchorKey
        {
            public SurfaceKey Surface;
        }

        private string AnchorClasses
            => ResolveClasses(new AnchorKey { Surface = SurfaceKeyValue },
                static component => ((FbPageAnchor)component).BuildAnchorClasses());

        private string BuildAnchorClasses()
        {
            var line = new FbAtomLine();
            AddSurfaceAtoms(line);
            return line.Classes(FbClasses.Join("fb-page-anchor", BuildSurfaceClasses()), null);
        }
        #endregion

        // ************************************************************************************************ //
        // ***************************************    PUBLIC   ******************************************** //
        // ************************************************************************************************ //

        /// <summary>
        /// The application layout, usually a single stack.
        /// </summary>
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "style", AggregatedStyles);
            builder.AddAttribute(2, "class", AnchorClasses);
            builder.AddContent(3, ChildContent);
            builder.CloseElement();
        }
    }
}
