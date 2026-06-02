using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace FractalBlazor.Components.Layout
{
    public abstract class FbLayoutComponentBase : FbComponentBase
    {
        internal static bool UseCaching { get; set; } = true;

        public static int ComputeHash(string input, int hash = 17)
        {
            foreach (char c in input)
                hash = hash * 31 + c;
            return hash;
        }

        public unsafe static int ComputeHash(byte* input, int size, int hash = 17)
        {
            for (int i = 0; i < size; i++)
                hash = hash * 31 + input[i];
            return hash;
        }

        private static object _locker = new object();
        private static Dictionary<int, string> _cache = new Dictionary<int, string>();
        private int _hash = 0;
        private string _style = null;

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        private unsafe struct ComponentBaseState
        {
            public ComponentBaseState()
            {

            }

            // -------- Fields
            public bool _noWrap = false;
            public bool _responsiveUnder = false;
            public BaseDisplayMode DisplayMode = BaseDisplayMode.None;
            public FbSpacing Padding = FbSpacing.None;
            public FbResponsiveBreakpoint ResponsiveBreakpoint = FbResponsiveBreakpoint.None;
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
            public bool IsBlock { get => DisplayMode == BaseDisplayMode.Block; set => DisplayMode = BaseDisplayMode.Block; }

            public bool IsInlineBloc { get => DisplayMode == BaseDisplayMode.InlineBloc; set => DisplayMode = BaseDisplayMode.InlineBloc; }

            public bool IsFlex { get => DisplayMode == BaseDisplayMode.Flex; set => DisplayMode = BaseDisplayMode.Flex; }

            public bool IsInlineFlex { get => DisplayMode == BaseDisplayMode.InlineFlex; set => DisplayMode = BaseDisplayMode.InlineFlex; }

            public string DisplayModeString
            {
                get
                {
                    GetHashCode();
                    switch (DisplayMode)
                    {
                        case BaseDisplayMode.Block: return "display:block;";
                        case BaseDisplayMode.InlineBloc: return "display:inline-block;";
                        case BaseDisplayMode.Flex: return "display:flex;";
                        case BaseDisplayMode.InlineFlex: return "display:inline-flex;";
                        case BaseDisplayMode.Table: return "display:table;";
                        case BaseDisplayMode.TableRow: return "display:table-row;";
                        case BaseDisplayMode.TableCell: return "display:table-cell;";
                    }
                    return "";
                }
            }
        }

        private ComponentBaseState _state = new ComponentBaseState();

        private string _widthBasis = "";

        protected BaseDisplayMode DisplayMode { get => _state.DisplayMode; set => _state.DisplayMode = value; }

        protected FbResponsiveBreakpoint ResponsiveBreakpoint { get => _state.ResponsiveBreakpoint; set => _state.ResponsiveBreakpoint = value; }

        protected FbSpacing Padding { get => _state.Padding; set => _state.Padding = value; }

        protected FbSpacing PaddingTop { get => _state.PaddingTop; set => _state.PaddingTop = value; }

        protected FbSpacing PaddingBottom { get => _state.PaddingBottom; set => _state.PaddingBottom = value; }

        protected FbSpacing PaddingLeft { get => _state.PaddingLeft; set => _state.PaddingLeft = value; }

        protected FbSpacing PaddingRight { get => _state.PaddingRight; set => _state.PaddingRight = value; }

        protected FbSpacing Margin { get => _state.Margin; set => _state.Margin = value; }

        protected FbSpacing MarginTop { get => _state.MarginTop; set => _state.MarginTop = value; }

        protected FbSpacing MarginBottom { get => _state.MarginBottom; set => _state.MarginBottom = value; }

        protected FbSpacing MarginLeft { get => _state.MarginLeft; set => _state.MarginLeft = value; }

        protected FbSpacing MarginRight { get => _state.MarginRight; set => _state.MarginRight = value; }

        protected bool IsBlock { get => _state.IsBlock; set => _state.IsBlock = value; }

        protected bool IsInlineBloc { get => _state.IsInlineBloc; set => _state.IsInlineBloc = value; }

        protected bool IsFlex { get => _state.IsFlex; set => _state.IsFlex = value; }

        protected bool IsInlineFlex { get => _state.IsInlineFlex; set => _state.IsInlineFlex = value; }

        protected unsafe int CpntHash
        {
            get
            {
                var hash = 0;
                fixed (ComponentBaseState* sptr = &_state)
                {
                    hash = ComputeHash((byte*)sptr, sizeof(ComponentBaseState));
                    if (!string.IsNullOrWhiteSpace(WidthBasis))
                        hash = ComputeHash(WidthBasis, hash);
                    if (!string.IsNullOrWhiteSpace(Style))
                        hash = ComputeHash(Style, hash);
                    return hash;
                }
            }
        }

        protected unsafe string ComputedBaseStyle
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
                            (Padding != FbSpacing.None ? $"padding:{FbLayoutPresets.ToRem(Padding)};" : "") +
                            (PaddingTop != FbSpacing.None ? $"padding-top:{FbLayoutPresets.ToRem(PaddingTop)};" : "") +
                            (PaddingBottom != FbSpacing.None ? $"padding-bottom:{FbLayoutPresets.ToRem(PaddingBottom)};" : "") +
                            (PaddingLeft != FbSpacing.None ? $"padding-left:{FbLayoutPresets.ToRem(PaddingLeft)};" : "") +
                            (PaddingRight != FbSpacing.None ? $"padding-right:{FbLayoutPresets.ToRem(PaddingRight)};" : "") +
                            (Margin != FbSpacing.None ? $"margin:{FbLayoutPresets.ToRem(Margin)};" : "") +
                            (MarginTop != FbSpacing.None ? $"margin-top:{FbLayoutPresets.ToRem(MarginTop)};" : "") +
                            (MarginBottom != FbSpacing.None ? $"margin-bottom:{FbLayoutPresets.ToRem(MarginBottom)};" : "") +
                            (MarginLeft != FbSpacing.None ? $"margin-left:{FbLayoutPresets.ToRem(MarginLeft)};" : "") +
                            (MarginRight != FbSpacing.None ? $"margin-left:{FbLayoutPresets.ToRem(MarginRight)};" : "") +
                            (_state._flex != int.MinValue ? $"flex:{_state._flex};" : "") +
                            (Hidden ? "visibility:hidden;" : "");
                    str += (string.IsNullOrWhiteSpace(WidthBasis) ? "" : $"flex-basis:{WidthBasis};") +
                            (string.IsNullOrWhiteSpace(Style) ? "" : Style + ";");
                    if (UseCaching && !_cache.ContainsKey(hash))
                        _cache[hash] = str;
                    _style = str;
                    return _style;
                }
            }
        }

        public void UpdateState()
        {
            InvokeAsync(() => { StateHasChanged(); });
        }

        // ************************************************************************************************ //
        // **********************************    PUBLIC PARAMETERS    ************************************* //
        // ************************************************************************************************ //

        /// <summary>
        /// Flex grow factor
        /// </summary>
        [Parameter]
        public int Flex
        {
            get => _state._flex;
            set
            {
                if (DisplayMode != BaseDisplayMode.Flex && DisplayMode != BaseDisplayMode.InlineFlex)
                    IsFlex = true;
                _state._flex = value;
            }
        }

        [Parameter]
        public bool FlexS { get => Flex == FbLayoutPresets.S_Flex; set => Flex = FbLayoutPresets.S_Flex; }

        [Parameter]
        public bool FlexM { get => Flex == FbLayoutPresets.M_Flex; set => Flex = FbLayoutPresets.M_Flex; }

        [Parameter]
        public bool FlexL { get => Flex == FbLayoutPresets.L_Flex; set => Flex = FbLayoutPresets.L_Flex; }

        [Parameter]
        public bool FlexX { get => Flex == FbLayoutPresets.X_Flex; set => Flex = FbLayoutPresets.X_Flex; }

        [Parameter]
        public bool FlexXX { get => Flex == FbLayoutPresets.XX_Flex; set => Flex = FbLayoutPresets.XX_Flex; }

        /// <summary>
        /// Disable flex grow/shrink
        /// </summary>
        [Parameter]
        public bool NoFlex
        {
            get => _state._flex == int.MinValue;
            set { _state._flex = int.MinValue; }
        }

        /// <summary>
        /// Flex basis width
        /// </summary>
        [Parameter]
        public string WidthBasis
        {
            get => _widthBasis;
            set
            {
                _widthBasis = value;
                if (!string.IsNullOrWhiteSpace(_widthBasis))
                    _state._flex = int.MinValue;
            }
        }

        /// <summary>
        /// Disable text wrap
        /// </summary>
        [Parameter]
        public bool NoTextWrap
        {
            get => _state._noWrap;
            set => _state._noWrap = true;
        }

        /// <summary>
        /// Enable text wrap
        /// </summary>
        [Parameter]
        public bool TextWrap
        {
            get => !_state._noWrap;
            set => _state._noWrap = false;
        }

        protected string WrapClassString
        {
            get
            {
                if (_state._noWrap)
                    return "fb-text-no-wrap";
                return "";
            }
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
        public bool Hidden { get; set; } = false;

        /// <summary>
        /// Responsive container setting
        /// </summary>
        [Parameter]
        public bool ResponsiveContainer { get; set; }

        /// <summary>
        /// Show when initialize container width ≥ 1536px
        /// </summary>
        [Parameter]
        public bool ShowOverXXL { get => ResponsiveBreakpoint == FbResponsiveBreakpoint.XXL_1536px && !_state._responsiveUnder; set { ResponsiveBreakpoint = FbResponsiveBreakpoint.XXL_1536px; _state._responsiveUnder = false; } }

        /// <summary>
        /// Show when initialize container width ≤ 1536px
        /// </summary>
        [Parameter]
        public bool ShowUnderXXL { get => ResponsiveBreakpoint == FbResponsiveBreakpoint.XXL_1536px && _state._responsiveUnder; set { ResponsiveBreakpoint = FbResponsiveBreakpoint.XXL_1536px; _state._responsiveUnder = true; } }

        /// <summary>
        /// Show when initialize container width ≥ 1280px
        /// </summary>
        [Parameter]
        public bool ShowOverXL { get => ResponsiveBreakpoint == FbResponsiveBreakpoint.XL_1280px && !_state._responsiveUnder; set { ResponsiveBreakpoint = FbResponsiveBreakpoint.XL_1280px; _state._responsiveUnder = false; } }

        /// <summary>
        /// Show when initialize container width ≤ 1280px
        /// </summary>
        [Parameter]
        public bool ShowUnderXL { get => ResponsiveBreakpoint == FbResponsiveBreakpoint.XL_1280px && _state._responsiveUnder; set { ResponsiveBreakpoint = FbResponsiveBreakpoint.XL_1280px; _state._responsiveUnder = true; } }

        /// <summary>
        /// Show when initialize container width ≥ 1024px
        /// </summary>
        [Parameter]
        public bool ShowOverL { get => ResponsiveBreakpoint == FbResponsiveBreakpoint.L_1024px && !_state._responsiveUnder; set { ResponsiveBreakpoint = FbResponsiveBreakpoint.L_1024px; _state._responsiveUnder = false; } }

        /// <summary>
        /// Show when initialize container width ≤ 1024px
        /// </summary>
        [Parameter]
        public bool ShowUnderL { get => ResponsiveBreakpoint == FbResponsiveBreakpoint.L_1024px && _state._responsiveUnder; set { ResponsiveBreakpoint = FbResponsiveBreakpoint.L_1024px; _state._responsiveUnder = true; } }

        /// <summary>
        /// Show when initialize container width ≥ 768px
        /// </summary>
        [Parameter]
        public bool ShowOverM { get => ResponsiveBreakpoint == FbResponsiveBreakpoint.M_768px && !_state._responsiveUnder; set { ResponsiveBreakpoint = FbResponsiveBreakpoint.M_768px; _state._responsiveUnder = false; } }

        /// <summary>
        /// Show when initialize container width ≤ 768px
        /// </summary>
        [Parameter]
        public bool ShowUnderM { get => ResponsiveBreakpoint == FbResponsiveBreakpoint.M_768px && _state._responsiveUnder; set { ResponsiveBreakpoint = FbResponsiveBreakpoint.M_768px; _state._responsiveUnder = true; } }

        /// <summary>
        /// Show when initialize container width ≥ 640px
        /// </summary>
        [Parameter]
        public bool ShowOverS { get => ResponsiveBreakpoint == FbResponsiveBreakpoint.S_640px && !_state._responsiveUnder; set { ResponsiveBreakpoint = FbResponsiveBreakpoint.S_640px; _state._responsiveUnder = false; } }

        /// <summary>
        /// Show when initialize container width ≤ 640px
        /// </summary>
        [Parameter]
        public bool ShowUnderS { get => ResponsiveBreakpoint == FbResponsiveBreakpoint.S_640px && _state._responsiveUnder; set { ResponsiveBreakpoint = FbResponsiveBreakpoint.S_640px; _state._responsiveUnder = true; } }

        /// <summary>
        /// Show when initialize container width ≥ 360px
        /// </summary>
        [Parameter]
        public bool ShowOverXS { get => ResponsiveBreakpoint == FbResponsiveBreakpoint.XS_360px && !_state._responsiveUnder; set { ResponsiveBreakpoint = FbResponsiveBreakpoint.XS_360px; _state._responsiveUnder = false; } }

        /// <summary>
        /// Show when initialize container width ≤ 360px
        /// </summary>
        [Parameter]
        public bool ShowUnderXS { get => ResponsiveBreakpoint == FbResponsiveBreakpoint.XS_360px && _state._responsiveUnder; set { ResponsiveBreakpoint = FbResponsiveBreakpoint.XS_360px; _state._responsiveUnder = true; } }

        protected string ResponsiveClassString
        {
            get
            {
                if (ResponsiveContainer)
                    return "fb-container-init";

                if (ResponsiveBreakpoint is not FbResponsiveBreakpoint.None)
                {
                    var responsiveResultClasse = "fb-container";
                    switch (ResponsiveBreakpoint)
                    {
                        case FbResponsiveBreakpoint.XXL_1536px: responsiveResultClasse = $"{responsiveResultClasse}-xxl"; break;
                        case FbResponsiveBreakpoint.XL_1280px: responsiveResultClasse = $"{responsiveResultClasse}-xl"; break;
                        case FbResponsiveBreakpoint.L_1024px: responsiveResultClasse = $"{responsiveResultClasse}-l"; break;
                        case FbResponsiveBreakpoint.M_768px: responsiveResultClasse = $"{responsiveResultClasse}-m"; break;
                        case FbResponsiveBreakpoint.S_640px: responsiveResultClasse = $"{responsiveResultClasse}-s"; break;
                        case FbResponsiveBreakpoint.XS_360px: responsiveResultClasse = $"{responsiveResultClasse}-xs"; break;
                    }

                    if (_state._responsiveUnder)
                        return $"{responsiveResultClasse}-none";
                    else
                        return $"{responsiveResultClasse}-flex";
                }

                return "";
            }
        }

        /// <summary>
        /// Padding
        /// </summary>
        [Parameter]
        public FbSpacing P { get => Padding; set => Padding = value; }

        /// <summary>
        /// Padding -> Top
        /// </summary>
        [Parameter]
        public FbSpacing PTop { get => PaddingTop; set => PaddingTop = value; }

        /// <summary>
        /// Padding -> Bottom
        /// </summary>
        [Parameter]
        public FbSpacing PBottom { get => PaddingBottom; set => PaddingBottom = value; }

        /// <summary>
        /// Padding -> Left
        /// </summary>
        [Parameter]
        public FbSpacing PLeft { get => PaddingLeft; set => PaddingLeft = value; }

        /// <summary>
        /// Padding -> Right
        /// </summary>
        [Parameter]
        public FbSpacing PRight { get => PaddingRight; set => PaddingRight = value; }

        /// <summary>
        /// Padding -> Horizontal
        /// </summary>
        [Parameter]
        public FbSpacing PH { get => PaddingLeft; set { PaddingLeft = PaddingRight = value; } }

        /// <summary>
        /// Padding -> Vertical
        /// </summary>
        [Parameter]
        public FbSpacing PV { get => PaddingTop; set { PaddingTop = PaddingBottom = value; } }

        /// <summary>
        /// Padding -> Small
        /// </summary>
        [Parameter]
        public bool PS { get => Padding == FbLayoutPresets.S; set => Padding = FbLayoutPresets.S; }

        /// <summary>
        /// Padding -> Medium
        /// </summary>
        [Parameter]
        public bool PM { get => Padding == FbLayoutPresets.M; set => Padding = FbLayoutPresets.M; }

        /// <summary>
        /// Padding -> Large
        /// </summary>
        [Parameter]
        public bool PL { get => Padding == FbLayoutPresets.L; set => Padding = FbLayoutPresets.L; }

        /// <summary>
        /// Padding -> Extra Large
        /// </summary>
        [Parameter]
        public bool PX { get => Padding == FbLayoutPresets.X; set => Padding = FbLayoutPresets.X; }

        /// <summary>
        /// Padding -> Vertical -> Small 
        /// </summary>
        [Parameter]
        public bool PVS { get => PaddingTop == FbLayoutPresets.S && PaddingBottom == FbLayoutPresets.S; set { PaddingTop = PaddingBottom = FbLayoutPresets.S; } }

        /// <summary>
        /// Padding -> Vertical -> Medium 
        /// </summary>
        [Parameter]
        public bool PVM { get => PaddingTop == FbLayoutPresets.M && PaddingBottom == FbLayoutPresets.M; set { PaddingTop = PaddingBottom = FbLayoutPresets.M; } }

        /// <summary>
        /// Padding -> Vertical -> Large 
        /// </summary>
        [Parameter]
        public bool PVL { get => PaddingTop == FbLayoutPresets.L && PaddingBottom == FbLayoutPresets.L; set { PaddingTop = PaddingBottom = FbLayoutPresets.L; } }

        /// <summary>
        /// Padding -> Vertical -> Extra Large 
        /// </summary>
        [Parameter]
        public bool PVX { get => PaddingTop == FbLayoutPresets.X && PaddingBottom == FbLayoutPresets.X; set { PaddingTop = PaddingBottom = FbLayoutPresets.X; } }

        /// <summary>
        /// Padding -> Horizontal -> Small 
        /// </summary>
        [Parameter]
        public bool PHS { get => PaddingLeft == FbLayoutPresets.S && PaddingRight == FbLayoutPresets.S; set { PaddingLeft = PaddingRight = FbLayoutPresets.S; } }

        /// <summary>
        /// Padding -> Horizontal -> Medium 
        /// </summary>
        [Parameter]
        public bool PHM { get => PaddingLeft == FbLayoutPresets.M && PaddingRight == FbLayoutPresets.M; set { PaddingLeft = PaddingRight = FbLayoutPresets.M; } }

        /// <summary>
        /// Padding -> Horizontal -> Large 
        /// </summary>
        [Parameter]
        public bool PHL { get => PaddingLeft == FbLayoutPresets.L && PaddingRight == FbLayoutPresets.L; set { PaddingLeft = PaddingRight = FbLayoutPresets.L; } }

        /// <summary>
        /// Padding -> Horizontal -> Extra Large 
        /// </summary>
        [Parameter]
        public bool PHX { get => PaddingLeft == FbLayoutPresets.X && PaddingRight == FbLayoutPresets.X; set { PaddingLeft = PaddingRight = FbLayoutPresets.X; } }

        /// <summary>
        /// Padding -> Top -> Small
        /// </summary>
        [Parameter]
        public bool PTS { get => PaddingTop == FbLayoutPresets.S; set => PaddingTop = FbLayoutPresets.S; }

        /// <summary>
        /// Padding -> Top -> Medium
        /// </summary>
        [Parameter]
        public bool PTM { get => PaddingTop == FbLayoutPresets.M; set => PaddingTop = FbLayoutPresets.M; }

        /// <summary>
        /// Padding -> Top -> Large
        /// </summary>
        [Parameter]
        public bool PTL { get => PaddingTop == FbLayoutPresets.L; set => PaddingTop = FbLayoutPresets.L; }

        /// <summary>
        /// Padding -> Top -> Extra Large
        /// </summary>
        [Parameter]
        public bool PTX { get => PaddingTop == FbLayoutPresets.X; set => PaddingTop = FbLayoutPresets.X; }

        /// <summary>
        /// Padding -> Bottom -> Small
        /// </summary>
        [Parameter]
        public bool PBS { get => PaddingBottom == FbLayoutPresets.S; set => PaddingBottom = FbLayoutPresets.S; }

        /// <summary>
        /// Padding -> Bottom -> Medium
        /// </summary>
        [Parameter]
        public bool PBM { get => PaddingBottom == FbLayoutPresets.M; set => PaddingBottom = FbLayoutPresets.M; }

        /// <summary>
        /// Padding -> Bottom -> Large
        /// </summary>
        [Parameter]
        public bool PBL { get => PaddingBottom == FbLayoutPresets.L; set => PaddingBottom = FbLayoutPresets.L; }

        /// <summary>
        /// Padding -> Bottom -> Extra Large
        /// </summary>
        [Parameter]
        public bool PBX { get => PaddingBottom == FbLayoutPresets.X; set => PaddingBottom = FbLayoutPresets.X; }

        /// <summary>
        /// Padding -> Left -> Small
        /// </summary>
        [Parameter]
        public bool PLS { get => PaddingLeft == FbLayoutPresets.S; set => PaddingLeft = FbLayoutPresets.S; }

        /// <summary>
        /// Padding -> Left -> Medium
        /// </summary>
        [Parameter]
        public bool PLM { get => PaddingLeft == FbLayoutPresets.M; set => PaddingLeft = FbLayoutPresets.M; }

        /// <summary>
        /// Padding -> Left -> Large
        /// </summary>
        [Parameter]
        public bool PLL { get => PaddingLeft == FbLayoutPresets.L; set => PaddingLeft = FbLayoutPresets.L; }

        /// <summary>
        /// Padding -> Left -> Extra Large
        /// </summary>
        [Parameter]
        public bool PLX { get => PaddingLeft == FbLayoutPresets.X; set => PaddingLeft = FbLayoutPresets.X; }

        /// <summary>
        /// Padding -> Right -> Small
        /// </summary>
        [Parameter]
        public bool PRS { get => PaddingRight == FbLayoutPresets.S; set => PaddingRight = FbLayoutPresets.S; }

        /// <summary>
        /// Padding -> Right -> Medium
        /// </summary>
        [Parameter]
        public bool PRM { get => PaddingRight == FbLayoutPresets.M; set => PaddingRight = FbLayoutPresets.M; }

        /// <summary>
        /// Padding -> Right -> Large
        /// </summary>
        [Parameter]
        public bool PRL { get => PaddingRight == FbLayoutPresets.L; set => PaddingRight = FbLayoutPresets.L; }

        /// <summary>
        /// Padding -> Right -> Extra Large
        /// </summary>
        [Parameter]
        public bool PRX { get => PaddingRight == FbLayoutPresets.X; set => PaddingRight = FbLayoutPresets.X; }

        /// <summary>
        /// Margin
        /// </summary>
        [Parameter]
        public FbSpacing M { get => Margin; set => Margin = value; }

        /// <summary>
        /// Margin -> Top
        /// </summary>
        [Parameter]
        public FbSpacing MTop { get => MarginTop; set => MarginTop = value; }

        /// <summary>
        /// Margin -> Bottom
        /// </summary>
        [Parameter]
        public FbSpacing MBottom { get => MarginBottom; set => MarginBottom = value; }

        /// <summary>
        /// Margin -> Left
        /// </summary>
        [Parameter]
        public FbSpacing MLeft { get => MarginLeft; set => MarginLeft = value; }

        /// <summary>
        /// Margin -> Right
        /// </summary>
        [Parameter]
        public FbSpacing MRight { get => MarginRight; set => MarginRight = value; }

        /// <summary>
        /// Margin -> Horizontal
        /// </summary>
        [Parameter]
        public FbSpacing MH { get => MarginLeft; set { MarginLeft = MarginRight = value; } }

        /// <summary>
        /// Margin -> Vertical
        /// </summary>
        [Parameter]
        public FbSpacing MV { get => MarginTop; set { MarginTop = MarginBottom = value; } }

        /// <summary>
        /// Margin -> Small
        /// </summary>
        [Parameter]
        public bool MS { get => Margin == FbLayoutPresets.S; set => Margin = FbLayoutPresets.S; }

        /// <summary>
        /// Margin -> Medium
        /// </summary>
        [Parameter]
        public bool MM { get => Margin == FbLayoutPresets.M; set => Margin = FbLayoutPresets.M; }

        /// <summary>
        /// Margin -> Large
        /// </summary>
        [Parameter]
        public bool ML { get => Margin == FbLayoutPresets.L; set => Margin = FbLayoutPresets.L; }

        /// <summary>
        /// Margin -> Extra Large
        /// </summary>
        [Parameter]
        public bool MX { get => Margin == FbLayoutPresets.X; set => Margin = FbLayoutPresets.X; }

        /// <summary>
        /// Margin -> Vertical -> Small 
        /// </summary>
        [Parameter]
        public bool MVS { get => MarginTop == FbLayoutPresets.S && MarginBottom == FbLayoutPresets.S; set { MarginTop = MarginBottom = FbLayoutPresets.S; } }

        /// <summary>
        /// Margin -> Vertical -> Medium 
        /// </summary>
        [Parameter]
        public bool MVM { get => MarginTop == FbLayoutPresets.M && MarginBottom == FbLayoutPresets.M; set { MarginTop = MarginBottom = FbLayoutPresets.M; } }

        /// <summary>
        /// Margin -> Vertical -> Large 
        /// </summary>
        [Parameter]
        public bool MVL { get => MarginTop == FbLayoutPresets.L && MarginBottom == FbLayoutPresets.L; set { MarginTop = MarginBottom = FbLayoutPresets.L; } }

        /// <summary>
        /// Margin -> Vertical -> Extra Large 
        /// </summary>
        [Parameter]
        public bool MVX { get => MarginTop == FbLayoutPresets.X && MarginBottom == FbLayoutPresets.X; set { MarginTop = MarginBottom = FbLayoutPresets.X; } }

        /// <summary>
        /// Margin -> Horizontal -> Small 
        /// </summary>
        [Parameter]
        public bool MHS { get => MarginLeft == FbLayoutPresets.S && MarginRight == FbLayoutPresets.S; set { MarginLeft = MarginRight = FbLayoutPresets.S; } }

        /// <summary>
        /// Margin -> Horizontal -> Medium 
        /// </summary>
        [Parameter]
        public bool MHM { get => MarginLeft == FbLayoutPresets.M && MarginRight == FbLayoutPresets.M; set { MarginLeft = MarginRight = FbLayoutPresets.M; } }

        /// <summary>
        /// Margin -> Horizontal -> Large 
        /// </summary>
        [Parameter]
        public bool MHL { get => MarginLeft == FbLayoutPresets.L && MarginRight == FbLayoutPresets.L; set { MarginLeft = MarginRight = FbLayoutPresets.L; } }

        /// <summary>
        /// Margin -> Horizontal -> Extra Large 
        /// </summary>
        [Parameter]
        public bool MHX { get => MarginLeft == FbLayoutPresets.X && MarginRight == FbLayoutPresets.X; set { MarginLeft = MarginRight = FbLayoutPresets.X; } }

        /// <summary>
        /// Margin -> Top -> Small
        /// </summary>
        [Parameter]
        public bool MTS { get => MarginTop == FbLayoutPresets.S; set => MarginTop = FbLayoutPresets.S; }

        /// <summary>
        /// Margin -> Top -> Medium
        /// </summary>
        [Parameter]
        public bool MTM { get => MarginTop == FbLayoutPresets.M; set => MarginTop = FbLayoutPresets.M; }

        /// <summary>
        /// Margin -> Top -> Large
        /// </summary>
        [Parameter]
        public bool MTL { get => MarginTop == FbLayoutPresets.L; set => MarginTop = FbLayoutPresets.L; }

        /// <summary>
        /// Margin -> Top -> Extra Large
        /// </summary>
        [Parameter]
        public bool MTX { get => MarginTop == FbLayoutPresets.X; set => MarginTop = FbLayoutPresets.X; }

        /// <summary>
        /// Margin -> Bottom -> Small
        /// </summary>
        [Parameter]
        public bool MBS { get => MarginBottom == FbLayoutPresets.S; set => MarginBottom = FbLayoutPresets.S; }

        /// <summary>
        /// Margin -> Bottom -> Medium
        /// </summary>
        [Parameter]
        public bool MBM { get => MarginBottom == FbLayoutPresets.M; set => MarginBottom = FbLayoutPresets.M; }

        /// <summary>
        /// Margin -> Bottom -> Large
        /// </summary>
        [Parameter]
        public bool MBL { get => MarginBottom == FbLayoutPresets.L; set => MarginBottom = FbLayoutPresets.L; }

        /// <summary>
        /// Margin -> Bottom -> Extra Large
        /// </summary>
        [Parameter]
        public bool MBX { get => MarginBottom == FbLayoutPresets.X; set => MarginBottom = FbLayoutPresets.X; }

        /// <summary>
        /// Margin -> Left -> Small
        /// </summary>
        [Parameter]
        public bool MLS { get => MarginLeft == FbLayoutPresets.S; set => MarginLeft = FbLayoutPresets.S; }

        /// <summary>
        /// Margin -> Left -> Medium
        /// </summary>
        [Parameter]
        public bool MLM { get => MarginLeft == FbLayoutPresets.M; set => MarginLeft = FbLayoutPresets.M; }

        /// <summary>
        /// Margin -> Left -> Large
        /// </summary>
        [Parameter]
        public bool MLL { get => MarginLeft == FbLayoutPresets.L; set => MarginLeft = FbLayoutPresets.L; }

        /// <summary>
        /// Margin -> Left -> Extra Large
        /// </summary>
        [Parameter]
        public bool MLX { get => MarginLeft == FbLayoutPresets.X; set => MarginLeft = FbLayoutPresets.X; }

        /// <summary>
        /// Margin -> Right -> Small
        /// </summary>
        [Parameter]
        public bool MRS { get => MarginRight == FbLayoutPresets.S; set => MarginRight = FbLayoutPresets.S; }

        /// <summary>
        /// Margin -> Right -> Medium
        /// </summary>
        [Parameter]
        public bool MRM { get => MarginRight == FbLayoutPresets.M; set => MarginRight = FbLayoutPresets.M; }

        /// <summary>
        /// Margin -> Right -> Large
        /// </summary>
        [Parameter]
        public bool MRL { get => MarginRight == FbLayoutPresets.L; set => MarginRight = FbLayoutPresets.L; }

        /// <summary>
        /// Margin -> Right -> Extra Large
        /// </summary>
        [Parameter]
        public bool MRX { get => MarginRight == FbLayoutPresets.X; set => MarginRight = FbLayoutPresets.X; }

        /// <summary>
        /// Store identifier
        /// </summary>
        [Parameter]
        public string StoreId { get; set; } = "";
    }
}
