namespace FractalBlazor.Components.Layout
{
    /// <summary>
    /// Edge a drawer slides from.
    /// </summary>
    public enum FbDrawerSide : byte
    {
        Left,
        Right,
        Top,
        Bottom
    }

    /// <summary>
    /// Where a drawer opens.
    /// </summary>
    public enum FbDrawerMode : byte
    {
        /// <summary>
        /// On top of the whole screen, in a native modal dialog: the page is inert, the focus
        /// stays in the drawer and comes back when it closes.
        /// </summary>
        Modal,

        /// <summary>
        /// At its place in the layout, which it pushes when it opens.
        /// </summary>
        Inline,

        /// <summary>
        /// At its place, but on top of its zone (its nearest positioned ancestor), with a
        /// backdrop limited to that zone. The rest of the page stays usable.
        /// </summary>
        InlineModal
    }
}
