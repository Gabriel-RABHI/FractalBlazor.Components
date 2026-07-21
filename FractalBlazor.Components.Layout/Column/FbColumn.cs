using System;
using System.Threading.Tasks;
using FractalBlazor.Components.Layout.Abstracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout.Column
{
    /// <summary>
    /// FbColumnContainner child column. Can hold a FbStack, a FbGridContainner or any usefull layout or any component.
    /// </summary>
    public class FbColumn : FbComponentBase
    {
        public FbColumn()
        {
        }

        private int RowSpan { get; set; } = 1;

        [Parameter]
        public bool Half { get => RowSpan == 6; set { if (value) RowSpan = 6; } }

        [Parameter]
        public bool Third { get => RowSpan == 4; set { if (value) RowSpan = 4; } }

        [Parameter]
        public bool Quarter { get => RowSpan == 3; set { if (value) RowSpan = 3; } }

        [Parameter]
        public bool Sixt { get => RowSpan == 2; set { if (value) RowSpan = 2; } }

        [Parameter]
        public bool C1 { get => RowSpan == 1; set { if (value) RowSpan = 1; } }

        [Parameter]
        public bool C2 { get => RowSpan == 2; set { if (value) RowSpan = 2; } }

        [Parameter]
        public bool C3 { get => RowSpan == 3; set { if (value) RowSpan = 3; } }

        [Parameter]
        public bool C4 { get => RowSpan == 4; set { if (value) RowSpan = 4; } }

        [Parameter]
        public bool C5 { get => RowSpan == 5; set { if (value) RowSpan = 5; } }

        [Parameter]
        public bool C6 { get => RowSpan == 6; set { if (value) RowSpan = 6; } }

        [Parameter]
        public bool C7 { get => RowSpan == 7; set { if (value) RowSpan = 7; } }

        [Parameter]
        public bool C8 { get => RowSpan == 8; set { if (value) RowSpan = 8; } }

        [Parameter]
        public bool C9 { get => RowSpan == 9; set { if (value) RowSpan = 9; } }

        [Parameter]
        public bool C10 { get => RowSpan == 10; set { if (value) RowSpan = 10; } }

        [Parameter]
        public bool C11 { get => RowSpan == 11; set { if (value) RowSpan = 11; } }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
        }
    }
}
