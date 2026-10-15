using System.Numerics;
using FractalBlazor.Components.Common.Contracts;

namespace FractalBlazor.Components.Forms.Components.Inputs
{
    /// <summary>
    /// A slider with one thumb: the track fills from its start to the value.
    /// </summary>
    /// <example><c>&lt;FbSlider TValue="int" TAction="FbSetValue&lt;int&gt;" @bind-Value="price" Max="100" Ticks="25" TL Format="C0" /&gt;</c></example>
    public class FbSlider<TValue, TAction> : FbSliderBase<TValue, TValue, TAction>
        where TValue : struct, INumber<TValue>
        where TAction : IStateAction<TValue>
    {
        protected override int ThumbCount => 1;

        protected override TValue ThumbValue(TValue model, int thumb) => model;

        protected override TValue WithThumb(TValue model, int thumb, TValue value) => value;
    }

    /// <summary>
    /// A slider with two thumbs, the bounds of a range: the track fills between them, and a
    /// thumb never passes the other one.
    /// </summary>
    /// <example><c>&lt;FbRangeSlider TValue="int" TAction="FbSetValue&lt;FbRange&lt;int&gt;&gt;" @bind-Value="hours" Min="11" Max="23" VT /&gt;</c></example>
    public class FbRangeSlider<TValue, TAction> : FbSliderBase<TValue, FbRange<TValue>, TAction>
        where TValue : struct, INumber<TValue>
        where TAction : IStateAction<FbRange<TValue>>
    {
        protected override int ThumbCount => 2;

        /// <summary>
        /// A range given upside down reads in order.
        /// </summary>
        protected override TValue ThumbValue(FbRange<TValue> model, int thumb)
            => thumb == 0 ? TValue.Min(model.Start, model.End) : TValue.Max(model.Start, model.End);

        protected override FbRange<TValue> WithThumb(FbRange<TValue> model, int thumb, TValue value)
        {
            var (start, end) = (ThumbValue(model, 0), ThumbValue(model, 1));
            return thumb == 0 ? new(TValue.Min(value, end), end) : new(start, TValue.Max(value, start));
        }
    }
}
