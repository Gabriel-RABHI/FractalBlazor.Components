using System;
using System.Diagnostics;
using System.Threading.Tasks;
using FractalBlazor.Components.Layout.Abstracts;
using FractalBlazor.Components.Layout.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout
{
    public class FbRowSeparator : FbComponentBase
    {
        #region HIDDEN
        private FbFrame _frame = FbFrame.Medium;

        protected FbFrame Frame { get => _frame; set => _frame = value; }

        private string FrameClass
        {
            get
            {
                string baseClasse = "fb-separator";

                if (_frame is FbFrame.Light)
                    return $"{baseClasse}-light-frame";
                else if (_frame is FbFrame.Medium)
                    return $"{baseClasse}-medium-frame";
                else if (_frame is FbFrame.Strong)
                    return $"{baseClasse}-strong-frame";
                else
                    return "";
            }
        }

        protected string ComputedRowSeparatorClasses
        {
            get
            {
                return $"{FrameClass}";
            }
        }

        /// <summary>
        /// Margin size
        /// </summary>
        protected FbSpacing Margin { get; set; }
        #endregion

        // ************************************************************************************************ //
        // ***************************************    PUBLIC   ******************************************** //
        // ************************************************************************************************ //
        // -------------- FbRowSeparator Frame ------------ //

        /// <summary>
        /// Frame -> Light
        /// </summary>
        [Parameter]
        public bool WFL { get => Frame == FbFrame.Light; set { if (value) Frame = FbFrame.Light; } }

        /// <summary>
        /// Frame -> Medium
        /// </summary>
        [Parameter]
        public bool WFM { get => Frame == FbFrame.Medium; set { if (value) Frame = FbFrame.Medium; } }

        /// <summary>
        /// Frame -> Strong
        /// </summary>
        [Parameter]
        public bool WFS { get => Frame == FbFrame.Strong; set { if (value) Frame = FbFrame.Strong; } }

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
        /// Margin -> Large
        /// </summary>
        [Parameter]
        public bool MX { get => Margin == FbSpacing.X; set { if (value) Margin = FbSpacing.X; } }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "cpnt", "row-separator");
            builder.AddAttribute(2, "style", $"display: flex; align-self: stretch;{(Margin != FbSpacing.None ? $"margin-left:{FbLayoutHelper.ToSpacingCss(Margin)};margin-right:{FbLayoutHelper.ToSpacingCss(Margin)};" : "")}");
            builder.AddAttribute(3, "class", ComputedRowSeparatorClasses);
            builder.CloseElement();
        }
    }
}
