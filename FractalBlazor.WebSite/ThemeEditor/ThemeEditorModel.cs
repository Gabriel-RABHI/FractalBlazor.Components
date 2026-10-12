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
    /// Values edited from the default theme: the spacings and corners of the theme, and the
    /// mixes and anchors of one branch. Every change bumps <see cref="Version"/>;
    /// <see cref="Setup"/> is rebuilt only then, so the preview theme is resolved again only
    /// when something was really edited.
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
            ResetShape();
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

        // -------- Spacings and corners, in rem: values of the theme, shared by its branches
        public double SpaceS { get => _spaceS; set => Set(ref _spaceS, value); }
        public double SpaceM { get => _spaceM; set => Set(ref _spaceM, value); }
        public double SpaceL { get => _spaceL; set => Set(ref _spaceL, value); }
        public double SpaceX { get => _spaceX; set => Set(ref _spaceX, value); }
        public double RadiusS { get => _radiusS; set => Set(ref _radiusS, value); }
        public double RadiusM { get => _radiusM; set => Set(ref _radiusM, value); }
        public double RadiusL { get => _radiusL; set => Set(ref _radiusL, value); }
        public double RadiusX { get => _radiusX; set => Set(ref _radiusX, value); }

        private double _spaceS, _spaceM, _spaceL, _spaceX, _radiusS, _radiusM, _radiusL, _radiusX;

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
        /// Starts again from the spacings and corners of the default theme. They belong to the
        /// theme, so <see cref="Load"/> keeps them.
        /// </summary>
        public void ResetShape()
        {
            _spaceS = Rem(_original.Spacings.S);
            _spaceM = Rem(_original.Spacings.M);
            _spaceL = Rem(_original.Spacings.L);
            _spaceX = Rem(_original.Spacings.X);
            _radiusS = Rem(_original.Corners.S);
            _radiusM = Rem(_original.Corners.M);
            _radiusL = Rem(_original.Corners.L);
            _radiusX = Rem(_original.Corners.X);
            Version++;
        }

        /// <summary>
        /// Starts again from the resolved values of <paramref name="branch"/>: its mixes and
        /// anchors. The spacings and corners of the theme are kept.
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
            const string ThemeIndent = "    ", BranchIndent = "            ";

            var theme = new StringBuilder();
            AppendGroup(theme, ThemeIndent, "Spacings", "FbThemeLayoutSpacings",
                LengthValue("S", _spaceS, _original.Spacings.S),
                LengthValue("M", _spaceM, _original.Spacings.M),
                LengthValue("L", _spaceL, _original.Spacings.L),
                LengthValue("X", _spaceX, _original.Spacings.X));
            AppendGroup(theme, ThemeIndent, "Corners", "FbThemeLayoutCorners",
                LengthValue("S", _radiusS, _original.Corners.S),
                LengthValue("M", _radiusM, _original.Corners.M),
                LengthValue("L", _radiusL, _original.Corners.L),
                LengthValue("X", _radiusX, _original.Corners.X));

            var branch = new StringBuilder();
            AppendGroup(branch, BranchIndent, "SurfaceMix", "FbThemeLayoutSurfaceMix",
                MixValue("SurfaceMix", _surfaceMix, _original.SurfaceMix.SurfaceMix),
                MixValue("AccentOffset", _accentOffset, _original.SurfaceMix.AccentOffset),
                MixValue("HighlightOffset", _highlightOffset, _original.SurfaceMix.HighlightOffset));
            AppendGroup(branch, BranchIndent, "BordersMix", "FbThemeLayoutBordersMix",
                MixValue("LightMix", _frameLight, _original.BordersMix.LightMix),
                MixValue("MediumMix", _frameMedium, _original.BordersMix.MediumMix),
                MixValue("StrongMix", _frameStrong, _original.BordersMix.StrongMix));
            AppendGroup(branch, BranchIndent, "TextVariantMix", "FbThemeFormTextVariantMix",
                MixValue("SubtleHighMix", _textSubtle, _original.TextVariant.SubtleHighMix),
                MixValue("MutedHighMix", _textMuted, _original.TextVariant.MutedHighMix),
                MixValue("DefaultHighMix", _textDefault, _original.TextVariant.DefaultHighMix),
                MixValue("HighlightHighMix", _textHighlight, _original.TextVariant.HighlightHighMix));

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

            if (theme.Length == 0 && branch.Length == 0)
                return "// Aucune valeur modifiée : le thème par défaut s'applique tel quel.";

            var body = new StringBuilder(theme.ToString());
            if (branch.Length > 0)
            {
                body.AppendLine("    Branches =");
                body.AppendLine("    [");
                body.AppendLine($"        new FbThemeBranch(FbThemeBranches.{Branch})");
                body.AppendLine("        {");
                body.Append(branch.ToString().TrimEnd().TrimEnd(',')).AppendLine();
                body.AppendLine("        }");
                body.AppendLine("    ]");
            }

            var code = new StringBuilder();
            code.AppendLine("var myTheme = new FbThemeSetup(\"MyTheme\", registry.Default)");
            code.AppendLine("{");
            code.Append(body.ToString().TrimEnd().TrimEnd(',')).AppendLine();
            code.Append("};");
            return code.ToString();
        }

        private FbThemeSetup BuildSetup() => new("Edited", _registry.Default)
        {
            Spacings = new FbThemeLayoutSpacings
            {
                S = CssRem(_spaceS),
                M = CssRem(_spaceM),
                L = CssRem(_spaceL),
                X = CssRem(_spaceX)
            },
            Corners = new FbThemeLayoutCorners
            {
                S = CssRem(_radiusS),
                M = CssRem(_radiusM),
                L = CssRem(_radiusL),
                X = CssRem(_radiusX)
            },
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

        private static (string Name, string Value, string Original) MixValue(string name, double value, string? original)
            => (name, Css(value), Css(Percent(original)));

        private static (string Name, string Value, string Original) LengthValue(string name, double value, string? original)
            => (name, CssRem(value), CssRem(Rem(original)));

        /// <summary>
        /// Appends <paramref name="property"/> with the values that differ from the default theme.
        /// </summary>
        private static void AppendGroup(StringBuilder code, string indent, string property, string type,
            params (string Name, string Value, string Original)[] values)
        {
            var changed = values.Where(v => v.Value != v.Original).ToList();
            if (changed.Count == 0)
                return;

            var assignments = string.Join(", ", changed.Select(v => $"{v.Name} = \"{v.Value}\""));
            code.AppendLine($"{indent}{property} = new {type} {{ {assignments} }},");
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
        /// A length of the theme in rem. A value in px counts 16px per rem; a unitless one is in rem.
        /// </summary>
        private static double Rem(string? value)
        {
            var text = value?.Trim() ?? "";
            var (number, scale) = text.EndsWith("rem", StringComparison.Ordinal) ? (text[..^3], 1d)
                : text.EndsWith("px", StringComparison.Ordinal) ? (text[..^2], 1d / 16)
                : (text, 1d);
            return double.TryParse(number, NumberStyles.Float, CultureInfo.InvariantCulture, out var length) ? length * scale : 0d;
        }

        public static string CssRem(double rem) => rem.ToString("0.####", CultureInfo.InvariantCulture) + "rem";

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
