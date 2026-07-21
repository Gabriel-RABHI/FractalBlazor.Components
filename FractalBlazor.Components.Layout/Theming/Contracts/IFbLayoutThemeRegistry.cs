using FractalBlazor.Components.Layout.Theming.Solver;

namespace FractalBlazor.Components.Layout.Theming.Contracts;

public interface IFbLayoutThemeRegistry
{
    FbLayoutThemeSetup Default { get; }

    void Register(FbLayoutThemeSetup theme);

    bool TryGet(string name, out FbLayoutThemeSetup theme);

    FbResolvedLayoutTheme Resolve(string theme);
}
