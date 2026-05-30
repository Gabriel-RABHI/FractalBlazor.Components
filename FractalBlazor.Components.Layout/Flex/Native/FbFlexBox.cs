using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout
{
    public class FbFlexBox : FbFlexBoxBase
    {
        /// <summary>
        /// Wrap
        /// </summary>
        [Parameter]
        public new FbFlexWrap Wrap { get => base.Wrap; set => base.Wrap = value; }

        /// <summary>
        /// No Wrap
        /// </summary>
        [Parameter]
        public bool NoWrap { get => base.Wrap == FbFlexWrap.NoWrap; set => base.Wrap = FbFlexWrap.NoWrap; }

        /// <summary>
        /// Do Wrap
        /// </summary>
        [Parameter]
        public bool DoWrap { get => base.Wrap == FbFlexWrap.Wrap; set => base.Wrap = FbFlexWrap.Wrap; }

        /// <summary>
        /// Wrap Reverse
        /// </summary>
        [Parameter]
        public bool WrapReverse { get => base.Wrap == FbFlexWrap.WrapReverse; set => base.Wrap = FbFlexWrap.WrapReverse; }

        // -------------- FbFlexDirection ------------ //
        /// <summary>
        /// Direction
        /// </summary>
        [Parameter]
        public new FbFlexDirection Direction { get => base.Direction; set => base.Direction = value; }
        
        /// <summary>
        /// Direction Row
        /// </summary>
        [Parameter]
        public bool DirectionRow { get => base.Direction == FbFlexDirection.Row; set => base.Direction = FbFlexDirection.Row; }
        
        /// <summary>
        /// Direction Row Reverse
        /// </summary>
        [Parameter]
        public bool DirectionRowReverse { get => base.Direction == FbFlexDirection.RowReverse; set => base.Direction = FbFlexDirection.RowReverse; }
        
        /// <summary>
        /// Direction Column
        /// </summary>
        [Parameter]
        public bool DirectionColumn { get => base.Direction == FbFlexDirection.Column; set => base.Direction = FbFlexDirection.Column; }
        
        /// <summary>
        /// Direction Column Reverse
        /// </summary>
        [Parameter]
        public bool DirectionColumnReverse { get => base.Direction == FbFlexDirection.ColumnReverse; set => base.Direction = FbFlexDirection.ColumnReverse; }

        // -------------- FbFlexJustify ------------ //
        /// <summary>
        /// Justify
        /// </summary>
        [Parameter]
        public new FbFlexJustify Justify { get => base.Justify; set => base.Justify = value; }

        /// <summary>
        /// Justify Start
        /// </summary>
        [Parameter]
        public bool JustifyStart { get => base.Justify == FbFlexJustify.Start; set => base.Justify = FbFlexJustify.Start; }

        /// <summary>
        /// Justify End
        /// </summary>
        [Parameter]
        public bool JustifyEnd { get => base.Justify == FbFlexJustify.End; set => base.Justify = FbFlexJustify.End; }

        /// <summary>
        /// Justify Center
        /// </summary>
        [Parameter]
        public bool JustifyCenter { get => base.Justify == FbFlexJustify.Center; set => base.Justify = FbFlexJustify.Center; }

        /// <summary>
        /// Justify Stretch
        /// </summary>
        [Parameter]
        public bool JustifyStretch { get => base.Justify == FbFlexJustify.Stretch; set => base.Justify = FbFlexJustify.Stretch; }

        /// <summary>
        /// Justify Space Betwen
        /// </summary>
        [Parameter]
        public bool JustifySpaceBetwen { get => base.Justify == FbFlexJustify.SpaceBetween; set => base.Justify = FbFlexJustify.SpaceBetween; }

        /// <summary>
        /// Justify Space Around
        /// </summary>
        [Parameter]
        public bool JustifySpaceAround { get => base.Justify == FbFlexJustify.SpaceAround; set => base.Justify = FbFlexJustify.SpaceAround; }

        /// <summary>
        /// Justify Evenly
        /// </summary>
        [Parameter]
        public bool JustifyEvenly { get => base.Justify == FbFlexJustify.Evenly; set => base.Justify = FbFlexJustify.Evenly; }

        // -------------- FbFlexAlignItems ------------ //
        /// <summary>
        /// Align Items
        /// </summary>
        [Parameter]
        public new FbFlexAlignItems AlignItems { get => base.AlignItems; set => base.AlignItems = value; }

        /// <summary>
        /// Align Items Start
        /// </summary>
        [Parameter]
        public bool AlignItemsStart { get => base.AlignItems == FbFlexAlignItems.Start; set => base.AlignItems = FbFlexAlignItems.Start; }

        /// <summary>
        /// Align Items End
        /// </summary>
        [Parameter]
        public bool AlignItemsEnd { get => base.AlignItems == FbFlexAlignItems.End; set => base.AlignItems = FbFlexAlignItems.End; }

        /// <summary>
        /// Align Items Center
        /// </summary>
        [Parameter]
        public bool AlignItemsCenter { get => base.AlignItems == FbFlexAlignItems.Center; set => base.AlignItems = FbFlexAlignItems.Center; }

        /// <summary>
        /// Align Items Baseline
        /// </summary>
        [Parameter]
        public bool AlignItemsBaseline { get => base.AlignItems == FbFlexAlignItems.Baseline; set => base.AlignItems = FbFlexAlignItems.Baseline; }

        /// <summary>
        /// Align Items Stretch
        /// </summary>
        [Parameter]
        public bool AlignItemsStretch { get => base.AlignItems == FbFlexAlignItems.Stretch; set => base.AlignItems = FbFlexAlignItems.Stretch; }

        // -------------- FbFlexAlignContent ------------ //
        /// <summary>
        /// Align Content
        /// </summary>
        [Parameter]
        public new FbFlexAlignContent AlignContent { get => base.AlignContent; set => base.AlignContent = value; }

        /// <summary>
        /// Align Content Start
        /// </summary>
        [Parameter]
        public bool AlignContentStart { get => base.AlignContent == FbFlexAlignContent.Start; set => base.AlignContent = FbFlexAlignContent.Start; }

        /// <summary>
        /// Align Content End
        /// </summary>
        [Parameter]
        public bool AlignContentEnd { get => base.AlignContent == FbFlexAlignContent.End; set => base.AlignContent = FbFlexAlignContent.End; }

        /// <summary>
        /// Align Content Center
        /// </summary>
        [Parameter]
        public bool AlignContentCenter { get => base.AlignContent == FbFlexAlignContent.Center; set => base.AlignContent = FbFlexAlignContent.Center; }

        /// <summary>
        /// Align Content Space Betwen
        /// </summary>
        [Parameter]
        public bool AlignContentSpaceBetwen { get => base.AlignContent == FbFlexAlignContent.SpaceBetween; set => base.AlignContent = FbFlexAlignContent.SpaceBetween; }

        /// <summary>
        /// Align Content Space Around
        /// </summary>
        [Parameter]
        public bool AlignContentSpaceAround { get => base.AlignContent == FbFlexAlignContent.SpaceAround; set => base.AlignContent = FbFlexAlignContent.SpaceAround; }

        /// <summary>
        /// Align Content Stretch
        /// </summary>
        [Parameter]
        public bool AlignContentStretch { get => base.AlignContent == FbFlexAlignContent.Stretch; set => base.AlignContent = FbFlexAlignContent.Stretch; }

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
