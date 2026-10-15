using System.Globalization;
using System.Numerics;
using FractalBlazor.Components.Common.Contracts;
using FractalBlazor.Components.Forms.Mapping;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace FractalBlazor.Components.Forms.Components.Inputs
{
    /// <summary>
    /// A slider drawn by the library, for any numeric type: one thumb (<see cref="FbSlider{TValue, TAction}"/>)
    /// or two (<see cref="FbRangeSlider{TValue, TAction}"/>), horizontal or <see cref="Vertical"/>,
    /// with ticks, labels and a value bubble.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The value moves by <see cref="Step"/> between <see cref="Min"/> and <see cref="Max"/>.
    /// The keyboard moves the focused thumb: arrows by a step, Page Up / Page Down by a tenth
    /// of the track, Home / End to the bounds. Each thumb is an ARIA <c>slider</c>.
    /// </para>
    /// <para>
    /// A drag is followed by the script of Layout: it moves the thumb at once, without waiting
    /// for .NET, and sends the value at the pointer. The slider rounds it to the step, then
    /// sends it to the state when the thumb is released, or while it moves with
    /// <see cref="Immediate"/>. Labels are rendered by <see cref="FbDynamicView{TSelector}"/>:
    /// the value, its <see cref="Format"/>, or the item <see cref="LabelOf"/> makes of it.
    /// </para>
    /// </remarks>
    public abstract class FbSliderBase<TValue, TModel, TAction> : FbInputBase<TModel, TAction>, IHandleAfterRender, IAsyncDisposable
        where TValue : struct, INumber<TValue>
        where TAction : IStateAction<TModel>
    {
        private const int MaxTicks = 200;

        private readonly EventCallback<KeyboardEventArgs>[] _keyDown;
        private ElementReference _element;
        private DotNetObjectReference<FbSliderBase<TValue, TModel, TAction>>? _self;
        private IJSObjectReference? _interop;
        private bool _started, _disposed, _sliding;
        private TModel _preview = default!;
        private Flags _flags, _defaultFlags;

        private struct Flags
        {
            public bool Vertical, Mirror, TD, TL, EL, VL, VT, Immediate;
        }

        protected FbSliderBase()
        {
            _keyDown =
            [
                EventCallback.Factory.Create<KeyboardEventArgs>(this, args => KeyDownAsync(0, args)),
                EventCallback.Factory.Create<KeyboardEventArgs>(this, args => KeyDownAsync(1, args))
            ];
        }

        [Inject]
        public IJSRuntime JS { get; set; } = default!;

        /// <summary>
        /// The lowest value, at the start of the track (left, or bottom when vertical).
        /// </summary>
        [Parameter]
        public TValue Min { get; set; } = TValue.Zero;

        /// <summary>
        /// The highest value, at the end of the track.
        /// </summary>
        [Parameter]
        public TValue Max { get; set; } = TValue.CreateChecked(100);

        /// <summary>
        /// The gap between two values the slider can take; zero or less leaves it continuous.
        /// </summary>
        [Parameter]
        public TValue Step { get; set; } = TValue.One;

        /// <summary>
        /// The gap between two ticks, from <see cref="Min"/>; none by default. <see cref="TD"/>
        /// draws them, <see cref="TL"/> labels them.
        /// </summary>
        [Parameter]
        public TValue? Ticks { get; set; }

        /// <summary>
        /// A .NET format for the labels of the values: <c>C0</c>, <c>0.#</c>, <c>P0</c>…
        /// </summary>
        [Parameter]
        public string? Format { get; set; }

        /// <summary>
        /// The item a label shows for a value, rendered by <see cref="FbDynamicView{TSelector}"/>:
        /// a date for a day number, an hour for a quarter… It wins over <see cref="Format"/>.
        /// </summary>
        [Parameter]
        public Func<TValue, object>? LabelOf { get; set; }

        /// <summary>
        /// Accessible name of the thumbs.
        /// </summary>
        [Parameter]
        public string? Title { get; set; }

        /// <summary>
        /// The track runs from bottom to top. The slider takes the height of its parent, 12rem
        /// at least.
        /// </summary>
        [Parameter]
        public bool Vertical { get => _flags.Vertical; set => _flags.Vertical = value; }

        /// <summary>
        /// The labels go on the other side: above the track (vertical: on its left), the values
        /// below it (on its right).
        /// </summary>
        [Parameter]
        public bool Mirror { get => _flags.Mirror; set => _flags.Mirror = value; }

        /// <summary>
        /// Ticks -> Dots: a dot on the track at each tick.
        /// </summary>
        [Parameter]
        public bool TD { get => _flags.TD; set => _flags.TD = value; }

        /// <summary>
        /// Ticks -> Labels: the value of each tick under the track.
        /// </summary>
        [Parameter]
        public bool TL { get => _flags.TL; set => _flags.TL = value; }

        /// <summary>
        /// Ends -> Labels: the values of both ends under the track.
        /// </summary>
        [Parameter]
        public bool EL { get => _flags.EL; set => _flags.EL = value; }

        /// <summary>
        /// Value -> Label: the value of each thumb, always shown above it.
        /// </summary>
        [Parameter]
        public bool VL { get => _flags.VL; set => _flags.VL = value; }

        /// <summary>
        /// Value -> Tooltip: the value of a thumb in a bubble, while it moves, is hovered or has
        /// the focus.
        /// </summary>
        [Parameter]
        public bool VT { get => _flags.VT; set => _flags.VT = value; }

        /// <summary>
        /// Send the value while the thumb moves, instead of when it is released.
        /// </summary>
        [Parameter]
        public bool Immediate { get => _flags.Immediate; set => _flags.Immediate = value; }

        /// <summary>
        /// The number of thumbs.
        /// </summary>
        protected abstract int ThumbCount { get; }

        /// <summary>
        /// The value of a thumb of <paramref name="model"/>, the first being the lowest.
        /// </summary>
        protected abstract TValue ThumbValue(TModel model, int thumb);

        /// <summary>
        /// <paramref name="model"/> with <paramref name="thumb"/> moved to
        /// <paramref name="value"/>, never past the other thumb.
        /// </summary>
        protected abstract TModel WithThumb(TModel model, int thumb, TValue value);

        /// <summary>
        /// The value shown: the one being dragged, or the one of the state.
        /// </summary>
        private TModel Shown => _sliding ? _preview : Value;

        protected override void CaptureParameterDefaults()
        {
            base.CaptureParameterDefaults();
            _defaultFlags = _flags;
        }

        protected override void RestoreParameterDefaults()
        {
            base.RestoreParameterDefaults();
            _flags = _defaultFlags;
        }

        // ------------------------------------------------------------------ Values

        /// <summary>
        /// The nearest value the slider can take: on a step from <see cref="Min"/>, between the
        /// bounds; anywhere between them when the step is not positive. A fractional step keeps
        /// the decimals of the step and of <see cref="Min"/>, so that 0.1 × 3 stays 0.3.
        /// </summary>
        internal TValue Snap(double raw)
        {
            var min = double.CreateChecked(Min);
            var max = double.CreateChecked(Max);
            var step = double.CreateChecked(Step);
            raw = Math.Clamp(raw, Math.Min(min, max), Math.Max(min, max));

            var value = step > 0
                ? Round(Min + TValue.CreateSaturating(Math.Round((raw - min) / step)) * Step, Math.Max(Decimals(Step), Decimals(Min)))
                : TValue.CreateSaturating(raw);
            return Clamp(value);
        }

        private TValue Clamp(TValue value) => TValue.Clamp(value, TValue.Min(Min, Max), TValue.Max(Min, Max));

        /// <summary>
        /// <paramref name="value"/> rounded to <paramref name="decimals"/>, which removes the
        /// error a binary fraction leaves (0.1 + 0.2 gives 0.30000000000000004).
        /// </summary>
        private static TValue Round(TValue value, int decimals)
            => decimals == 0 ? value : TValue.CreateSaturating(Math.Round(double.CreateChecked(value), decimals));

        /// <summary>
        /// The decimals <paramref name="number"/> is written with, 15 at most: 2 for 0.25, 5 for 0.00001.
        /// </summary>
        private static int Decimals(TValue number)
        {
            var text = number.ToString("0.###############", CultureInfo.InvariantCulture);
            var point = text.IndexOf('.');
            return point < 0 ? 0 : text.Length - point - 1;
        }

        /// <summary>
        /// The position of <paramref name="value"/> on the track, in percent.
        /// </summary>
        private double Percent(TValue value)
        {
            var span = double.CreateChecked(Max) - double.CreateChecked(Min);
            return span <= 0 ? 0 : Math.Clamp((double.CreateChecked(value) - double.CreateChecked(Min)) / span * 100, 0, 100);
        }

        private static string Css(double percent) => percent.ToString("0.###", CultureInfo.InvariantCulture) + "%";

        private static string Invariant(TValue value) => value.ToString(null, CultureInfo.InvariantCulture);

        /// <summary>
        /// A tenth of the track, on a step: what Page Up and Page Down move.
        /// </summary>
        private TValue LargeStep
        {
            get
            {
                var tenth = Snap(double.CreateChecked(Min) + (double.CreateChecked(Max) - double.CreateChecked(Min)) / 10) - Min;
                return TValue.Max(TValue.Abs(tenth), TValue.Abs(Step));
            }
        }

        /// <summary>
        /// The item a label shows for <paramref name="value"/>.
        /// </summary>
        internal object LabelItem(TValue value)
            => LabelOf?.Invoke(value) ?? (Format is null ? value : value.ToString(Format, CultureInfo.CurrentCulture));

        /// <summary>
        /// The ticks, from <see cref="Min"/> by <see cref="Ticks"/>, <see cref="MaxTicks"/> at most.
        /// Each one is computed from its index and rounded, not added to the one before, so that
        /// a fractional gap does not drift: ten ticks of 0.1 end on 1, not on 0.9999999999999999.
        /// </summary>
        internal IEnumerable<TValue> TickValues()
        {
            if (Ticks is not { } gap || gap <= TValue.Zero || Max <= Min)
                yield break;

            var decimals = Math.Max(Decimals(gap), Decimals(Min));
            for (var index = 0; index < MaxTicks; index++)
            {
                var value = Round(Min + TValue.CreateSaturating(index) * gap, decimals);
                if (value > Max)
                    yield break;
                yield return value;
            }
        }

        // ------------------------------------------------------------------ Changes

        /// <summary>
        /// Called by the script while a thumb is dragged, with the value at the pointer, and
        /// once more, <paramref name="done"/>, when it is released.
        /// </summary>
        [JSInvokable]
        public async Task OnSlide(int thumb, double value, bool done)
        {
            if (_disposed || IsDisabled || thumb < 0 || thumb >= ThumbCount)
                return;

            var model = WithThumb(Value, thumb, Snap(value));
            if (done)
            {
                _sliding = false;
                if (!EqualityComparer<TModel>.Default.Equals(model, Value))
                    await CommitAsync(model);
                Render();
                return;
            }

            if (Immediate)
            {
                if (!EqualityComparer<TModel>.Default.Equals(model, Value))
                    await CommitAsync(model);
                return;
            }

            (_sliding, _preview) = (true, model);
            Render();
        }

        private async Task KeyDownAsync(int thumb, KeyboardEventArgs args)
        {
            if (IsDisabled || thumb >= ThumbCount)
                return;

            var current = ThumbValue(Value, thumb);
            TValue? target = args.Key switch
            {
                "ArrowRight" or "ArrowUp" => current + TValue.Abs(Step),
                "ArrowLeft" or "ArrowDown" => current - TValue.Abs(Step),
                "PageUp" => current + LargeStep,
                "PageDown" => current - LargeStep,
                "Home" => TValue.Min(Min, Max),
                "End" => TValue.Max(Min, Max),
                _ => null
            };
            if (target is not { } value)
                return;

            var model = WithThumb(Value, thumb, Snap(double.CreateChecked(value)));
            if (!EqualityComparer<TModel>.Default.Equals(model, Value))
                await CommitAsync(model);
        }

        // ------------------------------------------------------------------ Rendering

        private string RootClassNames
        {
            get
            {
                var classes = "fb-slider";
                if (_flags.Vertical) classes += " fb-slider-vertical";
                if (ThumbCount > 1) classes += " fb-slider-range";
                if (_flags.Mirror) classes += " fb-slider-mirror";
                if (_flags.TD) classes += " fb-slider-dots";
                if (_flags.TL || _flags.EL) classes += " fb-slider-scale";
                if (_flags.VL) classes += " fb-slider-values";
                if (IsDisabled) classes += " fb-slider-disabled";
                return string.IsNullOrWhiteSpace(Classes) ? classes : classes + " " + Classes.Trim();
            }
        }

        /// <summary>
        /// The track: a line, the filled part between the thumbs (from the start for a single
        /// thumb), the ticks and the thumbs, placed by the custom properties of the root. The
        /// script places a dragged thumb on the rail, over them, until the state follows.
        /// </summary>
        protected override void BuildControl(RenderTreeBuilder builder)
        {
            var model = Shown;
            var from = ThumbCount > 1 ? Percent(ThumbValue(model, 0)) : 0;
            var to = Percent(ThumbValue(model, ThumbCount - 1));

            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", RootClasses(RootClassNames));
            builder.AddAttribute(2, "style", $"--fb-slider-from:{Css(from)};--fb-slider-to:{Css(to)};");
            builder.AddAttribute(3, "data-min", Invariant(Min));
            builder.AddAttribute(4, "data-max", Invariant(Max));
            builder.AddAttribute(5, "data-step", Invariant(Step));
            builder.AddElementReferenceCapture(6, element => _element = element);

            builder.OpenElement(7, "div");
            builder.AddAttribute(8, "class", "fb-slider-rail");

            builder.OpenElement(9, "div");
            builder.AddAttribute(10, "class", "fb-slider-track");
            builder.CloseElement();
            builder.OpenElement(11, "div");
            builder.AddAttribute(12, "class", "fb-slider-fill");
            builder.CloseElement();

            builder.OpenRegion(13);
            BuildTicks(builder, from, to);
            builder.CloseRegion();

            for (var thumb = 0; thumb < ThumbCount; thumb++)
            {
                builder.OpenRegion(14 + thumb);
                BuildThumb(builder, model, thumb);
                builder.CloseRegion();
            }

            builder.CloseElement();
            builder.CloseElement();
        }

        private void BuildTicks(RenderTreeBuilder builder, double from, double to)
        {
            var ticks = TickValues().ToList();
            var ends = _flags.EL && !(_flags.TL && ticks.Count > 0);
            if (ends)
            {
                Tick(builder, Min, 0, from, to, label: true);
                Tick(builder, Max, 100, from, to, label: true);
            }

            foreach (var tick in ticks)
                Tick(builder, tick, Percent(tick), from, to, label: _flags.TL);
        }

        private void Tick(RenderTreeBuilder builder, TValue value, double at, double from, double to, bool label)
        {
            var classes = "fb-slider-tick";
            if (at >= from - 0.0001 && at <= to + 0.0001) classes += " fb-slider-tick-in";
            if (at <= 0.0001) classes += " fb-slider-tick-start";
            else if (at >= 99.9999) classes += " fb-slider-tick-end";

            builder.OpenElement(0, "span");
            builder.AddAttribute(1, "class", classes);
            builder.AddAttribute(2, "style", $"--fb-slider-at:{Css(at)};");
            if (label)
                Label(builder, 3, "fb-slider-label", value);
            builder.CloseElement();
        }

        private void BuildThumb(RenderTreeBuilder builder, TModel model, int thumb)
        {
            var value = ThumbValue(model, thumb);
            var position = ThumbCount > 1 && thumb == 0 ? "from" : "to";

            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "fb-slider-thumb fb-slider-thumb-" + position);
            builder.AddAttribute(2, "data-position", position);
            builder.AddAttribute(3, "role", "slider");
            builder.AddAttribute(4, "tabindex", IsDisabled ? "-1" : "0");
            builder.AddAttribute(5, "aria-label", Title);
            builder.AddAttribute(6, "aria-orientation", _flags.Vertical ? "vertical" : "horizontal");
            builder.AddAttribute(7, "aria-valuemin", Invariant(ThumbCount > 1 && thumb == 1 ? ThumbValue(model, 0) : Min));
            builder.AddAttribute(8, "aria-valuemax", Invariant(ThumbCount > 1 && thumb == 0 ? ThumbValue(model, 1) : Max));
            builder.AddAttribute(9, "aria-valuenow", Invariant(value));
            builder.AddAttribute(10, "aria-valuetext", FbValueView.TextOf(LabelItem(value)));
            builder.AddAttribute(11, "aria-disabled", IsDisabled ? "true" : null);
            builder.AddAttribute(12, "data-fb-nav-keys", true);
            builder.AddAttribute(13, "onkeydown", _keyDown[thumb]);
            if (_flags.VL)
                Label(builder, 14, "fb-slider-value", value);
            if (_flags.VT)
                Label(builder, 15, "fb-slider-bubble", value);
            builder.CloseElement();
        }

        private void Label(RenderTreeBuilder builder, int sequence, string kind, TValue value)
        {
            builder.OpenElement(sequence, "span");
            builder.AddAttribute(sequence + 1, "class", kind);
            builder.OpenComponent<FbDynamicView<FbViewSelector>>(sequence + 2);
            builder.AddComponentParameter(sequence + 3, nameof(FbDynamicView<FbViewSelector>.Item), LabelItem(value));
            builder.AddComponentParameter(sequence + 4, nameof(FbDynamicView<FbViewSelector>.Selector), FbViewSelector.Item);
            builder.CloseComponent();
            builder.CloseElement();
        }

        // ------------------------------------------------------------------ Script

        /// <summary>
        /// Once in the page, the slider hands its element to the script, which follows the drags.
        /// </summary>
        async Task IHandleAfterRender.OnAfterRenderAsync()
        {
            if (_started || _disposed)
                return;

            _started = true;
            _self = DotNetObjectReference.Create(this);
            try
            {
                _interop = await JS.InvokeAsync<IJSObjectReference>("FractalBlazor.initSlider", _element, _self);
            }
            catch (JSDisconnectedException)
            {
                // The circuit is gone: there is nothing left to drag.
            }
        }

        public async ValueTask DisposeAsync()
        {
            _disposed = true;
            if (_interop is not null)
            {
                try
                {
                    await _interop.InvokeVoidAsync("dispose");
                    await _interop.DisposeAsync();
                }
                catch (JSDisconnectedException)
                {
                    // The circuit is gone: the browser side no longer exists.
                }
                _interop = null;
            }
            _self?.Dispose();
        }
    }
}
