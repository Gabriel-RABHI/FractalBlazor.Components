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
        /// Bumped when the shape changes. Format 2 is the three-anchor gradient; a format 1
        /// session still gives back what kept its meaning (see <c>ThemeEditorModel.Restore</c>).
        /// </summary>
        public const int CurrentFormat = 2;

        public const int PreviousFormat = 1;
    }

    /// <summary>
    /// The values of one branch. A save without a master tint reads <c>null</c> and keeps the
    /// tint of the default theme.
    /// </summary>
    internal sealed record BranchSnapshot(
        double NeutralMix, double SurfaceOffset, double AccentOffset, double HighlightOffset, double HoverOffset,
        double FrameLight, double FrameMedium, double FrameStrong,
        double TextSubtle, double TextMuted, double TextDefault, double TextHighlight,
        Dictionary<string, AnchorSnapshot> Anchors,
        string? TintColor = null, double? TintMix = null);

    internal sealed record AnchorSnapshot(string? Low, string? Mid, string? High);

    /// <summary>
    /// Generated serialization: no reflection, safe once the WebAssembly host is trimmed.
    /// </summary>
    [JsonSerializable(typeof(ThemeEditorSnapshot))]
    internal sealed partial class ThemeEditorJson : JsonSerializerContext
    {
    }
}
