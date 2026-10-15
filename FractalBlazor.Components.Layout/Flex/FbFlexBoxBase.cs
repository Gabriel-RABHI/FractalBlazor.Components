using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using FractalBlazor.Components.Layout.Abstracts;
using FractalBlazor.Components.Layout.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout.Flex
{
    public abstract class FbFlexBoxBase : FbLayoutSurfaceComponentBase
    {
        #region HIDDEN
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct FbFlexBoxBaseState
        {
            public FbFlexBoxBaseState()
            {

            }

            // -------- Fields
            public FbFlexJustify _justify = FbFlexJustify.SpaceBetween;
            public FbFlexDirection _direction = FbFlexDirection.Row;
            public FbFlexWrap _wrap = FbFlexWrap.NoWrap;
            public FbFlexAlignItems _alignItems = FbFlexAlignItems.Center;
            public FbFlexPlaceItems _placeItems = FbFlexPlaceItems.None;
            public FbFlexAlignContent _alignContent = FbFlexAlignContent.Center;
            public FbFlexItemSelfAlign _selfAlign = FbFlexItemSelfAlign.None;
            public FbFlexSize _minWithSize = FbFlexSize.None;
            public FbFlexSize _minHeightSize = FbFlexSize.None;
            public FbFlexSize _maxHeightSize = FbFlexSize.None;

            public FbSpacing Gutter = FbSpacing.None;
            //public FbSpacing RowGutter = FbSpacing.None;
            //public FbSpacing ColumnGutter = FbSpacing.None;

            public bool ColumnDisplay = false;

            public int Order = int.MinValue;
            public int Grow = int.MinValue;
            public int Shrink = int.MinValue;

            public bool Scrollable = false;

            public bool Joined = false;
        }

        private FbFlexBoxBaseState _state = new FbFlexBoxBaseState();
        private FbFlexBoxBaseState _defaultState;

        /// <summary>
        /// Class cache key of this level: the surface key plus the flexbox state.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct FlexKey
        {
            public SurfaceKey Surface;
            public FbFlexBoxBaseState Flex;
        }

        internal FlexKey FlexKeyValue => new() { Surface = SurfaceKeyValue, Flex = _state };

        public FbFlexBoxBase()
        {
            IsFlex = true;
        }

        protected override void CaptureParameterDefaults()
        {
            base.CaptureParameterDefaults();
            _defaultState = _state;
        }

        protected override void RestoreParameterDefaults()
        {
            base.RestoreParameterDefaults();
            _state = _defaultState;
        }

        // -------------- As flex item ------------ //
        protected FbSpacing Gutter { get => _state.Gutter; set => _state.Gutter = value; }

        //protected FbSpacing RowGutter { get => _state.RowGutter; set => _state.RowGutter = value; }

        //protected FbSpacing ColumnGutter { get => _state.ColumnGutter; set => _state.ColumnGutter = value; }

        protected bool ColumnDisplay { get => _state.ColumnDisplay; set => _state.ColumnDisplay = value; }

        // -------------- As flex item ------------ //
        protected int Order { get => _state.Order; set => _state.Order = value; }

        protected int Grow { get => _state.Grow; set => _state.Grow = value; }

        protected int Shrink { get => _state.Shrink; set => _state.Shrink = value; }

        // -------------- FbFlexDirection ------------ //
        protected FbFlexDirection Direction { get => _state._direction; set => _state._direction = value; }

        // -------------- FbFlexWrap ------------ //
        protected FbFlexWrap Wrap { get => _state._wrap; set => _state._wrap = value; }

        // -------------- FbFlexJustify ------------ //
        protected FbFlexJustify Justify { get => _state._justify; set => _state._justify = value; }

        // -------------- FbFlexAlignItems ------------ //
        protected FbFlexAlignItems AlignItems { get => _state._alignItems; set => _state._alignItems = value; }

        // -------------- FbFlexAlignContent ------------ //
        protected FbFlexAlignContent AlignContent { get => _state._alignContent; set => _state._alignContent = value; }

        // -------------- FbFlexPlaceItems ------------ //
        protected FbFlexPlaceItems PlaceItems { get => _state._placeItems; set => _state._placeItems = value; }

        // -------- Value assignation
        protected string? MinWidth { get; set; }

        protected bool MinWidthMaxContent { get => _state._minWithSize == FbFlexSize.MaxContent; set { if (value) _state._minWithSize = FbFlexSize.MaxContent; } }

        protected bool MinWidthMinContent { get => _state._minWithSize == FbFlexSize.MinContent; set { if (value) _state._minWithSize = FbFlexSize.MinContent; } }

        protected string? MaxWidth { get; set; }

        // -------- Value assignation
        protected string? MinHeight { get; set; }

        protected bool MinHeightMaxContent { get => _state._minHeightSize == FbFlexSize.MaxContent; set { if (value) _state._minHeightSize = FbFlexSize.MaxContent; } }

        protected bool MinHeightMinContent { get => _state._minHeightSize == FbFlexSize.MinContent; set { if (value) _state._minHeightSize = FbFlexSize.MinContent; } }

        // -------- Value assignation
        protected string? MaxHeight { get; set; }

        protected bool MaxHeightMaxContent { get => _state._maxHeightSize == FbFlexSize.MaxContent; set { if (value) _state._maxHeightSize = FbFlexSize.MaxContent; } }

        protected bool MaxHeightMinContent { get => _state._maxHeightSize == FbFlexSize.MinContent; set { if (value) _state._maxHeightSize = FbFlexSize.MinContent; } }

        /// <summary>
        /// Inline style of this level: the surface one plus free-form sizes and the order,
        /// grow and shrink factors, which have no atom.
        /// </summary>
        protected new string? AggregatedStyles
            => ResolveStyle(
                FlexKeyValue,
                new FbStyleInputs(FlexBasis, Style, HoverMixOffset, MinWidth, MaxWidth, MinHeight, MaxHeight),
                static component => ((FbFlexBoxBase)component).BuildFlexStyle());

        private string? BuildFlexStyle()
            => FbInlineStyle.Join(
                BuildSurfaceStyle(),
                Grow != int.MinValue ? $"flex-grow:{Grow}" : null,
                Shrink != int.MinValue ? $"flex-shrink:{Shrink}" : null,
                Order != int.MinValue ? $"order:{Order}" : null,
                MinWidth is null ? null : $"min-width:{MinWidth}",
                MaxWidth is null ? null : $"max-width:{MaxWidth}",
                MinHeight is null ? null : $"min-height:{MinHeight}",
                MaxHeight is null ? null : $"max-height:{MaxHeight}");

        /// <summary>
        /// Adds the atoms of this level and of the levels below: position, direction, wrap,
        /// alignments, gutter and sizes. A free-form size overrides its atom from the inline
        /// style.
        /// </summary>
        private protected void AddFlexAtoms(FbAtomLine line)
        {
            AddSurfaceAtoms(line);
            RenderingStatistics.AddFlexBoxBaseStyleComputation();

            line.Set(FbAxis.Position, FbAtoms.Relative);
            line.Set(FbAxis.AlignSelf, (int)_state._selfAlign);
            line.Set(FbAxis.FlexDirection, (int)_state._direction);
            line.Set(FbAxis.FlexWrap, (int)_state._wrap);
            line.Set(FbAxis.JustifyContent, (int)_state._justify);
            line.Set(FbAxis.AlignItems, (int)_state._alignItems);
            line.Set(FbAxis.AlignContent, (int)_state._alignContent);
            line.Set(FbAxis.PlaceItems, (int)_state._placeItems);
            line.Set(FbAxis.Gap, (int)Gutter);
            line.Set(FbAxis.MinWidth, (int)_state._minWithSize);
            line.Set(FbAxis.MinHeight, (int)_state._minHeightSize);
            line.Set(FbAxis.MaxHeight, (int)_state._maxHeightSize);
            if (ColumnDisplay)
                line.Set(FbAxis.FlexFlow, FbAtoms.Column);
        }

        /// <summary>
        /// The atoms of a component built with its constructor defaults only: what the base
        /// rule of its class (<c>.fb-row</c>, <c>.fb-stack</c>) declares in the stylesheet.
        /// </summary>
        internal FbAtomLine DefaultAtoms()
        {
            var line = new FbAtomLine();
            AddFlexAtoms(line);
            return line;
        }

        /// <summary>
        /// Classes of a flexbox: <paramref name="classes"/>, then the atoms that differ from
        /// <paramref name="baseline"/>.
        /// </summary>
        private protected string BuildAtomizedFlexClasses(string classes, FbAtomLine baseline)
        {
            var line = new FbAtomLine();
            AddFlexAtoms(line);
            return line.Classes(classes, baseline);
        }
        #endregion

        // ************************************************************************************************ //
        // ***************************************    PUBLIC   ******************************************** //
        // ************************************************************************************************ //
        #region PUBLIC PARAMETERS

        /// <summary>
        /// Flex grow factor
        /// </summary>
        [Parameter]
        public new int Flex {
            get => base.Flex;
            set {
                if (!IsFlex && !IsInlineFlex)
                    IsFlex = true;
                base.Flex = value;
            }
        }

        /// <summary>
        /// Disable flex grow/shrink
        /// </summary>
        [Parameter]
        public new bool NoFlex {
            get => base.Flex == int.MinValue;
            set { if (value) base.Flex = int.MinValue; }
        }

        /// <summary>
        /// Flex basis width
        /// </summary>
        [Parameter]
        public new string FlexBasis {
            get => base.FlexBasis;
            set {
                base.FlexBasis = value;
                if (!string.IsNullOrWhiteSpace(base.FlexBasis))
                    base.Flex = int.MinValue;
            }
        }

        /// <summary>
        /// Child content
        /// </summary>
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Width master identifier
        /// </summary>
        [Parameter]
        public string? WidthMasterId { get; set; }

        /// <summary>
        /// Width slave identifier
        /// </summary>
        [Parameter]
        public string? WidthSlaveId { get; set; }

        /// <summary>
        /// Scrolls the content when it does not fit (class <c>fb-scrollable</c>). Inside an
        /// <c>FbPageAnchor</c>, the area takes the space left by its siblings instead of growing
        /// with its content.
        /// </summary>
        [Parameter]
        public bool Scrollable { get => _state.Scrollable; set => _state.Scrollable = value; }

        /// <summary>
        /// The children form one group: the first and the last take the outer corners of the
        /// container (class <c>fb-joined</c>), so that a fill or a frame at an end follows its
        /// rounding. The children keep their other corners and their frames: <c>WFN WRN</c>
        /// removes them, <c>WS*</c> separates them. Nothing is clipped. Meant for a single line:
        /// a row that wraps has no ends.
        /// </summary>
        [Parameter]
        public bool Joined { get => _state.Joined; set => _state.Joined = value; }

        /// <summary>
        /// <c>fb-joined</c>, with <c>fb-reverse</c> when the direction is reversed: the first
        /// child of the DOM then sits at the other end.
        /// </summary>
        private protected string? JoinedClasses
            => !_state.Joined ? null
                : _state._direction is FbFlexDirection.RowReverse or FbFlexDirection.ColumnReverse ? "fb-joined fb-reverse"
                : "fb-joined";

        #region GUTTER
        /// <summary>
        /// Gutter -> Small
        /// </summary>
        [Parameter]
        public bool GS { get => Gutter == FbSpacing.S; set { if (value) Gutter = FbSpacing.S; } }

        /// <summary>
        /// Gutter -> Medium
        /// </summary>
        [Parameter]
        public bool GM { get => Gutter == FbSpacing.M; set { if (value) Gutter = FbSpacing.M; } }

        /// <summary>
        /// Gutter -> Large
        /// </summary>
        [Parameter]
        public bool GL { get => Gutter == FbSpacing.L; set { if (value) Gutter = FbSpacing.L; } }

        /// <summary>
        /// Gutter -> Extra Large
        /// </summary>
        [Parameter]
        public bool GX { get => Gutter == FbSpacing.X; set { if (value) Gutter = FbSpacing.X; } }

        // -------- In Row
        /// <summary>
        /// Self -> Justify -> Horizontal -> Default
        /// </summary>
        [Parameter]
        public bool SJHD { get => _state._selfAlign == FbFlexItemSelfAlign.None; set { if (value) _state._selfAlign = FbFlexItemSelfAlign.None; } }

        /// <summary>
        /// Self -> Justify -> Horizontal -> Start
        /// </summary>
        [Parameter]
        public bool SJHS { get => _state._selfAlign == FbFlexItemSelfAlign.Start; set { if (value) _state._selfAlign = FbFlexItemSelfAlign.Start; } }

        /// <summary>
        /// Self -> Justify -> Horizontal -> End
        /// </summary>
        [Parameter]
        public bool SJHE { get => _state._selfAlign == FbFlexItemSelfAlign.End; set { if (value) _state._selfAlign = FbFlexItemSelfAlign.End; } }

        /// <summary>
        /// Self -> Justify -> Horizontal -> Center
        /// </summary>
        [Parameter]
        public bool SJHC { get => _state._selfAlign == FbFlexItemSelfAlign.Center; set { if (value) _state._selfAlign = FbFlexItemSelfAlign.Center; } }

        /// <summary>
        /// Self -> Justify -> Horizontal -> Stretch
        /// </summary>
        [Parameter]
        public bool SJHSt { get => _state._selfAlign == FbFlexItemSelfAlign.Stretch; set { if (value) _state._selfAlign = FbFlexItemSelfAlign.Stretch; } }

        /// <summary>
        /// Self -> Justify -> Horizontal -> Baseline
        /// </summary>
        [Parameter]
        public bool SJHBl { get => _state._selfAlign == FbFlexItemSelfAlign.Baseline; set { if (value) _state._selfAlign = FbFlexItemSelfAlign.Baseline; } }

        // -------- In Column
        /// <summary>
        /// Self -> Justify -> Vertical -> Default
        /// </summary>
        [Parameter]
        public bool SJVD { get => _state._selfAlign == FbFlexItemSelfAlign.None; set { if (value) _state._selfAlign = FbFlexItemSelfAlign.None; } }

        /// <summary>
        /// Self -> Justify -> Vertical -> Start
        /// </summary>
        [Parameter]
        public bool SJVS { get => _state._selfAlign == FbFlexItemSelfAlign.Start; set { if (value) _state._selfAlign = FbFlexItemSelfAlign.Start; } }

        /// <summary>
        /// Self -> Justify -> Vertical -> End
        /// </summary>
        [Parameter]
        public bool SJVE { get => _state._selfAlign == FbFlexItemSelfAlign.End; set { if (value) _state._selfAlign = FbFlexItemSelfAlign.End; } }

        /// <summary>
        /// Self -> Justify -> Vertical -> Center
        /// </summary>
        [Parameter]
        public bool SJVC { get => _state._selfAlign == FbFlexItemSelfAlign.Center; set { if (value) _state._selfAlign = FbFlexItemSelfAlign.Center; } }

        /// <summary>
        /// Self -> Justify -> Vertical -> Stretch
        /// </summary>
        [Parameter]
        public bool SJVSt { get => _state._selfAlign == FbFlexItemSelfAlign.Stretch; set { if (value) _state._selfAlign = FbFlexItemSelfAlign.Stretch; } }

        /// <summary>
        /// Self -> Justify -> Vertical -> Baseline
        /// </summary>
        [Parameter]
        public bool SJVBl { get => _state._selfAlign == FbFlexItemSelfAlign.Baseline; set { if (value) _state._selfAlign = FbFlexItemSelfAlign.Baseline; } }
        #endregion

        #endregion
    }
}
