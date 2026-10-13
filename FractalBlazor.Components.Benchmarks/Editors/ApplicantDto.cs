using FractalBlazor.Components.Common.Contracts;

namespace FractalBlazor.Components.Benchmarks.Editors
{
    public enum ApplicantStatus
    {
        Pending,
        Review,
        Admitted,
        Fail
    }

    /// <summary>
    /// The edited object. MudBlazor binds its properties; FractalBlazor sends it the actions
    /// below, which it handles by itself (state-first).
    /// </summary>
    public sealed class ApplicantDto
    {
        public sealed record SetFirstName(string Value) : IStateAction<string>;
        public sealed record SetLastName(string Value) : IStateAction<string>;
        public sealed record SetCity(string Value) : IStateAction<string>;
        public sealed record SetStatus(ApplicantStatus Value) : IStateAction<ApplicantStatus>;
        public sealed record SetInterviewed(bool Value) : IStateAction<bool>;

        private static readonly string[] FirstNames = ["Ada", "Alan", "Grace", "Linus", "Margaret", "Dennis", "Barbara", "Ken", "Frances", "Edsger"];
        private static readonly string[] LastNames = ["Lovelace", "Turing", "Hopper", "Torvalds", "Hamilton", "Ritchie", "Liskov", "Thompson", "Allen", "Dijkstra"];
        private static readonly string[] Cities = ["Paris", "Lyon", "Marseille", "Toulouse", "Nantes", "Lille", "Bordeaux", "Strasbourg"];

        public static IReadOnlyList<ApplicantStatus> Statuses { get; } = Enum.GetValues<ApplicantStatus>();

        public string FirstName { get; set; } = "";

        public string LastName { get; set; } = "";

        public string City { get; set; } = "";

        public DateTime BirthDate { get; set; }

        public ApplicantStatus Status { get; set; }

        public bool Interviewed { get; set; }

        public void Handle(SetFirstName action) => FirstName = action.Value;

        public void Handle(SetLastName action) => LastName = action.Value;

        public void Handle(SetCity action) => City = action.Value;

        public void Handle(SetStatus action) => Status = action.Value;

        public void Handle(SetInterviewed action) => Interviewed = action.Value;

        /// <summary>
        /// The same <paramref name="count"/> applicants at each call.
        /// </summary>
        public static IReadOnlyList<ApplicantDto> Generate(int count)
        {
            var random = new Random(42);
            var applicants = new ApplicantDto[count];
            for (var i = 0; i < count; i++)
            {
                applicants[i] = new ApplicantDto
                {
                    FirstName = FirstNames[random.Next(FirstNames.Length)],
                    LastName = LastNames[random.Next(LastNames.Length)],
                    City = Cities[random.Next(Cities.Length)],
                    BirthDate = new DateTime(1960, 1, 1).AddDays(random.Next(365 * 45)),
                    Status = Statuses[random.Next(Statuses.Count)],
                    Interviewed = random.Next(2) == 1
                };
            }
            return applicants;
        }
    }
}
