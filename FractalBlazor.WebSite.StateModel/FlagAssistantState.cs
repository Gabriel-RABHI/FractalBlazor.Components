using FractalBlazor.Components.Common.Contracts;

namespace FractalBlazor.WebSite.StateModel
{
    /// <summary>
    /// The flag assistant, opened from the header in a drawer. The flag field filters the
    /// flags whose name starts with it. Its letters can also be chosen part by part: the
    /// first column lists how a flag can start, and each choice opens the next column, the
    /// parts that can follow, grouped by kind. The component search, a full name or a part of
    /// it, narrows the flags to those components; choosing a component puts its name there.
    /// </summary>
    /// <remarks>
    /// Typed letters can read several ways: <c>PL</c> is Padding → Large, and the start of
    /// Padding → Left → Small. Both parts are then selected, until more letters decide.
    /// </remarks>
    public sealed class FlagAssistantState : PopupState
    {
        private readonly IFlagCatalog _catalog;
        private IReadOnlyList<FlagInfo> _pool = [];

        public sealed record SetQuery(string Value) : IStateAction<string>;

        public sealed record SetComponentQuery(string Value) : IStateAction<string>;

        /// <summary>
        /// Chooses a part in a column: the field then spells the parts before it, and this
        /// one. Choosing a selected part again removes it.
        /// </summary>
        public sealed record ChooseToken(int Column, FlagToken Token) : IStateAction;

        public sealed record ChooseFlag(string Name) : IStateAction;

        /// <summary>
        /// Puts a component in the search, which then keeps its flags only; choosing it
        /// again clears the search.
        /// </summary>
        public sealed record ChooseComponent(string Name) : IStateAction;

        public sealed record ClearQuery : IStateAction;

        public sealed record ClearComponentQuery : IStateAction;

        public FlagAssistantState(IFlagCatalog catalog)
        {
            _catalog = catalog;
            Update();
        }

        /// <summary>
        /// The flag field, as typed: letters are compared without case.
        /// </summary>
        public string Query { get; private set; } = "";

        public string ComponentQuery { get; private set; } = "";

        /// <summary>
        /// The flags whose name starts with <see cref="Query"/>, among those of the
        /// components searched, in the order of the catalog.
        /// </summary>
        public IReadOnlyList<FlagInfo> Matches { get; private set; } = [];

        /// <summary>
        /// The columns of parts: the first one, then one more after each selected part.
        /// </summary>
        public IReadOnlyList<FlagColumn> Columns { get; private set; } = [];

        /// <summary>
        /// The components searched that accept one of <see cref="Matches"/>.
        /// </summary>
        public IReadOnlyList<string> Components { get; private set; } = [];

        public void Handle(SetQuery action) => Change((action.Value ?? "").Trim(), ComponentQuery);

        public void Handle(SetComponentQuery action) => Change(Query, action.Value ?? "");

        public void Handle(ChooseFlag action) => Change(action.Name, ComponentQuery);

        public void Handle(ClearQuery action) => Change("", ComponentQuery);

        public void Handle(ClearComponentQuery action) => Change(Query, "");

        public void Handle(ChooseComponent action)
            => Change(Query, string.Equals(ComponentQuery.Trim(), action.Name, StringComparison.OrdinalIgnoreCase) ? "" : action.Name);

        public void Handle(ChooseToken action)
        {
            var column = action.Column;
            var flag = _pool.FirstOrDefault(f => f.Tokens.Count > column && f.Tokens[column] == action.Token && Covers(f, column, Query));
            if (flag is null)
                return;

            var before = string.Concat(flag.Tokens.Take(column).Select(token => token.Code));
            var selected = column < Columns.Count
                && Columns[column].Groups.Any(group => group.Choices.Any(choice => choice.Token == action.Token && choice.State == FlagChoiceState.Selected));
            Change(selected ? before : before + action.Token.Code, ComponentQuery);
        }

        private void Change(string query, string componentQuery)
        {
            if (query == Query && componentQuery == ComponentQuery)
                return;

            Query = query;
            ComponentQuery = componentQuery;
            Update();
            NotifyStateChanged();
        }

        private void Update()
        {
            var components = SearchedComponents();
            var searched = components.ToHashSet(StringComparer.Ordinal);
            _pool = ComponentQuery.Trim().Length == 0
                ? _catalog.Flags
                : _catalog.Flags.Where(flag => flag.Components.Any(searched.Contains)).ToList();
            Matches = _pool.Where(IsMatch).ToList();
            Components = Query.Length == 0
                ? components
                : components.Where(component => Matches.Any(flag => flag.Components.Contains(component))).ToList();
            Columns = BuildColumns();
        }

        /// <summary>
        /// The components named by the search: the one it names exactly, else those whose
        /// name contains it, else all of them.
        /// </summary>
        private IReadOnlyList<string> SearchedComponents()
        {
            var search = ComponentQuery.Trim();
            if (search.Length == 0)
                return _catalog.Components;

            var exact = _catalog.Components.FirstOrDefault(component => string.Equals(component, search, StringComparison.OrdinalIgnoreCase));
            return exact is not null
                ? [exact]
                : _catalog.Components.Where(component => component.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        private List<FlagColumn> BuildColumns()
        {
            // When nothing matches, the parts the letters spell stay selected: the columns show
            // where the name went astray.
            var anyMatch = Matches.Count > 0;
            var columns = new List<FlagColumn>();
            for (var index = 0; ; index++)
            {
                var choices = new List<FlagChoice>();
                foreach (var flag in _pool)
                {
                    if (flag.Tokens.Count <= index || !Covers(flag, index, Query))
                        continue;

                    var matches = IsMatch(flag);
                    var state = Covers(flag, index + 1, Query) && (matches || !anyMatch) ? FlagChoiceState.Selected
                        : matches ? FlagChoiceState.Possible
                        : FlagChoiceState.Other;
                    var token = flag.Tokens[index];
                    var at = choices.FindIndex(choice => choice.Token == token);
                    if (at < 0)
                        choices.Add(new FlagChoice(token, state));
                    else if (state < choices[at].State)
                        choices[at] = choices[at] with { State = state };
                }

                if (choices.Count == 0)
                    break;

                columns.Add(new FlagColumn(index, choices
                    .GroupBy(choice => choice.Token.Category)
                    .Select(group => new FlagChoiceGroup(group.Key, group.ToList()))
                    .ToList()));
                if (!choices.Any(choice => choice.State == FlagChoiceState.Selected))
                    break;
            }
            return columns;
        }

        private bool IsMatch(FlagInfo flag) => flag.Name.StartsWith(Query, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Whether the first <paramref name="count"/> parts of <paramref name="flag"/> spell
        /// the start of <paramref name="query"/>.
        /// </summary>
        private static bool Covers(FlagInfo flag, int count, string query)
        {
            var length = 0;
            for (var i = 0; i < count; i++)
            {
                var code = flag.Tokens[i].Code;
                if (length + code.Length > query.Length
                    || string.Compare(query, length, code, 0, code.Length, StringComparison.OrdinalIgnoreCase) != 0)
                    return false;
                length += code.Length;
            }
            return true;
        }
    }

    /// <summary>
    /// How a part relates to the flag field: spelled by it, still possible, or leading to
    /// no flag that starts with it. Ordered from the strongest.
    /// </summary>
    public enum FlagChoiceState
    {
        Selected,
        Possible,
        Other
    }

    public sealed record FlagChoice(FlagToken Token, FlagChoiceState State);

    public sealed record FlagChoiceGroup(string Category, IReadOnlyList<FlagChoice> Choices);

    /// <summary>
    /// The parts that can come at position <see cref="Index"/>, grouped by kind.
    /// </summary>
    public sealed record FlagColumn(int Index, IReadOnlyList<FlagChoiceGroup> Groups);
}
