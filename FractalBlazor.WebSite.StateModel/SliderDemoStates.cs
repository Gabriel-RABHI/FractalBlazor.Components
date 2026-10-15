using FractalBlazor.Components.Common.Contracts;

namespace FractalBlazor.WebSite.StateModel
{
    /// <summary>
    /// Sliders with a labeled scale: a price, a day shown as a date, and a range of hours shown
    /// as times, with a bubble.
    /// </summary>
    public sealed class ScaleSliderDemoState : ObservableState
    {
        public sealed record SetPrice(int Value) : IStateAction<int>;

        public sealed record SetDay(int Value) : IStateAction<int>;

        public sealed record SetHours(FbRange<int> Value) : IStateAction<FbRange<int>>;

        /// <summary>
        /// The first day of the date slider: its value counts the days from it.
        /// </summary>
        public static DateOnly FirstDay { get; } = new(2022, 6, 13);

        public int Price { get; private set; } = 50;

        public int Day { get; private set; } = 4;

        public FbRange<int> Hours { get; private set; } = new(14, 21);

        public DateOnly Date => FirstDay.AddDays(Day);

        public void Handle(SetPrice action)
        {
            if (Price == action.Value)
                return;
            Price = action.Value;
            NotifyStateChanged();
        }

        public void Handle(SetDay action)
        {
            if (Day == action.Value)
                return;
            Day = action.Value;
            NotifyStateChanged();
        }

        public void Handle(SetHours action)
        {
            if (Hours == action.Value)
                return;
            Hours = action.Value;
            NotifyStateChanged();
        }
    }

    /// <summary>
    /// Vertical sliders: a level and a band, with their scale on either side.
    /// </summary>
    public sealed class VerticalSliderDemoState : ObservableState
    {
        public sealed record SetLevel(int Value) : IStateAction<int>;

        public sealed record SetBand(FbRange<int> Value) : IStateAction<FbRange<int>>;

        public int Level { get; private set; } = 64;

        public FbRange<int> Band { get; private set; } = new(25, 75);

        public void Handle(SetLevel action)
        {
            if (Level == action.Value)
                return;
            Level = action.Value;
            NotifyStateChanged();
        }

        public void Handle(SetBand action)
        {
            if (Band == action.Value)
                return;
            Band = action.Value;
            NotifyStateChanged();
        }
    }

    /// <summary>
    /// The marks of the ticks: one level, shown by a slider per mark, horizontal and vertical.
    /// </summary>
    public sealed class TickMarkSliderDemoState : ObservableState
    {
        public sealed record SetLevel(int Value) : IStateAction<int>;

        public int Level { get; private set; } = 3;

        public void Handle(SetLevel action)
        {
            if (Level == action.Value)
                return;
            Level = action.Value;
            NotifyStateChanged();
        }
    }

    /// <summary>
    /// Sliders with dots and values, shown twice, horizontal and vertical, on the same values.
    /// </summary>
    public sealed class DotsSliderDemoState : ObservableState
    {
        public sealed record SetRating(int Value) : IStateAction<int>;

        public sealed record SetSpan(FbRange<int> Value) : IStateAction<FbRange<int>>;

        public sealed record SetStep(int Value) : IStateAction<int>;

        public sealed record SetSteps(FbRange<int> Value) : IStateAction<FbRange<int>>;

        public int Rating { get; private set; } = 7;

        public FbRange<int> Span { get; private set; } = new(10, 90);

        public int Step { get; private set; } = 5;

        public FbRange<int> Steps { get; private set; } = new(1, 8);

        public void Handle(SetRating action)
        {
            if (Rating == action.Value)
                return;
            Rating = action.Value;
            NotifyStateChanged();
        }

        public void Handle(SetSpan action)
        {
            if (Span == action.Value)
                return;
            Span = action.Value;
            NotifyStateChanged();
        }

        public void Handle(SetStep action)
        {
            if (Step == action.Value)
                return;
            Step = action.Value;
            NotifyStateChanged();
        }

        public void Handle(SetSteps action)
        {
            if (Steps == action.Value)
                return;
            Steps = action.Value;
            NotifyStateChanged();
        }
    }
}
