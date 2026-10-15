using FractalBlazor.Components.Forms.Components.Display;
using FractalBlazor.WebSite.StateModel;

namespace FractalBlazor.WebSite.Flags;

/// <summary>
/// The bubbles of the code blocks of the site: what a flag means on the component it is
/// written on, read in the catalog of the flags (<c>JHCC</c> on FbGridContainer: Justify →
/// Horizontal → Content → Center). An attribute that is not a flag, or a component outside the
/// libraries, has none.
/// </summary>
public sealed class FlagCodeGlossary : IFbCodeGlossary
{
    private readonly Dictionary<(string Component, string Flag), string> _meanings = [];

    public FlagCodeGlossary(IFlagCatalog catalog)
    {
        foreach (var flag in catalog.Flags)
        {
            foreach (var component in flag.Components)
                _meanings.TryAdd((component, flag.Name), flag.Meaning);
        }
    }

    public string? Explain(string element, string attribute) => _meanings.GetValueOrDefault((element, attribute));
}
