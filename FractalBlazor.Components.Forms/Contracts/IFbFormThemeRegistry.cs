using FractalBlazor.Components.Forms.Theming.Model;
using FractalBlazor.Components.Forms.Theming.Solver;

namespace FractalBlazor.Components.Forms.Contracts;

public interface IFbFormThemeRegistry
{
    FbThemeSetup Default { get; }

    void Register(FbThemeSetup theme);

    bool TryGet(string name, out FbThemeSetup theme);

    FbResolvedTheme Resolve(string theme, string branch);

    /// <summary>
    /// Resolves a theme that is not registered, for previews and theme editors. The theme must
    /// inherit from a registered theme; it is validated like a registration but neither stored
    /// nor cached.
    /// </summary>
    FbResolvedTheme Resolve(FbThemeSetup theme, string branch);
}
