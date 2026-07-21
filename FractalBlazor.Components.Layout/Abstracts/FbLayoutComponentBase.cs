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
        internal static bool UseCaching { get; set; } = true;

        private static object _locker = new object();
        private static Dictionary<int, string> _cache = new Dictionary<int, string>();

        private int _hash = 0;
        private string _style = null;

        internal static int ComputeHash(string input, int hash = 17)
        {
            foreach (char c in input)
                hash = hash * 31 + c;
            return hash;
        }

        internal unsafe static int ComputeHash(byte* input, int size, int hash = 17)
        {
            for (int i = 0; i < size; i++)
                hash = hash * 31 + input[i];
            return hash;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        private unsafe struct ComponentBaseState
        {
            public ComponentBaseState()
            {

            }

            // -------- Fields
            public bool _noWrap = false;
            public bool _noSelect = false;
            public bool _canSelect = false;
            public bool _responsiveUnder = false;
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

            public string DisplayModeString
            {
                get
                {
                    GetHashCode();
                    switch (DisplayMode)
                    {
                        case FbBaseDisplayMode.Block: return "display:block;";
                        case FbBaseDisplayMode.InlineBloc: return "display:inline-block;";
                        case FbBaseDisplayMode.Flex: return "display:flex;";
                        case FbBaseDisplayMode.InlineFlex: return "display:inline-flex;";
                        case FbBaseDisplayMode.Table: return "display:table;";
                        case FbBaseDisplayMode.TableRow: return "display:table-row;";
                        case FbBaseDisplayMode.TableCell: return "display:table-cell;";
                        case FbBaseDisplayMode.Grid: return "display:grid;";
                    }
                    return "";
                }
            }
        }

        private ComponentBaseState _state = new ComponentBaseState();

        private string _flexBasis = "";

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

        private string WrapClassString {
            get {
                if (_state._noWrap)
                    return "fb-text-no-wrap";
                return "";
            }
        }

        private string ResponsiveClassString {
            get {
                if (ResponsiveContainer)
                    return "fb-container-init";

                if (ResponsiveBreakpoint is not FbBreaks.None)
                {
                    var responsiveResultClasse = "fb-container";
                    switch (ResponsiveBreakpoint)
                    {
                        case FbBreaks.XXL: responsiveResultClasse = $"{responsiveResultClasse}-xxl"; break;
                        case FbBreaks.XL: responsiveResultClasse = $"{responsiveResultClasse}-xl"; break;
                        case FbBreaks.L: responsiveResultClasse = $"{responsiveResultClasse}-l"; break;
                        case FbBreaks.M: responsiveResultClasse = $"{responsiveResultClasse}-m"; break;
                        case FbBreaks.S: responsiveResultClasse = $"{responsiveResultClasse}-s"; break;
                        case FbBreaks.XS: responsiveResultClasse = $"{responsiveResultClasse}-xs"; break;
                    }

                    if (_state._responsiveUnder)
                        return $"{responsiveResultClasse}-none";
                    else
                        return $"{responsiveResultClasse}-flex";
                }

                return "";
            }
        }

        // ************************************************************************************************ //
        // **********************************   PROTECTED PARAMETERS   ************************************ //
        // ************************************************************************************************ //
        protected unsafe int CpntHash {
            get {
                var hash = 0;
                fixed (ComponentBaseState* sptr = &_state)
                {
                    hash = ComputeHash((byte*)sptr, sizeof(ComponentBaseState));
                    if (!string.IsNullOrWhiteSpace(FlexBasis))
                        hash = ComputeHash(FlexBasis, hash);
                    if (!string.IsNullOrWhiteSpace(Style))
                        hash = ComputeHash(Style, hash);
                    //if (Variables is not null)
                    //    hash = ComputeHash(Variables.ToCssVariables(), hash);
                    return hash;
                }
            }
        }

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
        protected string AggregatedStyles
        {
            get
            {
                var hash = CpntHash;
                if (_hash == hash && _style != null)
                    return _style;
                _hash = hash;
                lock (_locker)
                {
                    if (UseCaching && _cache.TryGetValue(hash, out var found))
                    {
                        _style = found;
                        return _style;
                    }

                    RenderingStatistics.AddComponentBaseStyleComputation();

                    var str = _state.DisplayModeString +
                            (Padding != FbSpacing.None ? $"padding:{FbLayoutHelper.ToSpacingCss(Padding)};" : "") +
                            (PaddingTop != FbSpacing.None ? $"padding-top:{FbLayoutHelper.ToSpacingCss(PaddingTop)};" : "") +
                            (PaddingBottom != FbSpacing.None ? $"padding-bottom:{FbLayoutHelper.ToSpacingCss(PaddingBottom)};" : "") +
                            (PaddingLeft != FbSpacing.None ? $"padding-left:{FbLayoutHelper.ToSpacingCss(PaddingLeft)};" : "") +
                            (PaddingRight != FbSpacing.None ? $"padding-right:{FbLayoutHelper.ToSpacingCss(PaddingRight)};" : "") +
                            (Margin != FbSpacing.None ? $"margin:{FbLayoutHelper.ToSpacingCss(Margin)};" : "") +
                            (MarginTop != FbSpacing.None ? $"margin-top:{FbLayoutHelper.ToSpacingCss(MarginTop)};" : "") +
                            (MarginBottom != FbSpacing.None ? $"margin-bottom:{FbLayoutHelper.ToSpacingCss(MarginBottom)};" : "") +
                            (MarginLeft != FbSpacing.None ? $"margin-left:{FbLayoutHelper.ToSpacingCss(MarginLeft)};" : "") +
                            (MarginRight != FbSpacing.None ? $"margin-right:{FbLayoutHelper.ToSpacingCss(MarginRight)};" : "") +
                            (_state._flex != int.MinValue ? $"flex:{_state._flex};" : "") +
                            (_state._noSelect ? $"-webkit-user-select: none;user-select: none;" : "") +
                            (_state._canSelect ? $"-webkit-user-select: text;user-select: text;" : "") +
                            (Hide ? "visibility:hidden;" : "");

                    str +=  (string.IsNullOrWhiteSpace(FlexBasis) ? "" : $"flex-basis:{FlexBasis};") +
                            (string.IsNullOrWhiteSpace(Style) ? "" : Style + ";");

                    if (UseCaching && !_cache.ContainsKey(hash))
                        _cache[hash] = str;
                    _style = str;
                    return _style;
                }
            }
        }

        // -------------------------------- LEVEL AGGREGATES -------------------------------- //
        protected string AggregatedClasses => $"{Classes} {WrapClassString} {ResponsiveClassString}".Trim();

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
        public bool Hide { get; set; } = false;

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
        /// Visible -> Under -> XS (width < 512px)
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

        /// <summary>
        /// Store identifier
        /// </summary>
        [Parameter]
        public string StoreId { get; set; } = "";
    }
}
