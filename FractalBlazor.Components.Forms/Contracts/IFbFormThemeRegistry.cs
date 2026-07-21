using FractalBlazor.Components.Forms.Theming.Model;
using FractalBlazor.Components.Forms.Theming.Solver;

namespace FractalBlazor.Components.Forms.Contracts;

public interface IFbFormThemeRegistry
{
    FbThemeSetup Default { get; }

    void Register(FbThemeSetup theme);

    bool TryGet(string name, out FbThemeSetup theme);

    FbResolvedTheme Resolve(string theme, string branch);
}
