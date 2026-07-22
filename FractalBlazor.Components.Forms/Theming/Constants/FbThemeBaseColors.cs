using System;
using System.Collections.Generic;
using System.Text;

namespace FractalBlazor.Components.Forms.Theming.Constants
{
    public enum FbThemeBaseColorsIndex : int
    {
        Red,
        Orange,
        Amber,
        Yellow,
        Lime,
        Green,
        Emerald,
        Teal,
        Cyan,
        Sky,
        Blue,
        Indigo,
        Violet,
        Purple,
        Fuchsia,
        Pink,
        Rose,
        // ------ Backgrounds
        Slate,
        Gray,
        Zinc,
        Neutral,
        Stone,
        Taupe,
        Mauve,
        Mist,
        Olive
    }

    public enum FbThemeBaseShadesIndex : int
    {
        _950,
        _900,
        _800,
        _700,
        _600,
        _500,
        _400,
        _300,
        _200,
        _100,
        _50
    }

    public static class FbThemeBaseColors
    {
        public static readonly string[,] FbBaseColors = new string[,]
        {
            // 950        900        800        700        600        500        400        300        200        100        50
            // Red
            { "#460809", "#82181A", "#9F0712", "#C10007", "#E7000B", "#FB2C36", "#FF6467", "#FFA2A2", "#FFC9C9", "#FFE2E2", "#FEF2F2" },
            // Orange
            { "#441306", "#7E2A0C", "#9F2D00", "#CA3500", "#F54900", "#FF6900", "#FF8904", "#FFB86A", "#FFD6A7", "#FFEDD4", "#FFF7ED" },
            // Amber
            { "#461901", "#7B3306", "#973C00", "#BB4D00", "#E17100", "#FE9A00", "#FFB900", "#FFD230", "#FEE685", "#FEF3C6", "#FFFBEB" },
            // Yellow
            { "#432004", "#733E0A", "#894B00", "#A65F00", "#D08700", "#F0B100", "#FDC700", "#FFDF20", "#FFF085", "#FEF9C2", "#FEFCE8" },
            // Lime
            { "#192E03", "#35530E", "#3C6300", "#497D00", "#5EA500", "#7CCF00", "#9AE600", "#BBF451", "#D8F999", "#ECFCCA", "#F7FEE7" },
            // Green
            { "#032E15", "#0D542B", "#016630", "#008236", "#00A63E", "#00C950", "#05DF72", "#7BF1A8", "#B9F8CF", "#DCFCE7", "#F0FDF4" },
            // Emerald
            { "#002C22", "#004F3B", "#006045", "#007A55", "#009966", "#00BC7D", "#00D492", "#5EE9B5", "#A4F4CF", "#D0FAE5", "#ECFDF5" },
            // Teal
            { "#022F2E", "#0B4F4A", "#005F5A", "#00786F", "#009689", "#00BBA7", "#00D5BE", "#46ECD5", "#96F7E4", "#CBFBF1", "#F0FDFA" },
            // Cyan
            { "#053345", "#104E64", "#005F78", "#007595", "#0092B8", "#00B8DB", "#00D3F2", "#53EAFD", "#A2F4FD", "#CEFAFE", "#ECFEFF" },
            // Sky
            { "#052F4A", "#024A70", "#00598A", "#0069A8", "#0084D1", "#00A6F4", "#00BCFF", "#74D4FF", "#B8E6FE", "#DFF2FE", "#F0F9FF" },
            // Blue
            { "#162456", "#1C398E", "#193CB8", "#1447E6", "#155DFC", "#2B7FFF", "#51A2FF", "#8EC5FF", "#BEDBFF", "#DBEAFE", "#EFF6FF" },
            // Indigo
            { "#1E1A4D", "#312C85", "#372AAC", "#432DD7", "#4F39F6", "#615FFF", "#7C86FF", "#A3B3FF", "#C6D2FF", "#E0E7FF", "#EEF2FF" },
            // Violet
            { "#2F0D68", "#4D179A", "#5D0EC0", "#7008E7", "#7F22FE", "#8E51FF", "#A684FF", "#C4B4FF", "#DDD6FF", "#EDE9FE", "#F5F3FF" },
            // Purple
            { "#3C0366", "#59168B", "#6E11B0", "#8200DB", "#9810FA", "#AD46FF", "#C27AFF", "#DAB2FF", "#E9D4FF", "#F3E8FF", "#FAF5FF" },
            // Fuchsia
            { "#4B004F", "#721378", "#8A0194", "#A800B7", "#C800DE", "#E12AFB", "#ED6AFF", "#F4A8FF", "#F6CFFF", "#FAE8FF", "#FDF4FF" },
            // Pink
            { "#510424", "#861043", "#A3004C", "#C6005C", "#E60076", "#F6339A", "#FB64B6", "#FDA5D5", "#FCCEE8", "#FCE7F3", "#FDF2F8" },
            // Rose
            { "#4D0218", "#8B0836", "#A50036", "#C70036", "#EC003F", "#FF2056", "#FF637E", "#FFA1AD", "#FFCCD3", "#FFE4E6", "#FFF1F2" },
            // Slate
            { "#020618", "#0F172B", "#1D293D", "#314158", "#45556C", "#62748E", "#90A1B9", "#CAD5E2", "#E2E8F0", "#F1F5F9", "#F8FAFC" },
            // Gray
            { "#030712", "#101828", "#1E2939", "#364153", "#4A5565", "#6A7282", "#99A1AF", "#D1D5DC", "#E5E7EB", "#F3F4F6", "#F9FAFB" },
            // Zinc
            { "#09090B", "#18181B", "#27272A", "#3F3F46", "#52525C", "#71717B", "#9F9FA9", "#D4D4D8", "#E4E4E7", "#F4F4F5", "#FAFAFA" },
            // Neutral
            { "#0A0A0A", "#171717", "#262626", "#404040", "#525252", "#737373", "#A1A1A1", "#D4D4D4", "#E5E5E5", "#F5F5F5", "#FAFAFA" },
            // Stone
            { "#0C0A09", "#1C1917", "#292524", "#44403B", "#57534D", "#79716B", "#A6A09B", "#D6D3D1", "#E7E5E4", "#F5F5F4", "#FAFAF9" },
            // Taupe
            { "#0C0A09", "#1D1816", "#2B2422", "#473C39", "#5B4F4B", "#7C6D67", "#ABA09C", "#D8D2D0", "#E8E4E3", "#F3F1F1", "#FBFAF9" },
            // Mauve
            { "#0C090C", "#1D161E", "#2A212C", "#463947", "#594C5B", "#79697B", "#A89EA9", "#D7D0D7", "#E7E4E7", "#F3F1F3", "#FAFAFA" },
            // Mist
            { "#090B0C", "#161B1D", "#22292B", "#394447", "#4B585B", "#67787C", "#9CA8AB", "#D0D6D8", "#E3E7E8", "#F1F3F3", "#F9FBFB" },
            // Olive
            { "#0C0C09", "#1D1D16", "#2B2B22", "#474739", "#5B5B4B", "#7C7C67", "#ABAB9C", "#D8D8D0", "#E8E8E3", "#F4F4F0", "#FBFBF9" }
        };

        public static string GetColor(FbThemeBaseColorsIndex color, FbThemeBaseShadesIndex shade = FbThemeBaseShadesIndex._500)
            => FbBaseColors[(int)color, (int)shade];

        /// <summary>
        /// Return color with corrections.
        /// </summary>
        /// <param name="color">Reference of the color</param>
        /// <param name="shade">Level (shade)</param>
        /// <param name="gamma">Gamma contract curvature (0 = no contrast)</param>
        /// <param name="tintColor">A color tint that is mixed with the found color</param>
        /// <param name="tintCoef">The tint mix level : 0 = plain source color, 1 = returned color is the tint of the tint color, with the same luminosity</param>
        /// <returns></returns>
        public static string GetColor(FbThemeBaseColorsIndex color, FbThemeBaseShadesIndex shade, double gamma, string tintColor, double tintCoef)
        {
            var sourceColor = GetColor(color, shade);
            // Compute the corrected color :
            // - First, the gamma is applied to color luminosity.
            // - Then, the tintColor is applyed : the color luminosity do not change, the tint only is changed, mixed with tintCoef 
            //   If the tintColor is simply a gray color, it desaturate the result color.
            return sourceColor;
        }

        public static double GetGamma(double x, double curvatur = 1)
        {
            x = Math.Min(1, Math.Max(0, x));
            return ((1 - x) * (x * x * curvatur)) + (x * (1 - ((1 - x) * (1 - x) * curvatur)));
        }

        public static string HtmlView {
            get {
                var builder = new StringBuilder();

                int rows = FbBaseColors.GetLength(0);
                int columns = FbBaseColors.GetLength(1);

                builder.Append(
                    $"<div style=\"display:grid;" +
                    $"grid-template-columns:repeat({columns},8px);" +
                    $"grid-template-rows:repeat({rows},8px);" +
                    $"gap:0;" +
                    $"width:{columns * 8}px;" +
                    $"height:{rows * 8}px;" +
                    $"line-height:0;\">");

                for (int row = 0; row < rows; row++)
                {
                    for (int column = 0; column < columns; column++)
                    {
                        string color = FbBaseColors[row, column];

                        builder.Append(
                            $"<div style=\"width:8px;height:8px;" +
                            $"background-color:{color};\"></div>");
                    }
                }

                builder.Append("</div>");

                return builder.ToString();
            }
        }
    }
}
