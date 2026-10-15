using FractalBlazor.Components.Layout.Utilities;
using Microsoft.AspNetCore.Components;

namespace FractalBlazor.Components.Layout.Abstracts
{
    public abstract class FbLayoutSurfaceComponentBase : FbLayoutComponentBase
    {
        private FbFrame _frame = FbFrame.None;
        private FbBackground _background = FbBackground.None;
        private FbSpacing _radius = FbSpacing.None;
        private bool _round;
        private bool _outline;
        private FbFrame _frameTop, _frameRight, _frameBottom, _frameLeft;
        private bool _frameDot;
        private (FbFrame Frame, FbBackground Background, FbSpacing Radius, bool Round, bool Outline,
            FbFrame Top, FbFrame Right, FbFrame Bottom, FbFrame Left, bool Dot) _defaults;

        protected override void CaptureParameterDefaults()
        {
            base.CaptureParameterDefaults();
            _defaults = (_frame, _background, _radius, _round, _outline, _frameTop, _frameRight, _frameBottom, _frameLeft, _frameDot);
        }

        protected override void RestoreParameterDefaults()
        {
            base.RestoreParameterDefaults();
            (_frame, _background, _radius, _round, _outline, _frameTop, _frameRight, _frameBottom, _frameLeft, _frameDot) = _defaults;
        }

        private string HoverClassString {
            get {
                if (Hover || !string.IsNullOrWhiteSpace(HoverMixOffset))
                    return "fb-hover";
                return "";
            }
        }

        private string OutlineClassString => EO ? "fb-eo" : "";

        /// <summary>
        /// A radius from the scale; setting one leaves the round corners of <see cref="WRR"/>.
        /// </summary>
        private FbSpacing Radius { get => _radius; set { _radius = value; _round = false; } }

        private string FrameClass => _frame switch
        {
            FbFrame.Light => "fb-wfl",
            FbFrame.Medium => "fb-wfm",
            FbFrame.Strong => "fb-wfs",
            _ => ""
        };

        // Classes of the side frames, indexed by strength: none, light, medium, strong.
        private static readonly string[] TopFrameClasses = ["", "fb-wftl", "fb-wftm", "fb-wfts"];
        private static readonly string[] RightFrameClasses = ["", "fb-wfrl", "fb-wfrm", "fb-wfrs"];
        private static readonly string[] BottomFrameClasses = ["", "fb-wfbl", "fb-wfbm", "fb-wfbs"];
        private static readonly string[] LeftFrameClasses = ["", "fb-wfll", "fb-wflm", "fb-wfls"];

        /// <summary>
        /// The frame, the side frames and their dotted style. Built only when the class cache
        /// misses.
        /// </summary>
        private string FrameClasses
            => FbClasses.Join(
                FrameClass,
                TopFrameClasses[(int)_frameTop],
                RightFrameClasses[(int)_frameRight],
                BottomFrameClasses[(int)_frameBottom],
                LeftFrameClasses[(int)_frameLeft],
                _frameDot ? "fb-wfd" : null);

        private string? RadiusClass => _round ? "fb-wrr" : null;

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
            public FbFrame FrameTop;
            public FbFrame FrameRight;
            public FbFrame FrameBottom;
            public FbFrame FrameLeft;
            public bool FrameDot;
            public bool Round;
            public bool Hover;
            public bool Outline;
        }

        internal SurfaceKey SurfaceKeyValue => new()
        {
            Layout = LayoutKeyValue,
            Radius = _radius,
            Background = _background,
            Frame = _frame,
            FrameTop = _frameTop,
            FrameRight = _frameRight,
            FrameBottom = _frameBottom,
            FrameLeft = _frameLeft,
            FrameDot = _frameDot,
            Round = _round,
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
            => FbClasses.Join(BuildLayoutClasses(), BackgroundClasses, FrameClasses, RadiusClass, HoverClassString, OutlineClassString);

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
        /// With -> Frame -> Dot: the frame and the side frames in dots.
        /// </summary>
        [Parameter]
        public bool WFD { get => _frameDot; set { if (value) _frameDot = true; } }

        /// <summary>
        /// With -> Frame -> Top -> Light: a line on the top side.
        /// </summary>
        [Parameter]
        public bool WFTL { get => _frameTop == FbFrame.Light; set { if (value) _frameTop = FbFrame.Light; } }

        /// <summary>
        /// With -> Frame -> Top -> Medium: a line on the top side.
        /// </summary>
        [Parameter]
        public bool WFTM { get => _frameTop == FbFrame.Medium; set { if (value) _frameTop = FbFrame.Medium; } }

        /// <summary>
        /// With -> Frame -> Top -> Strong: a line on the top side.
        /// </summary>
        [Parameter]
        public bool WFTS { get => _frameTop == FbFrame.Strong; set { if (value) _frameTop = FbFrame.Strong; } }

        /// <summary>
        /// With -> Frame -> Right -> Light: a line on the right side.
        /// </summary>
        [Parameter]
        public bool WFRL { get => _frameRight == FbFrame.Light; set { if (value) _frameRight = FbFrame.Light; } }

        /// <summary>
        /// With -> Frame -> Right -> Medium: a line on the right side.
        /// </summary>
        [Parameter]
        public bool WFRM { get => _frameRight == FbFrame.Medium; set { if (value) _frameRight = FbFrame.Medium; } }

        /// <summary>
        /// With -> Frame -> Right -> Strong: a line on the right side.
        /// </summary>
        [Parameter]
        public bool WFRS { get => _frameRight == FbFrame.Strong; set { if (value) _frameRight = FbFrame.Strong; } }

        /// <summary>
        /// With -> Frame -> Bottom -> Light: a line on the bottom side.
        /// </summary>
        [Parameter]
        public bool WFBL { get => _frameBottom == FbFrame.Light; set { if (value) _frameBottom = FbFrame.Light; } }

        /// <summary>
        /// With -> Frame -> Bottom -> Medium: a line on the bottom side.
        /// </summary>
        [Parameter]
        public bool WFBM { get => _frameBottom == FbFrame.Medium; set { if (value) _frameBottom = FbFrame.Medium; } }

        /// <summary>
        /// With -> Frame -> Bottom -> Strong: a line on the bottom side.
        /// </summary>
        [Parameter]
        public bool WFBS { get => _frameBottom == FbFrame.Strong; set { if (value) _frameBottom = FbFrame.Strong; } }

        /// <summary>
        /// With -> Frame -> Left -> Light: a line on the left side.
        /// </summary>
        [Parameter]
        public bool WFLL { get => _frameLeft == FbFrame.Light; set { if (value) _frameLeft = FbFrame.Light; } }

        /// <summary>
        /// With -> Frame -> Left -> Medium: a line on the left side.
        /// </summary>
        [Parameter]
        public bool WFLM { get => _frameLeft == FbFrame.Medium; set { if (value) _frameLeft = FbFrame.Medium; } }

        /// <summary>
        /// With -> Frame -> Left -> Strong: a line on the left side.
        /// </summary>
        [Parameter]
        public bool WFLS { get => _frameLeft == FbFrame.Strong; set { if (value) _frameLeft = FbFrame.Strong; } }

        /// <summary>
        /// Enable -> Outline
        /// </summary>
        [Parameter]
        public bool EO { get => _outline; set { if (value) _outline = true; } }

        /// <summary>
        /// With -> Radius -> None: square corners, even when the component defaults round them.
        /// </summary>
        [Parameter]
        public bool WRN { get => Radius == FbSpacing.None && !_round; set { if (value) Radius = FbSpacing.None; } }

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
        /// With -> Radius -> Round: a radius of 50 %, a circle on a square box.
        /// </summary>
        [Parameter]
        public bool WRR { get => _round; set { if (value) { _radius = FbSpacing.None; _round = true; } } }

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
