using FractalBlazor.Components.Common.Contracts;

namespace FractalBlazor.WebSite.StateModel
{
    /// <summary>
    /// Bars, horizontal and vertical, filled up to the share a slider sets.
    /// </summary>
    public sealed class LinearProgressDemoState : ObservableState
    {
        public sealed record SetDone(int Value) : IStateAction<int>;

        public int Done { get; private set; } = 62;

        public void Handle(SetDone action)
        {
            if (Done == action.Value)
                return;
            Done = action.Value;
            NotifyStateChanged();
        }
    }

    /// <summary>
    /// Rings of several sizes, filled up to the share a slider sets.
    /// </summary>
    public sealed class CircularProgressDemoState : ObservableState
    {
        public sealed record SetDone(int Value) : IStateAction<int>;

        public int Done { get; private set; } = 62;

        public void Handle(SetDone action)
        {
            if (Done == action.Value)
                return;
            Done = action.Value;
            NotifyStateChanged();
        }
    }

    /// <summary>
    /// A task whose progress cannot be told while it loads: its bars and its ring move until
    /// it is done, then show it full.
    /// </summary>
    public sealed class IndeterminateProgressDemoState : ObservableState
    {
        public sealed record ToggleLoading : IStateAction;

        public bool Loading { get; private set; } = true;

        public void Handle(ToggleLoading action)
        {
            Loading = !Loading;
            NotifyStateChanged();
        }
    }
}
