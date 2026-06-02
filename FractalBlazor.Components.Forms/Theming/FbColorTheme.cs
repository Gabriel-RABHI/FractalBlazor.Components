using FractalBlazor.Components.Layout;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace FractalBlazor.Components.Forms.Theming
{
    public class FbColorTheme : FbComponentBase
    {
        [Parameter]
        public string Color { get; set; } = "#D8D8D8";

        [Parameter]
        public string PrimaryColor { get; set; } = "#4050D8";

        // -------- Text
        [Parameter]
        public string TextColor { get; set; } = "D8D8D8";

        [Parameter]
        public string TextMutedColor { get; set; } = "#4050D8";

        [Parameter]
        public string TextAccentColor { get; set; } = "4050D8";

        [Parameter]
        public string TextPrimaryColor { get; set; } = "4050D8";

        [Parameter]
        public string TextErrorColor { get; set; } = "4050D8";
    }

    public class FbFontTheme : FbComponentBase
    {
        // -------- Default
        [Parameter]
        public string FontSizeBase { get; set; } = "1.4";

        [Parameter]
        public string FontWeight { get; set; } = "400";

        [Parameter]
        public string LineHeight { get; set; } = "1.4";

        // -------- Sizes
        [Parameter]
        public string SmallCoef { get; set; } = "0.85";

        [Parameter]
        public string MediumCoef { get; set; } = "1";

        [Parameter]
        public string LargeCoef { get; set; } = "1.25";

        [Parameter]
        public string ExtraLargeCoef { get; set; } = "1.6";

        // -------- Weights
        [Parameter]
        public string ThinWeight { get; set; } = "300";

        [Parameter]
        public string BoldWeight { get; set; } = "500";

        [Parameter]
        public string ExtraBoldWeight { get; set; } = "700";
    }
}
