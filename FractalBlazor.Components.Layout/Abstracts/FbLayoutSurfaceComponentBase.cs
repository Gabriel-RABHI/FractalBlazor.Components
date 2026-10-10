using FractalBlazor.Components.Layout.Utilities;
using Microsoft.AspNetCore.Components;

namespace FractalBlazor.Components.Layout.Abstracts
{
    public abstract class FbLayoutSurfaceComponentBase : FbLayoutComponentBase
    {
        private FbFrame _frame = FbFrame.None;
        private FbBackground _background = FbBackground.None;
        private FbSpacing _radius = FbSpacing.None;
        private bool _outline;
        private (FbFrame Frame, FbBackground Background, FbSpacing Radius, bool Outline) _defaults;

        protected override void CaptureParameterDefaults()
        {
            base.CaptureParameterDefaults();
            _defaults = (_frame, _background, _radius, _outline);
        }

        protected override void RestoreParameterDefaults()
        {
            base.RestoreParameterDefaults();
            (_frame, _background, _radius, _outline) = _defaults;
        }

        private string HoverClassString {
            get {
                if (Hover || !string.IsNullOrWhiteSpace(HoverMixOffset))
                    return "fb-hover";
                return "";
            }
        }

        private string OutlineClassString => EOutline ? "fb-outline" : "";

        private FbSpacing Radius { get => _radius; set => _radius = value; }

        private string FrameClass {
            get {
                string baseClasse = "fb-framed";

                if (_frame is FbFrame.Light)
                    return $"{baseClasse} {baseClasse}-light";
                else if (_frame is FbFrame.Medium)
                    return $"{baseClasse} {baseClasse}-medium";
                else if (_frame is FbFrame.Strong)
                    return $"{baseClasse} {baseClasse}-strong";
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
        protected new string AggregatedStyles
            => ResolveStyle(SurfaceStyleKey, SurfaceStyleInputs, static component => ((FbLayoutSurfaceComponentBase)component).BuildSurfaceStyle());

        /// <summary>
        /// Style cache key of this level: the layout key plus the radius.
        /// </summary>
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1)]
        internal struct SurfaceKey
        {
            public ComponentBaseState Layout;
            public FbSpacing Radius;
        }

        internal SurfaceKey SurfaceStyleKey => new() { Layout = LayoutStyleKey, Radius = _radius };

        private protected FbStyleInputs SurfaceStyleInputs => new(FlexBasis, Style, HoverMixOffset);

        private protected string BuildSurfaceStyle()
            => BuildLayoutStyle() +
               (string.IsNullOrWhiteSpace(HoverMixOffset) ? "" : $"--fb-hover-mix-offset:{HoverMixOffset};") +
               (Radius != FbSpacing.None ? $"border-radius:{FbLayoutHelper.ToRadiusCss(Radius)};" : "");

        // -------------------------------- LEVEL AGGREGATES -------------------------------- //
        protected new string AggregatedClasses => $"{base.AggregatedClasses} {BackgroundClasses} {ComputedFrameClasses} {HoverClassString} {OutlineClassString}".Trim();

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
        /// Enable -> Outline
        /// </summary>
        [Parameter]
        public bool EOutline { get => _outline; set { if (value) _outline = true; } }

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
