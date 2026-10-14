using System.Text.Json.Serialization;

namespace FractalBlazor.WebSite.ThemeEditor
{
    /// <summary>
    /// Everything the theme editor needs to come back as it was left: the values of the theme,
    /// those of each edited branch, and what was open. Saved as JSON in the browser.
    /// </summary>
    internal sealed record ThemeEditorSnapshot(
        int Format,
        string Branch,
        string SelectedVariant,
        int EditTab,
        double SpaceS, double SpaceM, double SpaceL, double SpaceX,
        double RadiusS, double RadiusM, double RadiusL, double RadiusX,
        string TextFontFamily, string CodeFontFamily, double FontSize, double LineHeight,
        double CoefXS, double CoefS, double CoefM, double CoefL, double CoefXL, double CoefXXL,
        double WeightThin, double WeightDefault, double WeightBold, double WeightExtraBold,
        Dictionary<string, BranchSnapshot> Branches)
    {
        /// <summary>
        /// Bumped when the shape changes: an older session is then ignored, never misread.
        /// </summary>
        public const int CurrentFormat = 1;
    }

    /// <summary>
    /// The values of one branch. The master tint came later: a save without it reads
    /// <c>null</c> and keeps the tint of the default theme.
    /// </summary>
    internal sealed record BranchSnapshot(
        double SurfaceMix, double AccentOffset, double HighlightOffset, double HoverOffset,
        double FrameLight, double FrameMedium, double FrameStrong,
        double TextSubtle, double TextMuted, double TextDefault, double TextHighlight,
        Dictionary<string, AnchorSnapshot> Anchors,
        string? TintColor = null, double? TintMix = null);

    internal sealed record AnchorSnapshot(
        string BgLow, string BgHigh, string FgLow, string FgHigh, string? FrameLow, string? FrameHigh);

    /// <summary>
    /// Generated serialization: no reflection, safe once the WebAssembly host is trimmed.
    /// </summary>
    [JsonSerializable(typeof(ThemeEditorSnapshot))]
    internal sealed partial class ThemeEditorJson : JsonSerializerContext
    {
    }
}
