namespace FractalBlazor.Components.Forms.Components.Display
{
    /// <summary>
    /// Says what the attributes of the code in an <see cref="FbCodeBlock"/> mean: the text of
    /// the bubble shown over an attribute. An application registers one in its services to give
    /// every code block its bubbles; without one, the attributes have none.
    /// </summary>
    /// <example><c>services.AddSingleton&lt;IFbCodeGlossary, MyGlossary&gt;();</c></example>
    public interface IFbCodeGlossary
    {
        /// <summary>
        /// What <paramref name="attribute"/> means on <paramref name="element"/>, both as written
        /// in the code (<c>FbGridContainer</c>, <c>JHCC</c>); <c>null</c> when there is nothing
        /// to say.
        /// </summary>
        string? Explain(string element, string attribute);
    }
}
