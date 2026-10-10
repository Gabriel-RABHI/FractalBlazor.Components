using FractalBlazor.Components.Layout.Abstracts;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout
{
    /// <summary>
    /// Small indeterminate spinner. Its colors come from the active theme.
    /// </summary>
    public sealed class FbCircularProgress : FbComponentBase
    {
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.AddMarkupContent(0,
                "<div class=\"fb-prog-loader\"><svg class=\"fb-prog-circular\" viewBox=\"0 0 24 24\">" +
                "<circle class=\"fb-prog-path-back\" cx=\"12\" cy=\"12\" r=\"10\" fill=\"none\" stroke-width=\"1\" stroke-miterlimit=\"10\"></circle>" +
                "<circle class=\"fb-prog-path\" cx=\"12\" cy=\"12\" r=\"10\" fill=\"none\" stroke-width=\"3\" stroke-miterlimit=\"10\"></circle>" +
                "</svg></div>");
        }
    }
}
