namespace FractalBlazor.Components.Forms.Contracts
{
    public interface IStateAction<TValue>
    {
    }

    public interface IStateAction
    {
    }

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
