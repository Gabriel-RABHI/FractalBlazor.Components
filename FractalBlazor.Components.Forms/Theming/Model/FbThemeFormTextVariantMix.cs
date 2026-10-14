namespace FractalBlazor.Components.Forms.Theming.Model;

/// <summary>
/// Positions of the texts on the gradient, relative to the neutral of their block and
/// measured from the end of the gradient: a text sits at
/// <c>100 − (100 − mix) × (100 − neutral) / 100</c> percent. At a neutral of 0 %, it sits at
/// its mix; the higher the neutral, the closer to the high anchor.
/// </summary>
public sealed class FbThemeFormTextVariantMix
{
    public string? DefaultHighMix { get; init; }

    public string? SubtleHighMix { get; init; }

    public string? MutedHighMix { get; init; }

    public string? HighlightHighMix { get; init; }
}
