using FractalBlazor.Components.Common.Contracts;

namespace FractalBlazor.Demo.States
{
    /// <summary>
    /// A contact form held entirely by its state. Fields send actions, the state validates
    /// and notifies, and every view of the state re-renders.
    /// </summary>
    public sealed class ContactState : ObservableState
    {
        public sealed record SetName(string Value) : IStateAction<string>;
        public sealed record SetAge(int Value) : IStateAction<int>;
        public sealed record SetBirth(DateOnly? Value) : IStateAction<DateOnly?>;
        public sealed record SetCountry(string Value) : IStateAction<string>;
        public sealed record SetNewsletter(bool Value) : IStateAction<bool>;
        public sealed record SetPremium(bool Value) : IStateAction<bool>;
        public sealed record SetPreferredContact(object Value) : IStateAction<object>;
        public sealed record Reset : IStateAction;

        public static IReadOnlyList<string> Countries { get; } = ["France", "Japon", "Canada", "Brésil"];

        public const string NoPreference = "Pas de préférence";

        /// <summary>
        /// A polymorphic list: one string, the channels, and people.
        /// </summary>
        public static IReadOnlyList<object> ContactOptions { get; } =
        [
            NoPreference,
            ContactChannel.Email,
            ContactChannel.Phone,
            ContactChannel.Sms,
            new ContactPerson("Ada Lovelace", "Assistante"),
            new ContactPerson("Alan Turing", "Responsable technique")
        ];

        public string Name { get; private set; } = "";

        public int Age { get; private set; } = 30;

        public DateOnly? Birth { get; private set; }

        public string? Country { get; private set; }

        public bool Newsletter { get; private set; }

        public bool Premium { get; private set; }

        /// <summary>
        /// A string, a channel or a person: the select renders each one with its own view.
        /// </summary>
        public object PreferredContact { get; private set; } = NoPreference;

        public bool NameIsValid => Name.Trim().Length >= 2;

        public bool IsComplete => NameIsValid && Country is not null;

        public void Handle(SetName action) => Update(() => Name = action.Value);

        public void Handle(SetAge action) => Update(() => Age = action.Value);

        public void Handle(SetBirth action) => Update(() => Birth = action.Value);

        public void Handle(SetCountry action) => Update(() => Country = action.Value);

        public void Handle(SetNewsletter action) => Update(() => Newsletter = action.Value);

        public void Handle(SetPremium action) => Update(() => Premium = action.Value);

        public void Handle(SetPreferredContact action) => Update(() => PreferredContact = action.Value);

        public void Handle(Reset action) => Update(() =>
        {
            Name = "";
            Age = 30;
            Birth = null;
            Country = null;
            Newsletter = false;
            Premium = false;
            PreferredContact = NoPreference;
        });

        private void Update(Action change)
        {
            change();
            NotifyStateChanged();
        }
    }
}

namespace FractalBlazor.Demo.States
{
    public enum ContactChannel
    {
        [System.ComponentModel.DataAnnotations.Display(Name = "Courriel")]
        Email,
        [System.ComponentModel.DataAnnotations.Display(Name = "Téléphone")]
        Phone,
        [System.ComponentModel.DataAnnotations.Display(Name = "SMS")]
        Sms
    }

    public sealed record ContactPerson(string Name, string Role);
}
