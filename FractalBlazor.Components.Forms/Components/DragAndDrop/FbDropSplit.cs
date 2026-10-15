using FractalBlazor.Components.Common.Contracts;

namespace FractalBlazor.Components.Forms.Components.DragAndDrop
{
    /// <summary>
    /// How a receiver splits its surface into drop zones.
    /// </summary>
    public enum FbDropSplit : byte
    {
        /// <summary>
        /// One zone, the whole surface.
        /// </summary>
        Center,

        /// <summary>
        /// Top and bottom halves.
        /// </summary>
        TopBottom,

        /// <summary>
        /// Top, center and bottom thirds.
        /// </summary>
        TopCenterBottom,

        /// <summary>
        /// A left and a right fifth; top, center and bottom thirds between them.
        /// </summary>
        LeftTopCenterBottomRight,

        /// <summary>
        /// A left and a right fifth; top and bottom halves between them.
        /// </summary>
        LeftTopBottomRight,

        /// <summary>
        /// A left and a right fifth, the center between them.
        /// </summary>
        LeftCenterRight,

        /// <summary>
        /// Left and right halves.
        /// </summary>
        LeftRight
    }

    /// <summary>
    /// The zones of each split and the class that lays them out: a grid of named areas in the
    /// stylesheet, so a zone carries no inline geometry.
    /// </summary>
    internal static class FbDropSplits
    {
        private static readonly FbDropZone[][] _zones =
        [
            [FbDropZone.Center],
            [FbDropZone.Top, FbDropZone.Bottom],
            [FbDropZone.Top, FbDropZone.Center, FbDropZone.Bottom],
            [FbDropZone.Left, FbDropZone.Top, FbDropZone.Center, FbDropZone.Bottom, FbDropZone.Right],
            [FbDropZone.Left, FbDropZone.Top, FbDropZone.Bottom, FbDropZone.Right],
            [FbDropZone.Left, FbDropZone.Center, FbDropZone.Right],
            [FbDropZone.Left, FbDropZone.Right]
        ];

        private static readonly string[] _classes =
        [
            "fb-drop-zones fb-drop-split-c",
            "fb-drop-zones fb-drop-split-tb",
            "fb-drop-zones fb-drop-split-tcb",
            "fb-drop-zones fb-drop-split-ltcbr",
            "fb-drop-zones fb-drop-split-ltbr",
            "fb-drop-zones fb-drop-split-lcr",
            "fb-drop-zones fb-drop-split-lr"
        ];

        public static IReadOnlyList<FbDropZone> Zones(FbDropSplit split) => _zones[(int)split];

        public static string Classes(FbDropSplit split) => _classes[(int)split];

        public static string ZoneClass(FbDropZone zone) => zone switch
        {
            FbDropZone.Top => "fb-drop-zone fb-drop-zone-top",
            FbDropZone.Bottom => "fb-drop-zone fb-drop-zone-bottom",
            FbDropZone.Left => "fb-drop-zone fb-drop-zone-left",
            FbDropZone.Right => "fb-drop-zone fb-drop-zone-right",
            _ => "fb-drop-zone fb-drop-zone-center"
        };
    }
}
