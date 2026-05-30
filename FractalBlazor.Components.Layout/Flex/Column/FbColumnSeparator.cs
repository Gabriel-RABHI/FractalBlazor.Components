using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout
{
    public class FbColumnSeparator : FbSimpleComponentBase
    {
        #region HIDDEN
        private FbFrame _frame = FbFrame.Medium;

        private string MarginSize => ((double)((int)Margin) / 16d).ToString().Replace(",", ".");

        protected string ComputedColumnSeparatorClasses
        {
            get
            {
                return $"{FrameClass}";
            }
        }

        // -------------- FbColumnSeparator Frame ------------ //
        protected FbFrame Frame { get => _frame; set => _frame = value; }
        #endregion

        // ************************************************************************************************ //
        // ***************************************    PUBLIC   ******************************************** //
        // ************************************************************************************************ //

        /// <summary>
        /// Frame -> Small
        /// </summary>
        [Parameter]
        public bool WSFrame { get => Frame == FbFrame.Small; set => Frame = FbFrame.Small; }

        /// <summary>
        /// Frame -> Medium
        /// </summary>
        [Parameter]
        public bool WMFrame { get => Frame == FbFrame.Medium; set => Frame = FbFrame.Medium; }

        /// <summary>
        /// Frame -> Large
        /// </summary>
        [Parameter]
        public bool WLFrame { get => Frame == FbFrame.Large; set => Frame = FbFrame.Large; }

        private string FrameClass
        {
            get
            {
                string baseClasse = "fb-separator";

                if (_frame is FbFrame.Small)
                    return $"{baseClasse}-small-frame";
                else if (_frame is FbFrame.Medium)
                    return $"{baseClasse}-medium-frame";
                else if (_frame is FbFrame.Large)
                    return $"{baseClasse}-large-frame";
                else
                    return "";
            }
        }

        /// <summary>
        /// Margin size
        /// </summary>
        [Parameter]
        public FbMargin Margin { get; set; }

        /// <summary>
        /// Margin -> Small
        /// </summary>
        [Parameter]
        public bool MS { get => Margin == FbPresets.S_Margin; set => Margin = FbPresets.S_Margin; }

        /// <summary>
        /// Margin -> Medium
        /// </summary>
        [Parameter]
        public bool MM { get => Margin == FbPresets.M_Margin; set => Margin = FbPresets.M_Margin; }

        /// <summary>
        /// Margin -> Large
        /// </summary>
        [Parameter]
        public bool ML { get => Margin == FbPresets.L_Margin; set => Margin = FbPresets.L_Margin; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "cpnt", "column-separator");
            builder.AddAttribute(2, "style", $"display: flex; align-self: stretch;{(Margin != FbMargin.None ? $"margin-left:{MarginSize}rem;margin-right:{MarginSize}rem;" : "")}");
            builder.AddAttribute(3, "class", ComputedColumnSeparatorClasses);
            builder.CloseElement();
        }
    }
}
