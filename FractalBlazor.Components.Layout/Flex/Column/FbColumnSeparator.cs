using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout
{
    public class FbColumnSeparator : FbComponentBase
    {
        #region HIDDEN
        private FbFrame _frame = FbFrame.Medium;

        protected string ComputedColumnSeparatorClasses
        {
            get
            {
                return $"{FrameClass}";
            }
        }

        // -------------- FbColumnSeparator Frame ------------ //
        protected FbFrame Frame { get => _frame; set => _frame = value; }


        /// <summary>
        /// Margin size
        /// </summary>
        [Parameter]
        public FbSpacing Margin { get; set; }

        #endregion

        // ************************************************************************************************ //
        // ***************************************    PUBLIC   ******************************************** //
        // ************************************************************************************************ //

        /// <summary>
        /// Frame -> Light
        /// </summary>
        [Parameter]
        public bool WLFrame { get => Frame == FbFrame.Light; set { if (value) Frame = FbFrame.Light; } }

        /// <summary>
        /// Frame -> Medium
        /// </summary>
        [Parameter]
        public bool WMFrame { get => Frame == FbFrame.Medium; set { if (value) Frame = FbFrame.Medium; } }

        /// <summary>
        /// Frame -> Strong
        /// </summary>
        [Parameter]
        public bool WSFrame { get => Frame == FbFrame.Strong; set { if (value) Frame = FbFrame.Strong; } }

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

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "cpnt", "column-separator");
            builder.AddAttribute(2, "style", $"display: flex; align-self: stretch;{(Margin != FbSpacing.None ? $"margin-left:{FbLayoutHelper.ToSpacingCss(Margin)};margin-right:{FbLayoutHelper.ToSpacingCss(Margin)};" : "")}");
            builder.AddAttribute(3, "class", ComputedColumnSeparatorClasses);
            builder.CloseElement();
        }
    }
}
