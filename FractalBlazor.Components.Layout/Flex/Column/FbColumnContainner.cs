using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout
{
    /// <summary>
    /// CSS container-query based column system.
    /// It is a 12 slots based to create a 2 (2 x 6), 3 (3 x 4), 4 (4 * 3) column system.
    /// </summary>
    public class FbColumnContainner : FbComponentBase
    {

        public FbColumnContainner()
        {
        }

        private FbBreaks BreakOn { get; set; } = FbBreaks.None;

        /// <summary>Overrides <see cref="Gap"/> for rows.</summary>
        private FbSpacing RowGap { get; set; } = FbSpacing.None;

        /// <summary>Overrides <see cref="Gap"/> for columns.</summary>
        private FbSpacing ColumnGap { get; set; } = FbSpacing.None;

        private FbFrame Separator { get; set; } = FbFrame.None;

        private FbSpacing SeparatorMargin { get; set; } = FbSpacing.None;

        // ************************************************************************************************ //
        // ***************************************    PUBLIC   ******************************************** //
        // ************************************************************************************************ //
        /// <summary>
        /// Gap -> Row -> Small
        /// </summary>
        [Parameter]
        public bool GRS { get => RowGap == FbSpacing.S; set { if (value) RowGap = FbSpacing.S; } }

        /// <summary>
        /// Gap -> Row -> Medium
        /// </summary>
        [Parameter]
        public bool GRM { get => RowGap == FbSpacing.M; set { if (value) RowGap = FbSpacing.M; } }

        /// <summary>
        /// Gap -> Row -> Large
        /// </summary>
        [Parameter]
        public bool GRL { get => RowGap == FbSpacing.L; set { if (value) RowGap = FbSpacing.L; } }

        /// <summary>
        /// Gap -> Row -> Extra Large
        /// </summary>
        [Parameter]
        public bool GRX { get => RowGap == FbSpacing.X; set { if (value) RowGap = FbSpacing.X; } }

        /// <summary>
        /// Gap -> Column -> Small
        /// </summary>
        [Parameter]
        public bool GCS { get => RowGap == FbSpacing.S; set { if (value) RowGap = FbSpacing.S; } }

        /// <summary>
        /// Gap -> Column -> Medium
        /// </summary>
        [Parameter]
        public bool GCM { get => RowGap == FbSpacing.M; set { if (value) RowGap = FbSpacing.M; } }

        /// <summary>
        /// Gap -> Column -> Large
        /// </summary>
        [Parameter]
        public bool GCL { get => RowGap == FbSpacing.L; set { if (value) RowGap = FbSpacing.L; } }

        /// <summary>
        /// Gap -> Column -> Extra Large
        /// </summary>
        [Parameter]
        public bool GCX { get => RowGap == FbSpacing.X; set { if (value) RowGap = FbSpacing.X; } }

        /// <summary>
        /// Break -> Under -> Extra Small
        /// </summary>
        public bool BU_XS { get => BreakOn == FbBreaks.XS; set { if (value) BreakOn = FbBreaks.XS; } }

        /// <summary>
        /// Break -> Under -> Small
        /// </summary>
        public bool BU_S { get => BreakOn == FbBreaks.S; set { if (value) BreakOn = FbBreaks.S; } }

        /// <summary>
        /// Break -> Under -> Medium
        /// </summary>
        public bool BU_M { get => BreakOn == FbBreaks.M; set { if (value) BreakOn = FbBreaks.M; } }

        /// <summary>
        /// Break -> Under -> Large
        /// </summary>
        public bool BU_L { get => BreakOn == FbBreaks.L; set { if (value) BreakOn = FbBreaks.L; } }

        /// <summary>
        /// Break -> Under -> Extra Large
        /// </summary>
        public bool BU_XL { get => BreakOn == FbBreaks.XL; set { if (value) BreakOn = FbBreaks.XL; } }

        /// <summary>
        /// Break -> Under -> Extra-Extra Large
        /// </summary>
        public bool BU_XXL { get => BreakOn == FbBreaks.XXL; set { if (value) BreakOn = FbBreaks.XXL; } }

        /// <summary>
        /// With -> Separator -> Light
        /// </summary>
        [Parameter]
        public bool WSL { get => Separator == FbFrame.Light; set { if (value) Separator = FbFrame.Light; } }

        /// <summary>
        /// With -> Separator -> Medium
        /// </summary>
        [Parameter]
        public bool WSM { get => Separator == FbFrame.Medium; set { if (value) Separator = FbFrame.Medium; } }

        /// <summary>
        /// With -> Separator -> Strong
        /// </summary>
        [Parameter]
        public bool WSS { get => Separator == FbFrame.Strong; set { if (value) Separator = FbFrame.Strong; } }

        /// <summary>
        /// With -> Separator -> Margin -> Small
        /// </summary>
        [Parameter]
        public bool WSMS { get => SeparatorMargin == FbSpacing.S; set { if (value) SeparatorMargin = FbSpacing.S; } }

        /// <summary>
        /// With -> Separator -> Margin -> Medium
        /// </summary>
        [Parameter]
        public bool WSMM { get => SeparatorMargin == FbSpacing.M; set { if (value) SeparatorMargin = FbSpacing.M; } }

        /// <summary>
        /// With -> Separator -> Margin -> Large
        /// </summary>
        [Parameter]
        public bool WSML { get => SeparatorMargin == FbSpacing.L; set { if (value) SeparatorMargin = FbSpacing.L; } }

        /// <summary>
        /// With -> Separator -> Margin -> Extra Large
        /// </summary>
        [Parameter]
        public bool WSMX { get => SeparatorMargin == FbSpacing.X; set { if (value) SeparatorMargin = FbSpacing.X; } }


        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
        }
    }
}
