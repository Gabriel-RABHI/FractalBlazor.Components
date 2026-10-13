using System.Globalization;
using System.Numerics;
using FractalBlazor.Components.Common.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Forms.Components.Inputs
{
    /// <summary>
    /// Numeric field on the native number input, for any .NET number type. The browser
    /// exchanges numbers in the invariant culture; the value sent is clamped to
    /// <see cref="Min"/> and <see cref="Max"/>.
    /// </summary>
    public class FbInputNumber<TValue, TAction> : FbFieldBase<TValue, TAction>
        where TValue : struct, INumber<TValue>
        where TAction : IStateAction<TValue>
    {
        [Parameter]
        public TValue? Min { get; set; }

        [Parameter]
        public TValue? Max { get; set; }

        [Parameter]
        public TValue? Step { get; set; }

        protected override string InputType => "number";

        protected override string Format(TValue value) => value.ToString(null, CultureInfo.InvariantCulture);

        protected override bool TryParse(string text, out TValue value)
        {
            if (!TValue.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
                return false;

            if (Min is { } min && value < min)
                value = min;
            if (Max is { } max && value > max)
                value = max;
            return true;
        }

        protected override void AddInputAttributes(RenderTreeBuilder builder)
        {
            builder.AddAttribute(100, "min", Min is { } min ? Format(min) : null);
            builder.AddAttribute(101, "max", Max is { } max ? Format(max) : null);
            builder.AddAttribute(102, "step", Step is { } step ? Format(step) : null);
        }
    }
}
