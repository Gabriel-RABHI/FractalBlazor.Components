using System;
using System.Diagnostics;
using System.Threading.Tasks;
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

        private string MarginSize => ((double)((int)Margin) / 16d).ToString().Replace(",", ".");

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
        /// Margin -> Large
        /// </summary>
        [Parameter]
        public bool MX { get => Margin == FbLayoutPresets.X; set => Margin = FbLayoutPresets.X; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "cpnt", "row-separator");
            builder.AddAttribute(2, "style", $"display: flex; align-self: stretch;{(Margin != FbSpacing.None ? $"margin-left:{MarginSize}rem;margin-right:{MarginSize}rem;" : "")}");
            builder.AddAttribute(3, "class", ComputedRowSeparatorClasses);
            builder.CloseElement();
        }
    }
}
