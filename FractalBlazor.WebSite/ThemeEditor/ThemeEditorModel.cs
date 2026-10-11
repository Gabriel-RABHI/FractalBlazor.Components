using System.Globalization;
using System.Text;
using FractalBlazor.Components.Forms.Contracts;
using FractalBlazor.Components.Forms.Theming.Constants;
using FractalBlazor.Components.Forms.Theming.Model;
using FractalBlazor.Components.Forms.Theming.Solver;
using FractalBlazor.Components.Layout.Colors;
using FractalBlazor.Components.Layout.Theming.Model;

namespace FractalBlazor.WebSite.ThemeEditor
{
    /// <summary>
    /// Values edited for one branch of the default theme. Every change bumps
    /// <see cref="Version"/>; <see cref="Setup"/> is rebuilt only then, so the preview theme is
    /// resolved again only when something was really edited.
    /// </summary>
    public sealed class ThemeEditorModel
    {
        private readonly IFbFormThemeRegistry _registry;
        private FbResolvedTheme _original = default!;
        private FbThemeSetup? _setup;
        private int _setupVersion = -1;

        public ThemeEditorModel(IFbFormThemeRegistry registry)
        {
            _registry = registry;
            Load(FbThemeBranches.Dark);
        }

        public static IReadOnlyList<string> Branches { get; } = [FbThemeBranches.Dark, FbThemeBranches.Light];

        public int Version { get; private set; }

        public string Branch { get; private set; } = FbThemeBranches.Dark;

        public string SelectedVariant { get; set; } = FbThemeVariants.Default;

        public IReadOnlyList<string> VariantNames => _original.VariantOrder;

        // -------- Mixes, in percent
        public double SurfaceMix { get => _surfaceMix; set => Set(ref _surfaceMix, value); }
        public double AccentOffset { get => _accentOffset; set => Set(ref _accentOffset, value); }
        public double HighlightOffset { get => _highlightOffset; set => Set(ref _highlightOffset, value); }
        public double FrameLight { get => _frameLight; set => Set(ref _frameLight, value); }
        public double FrameMedium { get => _frameMedium; set => Set(ref _frameMedium, value); }
        public double FrameStrong { get => _frameStrong; set => Set(ref _frameStrong, value); }
        public double TextSubtle { get => _textSubtle; set => Set(ref _textSubtle, value); }
        public double TextMuted { get => _textMuted; set => Set(ref _textMuted, value); }
        public double TextDefault { get => _textDefault; set => Set(ref _textDefault, value); }
        public double TextHighlight { get => _textHighlight; set => Set(ref _textHighlight, value); }

        private double _surfaceMix, _accentOffset, _highlightOffset, _frameLight, _frameMedium, _frameStrong,
            _textSubtle, _textMuted, _textDefault, _textHighlight;

        private readonly Dictionary<string, Anchors> _anchors = new(StringComparer.OrdinalIgnoreCase);

        public Anchors Selected => _anchors[SelectedVariant];

        /// <summary>
        /// Theme built from the edited values, inheriting from the registered default theme.
        /// </summary>
        public FbThemeSetup Setup
        {
            get
            {
                if (_setup is null || _setupVersion != Version)
                {
                    _setup = BuildSetup();
                    _setupVersion = Version;
                }
                return _setup;
            }
        }

        /// <summary>
        /// Starts again from the resolved values of <paramref name="branch"/>.
        /// </summary>
        public void Load(string branch)
        {
            Branch = branch;
            _original = _registry.Resolve(_registry.Default.Name, branch);

            _surfaceMix = Percent(_original.SurfaceMix.SurfaceMix);
            _accentOffset = Percent(_original.SurfaceMix.AccentOffset);
            _highlightOffset = Percent(_original.SurfaceMix.HighlightOffset);
            _frameLight = Percent(_original.BordersMix.LightMix);
            _frameMedium = Percent(_original.BordersMix.MediumMix);
            _frameStrong = Percent(_original.BordersMix.StrongMix);
            _textSubtle = Percent(_original.TextVariant.SubtleHighMix);
            _textMuted = Percent(_original.TextVariant.MutedHighMix);
            _textDefault = Percent(_original.TextVariant.DefaultHighMix);
            _textHighlight = Percent(_original.TextVariant.HighlightHighMix);

            _anchors.Clear();
            foreach (var name in _original.VariantOrder)
            {
                var variant = _original.GetVariant(name);
                _anchors[name] = new Anchors(this,
                    variant.LayoutColors.LowAnchor!, variant.LayoutColors.HighAnchor!,
                    variant.FormColors.LowAnchor!, variant.FormColors.HighAnchor!);
            }

            Version++;
        }

        /// <summary>
        /// C# of a theme that declares only what differs from the default theme.
        /// </summary>
        public string ToCSharp()
        {
            var branch = new StringBuilder();
            AppendGroup(branch, "SurfaceMix", "FbThemeLayoutSurfaceMix",
                ("SurfaceMix", _surfaceMix, _original.SurfaceMix.SurfaceMix),
                ("AccentOffset", _accentOffset, _original.SurfaceMix.AccentOffset),
                ("HighlightOffset", _highlightOffset, _original.SurfaceMix.HighlightOffset));
            AppendGroup(branch, "BordersMix", "FbThemeLayoutBordersMix",
                ("LightMix", _frameLight, _original.BordersMix.LightMix),
                ("MediumMix", _frameMedium, _original.BordersMix.MediumMix),
                ("StrongMix", _frameStrong, _original.BordersMix.StrongMix));
            AppendGroup(branch, "TextVariantMix", "FbThemeFormTextVariantMix",
                ("SubtleHighMix", _textSubtle, _original.TextVariant.SubtleHighMix),
                ("MutedHighMix", _textMuted, _original.TextVariant.MutedHighMix),
                ("DefaultHighMix", _textDefault, _original.TextVariant.DefaultHighMix),
                ("HighlightHighMix", _textHighlight, _original.TextVariant.HighlightHighMix));

            var variants = _original.VariantOrder.Where(name => _anchors[name].Changed).ToList();
            if (variants.Count > 0)
            {
                branch.AppendLine("            Variants =");
                branch.AppendLine("            [");
                for (var i = 0; i < variants.Count; i++)
                {
                    var a = _anchors[variants[i]];
                    branch.AppendLine($"                new FbThemeColorVariant(\"{variants[i]}\")");
                    branch.AppendLine("                {");
                    branch.AppendLine($"                    LayoutColors = new FbThemeLayoutColors {{ LowAnchor = \"{a.BgLow}\", HighAnchor = \"{a.BgHigh}\" }},");
                    branch.AppendLine($"                    FormColors = new FbThemeFormColors {{ LowAnchor = \"{a.FgLow}\", HighAnchor = \"{a.FgHigh}\" }}");
                    branch.AppendLine(i < variants.Count - 1 ? "                }," : "                }");
                }
                branch.AppendLine("            ]");
            }

            if (branch.Length == 0)
                return "// Aucune valeur modifiée : le thème par défaut s'applique tel quel.";

            var code = new StringBuilder();
            code.AppendLine("var myTheme = new FbThemeSetup(\"MyTheme\", registry.Default)");
            code.AppendLine("{");
            code.AppendLine("    Branches =");
            code.AppendLine("    [");
            code.AppendLine($"        new FbThemeBranch(FbThemeBranches.{Branch})");
            code.AppendLine("        {");
            code.Append(branch.ToString().TrimEnd().TrimEnd(',')).AppendLine();
            code.AppendLine("        }");
            code.AppendLine("    ]");
            code.Append("};");
            return code.ToString();
        }

        private FbThemeSetup BuildSetup() => new("Edited", _registry.Default)
        {
            Branches =
            [
                new FbThemeBranch(Branch)
                {
                    SurfaceMix = new FbThemeLayoutSurfaceMix
                    {
                        SurfaceMix = Css(_surfaceMix),
                        AccentOffset = Css(_accentOffset),
                        HighlightOffset = Css(_highlightOffset)
                    },
                    BordersMix = new FbThemeLayoutBordersMix
                    {
                        LightMix = Css(_frameLight),
                        MediumMix = Css(_frameMedium),
                        StrongMix = Css(_frameStrong)
                    },
                    TextVariantMix = new FbThemeFormTextVariantMix
                    {
                        SubtleHighMix = Css(_textSubtle),
                        MutedHighMix = Css(_textMuted),
                        DefaultHighMix = Css(_textDefault),
                        HighlightHighMix = Css(_textHighlight)
                    },
                    Variants = _original.VariantOrder.Select(name =>
                    {
                        var a = _anchors[name];
                        return new FbThemeColorVariant(name)
                        {
                            LayoutColors = new FbThemeLayoutColors { LowAnchor = a.BgLow, HighAnchor = a.BgHigh },
                            FormColors = new FbThemeFormColors { LowAnchor = a.FgLow, HighAnchor = a.FgHigh }
                        };
                    }).ToList()
                }
            ]
        };

        private static void AppendGroup(StringBuilder code, string property, string type,
            params (string Name, double Value, string? Original)[] values)
        {
            var changed = values.Where(v => Css(v.Value) != Css(Percent(v.Original))).ToList();
            if (changed.Count == 0)
                return;

            var assignments = string.Join(", ", changed.Select(v => $"{v.Name} = \"{Css(v.Value)}\""));
            code.AppendLine($"            {property} = new {type} {{ {assignments} }},");
        }

        private void Set(ref double field, double value)
        {
            if (field == value)
                return;
            field = value;
            Version++;
        }

        internal void Touch() => Version++;

        private static double Percent(string? value) => FbColorMath.TryParsePercent(value, out var percent) ? percent : 0d;

        public static string Css(double percent) => percent.ToString("0.##", CultureInfo.InvariantCulture) + "%";

        /// <summary>
        /// The four anchors of one variant. Colors are kept in the lowercase form of the color picker.
        /// </summary>
        public sealed class Anchors
        {
            private readonly ThemeEditorModel _model;
            private readonly string _bgLow, _bgHigh, _fgLow, _fgHigh;
            private string _currentBgLow, _currentBgHigh, _currentFgLow, _currentFgHigh;

            internal Anchors(ThemeEditorModel model, string bgLow, string bgHigh, string fgLow, string fgHigh)
            {
                _model = model;
                _currentBgLow = _bgLow = bgLow.ToLowerInvariant();
                _currentBgHigh = _bgHigh = bgHigh.ToLowerInvariant();
                _currentFgLow = _fgLow = fgLow.ToLowerInvariant();
                _currentFgHigh = _fgHigh = fgHigh.ToLowerInvariant();
            }

            public string BgLow { get => _currentBgLow; set => Set(ref _currentBgLow, value); }
            public string BgHigh { get => _currentBgHigh; set => Set(ref _currentBgHigh, value); }
            public string FgLow { get => _currentFgLow; set => Set(ref _currentFgLow, value); }
            public string FgHigh { get => _currentFgHigh; set => Set(ref _currentFgHigh, value); }

            public bool Changed => _currentBgLow != _bgLow || _currentBgHigh != _bgHigh || _currentFgLow != _fgLow || _currentFgHigh != _fgHigh;

            private void Set(ref string field, string value)
            {
                var normalized = value.ToLowerInvariant();
                if (field == normalized)
                    return;
                field = normalized;
                _model.Touch();
            }
        }
    }
}
