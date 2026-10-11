using FractalBlazor.Components.Common.Contracts;

namespace FractalBlazor.Components.Forms.Inputs
{
    /// <summary>
    /// Numeric field on the native slider. <c>Min</c>, <c>Max</c> and <c>Step</c> set its
    /// track; use <c>Immediate</c> to send the value while the thumb moves.
    /// </summary>
    public class FbInputRange<TValue, TAction> : FbInputNumber<TValue, TAction>
        where TValue : struct, System.Numerics.INumber<TValue>
        where TAction : IStateAction<TValue>
    {
        protected override string InputType => "range";
    }
}
