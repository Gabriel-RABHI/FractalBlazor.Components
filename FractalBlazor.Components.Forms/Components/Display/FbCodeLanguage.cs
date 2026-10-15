namespace FractalBlazor.Components.Forms.Components.Display
{
    /// <summary>
    /// The language an <see cref="FbCodeBlock"/> colors its code in.
    /// </summary>
    public enum FbCodeLanguage : byte
    {
        /// <summary>
        /// Razor: tags, components and their attributes, <c>@</c> transitions and directives;
        /// a line that does not hold markup is read as C#.
        /// </summary>
        Razor,

        /// <summary>
        /// C#: keywords, types, calls, strings, numbers and comments.
        /// </summary>
        CSharp,

        /// <summary>
        /// CSS: selectors, properties, custom properties, functions and values.
        /// </summary>
        Css,

        /// <summary>
        /// Plain text, without colors.
        /// </summary>
        Plain
    }
}
