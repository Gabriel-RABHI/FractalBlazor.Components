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

        private string OutlineClassString => EOutline ? "fb-eoutline" : "";

        private FbSpacing Radius { get => _radius; set => _radius = value; }

        private string FrameClass => _frame switch
        {
            FbFrame.Light => "fb-wfl",
            FbFrame.Medium => "fb-wfm",
            FbFrame.Strong => "fb-wfs",
            _ => ""
        };

        private string BackgroundClasses {
            get {
                switch (_background)
                {
                    case FbBackground.Surface:
                        return "fb-wbs";
                    case FbBackground.Accent:
                        return "fb-wba";
                    case FbBackground.Highlight:
                        return "fb-wbh";
                }
                return "";
            }
        }

        // ************************************************************************************************ //
        // **********************************   PROTECTED PARAMETERS   ************************************ //
        // ************************************************************************************************ //

        // -------------------------------- LEVEL AGGREGATES -------------------------------- //
        /// <summary>
        /// Inline style of this level: the layout one plus a custom hover offset.
        /// </summary>
        protected new string? AggregatedStyles
            => ResolveStyle(SurfaceKeyValue, SurfaceStyleInputs, static component => ((FbLayoutSurfaceComponentBase)component).BuildSurfaceStyle());

        /// <summary>
        /// Class cache key of this level: the layout key plus radius, fill, frame, hover and
        /// outline.
        /// </summary>
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1)]
        internal struct SurfaceKey
        {
            public LayoutKey Layout;
            public FbSpacing Radius;
            public FbBackground Background;
            public FbFrame Frame;
            public bool Hover;
            public bool Outline;
        }

        internal SurfaceKey SurfaceKeyValue => new()
        {
            Layout = LayoutKeyValue,
            Radius = _radius,
            Background = _background,
            Frame = _frame,
            Hover = Hover || !string.IsNullOrWhiteSpace(HoverMixOffset),
            Outline = _outline
        };

        private protected FbStyleInputs SurfaceStyleInputs => new(FlexBasis, Style, HoverMixOffset);

        private protected string? BuildSurfaceStyle()
            => FbInlineStyle.Join(
                BuildLayoutStyle(),
                string.IsNullOrWhiteSpace(HoverMixOffset) ? null : $"--fb-hover-mix-offset:{HoverMixOffset}");

        /// <summary>
        /// Adds the atoms of this level and of the levels below: the radius.
        /// </summary>
        private protected void AddSurfaceAtoms(FbAtomLine line)
        {
            AddLayoutAtoms(line);
            line.Set(FbAxis.Radius, (int)Radius);
        }

        // -------------------------------- LEVEL AGGREGATES -------------------------------- //
        /// <summary>
        /// Classes of this level: the named classes, fill, frame, hover and outline, then the
        /// atoms of this level and of the levels below.
        /// </summary>
        protected new string AggregatedClasses
            => ResolveClasses(SurfaceKeyValue, static component => ((FbLayoutSurfaceComponentBase)component).BuildAtomizedSurfaceClasses());

        private string BuildAtomizedSurfaceClasses()
        {
            var line = new FbAtomLine();
            AddSurfaceAtoms(line);
            return line.Classes(BuildSurfaceClasses(), null);
        }

        /// <summary>
        /// Named classes of this level and of the levels below, without atoms.
        /// </summary>
        private protected string BuildSurfaceClasses()
            => FbClasses.Join(BuildLayoutClasses(), BackgroundClasses, FrameClass, HoverClassString, OutlineClassString);

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
        /// With -> Frame -> None: removes a frame set by the component defaults.
        /// </summary>
        [Parameter]
        public bool WFN { get => _frame == FbFrame.None; set { if (value) _frame = FbFrame.None; } }

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
        /// With -> Radius -> None: square corners, even when the component defaults round them.
        /// </summary>
        [Parameter]
        public bool WRN { get => Radius == FbSpacing.None; set { if (value) Radius = FbSpacing.None; } }

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
        /// With -> Background -> None: transparent, even when the component defaults paint a fill.
        /// </summary>
        [Parameter]
        public bool WBN { get => _background == FbBackground.None; set { if (value) _background = FbBackground.None; } }

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
