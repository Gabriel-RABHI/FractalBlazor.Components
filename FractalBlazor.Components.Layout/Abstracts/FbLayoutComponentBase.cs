using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using FractalBlazor.Components.Layout.Utilities;
using Microsoft.AspNetCore.Components;

namespace FractalBlazor.Components.Layout.Abstracts
{


    public abstract class FbLayoutComponentBase : FbComponentBase
    {
        #region HIDDEN
        private FbStyleMemo _styleMemo;
        private FbStyleMemo _classMemo;

        /// <summary>
        /// Class cache key of this level: every value that shapes the classes, except the
        /// free-form <c>Classes</c>.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct LayoutKey
        {
            public ComponentBaseState State;
            public bool ResponsiveContainer;
        }

        /// <summary>
        /// Every parameter of this level, except free-form strings. Its raw bytes are part of
        /// the class cache key, hence <c>Pack = 1</c>: no padding bytes.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct ComponentBaseState
        {
            public ComponentBaseState()
            {

            }

            // -------- Fields
            public bool _noWrap = false;
            public bool _noSelect = false;
            public bool _canSelect = false;
            public bool _responsiveUnder = false;
            public bool _hide = false;
            public FbBaseDisplayMode DisplayMode = FbBaseDisplayMode.None;
            public FbBreaks ResponsiveBreakpoint = FbBreaks.None;
            public FbSpacing Padding = FbSpacing.None;
            public FbSpacing PaddingTop = FbSpacing.None;
            public FbSpacing PaddingBottom = FbSpacing.None;
            public FbSpacing PaddingLeft = FbSpacing.None;
            public FbSpacing PaddingRight = FbSpacing.None;
            public FbSpacing Margin = FbSpacing.None;
            public FbSpacing MarginTop = FbSpacing.None;
            public FbSpacing MarginBottom = FbSpacing.None;
            public FbSpacing MarginLeft = FbSpacing.None;
            public FbSpacing MarginRight = FbSpacing.None;
            public int _flex = int.MinValue;

            // -------- Properties
            public bool IsBlock { get => DisplayMode == FbBaseDisplayMode.Block; set { if (value) DisplayMode = FbBaseDisplayMode.Block; } }

            public bool IsInlineBloc { get => DisplayMode == FbBaseDisplayMode.InlineBloc; set { if (value) DisplayMode = FbBaseDisplayMode.InlineBloc; } }

            public bool IsFlex { get => DisplayMode == FbBaseDisplayMode.Flex; set { if (value) DisplayMode = FbBaseDisplayMode.Flex; } }

            public bool IsInlineFlex { get => DisplayMode == FbBaseDisplayMode.InlineFlex; set { if (value) DisplayMode = FbBaseDisplayMode.InlineFlex; } }

            public bool IsGrid { get => DisplayMode == FbBaseDisplayMode.Grid; set { if (value) DisplayMode = FbBaseDisplayMode.Grid; } }
        }

        private ComponentBaseState _state = new ComponentBaseState();
        private ComponentBaseState _defaultState;

        private string _flexBasis = "";
        private string _defaultFlexBasis = "";

        protected override void CaptureParameterDefaults()
        {
            base.CaptureParameterDefaults();
            _defaultState = _state;
            _defaultFlexBasis = _flexBasis;
        }

        protected override void RestoreParameterDefaults()
        {
            base.RestoreParameterDefaults();
            _state = _defaultState;
            _flexBasis = _defaultFlexBasis;
        }

        private FbBaseDisplayMode DisplayMode { get => _state.DisplayMode; set => _state.DisplayMode = value; }

        private FbBreaks ResponsiveBreakpoint { get => _state.ResponsiveBreakpoint; set => _state.ResponsiveBreakpoint = value; }

        private FbSpacing Padding { get => _state.Padding; set => _state.Padding = value; }

        private FbSpacing PaddingTop { get => _state.PaddingTop; set => _state.PaddingTop = value; }

        private FbSpacing PaddingBottom { get => _state.PaddingBottom; set => _state.PaddingBottom = value; }

        private FbSpacing PaddingLeft { get => _state.PaddingLeft; set => _state.PaddingLeft = value; }

        private FbSpacing PaddingRight { get => _state.PaddingRight; set => _state.PaddingRight = value; }

        private FbSpacing Margin { get => _state.Margin; set => _state.Margin = value; }

        private FbSpacing MarginTop { get => _state.MarginTop; set => _state.MarginTop = value; }

        private FbSpacing MarginBottom { get => _state.MarginBottom; set => _state.MarginBottom = value; }

        private FbSpacing MarginLeft { get => _state.MarginLeft; set => _state.MarginLeft = value; }

        private FbSpacing MarginRight { get => _state.MarginRight; set => _state.MarginRight = value; }

        /// <summary>
        /// Class of <see cref="WNTW"/>, named after the flag.
        /// </summary>
        private string WrapClassString => _state._noWrap ? "fb-wntw" : "";

        /// <summary>
        /// Class of the <c>VO_*</c> and <c>VU_*</c> flags, named after the flag:
        /// <c>fb-vo-xl</c>, <c>fb-vu-xl</c>…
        /// </summary>
        private string ResponsiveClassString {
            get {
                if (ResponsiveContainer)
                    return "fb-container-init";

                var breakpoint = ResponsiveBreakpoint switch
                {
                    FbBreaks.XXL => "xxl",
                    FbBreaks.XL => "xl",
                    FbBreaks.L => "l",
                    FbBreaks.M => "m",
                    FbBreaks.S => "s",
                    FbBreaks.XS => "xs",
                    _ => null
                };
                if (breakpoint is null)
                    return "";

                return _state._responsiveUnder ? $"fb-vu-{breakpoint}" : $"fb-vo-{breakpoint}";
            }
        }

        // ************************************************************************************************ //
        // **********************************   PROTECTED PARAMETERS   ************************************ //
        // ************************************************************************************************ //
        internal LayoutKey LayoutKeyValue => new() { State = _state, ResponsiveContainer = ResponsiveContainer };

        protected bool IsBlock { get => _state.IsBlock; set => _state.IsBlock = value; }

        protected bool IsInlineBloc { get => _state.IsInlineBloc; set => _state.IsInlineBloc = value; }

        protected bool IsFlex { get => _state.IsFlex; set => _state.IsFlex = value; }

        protected bool IsInlineFlex { get => _state.IsInlineFlex; set => _state.IsInlineFlex = value; }

        protected bool IsGrid { get => _state.IsGrid; set => _state.IsGrid = value; }

        protected int Flex {
            get => _state._flex;
            set => _state._flex = value;
        }

        protected bool NoFlex {
            get => _state._flex == int.MinValue;
            set { if (value) _state._flex = int.MinValue; }
        }

        protected string FlexBasis {
            get => _flexBasis;
            set => _flexBasis = value;
        }

        /// <summary>
        /// Responsive container setting
        /// </summary>
        protected bool ResponsiveContainer { get; set; }

        // -------------------------------- LEVEL AGGREGATES -------------------------------- //
        /// <summary>
        /// Inline style of this level: only what no atom can express, <see cref="Style"/>,
        /// <see cref="FlexBasis"/> and a flex factor above <see cref="FbAtoms.MaxFlex"/>.
        /// <c>null</c> when there is none, so that no attribute is rendered.
        /// </summary>
        protected string? AggregatedStyles
            => ResolveStyle(LayoutKeyValue, new FbStyleInputs(FlexBasis, Style), static component => component.BuildLayoutStyle());

        /// <summary>
        /// Returns the style for <paramref name="key"/>: from this component's last render when
        /// nothing changed, otherwise from <paramref name="build"/>. Styles hold free-form
        /// values only, so they are never shared.
        /// </summary>
        private protected string? ResolveStyle<TKey>(in TKey key, in FbStyleInputs inputs, Func<FbLayoutComponentBase, string?> build)
            where TKey : unmanaged
        {
            if (_styleMemo.TryGet(in key, in inputs, out var style))
                return style;

            style = build(this);
            _styleMemo.Set(in key, in inputs, style);
            return style;
        }

        /// <summary>
        /// Returns the classes for <paramref name="key"/>: from this component's last render
        /// when nothing changed, otherwise from the shared cache, where <paramref name="build"/>
        /// runs once per state. <see cref="Classes"/> is appended for this component only. Each
        /// key type must always produce the same classes: a component that adds its own class
        /// uses its own key type.
        /// </summary>
        private protected string ResolveClasses<TKey>(in TKey key, Func<FbLayoutComponentBase, string> build)
            where TKey : unmanaged
        {
            var inputs = new FbStyleInputs(Classes, null);
            if (_classMemo.TryGet(in key, in inputs, out var classes))
                return classes!;

            if (!FbClassCache<TKey>.TryGet(in key, out var shared))
                shared = FbClassCache<TKey>.Add(in key, build(this));
            classes = string.IsNullOrWhiteSpace(Classes) ? shared : FbClasses.Join(shared, Classes);

            _classMemo.Set(in key, in inputs, classes);
            return classes;
        }

        private protected string? BuildLayoutStyle()
            => FbInlineStyle.Join(
                _state._flex != int.MinValue && !FbAtoms.HasFlex(_state._flex) ? $"flex:{_state._flex}" : null,
                string.IsNullOrWhiteSpace(FlexBasis) ? null : $"flex-basis:{FlexBasis}",
                Style);

        /// <summary>
        /// Adds the atoms of this level: display, padding, margin, flex factor, selection and
        /// visibility.
        /// </summary>
        private protected void AddLayoutAtoms(FbAtomLine line)
        {
            RenderingStatistics.AddComponentBaseStyleComputation();

            line.Set(FbAxis.Display, (int)DisplayMode);
            line.SetBox(FbAxis.Padding, Padding, PaddingTop, PaddingBottom, PaddingLeft, PaddingRight);
            line.SetBox(FbAxis.Margin, Margin, MarginTop, MarginBottom, MarginLeft, MarginRight);
            if (FbAtoms.HasFlex(_state._flex))
                line.Set(FbAxis.Flex, _state._flex);
            if (_state._canSelect)
                line.Set(FbAxis.UserSelect, FbAtoms.UserSelectText);
            else if (_state._noSelect)
                line.Set(FbAxis.UserSelect, FbAtoms.UserSelectNone);
            if (_state._hide)
                line.Set(FbAxis.Visibility, FbAtoms.Hidden);
        }

        // -------------------------------- LEVEL AGGREGATES -------------------------------- //
        /// <summary>
        /// Classes of this level: wrap and responsive classes, the atoms of this level, then
        /// <see cref="Classes"/>.
        /// </summary>
        protected string AggregatedClasses
            => ResolveClasses(LayoutKeyValue, static component => component.BuildAtomizedLayoutClasses());

        private string BuildAtomizedLayoutClasses()
        {
            var line = new FbAtomLine();
            AddLayoutAtoms(line);
            return line.Classes(BuildLayoutClasses(), null);
        }

        /// <summary>
        /// Named classes of this level, without atoms nor the free-form <see cref="Classes"/>.
        /// </summary>
        private protected string BuildLayoutClasses() => FbClasses.Join(WrapClassString, ResponsiveClassString);

        #endregion

        // ************************************************************************************************ //
        // **********************************    PUBLIC PARAMETERS    ************************************* //
        // ************************************************************************************************ //

        /// <summary>
        /// With -> NO -> Text Wrap : disable text wrap
        /// </summary>
        [Parameter]
        public bool WNTW
        {
            get => _state._noWrap;
            set { if (value) _state._noWrap = true; }
        }

        /// <summary>
        /// With -> Text Wrap : enable text wrap
        /// </summary>
        [Parameter]
        public bool WTW
        {
            get => !_state._noWrap;
            set { if (value) _state._noWrap = false; }
        }

        /// <summary>
        /// Disable -> Sellection : disable text selection
        /// </summary>
        [Parameter]
        public bool DS {
            get => _state._noSelect;
            set { if (value) _state._noSelect = true; }
        }

        /// <summary>
        /// Enable -> Sellection : disable text wrap
        /// </summary>
        [Parameter]
        public bool ES {
            get => _state._canSelect;
            set { if (value) _state._canSelect = true; }
        }

        /// <summary>
        /// Custom inline CSS style
        /// </summary>
        [Parameter]
        public string Style { get; set; } = "";

        /// <summary>
        /// Custom CSS classes
        /// </summary>
        [Parameter]
        public string Classes { get; set; } = "";

        /// <summary>
        /// Hide element using visibility hidden
        /// </summary>
        [Parameter]
        public bool Hide { get => _state._hide; set => _state._hide = value; }

        /// <summary>
        /// Show when initialize container width ≥ 1536px
        /// </summary>
        [Parameter]
        public bool VO_XXL { get => ResponsiveBreakpoint == FbBreaks.XXL && !_state._responsiveUnder; set { if (value) { ResponsiveBreakpoint = FbBreaks.XXL; _state._responsiveUnder = false; } } }

        /// <summary>
        /// Show when initialize container width &lt; 1536px
        /// </summary>
        [Parameter]
        public bool VU_XXL { get => ResponsiveBreakpoint == FbBreaks.XXL && _state._responsiveUnder; set { if (value) { ResponsiveBreakpoint = FbBreaks.XXL; _state._responsiveUnder = true; } } }

        /// <summary>
        /// Show when initialize container width ≥ 1280px
        /// </summary>
        [Parameter]
        public bool VO_XL { get => ResponsiveBreakpoint == FbBreaks.XL && !_state._responsiveUnder; set { if (value) { ResponsiveBreakpoint = FbBreaks.XL; _state._responsiveUnder = false; } } }

        /// <summary>
        /// Show when initialize container width &lt; 1280px
        /// </summary>
        [Parameter]
        public bool VU_XL { get => ResponsiveBreakpoint == FbBreaks.XL && _state._responsiveUnder; set { if (value) { ResponsiveBreakpoint = FbBreaks.XL; _state._responsiveUnder = true; } } }

        /// <summary>
        /// Show when initialize container width ≥ 1024px
        /// </summary>
        [Parameter]
        public bool VO_L { get => ResponsiveBreakpoint == FbBreaks.L && !_state._responsiveUnder; set { if (value) { ResponsiveBreakpoint = FbBreaks.L; _state._responsiveUnder = false; } } }

        /// <summary>
        /// Show when initialize container width &lt; 1024px
        /// </summary>
        [Parameter]
        public bool VU_L { get => ResponsiveBreakpoint == FbBreaks.L && _state._responsiveUnder; set { if (value) { ResponsiveBreakpoint = FbBreaks.L; _state._responsiveUnder = true; } } }

        /// <summary>
        /// Show when initialize container width ≥ 768px
        /// </summary>
        [Parameter]
        public bool VO_M { get => ResponsiveBreakpoint == FbBreaks.M && !_state._responsiveUnder; set { if (value) { ResponsiveBreakpoint = FbBreaks.M; _state._responsiveUnder = false; } } }

        /// <summary>
        /// Show when initialize container width &lt; 768px
        /// </summary>
        [Parameter]
        public bool VU_M { get => ResponsiveBreakpoint == FbBreaks.M && _state._responsiveUnder; set { if (value) { ResponsiveBreakpoint = FbBreaks.M; _state._responsiveUnder = true; } } }

        /// <summary>
        /// Show when initialize container width ≥ 640px
        /// </summary>
        [Parameter]
        public bool VO_S { get => ResponsiveBreakpoint == FbBreaks.S && !_state._responsiveUnder; set { if (value) { ResponsiveBreakpoint = FbBreaks.S; _state._responsiveUnder = false; } } }

        /// <summary>
        /// Show when initialize container width &lt; 640px
        /// </summary>
        [Parameter]
        public bool VU_S { get => ResponsiveBreakpoint == FbBreaks.S && _state._responsiveUnder; set { if (value) { ResponsiveBreakpoint = FbBreaks.S; _state._responsiveUnder = true; } } }

        /// <summary>
        /// Show when initialize container width ≥ 512px
        /// </summary>
        [Parameter]
        public bool VO_XS { get => ResponsiveBreakpoint == FbBreaks.XS && !_state._responsiveUnder; set { if (value) { ResponsiveBreakpoint = FbBreaks.XS; _state._responsiveUnder = false; } } }

        /// <summary>
        /// Visible -> Under -> XS (width &lt; 512px)
        /// </summary>
        [Parameter]
        public bool VU_XS { get => ResponsiveBreakpoint == FbBreaks.XS && _state._responsiveUnder; set { if (value) { ResponsiveBreakpoint = FbBreaks.XS; _state._responsiveUnder = true; } } }

        /// <summary>
        /// Padding -> Small
        /// </summary>
        [Parameter]
        public bool PS { get => Padding == FbSpacing.S; set { if (value) Padding = FbSpacing.S; } }

        /// <summary>
        /// Padding -> Medium
        /// </summary>
        [Parameter]
        public bool PM { get => Padding == FbSpacing.M; set { if (value) Padding = FbSpacing.M; } }

        /// <summary>
        /// Padding -> Large
        /// </summary>
        [Parameter]
        public bool PL { get => Padding == FbSpacing.L; set { if (value) Padding = FbSpacing.L; } }

        /// <summary>
        /// Padding -> Extra Large
        /// </summary>
        [Parameter]
        public bool PX { get => Padding == FbSpacing.X; set { if (value) Padding = FbSpacing.X; } }

        /// <summary>
        /// Padding -> Vertical -> Small 
        /// </summary>
        [Parameter]
        public bool PVS { get => PaddingTop == FbSpacing.S && PaddingBottom == FbSpacing.S; set { if (value) PaddingTop = PaddingBottom = FbSpacing.S; } }

        /// <summary>
        /// Padding -> Vertical -> Medium 
        /// </summary>
        [Parameter]
        public bool PVM { get => PaddingTop == FbSpacing.M && PaddingBottom == FbSpacing.M; set { if (value) PaddingTop = PaddingBottom = FbSpacing.M; } }

        /// <summary>
        /// Padding -> Vertical -> Large 
        /// </summary>
        [Parameter]
        public bool PVL { get => PaddingTop == FbSpacing.L && PaddingBottom == FbSpacing.L; set { if (value) PaddingTop = PaddingBottom = FbSpacing.L; } }

        /// <summary>
        /// Padding -> Vertical -> Extra Large 
        /// </summary>
        [Parameter]
        public bool PVX { get => PaddingTop == FbSpacing.X && PaddingBottom == FbSpacing.X; set { if (value) PaddingTop = PaddingBottom = FbSpacing.X; } }

        /// <summary>
        /// Padding -> Horizontal -> Small 
        /// </summary>
        [Parameter]
        public bool PHS { get => PaddingLeft == FbSpacing.S && PaddingRight == FbSpacing.S; set { if (value) PaddingLeft = PaddingRight = FbSpacing.S; } }

        /// <summary>
        /// Padding -> Horizontal -> Medium 
        /// </summary>
        [Parameter]
        public bool PHM { get => PaddingLeft == FbSpacing.M && PaddingRight == FbSpacing.M; set { if (value) PaddingLeft = PaddingRight = FbSpacing.M; } }

        /// <summary>
        /// Padding -> Horizontal -> Large 
        /// </summary>
        [Parameter]
        public bool PHL { get => PaddingLeft == FbSpacing.L && PaddingRight == FbSpacing.L; set { if (value) PaddingLeft = PaddingRight = FbSpacing.L; } }

        /// <summary>
        /// Padding -> Horizontal -> Extra Large 
        /// </summary>
        [Parameter]
        public bool PHX { get => PaddingLeft == FbSpacing.X && PaddingRight == FbSpacing.X; set { if (value) PaddingLeft = PaddingRight = FbSpacing.X; } }

        /// <summary>
        /// Padding -> Top -> Small
        /// </summary>
        [Parameter]
        public bool PTS { get => PaddingTop == FbSpacing.S; set { if (value) PaddingTop = FbSpacing.S; } }

        /// <summary>
        /// Padding -> Top -> Medium
        /// </summary>
        [Parameter]
        public bool PTM { get => PaddingTop == FbSpacing.M; set { if (value) PaddingTop = FbSpacing.M; } }

        /// <summary>
        /// Padding -> Top -> Large
        /// </summary>
        [Parameter]
        public bool PTL { get => PaddingTop == FbSpacing.L; set { if (value) PaddingTop = FbSpacing.L; } }

        /// <summary>
        /// Padding -> Top -> Extra Large
        /// </summary>
        [Parameter]
        public bool PTX { get => PaddingTop == FbSpacing.X; set { if (value) PaddingTop = FbSpacing.X; } }

        /// <summary>
        /// Padding -> Bottom -> Small
        /// </summary>
        [Parameter]
        public bool PBS { get => PaddingBottom == FbSpacing.S; set { if (value) PaddingBottom = FbSpacing.S; } }

        /// <summary>
        /// Padding -> Bottom -> Medium
        /// </summary>
        [Parameter]
        public bool PBM { get => PaddingBottom == FbSpacing.M; set { if (value) PaddingBottom = FbSpacing.M; } }

        /// <summary>
        /// Padding -> Bottom -> Large
        /// </summary>
        [Parameter]
        public bool PBL { get => PaddingBottom == FbSpacing.L; set { if (value) PaddingBottom = FbSpacing.L; } }

        /// <summary>
        /// Padding -> Bottom -> Extra Large
        /// </summary>
        [Parameter]
        public bool PBX { get => PaddingBottom == FbSpacing.X; set { if (value) PaddingBottom = FbSpacing.X; } }

        /// <summary>
        /// Padding -> Left -> Small
        /// </summary>
        [Parameter]
        public bool PLS { get => PaddingLeft == FbSpacing.S; set { if (value) PaddingLeft = FbSpacing.S; } }

        /// <summary>
        /// Padding -> Left -> Medium
        /// </summary>
        [Parameter]
        public bool PLM { get => PaddingLeft == FbSpacing.M; set { if (value) PaddingLeft = FbSpacing.M; } }

        /// <summary>
        /// Padding -> Left -> Large
        /// </summary>
        [Parameter]
        public bool PLL { get => PaddingLeft == FbSpacing.L; set { if (value) PaddingLeft = FbSpacing.L; } }

        /// <summary>
        /// Padding -> Left -> Extra Large
        /// </summary>
        [Parameter]
        public bool PLX { get => PaddingLeft == FbSpacing.X; set { if (value) PaddingLeft = FbSpacing.X; } }

        /// <summary>
        /// Padding -> Right -> Small
        /// </summary>
        [Parameter]
        public bool PRS { get => PaddingRight == FbSpacing.S; set { if (value) PaddingRight = FbSpacing.S; } }

        /// <summary>
        /// Padding -> Right -> Medium
        /// </summary>
        [Parameter]
        public bool PRM { get => PaddingRight == FbSpacing.M; set { if (value) PaddingRight = FbSpacing.M; } }

        /// <summary>
        /// Padding -> Right -> Large
        /// </summary>
        [Parameter]
        public bool PRL { get => PaddingRight == FbSpacing.L; set { if (value) PaddingRight = FbSpacing.L; } }

        /// <summary>
        /// Padding -> Right -> Extra Large
        /// </summary>
        [Parameter]
        public bool PRX { get => PaddingRight == FbSpacing.X; set { if (value) PaddingRight = FbSpacing.X; } }

        /// <summary>
        /// Margin -> Small
        /// </summary>
        [Parameter]
        public bool MS { get => Margin == FbSpacing.S; set { if (value) Margin = FbSpacing.S; } }

        /// <summary>
        /// Margin -> Medium
        /// </summary>
        [Parameter]
        public bool MM { get => Margin == FbSpacing.M; set { if (value) Margin = FbSpacing.M; } }

        /// <summary>
        /// Margin -> Large
        /// </summary>
        [Parameter]
        public bool ML { get => Margin == FbSpacing.L; set { if (value) Margin = FbSpacing.L; } }

        /// <summary>
        /// Margin -> Extra Large
        /// </summary>
        [Parameter]
        public bool MX { get => Margin == FbSpacing.X; set { if (value) Margin = FbSpacing.X; } }

        /// <summary>
        /// Margin -> Vertical -> Small 
        /// </summary>
        [Parameter]
        public bool MVS { get => MarginTop == FbSpacing.S && MarginBottom == FbSpacing.S; set { if (value) MarginTop = MarginBottom = FbSpacing.S; } }

        /// <summary>
        /// Margin -> Vertical -> Medium 
        /// </summary>
        [Parameter]
        public bool MVM { get => MarginTop == FbSpacing.M && MarginBottom == FbSpacing.M; set { if (value) MarginTop = MarginBottom = FbSpacing.M; } }

        /// <summary>
        /// Margin -> Vertical -> Large 
        /// </summary>
        [Parameter]
        public bool MVL { get => MarginTop == FbSpacing.L && MarginBottom == FbSpacing.L; set { if (value) MarginTop = MarginBottom = FbSpacing.L; } }

        /// <summary>
        /// Margin -> Vertical -> Extra Large 
        /// </summary>
        [Parameter]
        public bool MVX { get => MarginTop == FbSpacing.X && MarginBottom == FbSpacing.X; set { if (value) MarginTop = MarginBottom = FbSpacing.X; } }

        /// <summary>
        /// Margin -> Horizontal -> Small 
        /// </summary>
        [Parameter]
        public bool MHS { get => MarginLeft == FbSpacing.S && MarginRight == FbSpacing.S; set { if (value) MarginLeft = MarginRight = FbSpacing.S; } }

        /// <summary>
        /// Margin -> Horizontal -> Medium 
        /// </summary>
        [Parameter]
        public bool MHM { get => MarginLeft == FbSpacing.M && MarginRight == FbSpacing.M; set { if (value) MarginLeft = MarginRight = FbSpacing.M; } }

        /// <summary>
        /// Margin -> Horizontal -> Large 
        /// </summary>
        [Parameter]
        public bool MHL { get => MarginLeft == FbSpacing.L && MarginRight == FbSpacing.L; set { if (value) MarginLeft = MarginRight = FbSpacing.L; } }

        /// <summary>
        /// Margin -> Horizontal -> Extra Large 
        /// </summary>
        [Parameter]
        public bool MHX { get => MarginLeft == FbSpacing.X && MarginRight == FbSpacing.X; set { if (value) MarginLeft = MarginRight = FbSpacing.X; } }

        /// <summary>
        /// Margin -> Top -> Small
        /// </summary>
        [Parameter]
        public bool MTS { get => MarginTop == FbSpacing.S; set { if (value) MarginTop = FbSpacing.S; } }

        /// <summary>
        /// Margin -> Top -> Medium
        /// </summary>
        [Parameter]
        public bool MTM { get => MarginTop == FbSpacing.M; set { if (value) MarginTop = FbSpacing.M; } }

        /// <summary>
        /// Margin -> Top -> Large
        /// </summary>
        [Parameter]
        public bool MTL { get => MarginTop == FbSpacing.L; set { if (value) MarginTop = FbSpacing.L; } }

        /// <summary>
        /// Margin -> Top -> Extra Large
        /// </summary>
        [Parameter]
        public bool MTX { get => MarginTop == FbSpacing.X; set { if (value) MarginTop = FbSpacing.X; } }

        /// <summary>
        /// Margin -> Bottom -> Small
        /// </summary>
        [Parameter]
        public bool MBS { get => MarginBottom == FbSpacing.S; set { if (value) MarginBottom = FbSpacing.S; } }

        /// <summary>
        /// Margin -> Bottom -> Medium
        /// </summary>
        [Parameter]
        public bool MBM { get => MarginBottom == FbSpacing.M; set { if (value) MarginBottom = FbSpacing.M; } }

        /// <summary>
        /// Margin -> Bottom -> Large
        /// </summary>
        [Parameter]
        public bool MBL { get => MarginBottom == FbSpacing.L; set { if (value) MarginBottom = FbSpacing.L; } }

        /// <summary>
        /// Margin -> Bottom -> Extra Large
        /// </summary>
        [Parameter]
        public bool MBX { get => MarginBottom == FbSpacing.X; set { if (value) MarginBottom = FbSpacing.X; } }

        /// <summary>
        /// Margin -> Left -> Small
        /// </summary>
        [Parameter]
        public bool MLS { get => MarginLeft == FbSpacing.S; set { if (value) MarginLeft = FbSpacing.S; } }

        /// <summary>
        /// Margin -> Left -> Medium
        /// </summary>
        [Parameter]
        public bool MLM { get => MarginLeft == FbSpacing.M; set { if (value) MarginLeft = FbSpacing.M; } }

        /// <summary>
        /// Margin -> Left -> Large
        /// </summary>
        [Parameter]
        public bool MLL { get => MarginLeft == FbSpacing.L; set { if (value) MarginLeft = FbSpacing.L; } }

        /// <summary>
        /// Margin -> Left -> Extra Large
        /// </summary>
        [Parameter]
        public bool MLX { get => MarginLeft == FbSpacing.X; set { if (value) MarginLeft = FbSpacing.X; } }

        /// <summary>
        /// Margin -> Right -> Small
        /// </summary>
        [Parameter]
        public bool MRS { get => MarginRight == FbSpacing.S; set { if (value) MarginRight = FbSpacing.S; } }

        /// <summary>
        /// Margin -> Right -> Medium
        /// </summary>
        [Parameter]
        public bool MRM { get => MarginRight == FbSpacing.M; set { if (value) MarginRight = FbSpacing.M; } }

        /// <summary>
        /// Margin -> Right -> Large
        /// </summary>
        [Parameter]
        public bool MRL { get => MarginRight == FbSpacing.L; set { if (value) MarginRight = FbSpacing.L; } }

        /// <summary>
        /// Margin -> Right -> Extra Large
        /// </summary>
        [Parameter]
        public bool MRX { get => MarginRight == FbSpacing.X; set { if (value) MarginRight = FbSpacing.X; } }
    }
}
