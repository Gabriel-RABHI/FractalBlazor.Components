using FractalBlazor.Components.Common.Contracts;

namespace FractalBlazor.Components.Forms.Inputs
{
    /// <summary>
    /// Single-line text field.
    /// </summary>
    public class FbInputText<TAction> : FbFieldBase<string, TAction>
        where TAction : IStateAction<string>
    {
        protected override string InputType => "text";

        protected override string Format(string value) => value ?? string.Empty;

        protected override bool TryParse(string text, out string value)
        {
            value = text;
            return true;
        }
    }
}
