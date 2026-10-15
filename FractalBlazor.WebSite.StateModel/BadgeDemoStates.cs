using FractalBlazor.Components.Common.Contracts;

namespace FractalBlazor.WebSite.StateModel
{
    /// <summary>
    /// A mailbox and a presence: the count of unread messages and a status, shown by badges.
    /// </summary>
    public sealed class BadgeDemoState : ObservableState
    {
        public sealed record Receive : IStateAction;

        public sealed record ReadAll : IStateAction;

        public sealed record ToggleOnline : IStateAction;

        public int Unread { get; private set; } = 3;

        public bool Online { get; private set; } = true;

        public void Handle(Receive action)
        {
            Unread++;
            NotifyStateChanged();
        }

        public void Handle(ReadAll action)
        {
            if (Unread == 0)
                return;
            Unread = 0;
            NotifyStateChanged();
        }

        public void Handle(ToggleOnline action)
        {
            Online = !Online;
            NotifyStateChanged();
        }
    }
}
