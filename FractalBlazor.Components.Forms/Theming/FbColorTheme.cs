using FractalBlazor.Components.Layout;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace FractalBlazor.Components.Forms.Theming
{
    internal class FbColorTheme : FbSimpleComponentBase
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

    internal class FbFontTheme : FbSimpleComponentBase
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
        public string MediumWeight { get; set; } = "500";

        [Parameter]
        public string BoldWeight { get; set; } = "700";
    }
}
