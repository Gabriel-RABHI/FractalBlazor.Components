namespace FractalBlazor.Components.Forms.Components.Inputs
{
    /// <summary>
    /// How a slider marks its ticks on the track. The marks of the filled part take the accent.
    /// </summary>
    public enum FbTickMark : byte
    {
        /// <summary>
        /// No mark: the ticks only carry their labels.
        /// </summary>
        None,

        /// <summary>
        /// A dot on the track.
        /// </summary>
        Dot,

        /// <summary>
        /// A short line across the track.
        /// </summary>
        Line,

        /// <summary>
        /// A short line beside the track, on the side of the values: above it, or on its left
        /// when vertical; <c>Mirror</c> moves it to the other side.
        /// </summary>
        ShiftedLine
    }
}
