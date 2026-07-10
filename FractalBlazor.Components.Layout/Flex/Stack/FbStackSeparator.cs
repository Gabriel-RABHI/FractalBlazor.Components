using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout
{
    public class FbStackSeparator : FbComponentBase
    {
        #region HIDDEN
        private FbFrame _frame = FbFrame.Medium;

        private string MarginSize => ((double)((int)Margin) / 16d).ToString().Replace(",", ".");

        // -------------- FbStackSeparator Frame ------------ //
        protected FbFrame Frame { get => _frame; set => _frame = value; }

        protected string ComputedStackSeparatorClasses
        {
            get
            {
                return $"{FrameClass}";
            }
        }

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
        protected FbSpacing Margin {
            get; set;
        }
        #endregion

        // ************************************************************************************************ //
        // ***************************************    PUBLIC   ******************************************** //
        // ************************************************************************************************ //
        /// <summary>
        /// Frame -> Small
        /// </summary>
        [Parameter]
        public bool WFS { get => Frame == FbFrame.Small; set => Frame = FbFrame.Small; }

        /// <summary>
        /// Frame -> Medium
        /// </summary>
        [Parameter]
        public bool WFM { get => Frame == FbFrame.Medium; set => Frame = FbFrame.Medium; }

        /// <summary>
        /// Frame -> Large
        /// </summary>
        [Parameter]
        public bool WFL { get => Frame == FbFrame.Large; set => Frame = FbFrame.Large; }


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
        /// Margin -> Extra-Large
        /// </summary>
        [Parameter]
        public bool MX { get => Margin == FbLayoutPresets.X; set => Margin = FbLayoutPresets.X; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "cpnt", "stack-separator");
            builder.AddAttribute(2, "style", $"display: flex; align-self: stretch;{(Margin != FbSpacing.None ? $"margin-top:{MarginSize}rem;margin-bottom:{MarginSize}rem;" : "")}");
            builder.AddAttribute(3, "class", ComputedStackSeparatorClasses);
            builder.CloseElement();
        }
    }
}
