using Microsoft.AspNetCore.Components;

namespace FractalBlazor.Components.Layout
{
    public abstract class FbLayoutVisibleComponentBase : FbLayoutComponentBase
    {
        private FbFrame _frame = FbFrame.None;
        private FbBackground _background = FbBackground.None;
        public FbSpacing _radius = FbSpacing.None;

        private string HoverClassString {
            get {
                if (Hover || !string.IsNullOrWhiteSpace(HoverMixOffset))
                    return "fb-hover";
                return "";
            }
        }

        private FbSpacing Radius { get => _radius; set => _radius = value; }

        private string FrameClass {
            get {
                string baseClasse = "framed-stack";

                if (_frame is FbFrame.Light)
                    return $"{baseClasse} {baseClasse}-light-frame";
                else if (_frame is FbFrame.Medium)
                    return $"{baseClasse} {baseClasse}-medium-frame";
                else if (_frame is FbFrame.Strong)
                    return $"{baseClasse} {baseClasse}-strong-frame";
                else
                    return "";
            }
        }

        private string ComputedFrameClasses {
            get {
                if (FrameClass != "")
                    return $"{FrameClass}";
                return "";
            }
        }

        private string BackgroundClasses {
            get {
                switch (_background)
                {
                    case FbBackground.Surface:
                        return "fb-bg-surface";
                    case FbBackground.Accent:
                        return "fb-bg-accent";
                    case FbBackground.Highlight:
                        return "fb-bg-highlight";
                }
                return "";
            }
        }

        // ************************************************************************************************ //
        // **********************************   PROTECTED PARAMETERS   ************************************ //
        // ************************************************************************************************ //

        // -------------------------------- LEVEL AGGREGATES -------------------------------- //
        protected string AggregatedStyles {
            get {
                return base.AggregatedStyles + (string.IsNullOrWhiteSpace(HoverMixOffset) ? "" : $"--fb-hover-mix-offset:{HoverMixOffset};") +
                       (Radius != FbSpacing.None ? $"border-radius:{FbLayoutHelper.ToRadiusCss(Radius)};" : "");
            }
        }

        // -------------------------------- LEVEL AGGREGATES -------------------------------- //
        protected string AggregatedClasses => $"{base.AggregatedClasses} {BackgroundClasses} {ComputedFrameClasses} {HoverClassString}".Trim();

        // ************************************************************************************************ //
        // **********************************    PUBLIC PARAMETERS    ************************************* //
        // ************************************************************************************************ //

        /// <summary>
        /// Enable CSS-only hover color offset.
        /// </summary>
        [Parameter]
        public bool Hover { get; set; }

        /// <summary>
        /// Offset added to background, frame and foreground mixes on hover.
        /// </summary>
        [Parameter]
        public string HoverMixOffset { get; set; } = "";

        /// <summary>
        /// With -> Frame -> Light
        /// </summary>
        [Parameter]
        public bool WFL { get => _frame == FbFrame.Light; set { if (value) _frame = FbFrame.Light; } }

        /// <summary>
        /// With -> Frame -> Medium
        /// </summary>
        [Parameter]
        public bool WFM { get => _frame == FbFrame.Medium; set { if (value) _frame = FbFrame.Medium; } }

        /// <summary>
        /// With -> Frame -> Strong
        /// </summary>
        [Parameter]
        public bool WFS { get => _frame == FbFrame.Strong; set { if (value) _frame = FbFrame.Strong; } }

        /// <summary>
        /// With -> Radius -> Small
        /// </summary>
        [Parameter]
        public bool WRS { get => Radius == FbSpacing.S; set { if (value) Radius = FbSpacing.S; } }

        /// <summary>
        /// With -> Radius -> Medium
        /// </summary>
        [Parameter]
        public bool WRM { get => Radius == FbSpacing.M; set { if (value) Radius = FbSpacing.M; } }

        /// <summary>
        /// With -> Radius -> Large
        /// </summary>
        [Parameter]
        public bool WRL { get => Radius == FbSpacing.L; set { if (value) Radius = FbSpacing.L; } }

        /// <summary>
        /// With -> Radius -> Extra Large
        /// </summary>
        [Parameter]
        public bool WRX { get => Radius == FbSpacing.X; set { if (value) Radius = FbSpacing.X; } }

        /// <summary>
        /// With -> Background -> Surface
        /// </summary>
        [Parameter]
        public bool WBS { get => _background == FbBackground.Surface; set { if (value) _background = FbBackground.Surface; } }

        /// <summary>
        /// With -> Background -> Accent
        /// </summary>
        [Parameter]
        public bool WBA { get => _background == FbBackground.Accent; set { if (value) _background = FbBackground.Accent; } }

        /// <summary>
        /// With -> Background -> Highlight
        /// </summary>
        [Parameter]
        public bool WBH { get => _background == FbBackground.Highlight; set { if (value) _background = FbBackground.Highlight; } }
    }
}
