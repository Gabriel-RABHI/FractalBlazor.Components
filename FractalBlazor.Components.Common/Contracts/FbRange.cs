namespace FractalBlazor.Components.Common.Contracts
{
    /// <summary>
    /// Two values that bound a range, the value of a range slider: <see cref="Start"/> is never
    /// above <see cref="End"/> once the slider has sent it.
    /// </summary>
    public readonly record struct FbRange<TValue>(TValue Start, TValue End);
}
