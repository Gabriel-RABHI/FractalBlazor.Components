namespace FractalBlazor.Components.Common.Contracts
{
    /// <summary>
    /// An action sent by a field to its state object. The state handles it with a public
    /// <c>void Handle(TAction action)</c> method, discovered once and compiled.
    /// </summary>
    public interface IStateAction<TValue> : IStateAction
    {
    }

    /// <summary>
    /// Marker of every action, including those without a value (sent by buttons).
    /// </summary>
    public interface IStateAction
    {
    }

    /// <summary>
    /// Generic action carrying a new value. It lets a field be used with <c>@bind-Value</c>
    /// alone (<c>TAction="FbSetValue&lt;string&gt;"</c>), or handled by a state that does not
    /// need a dedicated action type.
    /// </summary>
    public sealed record FbSetValue<TValue>(TValue Value) : IStateAction<TValue>;

    public interface INotifyStateChanged
    {
        event Action? OnStateChanged;
    }

    public abstract class ObservableState : INotifyStateChanged
    {
        public event Action? OnStateChanged;

        // Protected helper so derived classes can easily trigger the UI
        protected void NotifyStateChanged()
        {
            OnStateChanged?.Invoke();
        }
    }
}
