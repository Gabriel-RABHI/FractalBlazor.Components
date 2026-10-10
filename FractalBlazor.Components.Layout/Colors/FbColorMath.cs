using System.Globalization;

namespace FractalBlazor.Components.Layout.Colors
{
    /// <summary>
    /// Color arithmetic that mirrors the stylesheets: <c>color-mix(in oklab, …)</c> and the WCAG
    /// contrast ratio. It lets C# know the colors the browser will actually paint.
    /// </summary>
    public static class FbColorMath
    {
        /// <summary>
        /// Same result as <c>color-mix(in oklab, <paramref name="from"/>, <paramref name="to"/> <paramref name="percent"/>%)</c>.
        /// </summary>
        public static string Mix(string from, string to, double percent)
        {
            var a = ToOklab(ParseHex(from));
            var b = ToOklab(ParseHex(to));
            var t = Math.Clamp(percent, 0d, 100d) / 100d;
            var mixed = new Oklab(
                a.L + ((b.L - a.L) * t),
                a.A + ((b.A - a.A) * t),
                a.B + ((b.B - a.B) * t));
            return ToHex(FromOklab(mixed));
        }

        /// <summary>
        /// WCAG 2.2 contrast ratio between two colors, from 1 to 21.
        /// </summary>
        public static double ContrastRatio(string first, string second)
        {
            var l1 = RelativeLuminance(ParseHex(first));
            var l2 = RelativeLuminance(ParseHex(second));
            return (Math.Max(l1, l2) + 0.05d) / (Math.Min(l1, l2) + 0.05d);
        }

        /// <summary>
        /// Parses a CSS percentage such as <c>8%</c> or <c>12.5%</c>.
        /// </summary>
        public static bool TryParsePercent(string? value, out double percent)
        {
            percent = 0d;
            if (string.IsNullOrWhiteSpace(value))
                return false;
            var text = value.Trim();
            if (text.EndsWith('%'))
                text = text[..^1];
            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out percent);
        }

        /// <summary>
        /// True for the <c>#RGB</c> and <c>#RRGGBB</c> forms that this class can compute with.
        /// </summary>
        public static bool IsHexColor(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;
            var hex = value.Trim().TrimStart('#');
            return (hex.Length is 3 or 6) && hex.All(Uri.IsHexDigit);
        }

        private readonly record struct LinearRgb(double R, double G, double B);

        private readonly record struct Oklab(double L, double A, double B);

        private static LinearRgb ParseHex(string value)
        {
            if (!IsHexColor(value))
                throw new FormatException($"'{value}' is not a #RGB or #RRGGBB color.");

            var hex = value.Trim().TrimStart('#');
            if (hex.Length == 3)
                hex = string.Concat(hex[0], hex[0], hex[1], hex[1], hex[2], hex[2]);

            return new LinearRgb(
                ToLinear(Convert.ToByte(hex[..2], 16) / 255d),
                ToLinear(Convert.ToByte(hex.Substring(2, 2), 16) / 255d),
                ToLinear(Convert.ToByte(hex.Substring(4, 2), 16) / 255d));
        }

        private static string ToHex(LinearRgb color)
            => string.Create(CultureInfo.InvariantCulture,
                $"#{ToByte(color.R):X2}{ToByte(color.G):X2}{ToByte(color.B):X2}");

        private static byte ToByte(double linear)
            => (byte)Math.Round(Math.Clamp(ToGamma(linear), 0d, 1d) * 255d, MidpointRounding.AwayFromZero);

        // sRGB transfer functions, as used by both CSS Color 4 and WCAG.
        private static double ToLinear(double channel)
            => channel <= 0.04045d ? channel / 12.92d : Math.Pow((channel + 0.055d) / 1.055d, 2.4d);

        private static double ToGamma(double channel)
            => channel <= 0.0031308d ? channel * 12.92d : (1.055d * Math.Pow(channel, 1d / 2.4d)) - 0.055d;

        private static double RelativeLuminance(LinearRgb color)
            => (0.2126d * color.R) + (0.7152d * color.G) + (0.0722d * color.B);

        // Oklab matrices from Björn Ottosson's reference implementation.
        private static Oklab ToOklab(LinearRgb c)
        {
            var l = Math.Cbrt((0.4122214708d * c.R) + (0.5363325363d * c.G) + (0.0514459929d * c.B));
            var m = Math.Cbrt((0.2119034982d * c.R) + (0.6806995451d * c.G) + (0.1073969566d * c.B));
            var s = Math.Cbrt((0.0883024619d * c.R) + (0.2817188376d * c.G) + (0.6299787005d * c.B));

            return new Oklab(
                (0.2104542553d * l) + (0.7936177850d * m) - (0.0040720468d * s),
                (1.9779984951d * l) - (2.4285922050d * m) + (0.4505937099d * s),
                (0.0259040371d * l) + (0.7827717662d * m) - (0.8086757660d * s));
        }

        private static LinearRgb FromOklab(Oklab c)
        {
            var l = c.L + (0.3963377774d * c.A) + (0.2158037573d * c.B);
            var m = c.L - (0.1055613458d * c.A) - (0.0638541728d * c.B);
            var s = c.L - (0.0894841775d * c.A) - (1.2914855480d * c.B);
            l *= l * l;
            m *= m * m;
            s *= s * s;

            return new LinearRgb(
                (4.0767416621d * l) - (3.3077115913d * m) + (0.2309699292d * s),
                (-1.2684380046d * l) + (2.6097574011d * m) - (0.3413193965d * s),
                (-0.0041960863d * l) - (0.7034186147d * m) + (1.7076147010d * s));
        }
    }
}
