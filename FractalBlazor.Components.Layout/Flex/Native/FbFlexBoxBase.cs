using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout
{
    public abstract class FbFlexBoxBase : FbLayoutComponentBase
    {
        #region HIDDEN
        private static object _locker = new object();
        private static Dictionary<int, string> _cache = new Dictionary<int, string>();

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        private unsafe struct FbFlexBoxBaseState
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
            public FbSpacing RowGutter = FbSpacing.None;
            public FbSpacing ColumnGutter = FbSpacing.None;
            
            public FbSpacing Radius = FbSpacing.None;

            public bool ColumnDisplay = false;

            public int Order = int.MinValue;
            public int Grow = int.MinValue;
            public int Shrink = int.MinValue;

            public bool Scrollable = false;
        }

        private FbFlexBoxBaseState _state = new FbFlexBoxBaseState();
        private int _hash = 0;
        private string _style = null;

        public FbFlexBoxBase()
        {
            IsFlex = true;
        }
        
        // -------------- As flex item ------------ //
        protected FbSpacing Radius { get => _state.Radius; set => _state.Radius = value; }

        protected FbSpacing Gutter { get => _state.Gutter; set => _state.Gutter = value; }

        protected FbSpacing RowGutter { get => _state.RowGutter; set => _state.RowGutter = value; }

        protected FbSpacing ColumnGutter { get => _state.ColumnGutter; set => _state.ColumnGutter = value; }

        protected bool ColumnDisplay { get => _state.ColumnDisplay; set => _state.ColumnDisplay = value; }

        // -------------- As flex item ------------ //
        protected int Order { get => _state.Order; set => _state.Order = value; }

        protected int Grow { get => _state.Grow; set => _state.Grow = value; }

        protected int Shrink { get => _state.Shrink; set => _state.Shrink = value; }

        // -------------- FbFlexDirection ------------ //
        protected FbFlexDirection Direction { get => _state._direction; set => _state._direction = value; }

        private string DirectionString
        {
            get
            {
                switch (_state._direction)
                {
                    case FbFlexDirection.Row:
                        return "row";
                    case FbFlexDirection.RowReverse:
                        return "row-reverse";
                    case FbFlexDirection.Column:
                        return "column";
                    case FbFlexDirection.ColumnReverse:
                        return "column-reverse";
                }
                return "";
            }
        }

        // -------------- FbFlexWrap ------------ //
        protected FbFlexWrap Wrap { get => _state._wrap; set => _state._wrap = value; }

        private string WrapString
        {
            get
            {
                switch (_state._wrap)
                {
                    case FbFlexWrap.NoWrap:
                        return "nowrap";
                    case FbFlexWrap.Wrap:
                        return "wrap";
                    case FbFlexWrap.WrapReverse:
                        return "wrap-reverse";
                }
                return "";
            }
        }

        // -------------- FbFlexJustify ------------ //
        protected FbFlexJustify Justify { get => _state._justify; set => _state._justify = value; }

        private string JustifyString
        {
            get
            {
                switch (_state._justify)
                {
                    case FbFlexJustify.Start:
                        return "flex-start";
                    case FbFlexJustify.End:
                        return "flex-end";
                    case FbFlexJustify.Center:
                        return "center";
                    case FbFlexJustify.Stretch:
                        return "stretch";
                    case FbFlexJustify.SpaceBetween:
                        return "space-between";
                    case FbFlexJustify.SpaceAround:
                        return "space-around";
                    case FbFlexJustify.Evenly:
                        return "space-evenly";
                }
                return "";
            }
        }

        // -------------- FbFlexAlignItems ------------ //
        protected FbFlexAlignItems AlignItems { get => _state._alignItems; set => _state._alignItems = value; }

        private string AlignItemsString
        {
            get
            {
                switch (_state._alignItems)
                {
                    case FbFlexAlignItems.Start:
                        return "flex-start";
                    case FbFlexAlignItems.End:
                        return "flex-end";
                    case FbFlexAlignItems.Center:
                        return "center";
                    case FbFlexAlignItems.Baseline:
                        return "baseline";
                    case FbFlexAlignItems.Stretch:
                        return "stretch";
                }
                return "";
            }
        }

        // -------------- FbFlexAlignContent ------------ //
        protected FbFlexAlignContent AlignContent { get => _state._alignContent; set => _state._alignContent = value; }

        private string AlignContentString
        {
            get
            {
                switch (_state._alignContent)
                {
                    case FbFlexAlignContent.Start:
                        return "flex-start";
                    case FbFlexAlignContent.End:
                        return "flex-end";
                    case FbFlexAlignContent.Center:
                        return "center";
                    case FbFlexAlignContent.SpaceBetween:
                        return "space-between";
                    case FbFlexAlignContent.SpaceAround:
                        return "space-around";
                    case FbFlexAlignContent.Stretch:
                        return "stretch";
                }
                return "";
            }
        }

        // -------------- FbFlexPlaceItems ------------ //
        protected FbFlexPlaceItems PlaceItems { get => _state._placeItems; set => _state._placeItems = value; }

        private string PlaceItemsString
        {
            get
            {
                switch (_state._placeItems)
                {
                    case FbFlexPlaceItems.Center:
                        return "center";
                    case FbFlexPlaceItems.Normal:
                        return "normal";
                    case FbFlexPlaceItems.Start:
                        return "start";
                    case FbFlexPlaceItems.End:
                        return "end";
                    case FbFlexPlaceItems.SelfStart:
                        return "self-start";
                    case FbFlexPlaceItems.SelfEnd:
                        return "self-end";
                    case FbFlexPlaceItems.FlexStart:
                        return "flex-start";
                    case FbFlexPlaceItems.FlexEnd:
                        return "flex-end";
                    case FbFlexPlaceItems.Baseline:
                        return "baseline";
                    case FbFlexPlaceItems.Stretch:
                        return "stretch";
                    default:
                        return "";
                }
            }
        }

        protected FbFlexMasterSlave MasterSlaveStatus
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(WidthMasterId))
                    return FbFlexMasterSlave.Master;
                if (!string.IsNullOrWhiteSpace(WidthSlaveId))
                    return FbFlexMasterSlave.Slave;
                return FbFlexMasterSlave.None;
            }
        }

        protected bool NoSelf { get => _state._selfAlign == FbFlexItemSelfAlign.None; set => _state._selfAlign = FbFlexItemSelfAlign.None; }

        // -------- In Row
        protected bool SelfOnTop { get => _state._selfAlign == FbFlexItemSelfAlign.Start; set => _state._selfAlign = FbFlexItemSelfAlign.Start; }

        protected bool SelfOnBottom { get => _state._selfAlign == FbFlexItemSelfAlign.End; set => _state._selfAlign = FbFlexItemSelfAlign.End; }

        protected bool SelfOnCenter { get => _state._selfAlign == FbFlexItemSelfAlign.Center; set => _state._selfAlign = FbFlexItemSelfAlign.Center; }

        protected bool SelfOnStretch { get => _state._selfAlign == FbFlexItemSelfAlign.Stretch; set => _state._selfAlign = FbFlexItemSelfAlign.Stretch; }

        protected bool SelfOnBaseline { get => _state._selfAlign == FbFlexItemSelfAlign.Baseline; set => _state._selfAlign = FbFlexItemSelfAlign.Baseline; }

        // -------- In Column
        protected bool SelfJustifyRight { get => _state._selfAlign == FbFlexItemSelfAlign.Start; set => _state._selfAlign = FbFlexItemSelfAlign.Start; }

        protected bool SelfJustifyLeft { get => _state._selfAlign == FbFlexItemSelfAlign.End; set => _state._selfAlign = FbFlexItemSelfAlign.End; }

        protected bool SelfJustifyCenter { get => _state._selfAlign == FbFlexItemSelfAlign.Center; set => _state._selfAlign = FbFlexItemSelfAlign.Center; }

        protected bool SelfJustifyStretch { get => _state._selfAlign == FbFlexItemSelfAlign.Stretch; set => _state._selfAlign = FbFlexItemSelfAlign.Stretch; }

        protected bool SelfJustifyBaseline { get => _state._selfAlign == FbFlexItemSelfAlign.Baseline; set => _state._selfAlign = FbFlexItemSelfAlign.Baseline; }

        private string SelfAlignString
        {
            get
            {
                // auto | flex-start | flex-end | center | baseline | stretch
                switch (_state._selfAlign)
                {
                    case FbFlexItemSelfAlign.Start:
                        return "flex-start";
                    case FbFlexItemSelfAlign.End:
                        return "flex-end";
                    case FbFlexItemSelfAlign.Center:
                        return "center";
                    case FbFlexItemSelfAlign.Stretch:
                        return "stretch";
                    case FbFlexItemSelfAlign.Baseline:
                        return "baseline";
                }
                return "";
            }
        }

        // -------- Value assignation
        protected new string MinWidth { get; set; }

        protected bool MinWidthMaxContent { get => _state._minWithSize == FbFlexSize.MaxContent; set => _state._minWithSize = FbFlexSize.MaxContent; }

        protected bool MinWidthMinContent { get => _state._minWithSize == FbFlexSize.MinContent; set => _state._minWithSize = FbFlexSize.MinContent; }

        private string MinWithString
        {
            get
            {
                if (MinWidth != null)
                    return MinWidth;

                switch (_state._minWithSize)
                {
                    case FbFlexSize.MaxContent:
                        return "max-content";
                    case FbFlexSize.MinContent:
                        return "min-content";
                    default:
                        return "";
                }
            }
        }

        // -------- Value assignation
        protected new string MinHeight { get; set; }

        protected bool MinHeightMaxContent { get => _state._minHeightSize == FbFlexSize.MaxContent; set => _state._minHeightSize = FbFlexSize.MaxContent; }

        protected bool MinHeightMinContent { get => _state._minHeightSize == FbFlexSize.MinContent; set => _state._minHeightSize = FbFlexSize.MinContent; }

        private string MinHeightString
        {
            get
            {
                if (MinHeight != null)
                    return MinHeight;

                switch (_state._minHeightSize)
                {
                    case FbFlexSize.MaxContent:
                        return "max-content";
                    case FbFlexSize.MinContent:
                        return "min-content";
                    default:
                        return "";
                }
            }
        }

        // -------- Value assignation
        protected new string MaxHeight { get; set; }

        protected bool MaxHeightMaxContent { get => _state._maxHeightSize == FbFlexSize.MaxContent; set => _state._maxHeightSize = FbFlexSize.MaxContent; }

        protected bool MaxHeightMinContent { get => _state._maxHeightSize == FbFlexSize.MinContent; set => _state._maxHeightSize = FbFlexSize.MinContent; }

        private string MaxHeightString
        {
            get
            {
                if (MaxHeight != null)
                    return MaxHeight;

                switch (_state._maxHeightSize)
                {
                    case FbFlexSize.MaxContent:
                        return "max-content";
                    case FbFlexSize.MinContent:
                        return "min-content";
                    default:
                        return "";
                }
            }
        }

        protected unsafe int BoxHash
        {
            get
            {
                var hash = CpntHash;
                fixed (FbFlexBoxBaseState* sptr = &_state)
                {
                    hash = ComputeHash((byte*)sptr, sizeof(FbFlexBoxBaseState), hash);
                    if (!string.IsNullOrWhiteSpace(MinWidth))
                        hash = ComputeHash(MinWidth, hash);
                    if (!string.IsNullOrWhiteSpace(MinHeight))
                        hash = ComputeHash(MinHeight, hash);
                    if (!string.IsNullOrWhiteSpace(MaxHeight))
                        hash = ComputeHash(MaxHeight, hash);
                    return hash;
                }
            }
        }

        protected new unsafe string ComputedStyle
        {
            get
            {
                var hash = BoxHash;
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

                    RenderingStatistics.AddFlexBoxBaseStyleComputation();

                    var str = $"position: relative;" +
                            (_state._selfAlign == FbFlexItemSelfAlign.None ? "" : $"align-self:{SelfAlignString};") +
                            $"flex-direction: {DirectionString}; flex-wrap: {WrapString}; justify-content: {JustifyString}; align-items: {AlignItemsString}; align-content: {AlignContentString}; " +
                            (PlaceItemsString != "" ? $"place-items: {PlaceItemsString};" : "") +
                            (Gutter != FbSpacing.None ? $"gap:{FbLayoutPresets.ToRem(Gutter)};" : "") +
                            (RowGutter != FbSpacing.None ? $"row-gap:{FbLayoutPresets.ToRem(RowGutter)};" : "") +
                            (ColumnGutter != FbSpacing.None ? $"column-gap:{FbLayoutPresets.ToRem(ColumnGutter)};" : "") +
                            (Grow != int.MinValue ? $"flex-grow:{Grow};" : "") +
                            (Shrink != int.MinValue ? $"flex-shrink:{Shrink};" : "") +
                            (Order != int.MinValue ? $"order:{Order};" : "") +
                            (MinWithString != "" ? $"min-width: {MinWithString};" : "") +
                            (MinHeightString != "" ? $"min-height: {MinHeightString};" : "") +
                            (MaxHeightString != "" ? $"max-height: {MaxHeightString};" : "") +
                            (ColumnDisplay ? "flex-flow : column;" : "") +
                            (Scrollable ? "overflow : scroll;" : "") +
                            (Radius != FbSpacing.None ? $"border-radius:{FbLayoutPresets.ToRem(Radius)};" : "") +
                            ComputedBaseStyle;
                    if (UseCaching && !_cache.ContainsKey(hash))
                        _cache[hash] = str;
                    _style = str;
                    return _style;
                }
            }
        }
        #endregion

        // ************************************************************************************************ //
        // ***************************************    PUBLIC   ******************************************** //
        // ************************************************************************************************ //
        #region PUBLIC PARAMETERS

        /// <summary>
        /// Child content
        /// </summary>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Width master identifier
        /// </summary>
        [Parameter]
        public string WidthMasterId { get; set; }

        /// <summary>
        /// Width slave identifier
        /// </summary>
        [Parameter]
        public string WidthSlaveId { get; set; }

        /// <summary>
        /// Scrollable setting
        /// </summary>
        [Parameter]
        public bool Scrollable { get => _state.Scrollable; set => _state.Scrollable = value; }

        #region GUTTER
        /// <summary>
        /// Gutter -> 0
        /// </summary>
        [Parameter]
        public FbSpacing G { get => Gutter; set => Gutter = value; }

        /// <summary>
        /// Gutter -> Small
        /// </summary>
        [Parameter]
        public bool GS { get => Gutter == FbLayoutPresets.S; set => Gutter = FbLayoutPresets.S; }

        /// <summary>
        /// Gutter -> Medium
        /// </summary>
        [Parameter]
        public bool GM { get => Gutter == FbLayoutPresets.M; set => Gutter = FbLayoutPresets.M; }

        /// <summary>
        /// Gutter -> Large
        /// </summary>
        [Parameter]
        public bool GL { get => Gutter == FbLayoutPresets.L; set => Gutter = FbLayoutPresets.L; }

        /// <summary>
        /// Gutter -> Extra Large
        /// </summary>
        [Parameter]
        public bool GX { get => Gutter == FbLayoutPresets.X; set => Gutter = FbLayoutPresets.X; }
        #endregion

        #endregion

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "style", ComputedStyle);
            builder.AddAttribute(2, "class", Classes);
            builder.AddContent(3, ChildContent);
            builder.CloseElement();
        }
    }
}
