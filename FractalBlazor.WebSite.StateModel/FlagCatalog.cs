namespace FractalBlazor.WebSite.StateModel
{
    /// <summary>
    /// One part of a flag name: its letters, what they stand for, and the kind of choice it
    /// belongs to. <c>PHS</c> reads as Padding, then Horizontal (a direction), then Small (a
    /// spacing).
    /// </summary>
    public sealed record FlagToken(string Code, string Meaning, string Category);

    /// <summary>
    /// A flag: its name, the parts it reads as, and the components that accept it.
    /// </summary>
    public sealed record FlagInfo(string Name, IReadOnlyList<FlagToken> Tokens, IReadOnlyList<string> Components)
    {
        /// <summary>
        /// The meanings of the parts, in order: <c>Padding → Horizontal → Small</c>.
        /// </summary>
        public string Meaning => string.Join(" → ", Tokens.Select(token => token.Meaning));
    }

    /// <summary>
    /// The flags of the libraries. The host reads them on the components and implements
    /// this interface: the state model knows no component.
    /// </summary>
    public interface IFlagCatalog
    {
        IReadOnlyList<FlagInfo> Flags { get; }

        /// <summary>
        /// Every component that accepts at least one flag, by name.
        /// </summary>
        IReadOnlyList<string> Components { get; }
    }
}
