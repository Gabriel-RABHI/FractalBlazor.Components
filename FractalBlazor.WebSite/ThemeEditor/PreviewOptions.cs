using System.ComponentModel.DataAnnotations;

namespace FractalBlazor.WebSite.ThemeEditor
{
    /// <summary>
    /// Background of a preview sample. <see cref="Hover"/> is a surface that reacts to the
    /// mouse; <see cref="None"/> leaves the parent background.
    /// </summary>
    public enum PreviewBackground
    {
        Surface,
        Accent,
        Highlight,

        [Display(Name = "Survol")]
        Hover,

        [Display(Name = "Sans fond")]
        None
    }

    public enum PreviewFrame
    {
        [Display(Name = "Sans frame")]
        None,

        [Display(Name = "Frame Light")]
        Light,

        [Display(Name = "Frame Medium")]
        Medium,

        [Display(Name = "Frame Strong")]
        Strong
    }

    public enum PreviewText
    {
        Subtle,
        Muted,
        Default,
        Highlight
    }

    public enum PreviewSize
    {
        XS,
        S,
        M,
        L,
        XL
    }

    /// <summary>
    /// The values each preview tab walks through.
    /// </summary>
    public static class PreviewOptions
    {
        public static IReadOnlyList<PreviewBackground> SurfaceBackgrounds { get; } =
            [PreviewBackground.Surface, PreviewBackground.Accent, PreviewBackground.Highlight, PreviewBackground.Hover];

        public static IReadOnlyList<PreviewBackground> ButtonBackgrounds { get; } =
            [PreviewBackground.Surface, PreviewBackground.Accent, PreviewBackground.Highlight, PreviewBackground.None];

        public static IReadOnlyList<PreviewFrame> Frames { get; } = [PreviewFrame.Light, PreviewFrame.Medium, PreviewFrame.Strong];

        public static IReadOnlyList<PreviewFrame> AllFrames { get; } = Enum.GetValues<PreviewFrame>();

        public static IReadOnlyList<PreviewText> Texts { get; } = Enum.GetValues<PreviewText>();

        public static IReadOnlyList<PreviewSize> Sizes { get; } = Enum.GetValues<PreviewSize>();
    }
}
