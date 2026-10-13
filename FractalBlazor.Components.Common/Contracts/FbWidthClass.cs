namespace FractalBlazor.Components.Common.Contracts
{
    /// <summary>
    /// Width category of an area: the largest breakpoint its width reaches, with the
    /// breakpoints of the responsive flags of Layout (<c>VO_*</c>, <c>VU_*</c>) and of the grid.
    /// The values are ordered, so a state compares them: <c>Width &lt; FbWidthClass.M</c>.
    /// </summary>
    public enum FbWidthClass : byte
    {
        /// <summary>Under 512 px.</summary>
        Base,

        /// <summary>512 px and more.</summary>
        XS,

        /// <summary>640 px and more.</summary>
        S,

        /// <summary>768 px and more.</summary>
        M,

        /// <summary>1024 px and more.</summary>
        L,

        /// <summary>1280 px and more.</summary>
        XL,

        /// <summary>1536 px and more.</summary>
        XXL
    }

    /// <summary>
    /// The breakpoints of <see cref="FbWidthClass"/>: the only source of their values in C#.
    /// </summary>
    public static class FbWidthClasses
    {
        /// <summary>
        /// Lower bound, in CSS pixels, of each category above <see cref="FbWidthClass.Base"/>,
        /// in order: <c>XS</c>, <c>S</c>, <c>M</c>, <c>L</c>, <c>XL</c>, <c>XXL</c>.
        /// </summary>
        public static IReadOnlyList<int> Thresholds { get; } = [512, 640, 768, 1024, 1280, 1536];

        /// <summary>
        /// Category of an area <paramref name="width"/> pixels wide.
        /// </summary>
        public static FbWidthClass Of(double width)
        {
            var category = FbWidthClass.Base;
            for (var index = 0; index < Thresholds.Count && width >= Thresholds[index]; index++)
                category = (FbWidthClass)(index + 1);
            return category;
        }
    }
}
