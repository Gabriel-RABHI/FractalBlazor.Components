using FractalBlazor.Components.Common.Contracts;

namespace FractalBlazor.Components.Forms.Inputs
{
    /// <summary>
    /// Color field on the native color picker. Values are <c>#RRGGBB</c> strings; the field
    /// shows and sends them in lowercase, the form the browser uses.
    /// </summary>
    public class FbInputColor<TAction> : FbFieldBase<string, TAction>
        where TAction : IStateAction<string>
    {
        protected override string InputType => "color";

        protected override string Format(string value) => TryNormalize(value, out var color) ? color : "#000000";

        protected override bool TryParse(string text, out string value) => TryNormalize(text, out value);

        /// <summary>
        /// Accepts <c>#RGB</c> and <c>#RRGGBB</c>, with or without the hash, in any case.
        /// </summary>
        private static bool TryNormalize(string? text, out string color)
        {
            color = "";
            var hex = text?.Trim().TrimStart('#') ?? "";
            if (hex.Length == 3)
                hex = string.Concat(hex[0], hex[0], hex[1], hex[1], hex[2], hex[2]);
            if (hex.Length != 6 || !hex.All(Uri.IsHexDigit))
                return false;

            color = "#" + hex.ToLowerInvariant();
            return true;
        }
    }
}
