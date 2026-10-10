using System.Globalization;
using FractalBlazor.Components.Forms.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Forms.Inputs
{
    /// <summary>
    /// Date field on the native date input: the browser provides the calendar, in the
    /// color scheme of the theme. Clearing the field sends <c>null</c>.
    /// </summary>
    public class FbInputDate<TAction> : FbFieldBase<DateOnly?, TAction>
        where TAction : IStateAction<DateOnly?>
    {
        private const string IsoFormat = "yyyy-MM-dd";

        [Parameter]
        public DateOnly? Min { get; set; }

        [Parameter]
        public DateOnly? Max { get; set; }

        protected override string InputType => "date";

        protected override string Format(DateOnly? value)
            => value?.ToString(IsoFormat, CultureInfo.InvariantCulture) ?? string.Empty;

        protected override bool TryParse(string text, out DateOnly? value)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                value = null;
                return true;
            }

            if (DateOnly.TryParseExact(text, IsoFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                value = date;
                return true;
            }

            value = null;
            return false;
        }

        protected override void AddInputAttributes(RenderTreeBuilder builder)
        {
            builder.AddAttribute(100, "min", Format(Min) is { Length: > 0 } min ? min : null);
            builder.AddAttribute(101, "max", Format(Max) is { Length: > 0 } max ? max : null);
        }
    }
}
